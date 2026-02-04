using System;
using System.Data;
using System.Windows.Forms;
using Kutuphane.DAL;
using MySqlConnector;

// ✅ Fotoğraf soldurma için gerekli
using System.Drawing;
using System.Drawing.Imaging;

namespace Kutuphane.UI
{
    public partial class MembersForm : Form
    {
        private int selectedMemberId = 0;

        public MembersForm()
        {
            InitializeComponent();
        }

        private void MembersForm_Load(object sender, EventArgs e)
        {
            // ✅ Arka plan fotoğrafını soldur (picBgMembers)
            if (picBgMembers != null && picBgMembers.Image != null)
            {
                picBgMembers.Image = SetImageOpacity(picBgMembers.Image, 0.55f); // 0.55 ideal
            }

            // DateTimePicker düzgün görünsün (isteğe bağlı)
            dtpMembershipDate.Format = DateTimePickerFormat.Custom;
            dtpMembershipDate.CustomFormat = "dd.MM.yyyy";

            LoadMembers();
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

        // ✅ Listeleme (SADECE AKTİF ÜYELER)
        private void LoadMembers(string search = "")
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    // ✅ 1) Sadece aktif üyeler
                    string sql = @"SELECT MemberId, FirstName, LastName, Email, Phone, MembershipDate
                                   FROM members
                                   WHERE IsActive = 1";

                    // ✅ 2) Arama varsa WHERE değil AND eklenir
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        sql += @" AND (FirstName LIKE @q
                                  OR LastName LIKE @q
                                  OR Email LIKE @q
                                  OR Phone LIKE @q)";
                    }

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(search))
                            da.SelectCommand.Parameters.AddWithValue("@q", "%" + search + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvMembers.DataSource = dt;

                        // ✅ görüntü güzelleştirme
                        if (dgvMembers.Columns.Contains("MemberId"))
                            dgvMembers.Columns["MemberId"].Visible = false;

                        // ✅ Üyelik Tarihi sütunu format + başlık
                        if (dgvMembers.Columns.Contains("MembershipDate"))
                        {
                            dgvMembers.Columns["MembershipDate"].HeaderText = "Üyelik Tarihi";
                            dgvMembers.Columns["MembershipDate"].DefaultCellStyle.Format = "dd.MM.yyyy";
                        }

                        // ✅ Diğer başlıklar
                        if (dgvMembers.Columns.Contains("FirstName")) dgvMembers.Columns["FirstName"].HeaderText = "Ad";
                        if (dgvMembers.Columns.Contains("LastName")) dgvMembers.Columns["LastName"].HeaderText = "Soyad";
                        if (dgvMembers.Columns.Contains("Email")) dgvMembers.Columns["Email"].HeaderText = "E-mail";
                        if (dgvMembers.Columns.Contains("Phone")) dgvMembers.Columns["Phone"].HeaderText = "Telefon";

                        dgvMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Listeleme hatası: " + ex.Message);
            }
        }

        // ✅ Temizle
        private void ClearInputs()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();

            dtpMembershipDate.Value = DateTime.Today;
            selectedMemberId = 0;
        }

        // ✅ Kaydet (IsActive=1)
        private void btnSave_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            DateTime membershipDate = dtpMembershipDate.Value.Date;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("❌ Ad ve Soyad zorunludur!");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    // ✅ IsActive=1 ekledik (kolonda default yoksa şart)
                    string sql = @"INSERT INTO members (FirstName, LastName, Email, Phone, MembershipDate, IsActive)
                                   VALUES (@fn, @ln, @em, @ph, @md, 1)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@fn", firstName);
                        cmd.Parameters.AddWithValue("@ln", lastName);

                        cmd.Parameters.AddWithValue("@em",
                            string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);

                        cmd.Parameters.AddWithValue("@ph",
                            string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone);

                        cmd.Parameters.AddWithValue("@md", membershipDate);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Üye eklendi!");
                LoadMembers(txtSearch.Text.Trim());
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Kaydetme hatası: " + ex.Message);
            }
        }

        // ✅ Seçince textbox + üyelik tarihi dolsun
        private void dgvMembers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvMembers.Rows[e.RowIndex];

            selectedMemberId = Convert.ToInt32(row.Cells["MemberId"].Value);

            txtFirstName.Text = row.Cells["FirstName"].Value?.ToString();
            txtLastName.Text = row.Cells["LastName"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString();

            if (row.Cells["MembershipDate"].Value != null && row.Cells["MembershipDate"].Value != DBNull.Value)
                dtpMembershipDate.Value = Convert.ToDateTime(row.Cells["MembershipDate"].Value);
            else
                dtpMembershipDate.Value = DateTime.Today;
        }

        // ✅ Güncelle
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedMemberId == 0)
            {
                MessageBox.Show("❌ Güncellemek için bir üye seç!");
                return;
            }

            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            DateTime membershipDate = dtpMembershipDate.Value.Date;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("❌ Ad ve Soyad zorunludur!");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"UPDATE members
                                   SET FirstName=@fn, LastName=@ln, Email=@em, Phone=@ph, MembershipDate=@md
                                   WHERE MemberId=@id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@fn", firstName);
                        cmd.Parameters.AddWithValue("@ln", lastName);

                        cmd.Parameters.AddWithValue("@em",
                            string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);

                        cmd.Parameters.AddWithValue("@ph",
                            string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone);

                        cmd.Parameters.AddWithValue("@md", membershipDate);
                        cmd.Parameters.AddWithValue("@id", selectedMemberId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Üye güncellendi!");
                LoadMembers(txtSearch.Text.Trim());
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Güncelleme hatası: " + ex.Message);
            }
        }

        // ✅ Sil butonu = PASİFE AL
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedMemberId == 0)
            {
                MessageBox.Show("❌ Pasife almak için bir üye seç!");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    // ✅ Aktif emanet var mı? (IsReturned=0)
                    string checkSql = "SELECT COUNT(*) FROM borrows WHERE MemberId=@id AND IsReturned=0";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", selectedMemberId);
                        int activeBorrowCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (activeBorrowCount > 0)
                        {
                            MessageBox.Show("❌ Bu üyenin aktif emaneti var, pasife alınamaz.\nÖnce iade alman lazım.");
                            return;
                        }
                    }

                    var confirm = MessageBox.Show(
                        "Bu üye silinemiyor çünkü geçmiş emanet kayıtları veritabanında duruyor.\n" +
                        "Bu yüzden rapor geçmişi bozulmasın diye üyeyi SİLMEK yerine PASİFE alıyoruz.\n\n" +
                        "Pasife alınan üye listede görünmez, fakat geçmiş emanet/rapor kayıtları korunur.\n\n" +
                        "Seçili üyeyi PASİFE almak istiyor musun?",
                        "Pasife Alma Onayı",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (confirm == DialogResult.No) return;

                    // ✅ Silmek yerine pasife al
                    string sql = "UPDATE members SET IsActive = 0 WHERE MemberId=@id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedMemberId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Üye pasife alındı!");
                LoadMembers(txtSearch.Text.Trim());
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Pasife alma hatası: " + ex.Message);
            }
        }

        // ✅ Arama
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadMembers(txtSearch.Text.Trim());
        }

        private void dgvMembers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Designer hata vermesin diye boş bırakıyoruz.
        }
    }
}