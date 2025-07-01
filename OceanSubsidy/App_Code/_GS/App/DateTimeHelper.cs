using System;
using System.Globalization;
using System.Text;
using GS.Extension;
using Org.BouncyCastle.Crypto.Digests;

namespace GS.App
{
    /// <summary>
    /// 時間相關
    /// </summary>
    public static class DateTimeHelper
    {
        private const int MinguoOffset = 1911;

        /// <summary>
        /// 將西元年轉為中華民國年 (例如 2025 -> 114)
        /// </summary>
        public static int GregorianYearToMinguo(int gregorianYear)
            => gregorianYear - MinguoOffset;

        /// <summary>
        /// 將中華民國年轉為西元年 (例如 114 -> 2025)
        /// </summary>
        public static int MinguoYearToGregorian(int minguoYear)
            => minguoYear + MinguoOffset;

        /// <summary>
        /// 將 DateTime 格式化為中華民國年日期 (YYY/MM/dd)
        /// </summary>
        public static string ToMinguoDate(this DateTime dt)
        {
            int rocYear = dt.Year - MinguoOffset;
            return $"{rocYear:D3}/{dt:MM/dd}";
        }

        /// <summary>
        /// 將 DateTime 格式化為中華民國年日期時間 (YYY/MM/dd HH:mm:ss)
        /// </summary>
        public static string ToMinguoDateTime(this DateTime dt, string tailFormat = "/MM/dd HH:mm:ss")
        {
            int rocYear = dt.Year - MinguoOffset;
            return rocYear.ToString("D3") + dt.ToString(tailFormat);
        }

        public static string ToMinguoDateTime(this DateTime? dt, string tailFormat = "/MM/dd HH:mm")
        {
            if (!dt.HasValue)
                return String.Empty;

            return ToMinguoDateTime(dt.Value, tailFormat);
        }

        /// <summary>
        /// 解析中華民國年日期字串 (YYY/MM/dd) 為 DateTime
        /// </summary>
        /// <exception cref="FormatException">字串格式不正確時拋出</exception>
        public static DateTime ParseMinguoDate(string minguoDate)
        {
            if (string.IsNullOrWhiteSpace(minguoDate))
                throw new FormatException("中華民國日期字串不可為空。");

            var parts = minguoDate.Split(new[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                throw new FormatException("中華民國日期格式必須為 YYY/MM/dd。");

            int rocYear = int.Parse(parts[0], CultureInfo.InvariantCulture);
            int month = int.Parse(parts[1], CultureInfo.InvariantCulture);
            int day = int.Parse(parts[2], CultureInfo.InvariantCulture);
            int year = MinguoYearToGregorian(rocYear);

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// 解析中華民國年日期時間字串 (YYY/MM/dd HH:mm:ss) 為 DateTime
        /// </summary>
        public static DateTime ParseMinguoDateTime(string minguoDateTime)
        {
            if (string.IsNullOrWhiteSpace(minguoDateTime))
                throw new FormatException("中華民國日期時間字串不可為空。");

            // 預期格式: "YYY/MM/dd" 或 "YYY/MM/dd HH:mm:ss"
            var segments = minguoDateTime.Split(' ');
            var datePart = segments[0];
            var date = ParseMinguoDate(datePart);

            if (segments.Length == 1)
                return date;

            // 時間部分
            if (TimeSpan.TryParse(segments[1], out var ts))
                return date.Add(ts);

            throw new FormatException("中華民國日期時間中的時間部分格式不正確。");
        }

        /// <summary>
        /// 將 UTC 時間轉換為台北時區時間
        /// </summary>
        public static DateTime ToTaipeiTime(this DateTime utcDate)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDate, DateTimeKind.Utc), tz);
        }

        /// <summary>
        /// 將 DateTime 轉為 Unix 時間戳 (秒)
        /// </summary>
        public static long ToUnixTimestamp(this DateTime dt)
        {
            var utc = dt.ToUniversalTime();
            return (long)(utc - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// 從 Unix 時間戳 (秒) 轉回 DateTime (UTC)
        /// </summary>
        public static DateTime FromUnixTimestamp(long timestamp)
            => new DateTime(1970, 1, 1).AddSeconds(timestamp).ToUniversalTime();

        /// <summary>
        /// 取得指定月份的第一天
        /// </summary>
        public static DateTime FirstDayOfMonth(this DateTime dt)
            => new DateTime(dt.Year, dt.Month, 1);

        /// <summary>
        /// 取得指定月份的最後一天
        /// </summary>
        public static DateTime LastDayOfMonth(this DateTime dt)
            => new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);

        /// <summary>
        /// 解析中華民國年日期字串 (YYY/MM/dd) 為 DateTime
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool TryParseMinguoDate(string input, out DateTime dt)
        {
            dt = default;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                dt = DateTimeHelper.ParseMinguoDate(input.Trim());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
