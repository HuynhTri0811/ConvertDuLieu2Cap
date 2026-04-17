using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{


    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();

        List<DiaChiMoi> DiaChiMoi_3Cap { get; set; }
        List<DiaChiMoi> DiaChiMoi_DiaChiFull { get; set; }


        List<TinhThanh> listTinhThanh { get; set; }
        List<TinhThanhCu> listTinhThanhCu { get; set; }
        List<QuanHuyen> listQuanHuyen { get; set; }
        List<QuanHuyenCu> listQuanHuyenCu { get; set; }
        List<XaPhuongCu> listXaPhuongCu { get; set; }
        List<TinhThanh_New> listTinhThanhNew { get; set; }
        List<TinhThanh_New> listTinhThanhNew_TonTai { get; set; }
        List<XaPhuong_New> ListXaPhuongNew_TonTai { get; set; }
        List<XaPhuong_New> ListXaPhuongNew { get; set; }
        List<TinhThanhTrenCoSoDuLieu> listTinhThanh_CoTrenCoSoDuLieu { get; set; }
        List<QuanHuyenTrenCoSoDuLieu> listQuanHuyen_CoTrenCoDuLieu { get; set; }
        List<DiaChiCu> listDiaChiCu { get; set; }
        List<DiaChiMoi> listDiaChiMoi { get; set; }
        public Form1()
        {
            InitializeComponent();
            listTinhThanh = GetTinhThanh();
            DiaChiMoi_3Cap = new List<DiaChiMoi>();
            DiaChiMoi_DiaChiFull = new List<DiaChiMoi>();
            listTinhThanhCu = GetTinhThanhCu();
            listQuanHuyen = GetQuanHuyen();
            listQuanHuyenCu = GetQuanHuyenCu();
            listXaPhuongCu = GetXaPhuongCu();
            listTinhThanh_CoTrenCoSoDuLieu = new List<TinhThanhTrenCoSoDuLieu>();
            listQuanHuyen_CoTrenCoDuLieu = new List<QuanHuyenTrenCoSoDuLieu>();
            listDiaChiCu = new List<DiaChiCu>();
            listDiaChiMoi = new List<DiaChiMoi>();
            AddDanhSachComboBox();

        }

        private List<XaPhuongCu> GetXaPhuongCu()
        {
            string debugPath = AppDomain.CurrentDomain.BaseDirectory + ".\\DanhSachCauTrucCu\\XaPhuongCu.xml";
            string xml = File.ReadAllText(debugPath);


            XmlSerializer serializer = new XmlSerializer(typeof(ListXaPhuongCu));
            using (StringReader reader = new StringReader(xml))
            {
                ListXaPhuongCu people = (ListXaPhuongCu)serializer.Deserialize(reader);
                return people.Items.Cast<XaPhuongCu>().ToList();
            }
        }

        public void AddDanhSachComboBox()
        {
            comboBox1.Items.Add("Địa chỉ thường trú");
            comboBox1.Items.Add("Nơi ở hiện nay");
            comboBox1.Items.Add("Nơi sinh");
            comboBox1.Items.Add("Quê quán");
        }

        /// <summary>
        /// Đọc từ file danh sách xml to list để lấy ra danh sách tỉnh thành
        /// </summary>
        /// <returns></returns>
        private List<TinhThanh> GetTinhThanh()
        {
            string debugPath = AppDomain.CurrentDomain.BaseDirectory + "DanhSachTinhThanhMoi.xml";
            string xml = File.ReadAllText(debugPath);


            XmlSerializer serializer = new XmlSerializer(typeof(LisstTinhThanh));
            using (StringReader reader = new StringReader(xml))
            {
                LisstTinhThanh people = (LisstTinhThanh)serializer.Deserialize(reader);
                return people.Items.Cast<TinhThanh>().ToList();
            }
        }

        private List<TinhThanhCu> GetTinhThanhCu()
        {
            string debugPath = AppDomain.CurrentDomain.BaseDirectory + ".\\DanhSachCauTrucCu\\DanhSachTinhThanhCu.xml";
            string xml = File.ReadAllText(debugPath);


            XmlSerializer serializer = new XmlSerializer(typeof(LisstTinhThanhCu));
            using (StringReader reader = new StringReader(xml))
            {
                LisstTinhThanhCu people = (LisstTinhThanhCu)serializer.Deserialize(reader);
                return people.Items.Cast<TinhThanhCu>().ToList();
            }
        }

        /// <summary>
        /// Đọc từ file danh sách xml to list để lấy ra danh sách quận huyện
        /// </summary>
        /// <returns></returns>
        private List<QuanHuyen> GetQuanHuyen()
        {
            string debugPath = AppDomain.CurrentDomain.BaseDirectory + "DanhSachQuanHuyenMoi.xml";
            string xml = File.ReadAllText(debugPath);
            XmlSerializer serializer = new XmlSerializer(typeof(ListQuanHuyen));
            using (StringReader reader = new StringReader(xml))
            {
                ListQuanHuyen people = (ListQuanHuyen)serializer.Deserialize(reader);
                return people.Items.Cast<QuanHuyen>().ToList();
            }
        }

        private List<QuanHuyenCu> GetQuanHuyenCu()
        {
            string debugPath = AppDomain.CurrentDomain.BaseDirectory + ".\\DanhSachCauTrucCu\\DanhSachQuanHuyenCu.xml";
            string xml = File.ReadAllText(debugPath);
            XmlSerializer serializer = new XmlSerializer(typeof(ListQuanHuyenCu));
            using (StringReader reader = new StringReader(xml))
            {
                ListQuanHuyenCu people = (ListQuanHuyenCu)serializer.Deserialize(reader);
                return people.Items.Cast<QuanHuyenCu>().ToList();
            }
        }


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
                    Console.WriteLine("✅ Connection successful.");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("❌ Connection failed: " + ex.Message);
                }


                string txtQueryTinhThanh = "Select * from TinhThanh WHERE GCRecord IS NULL";
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
                        Console.WriteLine("❌ Error: " + ex.Message);
                    }
                }

                connection.Close();


            }

        }



        private string ConvertTinhThanhToTinhThanh(string Name)
        {
            string result = Name;

            result = result.Replace("Tỉnh", "");
            result = result.Replace("Thành phố", "");
            result = result.Replace(" ", "");

            return result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string Connvectstring = textBox1.Text;
            List<TinhThanh_New> DanhSachTinhThanh = new List<TinhThanh_New>();
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
            string text = comboBox1.SelectedText;
            string Connvectstring = textBox1.Text;
            listDiaChiCu = new List<DiaChiCu>();
            string txtQueryTinhThanh = "";
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
                if (comboBox1.Text == "Địa chỉ thường trú")
                {
                    txtQueryTinhThanh =
                                            "SELECT hs.Oid AS ThongTinNhanVien_Oid , dc.FullDiaChi ," +
                     "(SELECT TOP 1 TenTinhThanh FROM TinhThanh tt WHERE tt.Oid = dc.TinhThanh) AS TinhThanh," +
                     "(SELECT TOP 1 TenQuanHuyen FROM QuanHuyen qh WHERE qh.Oid = dc.QuanHuyen) AS QuanHuyen," +
                     "(SELECT TOP 1 TenXaPhuong FROM XaPhuong xp WHERE xp.Oid = dc.XaPhuong) AS XaPhuong, dc.SoNha " +
                     "FROM HoSo hs JOIN DiaChi dc ON hs.DiaChiThuongTru = dc.Oid WHERE hs.GCRecord IS NULL AND ISNULL(dc.FullDiaChi,'') <>''";
                }
                if(comboBox1.Text == "Nơi ở hiện nay")
                {
                    txtQueryTinhThanh = "" +
                    "SELECT hs.Oid AS ThongTinNhanVien_Oid , dc.FullDiaChi ," +
                     "(SELECT TOP 1 TenTinhThanh FROM TinhThanh tt WHERE tt.Oid = dc.TinhThanh) AS TinhThanh," +
                     "(SELECT TOP 1 TenQuanHuyen FROM QuanHuyen qh WHERE qh.Oid = dc.QuanHuyen) AS QuanHuyen," +
                     "(SELECT TOP 1 TenXaPhuong FROM XaPhuong xp WHERE xp.Oid = dc.XaPhuong) AS XaPhuong, dc.SoNha " +
                     "FROM HoSo hs JOIN DiaChi dc ON hs.NoiOHienNay = dc.Oid WHERE hs.GCRecord IS NULL AND ISNULL(dc.FullDiaChi,'') <>''";
                }
                if (comboBox1.Text == "Nơi sinh")
                {
                    txtQueryTinhThanh = "" +
                    "SELECT hs.Oid AS ThongTinNhanVien_Oid , dc.FullDiaChi ," +
                     "(SELECT TOP 1 TenTinhThanh FROM TinhThanh tt WHERE tt.Oid = dc.TinhThanh) AS TinhThanh," +
                     "(SELECT TOP 1 TenQuanHuyen FROM QuanHuyen qh WHERE qh.Oid = dc.QuanHuyen) AS QuanHuyen," +
                     "(SELECT TOP 1 TenXaPhuong FROM XaPhuong xp WHERE xp.Oid = dc.XaPhuong) AS XaPhuong, dc.SoNha " +
                     "FROM HoSo hs JOIN DiaChi dc ON hs.NoiSinh = dc.Oid WHERE hs.GCRecord IS NULL AND ISNULL(dc.FullDiaChi,'') <>''";
                }
                if (comboBox1.Text == "Quê quán")
                {
                    txtQueryTinhThanh = "" +
                    "SELECT hs.Oid AS ThongTinNhanVien_Oid , dc.FullDiaChi ," +
                     "(SELECT TOP 1 TenTinhThanh FROM TinhThanh tt WHERE tt.Oid = dc.TinhThanh) AS TinhThanh," +
                     "(SELECT TOP 1 TenQuanHuyen FROM QuanHuyen qh WHERE qh.Oid = dc.QuanHuyen) AS QuanHuyen," +
                     "(SELECT TOP 1 TenXaPhuong FROM XaPhuong xp WHERE xp.Oid = dc.XaPhuong) AS XaPhuong, dc.SoNha " +
                     "FROM HoSo hs JOIN DiaChi dc ON hs.QueQuan = dc.Oid WHERE hs.GCRecord IS NULL AND ISNULL(dc.FullDiaChi,'') <>''";
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
            tenQuanHuyenCu = tenQuanHuyenCu.Replace("Quận", "").Replace("Huyện", "").Replace("Thành phố", "").Replace("quận", "").Replace("huyện", "").Replace("Thành phố", "").Replace("thành phố", "").Replace("tp", "").Replace("Tp", "").Replace("TX", "").Replace("Thị xã", "").Replace("thị xã", "").Replace("TT","");
            tenXaPhuongCu = tenXaPhuongCu.Replace("Phường", "").Replace("phường", "").Replace("Thị trấn", "").Replace("P.", "").Replace("p.", "").Replace(" ", "").Replace("TT", "").Replace("Xã", "").Replace("X", "").Replace("xã", "");
            XaPhuongCu temp = listXaPhuongCu.Where(x => ChuanHoaChuoi(tenXaPhuongCu) == ChuanHoaChuoi(x.Ten) && TimRaDonViChaThongQuaMa(tenQuanHuyenCu,x.MaTenDonViCha)).FirstOrDefault();
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
            tenQuanHuyenCu = tenQuanHuyenCu.Replace("Quận", "").Replace("Huyện", "").Replace("Thành phố", "").Replace("quận", "").Replace("huyện", "").Replace("Thành phố", "").Replace("thành phố", "").Replace("tp", "").Replace("Tp", "").Replace("TP", "").Replace("TX","").Replace("Thị xã","").Replace("thị xã","").Replace("TT","");
            QuanHuyenCu temp = listQuanHuyenCu.Where(x => ChuanHoaChuoi(tenQuanHuyenCu) == ChuanHoaChuoi(x.Ten)  && GetNameTinhThanh(tenTinhThanhCu)  == x.TenDonViCha).FirstOrDefault();
            if (temp != null)
                return temp.Ma;
            return tenQuanHuyenCu;
        }

        private string GetCodeTinhThanh(string item)
        {
            item = item.Replace("tỉnh", "").Replace("Tỉnh", "").Replace("Thành phố", "").Replace("thành phố", "").Replace("tp", "").Replace("Tp", "").Replace("TP", "");
            TinhThanhCu temp = listTinhThanhCu.Where(x => ChuanHoaChuoi(item) == ChuanHoaChuoi(x.name)).FirstOrDefault();
            if (temp != null)
                return temp.code;
            return item;
        }

        private string GetNamTinhThanh(string item)
        {
            item = item.Replace("tỉnh", "").Replace("Tỉnh", "").Replace("Thành phố", "").Replace("thành phố", "").Replace("tp", "").Replace("TP", "");
            TinhThanhCu temp = listTinhThanhCu.Where(x => ChuanHoaChuoi(item) == ChuanHoaChuoi(x.name)).FirstOrDefault();
            if (temp != null)
                return temp.name;
            return null;
        }

        private string GetNameTinhThanh(string item)
        {
            item = item.Replace("tỉnh", "").Replace("Tỉnh", "").Replace("Thành phố", "").Replace("thành phố", "").Replace("tp", "").Replace("TP", "");
            TinhThanhCu temp = listTinhThanhCu.Where(x => ChuanHoaChuoi(item) == ChuanHoaChuoi(x.code)).FirstOrDefault();
            if (temp != null)
                return temp.name;
            return null;
        }

        public string ChuanHoaChuoi(string input)
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
            string item = cleaned.Replace(" ", "");
            return item;
        }



        private async void button4_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = null;
            dataGridView2.Refresh();
            List<DiaChiCu> listDanhSachDiaChiCuCo3Cap = new List<DiaChiCu>();
            List<DiaChiCu> listDanhSachDiaChiCuCoDiaChi = new List<DiaChiCu>();
            listDiaChiMoi = new List<DiaChiMoi>();
            DiaChiMoi_3Cap = new List<DiaChiMoi>();
            DiaChiMoi_DiaChiFull = new List<DiaChiMoi>();
            listTinhThanhNew = GetDanhSachTinhThanhNew();
            ListXaPhuongNew = GetDanhSachXaPhuongNew();
            listTinhThanhNew_TonTai = new List<TinhThanh_New>();
            ListXaPhuongNew_TonTai = new List<XaPhuong_New>();


            foreach(var x in listDiaChiCu)
            {
                if (string.IsNullOrEmpty(x.TenXaPhuongCu) == false && string.IsNullOrEmpty(x.TenQuanHuyenCu) == false && string.IsNullOrEmpty(x.TenTinhThanhCu) == false)
                    listDanhSachDiaChiCuCo3Cap.Add(x);
                else
                    listDanhSachDiaChiCuCoDiaChi.Add(x);
            }

                Task task1 = XuLyList3Cap(listDanhSachDiaChiCuCo3Cap, "List 1");
                Task task2 = XuLyListFull(listDanhSachDiaChiCuCoDiaChi, "List 2");




                await Task.WhenAll(task1, task2);


                listDiaChiMoi.AddRange(DiaChiMoi_DiaChiFull);
                listDiaChiMoi.AddRange(DiaChiMoi_3Cap);

                BindingList<DiaChiMoi> items = new BindingList<DiaChiMoi>();

                foreach (var x in listDiaChiMoi)
                {
                    items.Add(x);
                }

            MessageBox.Show("Thành công");

            dataGridView2.DataSource = items;
        }



        private async Task XuLyList3Cap(List<DiaChiCu> listDanhSachDiaChiCuCo3Cap, string v)
        {
            foreach (var x in listDanhSachDiaChiCuCo3Cap)
            {
                await ConvertAddressAsync_3Cap(x, x.HoSo);
            }
        }

        private async Task XuLyListFull(List<DiaChiCu> listDanhSachDiaChiCuCo3Cap, string v)
        {
            foreach (var x in listDanhSachDiaChiCuCo3Cap)
            {
                await ConvertAddressAsync(x.FullDiaChi, x.HoSo);
            }
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
                string txtQueryTinhThanh = "Select Oid,MaQuanLy,TenXaPhuong,TinhThanh_New from XaPhuong_New WHERE GCRecord IS NULL";
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
                string txtQueryTinhThanh = "Select Oid,MaQuanLy,TenTinhThanh from TinhThanh_New WHERE GCRecord IS NULL";
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

        private async Task ConvertAddressAsync(string DiaChiCu, Guid HoSo)
        {


            string oldAddress = ConvertNhungTinhThanhVietTat(DiaChiCu);

            DiaChiMoi dcm = new DiaChiMoi();
            dcm.HoSo = HoSo;
            DiaChiMoi_DiaChiFull.Add(dcm);

            var model = new RequestModel { oldAddress = oldAddress };
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {


                var response = await client.PostAsync("https://production.cas.so/address-kit/convert", content);
                response.EnsureSuccessStatusCode();

                var responseResult = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<ApiResponseDto>(responseResult);

                //textBox1.Text = apiResult?.NewAddress != null
                //    ? $"Địa chỉ mới: {apiResult.NewAddress.FullAddress}"
                //    : "Không thể chuyển đổi địa chỉ.";
                dcm.TinhThanhNew = GetTinhThanhNewWithTen(apiResult.NewAddress.Province);
                dcm.XaPhuong_New = GetXaPhuongNewWithCode(apiResult.NewAddress.Code);
                dcm.FullDiaChi = apiResult.NewAddress.FullAddress;
            }
            catch (Exception ex)
            {
                return;
            }

        }


        private string ConvertNhungTinhThanhVietTat(string DiaChiCu)
        {
            return DiaChiCu.Replace("TP.", "Thành phố Hồ Chí Minh");
        }

        private TinhThanh_New GetTinhThanhNewWithTen(string province)
        {
            TinhThanh_New result = listTinhThanhNew_TonTai.Where(x => x.TenTinhThanh_New == province).FirstOrDefault();
            if (result == null)
            {
                result = listTinhThanhNew.Where(item => item.TenTinhThanh_New == province).FirstOrDefault();
                if (result != null)
                {
                    listTinhThanhNew_TonTai.Add(result);
                }
            }

            return result;
        }

        private TinhThanh_New GetTinhThanhNewWithCode(string province)
        {
            TinhThanh_New result = listTinhThanhNew_TonTai.Where(x => x.MaQuanLy == province).FirstOrDefault();
            if (result == null)
            {
                result = listTinhThanhNew.Where(item => item.MaQuanLy == province).FirstOrDefault();
                if (result != null)
                {
                    listTinhThanhNew_TonTai.Add(result);
                }
            }

            return result;
        }

        private XaPhuong_New GetXaPhuongNewWithCode(string Code)
        {

            XaPhuong_New result = ListXaPhuongNew.Where(x => x.MaQuanLy == Code).FirstOrDefault();
            if (result == null)
            {
                result = ListXaPhuongNew.Where(item => item.MaQuanLy == Code).FirstOrDefault();
                if (result != null)
                {
                    ListXaPhuongNew_TonTai.Add(result);
                }
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
            if (item == null)
                return "NULL";
            if (item.XaPhuong_New == null)
                return "NULL";
            return "'" + item.XaPhuong_New.Oid.ToString() + "'";
        }

        public string GetTinhThanhNew(DiaChiMoi item)
        {
            if (item == null)
                return "NULL";
            if (item.TinhThanhNew == null)
                return "NULL";
            return "'" + item.TinhThanhNew.Oid.ToString() + "'";
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

        private async void button7_Click(object sender, EventArgs e)
        {
            listTinhThanhNew = GetDanhSachTinhThanhNew();
            ListXaPhuongNew = GetDanhSachXaPhuongNew();
            listTinhThanhNew_TonTai = new List<TinhThanh_New>();
            ListXaPhuongNew_TonTai = new List<XaPhuong_New>();

            foreach (var x in listDiaChiCu)
            {
                await ConvertAddressAsync_3Cap(x, x.HoSo);
            }
            dataGridView2.DataSource = listDiaChiMoi;
        }

        RequesModel_3Cap Convert(DiaChiCu item)
        {
            RequesModel_3Cap x = new RequesModel_3Cap();
            x.provinceCode = item.TenTinhThanhCu;
            x.districtCode = item.TenQuanHuyenCu;
            x.wardCode = item.TenXaPhuongCu;
            x.streetAddress = item.SoNha;
            return x;
        }

        private async Task ConvertAddressAsync_3Cap(DiaChiCu fullDiaChi, Guid hoSo)
        {

            DiaChiMoi dcm = new DiaChiMoi();
            dcm.HoSo = hoSo;
            DiaChiMoi_3Cap.Add(dcm);

            var model = new RequesModel_3Cap { provinceCode = fullDiaChi.TenTinhThanhCu , districtCode = fullDiaChi.TenQuanHuyenCu , wardCode = fullDiaChi.TenXaPhuongCu ,streetAddress = fullDiaChi.SoNha };
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "hvn_u0bexhuCKIxin0sDBrbMe1Rf4d4m8a3c");
                var response = await client.PostAsync("https://tinhthanhpho.com/api/v1/convert/address", content);
                response.EnsureSuccessStatusCode();

                var responseResult = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<Root>(responseResult);

                //textBox1.Text = apiResult?.NewAddress != null
                //    ? $"Địa chỉ mới: {apiResult.NewAddress.FullAddress}"
                //    : "Không thể chuyển đổi địa chỉ.";
                dcm.TinhThanhNew = GetTinhThanhNewWithCode(apiResult.data.@new.province.code);
                dcm.XaPhuong_New = GetXaPhuongNewWithCode(apiResult.data.@new.ward.code);
                dcm.FullDiaChi = apiResult.data.@new.fullAddress;
            }
            catch (Exception ex)
            {
                return;
            }
        }
    } 
}
