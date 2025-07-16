<%@ Page Language="C#" MasterPageFile="~/LoginMaster.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">海洋領域補助案入口網登入 | 海洋委員會</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .login-form-ver2 {
            display: -webkit-box;
            display: -webkit-flex;
            display: -ms-flexbox;
            display: flex;
            -webkit-box-orient: vertical;
            -webkit-box-direction: normal;
            -webkit-flex-direction: column;
            -ms-flex-direction: column;
            flex-direction: column;
            gap: 16px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div class="login-wrap">
        <div class="login-container">
            <div class="logo">
                <img src="assets/img/login/login-logo.svg" alt="logo">
            </div>
            <h1>海洋領域補助案入口網</h1>

            <div class="login-form">
                <div class="login-form-ver2">
                    <!-- Email帳號 -->
                    <div class="form-group">
                        <div class="form-title">
                            <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail">帳號</asp:Label>
                            <a href="#" data-bs-toggle="modal" data-bs-target="#registerModal">
                                <i class="fas fa-user-plus"></i>
                                申請帳號
                            </a>
                        </div>

                        <div class="form-icon-input">
                            <div class="icon">
                                <i class="fas fa-user"></i>
                            </div>
                            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="請輸入帳號" />
                        </div>
                        <div class="mt-2">
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                ControlToValidate="txtEmail" ErrorMessage="請輸入帳號"
                                ValidationGroup="Login" CssClass="invalid" Display="Dynamic" />
                            <asp:RegularExpressionValidator ID="revEmail" runat="server"
                                ControlToValidate="txtEmail"
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                ErrorMessage="帳號格式不正確"
                                ValidationGroup="Login" CssClass="invalid" Display="Dynamic" />
                        </div>
                    </div>
                    <!-- 密碼 -->
                    <div class="form-group">
                        <div class="form-title">
                            <asp:Label ID="lblPassword" runat="server" AssociatedControlID="password">密碼</asp:Label>
                            <a href="#" data-bs-toggle="modal" data-bs-target="#forgotModal">
                                <i class="fas fa-question-circle"></i>
                                忘記密碼
                            </a>
                        </div>

                        <div class="form-icon-input">
                            <div class="icon">
                                <i class="fas fa-lock"></i>
                            </div>
                            <asp:TextBox ID="password" ClientIDMode="Static" runat="server" TextMode="Password" placeholder="請輸入密碼" />

                            <div class="eye-warp">
                                <button class="eye-open" type="button" style="display: none;" onclick="togglePasswordVisibility()">
                                    <i class="fas fa-eye"></i>
                                </button>
                                <button class="eye-close" type="button" onclick="togglePasswordVisibility()">
                                    <i class="fas fa-eye-slash"></i>
                                </button>
                            </div>
                        </div>
                        <div class="mt-2">
                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                                ControlToValidate="password" ErrorMessage="請輸入密碼"
                                ValidationGroup="Login" CssClass="invalid" Display="Dynamic" />
                        </div>

                    </div>

                    <!-- 驗證碼 -->
                    <div class="form-group">
                        <div class="form-title">
                            <asp:Label ID="lblCaptcha" runat="server" AssociatedControlID="txtCaptcha" Text="驗證碼"></asp:Label>
                        </div>
                        <div class="d-flex gap-2 align-items-center">
                            <div class="form-icon-input">
                                <asp:TextBox ID="txtCaptcha" runat="server" placeholder="請輸入驗證碼" />
                            </div>
                            <a href="#" class="d-grid">
                                <span class="fs-12">點擊圖片可換驗證碼</span>
                                <img id="imgValidate" onclick="return reloadcode();" alt="驗證碼" />
                            </a>
                        </div>
                        <div class="mt-2">
                            <asp:RequiredFieldValidator ID="rfvCaptcha" runat="server"
                                ControlToValidate="txtCaptcha"
                                ErrorMessage="請輸入驗證碼"
                                ValidationGroup="Login"
                                CssClass="invalid"
                                Display="Dynamic" />

                            <asp:CustomValidator ID="cvCaptcha" runat="server"
                                ControlToValidate="txtCaptcha"
                                OnServerValidate="cvCaptcha_ServerValidate"
                                ErrorMessage="驗證碼錯誤"
                                ValidationGroup="Login"
                                CssClass="invalid"
                                Display="Dynamic" />
                        </div>
                    </div>

                    <!-- 登入按鈕 -->
                    <asp:Button ID="btnLogin" runat="server" Text="登入" ValidationGroup="Login"
                        CssClass="btn btn-cyan" OnClick="btnLogin_Click" />

                </div>

            </div>
            <ul class="info">
                <li>地址：806高雄市前鎮區成功二路25號4樓</li>
                <li>電話：(07)338-1810 Copyright © 海洋委員會 版權所有</li>
            </ul>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ModalContent" runat="server">

    <!-- ===== 申請帳號 Modal ===== -->
    <div class="modal fade" id="registerModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="applyAccountModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="upRegisterUnit" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="pnlRegister">
                            <div class="modal-header">
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                    <i class="fa-solid fa-circle-xmark"></i>
                                </button>
                            </div>
                            <div class="modal-body">
                                <!-- 系統選擇 -->
                                <div class="bg-light-green d-flex align-items-start  align-items-lg-center gap-1 gap-lg-5 p-4 rounded-1 mb-3 flex-column flex-lg-row">
                                    <span class="fs-20 text-blue-green fw-bold lh-base text-nowrap">欲申請帳號之系統</span>
                                    <asp:CheckBoxList ID="cblRegSystems" runat="server"
                                        CssClass="form-check-input-group"
                                        Style="--grid-columns: 1;"
                                        RepeatDirection="Horizontal"
                                        RepeatLayout="Flow">
                                    </asp:CheckBoxList>
                                </div>


                                <div class="table-responsive">
                                    <table class="table align-middle gray-table side-table">
                                        <tbody>
                                            <!-- 單位選擇 -->
                                            <tr>
                                                <th><span class="text-pink">*</span> 單位類別</th>
                                                <td>
                                                    <asp:RadioButtonList
                                                        ID="rblRegUnitType" runat="server"
                                                        RepeatDirection="Horizontal"
                                                        RepeatLayout="Flow"
                                                        CssClass="form-check-input-group"
                                                        AutoPostBack="true"
                                                        OnSelectedIndexChanged="rblRegUnitType_SelectedIndexChanged">
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <!-- 政府機關下拉-->
                                            <tr id="trGovUnit" runat="server" visible="false">
                                                <th><span class="text-pink">*</span> 單位名稱</th>
                                                <td class="d-flex gap-2">
                                                    <asp:DropDownList
                                                        ID="ddlRegUnits" runat="server"
                                                        CssClass="form-select"
                                                        AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlRegUnits_SelectedIndexChanged" />
                                                    <asp:TextBox
                                                        ID="txtOtherGovUnit" runat="server"
                                                        CssClass="form-control"
                                                        Placeholder="請輸入其他單位名稱"
                                                        Visible="false" />
                                                    <asp:RequiredFieldValidator
                                                        ID="rfvOtherGovUnit" runat="server"
                                                        ControlToValidate="txtOtherGovUnit"
                                                        ErrorMessage="請輸入其他單位名稱"
                                                        ValidationGroup="Register"
                                                        CssClass="invalid"
                                                        Display="Dynamic"
                                                        Enabled="false" />
                                                </td>
                                            </tr>
                                            <!-- 非政府機關輸入-->
                                            <tr id="trOtherUnit" runat="server" visible="false">
                                                <th><span class="text-pink">*</span> 單位名稱</th>
                                                <td>
                                                    <asp:TextBox
                                                        ID="txtOtherUnit" runat="server"
                                                        CssClass="form-control"
                                                        Placeholder="請輸入單位名稱" />
                                                    <asp:RequiredFieldValidator
                                                        ID="rfvOtherUnit" runat="server"
                                                        ControlToValidate="txtOtherUnit"
                                                        ErrorMessage="請輸入單位名稱"
                                                        ValidationGroup="Register"
                                                        CssClass="invalid"
                                                        Display="Dynamic"
                                                        Enabled="false" />
                                                </td>
                                            </tr>
                                            <!-- 帳號 -->
                                            <tr>
                                                <th>
                                                    <span class="text-pink">*</span>帳號(電子郵件)
                                                </th>
                                                <td>
                                                    <div class="verification-code">
                                                        <asp:TextBox ID="txtRegEmail" runat="server"
                                                            CssClass="form-control" TextMode="Email"
                                                            Placeholder="以電子郵件作為帳號" />
                                                        <button
                                                            id="btnSendCode"
                                                            runat="server"
                                                            type="button"
                                                            class="btn btn-cyan"
                                                            onserverclick="btnSendCode_Click">
                                                            發送驗證碼
                                                        </button>
                                                    </div>
                                                    <asp:RequiredFieldValidator runat="server"
                                                        ControlToValidate="txtRegEmail"
                                                        ErrorMessage="請輸入帳號"
                                                        ValidationGroup="Account"
                                                        CssClass="invalid" Display="Dynamic" />
                                                    <asp:RegularExpressionValidator runat="server"
                                                        ControlToValidate="txtRegEmail"
                                                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                                        ErrorMessage="帳號格式不正確"
                                                        ValidationGroup="Account"
                                                        CssClass="invalid" Display="Dynamic" />
                                                    <asp:CustomValidator
                                                        ID="cvRegisterAccountExists" runat="server"
                                                        ControlToValidate="txtRegEmail"
                                                        ErrorMessage="此帳號已存在"
                                                        ValidationGroup="Account"
                                                        CssClass="invalid" Display="Dynamic"
                                                        EnableClientScript="false" />
                                                </td>
                                            </tr>
                                            <!-- 帳號驗証碼 -->
                                            <tr>
                                                <th>
                                                    <span class="text-pink">*</span>帳號驗証碼
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtRegCode" runat="server"
                                                        CssClass="form-control"
                                                        placeholder="請輸入驗證碼" />
                                                    <asp:RequiredFieldValidator ID="rfvRegCode" runat="server"
                                                        ControlToValidate="txtRegCode"
                                                        ErrorMessage="請輸入驗證碼"
                                                        ValidationGroup="Register"
                                                        CssClass="invalid" Display="Dynamic" />
                                                    <asp:CustomValidator ID="cvRegCode" runat="server"
                                                        ControlToValidate="txtRegCode"
                                                        OnServerValidate="cvRegCode_ServerValidate"
                                                        ErrorMessage="驗證碼錯誤"
                                                        ValidationGroup="Register"
                                                        CssClass="invalid" Display="Dynamic"
                                                        EnableClientScript="false" />
                                                </td>
                                            </tr>
                                            <!-- 姓名 -->
                                            <tr>
                                                <th>
                                                    <span class="text-pink">*</span>姓名
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtRegName" runat="server"
                                                        CssClass="form-control" Placeholder="請輸入中文姓名" />
                                                    <asp:RequiredFieldValidator runat="server"
                                                        ControlToValidate="txtRegName"
                                                        ErrorMessage="請輸入姓名"
                                                        ValidationGroup="Register"
                                                        CssClass="invalid" Display="Dynamic" />
                                                </td>
                                            </tr>
                                            <!-- 電話 -->
                                            <tr>
                                                <th>
                                                    <span class="text-pink">*</span>電話
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtRegTel" runat="server" TextMode="Phone"
                                                        CssClass="form-control" Placeholder="請輸入電話號碼或手機" />
                                                    <asp:RequiredFieldValidator runat="server"
                                                        ControlToValidate="txtRegTel"
                                                        ErrorMessage="請輸入電話"
                                                        ValidationGroup="Register"
                                                        CssClass="invalid" Display="Dynamic" />
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <asp:LinkButton ID="btnRegister" runat="server"
                                    CssClass="btn btn-cyan d-table mx-auto mt-3"
                                    ValidationGroup="Register"
                                    OnClick="btnRegister_Click">
                                <i class="fa-solid fa-check"></i>
                                帳號申請
                                </asp:LinkButton>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- ===== 忘記密碼 Modal ===== -->
    <div class="modal fade" id="forgotModal" tabindex="-1" data-bs-backdrop="static" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="pnlForgot">
                            <div class="modal-header">
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                    <i class="fa-solid fa-circle-xmark"></i>
                                </button>
                            </div>
                            <div class="modal-body">

                                <div class="bg-light-green p-4 rounded-1 mb-3 d-flex justify-content-center">
                                    <span class="fs-18 lh-base">填寫以下欄位後，通過初步檢核，系統即發送密碼重置信件至您當初的信箱。</span>
                                </div>

                                <div class="table-responsive">
                                    <table class="table align-middle gray-table side-table">
                                        <tbody>
                                            <tr>
                                                <th>
                                                    <span class="text-pink">*</span>帳號(電子郵件)
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtForgotEmail" runat="server"
                                                        CssClass="form-control" Placeholder="請輸入帳號(電子郵件)" />
                                                    <asp:RequiredFieldValidator runat="server"
                                                        ControlToValidate="txtForgotEmail"
                                                        ErrorMessage="請輸入帳號"
                                                        ValidationGroup="Forgot"
                                                        CssClass="invalid" Display="Dynamic" />
                                                    <asp:RegularExpressionValidator runat="server"
                                                        ControlToValidate="txtForgotEmail"
                                                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                                        ErrorMessage="帳號格式不正確"
                                                        ValidationGroup="Forgot"
                                                        CssClass="invalid" Display="Dynamic" />
                                                    <asp:CustomValidator ID="cvForgetAccountExists" runat="server"
                                                        ControlToValidate="txtForgotEmail"
                                                        EnableClientScript="false"
                                                        ErrorMessage="帳號不存在或未通過審核"
                                                        ValidationGroup="Forgot"
                                                        CssClass="invalid" Display="Dynamic" />
                                                </td>
                                            </tr>

                                        </tbody>
                                    </table>
                                </div>

                                <asp:LinkButton ID="btnForgot" runat="server"
                                    CssClass="btn btn-cyan d-table mx-auto mt-3"
                                    ValidationGroup="Forgot"
                                    OnClick="btnForgot_Click">
                                    <i class="fa-solid fa-check"></i>
                                    忘記密碼申請
                                </asp:LinkButton>

                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- ===== 設定or重設密碼 Modal ===== -->
    <div class="modal fade" id="resetModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="forgetPasswordModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="upResetModal" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="pnlResetForm">
                            <div class="modal-header">
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                    <i class="fa-solid fa-circle-xmark"></i>
                                </button>
                            </div>
                            <div class="modal-body">
                                <div class="table-responsive">
                                    <table class="table align-middle gray-table side-table">
                                        <tbody>
                                            <tr>
                                                <th>帳號(電子郵件)
                                                </th>
                                                <td>
                                                    <asp:Label ID="lblResetAccount" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    <span class="text-pink">*</span>新密碼
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtResetPwd" runat="server"
                                                        CssClass="form-control" TextMode="Password"
                                                        Placeholder="請輸入新密碼" />
                                                    <asp:RequiredFieldValidator runat="server"
                                                        ControlToValidate="txtResetPwd"
                                                        ErrorMessage="請輸入新密碼"
                                                        ValidationGroup="Reset"
                                                        CssClass="invalid" Display="Dynamic" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    <span class="text-pink">*</span>新密碼確認欄
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtResetPwdConfirm" runat="server"
                                                        CssClass="form-control" TextMode="Password"
                                                        Placeholder="請輸入新密碼確認" />
                                                    <asp:RequiredFieldValidator runat="server"
                                                        ControlToValidate="txtResetPwdConfirm"
                                                        ErrorMessage="請再次輸入新密碼"
                                                        ValidationGroup="Reset"
                                                        CssClass="invalid" Display="Dynamic" />
                                                    <asp:CompareValidator runat="server"
                                                        ControlToValidate="txtResetPwdConfirm"
                                                        ControlToCompare="txtResetPwd"
                                                        ErrorMessage="兩次密碼不一致"
                                                        ValidationGroup="Reset"
                                                        CssClass="invalid" Display="Dynamic" />
                                                </td>
                                        </tbody>
                                    </table>
                                </div>

                                <asp:LinkButton ID="btnReset" runat="server"
                                    CssClass="btn btn-cyan d-table mx-auto mt-3"
                                    ValidationGroup="Reset"
                                    OnClick="btnReset_Click">
                                    <i class="fa-solid fa-check"></i>
                                    密碼重置
                                </asp:LinkButton>

                                <!-- 隱藏欄位儲存 Token -->
                                <asp:HiddenField ID="hfResetToken" runat="server" />
                                <asp:HiddenField ID="hfResetSalt" runat="server" />
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

</asp:Content>



<asp:Content ID="LoginScripts" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script type="text/javascript">
        // 樣式調整
        $(function () {
            function applyClasses() {
                $('#<%= cblRegSystems.ClientID %> input[type=checkbox]')
                    .addClass('form-check-input');
                $('#<%= rblRegUnitType.ClientID %> input[type=radio]')
                    .addClass('form-check-input');
            }

            $('#registerModal').on('shown.bs.modal', applyClasses);

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(applyClasses);
        });


        //圖形碼的動態載入
        var isReloading = false;

        document.addEventListener("DOMContentLoaded", function () {
            reloadcode();
            // 點擊事件也要綁上
            document.getElementById("imgValidate")
                .addEventListener("click", reloadcode);
        });

        function reloadcode() {
            if (isReloading) return false;
            isReloading = true;

            var img = document.getElementById("imgValidate");
            var btn = document.getElementById("<%= btnLogin.ClientID %>");

            img.setAttribute("disabled", "disabled");
            btn.setAttribute("disabled", "disabled");
            img.src = '<%= ResolveUrl("~/wwwroot/System/loading.gif") %>';

            setTimeout(function () {
                loadPic();
                isReloading = false;
            }, 1000);
            return false;
        }

        function loadPic() {
            var img = document.getElementById("imgValidate");
            var btn = document.getElementById("<%= btnLogin.ClientID %>");
            var url = '<%= ResolveUrl("~/Service/ValidateNumber.ashx") %>';

            img.src = url + '?r=' + Math.random();
            img.removeAttribute("disabled");
            btn.removeAttribute("disabled");
        }

    </script>
</asp:Content>

