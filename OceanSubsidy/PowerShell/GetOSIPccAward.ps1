
# 設定日誌路徑和檔案管理
$scriptName = "GetOSIPccAward"
$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$logDir = Join-Path -Path $scriptDir -ChildPath "..\..\Logs\$scriptName"
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$logPath = Join-Path -Path $logDir -ChildPath "${scriptName}_${timestamp}.log"

# 建立日誌目錄
if (-not (Test-Path -LiteralPath $logDir)) {
    New-Item -Path $logDir -ItemType Directory -Force
}

# 檔案滾動管理：保留最多30份日誌
$logFiles = Get-ChildItem -Path $logDir -Filter "${scriptName}_*.log" | Sort-Object CreationTime -Descending
if ($logFiles.Count -ge 30) {
    $filesToDelete = $logFiles | Select-Object -Skip 29
    foreach ($file in $filesToDelete) {
        Remove-Item -Path $file.FullName -Force
    }
}

try {
    $url = "http://localhost/OceanSubsidy/service/OSI_GetPccAwardData.ashx?isDo=y"
    $response = Invoke-RestMethod -Uri $url -Method Get -UseBasicParsing -TimeoutSec 300
    $responseText = $response | ConvertTo-Json -Compress -Depth 3
    $log = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') SUCCESS: $responseText"
    $log | Out-File -FilePath $logPath -Encoding utf8
    exit 0
}
catch {
    $err = $_.Exception.Message
    $log = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') ERROR: $err"
    $log | Out-File -FilePath $logPath -Encoding utf8
    exit 1
}