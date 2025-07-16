/**
 * 成果與績效 JavaScript 功能
 * 檔案: SciOutcomes.js
 * 作者: 系統生成
 * 說明: 處理成果與績效頁面的前端互動功能
 */


// 前端驗證函數 - 供未來需要時使用
function validateForm() {
    // 可以在這裡加入前端驗證邏輯
    return true;
}

// 填寫範例資料
var exampleTexts = {
    "tech_transfer": "完成○○技術移轉○件，技轉金額○千元。",
    "patent": "提出申請美國專利○件，取得中華民國發明專利○件。",
    "talent": "參與本計畫○○研究工作，培育博士○人、碩士○人，其中畢業後任職於相關企業○人。",
    "papers": "產出期刊論文○篇、研討會論文○篇。",
    "industry_collab": "促成產學研合作研究○件，合作研究金額○○○千元。",
    "investment": "促成業者布建產線○○○○千元及添購量產設備○○○○千元，累計促成投資金額○○○○千元。",
    "products": "衍生○○新產品○項，產品單價○千元*預期銷售量○○○份=增加產值○○○千元。",
    "cost_reduction": "利用○○技術，節省人力工資○○○○元/人天，以1年365天計算，每年可節省人力成本○○○○千元。",
    "promo_events": "辦理示範觀摩會○場及成果發表會○場，媒合潛在技轉業者○○家。",
    "tech_services": "應用○○研發成果，進行檢測技術服務○○次，技術服務收入○○千元。"
};

// 帶入填寫範例功能
function loadExample(indicatorType, linkElement) {
    try {
        // 找到該行的說明欄位
        var $row = $(linkElement).closest('tr');
        var $descriptionTextarea = $row.find('textarea[name$="_description"]'); // 說明欄位
        
        // 取得對應的範例文字
        var exampleText = exampleTexts[indicatorType] || "";
        
        if (exampleText) {
            // 將範例文字填入說明欄位
            $descriptionTextarea.val(exampleText);
        } else {
            alert("找不到對應的填寫範例");
        }
    } catch (ex) {
        console.error("帶入範例時發生錯誤:", ex);
        alert("帶入範例時發生錯誤");
    }
}

// 頁面載入後初始化
$(document).ready(function () {
    // 為所有「帶入填寫範例」連結加上點擊事件
    $('.link-teal').on('click', function(e) {
        e.preventDefault(); // 防止連結跳轉
        
        // 根據該行的績效指標類型決定indicatorType
        var $this = $(this);
        var $row = $this.closest('tr');
        var labelText = $row.find('th.label').text().trim();
        var indicatorType = "";
        
        switch (labelText) {
            case "(1)技術移轉":
                indicatorType = "tech_transfer";
                break;
            case "(2)專利":
                indicatorType = "patent";
                break;
            case "(3)人才培育":
                indicatorType = "talent";
                break;
            case "(4)論文":
                indicatorType = "papers";
                break;
            case "(5)促成產學研合作":
                indicatorType = "industry_collab";
                break;
            case "(6)促成投資":
                indicatorType = "investment";
                break;
            case "(7)衍生產品":
                indicatorType = "products";
                break;
            case "(8)降低人力成本":
                indicatorType = "cost_reduction";
                break;
            case "(9)技術推廣活動":
                indicatorType = "promo_events";
                break;
            case "(10)技術服務":
                indicatorType = "tech_services";
                break;
            case "(11) 其他":
                indicatorType = "other";
                break;
            default:
                alert("無法識別的績效指標類型");
                return;
        }
        
        // 呼叫帶入範例功能
        loadExample(indicatorType, this);
    });
    
    console.log("SciOutcomes 頁面已載入，填寫範例功能已初始化");
});