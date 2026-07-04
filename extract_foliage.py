import os
from PIL import Image
import numpy as np
import cv2

src_path = r'd:\KiwiKing Studios\PlatformALife\Assets\Tilesets\NZ_tileset2.png'
out_dir = r'd:\KiwiKing Studios\PlatformALife\Assets\Sprites\Foliage'
os.makedirs(out_dir, exist_ok=True)

img = Image.open(src_path).convert('RGBA')
arr = np.array(img)

# Find alpha channel
alpha = arr[:,:,3]
# Threshold to binary (solid vs transparent)
_, thresh = cv2.threshold(alpha, 127, 255, cv2.THRESH_BINARY)

# Find contours
contours, _ = cv2.findContours(thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

count = 1
for cnt in contours:
    x, y, w, h = cv2.boundingRect(cnt)
    # Trees and bushes are usually large. Let's say > 32x32 and not extremely wide.
    if w > 32 and h > 32 and w < 256 and h < 256:
        # Also let's avoid standard square blocks (tiles), foliage usually has irregular aspect ratios or is very tall.
        if (h > w * 1.2) or (w > 64 and h > 64): 
            box = (x, y, x + w, y + h)
            crop = img.crop(box)
            crop.save(os.path.join(out_dir, f'Foliage_{count}.png'))
            print(f"Extracted Foliage_{count}.png: {w}x{h} at ({x}, {y})")
            count += 1
            if count > 5: # Just grab 5 distinct ones
                break

print("Extraction complete.")
