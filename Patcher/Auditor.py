import os
import hashlib
import json

def calculate_sha256(filepath):
    sha256_hash = hashlib.sha256()
    with open(filepath, "rb") as f:
        for byte_block in iter(lambda: f.read(4096), b""):
            sha256_hash.update(byte_block)
    return sha256_hash.hexdigest()

def run_integrity_audit():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    root_dir = os.path.dirname(script_dir)
    audit_manifest_path = os.path.join(script_dir, "audit_manifest.json")
    
    with open(audit_manifest_path, 'r') as f:
        master_manifest = json.load(f)

    print(f"--- Initiating Full File System Integrity Audit ---")
    
    # Check for unexpected files or missing files
    for root, dirs, files in os.walk(root_dir):
        # Ignore the patcher folder itself during scan
        if "Patcher" in root: continue 
        
        for file in files:
            full_path = os.path.join(root, file)
            rel_path = os.path.relpath(full_path, root_dir)
            
            if rel_path not in master_manifest:
                print(f"[UNTRACKED]: {rel_path} - Flagged for documentation.")
            else:
                actual_hash = calculate_sha256(full_path)
                if actual_hash != master_manifest[rel_path]:
                    print(f"[CORRUPTED]: {rel_path} - Hash Mismatch!")
    
    print("--- Audit Complete ---")

if __name__ == "__main__":
    run_integrity_audit()