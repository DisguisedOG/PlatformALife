#!/usr/bin/env python3
"""Rebuild Assets/AorakiAtlas.png as a flush 5x4 grid of 32x32 terrain tiles.

The source JPEG is a concept sheet: each tile sits inside a magenta cell with
large gutters. The previous packer resized those whole cells (gutters included)
down to 32x32, which baked transparent margins into every atlas tile and left
visible cracks in the TileMap.

This packer:
  1. Crops each tile window from the JPEG
  2. Chroma-keys magenta / pink JPEG fringe
  3. Tight-crops to the opaque bounding box (optional inset)
  4. Nearest-neighbour scales the art to fill a 32x32 cell
  5. Extends edge pixels so terrain tiles are fully opaque and seam-free
"""

from __future__ import annotations

import os
import sys

from PIL import Image

REPO_ROOT = os.path.dirname(os.path.abspath(__file__))
JPEG_PATH = os.path.join(REPO_ROOT, "Assets", "AorakiAtlas.jpeg")
PNG_PATH = os.path.join(REPO_ROOT, "Assets", "AorakiAtlas.png")

TILE = 32
ATLAS_COLS = 5
ATLAS_ROWS = 4

# Inclusive-exclusive crop windows on the 2752x1536 concept sheet.
# These sit on the actual pixel-art cells, not the surrounding magenta gutters.
GRASS_TL = (56, 62, 253, 247)
GRASS_TR = (262, 62, 459, 247)
ROCK_L = (504, 263, 694, 453)
ROCK_C = (704, 261, 918, 453)


def is_chroma(r: int, g: int, b: int) -> bool:
    """True for magenta/pink matte and JPEG ringing around it."""
    if r > 160 and b > 150 and g < min(r, b) * 0.75:
        return True
    if r > 140 and b > 130 and g < 90 and (r - g) > 40:
        return True
    # Darkened magenta leftover from JPEG compression (purple-black seams).
    if r >= 40 and b >= 40 and g < 110:
        rb = (r + b) / 2.0
        if g <= rb * 0.65 and abs(r - b) <= max(40.0, rb * 0.35):
            return True
    return False


def key_magenta(img: Image.Image) -> Image.Image:
    src = img.convert("RGBA")
    keyed = Image.new("RGBA", src.size)
    keyed.putdata(
        [
            (0, 0, 0, 0) if is_chroma(r, g, b) else (r, g, b, a)
            for (r, g, b, a) in src.getdata()
        ]
    )
    return keyed


def opaque_bbox(img: Image.Image, alpha_min: int = 24) -> tuple[int, int, int, int] | None:
    pixels = img.load()
    w, h = img.size
    minx, miny, maxx, maxy = w, h, -1, -1
    for y in range(h):
        for x in range(w):
            if pixels[x, y][3] >= alpha_min:
                minx = min(minx, x)
                maxx = max(maxx, x)
                miny = min(miny, y)
                maxy = max(maxy, y)
    if maxx < 0:
        return None
    return minx, miny, maxx + 1, maxy + 1


def inset_bbox(box: tuple[int, int, int, int], inset: int, size: tuple[int, int]) -> tuple[int, int, int, int]:
    x0, y0, x1, y1 = box
    w, h = size
    x0 = min(x0 + inset, x1 - 1)
    y0 = min(y0 + inset, y1 - 1)
    x1 = max(x1 - inset, x0 + 1)
    y1 = max(y1 - inset, y0 + 1)
    return max(0, x0), max(0, y0), min(w, x1), min(h, y1)


def extract_window(sheet: Image.Image, box: tuple[int, int, int, int], inset: int = 3) -> Image.Image:
    x0, y0, x1, y1 = box
    cell = key_magenta(sheet.crop((x0, y0, x1, y1)))
    bbox = opaque_bbox(cell)
    if bbox is None:
        raise RuntimeError(f"No opaque pixels in window {box}")
    bbox = inset_bbox(bbox, inset, cell.size)
    return cell.crop(bbox)


def dirt_only(tile: Image.Image) -> Image.Image:
    """Crop the mid-dirt band, skipping grass caps and the cell's dark bottom rim."""
    pixels = tile.load()
    w, h = tile.size
    grass_end = 0
    brown_run = 0
    for y in range(h):
        green = brown = 0
        for x in range(w):
            r, g, b, a = pixels[x, y]
            if a < 24:
                continue
            if g > r and g >= b:
                green += 1
            else:
                brown += 1
        if brown > green and brown > w * 0.25:
            brown_run += 1
            if brown_run >= 3:
                grass_end = y - 2
                break
        else:
            brown_run = 0
    y0 = min(h - 16, grass_end + max(8, h // 8))
    y1 = max(y0 + 16, int(h * 0.72))
    cropped = tile.crop((0, y0, w, y1))
    bbox = opaque_bbox(cropped)
    if bbox is None:
        return tile
    return cropped.crop(bbox)


def fill_opaque(tile: Image.Image) -> Image.Image:
    """Extend nearest opaque pixels so terrain tiles have no transparent gutters."""
    img = tile.copy()
    pixels = img.load()
    w, h = img.size

    def write_span(axis_major_is_x: bool) -> None:
        major = w if axis_major_is_x else h
        minor = h if axis_major_is_x else w
        for j in range(minor):
            last = None
            for i in range(major):
                x, y = (i, j) if axis_major_is_x else (j, i)
                r, g, b, a = pixels[x, y]
                if a >= 128:
                    last = (r, g, b, 255)
                    pixels[x, y] = last
                elif last is not None:
                    pixels[x, y] = last
            last = None
            for i in range(major - 1, -1, -1):
                x, y = (i, j) if axis_major_is_x else (j, i)
                r, g, b, a = pixels[x, y]
                if a >= 128:
                    last = (r, g, b, 255)
                    pixels[x, y] = last
                elif last is not None:
                    pixels[x, y] = last

    write_span(True)
    write_span(False)
    return img


def strip_cell_outline(tile: Image.Image, preserve_top: bool = False) -> Image.Image:
    """Replace the outer 1px (concept-sheet cell borders) with interior pixels."""
    img = tile.copy()
    pixels = img.load()
    w, h = img.size
    for y in range(h):
        pixels[0, y] = pixels[1, y]
        pixels[w - 1, y] = pixels[w - 2, y]
    for x in range(w):
        if not preserve_top:
            pixels[x, 0] = pixels[x, 1]
        pixels[x, h - 1] = pixels[x, h - 2]
    return img


def to_cell(tile: Image.Image, preserve_top: bool = False) -> Image.Image:
    filled = fill_opaque(tile.resize((TILE, TILE), Image.Resampling.NEAREST))
    return strip_cell_outline(filled, preserve_top=preserve_top)


def process_atlas() -> Image.Image:
    if not os.path.exists(JPEG_PATH):
        raise FileNotFoundError(f"Missing source sheet: {JPEG_PATH}")

    sheet = Image.open(JPEG_PATH).convert("RGBA")
    grass_tl = extract_window(sheet, GRASS_TL)
    grass_tr = extract_window(sheet, GRASS_TR)
    rock_l = extract_window(sheet, ROCK_L)
    rock_c = extract_window(sheet, ROCK_C)

    surface = [
        to_cell(grass_tl, preserve_top=True),
        to_cell(grass_tl, preserve_top=True),
        to_cell(grass_tr, preserve_top=True),
        to_cell(rock_l, preserve_top=True),
        to_cell(rock_c, preserve_top=True),
    ]
    rock_l_fill = dirt_only(rock_l)
    rock_c_fill = dirt_only(rock_c)
    fill = [
        to_cell(dirt_only(grass_tl)),
        to_cell(dirt_only(grass_tl)),
        to_cell(dirt_only(grass_tr)),
        to_cell(rock_l_fill if rock_l_fill.size[1] > 8 else rock_l),
        to_cell(rock_c_fill if rock_c_fill.size[1] > 8 else rock_c),
    ]

    atlas = Image.new("RGBA", (ATLAS_COLS * TILE, ATLAS_ROWS * TILE), (0, 0, 0, 0))
    for x, tile in enumerate(surface):
        atlas.paste(tile, (x * TILE, 0))
    for y in range(1, ATLAS_ROWS):
        for x, tile in enumerate(fill):
            atlas.paste(tile, (x * TILE, y * TILE))
    return atlas


def edge_report(atlas: Image.Image) -> list[str]:
    lines = []
    pixels = atlas.load()
    for ty in range(ATLAS_ROWS):
        for tx in range(ATLAS_COLS):
            opaque = left = right = top = bottom = 0
            for y in range(TILE):
                for x in range(TILE):
                    a = pixels[tx * TILE + x, ty * TILE + y][3]
                    if a < 250:
                        continue
                    opaque += 1
                    if x == 0:
                        left += 1
                    if x == TILE - 1:
                        right += 1
                    if y == 0:
                        top += 1
                    if y == TILE - 1:
                        bottom += 1
            lines.append(
                f"tile({tx},{ty}) opaque={opaque}/1024 edges LTRB="
                f"({left},{top},{right},{bottom})"
            )
            if left < TILE or right < TILE or top < TILE or bottom < TILE:
                raise RuntimeError(
                    f"Tile ({tx},{ty}) still has transparent margins: "
                    f"L={left} T={top} R={right} B={bottom}"
                )
    return lines


def main() -> int:
    atlas = process_atlas()
    report = edge_report(atlas)
    os.makedirs(os.path.dirname(PNG_PATH), exist_ok=True)
    atlas.save(PNG_PATH, "PNG")
    print(f"Wrote {PNG_PATH} ({atlas.size[0]}x{atlas.size[1]})")
    for line in report:
        print(" ", line)
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:  # pragma: no cover - CLI guard
        print(f"ERROR: {exc}", file=sys.stderr)
        raise
