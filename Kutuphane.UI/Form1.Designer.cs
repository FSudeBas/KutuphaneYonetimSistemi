namespace Kutuphane.UI
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnBooks = new System.Windows.Forms.Button();
            this.btnMembers = new System.Windows.Forms.Button();
            this.btnCategories = new System.Windows.Forms.Button();
            this.btnBorrows = new System.Windows.Forms.Button();
            this.btnReports = new System.Windows.Forms.Button();
            this.btnRequests = new System.Windows.Forms.Button();
            this.picBgMenu = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBgMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBooks
            // 
            this.btnBooks.BackColor = System.Drawing.Color.Linen;
            this.btnBooks.Font = new System.Drawing.Font("Palatino Linotype", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnBooks.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnBooks.Location = new System.Drawing.Point(229, 90);
            this.btnBooks.Name = "btnBooks";
            this.btnBooks.Size = new System.Drawing.Size(165, 59);
            this.btnBooks.TabIndex = 1;
            this.btnBooks.Text = "Kitaplar";
            this.btnBooks.UseVisualStyleBackColor = false;
            this.btnBooks.Click += new System.EventHandler(this.btnBooks_Click);
            // 
            // btnMembers
            // 
            this.btnMembers.BackColor = System.Drawing.Color.Linen;
            this.btnMembers.Font = new System.Drawing.Font("Palatino Linotype", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnMembers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnMembers.Location = new System.Drawing.Point(466, 90);
            this.btnMembers.Name = "btnMembers";
            this.btnMembers.Size = new System.Drawing.Size(165, 59);
            this.btnMembers.TabIndex = 2;
            this.btnMembers.Text = "Üyeler";
            this.btnMembers.UseVisualStyleBackColor = false;
            this.btnMembers.Click += new System.EventHandler(this.btnMembers_Click);
            // 
            // btnCategories
            // 
            this.btnCategories.BackColor = System.Drawing.Color.Linen;
            this.btnCategories.Font = new System.Drawing.Font("Palatino Linotype", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnCategories.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnCategories.Location = new System.Drawing.Point(229, 194);
            this.btnCategories.Name = "btnCategories";
            this.btnCategories.Size = new System.Drawing.Size(165, 59);
            this.btnCategories.TabIndex = 3;
            this.btnCategories.Text = "Kategoriler";
            this.btnCategories.UseVisualStyleBackColor = false;
            this.btnCategories.Click += new System.EventHandler(this.btnCategories_Click);
            // 
            // btnBorrows
            // 
            this.btnBorrows.BackColor = System.Drawing.Color.Linen;
            this.btnBorrows.Font = new System.Drawing.Font("Palatino Linotype", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnBorrows.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnBorrows.Location = new System.Drawing.Point(466, 194);
            this.btnBorrows.Name = "btnBorrows";
            this.btnBorrows.Size = new System.Drawing.Size(165, 59);
            this.btnBorrows.TabIndex = 4;
            this.btnBorrows.Text = "Emanet";
            this.btnBorrows.UseVisualStyleBackColor = false;
            this.btnBorrows.Click += new System.EventHandler(this.btnBorrows_Click);
            // 
            // btnReports
            // 
            this.btnReports.BackColor = System.Drawing.Color.Linen;
            this.btnReports.Font = new System.Drawing.Font("Palatino Linotype", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnReports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnReports.Location = new System.Drawing.Point(229, 296);
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(165, 59);
            this.btnReports.TabIndex = 5;
            this.btnReports.Text = "Raporlar";
            this.btnReports.UseVisualStyleBackColor = false;
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
            // 
            // btnRequests
            // 
            this.btnRequests.BackColor = System.Drawing.Color.Linen;
            this.btnRequests.Font = new System.Drawing.Font("Palatino Linotype", 13.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnRequests.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnRequests.Location = new System.Drawing.Point(466, 296);
            this.btnRequests.Name = "btnRequests";
            this.btnRequests.Size = new System.Drawing.Size(165, 59);
            this.btnRequests.TabIndex = 7;
            this.btnRequests.Text = "Talepler";
            this.btnRequests.UseVisualStyleBackColor = false;
            this.btnRequests.Click += new System.EventHandler(this.btnRequests_Click);
            // 
            // picBgMenu
            // 
            this.picBgMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBgMenu.Image = global::Kutuphane.UI.Properties.Resources.pngtree_an_old_style_library_with_shelves_full_of_books_image_2679112;
            this.picBgMenu.Location = new System.Drawing.Point(0, 0);
            this.picBgMenu.Name = "picBgMenu";
            this.picBgMenu.Size = new System.Drawing.Size(840, 456);
            this.picBgMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBgMenu.TabIndex = 6;
            this.picBgMenu.TabStop = false;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(840, 456);
            this.Controls.Add(this.btnRequests);
            this.Controls.Add(this.btnReports);
            this.Controls.Add(this.btnBorrows);
            this.Controls.Add(this.btnCategories);
            this.Controls.Add(this.btnMembers);
            this.Controls.Add(this.btnBooks);
            this.Controls.Add(this.picBgMenu);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.picBgMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnBooks;
        private System.Windows.Forms.Button btnMembers;
        private System.Windows.Forms.Button btnCategories;
        private System.Windows.Forms.Button btnBorrows;
        private System.Windows.Forms.Button btnReports;
        private System.Windows.Forms.PictureBox picBgMenu;
        private System.Windows.Forms.Button btnRequests;
    }
}

