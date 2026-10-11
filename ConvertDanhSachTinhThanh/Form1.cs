using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using System.Xml.Serialization;
using ConvertDanhSachTinhThanh.Constants;

namespace ConvertDanhSachTinhThanh
{


    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();

        List<DiaChiMoi> DiaChiMoi_3Cap { get; set; }
        List<DiaChiMoi> DiaChiMoi_DiaChiFull { get; set; }


        List<TinhThanhCu> listTinhThanhCu { get; set; }
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
            DiaChiMoi_3Cap = new List<DiaChiMoi>();
            DiaChiMoi_DiaChiFull = new List<DiaChiMoi>();
            listTinhThanhCu = GetTinhThanhCu();
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
            string debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DanhSachCauTrucCu", "XaPhuongCu.xml");
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
            comboBox1.Items.Add(AppConstants.ADDRESS_TYPE_PERMANENT);
            comboBox1.Items.Add(AppConstants.ADDRESS_TYPE_CURRENT);
            comboBox1.Items.Add(AppConstants.ADDRESS_TYPE_BIRTH);
            comboBox1.Items.Add(AppConstants.ADDRESS_TYPE_HOMETOWN);
        }

        private List<TinhThanhCu> GetTinhThanhCu()
        {
            string debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DanhSachCauTrucCu", "DanhSachTinhThanhCu.xml");
            string xml = File.ReadAllText(debugPath);


            XmlSerializer serializer = new XmlSerializer(typeof(LisstTinhThanhCu));
            using (StringReader reader = new StringReader(xml))
            {
                LisstTinhThanhCu people = (LisstTinhThanhCu)serializer.Deserialize(reader);
                return people.Items.Cast<TinhThanhCu>().ToList();
            }
        }

        private List<QuanHuyenCu> GetQuanHuyenCu()
        {
            string debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DanhSachCauTrucCu", "DanhSachQuanHuyenCu.xml");
            string xml = File.ReadAllText(debugPath);
            XmlSerializer serializer = new XmlSerializer(typeof(ListQuanHuyenCu));
            using (StringReader reader = new StringReader(xml))
            {
                ListQuanHuyenCu people = (ListQuanHuyenCu)serializer.Deserialize(reader);
                return people.Items.Cast<QuanHuyenCu>().ToList();
            }
        }
    }
}
