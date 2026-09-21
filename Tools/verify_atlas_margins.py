#!/usr/bin/env python3
"""Fail if AorakiAtlas.png terrain tiles still have transparent gutters."""

from __future__ import annotations

import os
import sys

from PIL import Image

REPO_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
PNG_PATH = os.path.join(REPO_ROOT, "Assets", "AorakiAtlas.png")
TILE = 32


def is_magenta(r: int, g: int, b: int, a: int) -> bool:
    if a < 250:
        return False
    if r < 40 or b < 40 or g >= 110:
        return False
    return abs(r - b) <= 40 and g <= ((r + b) / 2) * 0.65


def main() -> int:
    atlas = Image.open(PNG_PATH).convert("RGBA")
    w, h = atlas.size
    if w % TILE or h % TILE:
        print(f"ERROR: atlas {w}x{h} is not a multiple of {TILE}", file=sys.stderr)
        return 1

    pixels = atlas.load()
    failures = []
    for ty in range(h // TILE):
        for tx in range(w // TILE):
            left = right = top = bottom = opaque = 0
            minx, miny, maxx, maxy = TILE, TILE, -1, -1
            magenta_at = None
            for y in range(TILE):
                for x in range(TILE):
                    r, g, b, a = pixels[tx * TILE + x, ty * TILE + y]
                    if is_magenta(r, g, b, a) and magenta_at is None:
                        magenta_at = (x, y, r, g, b)
                    if a < 250:
                        continue
                    opaque += 1
                    minx = min(minx, x)
                    maxx = max(maxx, x)
                    miny = min(miny, y)
                    maxy = max(maxy, y)
                    if x == 0:
                        left += 1
                    if x == TILE - 1:
                        right += 1
                    if y == 0:
                        top += 1
                    if y == TILE - 1:
                        bottom += 1
            bbox = (minx, miny, maxx, maxy)
            ok = left == TILE and right == TILE and top == TILE and bottom == TILE
            status = "ok" if ok else "MARGIN"
            print(
                f"{status} tile({tx},{ty}) bbox={bbox} opaque={opaque}/1024 "
                f"edges LTRB=({left},{top},{right},{bottom})"
            )
            if not ok:
                failures.append((tx, ty, "margin"))
            if magenta_at is not None:
                x, y, r, g, b = magenta_at
                print(f"MAGENTA tile({tx},{ty}) at local ({x},{y}) rgb=({r},{g},{b})")
                failures.append((tx, ty, "magenta"))

    if failures:
        print(f"ERROR: {len(failures)} tile check(s) failed", file=sys.stderr)
        return 1
    print("All atlas tiles fill their 32x32 cells with opaque edges.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
