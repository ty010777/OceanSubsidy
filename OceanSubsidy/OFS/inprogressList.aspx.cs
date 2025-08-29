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
            LoadData();
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化頁面時發生錯誤");
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
            
            // 從資料庫取得申請單位清單
            DbHelper db = new DbHelper();
            db.CommandText = @"SELECT DISTINCT OrgName FROM V_OFS_InprogressList 
                              WHERE OrgName IS NOT NULL AND OrgName <> ''
                              ORDER BY OrgName";
            DataTable dt = db.GetTable();
            
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
            
            // 從資料庫取得主管單位清單，參考 Type6 的實作方式
            DbHelper db = new DbHelper();
            db.CommandText = @"SELECT DISTINCT SupervisoryUnit 
                              FROM V_OFS_InprogressList 
                              WHERE SupervisoryUnit IS NOT NULL AND SupervisoryUnit <> ''
                              ORDER BY SupervisoryUnit";
            DataTable dt = db.GetTable();
            
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
            
            db.Dispose();
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
            // 建構查詢條件
            string whereClause = "WHERE 1=1";
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            
            if (!string.IsNullOrEmpty(ddlYear.SelectedValue))
            {
                whereClause += " AND [Year] = @Year";
                parameters.Add(new KeyValuePair<string, object>("@Year", ddlYear.SelectedValue));
            }
            
            if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
            {
                whereClause += " AND Category = @Category";
                parameters.Add(new KeyValuePair<string, object>("@Category", ddlCategory.SelectedValue));
            }
            
            if (!string.IsNullOrEmpty(ddlApplyUnit.SelectedValue))
            {
                whereClause += " AND OrgName = @OrgName";
                parameters.Add(new KeyValuePair<string, object>("@OrgName", ddlApplyUnit.SelectedValue));
            }
            
            if (!string.IsNullOrEmpty(ddlSupervisoryUnit.SelectedValue))
            {
                whereClause += " AND SupervisoryUnit = @SupervisoryUnit";
                parameters.Add(new KeyValuePair<string, object>("@SupervisoryUnit", ddlSupervisoryUnit.SelectedValue));
            }
            
            if (!string.IsNullOrEmpty(txtProjectKeyword.Text.Trim()))
            {
                whereClause += " AND (ProjectID LIKE @Keyword OR ProjectNameTw LIKE @Keyword)";
                parameters.Add(new KeyValuePair<string, object>("@Keyword", $"%{txtProjectKeyword.Text.Trim()}%"));
            }
            
            if (!string.IsNullOrEmpty(txtContentKeyword.Text.Trim()))
            {
                whereClause += " AND (ProjectContent LIKE @ContentKeyword OR KeyWords LIKE @ContentKeyword)";
                parameters.Add(new KeyValuePair<string, object>("@ContentKeyword", $"%{txtContentKeyword.Text.Trim()}%"));
            }
            
            // 取得全部資料
            DbHelper dataDb = new DbHelper();
            dataDb.CommandText = $@"
                SELECT [Year], Category, ProjectID, ProjectNameTw, OrgName, 
                       SupervisoryUnit, LastOperation,TaskNameEn, TaskName, ProjectContent, KeyWords
                FROM V_OFS_InprogressList
                {whereClause}
                ORDER BY [Year] DESC, ProjectID";
            
            foreach (var param in parameters)
            {
                dataDb.Parameters.Add(param.Key, param.Value);
            }
            
            DataTable result = dataDb.GetTable();
            totalRecords = result.Rows.Count; // 設定總記錄數
            dataDb.Dispose();
            
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
        
        // 透過 JavaScript 更新前端資料
        string script = $"updatePaginationData({jsonData});"; 
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