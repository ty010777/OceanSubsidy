@echo off
REM OceanSubsidy Log Backup Script

set DividingLine=------------------------------------------------------------------------------------

REM Set backup target path
set BackupBasePath=D:\Website\backup

REM Set source paths
set PathWinevtLogs=C:\Windows\System32\winevt\Logs
set PathApWebLogs=C:\inetpub\logs\LogFiles

REM Set backup target subdirectories
set WinEventLogsBackup=%BackupBasePath%\WinEventLogs
set IISLogsBackup=%BackupBasePath%\IISLogs

REM Create backup directories
if not exist %BackupBasePath% mkdir %BackupBasePath%
if not exist %WinEventLogsBackup% mkdir %WinEventLogsBackup%
if not exist %IISLogsBackup% mkdir %IISLogsBackup%

REM Clear existing backup files
del /q /s %WinEventLogsBackup%\*.* 2>nul
del /q /s %IISLogsBackup%\*.* 2>nul

REM Output backup process start
Echo %DividingLine% > %BackupBasePath%\backup_log.txt
Echo %Date% %Time% >> %BackupBasePath%\backup_log.txt

REM Task 1: Backup Windows Event Logs
Echo Generate exclude list for files older than 30 days >> %BackupBasePath%\backup_log.txt
FORFILES /P %PathWinevtLogs% /M "*" /S /D -30 /C "cmd /c dir @path /b/s" > %BackupBasePath%\OverNDaysFilesList1.txt 2>nul
Echo xcopy Windows Event Logs excluding files older than 30 days >> %BackupBasePath%\backup_log.txt
xcopy %PathWinevtLogs%\*.* %WinEventLogsBackup%\ /s /y /EXCLUDE:%BackupBasePath%\OverNDaysFilesList1.txt >> %BackupBasePath%\backup_log.txt

REM Task 2: Backup IIS Web Logs
Echo Generate exclude list for IIS logs older than 30 days >> %BackupBasePath%\backup_log.txt
FORFILES /P %PathApWebLogs% /M "*" /S /D -30 /C "cmd /c dir @path /b/s" > %BackupBasePath%\OverNDaysFilesList2.txt 2>nul
Echo xcopy IIS Web Logs excluding files older than 30 days >> %BackupBasePath%\backup_log.txt
xcopy %PathApWebLogs%\*.* %IISLogsBackup%\ /s /y /EXCLUDE:%BackupBasePath%\OverNDaysFilesList2.txt >> %BackupBasePath%\backup_log.txt

REM Clean up temporary files
del %BackupBasePath%\OverNDaysFilesList*.txt 2>nul

Echo Log backup completed >> %BackupBasePath%\backup_log.txt
Echo %DividingLine% >> %BackupBasePath%\backup_log.txt

REM Compress backup files
Echo Starting compression of backup files
set BackupDate=%date:~0,4%%date:~5,2%%date:~8,2%
tar -czvf D:\Backup\LogBackup_%BackupDate=%.zip D:\website\backup\.
Echo Compression completed

REM Clean up Code directory - delete files older than 30 days
Echo Cleaning up Code directory - removing files older than 30 days >> %BackupBasePath%\backup_log.txt
if exist D:\Website\backup\Code (
    FORFILES /P D:\Website\backup\Code /M "*" /S /D -30 /C "cmd /c del @path" 2>nul
    FORFILES /P D:\Website\backup\Code /M "*" /S /D -30 /C "cmd /c if @isdir==TRUE rd /s /q @path" 2>nul
    Echo Code directory old files cleaned (30+ days) >> %BackupBasePath%\backup_log.txt
) else (
    Echo Code directory does not exist, skip cleaning >> %BackupBasePath%\backup_log.txt
)