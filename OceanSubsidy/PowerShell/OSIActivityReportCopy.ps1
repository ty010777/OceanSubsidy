
# 讀取 Web.config 中的主機設定
function Get-PowerShellHost {
    try {
        $webConfigPath = "$PSScriptRoot\..\Web.config"
        
        if (Test-Path $webConfigPath) {
            [xml]$webConfig = Get-Content $webConfigPath -Encoding UTF8
            $hostSetting = $webConfig.configuration.appSettings.add | Where-Object { $_.key -eq "Host" }
            if ($hostSetting) {
                return $hostSetting.value
            }
        }
        
        # 如果無法從 Web.config 取得 Host 設定，記錄錯誤並結束
        $errorMsg = "無法從 Web.config 取得 Host 設定"
        $log = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') ERROR: $errorMsg"
        $log | Out-File -FilePath $logPath -Encoding utf8
        Write-Error $errorMsg
        exit 1
    }
    catch {
        # 如果無法讀取 Web.config，記錄錯誤並結束
        $errorMsg = "無法讀取 Web.config 檔案: $($_.Exception.Message)"
        $log = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') ERROR: $errorMsg"
        $log | Out-File -FilePath $logPath -Encoding utf8
        Write-Error $errorMsg
        exit 1
    }
}

# 設定日誌路徑和檔案管理
$scriptName = "OSIActivityReportCopy"
$logDir = Join-Path -Path $PSScriptRoot -ChildPath "..\..\Logs\$scriptName"
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
    $hostUrl = Get-PowerShellHost
    $url = "$hostUrl/OceanSubsidy/service/OSIActivityReportCopy.ashx"
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
