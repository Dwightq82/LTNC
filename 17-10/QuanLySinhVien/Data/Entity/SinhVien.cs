namespace QuanLySinhVien.Data.Entity
{
    public class SinhVien
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string NgaySinh { get; set; }      // dạng dd/MM/yyyy
        public string SoDienThoai { get; set; }
        public string MaLop { get; set; }
        public double Diem { get; set; }
        public string Email { get; set; }
    }
}
