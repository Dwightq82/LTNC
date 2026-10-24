using System.Data;
using Microsoft.Data.SqlClient;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien.Data.DAL
{
    public class LopHocDAL
    {
        private readonly DataHelper dataHelper = new DataHelper();

        private static object ToDb(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? DBNull.Value : s.Trim();
        }

        private LopHoc MapToLopHoc(DataRow dr)
        {
            LopHoc lop = new LopHoc();
            lop.MaLop = dr["MaLop"].ToString();
            lop.TenLop = dr["TenLop"].ToString();
            return lop;
        }

        public List<LopHoc> GetAllLopHoc()
        {
            try
            {
                DataTable dt = dataHelper.GetDataTable("SELECT * FROM LopHoc ORDER BY MaLop");
                List<LopHoc> list = new List<LopHoc>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(MapToLopHoc(dr));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách lớp học: " + ex.Message, ex);
            }
        }

        // Trả về null nếu không có lớp nào có mã này
        public LopHoc GetLopHocByMaLop(string maLop)
        {
            try
            {
                DataTable dt = dataHelper.GetDataTable(
                    "SELECT * FROM LopHoc WHERE MaLop = @MaLop",
                    new SqlParameter("@MaLop", ToDb(maLop)));
                return dt.Rows.Count > 0 ? MapToLopHoc(dt.Rows[0]) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm lớp học theo mã: " + ex.Message, ex);
            }
        }

        public void AddLopHoc(LopHoc lop)
        {
            if (lop == null) throw new ArgumentNullException(nameof(lop), "Lớp học không được để trống.");
            try
            {
                dataHelper.ExecuteNonQuery(
                    "INSERT INTO LopHoc (MaLop, TenLop) VALUES (@MaLop, @TenLop)",
                    new SqlParameter("@MaLop", ToDb(lop.MaLop)),
                    new SqlParameter("@TenLop", ToDb(lop.TenLop)));
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm lớp học: " + ex.Message, ex);
            }
        }

        public void UpdateLopHoc(LopHoc lop)
        {
            if (lop == null) throw new ArgumentNullException(nameof(lop), "Lớp học không được để trống.");
            int rows;
            try
            {
                rows = dataHelper.ExecuteNonQuery(
                    "UPDATE LopHoc SET TenLop = @TenLop WHERE MaLop = @MaLop",
                    new SqlParameter("@MaLop", ToDb(lop.MaLop)),
                    new SqlParameter("@TenLop", ToDb(lop.TenLop)));
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật lớp học: " + ex.Message, ex);
            }
            if (rows == 0)
                throw new Exception($"Không tìm thấy lớp học có mã '{lop.MaLop}' để cập nhật.");
        }

        public void DeleteLopHoc(string maLop)
        {
            int rows;
            try
            {
                rows = dataHelper.ExecuteNonQuery(
                    "DELETE FROM LopHoc WHERE MaLop = @MaLop",
                    new SqlParameter("@MaLop", ToDb(maLop)));
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa lớp học: " + ex.Message, ex);
            }
            if (rows == 0)
                throw new Exception($"Không tìm thấy lớp học có mã '{maLop}' để xóa.");
        }
    }
}
