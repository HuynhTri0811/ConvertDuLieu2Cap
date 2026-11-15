using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ConvertDanhSachTinhThanh.Helpers
{
    /// <summary>
    /// Chuẩn hóa tên đơn vị hành chính để so khớp tên trong DB với danh mục XML.
    /// Các hàm Normalize... dùng string.Replace: xóa chuỗi con ở MỌI vị trí (không chỉ tiền tố)
    /// và phân biệt hoa/thường. Kết quả thường còn dấu cách đầu; nơi gọi so sánh qua StandardizeString.
    /// </summary>
    public static class StringNormalizer
    {
        /// <summary>
        /// Bỏ tiền tố tỉnh/thành phố ("Tỉnh", "Thành phố", "TP.", "tp", "Tp"). Giữ nguyên null và chuỗi rỗng.
        /// </summary>
        public static string NormalizeProvinceName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string result = input;
            result = result.Replace("Tỉnh", "")
                          .Replace("Thành phố", "")
                          .Replace("thành phố", "")
                          .Replace("TP.", "")
                          .Replace("tp", "")
                          .Replace("Tp", "");
            return result;
        }

        /// <summary>
        /// Bỏ tiền tố quận/huyện/thị xã/thành phố và viết tắt ("TP", "TX", "TT"). Giữ nguyên null và chuỗi rỗng.
        /// </summary>
        public static string NormalizeDistrictName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string result = input;
            result = result.Replace("Quận", "")
                          .Replace("Huyện", "")
                          .Replace("Thành phố", "")
                          .Replace("quận", "")
                          .Replace("huyện", "")
                          .Replace("thành phố", "")
                          .Replace("tp", "")
                          .Replace("Tp", "")
                          .Replace("TP", "")
                          .Replace("TX", "")
                          .Replace("Thị xã", "")
                          .Replace("thị xã", "")
                          .Replace("TT", "");
            return result;
        }

        /// <summary>
        /// Bỏ tiền tố phường/xã/thị trấn và viết tắt ("P.", "X.", "TT"). Giữ nguyên null và chuỗi rỗng.
        /// Chỉ bỏ "X." chứ không bỏ chữ "X" đơn lẻ, để tên như "Xuân La" giữ nguyên.
        /// </summary>
        public static string NormalizeWardName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string result = input;
            result = result.Replace("Phường", "")
                          .Replace("phường", "")
                          .Replace("Thị trấn", "")
                          .Replace("P.", "")
                          .Replace("p.", "")
                          .Replace("TT", "")
                          .Replace("Xã", "")
                          .Replace("X.", "")
                          .Replace("xã", "");
            return result;
        }

        /// <summary>
        /// Đưa chuỗi về dạng so sánh: chữ thường, bỏ dấu tiếng Việt, đổi đ → d, chỉ giữ a-z và 0-9.
        /// Ví dụ "Đường Lê Lợi, P. Bến Nghé" → "duongleloipbennghe". Ném NullReferenceException nếu input là null.
        /// </summary>
        public static string StandardizeString(string input)
        {
            // 1. Chuyển về chữ thường
            string lower = input.ToLower();

            // 2. Chuẩn hóa Unicode để tách dấu
            string normalized = lower.Normalize(NormalizationForm.FormD);

            // 3. Loại bỏ dấu bằng Regex
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string noDiacritics = regex.Replace(normalized, "")
                                       .Replace('đ', 'd');

            // 4. Loại bỏ khoảng trắng và ký tự đặc biệt
            string cleaned = Regex.Replace(noDiacritics, @"[^a-z0-9]", "");
            return cleaned.Replace(" ", "");
        }

        // "TP.HCM", "TP HCM", "Tp.HCM", "TPHCM"... (không phân biệt hoa/thường).
        private static readonly Regex HoChiMinhAbbreviation = new Regex(@"\bTP\.?\s*HCM\b", RegexOptions.IgnoreCase);

        // "TP." đứng cuối địa chỉ, không kèm tên: giữ cách hiểu cũ là TP. Hồ Chí Minh.
        private static readonly Regex TrailingCityAbbreviation = new Regex(@"\bTP\.\s*$");

        // "TP." theo sau là tên thành phố, ví dụ "TP. Hà Nội", "TP.Thủ Đức".
        private static readonly Regex CityAbbreviation = new Regex(@"\bTP\.\s*(?=\w)");

        /// <summary>
        /// Mở rộng viết tắt thành phố trước khi gửi địa chỉ cho API:
        /// "TP.HCM" → "Thành phố Hồ Chí Minh"; "TP. Hà Nội" → "Thành phố Hà Nội";
        /// "TP." đứng cuối, không kèm tên → "Thành phố Hồ Chí Minh".
        /// Dạng "TP." chỉ nhận chữ hoa; dạng HCM nhận mọi kiểu hoa/thường. Ném NullReferenceException nếu address là null.
        /// </summary>
        public static string ConvertVietnameseAbbreviations(string address)
        {
            if (address.IndexOf("TP", StringComparison.OrdinalIgnoreCase) < 0)
                return address;

            string result = HoChiMinhAbbreviation.Replace(address, "Thành phố Hồ Chí Minh");
            result = TrailingCityAbbreviation.Replace(result, "Thành phố Hồ Chí Minh");
            return CityAbbreviation.Replace(result, "Thành phố ");
        }
    }
}
