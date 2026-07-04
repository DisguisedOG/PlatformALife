import requests
import os
import shutil

# Configuration
REPO = "your_username/PlatformALife"
API_URL = f"https://api.github.com/repos/{REPO}/contents/scripts"
HEADERS = {"Authorization": "token YOUR_GITHUB_PAT"}

def sync_from_github():
    print("--- Initiating GitHub Sync Protocol ---")
    response = requests.get(API_URL, headers=HEADERS)
    files = response.json()
    
    for file in files:
        if file['name'].endswith('.cs'):
            # Download file content
            content = requests.get(file['download_url']).content
            # Save directly to production path
            dest_path = os.path.join("scripts", file['name'])
            with open(dest_path, 'wb') as f:
                f.write(content)
            print(f"SYNCED: {file['name']} -> {dest_path}")
    print("--- Sync Complete ---")

if __name__ == "__main__":
    sync_from_github()