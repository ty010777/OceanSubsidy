<%-- <%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/OFS/SCI/SciOutcomes.aspx.cs" Inherits="OFS_SciOutcomes" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" %> --%>
<%-- <%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %> --%>
<%-- --%>
<%-- <asp:Content ID="ApplicationTitle" ContentPlaceHolderID="ApplicationTitle" runat="server"> --%>
<%--     成果與績效 - 海洋領域補助計畫管理資訊系統 --%>
<%-- </asp:Content> --%>
<%-- --%>
<%-- <asp:Content ID="HeadExtra" ContentPlaceHolderID="HeadExtra" runat="server"> --%>
<%--     <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script> --%>
<%--     <script src="<%=ResolveUrl("~/script/OFS/SCI/SciOutcomes.js")%>"></script> --%>
<%-- </asp:Content> --%>
<%-- --%>
<%-- <asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server"> --%>
<%--     <!-- 內容區塊 --> --%>
<%--     <div class="block"> --%>
<%--         <h5 class="square-title d-flex flex-wrap gap-2">成果與績效 --%>
<%--             <span class="text-pink fw-normal">＊應至少填列 3 項</span> --%>
<%--         </h5> --%>
<%--         <div class="table-responsive mt-3"> --%>
<%--             <table class="table align-middle gray-table"> --%>
<%--                 <thead class="text-center"> --%>
<%--                     <tr> --%>
<%--                         <th width="200" rowspan="2">績效指標項目</th> --%>
<%--                         <th colspan="2">預估產出數</th> --%>
<%--                         <th width="300" rowspan="2">說明</th> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th width="250">計畫執行期間</th> --%>
<%--                         <th width="250">績效追蹤期間</th> --%>
<%--                     </tr> --%>
<%--                 </thead> --%>
<%--                 <tbody> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(1)技術移轉</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="tech_transfer_plan_count"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">件</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="tech_transfer_plan_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="tech_transfer_track_count"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">件</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="tech_transfer_track_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="tech_transfer_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(2)專利</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="patent_plan_apply"> --%>
<%--                                     <span class="input-group-text">申請件</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="patent_plan_grant"> --%>
<%--                                     <span class="input-group-text">取得件</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="patent_track_apply"> --%>
<%--                                     <span class="input-group-text">申請件</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="patent_track_grant"> --%>
<%--                                     <span class="input-group-text">取得件</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="patent_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(3)人才培育</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="talent_plan_phd"> --%>
<%--                                     <span class="input-group-text">博士人</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="talent_plan_master"> --%>
<%--                                     <span class="input-group-text">碩士人</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="talent_plan_others"> --%>
<%--                                     <span class="input-group-text">其他人</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="talent_track_phd"> --%>
<%--                                     <span class="input-group-text">博士人</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="talent_track_master"> --%>
<%--                                     <span class="input-group-text">碩士人</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="talent_track_others"> --%>
<%--                                     <span class="input-group-text">其他人</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="talent_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(4)論文</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="papers_plan"> --%>
<%--                                     <span class="input-group-text">篇</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="papers_track"> --%>
<%--                                     <span class="input-group-text">篇</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="papers_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(5)促成產學研合作</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="industry_collab_plan_count"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">件</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="industry_collab_plan_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="industry_collab_track_count"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">件</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="industry_collab_track_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="industry_collab_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(6)促成投資</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="investment_plan_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="investment_track_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="investment_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(7)衍生產品</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="products_plan_count"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">項</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="products_plan_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="products_track_count"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">項</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="products_track_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="products_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(8)降低人力成本</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="cost_reduction_plan_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="cost_reduction_track_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="cost_reduction_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(9)技術推廣活動</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="promo_events_plan"> --%>
<%--                                     <span class="input-group-text">場</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="promo_events_track"> --%>
<%--                                     <span class="input-group-text">場</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="promo_events_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(10)技術服務</th> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="tech_services_plan_count"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">次</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="tech_services_plan_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <div class="row g-2"> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="tech_services_track_count"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">次</span> --%>
<%--                                 </div> --%>
<%--                                 <div class="input-group"> --%>
<%--                                     <input type="text" class="form-control" name="tech_services_track_price"> --%>
<%--                                     <span class="input-group-text" style="width: 70px;">千元</span> --%>
<%--                                 </div> --%>
<%--                             </div> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control mb-2" rows="2" placeholder="請輸入" name="tech_services_description"></textarea> --%>
<%--                             <a href="#" class="link-teal">帶入填寫範例</a> --%>
<%--                         </td> --%>
<%--                     </tr> --%>
<%--                     <tr> --%>
<%--                         <th class="label">(11) 其他</th> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control" rows="3" placeholder="請輸入" name="other_plan_description"></textarea> --%>
<%--                         </td> --%>
<%--                         <td> --%>
<%--                             <textarea class="form-control" rows="3" placeholder="請輸入" name="other_track_description"></textarea> --%>
<%--                         </td> --%>
<%--                         <td></td> --%>
<%--                     </tr> --%>
<%--                 </tbody> --%>
<%--             </table> --%>
<%--         </div> --%>
<%--     </div> --%>
<%--      --%>
<%--     <!-- 變更說明 UserControl --> --%>
<%--     <uc:ChangeDescriptionControl ID="ucChangeDescription" runat="server" /> --%>
<%--      --%>
<%--     <!-- 底部區塊 --> --%>
<%--     <div class="block-bottom bg-light-teal"> --%>
<%--         <asp:Button ID="btnTempSave" runat="server" Text="暫存" CssClass="btn btn-outline-teal" OnClick="btnTempSave_Click" /> --%>
<%--         <asp:Button ID="btnNext" runat="server" Text="完成本頁，下一步" CssClass="btn btn-teal" OnClick="btnNext_Click" /> --%>
<%--     </div> --%>
<%-- </asp:Content> --%>