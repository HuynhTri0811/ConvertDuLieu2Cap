using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
    public class DiaChiCu
    {
        public Guid HoSo { get; set; }
        public string TenTinhThanhCu { get; set; }
        public string TenQuanHuyenCu { get; set; }
        public string TenXaPhuongCu { get; set; }
        public string SoNha { get; set; }
        public string FullDiaChi { get; set; }

        public DiaChiCu(Guid Oid_,string FullDiaChi_,string TenTinhThanhCu_,string TenQuanHuyenCu_,string TenXaPhuongCu_)
        {
            this.HoSo = Oid_;
            this.FullDiaChi = FullDiaChi_;
            this.TenTinhThanhCu = TenTinhThanhCu_;
            this.TenQuanHuyenCu = TenQuanHuyenCu_;
            this.TenXaPhuongCu = TenXaPhuongCu_;
        }
    }



    public class NewAddressDTO
    {
        public string Code { get; set; }
        public string Commune { get; set; }
        public string Province { get; set; }
        public string FullAddress { get; set; }
    }


    public class ApiResponseDto
    {
        public NewAddressDTO NewAddress { get; set; }
    }


    public class RequestModel
    {
        public string oldAddress { get; set; }
    }


    public class RequesModel_3Cap
    {
        public string provinceCode { get; set; }
        public string districtCode { get; set; }
        public string wardCode { get; set; }
        public string streetAddress { get; set; }
    }
    public class Data
    {
        public Old old { get; set; }
        public New @new { get; set; }
        public MergeInfo mergeInfo { get; set; }
    }

    public class District
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class MergeInfo
    {
        public string notes { get; set; }
    }

    public class New
    {
        public Province province { get; set; }
        public Ward ward { get; set; }
        public string fullAddress { get; set; }
    }

    public class Old
    {
        public Province province { get; set; }
        public District district { get; set; }
        public Ward ward { get; set; }
        public string fullAddress { get; set; }
    }

    public class Province
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Ward
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }


}
