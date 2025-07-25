<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReviewChecklist.aspx.cs" Inherits="OFS_ReviewChecklist" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFSMaster.master" EnableViewState="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="<%= ResolveUrl("~/script/OFS/ReviewChecklist.js") %>"></script>

    
    <script>
        $(function() {
            // 初始化審查清單頁面
            if (window.ReviewChecklist) {
                window.ReviewChecklist.init();
            }
        });

    </script>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
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
                    <span class="count">6</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-2">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(2)">
                <div class="total-item-title">領域審查/初審</div>
                <div class="total-item-content">
                    <span class="count">9</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-3">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(3)">
                <div class="total-item-title">技術審查/複審</div>
                <div class="total-item-content">
                    <span class="count">6</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-4">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(4)">
                <div class="total-item-title">決審核定</div>
                <div class="total-item-content">
                    <span class="count">6</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-5">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(5)">
                <div class="total-item-title">計畫變更審核</div>
                <div class="total-item-content">
                    <span class="count">6</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" id="total-item-6">
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(6)">
                <div class="total-item-title">執行計畫審核</div>
                <div class="total-item-content">
                    <span class="count">6</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item " id="total-item-7" >
            <a href="javascript:void(0)" onclick="ReviewChecklist.switchReviewType(7)">
                <div class="total-item-title">階段報告審核</div>
                <div class="total-item-content">
                    <span class="count">6</span>
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
	                       <div class="fs-16 text-gray mb-2">階段</div>
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
	   
	           <asp:Button ID="btnSearch_Type1" runat="server" CssClass="btn btn-teal-dark d-table mx-auto" 
	                       OnClick="btnSearch_Type1_Click" Text="🔍 查詢" />
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
                    <span>共 <span class="text-teal">27</span> 筆資料</span>
                </div>
          
                <button class="btn btn-teal-dark" type="button"><i class="fas fa-download"></i>匯出審查結果</button>
            </div>
          
            <div class="table-responsive mb-0">
                <table class="table teal-table">
                    <thead>
                        <tr>
                            <th width="60">
                                <input class="form-check-input check-teal | checkAll" type="checkbox" name="" >
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
            <div class="bg-light-teal-100 mb-5 | checkPlanBtnPanel checkPlanBtnPanel-type1" style="display: none;">
                <div class="p-3 d-flex justify-content-between align-items-center">
                    <div class="d-flex gap-3">
                        <button class="btn btn-teal" type="button" onclick="handleBatchApproval('轉入下一階段')"><i class="fa-solid fa-check"></i>批次通過，轉入下一階段</button>
                        <button class="btn btn-pink" type="button"><i class="fa-solid fa-xmark"></i>批次不通過，提送申請者</button>
                    </div>
                </div>
            </div>
              <!-- 分頁 -->
            <div class="d-flex align-items-center justify-content-between flex-wrap gap-2">
                <nav class="pagination justify-content-start" aria-label="Pagination">
                    <button class="nav-button" aria-label="Previous page" disabled>
                        <i class="fas fa-chevron-left"></i>
                    </button>
                  
                    <button class="pagination-item active">
                        <span class="page-number">1</span>
                    </button>
                  
                    <button class="pagination-item">
                        <span class="page-number">2</span>
                    </button>
                  
                    <div class="pagination-item ellipsis">
                        <span class="">...</span>
                    </div>
                  
                    <button class="pagination-item">
                        <span class="page-number">9</span>
                    </button>
                   
                    <button class="pagination-item">
                        <span class="page-number">10</span>
                    </button>
                
                    <button class="nav-button" aria-label="Next page">
                        <i class="fas fa-chevron-right"></i>
                    </button>
                </nav>
          
                <div class="page-number-control">
                    <div class="page-number-control-item">
                        <span>跳到</span>
                        <select class="form-select">
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                            <option value="9">9</option>
                            <option value="10" selected>10</option>
                        </select>
                        <span>頁</span>
                        <span>,</span>
                    </div>
                    <div class="page-number-control-item">
                        <span>每頁顯示</span>
                        <select class="form-select">
                            <option value="10">10</option>
                            <option value="20">20</option>
                            <option value="30">30</option>
                        </select>
                        <span>筆</span>
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
            
              <asp:Button ID="btnSearch_Type2" runat="server" CssClass="btn btn-teal-dark d-table mx-auto" 
                          OnClick="btnSearch_Type2_Click" Text="🔍 查詢" />
            </div>
        </div>   
        <div class="block rounded-bottom-4">
          <div class="title border-teal">
        	  <div class="d-flex align-items-center gap-2">
        		  <h4 class="text-teal">
                      <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
        			  <span>列表</span>
        		  </h4>
        		  <span>共 <span class="text-teal">27</span> 筆資料</span>
        	  </div>
        
        	  <div>
        		  <button class="btn btn-teal-dark" type="button" data-bs-toggle="modal" data-bs-target="#reviewResultModal">
        			  <i class="fas fa-list-ol"></i>
        			  審查結果排名
        		  </button>
        		  <button class="btn btn-teal-dark" type="button"><i class="fas fa-download"></i>匯出申請資料</button>
        	  </div>
        
          </div>
        
          
          
          <div class="table-responsive mb-0">
        	  <table class="table teal-table">
        		  <thead>
        			  <tr>
        				  <th width="60">
        					  <input class="form-check-input check-teal | checkAll" type="checkbox" name="" >
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
          <div class="bg-light-teal-100 mb-5 | checkPlanBtnPanel checkPlanBtnPanel-type2" style="display: none;">
        	  <div class="p-3 d-flex justify-content-between align-items-start gap-3 flex-wrap">
        		  <div class="d-flex gap-3 flex-wrap">
        			  <button class="btn btn-royal-blue" type="button" data-bs-toggle="modal" data-bs-target="#tipModal2"><i class="fa-solid fa-check"></i>提送至申請者</button>
        			  <button class="btn btn-teal" type="button" onclick="handleBatchApproval('轉入下一階段')"><i class="fa-solid fa-check"></i>批次通過，轉入下一階段</button>
        			  <button class="btn btn-teal" type="button" onclick="handleBatchApproval('進入決審')"><i class="fa-solid fa-check"></i>批次通過，進入決審</button>
        			  <button class="btn btn-pink" type="button"><i class="fa-solid fa-xmark"></i>批次不通過</button>
        		  </div>
          
        	  </div>
          </div>
        
        
          <!-- 分頁 -->
          <div class="d-flex align-items-center justify-content-between flex-wrap gap-2">
        	  <nav class="pagination justify-content-start" aria-label="Pagination">
        		  <button class="nav-button" aria-label="Previous page" disabled>
        			  <i class="fas fa-chevron-left"></i>
        		  </button>
        	  
        		  <button class="pagination-item active">
        			  <span class="page-number">1</span>
        		  </button>
        	  
        		  <button class="pagination-item">
        			  <span class="page-number">2</span>
        		  </button>
        	  
        		  <div class="pagination-item ellipsis">
        			  <span class="">...</span>
        		  </div>
        	  
        		  <button class="pagination-item">
        			  <span class="page-number">9</span>
        		  </button>
        	  
        		  <button class="pagination-item">
        			  <span class="page-number">10</span>
        		  </button>
        	  
        		  <button class="nav-button" aria-label="Next page">
        			  <i class="fas fa-chevron-right"></i>
        		  </button>
        	  </nav>
        
        	  <div class="page-number-control">
        		  <div class="page-number-control-item">
        			  <span>跳到</span>
        			  <select class="form-select">
        				  <option value="1">1</option>
        				  <option value="2">2</option>
        				  <option value="3">3</option>
        				  <option value="4">4</option>
        				  <option value="5">5</option>
        				  <option value="6">6</option>
        				  <option value="7">7</option>
        				  <option value="8">8</option>
        				  <option value="9">9</option>
        				  <option value="10" selected>10</option>
        			  </select>
        			  <span>頁</span>
        			  <span>,</span>
        		  </div>
        		  <div class="page-number-control-item">
        			  <span>每頁顯示</span>
        			  <select class="form-select">
        				  <option value="10">10</option>
        				  <option value="20">20</option>
        				  <option value="30">30</option>
        			  </select>
        			  <span>筆</span>
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
         
                 <asp:Button ID="btnSearch_Type3" runat="server" CssClass="btn btn-teal-dark d-table mx-auto" 
                             OnClick="btnSearch_Type3_Click" Text="🔍 查詢" />
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
                     <span>共 <span class="text-teal">27</span> 筆資料</span>
                 </div>
         
                 <div>
                     <button class="btn btn-teal-dark" type="button" data-bs-toggle="modal" data-bs-target="#reviewResultModal">
                         <i class="fas fa-list-ol"></i>
                         審查結果排名
                     </button>
                     <button class="btn btn-teal-dark" type="button"><i class="fas fa-download"></i>批次匯出簡報</button>
                 </div>
         
             </div>
         
             
             
             <div class="table-responsive mb-0">
                 <table class="table teal-table">
                     <thead>
                         <tr>
                             <th width="60">
                                 <input class="form-check-input check-teal | checkAll" type="checkbox" name="" >
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
             <div class="bg-light-teal-100 mb-5 | checkPlanBtnPanel checkPlanBtnPanel-type3" style="display: none;">
                 <div class="p-3 d-flex justify-content-between align-items-start gap-3 flex-wrap">
                     <div class="d-flex gap-3 flex-wrap">
                         <button class="btn btn-royal-blue" type="button" data-bs-toggle="modal" data-bs-target="#tipModal2"><i class="fa-solid fa-check"></i>提送至申請者</button>
                         <button class="btn btn-teal" type="button" onclick="handleBatchApproval('轉入下一階段')"><i class="fa-solid fa-check"></i>批次通過，轉入下一階段</button>
                         <button class="btn btn-pink" type="button"><i class="fa-solid fa-xmark"></i>批次不通過</button>
                     </div>
             
                     
                 </div>
             </div>
         
         
             <!-- 分頁 -->
             <div class="d-flex align-items-center justify-content-between flex-wrap gap-2">
                 <nav class="pagination justify-content-start" aria-label="Pagination">
                     <button class="nav-button" aria-label="Previous page" disabled>
                         <i class="fas fa-chevron-left"></i>
                     </button>
                 
                     <button class="pagination-item active">
                         <span class="page-number">1</span>
                     </button>
                 
                     <button class="pagination-item">
                         <span class="page-number">2</span>
                     </button>
                 
                     <div class="pagination-item ellipsis">
                         <span class="">...</span>
                     </div>
                 
                     <button class="pagination-item">
                         <span class="page-number">9</span>
                     </button>
                 
                     <button class="pagination-item">
                         <span class="page-number">10</span>
                     </button>
                 
                     <button class="nav-button" aria-label="Next page">
                         <i class="fas fa-chevron-right"></i>
                     </button>
                 </nav>
         
                 <div class="page-number-control">
                     <div class="page-number-control-item">
                         <span>跳到</span>
                         <select class="form-select">
                             <option value="1">1</option>
                             <option value="2">2</option>
                             <option value="3">3</option>
                             <option value="4">4</option>
                             <option value="5">5</option>
                             <option value="6">6</option>
                             <option value="7">7</option>
                             <option value="8">8</option>
                             <option value="9">9</option>
                             <option value="10" selected>10</option>
                         </select>
                         <span>頁</span>
                         <span>,</span>
                     </div>
                     <div class="page-number-control-item">
                         <span>每頁顯示</span>
                         <select class="form-select">
                             <option value="10">10</option>
                             <option value="20">20</option>
                             <option value="30">30</option>
                         </select>
                         <span>筆</span>
                     </div>
                 </div>
         
             </div>
         </div>
    </div>
    
    <!-- 類型4：結果公告 -->
    <div id="content-type-4" class="review-content" style="display: none;">
        <!-- 搜尋表單 -->
        <div class="search bg-light-teal-100 rounded-0">
          <!-- 查詢表單 -->
          <div class="search-form" action="">
              <div class="column-2">
                  <!-- 計畫編號或名稱關鍵字 -->
                  <div class="search-item">
                      <div class="fs-16 text-gray mb-2">計畫編號或名稱關鍵字</div>
                      <input type="text" name=""  class="form-control" placeholder="請輸入計畫編號、計畫名稱相關文字">
                  </div>
                  
                  <!-- 年度/類別 -->
                  <div class="row g-3">
                      <div class="col-12 col-lg-6">
                          <div class="fs-16 text-gray mb-2">年度</div>
                          <select class="form-select" name="" >
                              <option value="">全部</option>
                              <option value="">114年</option>
                              <option value="">113年</option>
                          </select>
                      </div>
                      <div class="col-12 col-lg-6">
                          <div class="fs-16 text-gray mb-2">類別</div>
                          <select class="form-select" name="" >
                              <option value="">全部</option>
                              <option value="">科專</option>
                              <option value="">文化</option>
                              <option value="">學校民間</option>
                              <option value="">學校社團</option>
                          </select>
                      </div>
                  </div>
              </div>
      
              <div class="column-2">
                  <div class="search-item">
                      <div class="fs-16 text-gray mb-2">申請單位</div>
                      <select class="form-select" name="" >
                          <option value="">全部申請單位</option>
                          <option value="">申請單位</option>
                          <option value="">申請單位</option>
                      </select>
                  </div>
                  <div class="search-item">
                      <div class="fs-16 text-gray mb-2">承辦人員</div>
                      <select class="form-select" name="" >
                          <option value="">林小名</option>
                          <option value="">林小名</option>
                          <option value="">林小名</option>
                      </select>
                  </div>
              </div>    
      
              <button type="button" class="btn btn-teal-dark d-table mx-auto">
                  <i class="fa-solid fa-magnifying-glass"></i>
                  查詢
              </button>
          </div>
        </div>
        <div class="horizontal-scrollable">
          <button class="btn-control btn-prev" role="button"><i class="fas fa-angle-left"></i></button>
          <ul>
        	  <li><button class="btn-type active">科專/資通訊</button></li>
        	  <li><button class="btn-type">科專</button></li>
        	  <li><button class="btn-type">資通訊</button></li>
        	  <li><button class="btn-type">科專/環境工程</button></li>
        	  <li><button class="btn-type">科專/材料科技</button></li>
        	  <li><button class="btn-type">科專/生醫工程</button></li>
        	  <li><button class="btn-type">科專/機械與機電工程</button></li>
        	  <li><button class="btn-type">文化/薪傳/航海智慧轉譯類</button></li>
        	  <li><button class="btn-type">文化/薪傳/海岸聚落發展類</button></li>
        	  <li><button class="btn-type">文化/薪傳/圖文繪本創新類</button></li>
        	  <li><button class="btn-type">文化/船藝/造舟技藝傳承類</button></li>
        	  <li><button class="btn-type">文化/船藝/航海實踐交流類</button></li>
        	  <li><button class="btn-type">文化/藝海/海洋主題創作類</button></li>
        	  <li><button class="btn-type">文化/藝海/海洋藝文扎根類</button></li>
        	  <li><button class="btn-type">學校.民間</button></li>
        	  <li><button class="btn-type">學校社團</button></li>
          </ul>
          <button class="btn-control btn-next" role="button"><i class="fas fa-angle-right"></i></button>
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
        
        		  <!-- 切換表格模式 -->
        		  <div class="btn-group-teal-dark">
        			  <input id="approvalMode" class="approval-mode"  type="radio" name="btn-checked-group" checked>
        			  <label  for="approvalMode">核定模式</label>
        			  <input id="sortMode" class="sort-mode" type="radio" name="btn-checked-group">
        			  <label  for="sortMode">排序模式</label>
        		  </div>
        		  <button class="btn btn-sm btn-teal-dark mb-0" type="button" data-bs-toggle="modal" data-bs-target="#reviewResultModal">
        			  <i class="fas fa-check"></i>
        			  儲存
        		  </button>
        	  </div>
        	  <button class="btn btn-teal-dark" type="button"><i class="fas fa-download"></i>匯出列表資料</button>
          </div>
        
          <!-- 核定模式列表 -->
          <div class="approval-mode-table">
        	  
        	  
        	  
        	  
        	  <div class="table-responsive mb-0">
        		  <table class="table teal-table">
        			  <thead>
        				  <tr>
        					  <th width="60">
        						  <input class="form-check-input check-teal | checkAll" type="checkbox" name="" >
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
        					  <th><span>序位點數</span></th>
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
        				  <tr>
        					  <td>
        						  <input class="form-check-input check-teal | checkPlan" type="checkbox" name="" >
        					  </td>
        					  <td data-th="排序:">1</td>
        					  <td data-th="年度:">114</td>
        					  <td data-th="計畫名稱:" style="text-align: left;">
        						  <a href="#" class="link-black" target="_blank">112-113年水下噪音監測調查計畫</a>
        					  </td>
        					  <td data-th="申請單位:" width="180" style="text-align: left;">淡江大學學校財團法人淡江大學</td>
        					  <td data-th="序位點數:">11</td>
        					  <td data-th="總分:" nowrap>450</td>
        					  <td data-th="申請經費:" width="120" style="text-align: center; text-wrap: nowrap;">3,000,000</td>
        					  <td data-th="核定經費:">
        						  <input type="text" class="form-control" value="3,000,000" style="width: 160px;">
        					  </td>
        					  <td data-th="備註:">
        						  <input type="text" class="form-control" value="因XXXXX">
        					  </td>
        					  <td data-th="修正計畫書:" class="text-center">
        						  <div class="d-flex align-items-center justify-content-center  gap-1">
        							  <button class="btn btn-sm btn-teal-dark" type="button" onclick="window.location.href='information-plan-review-result.html'">
        								  <i class="fas fa-clipboard-check" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="審查"></i>
        							  </button>
        						  </div>
        						  
        					  </td>
        				  </tr>
        				  <tr>
        					  <td>
        						  <input class="form-check-input check-teal | checkPlan" type="checkbox" name="" >
        					  </td>
        					  <td data-th="排序:">1</td>
        					  <td data-th="年度:">114</td>
        					  <td data-th="計畫名稱:" style="text-align: left;">
        						  <a href="#" class="link-black" target="_blank">112-113年水下噪音監測調查計畫</a>
        					  </td>
        					  <td data-th="申請單位:" width="180" style="text-align: left;">淡江大學學校財團法人淡江大學</td>
        					  <td data-th="序位點數:">11</td>
        					  <td data-th="總分:" nowrap>450</td>
        					  <td data-th="申請經費:" width="120" style="text-align: center; text-wrap: nowrap;">3,000,000</td>
        					  <td data-th="核定經費:">
        						  <input type="text" class="form-control" value="3,000,000" style="width: 160px;">
        					  </td>
        					  <td data-th="備註:">
        						  <input type="text" class="form-control" value="因XXXXX">
        					  </td>
        					  <td data-th="修正計畫書:" class="text-center">
        						  通過
        					  </td>
        				  </tr>
        				  <tr>
        					  <td>
        						  <input class="form-check-input check-teal | checkPlan" type="checkbox" name="" >
        					  </td>
        					  <td data-th="排序:">1</td>
        					  <td data-th="年度:">114</td>
        					  <td data-th="計畫名稱:" style="text-align: left;">
        						  <a href="#" class="link-black" target="_blank">112-113年水下噪音監測調查計畫</a>
        					  </td>
        					  <td data-th="申請單位:" width="180" style="text-align: left;">淡江大學學校財團法人淡江大學</td>
        					  <td data-th="序位點數:">11</td>
        					  <td data-th="總分:" nowrap>450</td>
        					  <td data-th="申請經費:" width="120" style="text-align: center; text-wrap: nowrap;">3,000,000</td>
        					  <td data-th="核定經費:">
        						  <input type="text" class="form-control" value="3,000,000" style="width: 160px;">
        					  </td>
        					  <td data-th="備註:">
        						  <input type="text" class="form-control" value="因XXXXX">
        					  </td>
        					  <td data-th="修正計畫書:" class="text-center">
        						  
        					  </td>
        				  </tr>
        			  </tbody>
        		  </table>
        	  </div>
        	  
        	  
        	  
        	  <!-- 審查勾選後底部功能按鈕 -->
        	  <div class="bg-light-teal-100 mb-5 | checkPlanBtnPanel checkPlanBtnPanel-type4" style="display: none;">
        		  <div class="p-3 d-flex justify-content-between align-items-start gap-3 flex-wrap">
        			  <div class="d-flex gap-3 flex-wrap">
        				  <button class="btn btn-royal-blue" type="button" data-bs-toggle="modal" data-bs-target="#tipModal2"><i class="fa-solid fa-check"></i>提送申請者修正計畫書</button>
        				  <button class="btn btn-teal" type="button" onclick="handleBatchApproval('轉入計畫執行階段')"><i class="fa-solid fa-check"></i>批次核定完成，轉入計畫執行階段</button>
        				  <button class="btn btn-pink" type="button"><i class="fa-solid fa-xmark"></i>批次不通過</button>
        			  </div>
        	  
        
        		  </div>
        	  </div>
          </div>
        
          <!-- 排序模式列表 -->
          <div class="sort-mode-table">
        	  <div class="table-responsive mb-0">
        		  <table class="table teal-table" id="sortTable">
        			  <thead>
        				  <tr>
        					  <th>排序</th>
        					  <th width="80">年度</th>
        					  <th width="220">
        						  <div class="hstack align-items-center">
        							  <span>計畫名稱</span>
        						  </div>
        					  </th>
        					  <th class="text-start">
        						  <span>申請單位</span>
        					  </th>
        					  <th><span>序位點數</span></th>
        					  <th><span>總分</span></th>
        					  <th class="text-center"><span>申請經費</span></th>
        					  <th class="text-start">備註</th>
        					  <th>功能</th>
        				  </tr>
        			  </thead>
        			  <tbody>
        				  <tr data-id="0" data-plan-name="112-113年水下噪音監測調查計畫">
        					  <td data-th="排序:">1</td>
        					  <td data-th="年度:">114</td>
        					  <td data-th="計畫名稱:" class="text-start">
        						  <a href="#" class="link-black" target="_blank">112-113年水下噪音監測調查計畫</a>
        					  </td>
        					  <td width="180" data-th="申請單位:" class="text-start">淡江大學學校財團法人淡江大學</td>
        					  <td data-th="序位點數:">11</td>
        					  <td data-th="總分:" nowrap>450</td>
        					  <td data-th="申請經費:" class="text-center" nowrap>3,000,000</td>
        					  <td data-th="備註:">
        						  <input type="text" class="form-control" value="因為XXXXX">
        					  </td>
        					  <td data-th="功能:">
        						  
        						  <div class="d-flex align-items-center justify-content-end  gap-1">
        							  <!-- 拖曳排序把手 -->
        							  <button class="btn btn-sm btn-teal-dark btnDrag" type="button">
        								  <i class="fas fa-arrows-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="拖曳"></i>
        							  </button>
        							  <!-- 置頂按鈕 -->
        							  <button class="btn btn-sm btn-outline-teal btnTop" type="button">
        								  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 12 18" fill="none" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="置頂">
        									  <g clip-path="url(#clip0_893_5801)">
        									  <path d="M10.8519 10.038C10.6229 10.037 10.4038 9.94042 10.2435 9.76976L6.00054 5.3408L1.75763 9.76943C1.36703 10.0487 0.833601 9.94474 0.566094 9.53701C0.372381 9.24183 0.365701 8.85468 0.548917 8.55253L5.39205 3.48839C5.7254 3.13744 6.26805 3.13511 6.60426 3.48308C6.60585 3.48474 6.60744 3.4864 6.60935 3.48839L11.4522 8.55253C11.7846 8.9015 11.7846 9.46529 11.4522 9.81425C11.2871 9.96666 11.0724 10.0467 10.8523 10.038H10.8519Z" fill="#26A69A"/>
        									  <path d="M5.99981 17.4999C5.5265 17.4999 5.14258 17.0992 5.14258 16.6051V4.07891C5.14258 3.58484 5.5265 3.18408 5.99981 3.18408C6.47312 3.18408 6.85705 3.58484 6.85705 4.07891V16.6051C6.85705 17.0992 6.47312 17.4999 5.99981 17.4999Z" fill="#26A69A"/>
        									  <path d="M11.1431 2.28932H0.857234C0.383926 2.28932 0 1.88889 0 1.39482C0 0.900762 0.383926 0.5 0.857234 0.5H11.1431C11.6164 0.5 12.0003 0.900762 12.0003 1.39482C12.0003 1.88889 11.6164 2.28965 11.1431 2.28965V2.28932Z" fill="#26A69A"/>
        									  </g>
        									  <defs>
        									  <clipPath id="clip0_893_5801">
        									  <rect width="12" height="17" fill="white" transform="translate(0 0.5)"/>
        									  </clipPath>
        									  </defs>
        								  </svg>
        							  </button>
        						  </div>
        					  </td>
        				  </tr>
        				  <tr data-id="1" data-plan-name="112-113年測試計畫名稱A">
        					  <td data-th="排序:">2</td>
        					  <td data-th="年度:">114</td>
        					  <td data-th="計畫名稱:" class="text-start">
        						  <a href="#" class="link-black" target="_blank">112-113年測試計畫名稱A</a>
        					  </td>
        					  <td width="180" data-th="申請單位:" class="text-start">淡江大學學校財團法人淡江大學</td>
        					  <td data-th="序位點數:">11</td>
        					  <td data-th="總分:" nowrap>450</td>
        					  <td data-th="申請經費:" class="text-center" nowrap>3,000,000</td>
        					  <td data-th="備註:">
        						  <input type="text" class="form-control" value="因為XXXXX">
        					  </td>
        					  <td data-th="功能:">
        						  
        						  <div class="d-flex align-items-center justify-content-end  gap-1">
        							  <!-- 拖曳排序把手 -->
        							  <button class="btn btn-sm btn-teal-dark btnDrag" type="button">
        								  <i class="fas fa-arrows-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="拖曳"></i>
        							  </button>
        							  <!-- 置頂按鈕 -->
        							  <button class="btn btn-sm btn-outline-teal btnTop" type="button">
        								  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 12 18" fill="none" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="置頂">
        									  <g clip-path="url(#clip0_893_5801)">
        									  <path d="M10.8519 10.038C10.6229 10.037 10.4038 9.94042 10.2435 9.76976L6.00054 5.3408L1.75763 9.76943C1.36703 10.0487 0.833601 9.94474 0.566094 9.53701C0.372381 9.24183 0.365701 8.85468 0.548917 8.55253L5.39205 3.48839C5.7254 3.13744 6.26805 3.13511 6.60426 3.48308C6.60585 3.48474 6.60744 3.4864 6.60935 3.48839L11.4522 8.55253C11.7846 8.9015 11.7846 9.46529 11.4522 9.81425C11.2871 9.96666 11.0724 10.0467 10.8523 10.038H10.8519Z" fill="#26A69A"/>
        									  <path d="M5.99981 17.4999C5.5265 17.4999 5.14258 17.0992 5.14258 16.6051V4.07891C5.14258 3.58484 5.5265 3.18408 5.99981 3.18408C6.47312 3.18408 6.85705 3.58484 6.85705 4.07891V16.6051C6.85705 17.0992 6.47312 17.4999 5.99981 17.4999Z" fill="#26A69A"/>
        									  <path d="M11.1431 2.28932H0.857234C0.383926 2.28932 0 1.88889 0 1.39482C0 0.900762 0.383926 0.5 0.857234 0.5H11.1431C11.6164 0.5 12.0003 0.900762 12.0003 1.39482C12.0003 1.88889 11.6164 2.28965 11.1431 2.28965V2.28932Z" fill="#26A69A"/>
        									  </g>
        									  <defs>
        									  <clipPath id="clip0_893_5801">
        									  <rect width="12" height="17" fill="white" transform="translate(0 0.5)"/>
        									  </clipPath>
        									  </defs>
        								  </svg>
        							  </button>
        						  </div>
        					  </td>
        				  </tr>
        				  <tr data-id="2" data-plan-name="112-113年測試計畫名稱B">
        					  <td data-th="排序:">3</td>
        					  <td data-th="年度:">114</td>
        					  <td data-th="計畫名稱:" class="text-start">
        						  <a href="#" class="link-black" target="_blank">112-113年測試計畫名稱B</a>
        					  </td>
        					  <td width="180" data-th="申請單位:" class="text-start">淡江大學學校財團法人淡江大學</td>
        					  <td data-th="序位點數:">11</td>
        					  <td data-th="總分:" nowrap>450</td>
        					  <td data-th="申請經費:" class="text-center" nowrap>3,000,000</td>
        					  <td data-th="備註:">
        						  <input type="text" class="form-control" value="因為XXXXX">
        					  </td>
        					  <td data-th="功能:">
        						  
        						  <div class="d-flex align-items-center justify-content-end  gap-1">
        							  <!-- 拖曳排序把手 -->
        							  <button class="btn btn-sm btn-teal-dark btnDrag" type="button">
        								  <i class="fas fa-arrows-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="拖曳"></i>
        							  </button>
        							  <!-- 置頂按鈕 -->
        							  <button class="btn btn-sm btn-outline-teal btnTop" type="button">
        								  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 12 18" fill="none" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="置頂">
        									  <g clip-path="url(#clip0_893_5801)">
        									  <path d="M10.8519 10.038C10.6229 10.037 10.4038 9.94042 10.2435 9.76976L6.00054 5.3408L1.75763 9.76943C1.36703 10.0487 0.833601 9.94474 0.566094 9.53701C0.372381 9.24183 0.365701 8.85468 0.548917 8.55253L5.39205 3.48839C5.7254 3.13744 6.26805 3.13511 6.60426 3.48308C6.60585 3.48474 6.60744 3.4864 6.60935 3.48839L11.4522 8.55253C11.7846 8.9015 11.7846 9.46529 11.4522 9.81425C11.2871 9.96666 11.0724 10.0467 10.8523 10.038H10.8519Z" fill="#26A69A"/>
        									  <path d="M5.99981 17.4999C5.5265 17.4999 5.14258 17.0992 5.14258 16.6051V4.07891C5.14258 3.58484 5.5265 3.18408 5.99981 3.18408C6.47312 3.18408 6.85705 3.58484 6.85705 4.07891V16.6051C6.85705 17.0992 6.47312 17.4999 5.99981 17.4999Z" fill="#26A69A"/>
        									  <path d="M11.1431 2.28932H0.857234C0.383926 2.28932 0 1.88889 0 1.39482C0 0.900762 0.383926 0.5 0.857234 0.5H11.1431C11.6164 0.5 12.0003 0.900762 12.0003 1.39482C12.0003 1.88889 11.6164 2.28965 11.1431 2.28965V2.28932Z" fill="#26A69A"/>
        									  </g>
        									  <defs>
        									  <clipPath id="clip0_893_5801">
        									  <rect width="12" height="17" fill="white" transform="translate(0 0.5)"/>
        									  </clipPath>
        									  </defs>
        								  </svg>
        							  </button>
        						  </div>
        					  </td>
        				  </tr>
        				  <tr data-id="3" data-plan-name="112-113年測試計畫名稱C">
        					  <td data-th="排序:">4</td>
        					  <td data-th="年度:">114</td>
        					  <td data-th="計畫名稱:" class="text-start">
        						  <a href="#" class="link-black" target="_blank">112-113年測試計畫名稱C</a>
        					  </td>
        					  <td width="180" data-th="申請單位:" class="text-start">淡江大學學校財團法人淡江大學</td>
        					  <td data-th="序位點數:">11</td>
        					  <td data-th="總分:" nowrap>450</td>
        					  <td data-th="申請經費:" class="text-center" nowrap>3,000,000</td>
        					  <td data-th="備註:">
        						  <input type="text" class="form-control" value="因為XXXXX">
        					  </td>
        					  <td data-th="功能:">
        						  
        						  <div class="d-flex align-items-center justify-content-end  gap-1">
        							  <!-- 拖曳排序把手 -->
        							  <button class="btn btn-sm btn-teal-dark btnDrag" type="button">
        								  <i class="fas fa-arrows-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="拖曳"></i>
        							  </button>
        							  <!-- 置頂按鈕 -->
        							  <button class="btn btn-sm btn-outline-teal btnTop" type="button">
        								  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 12 18" fill="none" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="置頂">
        									  <g clip-path="url(#clip0_893_5801)">
        									  <path d="M10.8519 10.038C10.6229 10.037 10.4038 9.94042 10.2435 9.76976L6.00054 5.3408L1.75763 9.76943C1.36703 10.0487 0.833601 9.94474 0.566094 9.53701C0.372381 9.24183 0.365701 8.85468 0.548917 8.55253L5.39205 3.48839C5.7254 3.13744 6.26805 3.13511 6.60426 3.48308C6.60585 3.48474 6.60744 3.4864 6.60935 3.48839L11.4522 8.55253C11.7846 8.9015 11.7846 9.46529 11.4522 9.81425C11.2871 9.96666 11.0724 10.0467 10.8523 10.038H10.8519Z" fill="#26A69A"/>
        									  <path d="M5.99981 17.4999C5.5265 17.4999 5.14258 17.0992 5.14258 16.6051V4.07891C5.14258 3.58484 5.5265 3.18408 5.99981 3.18408C6.47312 3.18408 6.85705 3.58484 6.85705 4.07891V16.6051C6.85705 17.0992 6.47312 17.4999 5.99981 17.4999Z" fill="#26A69A"/>
        									  <path d="M11.1431 2.28932H0.857234C0.383926 2.28932 0 1.88889 0 1.39482C0 0.900762 0.383926 0.5 0.857234 0.5H11.1431C11.6164 0.5 12.0003 0.900762 12.0003 1.39482C12.0003 1.88889 11.6164 2.28965 11.1431 2.28965V2.28932Z" fill="#26A69A"/>
        									  </g>
        									  <defs>
        									  <clipPath id="clip0_893_5801">
        									  <rect width="12" height="17" fill="white" transform="translate(0 0.5)"/>
        									  </clipPath>
        									  </defs>
        								  </svg>
        							  </button>
        						  </div>
        					  </td>
        				  </tr>
        				  <tr data-id="4" data-plan-name="112-113年測試計畫名稱D">
        					  <td data-th="排序:">5</td>
        					  <td data-th="年度:">114</td>
        					  <td data-th="計畫名稱:" class="text-start">
        						  <a href="#" class="link-black" target="_blank">112-113年測試計畫名稱D</a>
        					  </td>
        					  <td width="180" data-th="申請單位:" class="text-start">淡江大學學校財團法人淡江大學</td>
        					  <td data-th="序位點數:">11</td>
        					  <td data-th="總分:" nowrap>450</td>
        					  <td data-th="申請經費:" class="text-center" nowrap>3,000,000</td>
        					  <td data-th="備註:">
        						  <input type="text" class="form-control" value="因為XXXXX">
        					  </td>
        					  <td data-th="功能:">
        						  
        						  <div class="d-flex align-items-center justify-content-end  gap-1">
        							  <!-- 拖曳排序把手 -->
        							  <button class="btn btn-sm btn-teal-dark btnDrag" type="button">
        								  <i class="fas fa-arrows-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="拖曳"></i>
        							  </button>
        							  <!-- 置頂按鈕 -->
        							  <button class="btn btn-sm btn-outline-teal btnTop" type="button">
        								  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 12 18" fill="none" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="置頂">
        									  <g clip-path="url(#clip0_893_5801)">
        									  <path d="M10.8519 10.038C10.6229 10.037 10.4038 9.94042 10.2435 9.76976L6.00054 5.3408L1.75763 9.76943C1.36703 10.0487 0.833601 9.94474 0.566094 9.53701C0.372381 9.24183 0.365701 8.85468 0.548917 8.55253L5.39205 3.48839C5.7254 3.13744 6.26805 3.13511 6.60426 3.48308C6.60585 3.48474 6.60744 3.4864 6.60935 3.48839L11.4522 8.55253C11.7846 8.9015 11.7846 9.46529 11.4522 9.81425C11.2871 9.96666 11.0724 10.0467 10.8523 10.038H10.8519Z" fill="#26A69A"/>
        									  <path d="M5.99981 17.4999C5.5265 17.4999 5.14258 17.0992 5.14258 16.6051V4.07891C5.14258 3.58484 5.5265 3.18408 5.99981 3.18408C6.47312 3.18408 6.85705 3.58484 6.85705 4.07891V16.6051C6.85705 17.0992 6.47312 17.4999 5.99981 17.4999Z" fill="#26A69A"/>
        									  <path d="M11.1431 2.28932H0.857234C0.383926 2.28932 0 1.88889 0 1.39482C0 0.900762 0.383926 0.5 0.857234 0.5H11.1431C11.6164 0.5 12.0003 0.900762 12.0003 1.39482C12.0003 1.88889 11.6164 2.28965 11.1431 2.28965V2.28932Z" fill="#26A69A"/>
        									  </g>
        									  <defs>
        									  <clipPath id="clip0_893_5801">
        									  <rect width="12" height="17" fill="white" transform="translate(0 0.5)"/>
        									  </clipPath>
        									  </defs>
        								  </svg>
        							  </button>
        						  </div>
        					  </td>
        				  </tr>
        				  <tr data-id="5" data-plan-name="112-113年測試計畫名稱E">
        					  <td data-th="排序:">6</td>
        					  <td data-th="年度:">114</td>
        					  <td data-th="計畫名稱:" class="text-start">
        						  <a href="#" class="link-black" target="_blank">112-113年測試計畫名稱E</a>
        					  </td>
        					  <td width="180" data-th="申請單位:" class="text-start">淡江大學學校財團法人淡江大學</td>
        					  <td data-th="序位點數:">11</td>
        					  <td data-th="總分:" nowrap>450</td>
        					  <td data-th="申請經費:" class="text-center" nowrap>3,000,000</td>
        					  <td data-th="備註:">
        						  <input type="text" class="form-control" value="因為XXXXX">
        					  </td>
        					  <td data-th="功能:">
        						  
        						  <div class="d-flex align-items-center justify-content-end  gap-1">
        							  <!-- 拖曳排序把手 -->
        							  <button class="btn btn-sm btn-teal-dark btnDrag" type="button">
        								  <i class="fas fa-arrows-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="拖曳"></i>
        							  </button>
        							  <!-- 置頂按鈕 -->
        							  <button class="btn btn-sm btn-outline-teal btnTop" type="button">
        								  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 12 18" fill="none" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="置頂">
        									  <g clip-path="url(#clip0_893_5801)">
        									  <path d="M10.8519 10.038C10.6229 10.037 10.4038 9.94042 10.2435 9.76976L6.00054 5.3408L1.75763 9.76943C1.36703 10.0487 0.833601 9.94474 0.566094 9.53701C0.372381 9.24183 0.365701 8.85468 0.548917 8.55253L5.39205 3.48839C5.7254 3.13744 6.26805 3.13511 6.60426 3.48308C6.60585 3.48474 6.60744 3.4864 6.60935 3.48839L11.4522 8.55253C11.7846 8.9015 11.7846 9.46529 11.4522 9.81425C11.2871 9.96666 11.0724 10.0467 10.8523 10.038H10.8519Z" fill="#26A69A"/>
        									  <path d="M5.99981 17.4999C5.5265 17.4999 5.14258 17.0992 5.14258 16.6051V4.07891C5.14258 3.58484 5.5265 3.18408 5.99981 3.18408C6.47312 3.18408 6.85705 3.58484 6.85705 4.07891V16.6051C6.85705 17.0992 6.47312 17.4999 5.99981 17.4999Z" fill="#26A69A"/>
        									  <path d="M11.1431 2.28932H0.857234C0.383926 2.28932 0 1.88889 0 1.39482C0 0.900762 0.383926 0.5 0.857234 0.5H11.1431C11.6164 0.5 12.0003 0.900762 12.0003 1.39482C12.0003 1.88889 11.6164 2.28965 11.1431 2.28965V2.28932Z" fill="#26A69A"/>
        									  </g>
        									  <defs>
        									  <clipPath id="clip0_893_5801">
        									  <rect width="12" height="17" fill="white" transform="translate(0 0.5)"/>
        									  </clipPath>
        									  </defs>
        								  </svg>
        							  </button>
        						  </div>
        					  </td>
        				  </tr>
        			  </tbody>
        		  </table>
        	  </div>
          </div>
        </div>
    </div>
     
</asp:Content>