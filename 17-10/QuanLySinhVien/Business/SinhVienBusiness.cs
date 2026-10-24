using QuanLySinhVien.Data.DAL;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien.Business
{
    // Tầng Business: kiểm tra dữ liệu hợp lệ, KHÔNG try/catch.
    // Có lỗi thì ném ngoại lệ lên cho tầng View xử lý.
    public class SinhVienBusiness
    {
        private readonly SinhVienDAL sinhVienDAL = new SinhVienDAL();
        private readonly LopHocDAL lopHocDAL = new LopHocDAL();

        public List<SinhVien> LayTatCa()
        {
            return sinhVienDAL.GetAllSinhVien();
        }

        public SinhVien TimTheoMa(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                throw new ArgumentException("Mã sinh viên không được để trống.");
            return sinhVienDAL.GetSinhVienByMaSV(maSV.Trim());
        }

        public List<SinhVien> LayTheoLop(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                throw new ArgumentException("Mã lớp không được để trống.");
            return sinhVienDAL.GetSinhViensByMaLop(maLop.Trim());
        }

        public void Them(SinhVien sv)
        {
            KiemTra(sv);
            if (sinhVienDAL.GetSinhVienByMaSV(sv.MaSV) != null)
                throw new Exception($"Mã sinh viên '{sv.MaSV}' đã tồn tại.");
            sinhVienDAL.AddSinhVien(sv);
        }

        public void Sua(SinhVien sv)
        {
            KiemTra(sv);
            sinhVienDAL.UpdateSinhVien(sv);
        }

        public void Xoa(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                throw new ArgumentException("Mã sinh viên không được để trống.");
            sinhVienDAL.DeleteSinhVien(maSV.Trim());
        }

        private void KiemTra(SinhVien sv)
        {
            if (sv == null) throw new ArgumentNullException(nameof(sv), "Sinh viên không được để trống.");
            if (string.IsNullOrWhiteSpace(sv.MaSV)) throw new ArgumentException("Mã sinh viên không được để trống.");
            if (string.IsNullOrWhiteSpace(sv.HoTen)) throw new ArgumentException("Họ tên không được để trống.");
            if (sv.Diem < 0 || sv.Diem > 10) throw new ArgumentException("Điểm phải nằm trong khoảng 0 đến 10.");
            if (!string.IsNullOrWhiteSpace(sv.Email) && !sv.Email.Contains('@'))
                throw new ArgumentException("Email không hợp lệ.");
            if (!string.IsNullOrWhiteSpace(sv.NgaySinh) && !SinhVienDAL.TryParseNgaySinh(sv.NgaySinh, out _))
                throw new ArgumentException("Ngày sinh không hợp lệ (định dạng dd/MM/yyyy).");
            if (string.IsNullOrWhiteSpace(sv.MaLop)) throw new ArgumentException("Mã lớp không được để trống.");
            if (lopHocDAL.GetLopHocByMaLop(sv.MaLop) == null)
                throw new Exception($"Lớp '{sv.MaLop}' không tồn tại. Hãy thêm lớp trước.");
        }
    }
}
