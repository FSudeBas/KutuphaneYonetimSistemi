using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Kutuphane.DAL;
using MySqlConnector;

namespace Kutuphane.UI
{
    public partial class BookRequestForm : Form
    {
        // ✅ Üye id'si bu form açılırken gönderilecek
        private readonly int _memberId;

        // ✅ Arka planı birden fazla kez soldurmayı engelle
        private bool _bgFaded = false;

        // ✅ Default ctor: designer bozulmasın diye kalsın
        public BookRequestForm() : this(0)
        {
        }

        // ✅ Asıl kullanım: new BookRequestForm(memberId)
        public BookRequestForm(int memberId)
        {
            InitializeComponent();

            _memberId = memberId;

            // ✅ Eventleri garanti bağla (Designer'da bağlı olsa da sorun olmaz)
            this.Load += BookRequestForm_Load;

            // Butonun adı sende "btnSendRequest" ise:
            if (btnSendRequest != null)
                btnSendRequest.Click += btnSendRequest_Click;
        }

        private void BookRequestForm_Load(object sender, EventArgs e)
        {
            // ✅ Arka planı soldur (1 kere)
            FadeBackgroundOnce(0.55f);

            // Form açılınca imleç kitap adına gelsin
            if (txtRequestedTitle != null)
                txtRequestedTitle.Focus();
        }

        // ✅ Sadece 1 kere soldur
        private void FadeBackgroundOnce(float opacity)
        {
            if (_bgFaded) return;
            _bgFaded = true;

            try
            {
                // 1) PictureBox varsa (adı sende farklıysa burayı değiştir!)
                // Örn: picBgRequest / picBgBookRequest / picBg
                Control[] found = this.Controls.Find("picBgRequest", true);
                if (found.Length > 0 && found[0] is PictureBox pb && pb.Image != null)
                {
                    pb.Dock = DockStyle.Fill;
                    pb.SendToBack();

                    pb.Image = SetImageOpacity(pb.Image, opacity);

                    // diğer kontroller öne gelsin
                    foreach (Control c in this.Controls)
                        if (c != pb) c.BringToFront();

                    pb.Refresh();
                    return;
                }

                // 2) Formun BackgroundImage'ı varsa
                if (this.BackgroundImage != null)
                    this.BackgroundImage = SetImageOpacity(this.BackgroundImage, opacity);
            }
            catch
            {
                // sessiz geç
            }
        }

        // ✅ Opacity fonksiyonu
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

        // ✅ Talep Gönder
        private void btnSendRequest_Click(object sender, EventArgs e)
        {
            if (_memberId <= 0)
            {
                MessageBox.Show("❌ Üye bilgisi bulunamadı. (MemberId=0)\nBu formu açarken MemberId göndermelisin.");
                return;
            }

            string title = txtRequestedTitle.Text.Trim();
            string author = txtRequestedAuthor.Text.Trim();
            string publisher = txtRequestedPublisher.Text.Trim();
            string note = txtNote.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("❌ Kitap Adı boş olamaz!");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
INSERT INTO book_requests
    (MemberId, RequestedTitle, RequestedAuthor, RequestedPublisher, Note, Status)
VALUES
    (@memberId, @title, @author, @publisher, @note, 'Beklemede');";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@memberId", _memberId);
                        cmd.Parameters.AddWithValue("@title", title);

                        cmd.Parameters.AddWithValue("@author",
                            string.IsNullOrWhiteSpace(author) ? (object)DBNull.Value : author);

                        cmd.Parameters.AddWithValue("@publisher",
                            string.IsNullOrWhiteSpace(publisher) ? (object)DBNull.Value : publisher);

                        cmd.Parameters.AddWithValue("@note",
                            string.IsNullOrWhiteSpace(note) ? (object)DBNull.Value : note);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Talebin gönderildi! (Beklemede)");

                // temizle
                txtRequestedTitle.Clear();
                txtRequestedAuthor.Clear();
                txtRequestedPublisher.Clear();
                txtNote.Clear();
                txtRequestedTitle.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Talep gönderme hatası: " + ex.Message);
            }
        }
    }
}
