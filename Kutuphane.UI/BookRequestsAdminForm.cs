using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Kutuphane.DAL;
using MySqlConnector;

namespace Kutuphane.UI
{
    public partial class BookRequestsAdminForm : Form
    {
        private int _selectedRequestId = 0;

        // ✅ Arka planı birden fazla kez soldurmayı engelle
        private bool _bgFaded = false;

        public BookRequestsAdminForm()
        {
            InitializeComponent();

            // ✅ Sadece DataBindingComplete'ı burada bağlayalım (çift event olmasın)
            dgvRequests.DataBindingComplete += (s, e) => ApplyHeaders();
        }

        private void BookRequestsAdminForm_Load(object sender, EventArgs e)
        {
            EnsureBackgroundIsBehind();
            FadeBackgroundOnce(0.55f);

            SetupGrid();
            LoadRequests();
        }

        // ✅ pictureBox1 kesin arkada kalsın, diğerleri önde olsun
        private void EnsureBackgroundIsBehind()
        {
            try
            {
                if (pictureBox1 != null)
                {
                    pictureBox1.Dock = DockStyle.Fill;
                    pictureBox1.SendToBack();
                }

                dgvRequests?.BringToFront();
                btnApprove?.BringToFront();
                btnReject?.BringToFront();
            }
            catch { }
        }

        // ✅ Sadece 1 kere soldur
        private void FadeBackgroundOnce(float opacity)
        {
            if (_bgFaded) return;
            _bgFaded = true;

            try
            {
                if (pictureBox1 != null && pictureBox1.Image != null)
                {
                    pictureBox1.Image = SetImageOpacity(pictureBox1.Image, opacity);
                    pictureBox1.Refresh();
                }
            }
            catch { }
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

        private void SetupGrid()
        {
            dgvRequests.ReadOnly = true;
            dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRequests.MultiSelect = false;
            dgvRequests.AllowUserToAddRows = false;
            dgvRequests.RowHeadersVisible = false;
        }

        private void LoadRequests()
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
SELECT
    r.RequestId,
    r.MemberId,
    r.RequestedTitle,
    r.RequestedAuthor,
    r.RequestedPublisher,
    r.Note,
    r.Status,
    r.RequestDate
FROM book_requests r
ORDER BY r.RequestDate DESC;";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvRequests.DataSource = dt;
                    }
                }

                _selectedRequestId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Talepler yüklenemedi: " + ex.Message);
            }
        }

        private void ApplyHeaders()
        {
            foreach (DataGridViewColumn col in dgvRequests.Columns)
            {
                switch (col.DataPropertyName)
                {
                    case "RequestId": col.HeaderText = "Talep No"; break;
                    case "MemberId": col.HeaderText = "Üye Id"; break;
                    case "RequestedTitle": col.HeaderText = "Kitap Adı"; break;
                    case "RequestedAuthor": col.HeaderText = "Yazar"; break;
                    case "RequestedPublisher": col.HeaderText = "Yayınevi"; break;
                    case "Note": col.HeaderText = "Not"; break;
                    case "Status": col.HeaderText = "Durum"; break;
                    case "RequestDate": col.HeaderText = "Tarih"; break;
                }
            }
        }

        private void dgvRequests_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvRequests.Rows[e.RowIndex];
            _selectedRequestId = Convert.ToInt32(row.Cells["RequestId"].Value);
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            UpdateStatus("Onaylandı");
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            UpdateStatus("Reddedildi");
        }

        private void UpdateStatus(string newStatus)
        {
            if (_selectedRequestId <= 0)
            {
                MessageBox.Show("❗ Lütfen tablodan bir talep seç.");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "UPDATE book_requests SET Status=@s WHERE RequestId=@id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@s", newStatus);
                        cmd.Parameters.AddWithValue("@id", _selectedRequestId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"✅ Talep güncellendi: {newStatus}");
                _selectedRequestId = 0;
                LoadRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Durum güncelleme hatası: " + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRequests();
        }
    }
}
