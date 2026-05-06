@echo off
REM ================================================
REM Lance le script PowerShell de setup git
REM Lilian BENARD - ISART
REM Non-interactif - logge dans setup-git.log
REM ================================================

cd /d "%~dp0"

powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0setup-git.ps1" > "%~dp0setup-git.log" 2>&1

echo DONE > "%~dp0setup-git.done"
