using System.Globalization;
using System.Text;
using QuanLySinhVien.Business;
using QuanLySinhVien.Data.DAL;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien
{
    // Tầng View (Console): chỉ nhập/xuất, và là nơi DUY NHẤT bắt (try/catch) ngoại lệ.
    internal class Program
    {
        static readonly SinhVienBusiness svBus = new SinhVienBusiness();
        static readonly LopHocBusiness lopBus = new LopHocBusiness();

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            try
            {
                DataHelper.InitDatabase();
            }
            catch (Exception ex)
            {
                BaoLoi("Không kết nối được SQL Server: " + ex.Message);
                Console.WriteLine($"Hãy kiểm tra lại ServerName trong Data/DAL/DataHelper.cs (hiện là {DataHelper.ServerName}).");
                Console.WriteLine("Nhấn phím bất kỳ để thoát...");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                HienMenu();
                string chon = Console.ReadLine()?.Trim();
                if (chon == "0") break;

                try
                {
                    XuLy(chon);
                }
                catch (Exception ex)
                {
                    BaoLoi(ex.Message);
                }

                Console.WriteLine();
                Console.WriteLine("Nhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
        }

        static void HienMenu()
        {
            Console.Clear();
            Console.WriteLine("===== QUẢN LÝ SINH VIÊN =====");
            Console.WriteLine("-- Sinh viên --");
            Console.WriteLine(" 1. Xem danh sách sinh viên");
            Console.WriteLine(" 2. Thêm sinh viên");
            Console.WriteLine(" 3. Sửa sinh viên");
            Console.WriteLine(" 4. Xóa sinh viên");
            Console.WriteLine(" 5. Tìm sinh viên theo mã");
            Console.WriteLine(" 6. Xem sinh viên theo lớp");
            Console.WriteLine("-- Lớp học --");
            Console.WriteLine(" 7. Xem danh sách lớp");
            Console.WriteLine(" 8. Thêm lớp");
            Console.WriteLine(" 9. Sửa lớp");
            Console.WriteLine("10. Xóa lớp");
            Console.WriteLine(" 0. Thoát");
            Console.Write("Chọn chức năng: ");
        }

        static void XuLy(string chon)
        {
            switch (chon)
            {
                case "1":
                    InDanhSachSV(svBus.LayTatCa());
                    break;
                case "2":
                    ThemSV();
                    break;
                case "3":
                    SuaSV();
                    break;
                case "4":
                    string maXoa = Nhap("Mã sinh viên cần xóa: ");
                    svBus.Xoa(maXoa);
                    BaoThanhCong("Đã xóa sinh viên.");
                    break;
                case "5":
                    SinhVien sv = svBus.TimTheoMa(Nhap("Mã sinh viên cần tìm: "));
                    if (sv == null) Console.WriteLine("Không tìm thấy sinh viên.");
                    else InDanhSachSV(new List<SinhVien> { sv });
                    break;
                case "6":
                    InDanhSachSV(svBus.LayTheoLop(Nhap("Mã lớp: ")));
                    break;
                case "7":
                    InDanhSachLop(lopBus.LayTatCa());
                    break;
                case "8":
                    lopBus.Them(new LopHoc { MaLop = Nhap("Mã lớp: "), TenLop = Nhap("Tên lớp: ") });
                    BaoThanhCong("Đã thêm lớp.");
                    break;
                case "9":
                    SuaLop();
                    break;
                case "10":
                    lopBus.Xoa(Nhap("Mã lớp cần xóa: "));
                    BaoThanhCong("Đã xóa lớp.");
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ.");
                    break;
            }
        }

        static void ThemSV()
        {
            SinhVien sv = new SinhVien();
            sv.MaSV = Nhap("Mã SV: ");
            sv.HoTen = Nhap("Họ tên: ");
            sv.GioiTinh = Nhap("Giới tính (Nam/Nữ): ");
            sv.NgaySinh = Nhap("Ngày sinh (dd/MM/yyyy): ");
            sv.SoDienThoai = Nhap("Số điện thoại: ");
            sv.MaLop = Nhap("Mã lớp: ");
            sv.Diem = DocDiem(Nhap("Điểm (0-10): "));
            sv.Email = Nhap("Email: ");
            svBus.Them(sv);
            BaoThanhCong("Đã thêm sinh viên.");
        }

        static void SuaSV()
        {
            SinhVien sv = svBus.TimTheoMa(Nhap("Mã sinh viên cần sửa: "));
            if (sv == null)
            {
                Console.WriteLine("Không tìm thấy sinh viên.");
                return;
            }
            Console.WriteLine("(Nhấn Enter để giữ nguyên giá trị cũ)");
            sv.HoTen = NhapSua("Họ tên", sv.HoTen);
            sv.GioiTinh = NhapSua("Giới tính", sv.GioiTinh);
            sv.NgaySinh = NhapSua("Ngày sinh (dd/MM/yyyy)", sv.NgaySinh);
            sv.SoDienThoai = NhapSua("Số điện thoại", sv.SoDienThoai);
            sv.MaLop = NhapSua("Mã lớp", sv.MaLop);
            string diem = NhapSua("Điểm", sv.Diem.ToString("0.##", CultureInfo.InvariantCulture));
            sv.Diem = DocDiem(diem);
            sv.Email = NhapSua("Email", sv.Email);
            svBus.Sua(sv);
            BaoThanhCong("Đã cập nhật sinh viên.");
        }

        static void SuaLop()
        {
            LopHoc lop = lopBus.TimTheoMa(Nhap("Mã lớp cần sửa: "));
            if (lop == null)
            {
                Console.WriteLine("Không tìm thấy lớp.");
                return;
            }
            lop.TenLop = NhapSua("Tên lớp", lop.TenLop);
            lopBus.Sua(lop);
            BaoThanhCong("Đã cập nhật lớp.");
        }

        // ---------- Hàm hỗ trợ nhập/xuất ----------

        static string Nhap(string nhan)
        {
            Console.Write(nhan);
            return Console.ReadLine()?.Trim() ?? "";
        }

        static string NhapSua(string nhan, string cu)
        {
            Console.Write($"{nhan} [{cu}]: ");
            string s = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(s) ? cu : s;
        }

        static double DocDiem(string s)
        {
            if (!double.TryParse(s.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                throw new FormatException("Điểm phải là một số (ví dụ 8.5).");
            return d;
        }

        static void InDanhSachSV(List<SinhVien> ds)
        {
            if (ds.Count == 0)
            {
                Console.WriteLine("(Không có dữ liệu)");
                return;
            }
            Console.WriteLine($"{"Mã SV",-8}{"Họ tên",-22}{"GT",-6}{"Ngày sinh",-12}{"SĐT",-13}{"Lớp",-9}{"Điểm",-6}Email");
            foreach (SinhVien s in ds)
            {
                Console.WriteLine($"{s.MaSV,-8}{s.HoTen,-22}{s.GioiTinh,-6}{s.NgaySinh,-12}{s.SoDienThoai,-13}{s.MaLop,-9}{s.Diem,-6:0.0}{s.Email}");
            }
        }

        static void InDanhSachLop(List<LopHoc> ds)
        {
            if (ds.Count == 0)
            {
                Console.WriteLine("(Không có dữ liệu)");
                return;
            }
            Console.WriteLine($"{"Mã lớp",-10}Tên lớp");
            foreach (LopHoc l in ds)
            {
                Console.WriteLine($"{l.MaLop,-10}{l.TenLop}");
            }
        }

        static void BaoLoi(string noiDung)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("LỖI: " + noiDung);
            Console.ResetColor();
        }

        static void BaoThanhCong(string noiDung)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(noiDung);
            Console.ResetColor();
        }
    }
}
