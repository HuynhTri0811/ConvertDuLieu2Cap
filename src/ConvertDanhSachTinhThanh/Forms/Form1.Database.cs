using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using ConvertDanhSachTinhThanh.Constants;
using ConvertDanhSachTinhThanh.Helpers;

namespace ConvertDanhSachTinhThanh
{
    /// <summary>
    /// Phần Form1 làm việc với SQL Server: nạp địa chỉ cũ và đổi tên sang mã, nạp danh mục mới,
    /// chạy script tạo stored procedure và ghi dữ liệu. Chuỗi kết nối lấy từ textBox1.
    /// </summary>
    public partial class Form1
    {
        /// <summary>
        /// Nút "Convert Tỉnh Thành": đọc bảng TinhThanh cũ vào listTinhThanh_CoTrenCoSoDuLieu.
        /// Danh sách này hiện chưa được bước nào khác dùng.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string Connvectstring = textBox1.Text;
            if (string.IsNullOrEmpty(Connvectstring) == true)
            {
                throw new Exception("Phải có chuỗi kết nối đến cơ sở dữ liệu trước");
            }
            using (SqlConnection connection = new SqlConnection(Connvectstring))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection successful.");
                }
                catch (SqlException ex)
                {
                    // Lỗi kết nối chỉ ghi ra console; lệnh truy vấn bên dưới sẽ ném lỗi tiếp.
                    Console.WriteLine("Connection failed: " + ex.Message);
                }
                string txtQueryTinhThanh = AppConstants.DB_QUERY_TINH_THANH;
                using (SqlCommand command = new SqlCommand(txtQueryTinhThanh, connection))
                {
                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            listTinhThanh_CoTrenCoSoDuLieu.Add(new TinhThanhTrenCoSoDuLieu(Guid.Parse(reader["Oid"].ToString()), reader["MaQuanLy"].ToString(), reader["TenTinhThanh"].ToString()));
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error: " + ex.Message);
                    }
                }

                connection.Close();


            }

        }

        /// <summary>
        /// Nút "Thêm mới danh sách tỉnh thành mới vào cơ sở dữ liệu" (làm một lần):
        /// tạo spd_InsertTinhThanhMoi từ Sql/Sql_procedure_InsertTinhThanh.sql, rồi gọi nó với toàn bộ
        /// tỉnh trong Data/DanhSachMoi/DanhSachTinhThanhMoi.xml. Procedure bỏ qua mã đã có nên chạy lại không trùng.
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            string Connvectstring = textBox1.Text;
            List<TinhThanh> list = new List<TinhThanh>();


            if (string.IsNullOrEmpty(Connvectstring) == true)
            {
                throw new Exception("Phải có chuỗi kết nối đến cơ sở dữ liệu trước");
            }

            using (SqlConnection connection = new SqlConnection(Connvectstring))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException ex)
                {
                    throw new Exception("❌ Connection failed: " + ex.Message);
                }

                string script = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sql", "Sql_procedure_InsertTinhThanh.sql"));
                // Script có nhiều batch ngăn bởi dòng GO. SqlCommand không hiểu GO nên phải tách ra và chạy từng phần.
                var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);


                string debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DanhSachMoi", "DanhSachTinhThanhMoi.xml");
                string xml = File.ReadAllText(debugPath);

                XmlSerializer serializer = new XmlSerializer(typeof(LisstTinhThanh));
                using (StringReader reader = new StringReader(xml))
                {
                    LisstTinhThanh people = (LisstTinhThanh)serializer.Deserialize(reader);
                    list = people.Items.Cast<TinhThanh>().ToList();
                }

                string SqlQuery = "";

                foreach (var item in list)
                {
                    // Dựng "SELECT ... UNION ALL SELECT ..." để procedure chạy bằng EXEC và chèn vào biến bảng.
                    // Giá trị được nối thẳng vào chuỗi: tên có dấu ' sẽ làm hỏng câu lệnh (docs/KNOWN_ISSUES.md).
                    SqlQuery += "SELECT N'" + item.code + "' as MaQuanLy ,N'" + item.name + "' as TenTinhThanh ";
                    SqlQuery += " UNION ALL ";
                }

                // Bỏ " UNION ALL " (11 ký tự) thừa ở cuối chuỗi.
                SqlQuery = SqlQuery.Remove(SqlQuery.Length - 11, 11);

                try
                {
                    // Kết nối "connection" ở trên chỉ dùng để kiểm tra; script và procedure chạy trên kết nối riêng này.
                    using (SqlConnection conn = new SqlConnection(Connvectstring))
                    {
                        conn.Open();
                        foreach (var command in commandStrings)
                        {
                            if (!string.IsNullOrWhiteSpace(command))
                            {
                                using (var cmd = new SqlCommand(command, conn))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        using (var cmd = new SqlCommand("spd_InsertTinhThanhMoi", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QueryText", SqlQuery);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                    MessageBox.Show("Thành công","Thành công");
                }
                catch(SqlException ex)
                {
                    throw new Exception("Lỗi " + ex.ToString());
                }
            }


        }

        /// <summary>
        /// Nút "Load danh sách địa chỉ cũ": đọc địa chỉ theo loại đang chọn trên ComboBox,
        /// đổi tên tỉnh/huyện/xã sang mã trong danh mục cũ, lưu vào listDiaChiCu và hiển thị ở dataGridView1.
        /// </summary>
        private void LoadDanhSachDiaChiCuTuTrongCoSoDuLieu_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();
            dataGridView2.DataSource = null;
            dataGridView2.Refresh();
            string Connvectstring = textBox1.Text;
            listDiaChiCu = new List<DiaChiCu>();
            string txtQueryTinhThanh = AddressQueryBuilder.Build(comboBox1.Text);
            if (txtQueryTinhThanh == null)
            {
                throw new InvalidOperationException("Vui lòng chọn loại địa chỉ hợp lệ.");
            }
            if (string.IsNullOrEmpty(Connvectstring) == true)
            {
                throw new Exception("Phải có chuỗi kết nối cơ sở dữ liệu");
            }
            BindingList<DiaChiCu> items = new BindingList<DiaChiCu>();

            using (SqlConnection connection = new SqlConnection(Connvectstring))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException ex)
                {
                    throw new Exception("❌ Connection failed: " + ex.Message);
                }
                using (SqlCommand command = new SqlCommand(txtQueryTinhThanh, connection))
                {
                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            // Đổi theo thứ tự: tên tỉnh → mã tỉnh; tên huyện → mã huyện (trong tỉnh đó); tên xã → mã xã (trong huyện đó).
                            // Cấp nào không tìm thấy thì giữ tên đã chuẩn hóa.
                            DiaChiCu x = new DiaChiCu(Guid.Parse(reader["ThongTinNhanVien_Oid"].ToString()), reader["FullDiaChi"].ToString(), GetCodeTinhThanh(reader["TinhThanh"].ToString()), reader["QuanHuyen"].ToString(), reader["XaPhuong"].ToString());
                            x.TenQuanHuyenCu = GetDanhSachQuanHuyenCu(x.TenTinhThanhCu, x.TenQuanHuyenCu);
                            x.TenXaPhuongCu = GetDanhSachXaPhuongCu(x.TenQuanHuyenCu, x.TenXaPhuongCu);
                            x.SoNha = reader["SoNha"].ToString();
                            items.Add(x);
                            listDiaChiCu.Add(x);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("❌ Error: " + ex.Message);
                    }
                }
            }
            dataGridView1.DataSource = items;

        }

        /// <summary>
        /// Tìm mã xã cũ theo tên xã và huyện cha. tenQuanHuyenCu lúc này là MÃ huyện
        /// (kết quả của GetDanhSachQuanHuyenCu). Trả về mã xã, hoặc tên đã bỏ tiền tố và dấu cách nếu không tìm thấy.
        /// </summary>
        private string GetDanhSachXaPhuongCu(string tenQuanHuyenCu, string tenXaPhuongCu)
        {
            tenQuanHuyenCu = StringNormalizer.NormalizeDistrictName(tenQuanHuyenCu);
            tenXaPhuongCu = StringNormalizer.NormalizeWardName(tenXaPhuongCu).Replace(" ", "");
            XaPhuongCu temp = listXaPhuongCu.Where(x => StringNormalizer.StandardizeString(tenXaPhuongCu) == StringNormalizer.StandardizeString(x.Ten) && TimRaDonViChaThongQuaMa(tenQuanHuyenCu, x.MaTenDonViCha)).FirstOrDefault();
            if (temp != null)
                return temp.Ma;
            return tenXaPhuongCu;
        }

        /// <summary>
        /// Kiểm tra MaTenDonViCha (dạng "001-BaĐình-HàNội") có chứa mã huyện hay không.
        /// Dùng Contains nên mã ngắn có thể khớp nhầm với phần khác của chuỗi (docs/KNOWN_ISSUES.md).
        /// </summary>
        private bool TimRaDonViChaThongQuaMa(string tenQuanHuyenCu, string maTenDonViCha)
        {
            return maTenDonViCha.Contains(tenQuanHuyenCu);
        }

        /// <summary>
        /// Tìm mã huyện cũ theo tên huyện trong đúng tỉnh. tenTinhThanhCu lúc này là MÃ tỉnh
        /// (kết quả của GetCodeTinhThanh). Trả về mã huyện, hoặc tên đã bỏ tiền tố nếu không tìm thấy.
        /// </summary>
        private string GetDanhSachQuanHuyenCu(string tenTinhThanhCu, string tenQuanHuyenCu)
        {
            tenQuanHuyenCu = StringNormalizer.NormalizeDistrictName(tenQuanHuyenCu);
            QuanHuyenCu temp = listQuanHuyenCu.Where(x => StringNormalizer.StandardizeString(tenQuanHuyenCu) == StringNormalizer.StandardizeString(x.Ten) && GetNameTinhThanh(tenTinhThanhCu) == x.TenDonViCha).FirstOrDefault();
            if (temp != null)
                return temp.Ma;
            return tenQuanHuyenCu;
        }

        /// <summary>
        /// Đổi tên tỉnh cũ (ví dụ "Tỉnh An Giang") sang mã tỉnh cũ ("89").
        /// Không tìm thấy thì trả về tên đã bỏ tiền tố.
        /// </summary>
        private string GetCodeTinhThanh(string item)
        {
            item = StringNormalizer.NormalizeProvinceName(item);
            TinhThanhCu temp = listTinhThanhCu.Where(x => StringNormalizer.StandardizeString(item) == StringNormalizer.StandardizeString(x.name)).FirstOrDefault();
            if (temp != null)
                return temp.code;
            return item;
        }

        /// <summary>
        /// Từ mã tỉnh cũ ra tên tỉnh trong XML (viết liền, ví dụ "AnGiang"); trả về null nếu không tìm thấy.
        /// </summary>
        private string GetNameTinhThanh(string item)
        {
            item = StringNormalizer.NormalizeProvinceName(item);
            TinhThanhCu temp = listTinhThanhCu.Where(x => StringNormalizer.StandardizeString(item) == StringNormalizer.StandardizeString(x.code)).FirstOrDefault();
            if (temp != null)
                return temp.name;
            return null;
        }

        /// <summary>
        /// Giữ để tương thích; gọi thẳng StringNormalizer.StandardizeString.
        /// </summary>
        public string ChuanHoaChuoi(string input)
        {
            return StringNormalizer.StandardizeString(input);
        }

        /// <summary>
        /// Đọc toàn bộ xã/phường mới từ bảng XaPhuong_New (cần chạy bước nạp danh mục mới trước).
        /// </summary>
        private List<XaPhuong_New> GetDanhSachXaPhuongNew()
        {
            List<XaPhuong_New> result = new List<XaPhuong_New>();
            string Connvectstring = textBox1.Text;
            if (string.IsNullOrEmpty(Connvectstring) == true)
            {
                throw new Exception("Phải có chuỗi kết nối đến cơ sở dữ liệu");
            }
            using (SqlConnection connection = new SqlConnection(Connvectstring))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException ex)
                {
                    throw new Exception("❌ Connection failed: " + ex.Message);
                }
                string txtQueryTinhThanh = AppConstants.DB_QUERY_XA_PHUONG_NEW;
                using (SqlCommand command = new SqlCommand(txtQueryTinhThanh, connection))
                {
                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result.Add(new XaPhuong_New(Guid.Parse(reader["Oid"].ToString()), reader["MaQuanLy"].ToString(), reader["TenXaPhuong"].ToString(), Guid.Parse(reader["TinhThanh_New"].ToString())));
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("❌ Error: " + ex.Message);
                    }
                }
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Đọc toàn bộ tỉnh mới từ bảng TinhThanh_New (cần chạy bước nạp danh mục mới trước).
        /// </summary>
        private List<TinhThanh_New> GetDanhSachTinhThanhNew()
        {
            List<TinhThanh_New> result = new List<TinhThanh_New>();
            string Connvectstring = textBox1.Text;
            if (string.IsNullOrEmpty(Connvectstring) == true)
            {
                throw new Exception("Phải có chuỗi kết nối đến cơ sở dữ liệu");
            }
            using (SqlConnection connection = new SqlConnection(Connvectstring))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException ex)
                {
                    throw new Exception("❌ Connection failed: " + ex.Message);
                }
                string txtQueryTinhThanh = AppConstants.DB_QUERY_TINH_THANH_NEW;
                using (SqlCommand command = new SqlCommand(txtQueryTinhThanh, connection))
                {
                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result.Add(new TinhThanh_New(Guid.Parse(reader["Oid"].ToString()), reader["MaQuanLy"].ToString(), reader["TenTinhThanh"].ToString()));
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("❌ Error: " + ex.Message);
                    }
                }
                connection.Close();
            }
            return result;
        }



        /// <summary>
        /// Nút "Insert vào địa chỉ mới": tạo spd_InsertDanhSachDiaChiMoi từ Sql/Sql_procedure.sql, rồi ghi
        /// listDiaChiMoi theo từng lô. Procedure tạo DiaChi_New và cập nhật cột *_New của HoSo theo loại địa chỉ.
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {

            string Connvectstring = textBox1.Text;
            if (string.IsNullOrEmpty(Connvectstring) == true)
            {
                throw new Exception("Phải kết nối cơ sở dữ liệu trước");
            }
            string script = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sql", "Sql_procedure.sql"));


            // Script có nhiều batch ngăn bởi dòng GO. SqlCommand không hiểu GO nên phải tách ra và chạy từng phần.
            var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            // Mỗi lô 100 địa chỉ để câu SQL động không quá dài. Vòng lặp chạy SoTongTrang + 1 lần,
            // nên lô cuối có thể rỗng khi số địa chỉ chia hết cho 100 (docs/KNOWN_ISSUES.md).
            int SoLuong = 100;
            int SoTongTrang = listDiaChiMoi.Count / SoLuong;

            for(int i =1;i <= SoTongTrang+1;i++)
            {
                var sb = new StringBuilder();

                foreach (var item in listDiaChiMoi.Skip((i - 1) * SoLuong).Take(SoLuong).ToList())
                {
                    // Bỏ địa chỉ không tra được tỉnh mới hoặc API không trả địa chỉ đầy đủ.
                    if (item.TinhThanhNew == null)
                        continue;
                    if (string.IsNullOrEmpty(item.FullDiaChi) == true)
                        continue;
                    sb.Append($"SELECT '" + item.HoSo + "' as HoSo," + GetTinhThanhNew(item) + " as TinhThanhNew , " + GetXaPhuongNew(item) + " AS XaPhuongNew ,N'" + item.FullDiaChi + "' AS FullDiaChi ");
                    sb.Append(" UNION ALL ");
                }

                // Bỏ " UNION ALL " (11 ký tự) thừa ở cuối chuỗi.
                if(sb.Length > 12 )
                    sb.Remove(sb.Length - 11, 11);

                string query = sb.ToString();

                try
                {
                    using (SqlConnection conn = new SqlConnection(Connvectstring))
                    {
                        conn.Open();
                        foreach (var command in commandStrings)
                        {
                            if (!string.IsNullOrWhiteSpace(command))
                            {
                                using (var cmd = new SqlCommand(command, conn))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        using (var cmd = new SqlCommand("spd_InsertDanhSachDiaChiMoi", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SqlInsert", query);
                            // @Loai quyết định cột nào của HoSo được cập nhật (DiaChiThuongTru_New, NoiOHienNay_New...).
                            cmd.Parameters.AddWithValue("@Loai", comboBox1.Text);
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("lỗi" + ex.ToString());
                }
            }






        }

        /// <summary>
        /// Oid xã/phường mới dưới dạng literal SQL ('...'), hoặc NULL nếu chưa tra được.
        /// </summary>
        private string GetXaPhuongNew(DiaChiMoi item)
        {
            if (item?.XaPhuong_New?.Oid != null)
                return $"'{item.XaPhuong_New.Oid}'";
            return "NULL";
        }

        /// <summary>
        /// Oid tỉnh mới dưới dạng literal SQL ('...'), hoặc NULL nếu chưa tra được.
        /// Khác với GetTinhThanhNew(Func) trong Form1.AddressConversion.cs (tra cứu trong danh mục).
        /// </summary>
        public string GetTinhThanhNew(DiaChiMoi item)
        {
            if (item?.TinhThanhNew?.Oid != null)
                return $"'{item.TinhThanhNew.Oid}'";
            return "NULL";
        }

        /// <summary>
        /// Nút "Thêm mới danh sách xã phương mới vào cơ sở dữ liệu" (làm một lần, sau khi đã nạp tỉnh mới):
        /// tạo spd_InsertXaPhuongMoi rồi gọi nó với toàn bộ xã/phường trong Data/DanhSachMoi/DanhSachQuanHuyenMoi.xml.
        /// Procedure liên kết xã với tỉnh qua mã tỉnh và bỏ qua mã đã có.
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            string Connvectstring = textBox1.Text;
            if (string.IsNullOrEmpty(Connvectstring) == true)
            {
                throw new Exception("Phải có chuỗi kết nối đến cơ sở dữ liệu trước");
            }
            List<QuanHuyen> list = new List<QuanHuyen>();
            using (SqlConnection connection = new SqlConnection(Connvectstring))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException ex)
                {
                    throw new Exception("❌ Connection failed: " + ex.Message);
                }

                string script = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sql", "Sql_procedure_InsertXaPhuong.sql"));
                // Script có nhiều batch ngăn bởi dòng GO. SqlCommand không hiểu GO nên phải tách ra và chạy từng phần.
                var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);


                string debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DanhSachMoi", "DanhSachQuanHuyenMoi.xml");
                string xml = File.ReadAllText(debugPath);

                XmlSerializer serializer = new XmlSerializer(typeof(ListQuanHuyen));
                using (StringReader reader = new StringReader(xml))
                {
                    ListQuanHuyen people = (ListQuanHuyen)serializer.Deserialize(reader);
                    list = people.Items.Cast<QuanHuyen>().ToList();
                }

                string SqlQuery = "";

                foreach (var item in list)
                {
                    // Giống button3_Click: dựng "SELECT ... UNION ALL ..." bằng cách nối chuỗi.
                    SqlQuery += "SELECT N'" + item.code + "' as MaQuanLy ,N'" + item.name + "' as TenXaPhuong ,N'" + item.provinceCode + "' AS MaQuanLyTinhThanh ";
                    SqlQuery += " UNION ALL ";
                }

                // Bỏ " UNION ALL " (11 ký tự) thừa ở cuối chuỗi.
                SqlQuery = SqlQuery.Remove(SqlQuery.Length - 11, 11);

                try
                {
                    // Kết nối "connection" ở trên chỉ dùng để kiểm tra; script và procedure chạy trên kết nối riêng này.
                    using (SqlConnection conn = new SqlConnection(Connvectstring))
                    {
                        conn.Open();
                        foreach (var command in commandStrings)
                        {
                            if (!string.IsNullOrWhiteSpace(command))
                            {
                                using (var cmd = new SqlCommand(command, conn))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        using (var cmd = new SqlCommand("spd_InsertXaPhuongMoi", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QueryText", SqlQuery);
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                catch(Exception ex)
                {
                    throw new Exception("Lỗi nè " + ex.ToString());
                }
            }
        }

        // Handler trống do Designer tạo; giữ lại vì Form1.Designer.cs đang nối event này.
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
