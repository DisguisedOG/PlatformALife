import os
import re

# Interpretation Mapping: Casual Talk -> Task Format
def parse_casual_input(text):
    # Mapping casual keywords to structured data
    mapping = {
        "audio": "Audio Director",
        "sound": "Audio Director",
        "code": "Lead Programmer",
        "script": "Lead Programmer",
        "art": "GFX Director",
        "visual": "GFX Director"
    }
    
    # Extract Department
    dept = "Project Manager" # Default
    for key, value in mapping.items():
        if key in text.lower():
            dept = value
            
    # Normalize Task Format
    # Logic: Look for action verbs to define title
    task_title = "General Development"
    if "fix" in text.lower(): task_title = "Remediation Task"
    if "add" in text.lower() or "new" in text.lower(): task_title = "Feature Implementation"
    
    return dept, task_title

def process_casual_talk(user_input):
    dept, title = parse_casual_input(user_input)
    task_entry = f"- [ ] [T{os.urandom(2).hex()}]: {title} ({user_input})\n  - Responsibility: {dept}\n  - Status: IN_DEVELOPMENT\n"
    
    # Append to Project Manager file
    with open("Project Manager", "a") as f:
        f.write(task_entry)
    print(f"INTERPRETED: {dept} assigned to: {user_input}")

# Integration: You can call this from your main loop
# process_casual_talk("Hey, can you add some wind sounds for the forest?")