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
    /// <summary>
    /// Phần Form1 chuyển địa chỉ cũ sang địa chỉ mới qua hai API và tra kết quả trong danh mục mới.
    /// </summary>
    public partial class Form1
    {
        /// <summary>
        /// Nút "ConverDiaChiCuThanhDiaChiMoi": chuyển toàn bộ listDiaChiCu, gom kết quả vào listDiaChiMoi
        /// và hiển thị ở dataGridView2.
        /// </summary>
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


            // Đủ 3 cấp → API 3 cấp theo mã; thiếu cấp → API nhận chuỗi địa chỉ đầy đủ.
            foreach(var x in listDiaChiCu)
            {
                if (string.IsNullOrEmpty(x.TenXaPhuongCu) == false && string.IsNullOrEmpty(x.TenQuanHuyenCu) == false && string.IsNullOrEmpty(x.TenTinhThanhCu) == false)
                    listDanhSachDiaChiCuCo3Cap.Add(x);
                else
                    listDanhSachDiaChiCuCoDiaChi.Add(x);
            }

                // Hai nhóm chạy song song; trong mỗi nhóm các request được gửi tuần tự.
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

        /// <summary>
        /// Gửi lần lượt từng địa chỉ đủ 3 cấp tới API 3 cấp. Tham số v (nhãn nhóm) hiện không dùng.
        /// </summary>
        private async Task XuLyList3Cap(List<DiaChiCu> listDanhSachDiaChiCuCo3Cap, string v)
        {
            foreach (var x in listDanhSachDiaChiCuCo3Cap)
            {
                await ConvertAddressAsync_3Cap(x, x.HoSo);
            }
        }

        /// <summary>
        /// Gửi lần lượt từng địa chỉ thiếu cấp tới API địa chỉ đầy đủ. Tham số v (nhãn nhóm) hiện không dùng.
        /// </summary>
        private async Task XuLyListFull(List<DiaChiCu> listDanhSachDiaChiCuCo3Cap, string v)
        {
            foreach (var x in listDanhSachDiaChiCuCo3Cap)
            {
                await ConvertAddressAsync(x.FullDiaChi, x.HoSo);
            }
        }

        /// <summary>
        /// Chuyển một chuỗi địa chỉ đầy đủ qua API_ENDPOINT_CONVERT_ADDRESS. Tỉnh mới được tra theo TÊN,
        /// xã mới theo MÃ trong phản hồi.
        /// </summary>
        private async Task ConvertAddressAsync(string DiaChiCu, Guid HoSo)
        {
            string oldAddress = StringNormalizer.ConvertVietnameseAbbreviations(DiaChiCu);

            DiaChiMoi dcm = new DiaChiMoi();
            dcm.HoSo = HoSo;
            // Thêm vào danh sách trước khi gọi API: nếu API lỗi, bản ghi vẫn có nhưng thiếu tỉnh/xã mới.
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
            // Lỗi API bị bỏ qua, người dùng không thấy lý do (docs/KNOWN_ISSUES.md).
            catch (Exception ex)
            {
                return;
            }
        }

        /// <summary>
        /// Tra tỉnh mới theo tên đầy đủ (ví dụ "Thành phố Hồ Chí Minh"), so khớp chính xác.
        /// </summary>
        private TinhThanh_New GetTinhThanhNewWithTen(string province)
        {
            return GetTinhThanhNew(x => x.TenTinhThanh_New == province);
        }

        /// <summary>
        /// Tra tỉnh mới theo mã quản lý (ví dụ "79").
        /// </summary>
        private TinhThanh_New GetTinhThanhNewWithCode(string province)
        {
            return GetTinhThanhNew(x => x.MaQuanLy == province);
        }

        /// <summary>
        /// Tìm tỉnh mới trong cache listTinhThanhNew_TonTai trước, rồi mới tới listTinhThanhNew;
        /// tìm thấy thì thêm vào cache. Trả về null nếu không có.
        /// </summary>
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

        /// <summary>
        /// Tra xã/phường mới theo mã quản lý, có cache giống GetTinhThanhNew. Trả về null nếu không có.
        /// </summary>
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

        /// <summary>
        /// Nút "Test": chạy riêng luồng API 3 cấp cho toàn bộ listDiaChiCu, kể cả địa chỉ thiếu cấp,
        /// rồi hiển thị kết quả ở dataGridView2. Giống nút chuyển đổi chính, kết quả trở thành listDiaChiMoi
        /// nên nút "Insert vào địa chỉ mới" sẽ ghi đúng những gì đang hiển thị.
        /// </summary>
        private async void button7_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = null;
            dataGridView2.Refresh();

            listTinhThanhNew = GetDanhSachTinhThanhNew();
            ListXaPhuongNew = GetDanhSachXaPhuongNew();
            listTinhThanhNew_TonTai = new List<TinhThanh_New>();
            ListXaPhuongNew_TonTai = new List<XaPhuong_New>();

            // ConvertAddressAsync_3Cap thêm kết quả vào DiaChiMoi_3Cap; làm mới để không lẫn kết quả lần chạy trước.
            DiaChiMoi_3Cap = new List<DiaChiMoi>();

            foreach (var x in listDiaChiCu)
            {
                await ConvertAddressAsync_3Cap(x, x.HoSo);
            }

            listDiaChiMoi = DiaChiMoi_3Cap;
            dataGridView2.DataSource = new BindingList<DiaChiMoi>(listDiaChiMoi);
        }

        /// <summary>
        /// Chuyển một địa chỉ qua API_ENDPOINT_CONVERT_3CAP bằng mã tỉnh/huyện/xã cũ.
        /// Các trường Ten...Cu lúc này đang chứa MÃ (đã đổi ở bước nạp địa chỉ cũ). Tỉnh và xã mới được tra theo mã.
        /// </summary>
        private async Task ConvertAddressAsync_3Cap(DiaChiCu fullDiaChi, Guid hoSo)
        {
            DiaChiMoi dcm = new DiaChiMoi();
            dcm.HoSo = hoSo;
            // Thêm vào danh sách trước khi gọi API: nếu API lỗi, bản ghi vẫn có nhưng thiếu tỉnh/xã mới.
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
                // .NET Framework 4.x có thể chưa bật TLS 1.2 mặc định. Header Authorization được gán lại
                // trước mỗi request trên HttpClient dùng chung.
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
            // Lỗi API bị bỏ qua, người dùng không thấy lý do (docs/KNOWN_ISSUES.md).
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
