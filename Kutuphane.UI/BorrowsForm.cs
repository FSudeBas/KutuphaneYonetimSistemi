using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Kutuphane.DAL;
using MySqlConnector;

// ✅ Fotoğraf soldurma için gerekli
using System.Drawing.Imaging;

namespace Kutuphane.UI
{
    public partial class BorrowsForm : Form
    {
        private int selectedBorrowId = 0;
        private int selectedBookId = 0;

        private DataTable membersTable;
        private DataTable booksTable;

        public BorrowsForm()
        {
            InitializeComponent();
        }

        private void BorrowsForm_Load(object sender, EventArgs e)
        {
            // ✅ Arka plan fotoğrafını soldur
            // PictureBox adın farklıysa: picBgBorrows ismini kendi adına göre değiştir.
            if (picBgBorrows != null && picBgBorrows.Image != null)
            {
                picBgBorrows.Image = SetImageOpacity(picBgBorrows.Image, 0.55f); // 0.55 ideal
            }

            LoadMembers();
            LoadBooks();
            LoadBorrows();
            ClearInputs();

            // ✅ İlk açılışta iade butonu pasif olsun
            btnReturn.Enabled = false;
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

        // ✅ Üyeleri ComboBox'a doldur
        private void LoadMembers()
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT MemberId, CONCAT(FirstName,' ',LastName) AS FullName FROM members ORDER BY FirstName";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        membersTable = new DataTable();
                        da.Fill(membersTable);

                        cmbMember.DataSource = membersTable;
                        cmbMember.DisplayMember = "FullName";
                        cmbMember.ValueMember = "MemberId";
                        cmbMember.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Üye yükleme hatası: " + ex.Message);
            }
        }

        // ✅ Kitapları ComboBox'a doldur (stok>0 olanlar)
        private void LoadBooks()
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        SELECT BookId, CONCAT(Title,' - ',Author) AS BookInfo
                        FROM books
                        WHERE Stock > 0
                        ORDER BY Title
                    ";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        booksTable = new DataTable();
                        da.Fill(booksTable);

                        cmbBook.DataSource = booksTable;
                        cmbBook.DisplayMember = "BookInfo";
                        cmbBook.ValueMember = "BookId";
                        cmbBook.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Kitap yükleme hatası: " + ex.Message);
            }
        }

        // ✅ Emanetleri listele (arama + sadece aktif filtre)
        private void LoadBorrows(string search = "")
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    bool onlyActive = chkActiveOnly.Checked;

                    string sql = @"
                        SELECT
                            br.BorrowId,
                            br.MemberId,
                            CONCAT(m.FirstName,' ',m.LastName) AS MemberName,
                            br.BookId,
                            CONCAT(b.Title,' - ',b.Author) AS BookName,
                            br.BorrowDate,
                            br.DueDate,
                            br.ReturnDate,
                            br.IsReturned,

                            CASE 
                                WHEN br.IsReturned = 1 THEN 'İade Edildi'
                                ELSE 'Aktif'
                            END AS StatusText,

                            CASE 
                                WHEN br.IsReturned = 0 AND br.DueDate < CURDATE() THEN 'Gecikti'
                                ELSE ''
                            END AS OverdueText

                        FROM borrows br
                        INNER JOIN members m ON br.MemberId = m.MemberId
                        INNER JOIN books b ON br.BookId = b.BookId
                    ";

                    string where = "";

                    // ✅ sadece aktif emanetler
                    if (onlyActive)
                        where += " br.IsReturned = 0 ";

                    // ✅ arama
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        if (!string.IsNullOrEmpty(where))
                            where += " AND ";

                        where += @" (
                            CONCAT(m.FirstName,' ',m.LastName) LIKE @q
                            OR b.Title LIKE @q
                            OR b.Author LIKE @q
                        ) ";
                    }

                    if (!string.IsNullOrEmpty(where))
                        sql += " WHERE " + where;

                    sql += " ORDER BY br.BorrowId DESC";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(search))
                            da.SelectCommand.Parameters.AddWithValue("@q", "%" + search + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvBorrows.DataSource = dt;
                        BeautifyGrid();
                        PaintOverdueRows(); // ✅ gecikenleri boya
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Listeleme hatası: " + ex.Message);
            }
        }

        // ✅ DGV'yi güzelleştirme (Sütun isimleri + gizleme + format)
        private void BeautifyGrid()
        {
            if (dgvBorrows.Columns.Contains("MemberId"))
                dgvBorrows.Columns["MemberId"].Visible = false;

            if (dgvBorrows.Columns.Contains("BookId"))
                dgvBorrows.Columns["BookId"].Visible = false;

            if (dgvBorrows.Columns.Contains("IsReturned"))
                dgvBorrows.Columns["IsReturned"].Visible = false;

            // ✅ Başlıklar
            if (dgvBorrows.Columns.Contains("BorrowId"))
                dgvBorrows.Columns["BorrowId"].HeaderText = "Emanet ID";

            if (dgvBorrows.Columns.Contains("BookName"))
                dgvBorrows.Columns["BookName"].HeaderText = "Kitap";

            if (dgvBorrows.Columns.Contains("MemberName"))
                dgvBorrows.Columns["MemberName"].HeaderText = "Üye";

            if (dgvBorrows.Columns.Contains("BorrowDate"))
                dgvBorrows.Columns["BorrowDate"].HeaderText = "Alış Tarihi";

            if (dgvBorrows.Columns.Contains("DueDate"))
                dgvBorrows.Columns["DueDate"].HeaderText = "Teslim Tarihi";

            if (dgvBorrows.Columns.Contains("ReturnDate"))
                dgvBorrows.Columns["ReturnDate"].HeaderText = "İade Tarihi";

            if (dgvBorrows.Columns.Contains("StatusText"))
                dgvBorrows.Columns["StatusText"].HeaderText = "Durum";

            if (dgvBorrows.Columns.Contains("OverdueText"))
                dgvBorrows.Columns["OverdueText"].HeaderText = "Gecikme";

            // ✅ Tarih formatı
            if (dgvBorrows.Columns.Contains("BorrowDate"))
                dgvBorrows.Columns["BorrowDate"].DefaultCellStyle.Format = "dd.MM.yyyy";

            if (dgvBorrows.Columns.Contains("DueDate"))
                dgvBorrows.Columns["DueDate"].DefaultCellStyle.Format = "dd.MM.yyyy";

            if (dgvBorrows.Columns.Contains("ReturnDate"))
                dgvBorrows.Columns["ReturnDate"].DefaultCellStyle.Format = "dd.MM.yyyy";

            // ✅ Genel görünüm
            dgvBorrows.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBorrows.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBorrows.MultiSelect = false;
            dgvBorrows.ReadOnly = true;
            dgvBorrows.AllowUserToAddRows = false;
            dgvBorrows.RowHeadersVisible = false;
        }

        // ✅ Geciken satırları renklendir
        private void PaintOverdueRows()
        {
            if (!dgvBorrows.Columns.Contains("OverdueText")) return;

            foreach (DataGridViewRow r in dgvBorrows.Rows)
            {
                if (r.Cells["OverdueText"].Value != null &&
                    r.Cells["OverdueText"].Value.ToString() == "Gecikti")
                {
                    r.DefaultCellStyle.BackColor = Color.MistyRose;
                    r.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
            }
        }

        // ✅ Temizleme
        private void ClearInputs()
        {
            cmbMember.SelectedIndex = -1;
            cmbBook.SelectedIndex = -1;

            dtBorrowDate.Value = DateTime.Now;
            dtDueDate.Value = DateTime.Now.AddDays(7);

            selectedBorrowId = 0;
            selectedBookId = 0;

            // ✅ iade butonu temizlenince pasif olsun
            btnReturn.Enabled = false;
        }

        // ✅ Alış tarihi değişince otomatik 7 gün ekle (Event bağlarsan çalışır)
        private void dtBorrowDate_ValueChanged(object sender, EventArgs e)
        {
            dtDueDate.Value = dtBorrowDate.Value.AddDays(7);
        }

        // ✅ Emanet Ver
        private void btnBorrow_Click(object sender, EventArgs e)
        {
            if (cmbMember.SelectedValue == null)
            {
                MessageBox.Show("❌ Lütfen üye seç!");
                return;
            }

            if (cmbBook.SelectedValue == null)
            {
                MessageBox.Show("❌ Lütfen kitap seç!");
                return;
            }

            int memberId = Convert.ToInt32(cmbMember.SelectedValue);
            int bookId = Convert.ToInt32(cmbBook.SelectedValue);

            DateTime borrowDate = dtBorrowDate.Value.Date;
            DateTime dueDate = dtDueDate.Value.Date;

            if (dueDate < borrowDate)
            {
                MessageBox.Show("❌ Teslim tarihi, alış tarihinden küçük olamaz!");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {
                        // ✅ 0) aynı kitap aynı üyede aktif mi?
                        string checkSql = @"SELECT COUNT(*) FROM borrows
                                            WHERE MemberId=@m AND BookId=@b AND IsReturned=0";
                        using (var checkCmd = new MySqlCommand(checkSql, conn, tran))
                        {
                            checkCmd.Parameters.AddWithValue("@m", memberId);
                            checkCmd.Parameters.AddWithValue("@b", bookId);

                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                            if (count > 0)
                            {
                                tran.Rollback();
                                MessageBox.Show("⚠️ Bu kitap zaten bu üyede emanet görünüyor!");
                                return;
                            }
                        }

                        // ✅ 1) borrows insert
                        string insertSql = @"
                            INSERT INTO borrows (MemberId, BookId, BorrowDate, DueDate, ReturnDate, IsReturned)
                            VALUES (@mId, @bId, @borrowDate, @dueDate, NULL, 0)
                        ";

                        using (var cmd = new MySqlCommand(insertSql, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@mId", memberId);
                            cmd.Parameters.AddWithValue("@bId", bookId);
                            cmd.Parameters.AddWithValue("@borrowDate", borrowDate);
                            cmd.Parameters.AddWithValue("@dueDate", dueDate);
                            cmd.ExecuteNonQuery();
                        }

                        // ✅ 2) stock azalt
                        string stockSql = "UPDATE books SET Stock = Stock - 1 WHERE BookId=@id AND Stock > 0";
                        using (var cmd2 = new MySqlCommand(stockSql, conn, tran))
                        {
                            cmd2.Parameters.AddWithValue("@id", bookId);
                            int affected = cmd2.ExecuteNonQuery();

                            if (affected == 0)
                            {
                                tran.Rollback();
                                MessageBox.Show("⚠️ Stok yok! Emanet verilemedi.");
                                return;
                            }
                        }

                        tran.Commit();
                    }
                }

                MessageBox.Show("✅ Emanet verildi!");

                LoadBooks();
                LoadBorrows(txtSearch.Text.Trim());
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Emanet verme hatası: " + ex.Message);
            }
        }

        // ✅ İade Al
        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (selectedBorrowId == 0 || selectedBookId == 0)
            {
                MessageBox.Show("❌ İade almak için listeden bir emanet seç!");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {
                        // ✅ 1) iade durumunu güncelle
                        string updateSql = @"
                            UPDATE borrows
                            SET IsReturned = 1,
                                ReturnDate = CURDATE()
                            WHERE BorrowId = @id
                        ";

                        using (var cmd = new MySqlCommand(updateSql, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedBorrowId);
                            cmd.ExecuteNonQuery();
                        }

                        // ✅ 2) stok artır
                        string stockSql = "UPDATE books SET Stock = Stock + 1 WHERE BookId=@bookId";
                        using (var cmd2 = new MySqlCommand(stockSql, conn, tran))
                        {
                            cmd2.Parameters.AddWithValue("@bookId", selectedBookId);
                            cmd2.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                }

                MessageBox.Show("✅ Kitap iade alındı!");

                LoadBooks();
                LoadBorrows(txtSearch.Text.Trim());
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ İade alma hatası: " + ex.Message);
            }
        }

        // ✅ DGV seçilince (seçilen emanet yakalansın + combobox dolsun)
        private void dgvBorrows_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvBorrows.Rows[e.RowIndex];

            selectedBorrowId = Convert.ToInt32(row.Cells["BorrowId"].Value);
            selectedBookId = Convert.ToInt32(row.Cells["BookId"].Value);

            cmbMember.SelectedValue = Convert.ToInt32(row.Cells["MemberId"].Value);
            cmbBook.SelectedValue = Convert.ToInt32(row.Cells["BookId"].Value);

            dtBorrowDate.Value = Convert.ToDateTime(row.Cells["BorrowDate"].Value);
            dtDueDate.Value = Convert.ToDateTime(row.Cells["DueDate"].Value);

            // ✅ Sadece aktif emanet seçilince iade al aktif olsun
            if (dgvBorrows.Columns.Contains("IsReturned"))
            {
                bool isReturned = Convert.ToInt32(row.Cells["IsReturned"].Value) == 1;
                btnReturn.Enabled = !isReturned;
            }
            else
            {
                btnReturn.Enabled = true;
            }
        }

        // ✅ Arama
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadBorrows(txtSearch.Text.Trim());
        }

        // ✅ Checkbox (Sadece aktif emanetler)
        private void chkActiveOnly_CheckedChanged(object sender, EventArgs e)
        {
            LoadBorrows(txtSearch.Text.Trim());
            ClearInputs();
        }
    }
}