using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.App;
using Newtonsoft.Json;
using GS.Data;
using GS.Data.Sql;

public partial class inprogressList : System.Web.UI.Page
{
    #region 私有變數和屬性
    /// <summary>
    /// 總記錄數 - 僅用於顯示
    /// </summary>
    private int totalRecords 
    { 
        get { return ViewState["TotalRecords"] != null ? (int)ViewState["TotalRecords"] : 0; }
        set { ViewState["TotalRecords"] = value; }
    }
    #endregion

    #region 頁面事件
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "頁面載入時發生錯誤");
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            LoadDropDownLists();
            LoadQueryStringParameters();
            LoadData();
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化頁面時發生錯誤");
        }
    }

    /// <summary>
    /// 從 URL QueryString 載入查詢條件
    /// </summary>
    private void LoadQueryStringParameters()
    {
        try
        {
            // 年度
            if (!string.IsNullOrEmpty(Request.QueryString["year"]))
            {
                string year = Request.QueryString["year"];
                if (ddlYear.Items.FindByValue(year) != null)
                {
                    ddlYear.SelectedValue = year;
                }
            }

            // 類別
            if (!string.IsNullOrEmpty(Request.QueryString["category"]))
            {
                string category = Request.QueryString["category"];
                if (ddlCategory.Items.FindByValue(category) != null)
                {
                    ddlCategory.SelectedValue = category;
                }
            }

            // 申請單位
            if (!string.IsNullOrEmpty(Request.QueryString["applyUnit"]))
            {
                string applyUnit = Request.QueryString["applyUnit"];
                if (ddlApplyUnit.Items.FindByValue(applyUnit) != null)
                {
                    ddlApplyUnit.SelectedValue = applyUnit;
                }
            }

            // 主管單位
            if (!string.IsNullOrEmpty(Request.QueryString["supervisoryUnit"]))
            {
                string supervisoryUnit = Request.QueryString["supervisoryUnit"];
                if (ddlSupervisoryUnit.Items.FindByValue(supervisoryUnit) != null)
                {
                    ddlSupervisoryUnit.SelectedValue = supervisoryUnit;
                }
            }

            // 計畫編號或名稱關鍵字
            if (!string.IsNullOrEmpty(Request.QueryString["projectKeyword"]))
            {
                txtProjectKeyword.Text = Request.QueryString["projectKeyword"];
            }

            // 計畫內容關鍵字
            if (!string.IsNullOrEmpty(Request.QueryString["contentKeyword"]))
            {
                txtContentKeyword.Text = Request.QueryString["contentKeyword"];
            }

            // 待回覆
            if (!string.IsNullOrEmpty(Request.QueryString["pendingReply"]))
            {
                string pendingReplyValue = Request.QueryString["pendingReply"].ToLower();
                chkPendingReply.Checked = (pendingReplyValue == "true" || pendingReplyValue == "1");
            }
        }
        catch (Exception ex)
        {
            // 載入 QueryString 失敗時不影響頁面正常運作
            System.Diagnostics.Debug.WriteLine($"載入 QueryString 參數時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 載入下拉選單資料
    /// </summary>
    private void LoadDropDownLists()
    {
        try
        {
            LoadYearDropDown();
            LoadCategoryDropDown();
            LoadApplyUnitDropDown();
            LoadSupervisoryUnitDropDown();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入下拉選單時發生錯誤");
        }
    }

    /// <summary>
    /// 載入年度選項
    /// </summary>
    private void LoadYearDropDown()
    {
        ddlYear.Items.Clear();
        ddlYear.Items.Add(new ListItem("全部", ""));
        ddlYear.Items.Add(new ListItem("114年","114"));

      
    }

    /// <summary>
    /// 載入類別選項
    /// </summary>
    private void LoadCategoryDropDown()
    {
        ddlCategory.Items.Clear();
        ddlCategory.Items.Add(new ListItem("全部", ""));
        ddlCategory.Items.Add(new ListItem("科專", "SCI"));
        ddlCategory.Items.Add(new ListItem("文化", "CUL"));
        ddlCategory.Items.Add(new ListItem("學校民間", "EDC"));
        ddlCategory.Items.Add(new ListItem("學校社團", "CLB"));
        ddlCategory.Items.Add(new ListItem("多元", "MUL"));
        ddlCategory.Items.Add(new ListItem("素養", "LIT"));
        ddlCategory.Items.Add(new ListItem("無障礙", "ACC"));
    }

    /// <summary>
    /// 載入申請單位選項
    /// </summary>
    private void LoadApplyUnitDropDown()
    {
        try
        {
            ddlApplyUnit.Items.Clear();
            ddlApplyUnit.Items.Add(new ListItem("全部", ""));

            // 從 Helper 取得申請單位清單
            DataTable dt = InprogressListHelper.GetApplyUnits();

            foreach (DataRow row in dt.Rows)
            {
                string orgName = row["OrgName"].ToString();
                ddlApplyUnit.Items.Add(new ListItem(orgName, orgName));
            }
        }
        catch (Exception ex)
        {
            // 載入失敗時使用預設選項
            ddlApplyUnit.Items.Clear();
            ddlApplyUnit.Items.Add(new ListItem("全部", ""));
        }
    }

    /// <summary>
    /// 載入主管單位選項
    /// </summary>
    private void LoadSupervisoryUnitDropDown()
    {
        try
        {
            ddlSupervisoryUnit.Items.Clear();
            ddlSupervisoryUnit.Items.Add(new ListItem("全部", ""));

            // 從 Helper 取得主管單位清單
            DataTable dt = InprogressListHelper.GetSupervisoryUnits();

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryUnit"] != DBNull.Value)
                {
                    string supervisoryUnit = row["SupervisoryUnit"].ToString().Trim();
                    if (!string.IsNullOrEmpty(supervisoryUnit))
                    {
                        ddlSupervisoryUnit.Items.Add(new ListItem(supervisoryUnit, supervisoryUnit));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 載入失敗時使用預設選項
            ddlSupervisoryUnit.Items.Clear();

        }
    }

    /// <summary>
    /// 載入資料
    /// </summary>
    private void LoadData()
    {
        try
        {
            DataTable data = GetInprogressListData();
            BindData(data);
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入資料時發生錯誤");
        }
    }

    /// <summary>
    /// 取得進度清單資料
    /// </summary>
    /// <returns>資料表</returns>
    private DataTable GetInprogressListData()
    {
        try
        {
            // 判斷當前使用者是否為主管機關人員
            bool isSupervisoryUser = IsSupervisoryUser();
            string userAccount = "";

            // 如果不是主管機關人員，只載入自己的案件
            if (!isSupervisoryUser)
            {
                var currentUser = GetCurrentUserInfo();
                if (currentUser != null)
                {
                    userAccount = currentUser.Account;
                }
            }

            // 從 Helper 取得進度清單資料
            DataTable result = InprogressListHelper.GetInprogressListData(
                ddlYear.SelectedValue,
                ddlCategory.SelectedValue,
                ddlApplyUnit.SelectedValue,
                ddlSupervisoryUnit.SelectedValue,
                txtProjectKeyword.Text.Trim(),
                txtContentKeyword.Text.Trim(),
                userAccount,
                chkPendingReply.Checked
            );

            totalRecords = result.Rows.Count; // 設定總記錄數

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得資料時發生錯誤: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 綁定資料到前端分頁系統
    /// </summary>
    /// <param name="data">資料表</param>
    private void BindData(DataTable data)
    {
        // 轉換 DataTable 為 JSON 格式供前端使用
        var jsonData = ConvertDataTableToJson(data);

        // 判斷當前使用者是否為主管機關人員
        bool isSupervisoryUser = IsSupervisoryUser();
        string userAccount = "";

        // 如果不是主管機關人員，只載入自己的案件統計
        if (!isSupervisoryUser)
        {
            var currentUser = GetCurrentUserInfo();
            if (currentUser != null)
            {
                userAccount = currentUser.Account;
            }
        }

        // 取得統計資訊
        DataRow statistics = InprogressListHelper.GetInprogressStatistics(
            ddlYear.SelectedValue,
            ddlCategory.SelectedValue,
            ddlApplyUnit.SelectedValue,
            ddlSupervisoryUnit.SelectedValue,
            txtProjectKeyword.Text.Trim(),
            txtContentKeyword.Text.Trim(),
            userAccount,
            chkPendingReply.Checked
        );

        // 建立統計資訊的 JSON 物件
        var statsJson = ConvertStatisticsToJson(statistics);

        // 透過 JavaScript 更新前端資料和統計數字
        string script = $@"
            updatePaginationData({jsonData});
            updateStatistics({statsJson});
        ";
        ScriptManager.RegisterStartupScript(this, GetType(), "updateData", script, true);

        lblTotalRecords.Text = totalRecords.ToString();
    }
    
    /// <summary>
    /// 將 DataTable 轉換為 JSON 字串
    /// </summary>
    /// <param name="dataTable">資料表</param>
    /// <returns>JSON 字串</returns>
    private string ConvertDataTableToJson(DataTable dataTable)
    {
        var jsonList = new List<Dictionary<string, object>>();

        foreach (DataRow row in dataTable.Rows)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in dataTable.Columns)
            {
                dict[col.ColumnName] = row[col] == DBNull.Value ? "" : row[col].ToString();
            }
            jsonList.Add(dict);
        }

        return JsonConvert.SerializeObject(jsonList);
    }

    /// <summary>
    /// 將統計資訊 DataRow 轉換為 JSON 字串
    /// </summary>
    /// <param name="statisticsRow">統計資訊的 DataRow</param>
    /// <returns>JSON 字串</returns>
    private string ConvertStatisticsToJson(DataRow statisticsRow)
    {
        if (statisticsRow == null)
        {
            return JsonConvert.SerializeObject(new
            {
                Total = 0,
                InProgress = 0,
                Overdue = 0,
                Closed = 0,
                Terminated = 0
            });
        }

        var stats = new Dictionary<string, object>
        {
            { "Total", statisticsRow["Total"] != DBNull.Value ? Convert.ToInt32(statisticsRow["Total"]) : 0 },
            { "InProgress", statisticsRow["InProgress"] != DBNull.Value ? Convert.ToInt32(statisticsRow["InProgress"]) : 0 },
            { "Overdue", statisticsRow["Overdue"] != DBNull.Value ? Convert.ToInt32(statisticsRow["Overdue"]) : 0 },
            { "Closed", statisticsRow["Closed"] != DBNull.Value ? Convert.ToInt32(statisticsRow["Closed"]) : 0 },
            { "Terminated", statisticsRow["Terminated"] != DBNull.Value ? Convert.ToInt32(statisticsRow["Terminated"]) : 0 }
        };

        return JsonConvert.SerializeObject(stats);
    }


    /// <summary>
    /// 處理例外狀況
    /// </summary>
    /// <param name="ex">例外物件</param>
    /// <param name="message">錯誤訊息</param>
    private void HandleException(Exception ex, string message)
    {
        // 記錄錯誤
        System.Diagnostics.Debug.WriteLine($"{message}: {ex.Message}");

        // 顯示錯誤訊息給使用者
        string script = $"alert('{message}');";
        ScriptManager.RegisterStartupScript(this, GetType(), "error", script, true);
    }

    /// <summary>
    /// 判斷當前使用者是否為主管單位人員
    /// </summary>
    /// <returns>true: 是主管單位人員, false: 不是主管單位人員</returns>
    private bool IsSupervisoryUser()
    {
        try
        {
            var currentUser = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);

            if (currentUser == null || currentUser.OFS_RoleName == null)
            {
                return false;
            }

            // 檢查角色名稱是否包含主管單位相關角色
            var supervisoryRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者" };

            foreach (var role in supervisoryRoles)
            {
                if (currentUser.OFS_RoleName.Contains(role))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"判斷使用者角色時發生錯誤：{ex.Message}");
            return false; // 發生錯誤時預設為非主管單位人員
        }
    }

    /// <summary>
    /// 取得目前登入使用者資訊
    /// </summary>
    private SessionHelper.UserInfoClass GetCurrentUserInfo()
    {
        try
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得使用者資訊時發生錯誤: {ex.Message}");
            return null;
        }
    }
    #endregion

    #region 事件處理
    /// <summary>
    /// 查詢按鈕點擊事件
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            LoadData();
        }
        catch (Exception ex)
        {
            HandleException(ex, "查詢時發生錯誤");
        }
    }
    #endregion
}