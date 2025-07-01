<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciOutcomes.aspx.cs" Inherits="OFS_SciOutcomes" Culture="zh-TW" UICulture="zh-TW" %>
<!DOCTYPE html>
<html lang="zh-Hant">
<head>
  <meta charset="UTF-8">
  <title>綜效指標項目</title>
  <style>
    body {
      font-family: Arial, sans-serif;
    }

    table {
      border-collapse: collapse;
      width: 100%;
    }

    th, td {
      border: 1px solid #ccc;
      padding: 8px;
      text-align: center;
      vertical-align: middle;
    }

    th {
      background-color: #d0f0f7;
    }

    td[colspan="2"] {
      background-color: #eef;
    }

    input[type="text"] {
      width: 50px;
    }

    textarea {
      width: 100%;
      height: 60px;
      resize: vertical;
    }

    .label {
      text-align: left;
    }

    .small-link {
      font-size: 12px;
      color: blue;
      cursor: pointer;
      display: inline-block;
      margin-top: 4px;
    }
  </style>
  <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
  <script>
function collectOutcomeData() {
  const result = [];

  $("table tbody tr").each(function () {
    const row = $(this);
    const title = row.find("td.label").text().trim();

    const inputs = row.find("input");
    const textareas = row.find("textarea");
    const description = textareas.length > 0 ? $(textareas[textareas.length - 1]).val() : "";

    let values = {};

    switch (title) {
      case "(1) 技術移轉":
        values = {
          TechTransfer_Plan_Count: $(inputs[0]).val(),
          TechTransfer_Plan_Price: $(inputs[1]).val(),
          TechTransfer_Track_Count: $(inputs[2]).val(),
          TechTransfer_Track_Price: $(inputs[3]).val()
        };
        break;
      case "(2) 專利":
        values = {
          Patent_Plan_Apply: $(inputs[0]).val(),
          Patent_Plan_Grant: $(inputs[1]).val(),
          Patent_Track_Apply: $(inputs[2]).val(),
          Patent_Track_Grant: $(inputs[3]).val()
        };
        break;
      case "(3) 人才培育":
        values = {
          Talent_Plan_PhD: $(inputs[0]).val(),
          Talent_Plan_Master: $(inputs[1]).val(),
          Talent_Plan_Others: $(inputs[2]).val(),
          Talent_Track_PhD: $(inputs[3]).val(),
          Talent_Track_Master: $(inputs[4]).val(),
          Talent_Track_Others: $(inputs[5]).val()
        };
        break;
      case "(4) 論文":
        values = {
          Papers_Plan: $(inputs[0]).val(),
          Papers_Track: $(inputs[1]).val()
        };
        break;
      case "(5) 促成產學研合作":
        values = {
          IndustryCollab_Plan_Count: $(inputs[0]).val(),
          IndustryCollab_Plan_Price: $(inputs[1]).val(),
          IndustryCollab_Track_Count: $(inputs[2]).val(),
          IndustryCollab_Track_Price: $(inputs[3]).val()
        };
        break;
      case "(6) 促成投資":
        values = {
          Investment_Plan_Price: $(inputs[0]).val(),
          Investment_Track_Price: $(inputs[1]).val()
        };
        break;
      case "(7) 衍生產品":
        values = {
          Products_Plan_Count: $(inputs[0]).val(),
          Products_Plan_Price: $(inputs[1]).val(),
          Products_Track_Count: $(inputs[2]).val(),
          Products_Track_Price: $(inputs[3]).val()
        };
        break;
      case "(8) 降低人力成本":
        values = {
          CostReduction_Plan_Price: $(inputs[0]).val(),
          CostReduction_Track_Price: $(inputs[1]).val()
        };
        break;
      case "(9) 技術推廣活動":
        values = {
          PromoEvents_Plan: $(inputs[0]).val(),
          PromoEvents_Track: $(inputs[1]).val()
        };
        break;
      case "(10) 技術服務":
        values = {
          TechServices_Plan_Count: $(inputs[0]).val(),
          TechServices_Plan_Price: $(inputs[1]).val(),
          TechServices_Track_Count: $(inputs[2]).val(),
          TechServices_Track_Price: $(inputs[3]).val()
        };
        break;
      case "(11) 其他":
        values = {
          Other_Plan_Description: $(textareas[0]).val(),
          Other_Track_Description: $(textareas[1]).val()
        };
        break;
    }

    result.push({
      item: title,
      values: values,
      description: description
    });
  });

  console.log(JSON.stringify(result, null, 2));
  return result;
}
function saveOutcomeData() {
  const data = collectOutcomeData(); // 收集所有輸入資料
  const projectId = new URLSearchParams(window.location.search).get("ProjectID"); // 從 URL 取 ProjectID

  $.ajax({
    type: "POST",
    url: "SciOutcomes.aspx/SaveOutcomeData", // ✅ 呼叫 WebMethod
    data: JSON.stringify({
      formData: {
        ProjectID: projectId,
        outcomeData: data
      }
    }),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      console.log("儲存成功:", response);
      alert("儲存成功");
    },
    error: function (xhr, status, error) {
      console.error("儲存失敗:", error);
      alert("儲存失敗，請稍後再試");
    }
  });
}


  </script>

</head>
<body>
<div class="links">
  <a href="http://localhost:50929/OFS/SciFunding.aspx?ProjectID=114SCI0006" target="_blank">📄 計畫經費填報</a>
  <a href="http://localhost:50929/OFS/SciApplication.aspx?ProjectID=114SCI0006" target="_blank">📝 科專申請資料</a>
  <a href="http://localhost:50929/OFS/SciOutcomes.aspx?ProjectID=114SCI0006" target="_blank">📊 成果與績效</a>
  <a href="http://localhost:50929/OFS/SciRecusedList.aspx?ProjectID=114SCI0006" target="_blank">📊 其他</a>

</div>

<form runat=server>
<table>
  <thead>
    <tr>
      <th rowspan="2">綜效指標項目</th>
      <th colspan="2">預估產出數</th>
      <th rowspan="2">說明</th>
    </tr>
    <tr>
      <th>計畫執行期間</th>
      <th>績效追蹤期間</th>
    </tr>
  </thead>
  <tbody>
    <!-- 技術移轉 -->
    <tr>
      <td class="label">(1) 技術移轉</td>
      <td>件 <input type="text"><br>千元 <input type="text"></td>
      <td>件 <input type="text"><br>千元 <input type="text"></td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 專利 -->
    <tr>
      <td class="label">(2) 專利</td>
      <td>申請 <input type="text"> 件<br>取得 <input type="text"> 件</td>
      <td>申請 <input type="text"> 件<br>取得 <input type="text"> 件</td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 人才培育 -->
    <tr>
      <td class="label">(3) 人才培育</td>
      <td>博士 <input type="text"> 人<br>碩士 <input type="text"> 人<br>其他 <input type="text"> 人</td>
      <td>博士 <input type="text"> 人<br>碩士 <input type="text"> 人<br>其他 <input type="text"> 人</td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 論文 -->
    <tr>
      <td class="label">(4) 論文</td>
      <td><input type="text"> 篇</td>
      <td><input type="text"> 篇</td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 促成產學研合作 -->
    <tr>
      <td class="label">(5) 促成產學研合作</td>
      <td><input type="text"> 件<br><input type="text"> 千元</td>
      <td><input type="text"> 件<br><input type="text"> 千元</td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 促成投資 -->
    <tr>
      <td class="label">(6) 促成投資</td>
      <td><input type="text"> 千元</td>
      <td><input type="text"> 千元</td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 衍生產品 -->
    <tr>
      <td class="label">(7) 衍生產品</td>
      <td><input type="text"> 項<br><input type="text"> 千元</td>
      <td><input type="text"> 項<br><input type="text"> 千元</td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 降低人力成本 -->
    <tr>
      <td class="label">(8) 降低人力成本</td>
      <td><input type="text"> 千元</td>
      <td><input type="text"> 千元</td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 技術推廣活動 -->
    <tr>
      <td class="label">(9) 技術推廣活動</td>
      <td><input type="text"> 場</td>
      <td><input type="text"> 場</td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 技術服務 -->
    <tr>
      <td class="label">(10) 技術服務</td>
      <td><input type="text"> 次<br><input type="text"> 千元</td>
      <td><input type="text"> 次<br><input type="text"> 千元</td>
      <td>
        <textarea></textarea>
        <div class="small-link">輸入填寫範例</div>
      </td>
    </tr>

    <!-- 其他 -->
    <tr>
      <td class="label">(11) 其他</td>
      <td>
        <textarea></textarea>
      </td>
      <td>
        <textarea></textarea>
      </td>
      <td></td>
    </tr>
  </tbody>
</table>
 <div style="text-align: center;">
      <asp:Button ID="btnTempSave" runat="server" Text="暫時儲存表單" CssClass="save-btn" OnClientClick="saveOutcomeData(); return false;" />
  </div>
</form>
</body>
</html>
