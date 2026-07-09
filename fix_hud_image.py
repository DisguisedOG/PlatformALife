from PIL import Image

def chroma_key(image_path, output_path):
    img = Image.open(image_path).convert("RGBA")
    data = img.getdata()
    
    new_data = []
    for item in data:
        r, g, b, a = item
        # Remove bright green backdrop (grid/chroma)
        if g > 150 and r < 100 and b < 100:
            new_data.append((0, 0, 0, 0)) # transparent
        else:
            new_data.append(item)
            
    img.putdata(new_data)
    img.save(output_path, "PNG")
    print("Background removed and saved to", output_path)

chroma_key("Assets/MAINUI/MAIN_UI_RAW.png", "Assets/MAINUI/MAIN_UI_FINAL.png")
