<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciWorkSch.aspx.cs" Inherits="OFS_SciWorkSch" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciWorkSch.js") %>"></script>
</asp:Content>

<asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <!-- 內容區塊 -->
                  <div class="block">
                  
                      <h5 class="square-title">期程及工作項目</h5>
                      <div class="mt-3" style="overflow-x: auto;">
                          <table class="table align-middle gray-table side-table" id= "workProjectList">
                              <tbody>
                                  <tr>
                                      <th>
                                          <span class="text-pink">*</span>
                                          計畫期程
                                      </th>
                                      <td>
                                          
                                          <div class="d-flex align-items-center flex-wrap gap-2">
                                              <div class="input-group" style="width: 400px;">
                                                  <input id="startDate" name="startDate" type="date" class="form-control" aria-label="計畫開始日期">
                                                  <span class="input-group-text">至</span>
                                                  <input id="endDate" name="endDate" type="date" class="form-control" aria-label="計畫結束日期">
                                              </div>
                                              <span class="text-dark-green2">(期程不可超過 115/04/30)</span>
                                          </div>
                                      </td>
                                  </tr>
                                  <tr>
                                      <th>
                                          延續性計畫
                                      </th>
                                      <td>
                                          <!-- 次表格 -->
                                          <div class="table-responsive sub-table">
                                              <table class="table align-middle gray-table">
                                                  <thead>
                                                      <tr>
                                                          <th>編號</th>
                                                          <th>
                                                              <span class="text-pink">*</span>
                                                              工作項目／工作子項
                                                          </th>
                                                          <th>
                                                              <span class="text-pink">*</span>
                                                              起訖月份
                                                          </th>
                                                          <th width="110">
                                                              <span class="text-pink">*</span>
                                                              權重
                                                          </th>
                                                          <th width="110">
                                                              <span class="text-pink">*</span>
                                                              投入<br>人月數
                                                          </th>
                                                          <th width="90">委外</th>
                                                          <th width="120">功能</th>
                                                      </tr>
                                                  </thead>
                                                  <!-- A項 -->
                                                  <tbody>
                                                      <tr>
                                                          <td>
                                                              A
                                                          </td>
                                                          <td>
                                                              <div class="mb-2">工作項目</div>
                                                              <input type="text" class="form-control" placeholder="請輸入">
                                                          </td>
                                                          <td>
                                                              
                                                          </td>
                                                          <td>
                                                              0%
                                                          </td>
                                                          <td class="text-center">
                                                              0
                                                          </td>
                                                          <td class="text-center">
                                                              
                                                          </td>
                                                          <td>
                                                              <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-trash-alt"></i></button>
                                                          </td>
                                                      </tr>
                                                      <tr>
                                                          <td>
                                                              A1
                                                          </td>
                                                          <td>
                                                              <div class="mb-2">子項目</div>
                                                              <input type="text" class="form-control" placeholder="請輸入">
                                                          </td>
                                                          <td>
                                                              <div class="input-group">
                                                                  <span class="input-group-text">開始</span>
                                                                  <select name=""  class="form-select month-select">
                                                                      <option value="" selected disabled>請選擇</option>
                                                                      <option value="1">01</option>
                                                                      <option value="2">02</option>
                                                                      <option value="3">03</option>
                                                                      <option value="4">04</option>
                                                                      <option value="5">05</option>
                                                                      <option value="6">06</option>
                                                                      <option value="7">07</option>
                                                                      <option value="8">08</option>
                                                                      <option value="9">09</option>
                                                                      <option value="10">10</option>
                                                                      <option value="11">11</option>
                                                                      <option value="12">12</option>
                                                                  </select>
                                                              </div>
                                                              <div class="input-group mt-2">
                                                                  <span class="input-group-text">結束</span>
                                                                  <select name=""  class="form-select month-select">
                                                                      <option value="" selected disabled>請選擇</option>
                                                                      <option value="1">01</option>
                                                                      <option value="2">02</option>
                                                                      <option value="3">03</option>
                                                                      <option value="4">04</option>
                                                                      <option value="5">05</option>
                                                                      <option value="6">06</option>
                                                                      <option value="7">07</option>
                                                                      <option value="8">08</option>
                                                                      <option value="9">09</option>
                                                                      <option value="10">10</option>
                                                                      <option value="11">11</option>
                                                                      <option value="12">12</option>
                                                                  </select>
                                                              </div>
                                                          </td>
                                                          <td>
                                                              <input type="text" class="form-control" placeholder="">
                                                          </td>
                                                          <td class="text-center">
                                                              <input type="text" class="form-control" placeholder="">
                                                          </td>
                                                          <td class="text-center">
                                                              <input class="form-check-input blue-green-check" type="checkbox" value="" checked>
                                                          </td>
                                                          <td>
                                                              <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-trash-alt"></i></button>
                                                          </td>
                                                      </tr>
                                                      <tr>
                                                          <td>
                                                              A2
                                                          </td>
                                                          <td>
                                                              <div class="mb-2">子項目</div>
                                                              <input type="text" class="form-control" placeholder="請輸入">
                                                          </td>
                                                          <td>
                                                              <div class="input-group">
                                                                  <span class="input-group-text">開始</span>
                                                                  <select name="" class="form-select month-select">
                                                                      <option value="" selected disabled>請選擇</option>
                                                                      <option value="1">01</option>
                                                                      <option value="2">02</option>
                                                                      <option value="3">03</option>
                                                                      <option value="4">04</option>
                                                                      <option value="5">05</option>
                                                                      <option value="6">06</option>
                                                                      <option value="7">07</option>
                                                                      <option value="8">08</option>
                                                                      <option value="9">09</option>
                                                                      <option value="10">10</option>
                                                                      <option value="11">11</option>
                                                                      <option value="12">12</option>
                                                                  </select>
                                                              </div>
                                                              <div class="input-group mt-2">
                                                                  <span class="input-group-text">結束</span>
                                                                  <select name=""  class="form-select month-select">
                                                                      <option value="" selected disabled>請選擇</option>
                                                                      <option value="1">01</option>
                                                                      <option value="2">02</option>
                                                                      <option value="3">03</option>
                                                                      <option value="4">04</option>
                                                                      <option value="5">05</option>
                                                                      <option value="6">06</option>
                                                                      <option value="7">07</option>
                                                                      <option value="8">08</option>
                                                                      <option value="9">09</option>
                                                                      <option value="10">10</option>
                                                                      <option value="11">11</option>
                                                                      <option value="12">12</option>
                                                                  </select>
                                                              </div>
                                                          </td>
                                                          <td>
                                                              <input type="text" class="form-control" placeholder="">
                                                          </td>
                                                          <td class="text-center">
                                                              <input type="text" class="form-control" placeholder="">
                                                          </td>
                                                          <td class="text-center">
                                                              <input class="form-check-input blue-green-check" type="checkbox" value="" checked>
                                                          </td>
                                                          <td>
                                                              <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-trash-alt"></i></button>
                                                              <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-plus"></i></button>
                                                          </td>
                                                      </tr>
                                                  </tbody>
                      
                                                  <!-- 新增B項 -->
                                                  <tbody>
                                                      <tr>
                                                          <td>
                                                              B
                                                          </td>
                                                          <td>
                                                              <div class="mb-2">工作項目</div>
                                                              <input type="text" class="form-control" placeholder="請輸入">
                                                          </td>
                                                          <td>
                                                              
                                                          </td>
                                                          <td>
                                                              0%
                                                          </td>
                                                          <td class="text-center">
                                                             0
                                                          </td>
                                                          <td class="text-center">
                                                              
                                                          </td>
                                                          <td>
                                                              <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-trash-alt"></i></button>
                                                          </td>
                                                      </tr>
                                                      <tr>
                                                          <td>
                                                              B1
                                                          </td>
                                                          <td>
                                                              <div class="mb-2">子項目</div>
                                                              <input type="text" class="form-control" placeholder="請輸入">
                                                          </td>
                                                          <td>
                                                              <div class="input-group">
                                                                  <span class="input-group-text">開始</span>
                                                                  <select name=""  class="form-select month-select">
                                                                      <option value="" selected disabled>請選擇</option>
                                                                      <option value="1">01</option>
                                                                      <option value="2">02</option>
                                                                      <option value="3">03</option>
                                                                      <option value="4">04</option>
                                                                      <option value="5">05</option>
                                                                      <option value="6">06</option>
                                                                      <option value="7">07</option>
                                                                      <option value="8">08</option>
                                                                      <option value="9">09</option>
                                                                      <option value="10">10</option>
                                                                      <option value="11">11</option>
                                                                      <option value="12">12</option>
                                                                  </select>
                                                              </div>
                                                              <div class="input-group mt-2">
                                                                  <span class="input-group-text">結束</span>
                                                                  <select name=""  class="form-select month-select">
                                                                      <option value="" selected disabled>請選擇</option>
                                                                      <option value="1">01</option>
                                                                      <option value="2">02</option>
                                                                      <option value="3">03</option>
                                                                      <option value="4">04</option>
                                                                      <option value="5">05</option>
                                                                      <option value="6">06</option>
                                                                      <option value="7">07</option>
                                                                      <option value="8">08</option>
                                                                      <option value="9">09</option>
                                                                      <option value="10">10</option>
                                                                      <option value="11">11</option>
                                                                      <option value="12">12</option>
                                                                  </select>
                                                              </div>
                                                          </td>
                                                          <td>
                                                              <input type="text" class="form-control" placeholder="">
                                                          </td>
                                                          <td class="text-center">
                                                              <input type="text" class="form-control" placeholder="">
                                                          </td>
                                                          <td class="text-center">
                                                              <input class="form-check-input blue-green-check" type="checkbox" value="" checked>
                                                          </td>
                                                          <td>
                                                              <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-trash-alt"></i></button>
                                                              <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-plus"></i></button>
                                                          </td>
                                                      </tr>
                                                  </tbody>
                      
                      
                                                  <!-- 總計列 -->
                                                  <tfoot>
                                                      <tr>
                                                          <td colspan="3">合計</td>
                                                          <td>0%</td>
                                                          <td>0</td>
                                                          <td colspan="2"></td>
                                                      </tr>
                                                  </tfoot>
                                              </table>
                                          </div>
                      
                                          <!-- 新增按鈕 -->
                                          <button class="btn btn-blue-green2" type="button">新增工作項目</button>
                                      </td>
                                  </tr>
                              </tbody>
                          </table>
                          
                      </div>
                      
                      
                      
                      
                      
                  
                      <h5 class="square-title mt-4 gap-2 flex-wrap">查核標準
                          <span class="text-pink fw-normal fs-16">＊每一完整季度（Q1-Q4）需至少兩項查核點</span>
                      </h5>
                      
                      <div class="table-responsive mt-3">
                          <table class="table align-middle gray-table" id= "checkStandards" >
                              <thead>
                                  <tr>
                                      <th>
                                          <span class="text-pink">*</span>
                                          對應工項
                                      </th>
                                      <th>編號</th>
                                      <th>
                                          <span class="text-pink">*</span>
                                          預定完成日
                                      </th>
                                      <th>
                                          <span class="text-pink">*</span>
                                          查核內容概述（請具體明確或量化）
                                      </th>
                                      <th width="120">功能</th>
                                  </tr>
                              </thead>
                              <tbody>
                                  <tr>
                                      <td class="align-middle">
                                          <select name=""  class="form-select">
                                              <option value="">A1.子項目XXXXXX</option>
                                              <option value="">A1.子項目XXXXXX</option>
                                              <option value="">A1.子項目XXXXXX</option>
                                          </select>
                                      </td>
                                      <td class="align-middle"></td>
                                      <td class="align-middle">
                                          <input type="date" name="" class="form-control">
                                      </td>
                                      <td class="align-middle" width="500">
                                          <span class="form-control textarea" role="textbox" contenteditable="" data-placeholder="請輸入" aria-label="文本輸入區域"></span>
                                      </td>
                                      <td class="align-middle">
                                          <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-trash-alt"></i></button>
                                          <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-plus"></i></button>
                                      </td>
                                  </tr>
                      
                              </tbody>
                          </table>
                      </div>
                      
                      
                      
                      
                  
                      <h5 class="square-title mt-4 gap-2 flex-wrap">計畫架構
                          <span class="text-pink fw-normal fs-16">＊務必說明</span><br>
                      </h5>
                      
                      
                      
                      <div class="mt-3">
                          <table class="table align-middle gray-table side-table">
                              <tbody>
                                  <tr>
                                      <th>
                                          <span class="text-pink">*</span>
                                          計畫架構
                                      </th>
                                      <td>
                                          <a href="<%= ResolveUrl("~/assets/img/project-structure-example.png") %>" class="link-teal" target="_blank">範例圖下載<i class="fas fa-file-download ms-1"></i></a>
                                          <div class="input-group mt-3">
                                              <input type="file" id="fileUploadDiagram"  name="fileUploadDiagram"
                                                     class="form-control" 
                                                     accept="image/*" />
                                              <button type="button" id="btnUploadDiagram" 
                                                      class="btn btn-blue-green2">
                                                  上傳
                                              </button>
                                          </div>
                      
                                          <div id="diagramPreviewContainer" class="mt-3" style="display: none;">
                                              <button type="button" id="btnDeleteDiagram" 
                                                      class="btn btn-outline-danger ms-auto d-table mb-2">
                                                  刪除
                                              </button>
                                              <img id="diagramPreview" class="img-fluid" src="" alt="計畫架構圖" />
                                          </div>
                      
                                          <ul class="list-unstyled text-gray lh-base">
                                              <li>請以樹狀圖撰寫（如有海洋科技技術移轉、委託研究等項目，亦請註明)</li>
                                              <li>請註明下列資料：</li>
                                              <li>1.執行該工作項目/開發技術之單位。</li>
                                              <li>2.若有委託海洋科技研究或技術移轉請一併列入計畫架構，且單獨列出，兩者權重合計須小於40%，超過者，不予受理。</li>
                                              <li>3.若有共同執行單為工作項目請一併列入計畫架構，且單獨列出。</li>
                                              <li>4.提案計畫主持人執行經費與工作比重（不含委託勞務）應超過總體60%，若未達，不予受理。</li>
                                          </ul>
                                      </td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>
                  </div>
                  
    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <asp:Button ID="btnTempSave" runat="server" 
            Text="暫存" 
            CssClass="btn btn-outline-teal" 
            OnClick="btnTempSave_Click" />
        <asp:Button ID="btnSaveAndNext" runat="server" 
            Text="完成本頁，下一步" 
            CssClass="btn btn-teal" 
            OnClick="btnSaveAndNext_Click" />
    </div>
</asp:Content>