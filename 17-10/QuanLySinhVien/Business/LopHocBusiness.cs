using QuanLySinhVien.Data.DAL;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien.Business
{
    // Tầng Business: kiểm tra dữ liệu hợp lệ, KHÔNG try/catch.
    public class LopHocBusiness
    {
        private readonly LopHocDAL lopHocDAL = new LopHocDAL();
        private readonly SinhVienDAL sinhVienDAL = new SinhVienDAL();

        public List<LopHoc> LayTatCa()
        {
            return lopHocDAL.GetAllLopHoc();
        }

        public LopHoc TimTheoMa(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                throw new ArgumentException("Mã lớp không được để trống.");
            return lopHocDAL.GetLopHocByMaLop(maLop.Trim());
        }

        public void Them(LopHoc lop)
        {
            KiemTra(lop);
            if (lopHocDAL.GetLopHocByMaLop(lop.MaLop) != null)
                throw new Exception($"Mã lớp '{lop.MaLop}' đã tồn tại.");
            lopHocDAL.AddLopHoc(lop);
        }

        public void Sua(LopHoc lop)
        {
            KiemTra(lop);
            lopHocDAL.UpdateLopHoc(lop);
        }

        public void Xoa(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                throw new ArgumentException("Mã lớp không được để trống.");
            if (sinhVienDAL.GetSinhViensByMaLop(maLop.Trim()).Count > 0)
                throw new Exception("Lớp còn sinh viên, không thể xóa. Hãy xóa hoặc chuyển sinh viên trước.");
            lopHocDAL.DeleteLopHoc(maLop.Trim());
        }

        private void KiemTra(LopHoc lop)
        {
            if (lop == null) throw new ArgumentNullException(nameof(lop), "Lớp học không được để trống.");
            if (string.IsNullOrWhiteSpace(lop.MaLop)) throw new ArgumentException("Mã lớp không được để trống.");
            if (string.IsNullOrWhiteSpace(lop.TenLop)) throw new ArgumentException("Tên lớp không được để trống.");
        }
    }
}
