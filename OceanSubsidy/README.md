# README.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 專案概述
OceanSubsidy 是一個海洋相關補助管理系統，採用 ASP.NET Web Forms 架構開發，使用 .NET Framework 4.8。系統包含兩個主要子系統：
- **OFS (Ocean Funding System)** - 海洋資金管理系統
- **OSI (Ocean Survey Information)** - 海洋調查資訊系統

## 專案架構

### 技術棧
- **框架**: ASP.NET Web Forms (.NET Framework 4.8)
- **資料庫**: SQL Server (連線字串在 Web.config)
- **前端**: jQuery, Bootstrap 5.3.3, OpenLayers (地圖功能)
- **主要套件**:
  - EPPlus (Excel處理)
  - iTextSharp (PDF)
  - log4net (日誌)
  - Newtonsoft.Json (JSON處理)
  - DocumentFormat.OpenXml (Office文件處理)
  - Microsoft.IO.RecyclableMemoryStream (記憶體管理)

### 核心目錄結構
```
OceanSubsidy/
├── App_Code/           # 商業邏輯層
│   ├── Entity/        # 實體類別 (資料庫映射)
│   │   ├── OFS/      # OFS系統實體
│   │   ├── OSI/      # OSI系統實體
│   │   └── Sys/      # 系統共用實體
│   ├── Operation/     # 資料操作層 (Helper類別)
│   │   ├── OFS/      # OFS系統操作
│   │   ├── OSI/      # OSI系統操作
│   │   └── System/   # 系統共用操作
│   └── _GS/          # 通用工具類別
├── OFS/              # OFS子系統頁面
│   └── SCI/         # 科技研究相關功能
├── OSI/              # OSI子系統頁面
├── Service/          # Web服務處理程序 (.ashx)
├── PowerShell/       # 排程腳本
└── Map/             # 地圖相關功能
```

### 資料庫連線管理
系統使用 GS.Data.Sql 套件進行資料庫操作，連線字串設定在 Web.config:
```xml
<connectionStrings>
  <add name="DefaultConnection" connectionString="..." />
</connectionStrings>
```
在 Global.asax 中註冊：`Generic.DBConnectionString = Env.S_DefaultConnection;`

### 權限控制架構
1. **登入驗證**: Forms Authentication (Login.aspx)
2. **Session 管理**: SessionHelper 類別管理使用者資訊
3. **權限檢查**: Global.asax 的 Application_AcquireRequestState 事件
4. **角色系統**: 
   - OSI 系統角色 (OSI_Role)
   - OFS 系統角色 (OFS_Role)

## 開發指南

### 建置與執行
```bash
# 使用 Visual Studio 2022
# 1. 開啟 OceanSubsidy.sln
# 2. 還原 NuGet 套件 (自動進行)
# 3. F5 執行偵錯模式 或 Ctrl+F5 執行 (不偵錯)
# 4. 預設開啟 Login.aspx (http://localhost:50929)

# 命令列建置 (在專案根目錄執行)
msbuild OceanSubsidy.sln /p:Configuration=Debug
# 或
dotnet build OceanSubsidy.sln
```

### 開發環境設定
1. **IIS Express**: 預設使用 port 50929
2. **偵錯模式**: Web.config 設定 `<compilation debug="true">`
3. **開發帳號自動登入**: 
   - 複製 `dev.config.example` 為 `dev.config`
   - 修改 `dev.config` 中的帳號資訊為您的開發帳號
   - `dev.config` 已加入 .gitignore，不會被提交到版本控制
   - 設定檔範例：
     ```xml
     <add key="DevAutoLogin.Enabled" value="true"/>
     <add key="DevAutoLogin.Account" value="your-email@example.com"/>
     <add key="DevAutoLogin.UserID" value="1"/>
     ```

### 資料庫操作模式
使用 Helper 模式進行資料庫操作：
```csharp
// 查詢範例
DataTable dt = SysUserHelper.QueryUserInfoByID(userId);

// 使用 GS.Data.Sql.Generic 類別
using (var g = new Generic())
{
    g.AddParameter("@UserID", userId);
    DataTable result = g.GetTable("SELECT * FROM Sys_User WHERE UserID = @UserID");
}
```

### 頁面開發模式
1. **Master Page 架構**:
   - LoginMaster.master - 登入頁面
   - BaseMaster.master - 基礎版面
   - OFSMaster.master - OFS子系統
   - OSIMaster.master - OSI子系統

2. **頁面權限設定**:
   - 在 Sys_Permission 資料表中設定頁面權限代碼
   - Global.asax 自動檢查權限

### 排程作業
PowerShell 腳本位於 `/PowerShell/` 目錄：
- `OSIReminderEmail.ps1` - 發送提醒郵件
- `OSIStartFilledReminderEmail.ps1` - 發送開始填報提醒郵件
- `OSIDataPeriodCreate.ps1` - 建立資料期間
- `OSIActivityReportCopy.ps1` - 複製活動報告資料
- `GetOSINSTCData.ps1` - 抓取國科會資料
- `GetOSIPccAward.ps1` - 抓取公共工程委員會資料
- `UserActive.ps1` - 使用者活躍度統計

透過 Windows 工作排程器執行這些腳本。

### 郵件發送
使用 EzMailSender 類別發送郵件：
```csharp
EzMailSender.SendMail(
    mailTo: "recipient@example.com",
    subject: "主旨",
    body: htmlContent,
    isBodyHtml: true
);
```
郵件範本位於 `/App_Code/_GS/App/MailContents/`

### 地圖功能
使用 OpenLayers 實作，相關檔案：
- `/script/map/` - 地圖核心函式庫
- `/script/map_custom/` - 客製化地圖功能
- `/Map/` - 地圖相關頁面

### 日誌記錄
使用 log4net，設定在 Web.config：
```xml
<log4net>
  <appender name="RollingFile" type="log4net.Appender.RollingFileAppender">
    <file value="${LogPath}/OceanSubsidy.log"/>
  </appender>
</log4net>
```
日誌路徑: `../Logs/OceanSubsidy/`

## 資料表命名規則
- **Sys_** 開頭：系統共用資料表
- **OFS_** 開頭：OFS子系統資料表
- **OSI_** 開頭：OSI子系統資料表
- **OFS_SCI_** 開頭：OFS科技研究相關資料表

## 檔案命名規則
- **Helper 類別**: `[功能名稱]Helper.cs`
- **實體類別**: 與資料表同名
- **服務處理程序**: `[功能名稱].ashx`
- **使用者控制項**: `.ascx` 檔案

## 重要設定檔
- `Web.config` - 主要設定檔 (資料庫連線、log4net、郵件設定)
- `packages.config` - NuGet 套件清單
- `Global.asax` - 應用程式生命週期事件 (資料庫連線註冊、權限檢查)
- `dev.config` - 開發環境自動登入設定 (不納入版控)

## 除錯與診斷
### 日誌系統
- **log4net 設定**: 自動記錄到 `../Logs/OceanSubsidy/OceanSubsidy.log`
- **日誌等級**: DEBUG (開發模式)
- **檔案滾動**: 依日期滾動，保留 30 天，單檔最大 400KB

### 除錯工具
- **Visual Studio 偵錯器**: F5 啟動偵錯模式
- **瀏覽器開發者工具**: 檢查前端 JavaScript 和 AJAX 呼叫
- **SQL Server Profiler**: 追蹤資料庫查詢效能
- **IIS Express 日誌**: 檢查 HTTP 請求和回應

### 開發環境自動登入
僅在 `debug="true"` 時啟用，透過 `dev.config` 設定測試帳號

## 常用開發操作

### 新增資料表映射
1. **建立實體類別**: 在 `App_Code/Entity/` 對應子系統目錄下建立與資料表同名的類別
2. **建立Helper類別**: 在 `App_Code/Operation/` 對應目錄下建立 `[資料表名稱]Helper.cs`
3. **實作CRUD方法**: 使用 `Generic` 類別進行資料庫操作

### 新增頁面功能
1. **建立頁面檔案**: 在對應子系統目錄下建立 `.aspx` 和 `.aspx.cs` 檔案
2. **設定Master Page**: 選擇適當的 Master Page (`OFSMaster.master` 或 `OSIMaster.master`)
3. **權限設定**: 在 `Sys_Permission` 資料表新增頁面權限記錄
4. **測試權限**: 確認 Global.asax 的權限檢查機制正常運作

### Web服務開發
建立 `.ashx` 處理程序於 `/Service/` 目錄，處理 AJAX 請求和檔案上傳下載。

## 部署注意事項
1. 修改 Web.config 的 `IsOnline` 設定為 `true`
2. 關閉偵錯模式 `<compilation debug="false">`
3. 更新連線字串為正式環境
4. 設定 IIS 應用程式集區為 .NET Framework 4.8
5. 確認排程腳本的執行權限