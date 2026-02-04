using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Kutuphane.DAL;

namespace Kutuphane.UI
{
    public partial class Form1 : Form
    {
        private readonly string _role;
        private readonly string _username;

        // ✅ Üye ise MemberId'yi burada tutacağız (talep/emanet vs. için lazım)
        private readonly int _memberId;

        // ✅ Yetkisiz butonları tamamen gizle
        private readonly bool HIDE_UNAUTHORIZED_BUTTONS = true;

        // ✅ 3 parametreli constructor
        public Form1(string role, string username, int memberId)
        {
            InitializeComponent();

            _role = role;
            _username = username;
            _memberId = memberId;

            this.Text = "Ana Menü";

            // ✅ Eventleri GARANTİ bağla (Designer’da bağlı olsa bile sorun olmaz)
            this.Load += Form1_Load;
            this.Resize += Form1_Resize;

            if (btnRequests != null) btnRequests.Click += btnRequests_Click;

            ApplyRolePermissions();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ✅ Arka plan fotoğrafını soldur
            if (picBgMenu != null && picBgMenu.Image != null)
            {
                picBgMenu.Image = SetImageOpacity(picBgMenu.Image, 0.55f);
            }

            ArrangeMenuButtons();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ArrangeMenuButtons();
        }

        // ✅ Fotoğrafı soldurma fonksiyonu (Opacity)
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

        // ✅ Yetkili/yetkisiz buton kontrolü
        private void SetButtonAccess(Button btn, bool canAccess)
        {
            if (btn == null) return;

            if (HIDE_UNAUTHORIZED_BUTTONS)
            {
                btn.Visible = canAccess; // ✅ gizle
            }
            else
            {
                btn.Visible = true;
                btn.Enabled = canAccess;
            }
        }

        // ✅ Rol bazlı yetkilendirme
        private void ApplyRolePermissions()
        {
            // Önce hepsini kapat
            SetButtonAccess(btnBooks, false);
            SetButtonAccess(btnMembers, false);
            SetButtonAccess(btnCategories, false);
            SetButtonAccess(btnBorrows, false);

            // ✅ Talepler
            SetButtonAccess(btnRequests, false);

            // Raporlar
            if (Controls.ContainsKey("btnReports"))
                SetButtonAccess(btnReports, false);

            if (_role == "Admin")
            {
                SetButtonAccess(btnBooks, true);
                SetButtonAccess(btnMembers, true);
                SetButtonAccess(btnCategories, true);
                SetButtonAccess(btnBorrows, true);

                // ✅ Admin talepleri görsün
                SetButtonAccess(btnRequests, true);

                if (Controls.ContainsKey("btnReports"))
                    SetButtonAccess(btnReports, true);
            }
            else if (_role == "Gorevli")
            {
                SetButtonAccess(btnBooks, true);
                SetButtonAccess(btnMembers, true);
                SetButtonAccess(btnBorrows, true);

                SetButtonAccess(btnCategories, false);

                // ✅ Görevli talepleri görsün
                SetButtonAccess(btnRequests, true);

                if (Controls.ContainsKey("btnReports"))
                    SetButtonAccess(btnReports, false);
            }
            else if (_role == "Uye")
            {
                // ✅ Üye: sadece kitaplar
                SetButtonAccess(btnBooks, true);

                SetButtonAccess(btnMembers, false);
                SetButtonAccess(btnCategories, false);
                SetButtonAccess(btnBorrows, false);

                // ✅ Üye talepleri MENÜDE görmesin
                SetButtonAccess(btnRequests, false);

                if (Controls.ContainsKey("btnReports"))
                    SetButtonAccess(btnReports, false);
            }

            ArrangeMenuButtons();
        }

        // ✅ GÖRÜNEN butonları otomatik hizalar (boşluk kalmaz)
        private void ArrangeMenuButtons()
        {
            var buttons = new List<Button>();

            if (btnBooks != null && btnBooks.Visible) buttons.Add(btnBooks);
            if (btnMembers != null && btnMembers.Visible) buttons.Add(btnMembers);
            if (btnCategories != null && btnCategories.Visible) buttons.Add(btnCategories);
            if (btnBorrows != null && btnBorrows.Visible) buttons.Add(btnBorrows);

            // ✅ Talepler butonu
            if (btnRequests != null && btnRequests.Visible) buttons.Add(btnRequests);

            if (Controls.ContainsKey("btnReports") && btnReports.Visible) buttons.Add(btnReports);

            if (buttons.Count == 0) return;

            int btnW = 220;
            int btnH = 70;
            int gapX = 40;
            int gapY = 25;

            int cols = (buttons.Count == 1) ? 1 : 2;
            int rows = (int)Math.Ceiling(buttons.Count / (double)cols);

            int totalW = cols * btnW + (cols - 1) * gapX;
            int totalH = rows * btnH + (rows - 1) * gapY;

            int startX = (this.ClientSize.Width - totalW) / 2;
            int startY = (this.ClientSize.Height - totalH) / 2;

            for (int i = 0; i < buttons.Count; i++)
            {
                int r = i / cols;
                int c = i % cols;

                var btn = buttons[i];

                btn.Size = new Size(btnW, btnH);
                btn.Anchor = AnchorStyles.None;

                int x = startX + c * (btnW + gapX);
                int y = startY + r * (btnH + gapY);

                // son satır tek butonsa ortala
                if (cols == 2 && (i == buttons.Count - 1) && (buttons.Count % 2 == 1))
                    x = (this.ClientSize.Width - btnW) / 2;

                btn.Location = new Point(x, y);
                btn.BringToFront();
            }
        }

        private void btnBooks_Click(object sender, EventArgs e)
        {
            bool readOnly = (_role == "Uye");
            BooksForm frm = new BooksForm(readOnly, _memberId);
            frm.ShowDialog();
        }

        private void btnMembers_Click(object sender, EventArgs e)
        {
            MembersForm frm = new MembersForm();
            frm.ShowDialog();
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            CategoriesForm frm = new CategoriesForm();
            frm.ShowDialog();
        }

        private void btnBorrows_Click(object sender, EventArgs e)
        {
            BorrowsForm frm = new BorrowsForm();
            frm.ShowDialog();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            if (_role != "Admin")
            {
                MessageBox.Show("❌ Bu modül sadece Admin için açıktır!");
                return;
            }

            ReportForm frm = new ReportForm();
            frm.ShowDialog();
        }

        private void btnRequests_Click(object sender, EventArgs e)
        {
            // ✅ Admin + Görevli
            if (_role != "Admin" && _role != "Gorevli")
            {
                MessageBox.Show("❌ Bu ekran sadece Admin / Görevli için açıktır!");
                return;
            }

            BookRequestsAdminForm frm = new BookRequestsAdminForm();
            frm.ShowDialog();
        }
    }
}
