using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Kutuphane.DAL;
using MySqlConnector;

namespace Kutuphane.UI
{
    public partial class BooksForm : Form
    {
        private int selectedBookId = 0;
        private DataTable categoriesTable;

        private readonly bool _readOnlyMode;
        private readonly int _memberId; // ✅ üye için lazım (talep oluşturma)

        // ✅ Arka planı birden fazla kez soldurmayı engelle
        private bool _bgFaded = false;

        // ✅ Varsayılan: admin gibi
        public BooksForm() : this(false, 0) { }

        // ✅ Üye için: new BooksForm(true, memberId)
        public BooksForm(bool readOnlyMode, int memberId)
        {
            InitializeComponent();
            _readOnlyMode = readOnlyMode;
            _memberId = memberId;

            dgvBooks.DataBindingComplete += dgvBooks_DataBindingComplete;

            // Form yeniden boyutlanınca konumlar bozulmasın
            this.Resize += BooksForm_Resize;
        }

        private void BooksForm_Load(object sender, EventArgs e)
        {
            // 1) Katman düzenini garantiye al (picBgBooks en arkada kalsın)
            EnsureBackgroundIsBehind();

            // 2) Arka planı soldur (1 kere)
            FadeBackgroundOnce(0.55f);

            LoadCategories();
            LoadBooks();

            ApplyRoleUI();

            // ✅ Üye butonlarını (görünüyorsa) tablonun yanında sağda hizala
            PositionMemberButtons();
        }

        private void BooksForm_Load_1(object sender, EventArgs e)
        {
            BooksForm_Load(sender, e);
        }

        private void BooksForm_Resize(object sender, EventArgs e)
        {
            PositionMemberButtons();
        }

        // ✅ picBgBooks kesin arkada kalsın, diğerleri önde olsun
        private void EnsureBackgroundIsBehind()
        {
            try
            {
                if (picBgBooks != null)
                {
                    picBgBooks.Dock = DockStyle.Fill;
                    picBgBooks.SendToBack();
                }

                foreach (Control c in this.Controls)
                {
                    if (c == picBgBooks) continue;
                    c.BringToFront();
                }
            }
            catch { }
        }

        // ✅ Üye butonlarını (3 adet) tablonun yanında sağda, alt alta ve dikey ortalı hizala
        private void PositionMemberButtons()
        {
            if (dgvBooks == null) return;

            // Üye modunda görünür olacak butonlar
            Button[] memberButtons = new Button[] { btnBookRequest, btnMyBorrows, };

            // Görünür olanları topla
            var visibleButtons = new System.Collections.Generic.List<Button>();
            foreach (var b in memberButtons)
            {
                if (b != null && b.Visible) visibleButtons.Add(b);
            }

            if (visibleButtons.Count == 0) return;

            int gapRightSide = 25;     // tablo ile sağ alan arası
            int rightMargin = 25;      // form sağ kenar boşluğu
            int gapBetween = 12;       // butonlar arası boşluk

            int areaLeft = dgvBooks.Right + gapRightSide;
            int areaRight = this.ClientSize.Width - rightMargin;
            int areaWidth = areaRight - areaLeft;

            // X: sağ alanın ortası
            int btnW = visibleButtons[0].Width;
            int x = areaLeft + Math.Max(0, (areaWidth - btnW) / 2);

            // Y: 3 butonun toplam yüksekliğini hesapla ve formda ortala
            int totalH = 0;
            for (int i = 0; i < visibleButtons.Count; i++)
                totalH += visibleButtons[i].Height;
            totalH += gapBetween * (visibleButtons.Count - 1);

            int startY = (this.ClientSize.Height - totalH) / 2;
            if (startY < 0) startY = 0;

            int y = startY;

            foreach (var btn in visibleButtons)
            {
                btn.Anchor = AnchorStyles.None; // sabit anchoring kapalı
                btn.Location = new Point(x, y);
                btn.BringToFront();
                y += btn.Height + gapBetween;
            }
        }

        // ✅ Tüm kontrolleri (iç içe olanlar dahil) dolaşmak için yardımcı
        private static System.Collections.Generic.IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (var child in GetAllControls(c))
                    yield return child;
            }
        }

        // ✅ Sağ taraftaki edit alanlarını topluca gizle/göster
        private void SetRightSideEditorVisible(bool visible)
        {
            if (dgvBooks == null) return;

            int boundaryX = dgvBooks.Right + 10;

            foreach (Control c in GetAllControls(this))
            {
                if (c == null) continue;

                if (c == dgvBooks) continue;
                if (c.Name == "txtSearch") continue;
                if (c.Name == "picBgBooks") continue;
                if (c is PictureBox) continue;

                // ✅ Üye butonları HER ZAMAN kalsın
                if (c.Name == "btnBookRequest") continue;
                if (c.Name == "btnMyBorrows") continue;
                if (c.Name == "btnMyRequests") continue;

                bool isRightSide = c.Left >= boundaryX;

                bool targetType =
                    (c is Label) ||
                    (c is TextBox) ||
                    (c is ComboBox) ||
                    (c is Button);

                if (isRightSide && targetType)
                    c.Visible = visible;
            }
        }

        private void ApplyRoleUI()
        {
            dgvBooks.ReadOnly = true;
            dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBooks.MultiSelect = false;
            dgvBooks.AllowUserToAddRows = false;
            dgvBooks.RowHeadersVisible = false;

            if (_readOnlyMode)
            {
                // ✅ Üye: ekleme/güncelleme/silme yok
                btnSave.Visible = false;
                btnUpdate.Visible = false;
                btnDelete.Visible = false;

                btnSave.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;

                // ✅ Edit alanları kilit
                txtTitle.ReadOnly = true;
                txtAuthor.ReadOnly = true;
                txtPublisher.ReadOnly = true;
                txtPublishYear.ReadOnly = true;
                txtISBN.ReadOnly = true;
                txtStock.ReadOnly = true;
                cmbCategory.Enabled = false;

                SetRightSideEditorVisible(false);

                // ✅ Üye butonları göster
                if (btnBookRequest != null) { btnBookRequest.Visible = true; btnBookRequest.Enabled = true; }
                if (btnMyBorrows != null) { btnMyBorrows.Visible = true; btnMyBorrows.Enabled = true; }

                selectedBookId = 0;
                ClearInputs();

                PositionMemberButtons();
            }
            else
            {
                // ✅ Admin/Görevli: normal
                txtTitle.ReadOnly = false;
                txtAuthor.ReadOnly = false;
                txtPublisher.ReadOnly = false;
                txtPublishYear.ReadOnly = false;
                txtISBN.ReadOnly = false;
                txtStock.ReadOnly = false;
                cmbCategory.Enabled = true;

                btnSave.Visible = true;
                btnUpdate.Visible = true;
                btnDelete.Visible = true;

                btnSave.Enabled = true;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;

                SetRightSideEditorVisible(true);

                // ✅ Üye butonları gizle
                if (btnBookRequest != null) btnBookRequest.Visible = false;
                if (btnMyBorrows != null) btnMyBorrows.Visible = false;
            }
        }

        // ✅ Sadece 1 kere soldur
        private void FadeBackgroundOnce(float opacity)
        {
            if (_bgFaded) return;
            _bgFaded = true;

            try
            {
                if (picBgBooks != null && picBgBooks.Image != null)
                {
                    picBgBooks.Image = SetImageOpacity(picBgBooks.Image, opacity);
                    picBgBooks.Refresh();
                    return;
                }

                if (this.BackgroundImage != null)
                {
                    this.BackgroundImage = SetImageOpacity(this.BackgroundImage, opacity);
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

        private void LoadCategories()
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT CategoryId, CategoryName FROM categories";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        categoriesTable = new DataTable();
                        da.Fill(categoriesTable);

                        cmbCategory.DataSource = categoriesTable;
                        cmbCategory.DisplayMember = "CategoryName";
                        cmbCategory.ValueMember = "CategoryId";
                        cmbCategory.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Kategori yükleme hatası: " + ex.Message);
            }
        }

        private void LoadBooks()
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            b.BookId,
                            b.Title,
                            b.Author,
                            b.Publisher,
                            b.PublishYear,
                            b.ISBN,
                            b.Stock,
                            b.CategoryId,
                            c.CategoryName
                        FROM books b
                        LEFT JOIN categories c ON b.CategoryId = c.CategoryId
                    ";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvBooks.DataSource = dt;
                        ApplyTurkishHeadersAndHideColumns();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Listeleme hatası: " + ex.Message);
            }
        }

        private void dgvBooks_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ApplyTurkishHeadersAndHideColumns();
        }

        private void ApplyTurkishHeadersAndHideColumns()
        {
            foreach (DataGridViewColumn col in dgvBooks.Columns)
            {
                string dp = col.DataPropertyName;

                if (dp == "BookId" || dp == "CategoryId") col.Visible = false;
                else if (dp == "Title") col.HeaderText = "Kitap Adı";
                else if (dp == "Author") col.HeaderText = "Yazar";
                else if (dp == "Publisher") col.HeaderText = "Yayınevi";
                else if (dp == "PublishYear") col.HeaderText = "Basım Yılı";
                else if (dp == "ISBN") col.HeaderText = "ISBN";
                else if (dp == "Stock") col.HeaderText = "Stok";
                else if (dp == "CategoryName") col.HeaderText = "Kategori";
            }
        }

        private void ClearInputs()
        {
            txtTitle.Clear();
            txtAuthor.Clear();
            txtPublisher.Clear();
            txtPublishYear.Clear();
            txtISBN.Clear();
            txtStock.Clear();
            cmbCategory.SelectedIndex = -1;
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (_readOnlyMode)
            {
                MessageBox.Show("❌ Üye rolünde kitap ekleme yetkin yok.");
                return;
            }

            string title = txtTitle.Text.Trim();
            string author = txtAuthor.Text.Trim();
            string publisher = txtPublisher.Text.Trim();
            string publishYearText = txtPublishYear.Text.Trim();
            string isbn = txtISBN.Text.Trim();
            string stockText = txtStock.Text.Trim();

            object categoryId = DBNull.Value;
            int catId;
            if (cmbCategory.SelectedValue != null && int.TryParse(cmbCategory.SelectedValue.ToString(), out catId))
                categoryId = catId;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
            {
                MessageBox.Show("❌ Kitap adı ve Yazar boş olamaz!");
                return;
            }

            int stock;
            if (!int.TryParse(stockText, out stock))
            {
                MessageBox.Show("❌ Stok sayı olmalı!");
                return;
            }

            object publishYear = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(publishYearText))
            {
                int year;
                if (!int.TryParse(publishYearText, out year))
                {
                    MessageBox.Show("❌ Basım yılı sayı olmalı!");
                    return;
                }
                publishYear = year;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO books (Title, Author, Publisher, PublishYear, ISBN, Stock, CategoryId)
                        VALUES (@title, @author, @publisher, @publishYear, @isbn, @stock, @categoryId)
                    ";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@author", author);
                        cmd.Parameters.AddWithValue("@publisher", string.IsNullOrWhiteSpace(publisher) ? (object)DBNull.Value : publisher);
                        cmd.Parameters.AddWithValue("@publishYear", publishYear);
                        cmd.Parameters.AddWithValue("@isbn", string.IsNullOrWhiteSpace(isbn) ? (object)DBNull.Value : isbn);
                        cmd.Parameters.AddWithValue("@stock", stock);
                        cmd.Parameters.AddWithValue("@categoryId", categoryId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Kitap eklendi!");
                ClearInputs();
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Kaydetme hatası: " + ex.Message);
            }
        }

        private void dgvBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (_readOnlyMode) return;

            var row = dgvBooks.Rows[e.RowIndex];

            selectedBookId = Convert.ToInt32(row.Cells["BookId"].Value);

            txtTitle.Text = row.Cells["Title"].Value == null ? "" : row.Cells["Title"].Value.ToString();
            txtAuthor.Text = row.Cells["Author"].Value == null ? "" : row.Cells["Author"].Value.ToString();
            txtPublisher.Text = row.Cells["Publisher"].Value == null ? "" : row.Cells["Publisher"].Value.ToString();
            txtPublishYear.Text = row.Cells["PublishYear"].Value == null ? "" : row.Cells["PublishYear"].Value.ToString();
            txtISBN.Text = row.Cells["ISBN"].Value == null ? "" : row.Cells["ISBN"].Value.ToString();
            txtStock.Text = row.Cells["Stock"].Value == null ? "" : row.Cells["Stock"].Value.ToString();

            if (row.Cells["CategoryId"].Value == DBNull.Value || row.Cells["CategoryId"].Value == null)
                cmbCategory.SelectedIndex = -1;
            else
                cmbCategory.SelectedValue = Convert.ToInt32(row.Cells["CategoryId"].Value);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_readOnlyMode)
            {
                MessageBox.Show("❌ Üye rolünde kitap güncelleme yetkin yok.");
                return;
            }

            if (selectedBookId == 0)
            {
                MessageBox.Show("Lütfen güncellemek için tablodan bir kitap seç.");
                return;
            }

            string title = txtTitle.Text.Trim();
            string author = txtAuthor.Text.Trim();
            string publisher = txtPublisher.Text.Trim();
            string publishYearText = txtPublishYear.Text.Trim();
            string isbn = txtISBN.Text.Trim();
            string stockText = txtStock.Text.Trim();

            object categoryId = DBNull.Value;
            int catId;
            if (cmbCategory.SelectedValue != null && int.TryParse(cmbCategory.SelectedValue.ToString(), out catId))
                categoryId = catId;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
            {
                MessageBox.Show("Kitap adı ve yazar boş olamaz!");
                return;
            }

            int stock;
            if (!int.TryParse(stockText, out stock))
            {
                MessageBox.Show("Stok sayı olmalı!");
                return;
            }

            object publishYear = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(publishYearText))
            {
                int year;
                if (!int.TryParse(publishYearText, out year))
                {
                    MessageBox.Show("❌ Basım yılı sayı olmalı!");
                    return;
                }
                publishYear = year;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        UPDATE books
                        SET 
                            Title=@title,
                            Author=@author,
                            Publisher=@publisher,
                            PublishYear=@publishYear,
                            ISBN=@isbn,
                            Stock=@stock,
                            CategoryId=@categoryId
                        WHERE BookId=@id
                    ";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@author", author);
                        cmd.Parameters.AddWithValue("@publisher", string.IsNullOrWhiteSpace(publisher) ? (object)DBNull.Value : publisher);
                        cmd.Parameters.AddWithValue("@publishYear", publishYear);
                        cmd.Parameters.AddWithValue("@isbn", string.IsNullOrWhiteSpace(isbn) ? (object)DBNull.Value : isbn);
                        cmd.Parameters.AddWithValue("@stock", stock);
                        cmd.Parameters.AddWithValue("@categoryId", categoryId);
                        cmd.Parameters.AddWithValue("@id", selectedBookId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Kitap güncellendi!");
                LoadBooks();
                selectedBookId = 0;
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Güncelleme hatası: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_readOnlyMode)
            {
                MessageBox.Show("❌ Üye rolünde kitap silme yetkin yok.");
                return;
            }

            if (selectedBookId == 0)
            {
                MessageBox.Show("❌ Lütfen silmek için tablodan bir kitap seç!");
                return;
            }

            var confirm = MessageBox.Show(
                "Seçili kitabı silmek istediğine emin misin?",
                "Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.No) return;

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "DELETE FROM books WHERE BookId=@id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedBookId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Kitap silindi!");
                selectedBookId = 0;
                ClearInputs();
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Silme hatası: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(search))
            {
                LoadBooks();
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            b.BookId,
                            b.Title,
                            b.Author,
                            b.Publisher,
                            b.PublishYear,
                            b.ISBN,
                            b.Stock,
                            b.CategoryId,
                            c.CategoryName
                        FROM books b
                        LEFT JOIN categories c ON b.CategoryId = c.CategoryId
                        WHERE b.Title LIKE @q
                           OR b.Author LIKE @q
                           OR b.ISBN LIKE @q
                           OR b.Publisher LIKE @q
                           OR CAST(b.PublishYear AS CHAR) LIKE @q
                           OR c.CategoryName LIKE @q
                    ";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@q", "%" + search + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvBooks.DataSource = dt;

                        ApplyTurkishHeadersAndHideColumns();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Arama hatası: " + ex.Message);
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                LoadBooks();
        }

        private void dgvBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // boş
        }

        // ✅ Designer bu olaya bağlı
        private void btnBookRequest_Click_1(object sender, EventArgs e)
        {
            if (!_readOnlyMode)
            {
                MessageBox.Show("Bu buton sadece Üye girişinde kullanılabilir.");
                return;
            }

            if (_memberId <= 0)
            {
                MessageBox.Show("❌ Üye ID bulunamadı. Login -> Form1 -> BooksForm'a memberId gönderilmesi gerekiyor.");
                return;
            }

            BookRequestForm frm = new BookRequestForm(_memberId);
            frm.ShowDialog();
        }

        private void btnMyBorrows_Click(object sender, EventArgs e)
        {
            if (!_readOnlyMode) return;
            if (_memberId <= 0)
            {
                MessageBox.Show("❌ Üye ID bulunamadı.");
                return;
            }

            UyePanelForm frm = new UyePanelForm(_memberId, "emanet");
            frm.ShowDialog();
        }

        private void btnMyRequests_Click(object sender, EventArgs e)
        {
            if (!_readOnlyMode) return;
            if (_memberId <= 0)
            {
                MessageBox.Show("❌ Üye ID bulunamadı.");
                return;
            }

            UyePanelForm frm = new UyePanelForm(_memberId, "talep");
            frm.ShowDialog();
        }

        private void txtISBN_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
