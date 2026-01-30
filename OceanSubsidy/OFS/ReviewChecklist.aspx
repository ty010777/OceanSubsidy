<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReviewChecklist.aspx.cs" Inherits="OFS_ReviewChecklist" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFSMaster.master" EnableViewState="true" EnableEventValidation="false" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="<%= ResolveUrl("~/script/OFS/ReviewChecklist.js") %>"></script>

</asp:Content>

<asp:Content ID="Breadcrumbs" ContentPlaceHolderID="Breadcrumbs" runat="server">
      <!-- 頁面標題 -->
       <div class="page-title">
           <img src="<%= ResolveUrl("~/assets/img/information-system-title-icon04.svg") %>" alt="logo">
           <div>
               <span>目前位置</span>
               <div class="d-flex align-items-end gap-3">
                   <h2 class="text-teal-dark">計畫審查</h2>
               </div>
           </div>
       </div>
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- 隱藏欄位和按鈕 -->
    <asp:HiddenField ID="hdnSelectedProjectIds" runat="server" ClientIDMode="Static"/>
    <asp:Button ID="btnSendToApplicant" runat="server" Text="提送至申請者"
                OnClick="btnSendToApplicant_Click" Style="display: none;" ClientIDMode="Static"/>


    <!-- 公告提醒 -->
    <div id="news-marquee">
        <news-marquee></news-marquee>
    </div>

    <!-- 總計列表 -->
    <ul class="total-list tab-light-green mt-4">
        <li class="total-item" id="total-item-1">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(1)">
                <div class="total-item-title">資格審查/內容審查</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-2">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(2)">
                <div class="total-item-title">實質審查/初審</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-3">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(3)">
                <div class="total-item-title">複審</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-4">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(4)">
                <div class="total-item-title">決審核定</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-5">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(5)">
                <div class="total-item-title">計畫變更審核</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-6">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(6)">
                <div class="total-item-title">執行計畫審核</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
    </ul>

    <div id="content-type-1" class="review-content" style="display: none;">
	    <div class="search bg-light-teal-100 rounded-0">
	     <!-- 查詢表單 -->
	       <div class="search-form" action="">
	           <div class="column-2">
	               <!-- 計畫編號或名稱關鍵字 -->
	               <div class="search-item">
	                   <div class="fs-16 text-gray mb-2">計畫編號或名稱關鍵字</div>
	                   <input type="text" name="txtKeyword_Type1" class="form-control" placeholder="請輸入計畫編號、計畫名稱相關文字">
	               </div>

	               <!-- 年度/類別/狀態 -->
	               <div class="row g-3">
	                   <div class="col-12 col-lg-3">
	                       <div class="fs-16 text-gray mb-2">年度</div>
	                       <asp:DropDownList ID="ddlYear_Type1" runat="server" CssClass="form-select">
	                       </asp:DropDownList>
	                   </div>
	                   <div class="col-12 col-lg-4">
	                       <div class="fs-16 text-gray mb-2">類別</div>
	                       <asp:DropDownList ID="ddlCategory_Type1" runat="server" CssClass="form-select">
	                       </asp:DropDownList>
	                   </div>
	                   <div class="col-12 col-lg-5">
	                       <div class="fs-16 text-gray mb-2">狀態</div>
	                       <asp:DropDownList ID="ddlStatus_Type1" runat="server" CssClass="form-select">
	                       </asp:DropDownList>
	                   </div>
	               </div>
	           </div>


	           <div class="column-2">
	               <div class="search-item">
	                   <div class="fs-16 text-gray mb-2">申請單位</div>
	                   <asp:DropDownList ID="ddlOrg_Type1" runat="server" CssClass="form-select">
	                   </asp:DropDownList>
	               </div>
	               <div class="search-item">
	                   <div class="fs-16 text-gray mb-2">承辦人員</div>
	                   <asp:DropDownList ID="ddlSupervisor_Type1" runat="server" CssClass="form-select">
	                   </asp:DropDownList>
	               </div>
	           </div>

	           <button type="button" id="btnSearch_Type1" class="btn btn-teal-dark d-table mx-auto" onclick="performAjaxSearch(1)">
	               🔍 查詢
	           </button>
	       </div>
		</div>

        <!-- 列表內容 -->

        <div class="block rounded-bottom-4">
            <div class="title border-teal">
                <div class="d-flex align-items-center gap-2">
                    <h4 class="text-teal">
                      <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
                      <span>列表</span>
                    </h4>
                    <span>共 <span class="text-teal" id="total-count-type1">0</span> 筆資料</span>
                </div>

                <div class="d-flex gap-2">
                    <button class="btn btn-teal-dark" type="button" onclick="exportType1ReviewingData()">
                        <i class="fas fa-download"></i>匯出審查中資料
                    </button>
                    <button class="btn btn-teal-dark" type="button" onclick="exportType1ReviewResults()">
                        <i class="fas fa-download"></i>匯出審查結果
                    </button>
                </div>
            </div>

            <div class="table-responsive mb-0">
                <table class="table teal-table" id="DataTable_Type1">
                    <thead>
                        <tr>
                            <th width="60">
                                <input class="form-check-input check-teal checkAll" type="checkbox" name="" >
                            </th>
                            <th width="50">年度</th>
                            <th width="100">
                                <span>類別</span>
                            </th>
                            <th width="140">
                                <span>計畫編號</span>
                            </th>
                            <th width="220">
                                <span>計畫名稱</span>
                            </th>
                            <th>
                                <span>申請單位</span>
                            </th>
                            <th width="150">
                                <span>申請經費</span>
                            </th>
                            <th>
                                <span>狀態</span>
                            </th>
                            <th>
                                <span>補正補件期限</span>
                            </th>
                            <th>
                                <span>承辦人員</span>
                            </th>
                            <th>功能</th>
                        </tr>
                    </thead>
                    <tbody>
                        <!-- 動態渲染的資料將會顯示在這裡 -->
                    </tbody>
                </table>
            </div>

            <!-- 審查勾選後底部功能按鈕 -->
            <div class="bg-light-teal-100 mb-5 checkPlanBtnPanel checkPlanBtnPanel-type1" style="display: none;">
                <div class="p-3 d-flex justify-content-between align-items-center">
                    <div class="d-flex gap-3">
                        <button class="btn btn-teal" type="button" onclick="openReviewerSetupModal()"><i class="fa-solid fa-check"></i>批次通過，轉入下一階段</button>
                        <button class="btn btn-pink" type="button" onclick="handleBatchReject('批次不通過')"><i class="fa-solid fa-xmark"></i>批次不通過，提送申請者</button>
                    </div>
                </div>
            </div>
              <!-- 分頁 -->
            <div id="pagination-type1" class="d-flex align-items-center justify-content-between flex-wrap gap-2 pagination-wrapper" data-review-type="1">
                <nav class="pagination justify-content-start" aria-label="Pagination">
                    <button class="nav-button btn-prev-page" aria-label="Previous page" disabled>
                        <i class="fas fa-chevron-left"></i>
                    </button>

                    <button class="nav-button btn-next-page" aria-label="Next page" disabled>
                        <i class="fas fa-chevron-right"></i>
                    </button>
                </nav>

                <div class="page-number-control">
                    <div class="page-number-control-item">
                        <span>跳到</span>
                        <select class="form-select jump-to-page">
                            <!-- 動態渲染頁數選項 -->
                        </select>
                        <span>頁</span>
                        <span>,</span>
                    </div>
                    <div class="page-number-control-item">
                        <span>每頁顯示</span>
                        <select class="form-select page-size-selector">
                            <option value="5">5</option>
                            <option value="10" selected>10</option>
                            <option value="20">20</option>
                            <option value="50">50</option>
                            <option value="100">100</option>
                        </select>
                        <span>筆</span>
                    </div>
                    <div class="pagination-info ms-3 text-muted small">
                        <!-- 分頁資訊將顯示在這裡 -->
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- 搜尋表單 -->
    <!-- 類型2：實質審查/初審 -->
    <div id="content-type-2" class="review-content" style="display: none;">
        <div class="search bg-light-teal-100 rounded-0">
            <!-- 查詢表單 -->
            <div class="search-form" action="">
              <div class="column-2">
            	  <!-- 計畫編號或名稱關鍵字 -->
            	  <div class="search-item">
            		  <div class="fs-16 text-gray mb-2">計畫編號或名稱關鍵字</div>
            		  <input type="text" name="txtKeyword_Type2" class="form-control" placeholder="請輸入計畫編號、計畫名稱相關文字">
            	  </div>

            	  <!-- 年度/類別/狀態 -->
            	  <div class="row g-3">
            		  <div class="col-12 col-lg-3">
            			  <div class="fs-16 text-gray mb-2">年度</div>
            			  <asp:DropDownList ID="ddlYear_Type2" runat="server" CssClass="form-select">
            			  </asp:DropDownList>
            		  </div>
            		  <div class="col-12 col-lg-4">
            			  <div class="fs-16 text-gray mb-2">類別</div>
            			  <asp:DropDownList ID="ddlCategory_Type2" runat="server" CssClass="form-select">
            			  </asp:DropDownList>
            		  </div>
            		  <div class="col-12 col-lg-5">
            			  <div class="fs-16 text-gray mb-2">審查進度</div>
            			  <asp:DropDownList ID="ddlProgress_Type2" runat="server" CssClass="form-select">
            			  </asp:DropDownList>
            		  </div>
            	  </div>
              </div>

              <div class="row">
            	  <div class="col-12 col-lg-2">
            		  <div class="fs-16 text-gray mb-2">回覆狀態</div>
            		  <asp:DropDownList ID="ddlReplyStatus_Type2" runat="server" CssClass="form-select">
            		  </asp:DropDownList>
            	  </div>
            	  <div class="col-12 col-lg-7">
            		  <div class="fs-16 text-gray mb-2">申請單位</div>
            		  <asp:DropDownList ID="ddlOrg_Type2" runat="server" CssClass="form-select">
            		  </asp:DropDownList>
            	  </div>
            	  <div class="col-12 col-lg-3">
            		  <div class="fs-16 text-gray mb-2">承辦人員</div>
            		  <asp:DropDownList ID="ddlSupervisor_Type2" runat="server" CssClass="form-select">
            		  </asp:DropDownList>
            	  </div>
              </div>

              <button type="button" id="btnSearch_Type2" class="btn btn-teal-dark d-table mx-auto" onclick="performAjaxSearch(2)">
                  🔍 查詢
              </button>
            </div>
        </div>
        <div class="block rounded-bottom-4">
          <div class="title border-teal">
        	  <div class="d-flex align-items-center gap-2">
        		  <h4 class="text-teal">
                      <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
        			  <span>列表</span>
        		  </h4>
        		  <span>共 <span class="text-teal" id="total-count-type2">0</span> 筆資料</span>
        	  </div>

        	  <div>
        		  <button class="btn btn-teal-dark" type="button" data-bs-toggle="modal" data-bs-target="#reviewResultModal">
        			  <i class="fas fa-list-ol"></i>
        			  審查結果排名
        		  </button>
        		  <button class="btn btn-teal-dark" type="button" onclick="exportApplicationPdfData()"><i class="fas fa-download"></i>匯出申請資料</button>
        	  </div>

          </div>



          <div class="table-responsive mb-0">
        	  <table class="table teal-table" id="DataTable_Type2">
        		  <thead>
        			  <tr>
        				  <th width="60">
        					  <input class="form-check-input check-teal checkAll" type="checkbox" name="" >
        				  </th>
        				  <th width="50">年度</th>
        				  <th width="100">
        					  <div class="hstack align-items-center justify-content-center">
        						  <span>類別</span>
        					  </div>
        				  </th>
        				  <th width="140">
        					  <div class="hstack align-items-center">
        						  <span>計畫編號</span>
        					  </div>
        				  </th>
        				  <th width="220">
        					  <div class="hstack align-items-center">
        						  <span>計畫名稱</span>
        					  </div>
        				  </th>
        				  <th>
        					  <div class="hstack align-items-center">
        						  <span>申請單位</span>
        					  </div>
        				  </th>
        				  <th width="150">
        					  <div class="hstack align-items-center justify-content-center">
        						  <span>計畫主題/審查組別</span>
        					  </div>
        				  </th>
        				  <th>
        					  <div class="hstack align-items-center justify-content-center">
        						  <span>審查進度</span>
        					  </div>
        				  </th>
        				  <th>
        					  <div class="hstack align-items-center justify-content-center">
        						  <span>回覆狀態</span>
        					  </div>
        				  </th>
        				  <th>
        					  <div class="hstack align-items-center justify-content-center">
        						  <span>承辦人員</span>
        					  </div>
        				  </th>
        				  <th>詳情</th>
        			  </tr>
        		  </thead>
        		  <tbody>
        			  <!-- 動態渲染的資料將會顯示在這裡 -->
        		  </tbody>
        	  </table>
          </div>

          <!-- 審查勾選後底部功能按鈕 -->
          <div class="bg-light-teal-100 mb-5 checkPlanBtnPanel checkPlanBtnPanel-type2" style="display: none;">
        	  <div class="p-3 d-flex justify-content-between align-items-start gap-3 flex-wrap">
        		  <div class="d-flex gap-3 flex-wrap">
        			  <button class="btn btn-royal-blue" type="button" onclick="handleSendReviewComments()"><i class="fa-solid fa-check"></i>審查意見 提送至申請者</button>
        			  <button class="btn btn-teal" type="button" onclick="openReviewerSetupModal('轉入下一階段')"><i class="fa-solid fa-check"></i>批次通過，轉入下一階段</button>
        			  <button class="btn btn-teal" type="button" onclick="handleBatchApproval('進入決審')"><i class="fa-solid fa-check"></i>批次通過，進入決審</button>
        			  <button class="btn btn-pink" type="button" onclick="handleBatchReject('批次不通過')"><i class="fa-solid fa-xmark"></i>批次不通過</button>
        		  </div>

        	  </div>
          </div>


          <!-- 分頁 -->
          <div id="pagination-type2" class="d-flex align-items-center justify-content-between flex-wrap gap-2 pagination-wrapper" data-review-type="2">
        	  <nav class="pagination justify-content-start" aria-label="Pagination">
        		  <button class="nav-button btn-prev-page" aria-label="Previous page" disabled>
        			  <i class="fas fa-chevron-left"></i>
        		  </button>

        		  <button class="nav-button btn-next-page" aria-label="Next page" disabled>
        			  <i class="fas fa-chevron-right"></i>
        		  </button>
        	  </nav>

        	  <div class="page-number-control">
        		  <div class="page-number-control-item">
        			  <span>跳到</span>
        			  <select class="form-select jump-to-page">
        				  <!-- 動態渲染頁數選項 -->
        			  </select>
        			  <span>頁</span>
        			  <span>,</span>
        		  </div>
        		  <div class="page-number-control-item">
        			  <span>每頁顯示</span>
        			  <select class="form-select page-size-selector">
        				  <option value="5">5</option>
        				  <option value="10" selected>10</option>
        				  <option value="20">20</option>
        				  <option value="50">50</option>
        				  <option value="100">100</option>
        			  </select>
        			  <span>筆</span>
        		  </div>
                  <div class="pagination-info ms-3 text-muted small">
                      <!-- 分頁資訊將顯示在這裡 -->
                  </div>
        	  </div>

          </div>
        </div>
    </div>

    <!-- 類型3：會議審查 -->
    <div id="content-type-3" class="review-content" style="display: none;">
        <!-- 搜尋表單 -->

       <div class="search bg-light-teal-100 rounded-0">
         <!-- 查詢表單 -->
             <div class="search-form" action="">
                 <div class="column-2">
                     <!-- 計畫編號或名稱關鍵字 -->
                     <div class="search-item">
                         <div class="fs-16 text-gray mb-2">計畫編號或名稱關鍵字</div>
                         <input type="text" name="txtKeyword_Type3" class="form-control" placeholder="請輸入計畫編號、計畫名稱相關文字">
                     </div>

                     <!-- 年度/類別/狀態 -->
                     <div class="row g-3">
                         <div class="col-12 col-lg-3">
                             <div class="fs-16 text-gray mb-2">年度</div>
                             <asp:DropDownList ID="ddlYear_Type3" runat="server" CssClass="form-select">
                             </asp:DropDownList>
                         </div>
                         <div class="col-12 col-lg-4">
                             <div class="fs-16 text-gray mb-2">類別</div>
                             <asp:DropDownList ID="ddlCategory_Type3" runat="server" CssClass="form-select">
                             </asp:DropDownList>
                         </div>
                         <div class="col-12 col-lg-5">
                             <div class="fs-16 text-gray mb-2">審查進度</div>
                             <asp:DropDownList ID="ddlProgress_Type3" runat="server" CssClass="form-select">
                             </asp:DropDownList>
                         </div>
                     </div>
                 </div>

                 <div class="row">
                     <div class="col-12 col-lg-2">
                         <div class="fs-16 text-gray mb-2">回覆狀態</div>
                         <asp:DropDownList ID="ddlReplyStatus_Type3" runat="server" CssClass="form-select">
                         </asp:DropDownList>
                     </div>
                     <div class="col-12 col-lg-7">
                         <div class="fs-16 text-gray mb-2">申請單位</div>
                         <asp:DropDownList ID="ddlOrg_Type3" runat="server" CssClass="form-select">
                         </asp:DropDownList>
                     </div>
                     <div class="col-12 col-lg-3">
                         <div class="fs-16 text-gray mb-2">承辦人員</div>
                         <asp:DropDownList ID="ddlSupervisor_Type3" runat="server" CssClass="form-select">
                         </asp:DropDownList>
                     </div>
                 </div>

                 <button type="button" id="btnSearch_Type3" class="btn btn-teal-dark d-table mx-auto" onclick="performAjaxSearch(3)">
                     🔍 查詢
                 </button>
             </div>
       </div>
       <!-- 列表內容 -->
         <div class="block rounded-bottom-4">
             <div class="title border-teal">
                 <div class="d-flex align-items-center gap-2">
                     <h4 class="text-teal">
                         <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
                         <span>列表</span>
                     </h4>
                     <span>共 <span class="text-teal" id="total-count-type3">0</span> 筆資料</span>
                 </div>

                 <div>
                     <button class="btn btn-teal-dark" type="button" data-bs-toggle="modal" data-bs-target="#reviewResultModal">
                         <i class="fas fa-list-ol"></i>
                         審查結果排名
                     </button>
                     <button class="btn btn-teal-dark" type="button" onclick="exportBatchPresentations()"><i class="fas fa-download"></i>批次匯出簡報</button>
                 </div>

             </div>



             <div class="table-responsive mb-0">
                 <table class="table teal-table">
                     <thead>
                         <tr>
                             <th width="60">
                                 <input class="form-check-input check-teal checkAll" type="checkbox" name="" >
                             </th>
                             <th width="50">年度</th>
                             <th width="100">
                                 <div class="hstack align-items-center justify-content-center">
                                     <span>類別</span>
                                 </div>
                             </th>
                             <th width="140">
                                 <div class="hstack align-items-center">
                                     <span>計畫編號</span>
                                 </div>
                             </th>
                             <th width="220">
                                 <div class="hstack align-items-center">
                                     <span>計畫名稱</span>
                                 </div>
                             </th>
                             <th>
                                 <div class="hstack align-items-center">
                                     <span>申請單位</span>
                                 </div>
                             </th>
                             <th width="150">
                                 <div class="hstack align-items-center justify-content-center">
                                     <span>計畫主題/審查組別</span>
                                 </div>
                             </th>
                             <th>
                                 <div class="hstack align-items-center justify-content-center">
                                     <span>審查進度</span>
                                 </div>
                             </th>
                             <th>
                                 <div class="hstack align-items-center justify-content-center">
                                     <span>回覆狀態</span>
                                 </div>
                             </th>
                             <th>
                                 <div class="hstack align-items-center justify-content-center">
                                     <span>承辦人員</span>
                                 </div>
                             </th>
                             <th>詳情</th>
                         </tr>
                     </thead>
                     <tbody>

                     </tbody>
                 </table>
             </div>

             <!-- 審查勾選後底部功能按鈕 -->
             <div class="bg-light-teal-100 mb-5 checkPlanBtnPanel checkPlanBtnPanel-type3" style="display: none;">
                 <div class="p-3 d-flex justify-content-between align-items-start gap-3 flex-wrap">
                     <div class="d-flex gap-3 flex-wrap">
                         <button class="btn btn-royal-blue" type="button" onclick="handleSendReviewComments()"><i class="fa-solid fa-check"></i>提送審查意見至申請者</button>
                         <button class="btn btn-teal" type="button" onclick="handleBatchApproval('轉入下一階段')"><i class="fa-solid fa-check"></i>批次通過，轉入下一階段</button>
                         <button class="btn btn-pink" type="button" onclick="handleBatchReject('批次不通過')"><i class="fa-solid fa-xmark"></i>批次不通過</button>
                     </div>


                 </div>
             </div>


             <!-- 分頁 -->
             <div id="pagination-type3" class="d-flex align-items-center justify-content-between flex-wrap gap-2 pagination-wrapper" data-review-type="3">
                 <nav class="pagination justify-content-start" aria-label="Pagination">
                     <button class="nav-button btn-prev-page" aria-label="Previous page" disabled>
                         <i class="fas fa-chevron-left"></i>
                     </button>

                     <button class="nav-button btn-next-page" aria-label="Next page" disabled>
                         <i class="fas fa-chevron-right"></i>
                     </button>
                 </nav>

                 <div class="page-number-control">
                     <div class="page-number-control-item">
                         <span>跳到</span>
                         <select class="form-select jump-to-page">
                             <!-- 動態渲染頁數選項 -->
                         </select>
                         <span>頁</span>
                         <span>,</span>
                     </div>
                     <div class="page-number-control-item">
                         <span>每頁顯示</span>
                         <select class="form-select page-size-selector">
                             <option value="5">5</option>
                             <option value="10" selected>10</option>
                             <option value="20">20</option>
                             <option value="50">50</option>
                             <option value="100">100</option>
                         </select>
                         <span>筆</span>
                     </div>
                     <div class="pagination-info ms-3 text-muted small">
                         <!-- 分頁資訊將顯示在這裡 -->
                     </div>
                 </div>

             </div>
         </div>
    </div>

    <!-- 類型4：決定審核清單 -->
    <div id="content-type-4" class="review-content" style="display: none;">
        <!-- 搜尋表單 -->
        <div class="search bg-light-teal-100 rounded-0">
          <!-- 查詢表單 -->
          <div class="search-form" action="">
              <div class="column-2">
                  <!-- 計畫編號或名稱關鍵字 -->
                  <div class="search-item">
                      <div class="fs-16 text-gray mb-2">計畫編號或名稱關鍵字</div>
                      <input type="text" name="txtKeyword_Type4" class="form-control" placeholder="請輸入計畫編號、計畫名稱相關文字">
                  </div>

                  <!-- 年度 -->
                  <div class="search-item">
                      <div class="fs-16 text-gray mb-2">年度</div>
                      <asp:DropDownList ID="ddlYear_Type4" runat="server" CssClass="form-select" ClientIDMode="Static">
                      </asp:DropDownList>
                  </div>
              </div>

              <div class="column-2">
                  <!-- 類別 -->
                  <div class="search-item">
                      <div class="fs-16 text-gray mb-2">類別</div>
                      <asp:DropDownList ID="ddlCategory_Type4" runat="server" CssClass="form-select" ClientIDMode="Static">
                      </asp:DropDownList>
                  </div>
                  <!-- 審查組別 -->
                  <div class="search-item">
                      <div class="fs-16 text-gray mb-2">審查組別</div>
                      <asp:DropDownList ID="ddlReviewGroup_Type4" runat="server" CssClass="form-select" ClientIDMode="Static" EnableEventValidation="false">
                      </asp:DropDownList>
                  </div>
              </div>

              <div class="column-2">
                  <div class="search-item">
                      <div class="fs-16 text-gray mb-2">申請單位</div>
                      <asp:DropDownList ID="ddlOrg_Type4" runat="server" CssClass="form-select" ClientIDMode="Static">
                      </asp:DropDownList>
                  </div>
                  <div class="search-item">
                      <div class="fs-16 text-gray mb-2">承辦人員</div>
                      <asp:DropDownList ID="ddlSupervisor_Type4" runat="server" CssClass="form-select" ClientIDMode="Static">
                      </asp:DropDownList>
                  </div>
              </div>

              <button type="button" id="btnSearch_Type4" class="btn btn-teal-dark d-table mx-auto" onclick="performType4Search()">
                  🔍 查詢
              </button>
          </div>
        </div>
        <!-- 列表內容 -->
        <div class="block rounded-bottom-4">
          <div class="title border-teal">
        	  <div class="d-flex align-items-center gap-2">
        		  <h4 class="text-teal">
                        <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
        			  <span>列表</span>
        		  </h4>
        		  <span>共 <span class="text-teal" id="total-count-type4">0</span> 筆資料</span>

        		  <button class="btn btn-sm btn-teal-dark mb-0" type="button" onclick="handleType4ApprovalSave()">
        			  <i class="fas fa-check"></i>
        			  儲存
        		  </button>
        		  <button class="btn btn-sm btn-outline-teal-dark mb-0" type="button" data-bs-toggle="modal" data-bs-target="#sortModeModal">
        			  <i class="fas fa-sort"></i>
        			  排序模式
        		  </button>
        	  </div>
        	  <button class="btn btn-teal-dark" type="button" onclick="exportType4ListData()"><i class="fas fa-download"></i>匯出列表資料</button>
          </div>

          <!-- 核定模式列表 -->
          <div class="approval-mode-table">
        	  <div class="table-responsive mb-0">
        		  <table class="table teal-table">
        			  <thead>
        				  <tr>
        					  <th width="60">
        						  <input class="form-check-input check-teal checkAll" type="checkbox" name="" >
        					  </th>
        					  <th>排序</th>
        					  <th width="50">年度</th>
        					  <th width="120">計畫編號</th>
        					  <th width="160">
        						  <div class="hstack align-items-center">
        							  <span>計畫名稱</span>
        						  </div>
        					  </th>
        					  <th class="text-start">
        						  <span>申請單位</span>
        					  </th>
        					  <th><span>總分</span></th>
        					  <th ><span>申請經費</span></th>
        					  <th>
        						  <div class="hstack align-items-center gap-1">
        							  <span>核定經費</span>
        							  <button type="button" class="btn-tooltip" data-bs-toggle="modal" data-bs-target="#payDetailModal">
        								  <i class="fa-solid fa-circle-info"></i>
        							  </button>
        						  </div>
        					  </th>
        					  <th  class="text-start">備註</th>
        					  <th class="text-center">修正<br>計畫書</th>
        				  </tr>
        			  </thead>
        			  <tbody>

        			  </tbody>
        		  </table>
        	  </div>

        	  <!-- 審查勾選後底部功能按鈕 -->
        	  <div class="bg-light-teal-100 mb-5 checkPlanBtnPanel checkPlanBtnPanel-type4" style="display: none;">
        		  <div class="p-3 d-flex justify-content-between align-items-start gap-3 flex-wrap">
        			  <div class="d-flex gap-3 flex-wrap">
        				  <button class="btn btn-royal-blue" type="button" onclick="handleSendReviewComments()"><i class="fa-solid fa-paper-plane"></i>提送審查意見至申請者</button>
        				  <button class="btn btn-royal-blue" type="button" onclick="handleSendToApplicant()"><i class="fa-solid fa-check"></i>提送申請者修正計畫書</button>
        				  <button class="btn btn-teal" type="button" onclick="handleBatchApproval('轉入計畫執行階段')"><i class="fa-solid fa-check"></i>批次核定完成，轉入計畫執行階段</button>
        				  <button class="btn btn-pink" type="button" onclick="handleBatchReject('批次不通過')"><i class="fa-solid fa-xmark"></i>批次不通過</button>
        			  </div>


        		  </div>
        	  </div>
          </div>

        </div>
    </div>

    <!-- 類型5：計畫變更審核 -->
    <div id="content-type-5" class="review-content" style="display: none;">
        <!-- 搜尋表單 -->
        <div class="search bg-light-teal-100 rounded-0">
          <!-- 查詢表單 -->
          <div class="search-form" action="">
              <!-- 第一行：年度、類別、申請單位、主管單位 -->
              <div class="row g-3">
                  <div class="col-12 col-lg-3">
                      <div class="fs-16 text-gray mb-2">年度</div>
                      <asp:DropDownList ID="ddlYear_Type5" runat="server" CssClass="form-select">

                      </asp:DropDownList>
                  </div>
                  <div class="col-12 col-lg-3">
                      <div class="fs-16 text-gray mb-2">類別</div>
                      <asp:DropDownList ID="ddlCategory_Type5" runat="server" CssClass="form-select">

                      </asp:DropDownList>
                  </div>
                  <div class="col-12 col-lg-3">
                      <div class="fs-16 text-gray mb-2">申請單位</div>
                      <asp:DropDownList ID="ddlOrg_Type5" runat="server" CssClass="form-select">
                      </asp:DropDownList>
                  </div>
                  <div class="col-12 col-lg-3">
                      <div class="fs-16 text-gray mb-2">主管單位</div>
                      <asp:DropDownList ID="ddlDepartment_Type5" runat="server" CssClass="form-select">
                      </asp:DropDownList>
                  </div>
              </div>

              <!-- 第二行：計畫編號或名稱關鍵字 -->
              <div class="search-item">
                  <div class="fs-16 text-gray mb-2">計畫編號或名稱關鍵字</div>
                  <input type="text" name="txtKeyword_Type5" class="form-control" placeholder="請輸入計畫編號、計畫名稱">
              </div>

              <!-- 查詢按鈕 -->
              <button type="button" id="btnSearch_Type5" class="btn btn-teal-dark d-table mx-auto" onclick="performAjaxSearch(5)">
                  <i class="fa-solid fa-magnifying-glass"></i>
                  查詢
              </button>
          </div>
        </div>

        <!-- 列表內容 -->
        <div class="block rounded-bottom-4">
          <div class="title border-teal">
              <div class="d-flex align-items-center gap-2">
                  <h4 class="text-teal">
                        <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
                      <span>列表</span>
                  </h4>
                  <span>共 <span class="text-teal" id="total-count-type5">0</span> 筆資料</span>
              </div>
        </div>

          <div class="table-responsive mb-0">
              <table class="table teal-table" id="DataTable_Type5">
                  <thead>
                      <tr>

                          <th width="80">年度</th>
                          <th width="140">
                              <span>計畫編號</span>
                          </th>
                          <th width="100">
                              <span>類別</span>
                          </th>
                          <th width="300">
                              <span>計畫名稱</span>
                          </th>
                          <th>
                              <span>申請單位</span>
                          </th>
                          <th width="120">操作</th>
                      </tr>
                  </thead>
                  <tbody>
                      <!-- 動態渲染的資料將會顯示在這裡 -->
                  </tbody>
              </table>
          </div>

          <!-- 分頁 -->
          <div id="pagination-type5" class="d-flex align-items-center justify-content-between flex-wrap gap-2 pagination-wrapper" data-review-type="5">
              <nav class="pagination justify-content-start" aria-label="Pagination">
                  <button class="nav-button btn-prev-page" aria-label="Previous page" disabled>
                      <i class="fas fa-chevron-left"></i>
                  </button>

                  <button class="nav-button btn-next-page" aria-label="Next page" disabled>
                      <i class="fas fa-chevron-right"></i>
                  </button>
              </nav>

              <div class="page-number-control">
                  <div class="page-number-control-item">
                      <span>跳到</span>
                      <select class="form-select jump-to-page">
                          <!-- 動態渲染頁數選項 -->
                      </select>
                      <span>頁</span>
                      <span>,</span>
                  </div>
                  <div class="page-number-control-item">
                      <span>每頁顯示</span>
                      <select class="form-select page-size-selector">
                          <option value="5">5</option>
                          <option value="10" selected>10</option>
                          <option value="20">20</option>
                          <option value="50">50</option>
                          <option value="100">100</option>
                      </select>
                      <span>筆</span>
                  </div>
                  <div class="pagination-info ms-3 text-muted small">
                      <!-- 分頁資訊將顯示在這裡 -->
                  </div>
              </div>
          </div>
        </div>
    </div>

    <!-- 類型6：執行計畫審核 -->
    <div id="content-type-6" class="review-content" style="display: none;">
        <!-- 搜尋表單 -->
        <div class="search bg-light-teal-100 rounded-0">
          <!-- 查詢表單 -->
          <div class="search-form" action="">
              <!-- 第一行：年度、類別、申請單位、主管單位 -->
              <div class="row g-3">
                  <div class="col-12 col-lg-3">
                      <div class="fs-16 text-gray mb-2">年度</div>
                      <asp:DropDownList ID="ddlYear_Type6" runat="server" CssClass="form-select">

                      </asp:DropDownList>
                  </div>
                  <div class="col-12 col-lg-3">
                      <div class="fs-16 text-gray mb-2">類別</div>
                      <asp:DropDownList ID="ddlCategory_Type6" runat="server" CssClass="form-select">
                      </asp:DropDownList>
                  </div>
                  <div class="col-12 col-lg-3">
                      <div class="fs-16 text-gray mb-2">申請單位</div>
                      <asp:DropDownList ID="ddlOrg_Type6" runat="server" CssClass="form-select">
                      </asp:DropDownList>
                  </div>
                  <div class="col-12 col-lg-3">
                      <div class="fs-16 text-gray mb-2">主管單位</div>
                      <asp:DropDownList ID="ddlDepartment_Type6" runat="server" CssClass="form-select">
                      </asp:DropDownList>
                  </div>
              </div>

              <!-- 第二行：計畫編號或名稱關鍵字 -->
              <div class="search-item">
                  <div class="fs-16 text-gray mb-2">計畫編號或名稱關鍵字</div>
                  <input type="text" name="txtKeyword_Type6" class="form-control" placeholder="請輸入計畫編號、計畫名稱">
              </div>

              <!-- 查詢按鈕 -->
              <button type="button" id="btnSearch_Type6" class="btn btn-teal-dark d-table mx-auto" onclick="performAjaxSearch(6)">
                  <i class="fa-solid fa-magnifying-glass"></i>
                  查詢
              </button>
          </div>
        </div>

        <!-- 列表內容 -->
        <div class="block rounded-bottom-4">
          <div class="title border-teal">
              <div class="d-flex align-items-center gap-2">
                  <h4 class="text-teal">
                        <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
                      <span>列表</span>
                  </h4>
                  <span>共 <span class="text-teal" id="total-count-type6">0</span> 筆資料</span>
              </div>
        </div>

          <div class="table-responsive mb-0">
              <table class="table teal-table" id="DataTable_Type6">
                  <thead>
                      <tr>

                          <th width="50">年度</th>
                          <th width="100">
                              <span>類別</span>
                          </th>
                          <th width="140">
                              <span>計畫編號</span>
                          </th>
                          <th width="300">
                              <span>計畫名稱</span>
                          </th>
                          <th>
                              <span>申請單位</span>
                          </th>
                          <th width="150">
                              <div class="hstack align-items-center">
                                  <span>待審項目</span>
                              </div>
                          </th>
                          <th width="130" class="review-progress-header" >
                              <div class="hstack align-items-center justify-content-center">
                                  <span>審查委員進度</span>
                              </div>
                          </th>
                          <th width="120">功能</th>
                      </tr>
                  </thead>
                  <tbody>
                      <!-- 動態渲染的資料將會顯示在這裡 -->
                  </tbody>
              </table>
          </div>



          <!-- 分頁 -->
          <div id="pagination-type6" class="d-flex align-items-center justify-content-between flex-wrap gap-2 pagination-wrapper" data-review-type="6">
              <nav class="pagination justify-content-start" aria-label="Pagination">
                  <button class="nav-button btn-prev-page" aria-label="Previous page" disabled>
                      <i class="fas fa-chevron-left"></i>
                  </button>

                  <button class="nav-button btn-next-page" aria-label="Next page" disabled>
                      <i class="fas fa-chevron-right"></i>
                  </button>
              </nav>

              <div class="page-number-control">
                  <div class="page-number-control-item">
                      <span>跳到</span>
                      <select class="form-select jump-to-page">
                          <!-- 動態渲染頁數選項 -->
                      </select>
                      <span>頁</span>
                      <span>,</span>
                  </div>
                  <div class="page-number-control-item">
                      <span>每頁顯示</span>
                      <select class="form-select page-size-selector">
                          <option value="5">5</option>
                          <option value="10" selected>10</option>
                          <option value="20">20</option>
                          <option value="50">50</option>
                          <option value="100">100</option>
                      </select>
                      <span>筆</span>
                  </div>
                  <div class="pagination-info ms-3 text-muted small">
                      <!-- 分頁資訊將顯示在這裡 -->
                  </div>
              </div>
          </div>
        </div>
    </div>

     <div class="modal fade" id="planDetailModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="planDetailModalLabel" aria-hidden="true">
           <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
               <div class="modal-content">
                   <div class="modal-header">
                       <h4 class="fs-24 fw-bold text-green-light">審查結果與意見回覆 - 實質審查/初審</h4>
                       <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                           <i class="fa-solid fa-circle-xmark"></i>
                       </button>
                   </div>
                   <div class="modal-body">

                       <div class="bg-light-gray p-3 mb-4">
                           <ul class="lh-lg">
                               <li>
                                   <span class="text-gray">年度 :</span>
                                   <span>114</span>
                               </li>
                               <li>
                                   <span class="text-gray">計畫編號 :</span>
                                   <span>1140023</span>
                               </li>
                               <li>
                                   <span class="text-gray">計畫類別 :</span>
                                   <span>114年度補助學術機構、研究機關(構)及海洋科技</span>
                               </li>
                               <li>
                                   <span class="text-gray">主題、領域 :</span>
                                   <span>海洋永續、環境工程</span>
                               </li>
                               <li>
                                   <span class="text-gray">計畫名稱 : </span>
                                   <span>海洋環境監測與預警系統建置計畫</span>
                               </li>
                               <li>
                                   <span class="text-gray">申請單位 : </span>
                                   <span>國家海洋研究院環境監測中心</span>
                               </li>
                           </ul>
                       </div>


                       <div class="table-responsive">
                           <table class="table align-middle gray-table lh-base">
                               <thead>
                                   <tr>
                                       <th>審查委員</th>
                                       <th>評分</th>
                                       <th>審查意見</th>
                                       <th>申請單位回覆</th>
                                   </tr>
                               </thead>
                               <tbody>
                                   <tr>
                                       <td>廖XXX</td>
                                       <td class="text-center">90</td>
                                       <td>意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見</td>
                                       <td>申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆</td>
                                   </tr>
                               </tbody>
                           </table>
                       </div>


                       <div class="d-flex justify-content-center mt-4 gap-4">
                           <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                               取消
                           </button>
                           <button type="button" class="btn btn-teal" onclick="exportReviewCommentsComparison()">
                               匯出歷次審查結果及回覆對照表
                           </button>
                       </div>
                   </div>

               </div>
           </div>
       </div>

    <!-- 排序模式 Modal -->
    <div class="modal fade" id="sortModeModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="sortModeModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-teal-dark">
                        <i class="fas fa-sort me-2"></i>
                        排序模式 - 決定審核清單
                    </h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <!-- 搜尋表單 -->
                    <div class="search bg-light-teal-100 rounded-3 mb-4">
                        <div class="search-form">
                            <div class="row g-3">
                                <div class="col-12 col-md-4">
                                    <div class="fs-16 text-gray mb-2">年度</div>
                                    <select id="sortingYear" class="form-select">
                                        <option value="">載入中...</option>
                                    </select>
                                </div>
                                <div class="col-12 col-md-4">
                                    <div class="fs-16 text-gray mb-2">類別</div>
                                    <select id="sortingCategory" class="form-select" >
                                        <option value="SCI">科專</option>
                                        <option value="CUL">文化</option>
                                        <option value="EDC">學校民間</option>
                                        <option value="CLB">學校社團</option>
                                        <option value="MUL">多元</option>
                                        <option value="LIT">素養</option>
                                        <option value="ACC">無障礙</option>
                                    </select>
                                </div>
                                <div class="col-12 col-md-4 d-flex align-items-end">
                                    <button id="btnSearchSorting" class="btn btn-teal-dark w-100" type="button" onclick="ReviewChecklist.searchSortingMode()">
                                        <i class="fas fa-search me-1"></i>查詢
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- 排序列表 -->
                    <div class="block rounded-4">
                        <div class="title border-teal">
                            <div class="d-flex align-items-center gap-2">
                                <h5 class="text-teal">
                                    <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
                                    <span>排序列表</span>
                                </h5>
                                <span id="sortingResultCount" class="text-teal">共 0 筆資料</span>
                            </div>
                            <button id="btnSaveSorting" class="btn btn-teal-dark" type="button" onclick="ReviewChecklist.saveSortingMode()">
                                <i class="fas fa-save me-1"></i>儲存排序
                            </button>
                        </div>

                        <div class="table-responsive mb-0">
                            <table class="table teal-table" id="sortingTable">
                                <thead>
                                    <tr>
                                        <th>排序</th>
                                        <th width="120">計畫編號</th>
                                        <th width="200">計畫名稱</th>
                                        <th>申請單位</th>
                                        <th width="120">審查組別</th>
                                        <th width="80">總分</th>
                                        <th width="200">備註</th>
                                        <th width="100">功能</th>
                                    </tr>
                                </thead>
                                <tbody id="sortingTableBody">
                                    <!-- 排序資料將在這裡動態載入 -->
                                </tbody>
                            </table>
                        </div>
                    </div>

                    <!-- 使用說明 -->
                    <div class="alert alert-info mt-3">
                        <div class="d-flex align-items-start">
                            <i class="fas fa-info-circle me-2 mt-1"></i>
                            <div>
                                <strong>使用說明：</strong>
                                <ul class="mb-0 mt-2">
                                    <li>使用 <i class="fas fa-arrows-alt text-teal"></i> 按鈕拖曳調整順序</li>
                                    <li>使用 <i class="fas fa-arrow-up text-teal"></i> 按鈕將項目置頂</li>
                                    <li>可以修改備註欄位內容</li>
                                    <li>完成排序後請點擊「儲存排序」按鈕</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
<!--  modal 資訊系統-計畫審查-決審核定-詳情 -->
  <div class="modal fade" id="payDetailModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="payDetailModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
          <div class="modal-content">
              <div class="modal-header">
                  <h4 class="fs-24 fw-bold text-green-light">補助上限說明</h4>
                  <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                      <i class="fa-solid fa-circle-xmark"></i>
                  </button>
              </div>
              <div class="modal-body">

                  <div class="table-responsive">
                      <table class="table align-middle gray-table lh-base">
                          <thead>
                              <tr>
                                  <th width="180">類別</th>
                                  <th>對象</th>
                                  <th>補助上限</th>
                              </tr>
                          </thead>
                          <tbody>
                              <tr>
                                  <td>科專</td>
                                  <td>學術及研究機關(構) / 學界科專
                                      學術及研究機關(構) / 法人科專
                                      海洋科技業者</td>
                                  <td>500萬</td>
                              </tr>
                              <tr>
                                  <td>文化</td>
                                  <td>公立之博物館及社教館所</td>
                                  <td>200萬</td>
                              </tr>
                              <tr>
                                  <td>文化</td>
                                  <td>公私立學校、大專校院</td>
                                  <td>100萬</td>
                              </tr>
                              <tr>
                                  <td>文化</td>
                                  <td>財團法人、社團法人及其他人民團體\</td>
                                  <td>50萬</td>
                              </tr>
                              <tr>
                                  <td>學校.民間</td>
                                  <td>學校</td>
                                  <td>2萬</td>
                              </tr>
                              <tr>
                                  <td>學校.民間</td>
                                  <td>民間團體</td>
                                  <td>2萬</td>
                              </tr>
                              <tr>
                                  <td>學校社團</td>
                                  <td>創社</td>
                                  <td>5萬</td>
                              </tr>
                              <tr>
                                  <td>學校社團</td>
                                  <td>社務活動</td>
                                  <td>3萬</td>
                              </tr>
                              <tr>
                                  <td>學校社團</td>
                                  <td>公共活動費</td>
                                  <td>2萬</td>
                              </tr>
                          </tbody>
                      </table>
                  </div>

              </div>

          </div>
      </div>
  </div>

  <!-- 審查結果排名 Modal -->
  <div class="modal fade" id="reviewResultModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="reviewResultModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
          <div class="modal-content">
              <div class="modal-header">
                  <h4 class="fs-24 fw-bold text-green-light">審查結果排名</h4>
                  <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                      <i class="fa-solid fa-circle-xmark"></i>
                  </button>
              </div>
              <div class="modal-body">
                  <!-- 載入中顯示 -->
                  <div id="rankingLoading" class="text-center py-5">
                      <div class="spinner-border text-teal" role="status">
                          <span class="visually-hidden">載入中...</span>
                      </div>
                      <div class="mt-2">載入審查結果排名中...</div>
                  </div>

                  <!-- 排名內容 -->
                  <div id="rankingContent" style="display: none;">
                      <!-- 審查組別選擇 -->
                      <div class="d-flex align-items-center gap-3 mb-4">
                          <span class="text-gray text-nowrap">審查組別</span>
                          <select class="form-select" id="reviewGroupSelect" style="width: 200px;">
                          </select>
                      </div>

                      <!-- 排名表格 -->
                      <div class="table-responsive">
                          <table class="table align-middle gray-table lh-base" id="reviewRankingTable">
                              <thead id="rankingTableHead">
                                  <!-- 動態產生的表頭 -->
                              </thead>
                              <tbody id="reviewRankingTableBody">
                                  <!-- 動態渲染的排名資料 -->
                              </tbody>
                          </table>
                      </div>
                  </div>

                  <!-- 錯誤訊息 -->
                  <div id="rankingError" class="alert alert-danger" style="display: none;">
                      <i class="fas fa-exclamation-triangle me-2"></i>
                      <span id="rankingErrorMessage">載入排名資料時發生錯誤</span>
                  </div>
              </div>
              <div class="modal-footer">
                  <button type="button" class="btn btn-gray" data-bs-dismiss="modal">取消</button>
                  <button type="button" class="btn btn-teal" id="btnExportRanking">匯出審查結果</button>
              </div>
          </div>
      </div>
  </div>

  <!-- Modal 設置審查人員 -->
  <div class="modal fade" id="reviewerSetupModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="reviewerSetupModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
          <div class="modal-content">
              <div class="modal-header">
                  <div>
                      <h4 class="fs-24 fw-bold text-green-light">設置審查人員</h4>
                      <p class="text-muted mb-0" style="font-size: 14px;">若專案無須設置審查人員，則請直接進行送出。</p>
                  </div>
                  <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                      <i class="fa-solid fa-circle-xmark"></i>
                  </button>
              </div>
              <div class="modal-body">
                  <!-- 設置審查人員開關 -->
                  <div class="mb-4">
                      <div class="form-check">
                          <input class="form-check-input check-teal" type="checkbox" id="chkEnableReviewerSetup" onchange="toggleReviewerSetup()">
                          <label class="form-check-label fs-16" for="chkEnableReviewerSetup">
                              需邀請審查委員進行審查（目前僅科專類、文化類）
                          </label>
                      </div>
                  </div>

                  <!-- 審查人員設置區域 (預設隱藏) -->
                  <div id="reviewerSetupArea" style="display: none;">
                      <!-- 領域選擇 -->
                      <div class="mb-4">
                          <div class="fs-16 text-gray mb-2">選擇審查組別</div>
                          <select id="ddlSubjectType" class="form-select" onchange="loadReviewersBySubject()">
                              <option value="ALL">全部</option>
                          </select>
                      </div>

                      <!-- 雙清單選擇器 -->
                      <div class="row g-3">
                          <!-- A清單：可選擇的審查委員 -->
                          <div class="col-12 col-md-5">
                              <div class="d-flex justify-content-between align-items-center mb-2">
                                  <span class="fs-16 text-gray">可選擇的審查委員</span>
                                  <span class="badge bg-teal" id="availableCount">0</span>
                              </div>
                              <!-- 搜尋框 -->
                              <div class="mb-2">
                                  <input type="text" id="availableReviewersSearch" class="form-control form-control-sm" placeholder="搜尋姓名或Email...">
                              </div>
                              <div class="border rounded p-2" style="height: 360px; overflow-y: auto;">
                                  <div id="availableReviewersList">
                                      <div class="text-center text-muted py-5">
                                          請先選擇主題
                                      </div>
                                  </div>
                              </div>
                          </div>

                          <!-- 中間操作按鈕 -->
                          <div class="col-12 col-md-2 d-flex flex-column justify-content-center align-items-center gap-2">
                              <button type="button" class="btn btn-teal" onclick="addSelectedReviewers()" title="加入">
                                  <i class="fa-solid fa-chevron-right"></i>
                              </button>
                              <button type="button" class="btn btn-pink" onclick="removeSelectedReviewers()" title="移除">
                                  <i class="fa-solid fa-chevron-left"></i>
                              </button>
                          </div>

                          <!-- B清單：已選擇的審查委員 -->
                          <div class="col-12 col-md-5">
                              <div class="d-flex justify-content-between align-items-center mb-2">
                                  <span class="fs-16 text-gray">已選擇的審查委員</span>
                                  <span class="badge bg-pink" id="selectedCount">0</span>
                              </div>
                              <div class="border rounded p-2" style="height: 400px; overflow-y: auto;">
                                  <div id="selectedReviewersList">
                                      <div class="text-center text-muted py-5">
                                          尚未選擇審查委員
                                      </div>
                                  </div>
                              </div>
                          </div>
                      </div>
                  </div>
              </div>
              <div class="modal-footer">
                  <button type="button" class="btn btn-gray" data-bs-dismiss="modal">取消</button>
                  <button type="button" class="btn btn-teal" onclick="confirmReviewerSetup()">確定轉入下一階段</button>
              </div>
          </div>
      </div>
  </div>

    <script>
        startVueApp("#news-marquee");

        // 審查人員設置相關變數
        var availableReviewers = []; // A清單：可選擇的審查委員
        var selectedReviewers = [];  // B清單：已選擇的審查委員
        var currentActionText = '';  // 當前操作文字

        // 開啟設置審查人員 Modal
        function openReviewerSetupModal(actionText) {
            // 儲存操作文字（預設為「轉入下一階段」）
            currentActionText = actionText || '轉入下一階段';

            // 重置清單
            availableReviewers = [];
            selectedReviewers = [];

            // 重置 checkbox 狀態
            $('#chkEnableReviewerSetup').prop('checked', false);
            $('#reviewerSetupArea').hide();

            // 清空下拉選單
            $('#ddlSubjectType').empty().append('<option value="">請選擇審查組別</option>');

            // 清空搜尋框
            $('#availableReviewersSearch').val('');

            // 清空 A清單
            $('#availableReviewersList').html('<div class="text-center text-muted py-5">請先選擇審查組別</div>');
            $('#availableCount').text('0');

            // 清空 B清單
            $('#selectedReviewersList').html('<div class="text-center text-muted py-5">尚未選擇審查委員</div>');
            $('#selectedCount').text('0');

            // 綁定搜尋框事件
            $('#availableReviewersSearch').off('input').on('input', function() {
                renderAvailableReviewers();
            });

            // 載入領域下拉選單
            loadSubjectTypes();

            // 顯示 modal
            var modal = new bootstrap.Modal(document.getElementById('reviewerSetupModal'));
            modal.show();
        }

        // 切換審查人員設置區域的顯示/隱藏
        function toggleReviewerSetup() {
            var isChecked = $('#chkEnableReviewerSetup').is(':checked');

            if (isChecked) {
                $('#reviewerSetupArea').slideDown(300);
            } else {
                $('#reviewerSetupArea').slideUp(300);

                // 隱藏時清空已選擇的審查委員
                selectedReviewers = [];
                renderSelectedReviewers();

                // 重置領域下拉選單
                $('#ddlSubjectType').val('');

                // 清空搜尋框
                $('#availableReviewersSearch').val('');

                // 清空 A清單
                $('#availableReviewersList').html('<div class="text-center text-muted py-5">請先選擇審查組別</div>');
                $('#availableCount').text('0');
            }
        }

        // 載入領域下拉選單
        function loadSubjectTypes() {
            $.ajax({
                type: "POST",
                url: "ReviewChecklist.aspx/GetSubjectTypes",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    var result = JSON.parse(response.d);
                    if (result.success) {
                        var ddl = $('#ddlSubjectType');
                        ddl.empty();
                        ddl.append('<option value="">請選擇審查組別</option>');
                        ddl.append('<option value="ALL">全部</option>');

                        result.data.forEach(function(item) {
                            ddl.append('<option value="' + item.code + '">' + item.name + '</option>');
                        });
                    } else {
                        Swal.fire('錯誤', result.message, 'error');
                    }
                },
                error: function(xhr, status, error) {
                    Swal.fire('錯誤', '載入領域清單失敗', 'error');
                }
            });
        }

        // 根據領域載入審查委員
        function loadReviewersBySubject() {
            var subjectCode = $('#ddlSubjectType').val();

            if (!subjectCode) {
                $('#availableReviewersList').html('<div class="text-center text-muted py-5">請先選擇審查組別</div>');
                $('#availableCount').text('0');
                availableReviewers = [];
                return;
            }

            $.ajax({
                type: "POST",
                url: "ReviewChecklist.aspx/GetReviewersBySubject",
                data: JSON.stringify({ subjectCode: subjectCode }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    var result = JSON.parse(response.d);
                    if (result.success) {
                        availableReviewers = result.data;
                        renderAvailableReviewers();
                    } else {
                        Swal.fire('錯誤', result.message, 'error');
                    }
                },
                error: function(xhr, status, error) {
                    Swal.fire('錯誤', '載入審查委員清單失敗', 'error');
                }
            });
        }

        // 渲染 A清單（可選擇的審查委員）
        function renderAvailableReviewers(searchKeyword) {
            var html = '';
            var keyword = searchKeyword || $('#availableReviewersSearch').val() || '';
            keyword = keyword.toLowerCase().trim();

            if (availableReviewers.length === 0) {
                html = '<div class="text-center text-muted py-5">此領域無可選擇的審查委員</div>';
            } else {
                var filteredReviewers = availableReviewers.filter(function(reviewer) {
                    // 檢查是否已在 B清單中
                    var isSelected = selectedReviewers.some(r => r.account === reviewer.account);
                    if (isSelected) return false;

                    // 如果有搜尋關鍵字，進行篩選
                    if (keyword) {
                        var account = (reviewer.account || '').toLowerCase();
                        var name = (reviewer.name || '').toLowerCase();
                        var displayName = (reviewer.displayName || '').toLowerCase();

                        return account.includes(keyword) ||
                               name.includes(keyword) ||
                               displayName.includes(keyword);
                    }

                    return true;
                });

                filteredReviewers.forEach(function(reviewer) {
                    html += '<div class="form-check mb-2">';
                    html += '<input class="form-check-input check-teal" type="checkbox" value="' + reviewer.account + '" id="avail_' + reviewer.account + '">';
                    html += '<label class="form-check-label" for="avail_' + reviewer.account + '">';
                    html += reviewer.displayName;
                    html += '</label>';
                    html += '</div>';
                });

                if (html === '') {
                    if (keyword) {
                        html = '<div class="text-center text-muted py-5">查無符合搜尋條件的審查委員</div>';
                    } else {
                        html = '<div class="text-center text-muted py-5">所有審查委員已被選擇</div>';
                    }
                }
            }

            $('#availableReviewersList').html(html);
            updateAvailableCount();
        }

        // 渲染 B清單（已選擇的審查委員）
        function renderSelectedReviewers() {
            var html = '';

            if (selectedReviewers.length === 0) {
                html = '<div class="text-center text-muted py-5">尚未選擇審查委員</div>';
            } else {
                selectedReviewers.forEach(function(reviewer) {
                    html += '<div class="form-check mb-2">';
                    html += '<input class="form-check-input check-pink" type="checkbox" value="' + reviewer.account + '" id="sel_' + reviewer.account + '">';
                    html += '<label class="form-check-label" for="sel_' + reviewer.account + '">';
                    html += reviewer.displayName;
                    html += '</label>';
                    html += '</div>';
                });
            }

            $('#selectedReviewersList').html(html);
            $('#selectedCount').text(selectedReviewers.length);
        }

        // 更新 A清單計數
        function updateAvailableCount() {
            var count = availableReviewers.filter(function(reviewer) {
                return !selectedReviewers.some(r => r.account === reviewer.account);
            }).length;
            $('#availableCount').text(count);
        }

        // 加入選中的審查委員到 B清單
        function addSelectedReviewers() {
            var checkedBoxes = $('#availableReviewersList input[type="checkbox"]:checked');

            if (checkedBoxes.length === 0) {
                Swal.fire('提示', '請先勾選要加入的審查委員', 'info');
                return;
            }

            checkedBoxes.each(function() {
                var account = $(this).val();
                var reviewer = availableReviewers.find(r => r.account === account);

                if (reviewer && !selectedReviewers.some(r => r.account === account)) {
                    selectedReviewers.push(reviewer);
                }
            });

            renderAvailableReviewers();
            renderSelectedReviewers();
        }

        // 從 B清單移除選中的審查委員
        function removeSelectedReviewers() {
            var checkedBoxes = $('#selectedReviewersList input[type="checkbox"]:checked');

            if (checkedBoxes.length === 0) {
                Swal.fire('提示', '請先勾選要移除的審查委員', 'info');
                return;
            }

            var accountsToRemove = [];
            checkedBoxes.each(function() {
                accountsToRemove.push($(this).val());
            });

            selectedReviewers = selectedReviewers.filter(function(reviewer) {
                return !accountsToRemove.includes(reviewer.account);
            });

            renderAvailableReviewers();
            renderSelectedReviewers();
        }

        // 確認設置完成，執行原本的批次通過功能
        function confirmReviewerSetup() {
            // 取得當前審查類型
            var currentType = window.ReviewChecklist.getCurrentType();

            // 檢查是否有選中的專案
            var currentContent = $('#content-type-' + currentType);
            var selectedCheckboxes = currentContent.find('.checkPlan:checked');

            // 檢查選中的專案中是否包含文化或科專
            var hasCulOrSci = false;
            selectedCheckboxes.each(function() {
                var projectId = $(this).val();
                if (projectId.includes('CUL') || projectId.includes('SCI')) {
                    hasCulOrSci = true;
                    return false; // 跳出 each 迴圈
                }
            });

            // 準備審查人員清單資料
            var reviewerList = selectedReviewers.map(function(reviewer) {
                return {
                    account: reviewer.account,
                    name: reviewer.name
                };
            });

            console.log('已選擇的審查委員:', reviewerList);
            console.log('操作類型:', currentActionText);
            console.log('是否包含文化或科專:', hasCulOrSci);

            // 如果有文化或科專專案，但沒有選擇審查委員，顯示提示
            if (hasCulOrSci && reviewerList.length === 0) {
                Swal.fire({
                    title: '提示',
                    text: '您選擇的專案中包含文化或科專項目，建議設置審查人員。確定要繼續嗎？',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: '確定繼續',
                    cancelButtonText: '取消',
                    confirmButtonColor: '#26A69A',
                    cancelButtonColor: '#d33'
                }).then((result) => {
                    if (result.isConfirmed) {
                        // 使用者確認繼續
                        proceedWithBatchApproval(reviewerList);
                    }
                    // 如果取消，不關閉 modal，讓使用者可以繼續選擇審查人員
                });
            } else {
                // 沒有文化或科專，或已選擇審查委員，直接執行
                proceedWithBatchApproval(reviewerList);
            }
        }

        // 執行批次通過處理
        function proceedWithBatchApproval(reviewerList) {
            // 關閉 modal
            var modal = bootstrap.Modal.getInstance(document.getElementById('reviewerSetupModal'));
            modal.hide();

            // 執行原本的批次通過功能，並傳遞審查人員清單
            handleBatchApproval(currentActionText, reviewerList);
        }
    </script>
</asp:Content>
