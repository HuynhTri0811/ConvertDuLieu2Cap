using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ConvertDanhSachTinhThanh.Helpers
{
    public static class StringNormalizer
    {
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
                          .Replace("X", "")
                          .Replace("xã", "");
            return result;
        }

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

        public static string ConvertVietnameseAbbreviations(string address)
        {
            return address.Replace("TP.", "Thành phố Hồ Chí Minh");
        }
    }
}
