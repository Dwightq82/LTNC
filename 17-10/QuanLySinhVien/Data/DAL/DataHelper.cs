using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLySinhVien.Data.DAL
{
    public class DataHelper
    {
        // ĐỔI Ở ĐÂY nếu máy bạn dùng SQL Server khác.
        // Ví dụ: @".\SQLEXPRESS"  hoặc  @"TENMAY\TENINSTANCE"
        public const string ServerName = @"(localdb)\MSSQLLocalDB";
        public const string DatabaseName = "QLSV";

        private static string BuildConnectionString(string database)
        {
            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                InitialCatalog = database,
                IntegratedSecurity = true,
                TrustServerCertificate = true
            };
            return b.ConnectionString;
        }

        private readonly string connectionString = BuildConnectionString(DatabaseName);

        // Mỗi lần truy vấn dùng 1 kết nối riêng và tự đóng bằng using.
        // Lỗi SQL (SqlException) được ném lên cho lớp gọi xử lý.

        // INSERT / UPDATE / DELETE: trả về số dòng bị ảnh hưởng.
        public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // SELECT: trả về DataTable.
        public DataTable GetDataTable(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Tự tạo database, bảng và dữ liệu mẫu nếu chưa có (chạy 1 lần khi mở chương trình).
        public static void InitDatabase()
        {
            using (SqlConnection con = new SqlConnection(BuildConnectionString("master")))
            using (SqlCommand cmd = new SqlCommand(
                $"IF DB_ID(N'{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}];", con))
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }

            string[] statements =
            {
                @"IF OBJECT_ID(N'dbo.LopHoc', N'U') IS NULL
                  CREATE TABLE dbo.LopHoc (
                      MaLop  NVARCHAR(20)  NOT NULL PRIMARY KEY,
                      TenLop NVARCHAR(100) NOT NULL);",

                @"IF OBJECT_ID(N'dbo.SinhVien', N'U') IS NULL
                  CREATE TABLE dbo.SinhVien (
                      MaSV        NVARCHAR(20)  NOT NULL PRIMARY KEY,
                      HoTen       NVARCHAR(100) NOT NULL,
                      GioiTinh    NVARCHAR(10)  NULL,
                      NgaySinh    DATE          NULL,
                      SoDienThoai NVARCHAR(20)  NULL,
                      MaLop       NVARCHAR(20)  NULL REFERENCES dbo.LopHoc(MaLop),
                      Diem        FLOAT         NULL,
                      Email       NVARCHAR(100) NULL);",

                @"IF NOT EXISTS (SELECT 1 FROM dbo.LopHoc)
                  INSERT INTO dbo.LopHoc (MaLop, TenLop) VALUES
                      (N'CNTT01', N'Công nghệ thông tin 01'),
                      (N'CNTT02', N'Công nghệ thông tin 02');",

                @"IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien)
                  INSERT INTO dbo.SinhVien (MaSV, HoTen, GioiTinh, NgaySinh, SoDienThoai, MaLop, Diem, Email) VALUES
                      (N'SV001', N'Nguyễn Văn An', N'Nam', '2005-03-12', N'0901234567', N'CNTT01', 8.5, N'an@example.com'),
                      (N'SV002', N'Trần Thị Bình', N'Nữ',  '2005-07-21', N'0912345678', N'CNTT01', 7.0, N'binh@example.com'),
                      (N'SV003', N'Lê Hoàng Cường', N'Nam', '2004-11-02', N'0923456789', N'CNTT02', 9.0, N'cuong@example.com');"
            };

            using (SqlConnection con = new SqlConnection(BuildConnectionString(DatabaseName)))
            {
                con.Open();
                foreach (string sql in statements)
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
