@echo off
echo Synchronizing project files to repository...

:: Add all files in the scripts folder
git add scripts/*.cs

:: Commit changes with a timestamp
git commit -m "Auto-sync: Update core modules - %date% %time%"

:: Push to your remote origin
git push origin main

echo Sync complete.
pause