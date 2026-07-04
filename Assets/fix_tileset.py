@echo off
setlocal enabledelayedexpansion

:: Define the source and output
set "INPUT=AorakiTileSet.tres"
set "OUTPUT=AorakiTileSet_Fixed.tres"

echo --- Initiating TileSet Integrity Repair ---

:: Use PowerShell to strip out the known problematic coordinate lines
:: We are specifically targeting lines containing the orphaned atlas references
powershell -Command ^
    "$lines = Get-Content '%INPUT%'; " ^
    "$cleaned = $lines | Where-Object { $_ -notmatch '0:4/' -and $_ -notmatch '0:5/' -and $_ -notmatch '0:6/' -and $_ -notmatch '0:7/' -and $_ -notmatch '1:4/' -and $_ -notmatch '2:4/' }; " ^
    "$cleaned | Set-Content '%OUTPUT%'"

echo --- Cleanup complete: Created %OUTPUT% ---
echo Please replace %INPUT% with %OUTPUT% after verifying the file.
pause