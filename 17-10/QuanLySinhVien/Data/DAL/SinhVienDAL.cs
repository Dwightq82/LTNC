using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien.Data.DAL
{
    public class SinhVienDAL
    {
        private readonly DataHelper dataHelper = new DataHelper();

        // Đọc ngày sinh dạng dd/MM/yyyy (hoặc d/M/yyyy)
        public static bool TryParseNgaySinh(string s, out DateTime ngay)
        {
            return DateTime.TryParseExact(s?.Trim(), new[] { "dd/MM/yyyy", "d/M/yyyy" },
                CultureInfo.InvariantCulture, DateTimeStyles.None, out ngay);
        }

        // Chuyển 1 dòng dữ liệu thành đối tượng SinhVien
        private SinhVien MapToSinhVien(DataRow dr)
        {
            SinhVien sv = new SinhVien();
            sv.MaSV = dr["MaSV"].ToString();
            sv.HoTen = dr["HoTen"].ToString();
            sv.GioiTinh = dr["GioiTinh"].ToString();
            sv.NgaySinh = dr["NgaySinh"] is DateTime d ? d.ToString("dd/MM/yyyy") : "";
            sv.SoDienThoai = dr["SoDienThoai"].ToString();
            sv.MaLop = dr["MaLop"].ToString();
            sv.Diem = dr["Diem"] == DBNull.Value ? 0 : Convert.ToDouble(dr["Diem"]);
            sv.Email = dr["Email"].ToString();
            return sv;
        }

        private static object ToDb(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? DBNull.Value : s.Trim();
        }

        private static object ToDbDate(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return DBNull.Value;
            if (!TryParseNgaySinh(s, out DateTime d))
                throw new FormatException("Ngày sinh không hợp lệ (định dạng dd/MM/yyyy).");
            return d;
        }

        private static SqlParameter[] BuildParameters(SinhVien sv)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@MaSV", ToDb(sv.MaSV)),
                new SqlParameter("@MaLop", ToDb(sv.MaLop)),
                new SqlParameter("@HoTen", ToDb(sv.HoTen)),
                new SqlParameter("@GioiTinh", ToDb(sv.GioiTinh)),
                new SqlParameter("@NgaySinh", ToDbDate(sv.NgaySinh)),
                new SqlParameter("@Diem", sv.Diem),
                new SqlParameter("@SoDienThoai", ToDb(sv.SoDienThoai)),
                new SqlParameter("@Email", ToDb(sv.Email))
            };
        }

        public List<SinhVien> GetAllSinhVien()
        {
            try
            {
                DataTable dt = dataHelper.GetDataTable("SELECT * FROM SinhVien ORDER BY MaSV");
                List<SinhVien> list = new List<SinhVien>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(MapToSinhVien(dr));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách sinh viên: " + ex.Message, ex);
            }
        }

        // Trả về null nếu không có sinh viên nào có mã này
        public SinhVien GetSinhVienByMaSV(string maSV)
        {
            try
            {
                DataTable dt = dataHelper.GetDataTable(
                    "SELECT * FROM SinhVien WHERE MaSV = @MaSV",
                    new SqlParameter("@MaSV", ToDb(maSV)));
                return dt.Rows.Count > 0 ? MapToSinhVien(dt.Rows[0]) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm sinh viên theo mã: " + ex.Message, ex);
            }
        }

        public List<SinhVien> GetSinhViensByMaLop(string maLop)
        {
            try
            {
                DataTable dt = dataHelper.GetDataTable(
                    "SELECT * FROM SinhVien WHERE MaLop = @MaLop ORDER BY MaSV",
                    new SqlParameter("@MaLop", ToDb(maLop)));
                List<SinhVien> list = new List<SinhVien>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(MapToSinhVien(dr));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy sinh viên theo lớp: " + ex.Message, ex);
            }
        }

        public void AddSinhVien(SinhVien sv)
        {
            if (sv == null) throw new ArgumentNullException(nameof(sv), "Sinh viên không được để trống.");
            try
            {
                string sql = "INSERT INTO SinhVien (MaSV, MaLop, HoTen, GioiTinh, NgaySinh, Diem, SoDienThoai, Email) " +
                             "VALUES (@MaSV, @MaLop, @HoTen, @GioiTinh, @NgaySinh, @Diem, @SoDienThoai, @Email)";
                dataHelper.ExecuteNonQuery(sql, BuildParameters(sv));
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm sinh viên: " + ex.Message, ex);
            }
        }

        public void UpdateSinhVien(SinhVien sv)
        {
            if (sv == null) throw new ArgumentNullException(nameof(sv), "Sinh viên không được để trống.");
            int rows;
            try
            {
                string sql = "UPDATE SinhVien SET HoTen = @HoTen, MaLop = @MaLop, GioiTinh = @GioiTinh, " +
                             "NgaySinh = @NgaySinh, Diem = @Diem, SoDienThoai = @SoDienThoai, Email = @Email " +
                             "WHERE MaSV = @MaSV";
                rows = dataHelper.ExecuteNonQuery(sql, BuildParameters(sv));
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật sinh viên: " + ex.Message, ex);
            }
            if (rows == 0)
                throw new Exception($"Không tìm thấy sinh viên có mã '{sv.MaSV}' để cập nhật.");
        }

        public void DeleteSinhVien(string maSV)
        {
            int rows;
            try
            {
                rows = dataHelper.ExecuteNonQuery(
                    "DELETE FROM SinhVien WHERE MaSV = @MaSV",
                    new SqlParameter("@MaSV", ToDb(maSV)));
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa sinh viên: " + ex.Message, ex);
            }
            if (rows == 0)
                throw new Exception($"Không tìm thấy sinh viên có mã '{maSV}' để xóa.");
        }
    }
}
