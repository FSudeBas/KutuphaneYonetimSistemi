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
    public partial class CategoriesForm : Form
    {
        private int selectedCategoryId = 0;

        public CategoriesForm()
        {
            InitializeComponent();
        }

        private void CategoriesForm_Load_1(object sender, EventArgs e)
        {
            // ✅ Arka plan fotoğrafını soldur
            // PictureBox adın farklıysa: picBgCategories ismini kendi adına göre değiştir.
            if (picBgCategories != null && picBgCategories.Image != null)
            {
                picBgCategories.Image = SetImageOpacity(picBgCategories.Image, 0.55f); // 0.55 ideal
            }

            LoadCategories();
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

        private void LoadCategories(string search = "")
        {
            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT CategoryId, CategoryName 
                                   FROM categories";

                    // Arama varsa filtre uygula
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        sql += " WHERE CategoryName LIKE @search";
                    }

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(search))
                            cmd.Parameters.AddWithValue("@search", "%" + search + "%");

                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgvCategories.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Listeleme hatası: " + ex.Message);
            }
        }

        private void ClearInputs()
        {
            txtCategoryName.Clear();
            selectedCategoryId = 0;
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            string categoryName = txtCategoryName.Text.Trim();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show("❌ Kategori adı boş olamaz!");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "INSERT INTO categories (CategoryName) VALUES (@name)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", categoryName);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Kategori eklendi!");

                ClearInputs();
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Kaydetme hatası: " + ex.Message);
            }
        }

        private void dgvCategories_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvCategories.Rows[e.RowIndex];

            selectedCategoryId = Convert.ToInt32(row.Cells["CategoryId"].Value);
            txtCategoryName.Text = row.Cells["CategoryName"].Value?.ToString();
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (selectedCategoryId == 0)
            {
                MessageBox.Show("❌ Güncellemek için tablodan bir kategori seç!");
                return;
            }

            string categoryName = txtCategoryName.Text.Trim();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show("❌ Kategori adı boş olamaz!");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"UPDATE categories 
                                   SET CategoryName=@name 
                                   WHERE CategoryId=@id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", categoryName);
                        cmd.Parameters.AddWithValue("@id", selectedCategoryId);

                        int affected = cmd.ExecuteNonQuery();

                        if (affected > 0)
                            MessageBox.Show("✅ Kategori güncellendi!");
                        else
                            MessageBox.Show("⚠️ Güncelleme yapılamadı.");
                    }
                }

                ClearInputs();
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Güncelleme hatası: " + ex.Message);
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (selectedCategoryId == 0)
            {
                MessageBox.Show("❌ Silmek için tablodan bir kategori seç!");
                return;
            }

            var confirm = MessageBox.Show(
                "Seçili kategoriyi silmek istediğine emin misin?",
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

                    string sql = "DELETE FROM categories WHERE CategoryId=@id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedCategoryId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Kategori silindi!");

                ClearInputs();
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Silme hatası: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged_1(object sender, EventArgs e)
        {
            LoadCategories(txtSearch.Text.Trim());
        }

        // Bunu boş bırakabilirsin (kullanmayacağız)
        private void dgvCategories_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}
