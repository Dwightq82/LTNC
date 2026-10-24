using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLTV_Project
{
    // Thêm class Program và hàm Main để sửa lỗi CS5001
    class Program
    {
        static void Main(string[] args)
        {
            // Không cần viết gì ở đây, chỉ cần có hàm Main để Project có thể Build được
        }
    }

    // Lớp thực thể 1
    public class TacGia
    {
        [Key]
        public int TacGiaId { get; set; }

        [Required]
        public string TenTacGia { get; set; }

        public virtual ICollection<Sach> CacSach { get; set; } = new List<Sach>();
    }

    // Lớp thực thể 2
    public class Sach
    {
        [Key]
        public int SachId { get; set; }

        [Required]
        public string TenSach { get; set; }

        public int TacGiaId { get; set; }

        public virtual TacGia TacGia { get; set; }
    }

    // Lớp DbContext
    public class QLTVDBContext : DbContext
    {
        public DbSet<TacGia> TacGias { get; set; }
        public DbSet<Sach> Sachs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=InitialLibraryDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }
}