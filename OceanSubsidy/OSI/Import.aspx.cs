using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.Data;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OSI.SpreadsheetReaders;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;

public partial class OSI_Import : System.Web.UI.Page
{
    private SessionHelper.UserInfoClass UserInfo
    {
        get => SessionHelper.Get<SessionHelper.UserInfoClass>(
                  SessionHelper.UserInfo)
                ?? new SessionHelper.UserInfoClass();
        set => SessionHelper.Set(SessionHelper.UserInfo, value);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            mvSteps.ActiveViewIndex = 0;
            ToggleAddButtonAvailability();

            btnUpload.Enabled = false;
            fuExcel.Attributes["accept"] = ".xlsx,.csv,.ods";
            fuExcel.Attributes["onchange"]
              = $"document.getElementById('{btnUpload.ClientID}').disabled = this.files.length === 0;";            
        }
    }

    // 判斷是否可以新增
    private void ToggleAddButtonAvailability()
    {
        GisTable tbl = OSIDataPeriodsHelper.QueryByDateTime(DateTime.Now);

        bool inAnyPeriod = tbl != null && tbl.Rows.Count > 0;
        fuExcel.Enabled = inAnyPeriod;

        if (inAnyPeriod)
        {
            int pid = Convert.ToInt32(tbl.Rows[0]["PeriodID"]);
            hdnPeriodID.Value = pid.ToString();
        }
        else
        {
            fuExcel.CssClass += " disabled";
            lblUploadError.Text = "目前非資料上傳時間";
        }
    }

    // Step1 → 上傳並檢核
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        lblUploadError.Text = "";

        // 1. 檔案必選
        if (!fuExcel.HasFile)
        {
            lblUploadError.Text = "請選擇一個檔案後再上傳";
            return;
        }

        // 2. 限定副檔名
        var ext = Path.GetExtension(fuExcel.FileName).ToLower();
        if (!SpreadsheetReaderFactory.IsSupportedFormat(ext))
        {
            lblUploadError.Text = "只允許上傳 .xlsx、.csv 或 .ods 檔";
            return;
        }

        // 3. 檢核試算表，取得所有筆的結果
        DataTable dtResults = CheckSpreadsheet(fuExcel.PostedFile.InputStream, ext);

        Session["ImportResults"] = dtResults;

        // 4. 綁定 GridView
        gvCheckResults.DataSource = dtResults;
        gvCheckResults.DataBind();

        // 5. 統計錯誤筆數
        int errCount = dtResults.AsEnumerable()
            .Count(r => r.Field<string>("檢核結果") != "通過");

        if (errCount == 0)
        {
            litStep2Message.Text = "<p>資料格式符合，通過檢核</p>";
            btnStep2Next.Enabled = true;
        }
        else
        {
            litStep2Message.Text = $"<p class='text-pink'>您有 {errCount} 筆資料錯誤，請修正後重新上傳</p>";
            btnStep2Next.Enabled = false;
        }

        // 切到 Step2
        mvSteps.ActiveViewIndex = 1;
    }

    protected void gvCheckResults_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // 只處理資料列
        if (e.Row.RowType != DataControlRowType.DataRow) return;

        // 取出 DataItem 裡的「檢核結果」
        string result = DataBinder.Eval(e.Row.DataItem, "檢核結果") as string;
        if (string.IsNullOrEmpty(result) || result == "通過")
        {
            // 通过的不用特別處理
            return;
        }

        // 拆成多行
        var lines = result.Split('；');
        string html = string.Join("<br/>", lines);

        // 清掉原本 cell 內容、改用 LiteralControl 直接輸出 HTML
        var cell = e.Row.Cells[0];
        cell.Controls.Clear();
        cell.Controls.Add(new LiteralControl(html));

        // 套紅色
        cell.Style.Add("color", "red");
    }

    protected void gvCheckResults_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            // 設定每個標題儲存格的寬度和樣式
            e.Row.Cells[0].Width = Unit.Pixel(200);     // 審核結果
            e.Row.Cells[1].Width = Unit.Pixel(200);     // 填報機關
            e.Row.Cells[2].Width = Unit.Pixel(200);     // 活動名稱
            e.Row.Cells[3].Width = Unit.Pixel(200);     // 活動性質
            e.Row.Cells[4].Width = Unit.Pixel(200);     // 活動性質(描述)
            e.Row.Cells[5].Width = Unit.Pixel(200);     // 活動執行者(類別)
            e.Row.Cells[6].Width = Unit.Pixel(200);     // 活動執行者(描述)
            e.Row.Cells[7].Width = Unit.Pixel(200);     // 研究調查日期(起始)
            e.Row.Cells[8].Width = Unit.Pixel(200);     // 研究調查日期(結束)
            e.Row.Cells[9].Width = Unit.Pixel(200);     // 研究調查日期(描述)
            e.Row.Cells[10].Width = Unit.Pixel(200);    // 使用載具名稱(類別)
            e.Row.Cells[11].Width = Unit.Pixel(200);    // 使用載具名稱(描述)
            e.Row.Cells[12].Width = Unit.Pixel(200);    // 使用載具名稱(核准文號)
            e.Row.Cells[13].Width = Unit.Pixel(200);    // 研究調查項目(類別)
            e.Row.Cells[14].Width = Unit.Pixel(200);    // 研究調查項目(描述)
            e.Row.Cells[15].Width = Unit.Pixel(200);    // 研究調查儀器
            e.Row.Cells[16].Width = Unit.Pixel(220);    // 研究調查活動內容概述
            e.Row.Cells[17].Width = Unit.Pixel(200);    // 研究調查範圍(縣市)
            e.Row.Cells[18].Width = Unit.Pixel(200);    // 研究調查範圍(描述)
            
            // 對所有標題儲存格套用自動換行樣式
            foreach (TableCell cell in e.Row.Cells)
            {
                cell.Style.Add("white-space", "normal");
                cell.Style.Add("word-wrap", "break-word");
                cell.Style.Add("word-break", "break-word");
            }
        }
    }

    protected void gvCheckResults_PreRender(object sender, EventArgs e)
    {
        // 將標題列移到 thead
        if (gvCheckResults.Rows.Count > 0 && gvCheckResults.HeaderRow != null)
        {
            gvCheckResults.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    // Step2 錯誤 → 返回 Step1
    protected void btnReupload_Click(object sender, EventArgs e)
    {
        mvSteps.ActiveViewIndex = 0;
    }

    // Step2 通過 → 下一步
    protected void btnToStep3_Click(object sender, EventArgs e)
    {
        // 匯入資料到資料庫
        ImportData();
        mvSteps.ActiveViewIndex = 2;
    }

    // Step3 完成 → 導回 列表 或 首頁
    protected void btnFinish_Click(object sender, EventArgs e)
    {
        // 匯入完可以導回主畫面
        Response.Redirect("~/OSI/ActivityReports.aspx");
    }

    // ===== stub methods =====
    private DataTable CheckSpreadsheet(Stream fileStream, string fileExtension)
    {
        // 根據檔案類型使用對應的讀取器
        var reader = SpreadsheetReaderFactory.GetReader(fileExtension);
        DataTable sourceData = null;
        
        try
        {
            sourceData = reader.ReadToDataTable(fileStream);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"無法讀取檔案: {ex.Message}", ex);
        }
        
        if (sourceData == null || sourceData.Rows.Count < 2)
        {
            throw new InvalidOperationException("檔案沒有資料或資料不足");
        }

        var dt = new DataTable();
        // 先定義要回傳的 DataTable 欄位
        dt.Columns.Add("檢核結果", typeof(string));                 // 檢核結果
        dt.Columns.Add("填報機關", typeof(string));                 // B 填報機關
        dt.Columns.Add("活動名稱", typeof(string));                 // C 活動名稱
        dt.Columns.Add("活動性質", typeof(string));                 // D 活動性質
        dt.Columns.Add("活動性質(描述)", typeof(string));           // E 活動性質(描述)
        dt.Columns.Add("活動執行者(類別)", typeof(string));         // F 活動執行者(類別)
        dt.Columns.Add("活動執行者(描述)", typeof(string));         // G 活動執行者(描述)
        dt.Columns.Add("研究調查日期(起始)", typeof(string));       // H 研究調查日期(起始)
        dt.Columns.Add("研究調查日期(結束)", typeof(string));       // I 研究調查日期(結束)
        dt.Columns.Add("研究調查日期(描述)", typeof(string));       // J 研究調查日期(描述)
        dt.Columns.Add("使用載具名稱(類別)", typeof(string));       // K 使用載具名稱(類別)
        dt.Columns.Add("使用載具名稱(描述)", typeof(string));       // L 使用載具名稱(描述)
        dt.Columns.Add("使用載具名稱(核准文號)", typeof(string));   // M 使用載具名稱(核准文號)
        dt.Columns.Add("研究調查項目(類別)", typeof(string));       // N 研究調查項目(類別)
        dt.Columns.Add("研究調查項目(描述)", typeof(string));       // O 研究調查項目(描述)
        dt.Columns.Add("研究調查儀器", typeof(string));             // P 研究調查儀器
        dt.Columns.Add("研究調查活動內容概述", typeof(string));     // Q 研究調查活動內容概述
        dt.Columns.Add("研究調查範圍(縣市)", typeof(string));       // R 研究調查範圍(縣市)
        dt.Columns.Add("研究調查範圍(描述)", typeof(string));       // S 研究調查範圍(描述)


        // 假設第一列是標題，資料從第二列開始
        for (int row = 1; row < sourceData.Rows.Count; row++)
        {
            var dataRow = sourceData.Rows[row];
            var errors = new System.Collections.Generic.List<string>();

            // 逐欄讀取（欄位索引從 0 開始，對應到 Excel 的 B-S 欄）
            string valB = GetCellValue(dataRow, 1);     // 填報機關
            string valC = GetCellValue(dataRow, 2);     // 活動名稱
            string valD = GetCellValue(dataRow, 3);     // 活動性質
            string valE = GetCellValue(dataRow, 4);     // 活動性質(描述)
            string valF = GetCellValue(dataRow, 5);     // 活動執行者(類別)
            string valG = GetCellValue(dataRow, 6);     // 活動執行者(描述)
            string valH = GetCellValue(dataRow, 7);     // 研究調查日期(起始)
            string valI = GetCellValue(dataRow, 8);     // 研究調查日期(結束)
            string valJ = GetCellValue(dataRow, 9);     // 研究調查日期(描述)
            string valK = GetCellValue(dataRow, 10);    // 使用載具名稱(類別)
            string valL = GetCellValue(dataRow, 11);    // 使用載具名稱(描述)
            string valM = GetCellValue(dataRow, 12);    // 使用載具名稱(核准文號)
            string valN = GetCellValue(dataRow, 13);    // 研究調查項目(類別)
            string valO = GetCellValue(dataRow, 14);    // 研究調查項目(描述)
            string valP = GetCellValue(dataRow, 15);    // 研究調查儀器
            string valQ = GetCellValue(dataRow, 16);    // 研究調查活動內容概述
            string valR = GetCellValue(dataRow, 17);    // 研究調查範圍(縣市)
            string valS = GetCellValue(dataRow, 18);    // 研究調查範圍(描述)

                // 1) 必填
                if (string.IsNullOrWhiteSpace(valB)) errors.Add("填報機關為必填");
                if (string.IsNullOrWhiteSpace(valC)) errors.Add("活動名稱為必填");
                if (string.IsNullOrWhiteSpace(valD)) errors.Add("活動性質為必填");
                if (string.IsNullOrWhiteSpace(valE)) errors.Add("活動性質(描述)為必填");
                if (string.IsNullOrWhiteSpace(valF)) errors.Add("活動執行者(類別)為必填");
                if (string.IsNullOrWhiteSpace(valH)) errors.Add("研究調查日期(起始)為必填");
                if (string.IsNullOrWhiteSpace(valI)) errors.Add("研究調查日期(結束)為必填");

                // 2) H/I 欄解析 ROC 日期
                if (!TryParseRocDate(valH, out DateTime resPerStartDate))
                    errors.Add("研究起始日期(起始)格式錯誤");
                if (!TryParseRocDate(valI, out DateTime resPerEndDate))
                    errors.Add("研究結束日期(結束)格式錯誤");
                if (resPerStartDate != null && resPerEndDate != null && resPerEndDate < resPerStartDate)
                    errors.Add("研究調查日期(結束)不能早於(起始)");

                // 3) B、D、F、K、N、R 欄跟 DB 驗證
                GisTable unitTbl =
                    (UserInfo.OSI_RoleName == "系統管理者") ?
                    SysUnitHelper.QueryAll() :
                    SysUnitHelper.QueryAllChildByID(UserInfo.UnitID.toInt());

                if (!string.IsNullOrWhiteSpace(valB) &&
                    !unitTbl.AsEnumerable().Select(u => u.Field<string>("UnitName")).Contains(valB))
                    errors.Add($"填報機關「{valB}」不存在於可選的項目");
                if (!string.IsNullOrWhiteSpace(valD) && !OSIActivityNaturesHelper.IsExistByNatureName(valD))
                    errors.Add($"活動性質「{valD}」不存在");
                if (!string.IsNullOrWhiteSpace(valF) && !OSIExecutorCategoriesHelper.IsExistByCategoryName(valF))
                    errors.Add($"活動執行者(類別)「{valF}」不存在");
                if (valK != "無"
                    && !string.IsNullOrWhiteSpace(valK)
                    && !OSICarrierTypesHelper.IsExistByCarrierTypeName(valK))
                    errors.Add($"使用載具名稱(類別)「{valK}」不存在");
                if (!string.IsNullOrWhiteSpace(valN) && !OSIResearchItemsHelper.IsExistByItemName(valN))
                    errors.Add($"研究調查項目(類別)「{valN}」不存在");
                if (valR != "請選擇"
                    && !string.IsNullOrWhiteSpace(valR)
                    && !OSISurveyCountiesHelper.IsExistByCountyName(valR))
                    errors.Add($"研究調查範圍(縣市)「{valR}」不存在");
                if ((valR == "請選擇" || string.IsNullOrWhiteSpace(valR)) && string.IsNullOrWhiteSpace(valS))
                    errors.Add("研究調查範圍(縣市) 與 研究調查範圍(描述) 至少須擇一填寫");

                // 組成 DataRow
                var dr = dt.NewRow();
                dr["檢核結果"] = errors.Count == 0 ? "通過" : string.Join("；", errors);
                dr["填報機關"] = valB;
                dr["活動名稱"] = valC;
                dr["活動性質"] = valD;
                dr["活動性質(描述)"] = valE;
                dr["活動執行者(類別)"] = valF;
                dr["活動執行者(描述)"] = valG;
                dr["研究調查日期(起始)"] = valH;
                dr["研究調查日期(結束)"] = valI;
                dr["研究調查日期(描述)"] = valJ;
                dr["使用載具名稱(類別)"] = valK;
                dr["使用載具名稱(描述)"] = valL;
                dr["使用載具名稱(核准文號)"] = valM;
                dr["研究調查項目(類別)"] = valN;
                dr["研究調查項目(描述)"] = valO;
                dr["研究調查儀器"] = valP;
                dr["研究調查活動內容概述"] = valQ;
                dr["研究調查範圍(縣市)"] = valR;
                dr["研究調查範圍(描述)"] = valS;

                dt.Rows.Add(dr);
            }

        return dt;
    }

    private void ImportData()
    {
        // 1. 先從 Session 拿回檢核過的 DataTable
        var dt = Session["ImportResults"] as DataTable;
        if (dt == null || dt.Rows.Count == 0) return;

        // 2. 取得目前要用的 PeriodID、以及登入者 ID
        int periodId = int.Parse(hdnPeriodID.Value);
        int userId = UserInfo.UserID.toInt();
        // 3. 準備 InsertReport 要用的 baseDir
        var webRoot = Server.MapPath("~");
        var projectRoot = Path.GetFullPath(Path.Combine(webRoot, ".."));
        var baseDir = Path.Combine(projectRoot, "UploadFiles", "OSI");

        // 4. 逐筆處理
        foreach (DataRow dr in dt.Rows)
        {
            // 只對「通過」的筆做匯入
            if (dr["檢核結果"].ToString() != "通過")
                continue;

            // 5. 建立主檔 Model            
            var report = new GS.OCA_OceanSubsidy.Entity.OSI_ActivityReports
            {
                PeriodID = periodId,
                ReportingUnitID = SysUnitHelper.QueryIDByName(dr["填報機關"].ToString()),
                ActivityName = dr["活動名稱"].ToString(),
                NatureID = OSIActivityNaturesHelper.QueryIDByName(dr["活動性質"].ToString()),
                NatureText = dr["活動性質(描述)"].ToString(),
                ResearchItemID = OSIResearchItemsHelper.QueryIDByName(dr["研究調查項目(類別)"].ToString()),
                ResearchItemNote = dr["研究調查項目(描述)"].ToString(),
                Instruments = dr["研究調查儀器"].ToString(),
                ActivityOverview = dr["研究調查活動內容概述"].ToString(),
                LastUpdated = DateTime.Now,
                LastUpdatedBy = userId,
                IsValid = true,
            };

            // 6. 只匯入一筆 executor
            var executors = new List<OSI_ActivityExecutors>
            {
                new OSI_ActivityExecutors
                {
                    CategoryID   = OSIExecutorCategoriesHelper.QueryIDByName(dr["活動執行者(類別)"].ToString()),
                    ExecutorName = dr["活動執行者(描述)"].ToString()
                }
            };

            // 7. 只匯入一筆 research period
            DateTime start, end;
            TryParseRocDate(dr["研究調查日期(起始)"].ToString(), out start);
            TryParseRocDate(dr["研究調查日期(結束)"].ToString(), out end);

            var resPeriods = new List<OSI_ResearchPeriods>
            {
                new OSI_ResearchPeriods
                {
                    StartDate   = start,
                    EndDate     = end,
                    PeriodLabel = dr["研究調查日期(描述)"].ToString()
                }
            };

            // 8. 只匯入一筆 survey County
            var surveyCounties = new List<OSI_SurveyCounties>();
            string surveyCountyStr = dr["研究調查範圍(縣市)"].ToString();
            if (!string.IsNullOrWhiteSpace(surveyCountyStr))
            {
                int countyID = OSISurveyCountiesHelper.QueryIDByName(surveyCountyStr);
                if (countyID > 0)
                {
                    surveyCounties.Add(new OSI_SurveyCounties
                    {
                        CountyID = countyID
                    });
                }                
            }

            // 9. 只匯入一筆 survey scope
            var surveyScopes = new List<OSI_SurveyScopes>();
            if (!string.IsNullOrWhiteSpace(dr["研究調查範圍(描述)"].ToString()))
            {
                surveyScopes.Add(new OSI_SurveyScopes
                {
                    SurveyScope = dr["研究調查範圍(描述)"].ToString()
                });
            }

            // 10. 只匯入一筆 carrier (條件式新增)
            var carriers = new List<OSI_Carrier>();
            string carrierTypeStr = dr["使用載具名稱(類別)"].ToString();
            string carrierDetail = dr["使用載具名稱(描述)"].ToString();
            string carrierNo = dr["使用載具名稱(核准文號)"].ToString();
            int? carrierTypeID = null;

            // 檢查載具類型是否有效
            if (!string.IsNullOrWhiteSpace(carrierTypeStr))
            {
                carrierTypeID = OSICarrierTypesHelper.QueryIDByName(carrierTypeStr);
                if (carrierTypeID <= 0) carrierTypeID = null; // 無效載具類型
            }

            // 只有當載具類型有效且載具名稱不為空時才新增
            if (carrierTypeID.HasValue || !string.IsNullOrWhiteSpace(carrierDetail) || !string.IsNullOrWhiteSpace(carrierNo))
            {
                carriers.Add(new OSI_Carrier
                {
                    CarrierTypeID = carrierTypeID,
                    CarrierDetail = carrierDetail,
                    CarrierNo = carrierNo
                });
            }

            // 11. 呼叫你原本的 InsertReport（files、delLists 都空清單）
            OSIActivityReportsHelper.InsertReport(
                report,
                executors,
                resPeriods,
                files: new List<OSI_ActivityFiles>(),
                surveyScopes,
                carriers,
                surveyCounties,
                delExecutors: new List<int>(),
                delResPeriods: new List<int>(),
                delFiles: new List<int>(),
                delSurveyScopes: new List<int>(),
                delCarriers: new List<int>(),
                delSurveyCounties: new List<int>(),
                baseDir
            );
        }

        // 12. 匯入完成後，可以清掉 Session
        Session.Remove("ImportResults");
    }

    private bool TryParseRocDate(string text, out DateTime result)
    {
        result = default;
        var m = Regex.Match(text, @"^(?<y>\d{1,3})[./\-](?<m>\d{1,2})[./\-](?<d>\d{1,2})$");
        if (!m.Success) return false;

        int rocY = int.Parse(m.Groups["y"].Value);
        int mm = int.Parse(m.Groups["m"].Value);
        int dd = int.Parse(m.Groups["d"].Value);
        int year = rocY + 1911;

        try
        {
            result = new DateTime(year, mm, dd);
            return true;
        }
        catch { return false; }
    }
    
    /// <summary>
    /// 安全地從 DataRow 取得欄位值
    /// </summary>
    private string GetCellValue(DataRow row, int columnIndex)
    {
        if (columnIndex >= row.Table.Columns.Count)
            return string.Empty;
            
        var value = row[columnIndex];
        return value?.ToString().Trim() ?? string.Empty;
    }



}