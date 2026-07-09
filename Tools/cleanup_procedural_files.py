import os
import glob

print("Cleaning up old procedural generation files...")

# Remove old chunk scenes
chunk_dir = "Scenes/Chunks/"
if os.path.exists(chunk_dir):
    for f in glob.glob(os.path.join(chunk_dir, "*.tscn")):
        print(f"Removing old chunk: {f}")
        os.remove(f)

# Remove old unused script
if os.path.exists("Scripts/ChunkInstantiator.cs"):
    print("Removing ChunkInstantiator.cs")
    os.remove("Scripts/ChunkInstantiator.cs")
    
if os.path.exists("update_worldgen.py"):
    print("Removing old worldgen python script")
    os.remove("update_worldgen.py")

print("Cleanup complete!")
