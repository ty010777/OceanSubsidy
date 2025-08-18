using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Entity.Base;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.App;

public partial class OFS_SCI_SciInprogress_Approved : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 目前檢視的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 檢查是否有計畫ID
            if (string.IsNullOrEmpty(ProjectID))
            {
                Response.Redirect("~/OFS/inprogressList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // 初始化頁面
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
            // 載入所有申請資料到 UserControl (檢視模式)
            LoadAllUserControlData();

            // 設定計畫資訊到 Master Page
             SetProjectInfoToMaster();
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化頁面時發生錯誤");
        }
    }

    /// <summary>
    /// 載入所有 UserControl 資料
    /// </summary>
    private void LoadAllUserControlData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 第一頁：申請表/聲明書 (檢視模式)
            ucSciApplication.LoadData(ProjectID, isViewMode: true);
            
            // 第二頁：期程／工作項目／查核 (檢視模式)
            ucSciWorkSch.LoadData(ProjectID, isViewMode: true);
            
            // 第三頁：經費／人事 (檢視模式)
            ucSciFunding.LoadData(ProjectID, isViewMode: true);
            
            // 第四頁：其他 (檢視模式)
            ucSciRecusedList.LoadData(ProjectID, isViewMode: true);
            
            // 第五頁：上傳附件/提送申請 (檢視模式)
            ucSciUploadAttachments.LoadData(ProjectID, isViewMode: true);
            
            // 註冊 JavaScript，在所有資料載入完成後重新應用檢視模式
            RegisterViewModeScript();
            
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入 UserControl 資料時發生錯誤");
        }
    }

    /// <summary>
    /// 設定計畫資訊到 Master Page
    /// </summary>
    private void SetProjectInfoToMaster()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
        
        }
        catch (Exception ex)
        {
            HandleException(ex, "設定計畫資訊時發生錯誤");
        }
    }

    /// <summary>
    /// 註冊檢視模式 JavaScript，在所有動態內容載入後執行
    /// </summary>
    private void RegisterViewModeScript()
    {
        string script = @"
            setTimeout(function() {
                // 將所有輸入元素設為唯讀
                $('input, textarea, select').each(function() {
                    var $element = $(this);
                    
                    // 統一設為 readOnly 或 disabled
                    if ($element.is('input[type=text], input[type=number], input[type=email], input[type=tel], textarea')) {
                        $element.prop('readOnly', true);
                    } else if ($element.is('select')) {
                        $element.prop('disabled', true); 
                    } else if ($element.is('input[type=checkbox], input[type=radio]')) {
                        $element.prop('disabled', true); 
                    }
                });
                
                // 隱藏操作按鈕，但保留重要的按鈕
                $('button').not('.btn-close, [data-bs-dismiss=modal], [data-bs-toggle=modal], .btn-teal-dark, .btn-pink').each(function() {
                    var $element = $(this);
                    var buttonText = $element.text().trim();
                    
                    // 保留這 4 個重要按鈕
                    if (buttonText.indexOf('計畫變更申請') === -1 && 
                        buttonText.indexOf('計畫變更紀錄') === -1 && 
                        buttonText.indexOf('下載核定計畫書') === -1 && 
                        buttonText.indexOf('計畫終止') === -1 &&
                        buttonText.indexOf('查核紀錄') === -1) {
                        $element.hide();
                    }
                });
             
                // 特別處理動態生成的關鍵字欄位
                $('.keyword-ch, .keyword-en').prop('readOnly', true);
                
                // 隱藏所有新增/刪除按鈕
                $('.delete-keyword, .add-keyword, .add-row, .delete-row, .btn-add, .btn-delete').hide();
            }, 1500);
        ";
        
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ApplyViewMode", script, true);
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        // 可以在這裡加入記錄或通知邏輯
    }

    #endregion
}