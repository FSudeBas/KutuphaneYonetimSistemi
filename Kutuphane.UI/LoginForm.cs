using System;
using System.Windows.Forms;
using Kutuphane.DAL;
using MySqlConnector;

// ✅ Fotoğraf soldurma için gerekli
using System.Drawing;
using System.Drawing.Imaging;

namespace Kutuphane.UI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (picBg != null && picBg.Image != null)
            {
                picBg.Image = SetImageOpacity(picBg.Image, 0.55f);
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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Kullanıcı adı ve şifre boş olamaz!");
                return;
            }

            try
            {
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();

                    // ✅ Role + MemberId tek sorguda (members.UserId ile bağlı)
                    string sql = @"
                        SELECT 
                            u.Role,
                            IFNULL(m.MemberId, 0) AS MemberId
                        FROM users u
                        LEFT JOIN members m ON m.UserId = u.UserId
                        WHERE u.Username = @u AND u.PasswordHash = @p
                        LIMIT 1;
                    ";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                MessageBox.Show("❌ Kullanıcı adı veya şifre yanlış!");
                                return;
                            }

                            string role = rd["Role"].ToString();
                            int memberId = Convert.ToInt32(rd["MemberId"]);

                            MessageBox.Show($"✅ Giriş başarılı!\nRol: {role}");

                            this.Hide();

                            // ✅ Form1 artık 3 parametre
                            Form1 frm = new Form1(role, username, memberId);
                            frm.ShowDialog();

                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Giriş hatası: " + ex.Message);
            }
        }
    }
}
