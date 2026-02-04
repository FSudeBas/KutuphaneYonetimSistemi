using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kutuphane.DAL;
using MySqlConnector;
using System.Drawing;
using System.Drawing.Imaging;

namespace Kutuphane.UI
{
    public partial class UyePanelForm : Form
    {
        private readonly int _memberId;
        private readonly string _focus;

        // ✅ Arka planı 1 kere soldurmak için
        private bool _bgFaded = false;

        public UyePanelForm(int memberId, string focus = "emanet")
        {
            InitializeComponent();

            _memberId = memberId;
            _focus = string.IsNullOrWhiteSpace(focus) ? "emanet" : focus;

            // ✅ Designer patlamasın
            this.Load += UyePanelForm_Load;

            // Events (null kontrolü ile)
            if (btnRefreshBorrows != null) btnRefreshBorrows.Click += (s, e) => LoadMyBorrows();
            if (btnRefreshRequests != null) btnRefreshRequests.Click += (s, e) => LoadMyRequests();
            if (chkOnlyActive != null) chkOnlyActive.CheckedChanged += (s, e) => LoadMyBorrows();

            if (dgvMyBorrows != null) dgvMyBorrows.DataBindingComplete += (s, e) => ApplyBorrowHeaders();
            if (dgvMyRequests != null) dgvMyRequests.DataBindingComplete += (s, e) => ApplyRequestHeaders();

            // ✅ Tab başlıklarını constructor’da güvenli şekilde set et
            EnsureLayoutSafe();
        }

        // ✅ pictureBox1 ve pictureBox2'yi 1 kere soldur
        private void FadeBackgroundOnce(float opacity)
        {
            if (_bgFaded) return;
            _bgFaded = true;

            try
            {
                // Emanetlerim sekmesi arka planı
                if (pictureBox1 != null && pictureBox1.Image != null)
                {
                    pictureBox1.Image = SetImageOpacity(pictureBox1.Image, opacity);
                    pictureBox1.SendToBack();
                }

                // Taleplerim sekmesi arka planı
                if (pictureBox2 != null && pictureBox2.Image != null)
                {
                    pictureBox2.Image = SetImageOpacity(pictureBox2.Image, opacity);
                    pictureBox2.SendToBack();
                }

                // Gridler ve butonlar öne gelsin (fotoğraf üstüne çıkmasın)
                dgvMyBorrows?.BringToFront();
                dgvMyRequests?.BringToFront();
                btnRefreshBorrows?.BringToFront();
                btnRefreshRequests?.BringToFront();
                chkOnlyActive?.BringToFront();
            }
            catch
            {
                // sessiz geç
            }
        }

        private Image SetImageOpacity(Image image, float opacity)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;

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

        // Formdaki tab control'u isim bağımsız bul (tabUye / tabControl1 vs fark etmez)
        private TabControl GetTabControl()
        {
            // 1) Eğer gerçekten tabUye varsa zaten buradan çalışır (field)
            if (this.Controls.Find("tabUye", true).FirstOrDefault() is TabControl t1) return t1;

            // 2) Aksi halde formdaki ilk TabControl'u bul
            return this.Controls.OfType<TabControl>().FirstOrDefault()
                ?? this.Controls.Cast<Control>().SelectMany(GetAllControls).OfType<TabControl>().FirstOrDefault();
        }

        private static System.Collections.Generic.IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (var child in GetAllControls(c))
                    yield return child;
            }
        }

        private void EnsureLayoutSafe()
        {
            try
            {
                var tab = GetTabControl();
                if (tab == null) return;

                tab.Dock = DockStyle.Fill;
                tab.BringToFront();

                if (tab.TabPages.Count >= 1) tab.TabPages[0].Text = "Emanetlerim";
                if (tab.TabPages.Count >= 2) tab.TabPages[1].Text = "Taleplerim";
            }
            catch
            {
                // designer-safe
            }
        }

        private void UyePanelForm_Load(object sender, EventArgs e)
        {
            // ✅ Designer açılırken DB çalışmasın
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            // ✅ Arka planları soldur (1 kere)
            FadeBackgroundOnce(0.55f);

            if (_memberId <= 0)
            {
                MessageBox.Show("❌ Üye bilgisi bulunamadı (MemberId=0).");
                Close();
                return;
            }

            // TabControl'u isim bağımsız al
            var tab = GetTabControl();
            if (tab == null)
            {
                MessageBox.Show("❌ TabControl bulunamadı. (Designer'da TabControl silinmiş olabilir)");
                Close();
                return;
            }

            // DataGridView kontrolleri yoksa patlamasın
            if (dgvMyBorrows == null || dgvMyRequests == null)
            {
                MessageBox.Show("❌ DataGridView kontrolleri bulunamadı (dgvMyBorrows / dgvMyRequests).");
                Close();
                return;
            }

            SetupGrid(dgvMyBorrows);
            SetupGrid(dgvMyRequests);

            LoadMyBorrows();
            LoadMyRequests();

            // Hangi sekmede açılacak?
            if (_focus == "talep" && tab.TabPages.Count > 1)
                tab.SelectedIndex = 1; // Taleplerim
            else
                tab.SelectedIndex = 0; // Emanetlerim
        }

        private void SetupGrid(DataGridView dgv)
        {
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ✅ Üyenin talepleri
        private void LoadMyRequests()
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
SELECT
    RequestId,
    RequestedTitle,
    RequestedAuthor,
    RequestedPublisher,
    Note,
    Status,
    RequestDate
FROM book_requests
WHERE MemberId = @mid
ORDER BY RequestDate DESC;";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@mid", _memberId);
                        var dt = new DataTable();
                        da.Fill(dt);
                        dgvMyRequests.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Talepler yüklenemedi: " + ex.Message);
            }
        }

        private void ApplyRequestHeaders()
        {
            foreach (DataGridViewColumn col in dgvMyRequests.Columns)
            {
                switch (col.DataPropertyName)
                {
                    case "RequestId": col.Visible = false; break;
                    case "RequestedTitle": col.HeaderText = "Kitap Adı"; break;
                    case "RequestedAuthor": col.HeaderText = "Yazar"; break;
                    case "RequestedPublisher": col.HeaderText = "Yayınevi"; break;
                    case "Note": col.HeaderText = "Not"; break;
                    case "Status": col.HeaderText = "Durum"; break;
                    case "RequestDate": col.HeaderText = "Tarih"; break;
                }
            }
        }

        // ✅ Üyenin emanetleri
        private void LoadMyBorrows()
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
SELECT
    b.BorrowId,
    bk.Title AS BookTitle,
    b.BorrowDate,
    b.DueDate,
    b.ReturnDate,
    CASE WHEN b.ReturnDate IS NULL THEN 'Aktif' ELSE 'İade Edildi' END AS StatusText,
    CASE 
        WHEN b.ReturnDate IS NULL AND b.DueDate < CURDATE() THEN DATEDIFF(CURDATE(), b.DueDate)
        ELSE 0
    END AS LateDays
FROM borrows b
JOIN books bk ON bk.BookId = b.BookId
WHERE b.MemberId = @mid
ORDER BY b.BorrowDate DESC;";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@mid", _memberId);
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (chkOnlyActive != null && chkOnlyActive.Checked)
                        {
                            var dv = dt.DefaultView;
                            dv.RowFilter = "ReturnDate IS NULL";
                            dgvMyBorrows.DataSource = dv;
                        }
                        else
                        {
                            dgvMyBorrows.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Emanetler yüklenemedi: " + ex.Message);
            }
        }

        private void ApplyBorrowHeaders()
        {
            foreach (DataGridViewColumn col in dgvMyBorrows.Columns)
            {
                switch (col.DataPropertyName)
                {
                    case "BorrowId": col.HeaderText = "Emanet ID"; break;
                    case "BookTitle": col.HeaderText = "Kitap"; break;
                    case "BorrowDate": col.HeaderText = "Alış Tarihi"; break;
                    case "DueDate": col.HeaderText = "Teslim Tarihi"; break;
                    case "ReturnDate": col.HeaderText = "İade Tarihi"; break;
                    case "StatusText": col.HeaderText = "Durum"; break;
                    case "LateDays": col.HeaderText = "Gecikme"; break;
                }
            }
        }
    }
}
