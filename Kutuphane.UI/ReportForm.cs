using Kutuphane.DAL;
using MySqlConnector;
using System;
using System.Data;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

// ✅ Fotoğraf soldurma için gerekli
using System.Drawing;
using System.Drawing.Imaging;

namespace Kutuphane.UI
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            // ✅ Arka plan fotoğrafını soldur
            if (picBgReport != null && picBgReport.Image != null)
            {
                picBgReport.Image = SetImageOpacity(picBgReport.Image, 0.55f); // 0.55 ideal
            }

            FillReportTypes();

            // ✅ Default tarih aralığı (son 30 gün)
            dtEnd.Value = DateTime.Now;
            dtStart.Value = DateTime.Now.AddDays(-30);

            // ✅ İlk raporu seçili yap
            if (cmbReportType.Items.Count > 0)
                cmbReportType.SelectedIndex = 0;
        }

        // ✅ Fotoğrafı soldurma fonksiyonu (Opacity)
        private Image SetImageOpacity(Image image, float opacity)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity; // 0.0 = şeffaf, 1.0 = normal

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                g.DrawImage(
                    image,
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel,
                    attributes
                );
            }

            return bmp;
        }

        // ✅ ComboBox içine rapor tiplerini doldur
        private void FillReportTypes()
        {
            cmbReportType.Items.Clear();

            cmbReportType.Items.Add("En Çok Ödünç Alınan Kitaplar");
            cmbReportType.Items.Add("Geciken Kitaplar");
            cmbReportType.Items.Add("Aktif Üyeler");
            cmbReportType.Items.Add("Kategori Bazlı Kitap Sayısı");
            cmbReportType.Items.Add("Aylık Ödünç Alma İstatistiği");

            // ✅ YENİ RAPOR
            cmbReportType.Items.Add("Toplam Kitap Sayısı");
        }

        // ✅ Rapor Getir butonu
        private void btnGetReport_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedItem == null)
            {
                MessageBox.Show("❌ Lütfen rapor türü seç!");
                return;
            }

            DateTime start = dtStart.Value.Date;
            DateTime end = dtEnd.Value.Date;

            if (end < start)
            {
                MessageBox.Show("❌ Bitiş tarihi, başlangıçtan küçük olamaz!");
                return;
            }

            string reportType = cmbReportType.SelectedItem.ToString();

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "";
                    bool useDateFilter = true;

                    // ✅ Rapor türüne göre SQL seç
                    switch (reportType)
                    {
                        case "En Çok Ödünç Alınan Kitaplar":
                            sql = @"
                                SELECT 
                                    b.Title AS Kitap,
                                    b.Author AS Yazar,
                                    COUNT(*) AS OduncSayisi
                                FROM borrows br
                                INNER JOIN books b ON br.BookId = b.BookId
                                WHERE br.BorrowDate BETWEEN @start AND @end
                                GROUP BY br.BookId, b.Title, b.Author
                                ORDER BY OduncSayisi DESC
                            ";
                            break;

                        case "Geciken Kitaplar":
                            sql = @"
                                SELECT
                                    CONCAT(m.FirstName,' ',m.LastName) AS Uye,
                                    CONCAT(b.Title,' - ',b.Author) AS Kitap,
                                    br.BorrowDate AS AlisTarihi,
                                    br.DueDate AS TeslimTarihi,
                                    DATEDIFF(CURDATE(), br.DueDate) AS GecikmeGun
                                FROM borrows br
                                INNER JOIN members m ON br.MemberId = m.MemberId
                                INNER JOIN books b ON br.BookId = b.BookId
                                WHERE br.IsReturned = 0
                                  AND br.DueDate < CURDATE()
                                  AND br.BorrowDate BETWEEN @start AND @end
                                ORDER BY GecikmeGun DESC
                            ";
                            break;

                        case "Aktif Üyeler":
                            sql = @"
                                SELECT
                                    CONCAT(m.FirstName,' ',m.LastName) AS Uye,
                                    COUNT(*) AS OduncSayisi
                                FROM borrows br
                                INNER JOIN members m ON br.MemberId = m.MemberId
                                WHERE br.BorrowDate BETWEEN @start AND @end
                                GROUP BY br.MemberId, m.FirstName, m.LastName
                                ORDER BY OduncSayisi DESC
                            ";
                            break;

                        case "Kategori Bazlı Kitap Sayısı":
                            useDateFilter = false;
                            sql = @"
                                SELECT
                                    COALESCE(c.CategoryName, 'Kategorisiz') AS Kategori,
                                    COUNT(*) AS KitapSayisi
                                FROM books b
                                LEFT JOIN categories c ON b.CategoryId = c.CategoryId
                                GROUP BY c.CategoryId, c.CategoryName
                                ORDER BY KitapSayisi DESC
                            ";
                            break;

                        case "Aylık Ödünç Alma İstatistiği":
                            sql = @"
                                SELECT
                                    DATE_FORMAT(br.BorrowDate, '%Y-%m') AS Ay,
                                    COUNT(*) AS OduncSayisi
                                FROM borrows br
                                WHERE br.BorrowDate BETWEEN @start AND @end
                                GROUP BY Ay
                                ORDER BY Ay
                            ";
                            break;

                        case "Toplam Kitap Sayısı":
                            // ✅ Tarih filtresi gereksiz
                            useDateFilter = false;

                            // ✅ Toplam = Raf Stok + Emanette (IsReturned=0)
                            // ✅ Altına da kitap listesi: kitap adı + sadece yazar
                            sql = @"
                                SELECT SortOrder, Bilgi, Deger, Detay
                                FROM
                                (
                                    SELECT 
                                        0 AS SortOrder,
                                        'Toplam Kitap (Envanter)' AS Bilgi,
                                        CAST(
                                            (
                                                (SELECT IFNULL(SUM(Stock),0) FROM books) +
                                                (SELECT COUNT(*) FROM borrows WHERE IsReturned = 0)
                                            ) AS CHAR
                                        ) AS Deger,
                                        'Raf Stok + Emanette' AS Detay

                                    UNION ALL

                                    SELECT 
                                        1 AS SortOrder,
                                        'Raf Stok Toplamı' AS Bilgi,
                                        CAST((SELECT IFNULL(SUM(Stock),0) FROM books) AS CHAR) AS Deger,
                                        'Şu an rafta mevcut' AS Detay

                                    UNION ALL

                                    SELECT 
                                        2 AS SortOrder,
                                        'Emanette Toplam' AS Bilgi,
                                        CAST((SELECT COUNT(*) FROM borrows WHERE IsReturned = 0) AS CHAR) AS Deger,
                                        'Şu an ödünçte' AS Detay

                                    UNION ALL

                                    SELECT
                                        3 AS SortOrder,
                                        'Kitap' AS Bilgi,
                                        b.Title AS Deger,
                                        b.Author AS Detay
                                    FROM books b
                                ) t
                                ORDER BY t.SortOrder, t.Deger;
                            ";
                            break;

                        default:
                            MessageBox.Show("❌ Geçersiz rapor türü seçildi!");
                            return;
                    }

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        if (useDateFilter)
                        {
                            da.SelectCommand.Parameters.AddWithValue("@start", start);
                            da.SelectCommand.Parameters.AddWithValue("@end", end);
                        }

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // ✅ SortOrder kolonunu gizleyelim
                        dgvReport.DataSource = dt;
                        BeautifyReportGrid();

                        if (dgvReport.Columns.Contains("SortOrder"))
                            dgvReport.Columns["SortOrder"].Visible = false;

                        // İstersen başlıkları sabitle
                        if (dgvReport.Columns.Contains("Bilgi")) dgvReport.Columns["Bilgi"].HeaderText = "Bilgi";
                        if (dgvReport.Columns.Contains("Deger")) dgvReport.Columns["Deger"].HeaderText = "Değer";
                        if (dgvReport.Columns.Contains("Detay")) dgvReport.Columns["Detay"].HeaderText = "Detay";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Rapor çekme hatası: " + ex.Message);
            }
        }

        // ✅ DataGrid görünümünü güzelleştir
        private void BeautifyReportGrid()
        {
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReport.ReadOnly = true;
            dgvReport.AllowUserToAddRows = false;
            dgvReport.RowHeadersVisible = false;
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReport.MultiSelect = false;
        }

        private void dgvReport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}