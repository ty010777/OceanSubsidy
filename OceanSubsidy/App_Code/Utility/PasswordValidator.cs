using System;
using System.Text.RegularExpressions;

namespace GS.OCA_OceanSubsidy.Utility
{
    /// <summary>
    /// 密碼驗證工具類別
    /// </summary>
    public static class PasswordValidator
    {
        /// <summary>
        /// 允許的特殊符號
        /// </summary>
        private const string SpecialChars = @"!@#$%^&*()_+\-=\[\]{}|;:,.<>?";

        /// <summary>
        /// 最小密碼長度
        /// </summary>
        private const int MinPasswordLength = 8;

        /// <summary>
        /// 驗證密碼複雜度
        /// </summary>
        /// <param name="password">要驗證的密碼</param>
        /// <param name="errorMessage">錯誤訊息（驗證失敗時）</param>
        /// <returns>true: 驗證通過, false: 驗證失敗</returns>
        public static bool ValidateComplexity(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            // 檢查密碼是否為空
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "密碼不能為空";
                return false;
            }

            // 檢查最小長度
            if (password.Length < MinPasswordLength)
            {
                errorMessage = $"密碼長度至少需要 {MinPasswordLength} 個字元";
                return false;
            }

            // 檢查是否包含大寫字母
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                errorMessage = "密碼必須包含至少一個大寫英文字母 (A-Z)";
                return false;
            }

            // 檢查是否包含小寫字母
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                errorMessage = "密碼必須包含至少一個小寫英文字母 (a-z)";
                return false;
            }

            // 檢查是否包含數字
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                errorMessage = "密碼必須包含至少一個數字 (0-9)";
                return false;
            }

            // 檢查是否包含特殊符號
            // 在字元類別 [] 內，只需要轉義 \、]、^ 和 -
            // 將 - 放在最後，] 需要轉義，其他符號不需要額外轉義
            string specialCharsPattern = @"[!@#$%^&*()_+=\[\]{}|;:,.<>?\-]";
            if (!Regex.IsMatch(password, specialCharsPattern))
            {
                errorMessage = "密碼必須包含至少一個特殊符號 (!@#$%^&*()_+-=[]{}|;:,.<>?)";
                return false;
            }

            // 所有驗證通過
            return true;
        }

        /// <summary>
        /// 取得密碼複雜度規則說明
        /// </summary>
        /// <returns>規則說明文字</returns>
        public static string GetPasswordRules()
        {
            return $"密碼規則：至少 {MinPasswordLength} 個字元，須包含大寫字母、小寫字母、數字及特殊符號 (!@#$%^&*()_+-=[]{{}}|;:,.<>?)";
        }
    }
}
