<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReviewChecklist.aspx.cs" Inherits="OFS_ReviewChecklist" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFSMaster.master" EnableViewState="true" EnableEventValidation="false" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="<%= ResolveUrl("~/script/OFS/ReviewChecklist.js") %>"></script>

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- 隱藏欄位和按鈕 -->
    <asp:HiddenField ID="hdnSelectedProjectIds" runat="server" ClientIDMode="Static"/>
    <asp:Button ID="btnSendToApplicant" runat="server" Text="提送至申請者" 
                OnClick="btnSendToApplicant_Click" Style="display: none;" ClientIDMode="Static"/>
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
    
    <!-- 公告提醒 -->
    <div class="notice">
        <div class="notice-content">
            <h3 class="notice-title">114/12/31 條款正式上線</h3>
        </div>
        <div class="notice-action">
            <a href="#" class="btn-link">全部公告</a>
        </div>
    </div>
    
    <!-- 總計列表 -->
    <ul class="total-list mt-4">
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
                <div class="total-item-title">領域審查/初審</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-3">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(3)">
                <div class="total-item-title">技術審查/複審</div>
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
	                       <asp:DropDownList ID="ddlStage_Type1" runat="server" CssClass="form-select">
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
                                <div class="hstack align-items-center justify-content-center">
                                    <span>類別</span>
            
                                    <!-- 排序按鈕： -->
                                    <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                    <button class="sort down">
                                        <i class="fa-solid fa-sort-up"></i>
                                        <i class="fa-solid fa-sort-down"></i>
                                    </button>
                                </div>
                            </th>
                            <th width="140">
                                <div class="hstack align-items-center">
                                    <span>計畫編號</span>
            
                                    <!-- 排序按鈕： -->
                                    <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                    <button class="sort up">
                                        <i class="fa-solid fa-sort-up"></i>
                                        <i class="fa-solid fa-sort-down"></i>
                                    </button>
                                </div>
                            </th>
                            <th width="220">
                                <div class="hstack align-items-center">
                                    <span>計畫名稱</span>
            
                                    <!-- 排序按鈕： -->
                                    <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                    <button class="sort">
                                        <i class="fa-solid fa-sort-up"></i>
                                        <i class="fa-solid fa-sort-down"></i>
                                    </button>
                                </div>
                            </th>
                            <th>
                                <div class="hstack align-items-center">
                                    <span>申請單位</span>
            
                                    <!-- 排序按鈕： -->
                                <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                  <button class="sort">
                                        <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                  </button>
                                </div>
                            </th>
                            <th width="150">
                                <div class="hstack align-items-center justify-content-center">
                                <span>申請經費</span>
          
                                    <!-- 排序按鈕： -->
                                    <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                    <button class="sort">
                                        <i class="fa-solid fa-sort-up"></i>
                                        <i class="fa-solid fa-sort-down"></i>
                                    </button>
                                </div>
                            </th>
                            <th>
                                <div class="hstack align-items-center justify-content-center">
                                    <span>狀態</span>
            
                                    <!-- 排序按鈕： -->
                                    <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                    <button class="sort">
                                        <i class="fa-solid fa-sort-up"></i>
                                        <i class="fa-solid fa-sort-down"></i>
                                    </button>
                                </div>
                            </th>
                            <th>
                                <div class="hstack align-items-center justify-content-center">
                                    <span>補正件期限</span>
                
                                    <!-- 排序按鈕： -->
                                    <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                    <button class="sort">
                                        <i class="fa-solid fa-sort-up"></i>
                                        <i class="fa-solid fa-sort-down"></i>
                                    </button>
                                </div>
                            </th>
                            <th>
                                <div class="hstack align-items-center justify-content-center">
                                    <span>承辦人員</span>
            
                                    <!-- 排序按鈕： -->
                                    <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                    <button class="sort">
                                        <i class="fa-solid fa-sort-up"></i>
                                        <i class="fa-solid fa-sort-down"></i>
                                    </button>
                                </div>
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
                        <button class="btn btn-teal" type="button" onclick="handleBatchApproval('轉入下一階段')"><i class="fa-solid fa-check"></i>批次通過，轉入下一階段</button>
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
    <!-- 類型2：領域審查/初審 -->
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
        						  <span>審查組別</span>
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
        			  <button class="btn btn-royal-blue" type="button" onclick="handleSendToApplicantType2Type3()"><i class="fa-solid fa-check"></i>提送至申請者</button>
        			  <button class="btn btn-teal" type="button" onclick="handleBatchApproval('轉入下一階段')"><i class="fa-solid fa-check"></i>批次通過，轉入下一階段</button>
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
                                     <span>審查組別</span>
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
                         <button class="btn btn-royal-blue" type="button" onclick="handleSendToApplicantType2Type3()"><i class="fa-solid fa-check"></i>提送至申請者</button>
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
                  
                  <!-- 類別/審查組別 -->
                  <div class="row g-3">
                      <div class="col-12 col-lg-6">
                          <div class="fs-16 text-gray mb-2">類別</div>
                          <asp:DropDownList ID="ddlCategory_Type4" runat="server" CssClass="form-select" ClientIDMode="Static">
                          </asp:DropDownList>
                      </div>
                      <div class="col-12 col-lg-6">
                          <div class="fs-16 text-gray mb-2">審查組別</div>
                          <asp:DropDownList ID="ddlReviewGroup_Type4" runat="server" CssClass="form-select" ClientIDMode="Static" EnableEventValidation="false">
                          </asp:DropDownList>
                      </div>
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
        		  <span>共 <span class="text-teal">3</span> 筆資料</span>
        
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
                              <div class="hstack align-items-center">
                                  <span>計畫編號</span>
                
                                  <!-- 排序按鈕： -->
                                  <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                  <button class="sort up">
                                      <i class="fa-solid fa-sort-up"></i>
                                      <i class="fa-solid fa-sort-down"></i>
                                  </button>
                              </div>
                          </th>
                          <th width="100">
                              <div class="hstack align-items-center justify-content-center">
                                  <span>類別</span>
                
                                  <!-- 排序按鈕： -->
                                  <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                  <button class="sort down">
                                      <i class="fa-solid fa-sort-up"></i>
                                      <i class="fa-solid fa-sort-down"></i>
                                  </button>
                              </div>
                          </th>
                          <th width="300">
                              <div class="hstack align-items-center">
                                  <span>計畫名稱</span>
                
                                  <!-- 排序按鈕： -->
                                  <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                  <button class="sort">
                                      <i class="fa-solid fa-sort-up"></i>
                                      <i class="fa-solid fa-sort-down"></i>
                                  </button>
                              </div>
                          </th>
                          <th>
                              <div class="hstack align-items-center">
                                  <span>申請單位</span>
                
                                  <!-- 排序按鈕： -->
                              <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                <button class="sort">
                                      <i class="fa-solid fa-sort-up"></i>
                                  <i class="fa-solid fa-sort-down"></i>
                                </button>
                              </div>
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
                              <div class="hstack align-items-center justify-content-center">
                                  <span>類別</span>
                
                                  <!-- 排序按鈕： -->
                                  <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                  <button class="sort down">
                                      <i class="fa-solid fa-sort-up"></i>
                                      <i class="fa-solid fa-sort-down"></i>
                                  </button>
                              </div>
                          </th>
                          <th width="140">
                              <div class="hstack align-items-center">
                                  <span>計畫編號</span>
                
                                  <!-- 排序按鈕： -->
                                  <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                  <button class="sort up">
                                      <i class="fa-solid fa-sort-up"></i>
                                      <i class="fa-solid fa-sort-down"></i>
                                  </button>
                              </div>
                          </th>
                          <th width="300">
                              <div class="hstack align-items-center">
                                  <span>計畫名稱</span>
                
                                  <!-- 排序按鈕： -->
                                  <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                  <button class="sort">
                                      <i class="fa-solid fa-sort-up"></i>
                                      <i class="fa-solid fa-sort-down"></i>
                                  </button>
                              </div>
                          </th>
                          <th>
                              <div class="hstack align-items-center">
                                  <span>申請單位</span>
                
                                  <!-- 排序按鈕： -->
                              <!-- 樣式 class="sort down" 表示降序、class="sort up" 表示升序、class="sort" 表示預設 -->
                                <button class="sort">
                                      <i class="fa-solid fa-sort-up"></i>
                                  <i class="fa-solid fa-sort-down"></i>
                                </button>
                              </div>
                          </th>
                          <th width="150">
                              <div class="hstack align-items-center">
                                  <span>待審項目</span>
                              </div>
                          </th>
                          <th width="130" class="review-progress-header" style="display: none;">
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
                       <h4 class="fs-24 fw-bold text-green-light">審查結果與意見回覆 - 領域審查/初審</h4>
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
                           <button type="button" class="btn btn-teal">
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
                                <div class="col-12 col-md-3">
                                    <div class="fs-16 text-gray mb-2">年度</div>
                                    <select id="sortingYear" class="form-select">
                                        <option value="113">113年</option>
                                        <option value="114">114年</option>
                                    </select>
                                </div>
                                <div class="col-12 col-md-3">
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
                                <div class="col-12 col-md-3">
                                    <div class="fs-16 text-gray mb-2">審查組別</div>
                                    <select id="sortingReviewGroup" class="form-select">
                                    </select>
                                </div>
                                <div class="col-12 col-md-3 d-flex align-items-end">
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
  
  
</asp:Content>