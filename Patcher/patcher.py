import os
import shutil
import json

def run_patch():
    # Force the script to look in the directory where the script itself is located
    script_dir = os.path.dirname(os.path.abspath(__file__))
    root_dir = os.path.dirname(script_dir) # One level up is your project root
    downloads_dir = r"C:\Users\subri\Downloads"
    manifest_path = os.path.join(script_dir, "manifest.json")
    
    print(f"--- Debug: Looking for manifest at: {manifest_path} ---")
    
    if not os.path.exists(manifest_path):
        print(f"CRITICAL ERROR: Manifest not found at {manifest_path}")
        return

    with open(manifest_path, 'r') as f:
        mapping = json.load(f)

    # Search Downloads for files matching the Manifest
    for filename in os.listdir(downloads_dir):
        for gen_id, target_path in mapping.items():
            if gen_id in filename:
                src = os.path.join(downloads_dir, filename)
                dest = os.path.join(root_dir, target_path)
                
                os.makedirs(os.path.dirname(dest), exist_ok=True)
                shutil.move(src, dest)
                print(f"INTEGRATED: {filename} -> {target_path}")

    print("--- Patch Complete ---")

if __name__ == "__main__":
    run_patch()