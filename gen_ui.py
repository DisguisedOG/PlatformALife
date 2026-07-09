from PIL import Image, ImageDraw

def create_nine_patch():
    # Create a 48x48 image
    img = Image.new('RGBA', (48, 48), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Colors
    gold = (212, 175, 55, 255) # Metallic gold
    dark_grey = (40, 40, 40, 255)
    black = (10, 10, 10, 255)
    
    # Outer gold border (4 pixels)
    draw.rectangle([0, 0, 47, 47], fill=gold)
    
    # Inner dark grey rim (4 pixels)
    draw.rectangle([4, 4, 43, 43], fill=dark_grey)
    
    # Deep black center (fill)
    draw.rectangle([8, 8, 39, 39], fill=black)
    
    img.save('Assets/MAINUI/UI_Panel_Gold.png')
    
    # Now create a horizontal progress bar fill (e.g. for HP/MP/EXP)
    bar = Image.new('RGBA', (32, 16), (0, 0, 0, 0))
    bar_draw = ImageDraw.Draw(bar)
    
    # Red for HP
    bar_draw.rectangle([0, 0, 31, 15], fill=(200, 30, 30, 255))
    bar_draw.rectangle([0, 0, 31, 3], fill=(255, 100, 100, 255)) # highlight
    bar.save('Assets/MAINUI/UI_Bar_HP.png')
    
    # Blue for MP
    bar_draw.rectangle([0, 0, 31, 15], fill=(30, 80, 200, 255))
    bar_draw.rectangle([0, 0, 31, 3], fill=(100, 150, 255, 255)) # highlight
    bar.save('Assets/MAINUI/UI_Bar_MP.png')
    
    # Yellow for EXP
    bar_draw.rectangle([0, 0, 31, 15], fill=(200, 180, 30, 255))
    bar_draw.rectangle([0, 0, 31, 3], fill=(255, 255, 100, 255)) # highlight
    bar.save('Assets/MAINUI/UI_Bar_EXP.png')
    
create_nine_patch()
print("UI Assets generated.")
