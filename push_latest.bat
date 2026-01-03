@echo off
REM Push all changes to the main branch

git add .
for /f "tokens=1-2 delims==." %%I in ('wmic os get localdatetime /value') do set datetime=%%J
set datestamp=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%
set timestamp=%datetime:~8,2%:%datetime:~10,2%:%datetime:~12,2%
git commit -m "Update project - %datestamp% %timestamp%"
git push origin main

echo All changes pushed to GitHub.
pause
