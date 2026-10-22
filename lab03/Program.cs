using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Quanlisinhvien
    public class Nguoi
    {
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }

        public Nguoi(string hoTen, DateTime ngaySinh)
        {
            HoTen = hoTen;
            NgaySinh = ngaySinh;
        }

        public virtual string LayThongTin()
        {
            return $"Ho ten: {HoTen} | Ngay sinh: {NgaySinh:dd/MM/yyyy}";
        }
    }

    public class SinhVien : Nguoi
    {
        private double _diemTrungBinh;

        public string MaSinhVien { get; set; }
        public string MaLop { get; set; }

        public double DiemTrungBinh
        {
            get { return _diemTrungBinh; }
            set
            {
                if (double.IsNaN(value) || value < 0 || value > 10)
                {
                    throw new ArgumentOutOfRangeException(nameof(DiemTrungBinh), "Diem trung binh phai nam trong khoang 0-10.");
                }
                _diemTrungBinh = value;
            }
        }

        public SinhVien(string maSinhVien, string hoTen, DateTime ngaySinh, string maLop, double diemTrungBinh)
            : base(hoTen, ngaySinh)
        {
            MaSinhVien = maSinhVien;
            MaLop = maLop;
            DiemTrungBinh = diemTrungBinh;
        }

        public string XepLoai()
        {
            if (DiemTrungBinh >= 8.0) return "Gioi";
            if (DiemTrungBinh >= 6.5) return "Kha";
            if (DiemTrungBinh >= 5.0) return "Trung binh";
            return "Yeu";
        }

        public override string LayThongTin()
        {
            return $"{MaSinhVien,-10}{HoTen,-25}{NgaySinh:dd/MM/yyyy}  {MaLop,-10}{DiemTrungBinh,5:0.0}  {XepLoai()}";
        }
    }

    public class QuanLySinhVien
    {
        private readonly List<SinhVien> _danhSach = new List<SinhVien>();

        public bool Them(SinhVien sv)
        {
            if (TimTheoMa(sv.MaSinhVien) != null)
            {
                return false;
            }
            _danhSach.Add(sv);
            return true;
        }

        public bool Sua(string maSinhVien, double diemMoi)
        {
            SinhVien? sv = TimTheoMa(maSinhVien);
            if (sv == null)
            {
                return false;
            }
            sv.DiemTrungBinh = diemMoi;
            return true;
        }

        public bool Xoa(string maSinhVien)
        {
            SinhVien? sv = TimTheoMa(maSinhVien);
            if (sv == null)
            {
                return false;
            }
            return _danhSach.Remove(sv);
        }

        public SinhVien? TimTheoMa(string maSinhVien)
        {
            return _danhSach.FirstOrDefault(sv =>
                string.Equals(sv.MaSinhVien, maSinhVien, StringComparison.OrdinalIgnoreCase));
        }

        public List<SinhVien> TimTheoTen(string tuKhoa)
        {
            return _danhSach
                .Where(sv => sv.HoTen.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        public List<SinhVien> SapXepTheoDiem()
        {
            return _danhSach.OrderByDescending(sv => sv.DiemTrungBinh).ToList();
        }

        public List<SinhVien> LocSinhVienDat()
        {
            return _danhSach.Where(sv => sv.DiemTrungBinh >= 5.0).ToList();
        }

        public List<SinhVien> LayDanhSach()
        {
            return _danhSach.ToList();
        }
    }

    internal class Program
    {
        private static readonly QuanLySinhVien _quanLy = new QuanLySinhVien();

        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            bool tiepTuc = true;
            while (tiepTuc)
            {
                HienThiMenu();
                string luaChon = (Console.ReadLine() ?? "").Trim();
                Console.WriteLine();

                switch (luaChon)
                {
                    case "1":
                        ThemSinhVien();
                        break;
                    case "2":
                        HienThiDanhSach(_quanLy.LayDanhSach());
                        break;
                    case "3":
                        TimSinhVienTheoMa();
                        break;
                    case "4":
                        TimSinhVienTheoTen();
                        break;
                    case "5":
                        SuaDiem();
                        break;
                    case "6":
                        XoaSinhVien();
                        break;
                    case "7":
                        Console.WriteLine("Danh sach sap xep theo diem giam dan:");
                        HienThiDanhSach(_quanLy.SapXepTheoDiem());
                        break;
                    case "8":
                        Console.WriteLine("Danh sach sinh vien dat (diem >= 5):");
                        HienThiDanhSach(_quanLy.LocSinhVienDat());
                        break;
                    case "0":
                        tiepTuc = false;
                        Console.WriteLine("Ket thuc chuong trinh. Tam biet!");
                        break;
                    default:
                        Console.WriteLine("Lua chon khong hop le. Vui long chon lai.");
                        break;
                }

                if (tiepTuc)
                {
                    Console.WriteLine();
                    Console.Write("Nhan Enter de tiep tuc...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        private static void HienThiMenu()
        {
            Console.WriteLine("===== QUAN LY SINH VIEN =====");
            Console.WriteLine("1. Them sinh vien");
            Console.WriteLine("2. Xuat danh sach");
            Console.WriteLine("3. Tim sinh vien theo ma");
            Console.WriteLine("4. Tim sinh vien theo ten");
            Console.WriteLine("5. Sua diem trung binh");
            Console.WriteLine("6. Xoa sinh vien");
            Console.WriteLine("7. Sap xep theo diem giam dan");
            Console.WriteLine("8. Loc sinh vien dat");
            Console.WriteLine("0. Thoat");
            Console.Write("Chon chuc nang: ");
        }

        private static void ThemSinhVien()
        {
            Console.WriteLine("--- THEM SINH VIEN ---");

            string ma = NhapChuoi("Nhap ma sinh vien: ");
            if (_quanLy.TimTheoMa(ma) != null)
            {
                Console.WriteLine("Ma sinh vien da ton tai.");
                return;
            }

            string hoTen = NhapChuoi("Nhap ho ten: ");
            DateTime ngaySinh = NhapNgaySinh("Nhap ngay sinh (dd/MM/yyyy): ");
            string maLop = NhapChuoi("Nhap ma lop: ");
            double diem = NhapDiem("Nhap diem trung binh (0-10): ");

            SinhVien sv = new SinhVien(ma, hoTen, ngaySinh, maLop, diem);
            if (_quanLy.Them(sv))
            {
                Console.WriteLine($"Them thanh cong. Xep loai: {sv.XepLoai()}");
            }
            else
            {
                Console.WriteLine("Ma sinh vien da ton tai.");
            }
        }

        private static void TimSinhVienTheoMa()
        {
            Console.WriteLine("--- TIM SINH VIEN THEO MA ---");
            string ma = NhapChuoi("Nhap ma sinh vien: ");
            SinhVien? sv = _quanLy.TimTheoMa(ma);

            if (sv == null)
            {
                Console.WriteLine("Khong tim thay sinh vien.");
                return;
            }

            InTieuDe();
            Console.WriteLine(sv.LayThongTin());
        }

        private static void TimSinhVienTheoTen()
        {
            Console.WriteLine("--- TIM SINH VIEN THEO TEN ---");
            string tuKhoa = NhapChuoi("Nhap tu khoa ho ten: ");
            List<SinhVien> ketQua = _quanLy.TimTheoTen(tuKhoa);

            if (ketQua.Count == 0)
            {
                Console.WriteLine("Khong tim thay sinh vien nao.");
                return;
            }

            HienThiDanhSach(ketQua);
        }

        private static void SuaDiem()
        {
            Console.WriteLine("--- SUA DIEM TRUNG BINH ---");
            string ma = NhapChuoi("Nhap ma sinh vien: ");

            if (_quanLy.TimTheoMa(ma) == null)
            {
                Console.WriteLine("Khong tim thay sinh vien.");
                return;
            }

            double diemMoi = NhapDiem("Nhap diem trung binh moi (0-10): ");
            if (_quanLy.Sua(ma, diemMoi))
            {
                Console.WriteLine("Cap nhat diem thanh cong.");
            }
            else
            {
                Console.WriteLine("Cap nhat that bai.");
            }
        }

        private static void XoaSinhVien()
        {
            Console.WriteLine("--- XOA SINH VIEN ---");
            string ma = NhapChuoi("Nhap ma sinh vien: ");

            if (_quanLy.Xoa(ma))
            {
                Console.WriteLine("Xoa sinh vien thanh cong.");
            }
            else
            {
                Console.WriteLine("Khong tim thay sinh vien.");
            }
        }

        private static void InTieuDe()
        {
            Console.WriteLine($"{"Ma SV",-10}{"Ho ten",-25}{"Ngay sinh",-12}{"Lop",-10}{"Diem",5}  Xep loai");
            Console.WriteLine(new string('-', 75));
        }

        private static void HienThiDanhSach(List<SinhVien> danhSach)
        {
            if (danhSach.Count == 0)
            {
                Console.WriteLine("Danh sach rong.");
                return;
            }

            InTieuDe();
            foreach (SinhVien sv in danhSach)
            {
                Console.WriteLine(sv.LayThongTin());
            }
            Console.WriteLine(new string('-', 75));
            Console.WriteLine($"Tong so sinh vien: {danhSach.Count}");
        }

        private static string NhapChuoi(string thongBao)
        {
            while (true)
            {
                Console.Write(thongBao);
                string s = (Console.ReadLine() ?? "").Trim();
                if (s.Length > 0)
                {
                    return s;
                }
                Console.WriteLine("Khong duoc de trong. Vui long nhap lai.");
            }
        }

        private static DateTime NhapNgaySinh(string thongBao)
        {
            string[] dinhDang = { "dd/MM/yyyy", "d/M/yyyy" };

            while (true)
            {
                Console.Write(thongBao);
                string s = (Console.ReadLine() ?? "").Trim();

                if (!DateTime.TryParseExact(s, dinhDang, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime ngaySinh))
                {
                    Console.WriteLine("Ngay sinh khong hop le. Vui long nhap theo dang dd/MM/yyyy.");
                    continue;
                }

                if (ngaySinh > DateTime.Today)
                {
                    Console.WriteLine("Ngay sinh khong duoc lon hon ngay hien tai.");
                    continue;
                }

                return ngaySinh;
            }
        }

        private static double NhapDiem(string thongBao)
        {
            while (true)
            {
                Console.Write(thongBao);
                string s = (Console.ReadLine() ?? "").Trim().Replace(',', '.');

                if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double diem))
                {
                    Console.WriteLine("Du lieu khong hop le. Vui long nhap mot so.");
                    continue;
                }

                if (double.IsNaN(diem) || diem < 0 || diem > 10)
                {
                    Console.WriteLine("Diem khong hop le (phai tu 0 den 10). Vui long nhap lai.");
                    continue;
                }

                return diem;
            }
        }
    }
}