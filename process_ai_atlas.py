from PIL import Image
import math
import os

def process_atlas():
    # Load AI image
    ai_img = Image.open('Assets/AorakiAtlas.jpeg').convert('RGBA')
    
    # Create the target 160x128 transparent atlas (Godot expects 5x4 grid of 32x32 tiles)
    out_img = Image.new('RGBA', (160, 128), (255, 255, 255, 0))
    
    # Helper to extract a cell, remove magenta, and resize to 32x32
    def get_tile(x_start, y_start, x_end, y_end):
        cell = ai_img.crop((x_start, y_start, x_end, y_end))
        cell = cell.resize((32, 32), Image.Resampling.LANCZOS)
        
        # Remove magenta
        datas = cell.getdata()
        new_data = []
        for item in datas:
            r, g, b, a = item
            if r > 180 and g < 100 and b > 180:
                new_data.append((255, 255, 255, 0))
            else:
                new_data.append(item)
        cell.putdata(new_data)
        return cell
        
    # Extract the 2x2 grass tiles from top-left of AI image
    grass_tl = get_tile(0, 0, 253, 247)
    grass_tr = get_tile(253, 0, 460, 247)
    grass_bl = get_tile(0, 247, 253, 454)
    grass_br = get_tile(253, 247, 460, 454)
    
    # Build 3x3 grass required by Godot build_tileset.gd (0,0 to 2,2)
    out_img.paste(grass_tl, (0*32, 0*32))
    out_img.paste(grass_tl, (1*32, 0*32)) # Center top
    out_img.paste(grass_tr, (2*32, 0*32))
    
    out_img.paste(grass_bl, (0*32, 1*32))
    out_img.paste(grass_bl, (1*32, 1*32)) # Center
    out_img.paste(grass_br, (2*32, 1*32))
    
    out_img.paste(grass_bl, (0*32, 2*32))
    out_img.paste(grass_bl, (1*32, 2*32))
    out_img.paste(grass_br, (2*32, 2*32))
    
    # Fill y=3 (Highlands biome)
    out_img.paste(grass_bl, (0*32, 3*32))
    out_img.paste(grass_bl, (1*32, 3*32))
    out_img.paste(grass_br, (2*32, 3*32))
    
    # Extract Volcano tiles to map as "Dirt" in the game
    volc_tl = get_tile(460, 0, 695, 247)
    volc_tr = get_tile(695, 0, 919, 247)
    volc_bl = get_tile(460, 247, 695, 454)
    volc_br = get_tile(695, 247, 919, 454)
    
    # Build dirt (3,0 to 4,3)
    out_img.paste(volc_tl, (3*32, 0*32))
    out_img.paste(volc_tr, (4*32, 0*32))
    
    out_img.paste(volc_bl, (3*32, 1*32))
    out_img.paste(volc_br, (4*32, 1*32))
    
    out_img.paste(volc_bl, (3*32, 2*32))
    out_img.paste(volc_br, (4*32, 2*32))
    
    # Fill y=3 (Highlands biome)
    out_img.paste(volc_bl, (3*32, 3*32))
    out_img.paste(volc_br, (4*32, 3*32))
    
    # Save perfectly formatted engine-ready atlas
    out_img.save('Assets/AorakiAtlas.png', 'PNG')
    print("New atlas compiled successfully and saved to Assets/AorakiAtlas.png")

if __name__ == "__main__":
    process_atlas()
