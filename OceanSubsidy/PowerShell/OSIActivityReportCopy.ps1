
$logPath = "D:\Project\OceanSubsidy\Logs\OSIActivityReportCopy.log"
$logDir  = Split-Path -Path $logPath -Parent

if (-not (Test-Path -LiteralPath $logDir)) {
    New-Item -Path $logDir -ItemType Directory -Force
}

try {
    $url = "http://localhost/OceanSubsidy/service/OSIActivityReportCopy.ashx"
    $response = Invoke-RestMethod -Uri $url -Method Get -UseBasicParsing -TimeoutSec 300
    $responseText = $response | ConvertTo-Json -Compress -Depth 3
    $log = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') SUCCESS: $responseText"
    $log | Out-File -FilePath $logPath -Encoding utf8 -Append
    exit 0
}
catch {
    $err = $_.Exception.Message
    $log = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') ERROR: $err"
    $log | Out-File -FilePath $logPath -Encoding utf8 -Append
    exit 1
}
