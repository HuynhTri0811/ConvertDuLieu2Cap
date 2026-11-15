using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConvertDanhSachTinhThanh.Constants;
using ConvertDanhSachTinhThanh.Helpers;

namespace ConvertDanhSachTinhThanh
{
    public partial class Form1
    {
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

        private async Task ConvertAddressAsync(string DiaChiCu, Guid HoSo)
        {
            string oldAddress = StringNormalizer.ConvertVietnameseAbbreviations(DiaChiCu);

            DiaChiMoi dcm = new DiaChiMoi();
            dcm.HoSo = HoSo;
            DiaChiMoi_DiaChiFull.Add(dcm);

            var model = new RequestModel { oldAddress = oldAddress };
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(AppConstants.API_ENDPOINT_CONVERT_ADDRESS, content);
                response.EnsureSuccessStatusCode();

                var responseResult = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<ApiResponseDto>(responseResult);

                dcm.TinhThanhNew = GetTinhThanhNewWithTen(apiResult.NewAddress.Province);
                dcm.XaPhuong_New = GetXaPhuongNewWithCode(apiResult.NewAddress.Code);
                dcm.FullDiaChi = apiResult.NewAddress.FullAddress;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private TinhThanh_New GetTinhThanhNewWithTen(string province)
        {
            return GetTinhThanhNew(x => x.TenTinhThanh_New == province);
        }

        private TinhThanh_New GetTinhThanhNewWithCode(string province)
        {
            return GetTinhThanhNew(x => x.MaQuanLy == province);
        }

        private TinhThanh_New GetTinhThanhNew(Func<TinhThanh_New, bool> predicate)
        {
            TinhThanh_New result = listTinhThanhNew_TonTai.FirstOrDefault(predicate);
            if (result == null)
            {
                result = listTinhThanhNew.FirstOrDefault(predicate);
                if (result != null)
                {
                    listTinhThanhNew_TonTai.Add(result);
                }
            }
            return result;
        }

        private XaPhuong_New GetXaPhuongNewWithCode(string Code)
        {
            XaPhuong_New result = ListXaPhuongNew_TonTai.FirstOrDefault(x => x.MaQuanLy == Code);
            if (result == null)
            {
                result = ListXaPhuongNew.FirstOrDefault(item => item.MaQuanLy == Code);
                if (result != null)
                {
                    ListXaPhuongNew_TonTai.Add(result);
                }
            }
            return result;
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

        private async Task ConvertAddressAsync_3Cap(DiaChiCu fullDiaChi, Guid hoSo)
        {
            DiaChiMoi dcm = new DiaChiMoi();
            dcm.HoSo = hoSo;
            DiaChiMoi_3Cap.Add(dcm);

            var model = new RequesModel_3Cap
            {
                provinceCode = fullDiaChi.TenTinhThanhCu,
                districtCode = fullDiaChi.TenQuanHuyenCu,
                wardCode = fullDiaChi.TenXaPhuongCu,
                streetAddress = fullDiaChi.SoNha
            };

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppConstants.API_AUTH_TOKEN);
                var response = await client.PostAsync(AppConstants.API_ENDPOINT_CONVERT_3CAP, content);
                response.EnsureSuccessStatusCode();

                var responseResult = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<Root>(responseResult);

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
