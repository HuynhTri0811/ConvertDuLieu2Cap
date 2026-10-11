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
    public partial class Form1
    {
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

                string script = File.ReadAllText(".\\Sql\\Sql_procedure_InsertTinhThanh.sql");
                var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);


                string debugPath = AppDomain.CurrentDomain.BaseDirectory + "DanhSachTinhThanhMoi.xml";
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
                    SqlQuery += "SELECT N'" + item.code + "' as MaQuanLy ,N'" + item.name + "' as TenTinhThanh ";
                    SqlQuery += " UNION ALL ";
                }

                SqlQuery = SqlQuery.Remove(SqlQuery.Length - 11, 11);

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

        private string GetDanhSachXaPhuongCu(string tenQuanHuyenCu, string tenXaPhuongCu)
        {
            tenQuanHuyenCu = StringNormalizer.NormalizeDistrictName(tenQuanHuyenCu);
            tenXaPhuongCu = StringNormalizer.NormalizeWardName(tenXaPhuongCu).Replace(" ", "");
            XaPhuongCu temp = listXaPhuongCu.Where(x => StringNormalizer.StandardizeString(tenXaPhuongCu) == StringNormalizer.StandardizeString(x.Ten) && TimRaDonViChaThongQuaMa(tenQuanHuyenCu, x.MaTenDonViCha)).FirstOrDefault();
            if (temp != null)
                return temp.Ma;
            return tenXaPhuongCu;
        }

        private bool TimRaDonViChaThongQuaMa(string tenQuanHuyenCu, string maTenDonViCha)
        {
            return maTenDonViCha.Contains(tenQuanHuyenCu);
        }

        private string GetDanhSachQuanHuyenCu(string tenTinhThanhCu, string tenQuanHuyenCu)
        {
            tenQuanHuyenCu = StringNormalizer.NormalizeDistrictName(tenQuanHuyenCu);
            QuanHuyenCu temp = listQuanHuyenCu.Where(x => StringNormalizer.StandardizeString(tenQuanHuyenCu) == StringNormalizer.StandardizeString(x.Ten) && GetNameTinhThanh(tenTinhThanhCu) == x.TenDonViCha).FirstOrDefault();
            if (temp != null)
                return temp.Ma;
            return tenQuanHuyenCu;
        }

        private string GetCodeTinhThanh(string item)
        {
            item = StringNormalizer.NormalizeProvinceName(item);
            TinhThanhCu temp = listTinhThanhCu.Where(x => StringNormalizer.StandardizeString(item) == StringNormalizer.StandardizeString(x.name)).FirstOrDefault();
            if (temp != null)
                return temp.code;
            return item;
        }

        private string GetNameTinhThanh(string item)
        {
            item = StringNormalizer.NormalizeProvinceName(item);
            TinhThanhCu temp = listTinhThanhCu.Where(x => StringNormalizer.StandardizeString(item) == StringNormalizer.StandardizeString(x.code)).FirstOrDefault();
            if (temp != null)
                return temp.name;
            return null;
        }

        public string ChuanHoaChuoi(string input)
        {
            return StringNormalizer.StandardizeString(input);
        }

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



        private void button5_Click(object sender, EventArgs e)
        {

            string Connvectstring = textBox1.Text;
            if (string.IsNullOrEmpty(Connvectstring) == true)
            {
                throw new Exception("Phải kết nối cơ sở dữ liệu trước");
            }
            string script = File.ReadAllText("Sql_procedure.sql");


            var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            int SoLuong = 100;
            int SoTongTrang = listDiaChiMoi.Count / SoLuong;

            for(int i =1;i <= SoTongTrang+1;i++)
            {
                var sb = new StringBuilder();

                foreach (var item in listDiaChiMoi.Skip((i - 1) * SoLuong).Take(SoLuong).ToList())
                {
                    if (item.TinhThanhNew == null)
                        continue;
                    if (string.IsNullOrEmpty(item.FullDiaChi) == true)
                        continue;
                    sb.Append($"SELECT '" + item.HoSo + "' as HoSo," + GetTinhThanhNew(item) + " as TinhThanhNew , " + GetXaPhuongNew(item) + " AS XaPhuongNew ,N'" + item.FullDiaChi + "' AS FullDiaChi ");
                    sb.Append(" UNION ALL ");
                }

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

        private string GetXaPhuongNew(DiaChiMoi item)
        {
            if (item?.XaPhuong_New?.Oid != null)
                return $"'{item.XaPhuong_New.Oid}'";
            return "NULL";
        }

        public string GetTinhThanhNew(DiaChiMoi item)
        {
            if (item?.TinhThanhNew?.Oid != null)
                return $"'{item.TinhThanhNew.Oid}'";
            return "NULL";
        }

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

                string script = File.ReadAllText("Sql_procedure_InsertXaPhuong.sql");
                var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);


                string debugPath = AppDomain.CurrentDomain.BaseDirectory + "DanhSachQuanHuyenMoi.xml";
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
                    SqlQuery += "SELECT N'" + item.code + "' as MaQuanLy ,N'" + item.name + "' as TenXaPhuong ,N'" + item.provinceCode + "' AS MaQuanLyTinhThanh ";
                    SqlQuery += " UNION ALL ";
                }

                SqlQuery = SqlQuery.Remove(SqlQuery.Length - 11, 11);

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
