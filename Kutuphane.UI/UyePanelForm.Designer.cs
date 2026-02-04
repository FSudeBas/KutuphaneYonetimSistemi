namespace Kutuphane.UI
{
    partial class UyePanelForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TabControl tabUye;
        private System.Windows.Forms.TabPage tabEmanetlerim;
        private System.Windows.Forms.TabPage tabTaleplerim;

        private System.Windows.Forms.DataGridView dgvMyBorrows;
        private System.Windows.Forms.DataGridView dgvMyRequests;

        private System.Windows.Forms.CheckBox chkOnlyActive;
        private System.Windows.Forms.Button btnRefreshBorrows;
        private System.Windows.Forms.Button btnRefreshRequests;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabUye = new System.Windows.Forms.TabControl();
            this.tabEmanetlerim = new System.Windows.Forms.TabPage();
            this.dgvMyBorrows = new System.Windows.Forms.DataGridView();
            this.chkOnlyActive = new System.Windows.Forms.CheckBox();
            this.btnRefreshBorrows = new System.Windows.Forms.Button();
            this.tabTaleplerim = new System.Windows.Forms.TabPage();
            this.dgvMyRequests = new System.Windows.Forms.DataGridView();
            this.btnRefreshRequests = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tabUye.SuspendLayout();
            this.tabEmanetlerim.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyBorrows)).BeginInit();
            this.tabTaleplerim.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyRequests)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tabUye
            // 
            this.tabUye.Controls.Add(this.tabEmanetlerim);
            this.tabUye.Controls.Add(this.tabTaleplerim);
            this.tabUye.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabUye.Font = new System.Drawing.Font("Palatino Linotype", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tabUye.Location = new System.Drawing.Point(0, 0);
            this.tabUye.Name = "tabUye";
            this.tabUye.SelectedIndex = 0;
            this.tabUye.Size = new System.Drawing.Size(1010, 542);
            this.tabUye.TabIndex = 0;
            // 
            // tabEmanetlerim
            // 
            this.tabEmanetlerim.Controls.Add(this.dgvMyBorrows);
            this.tabEmanetlerim.Controls.Add(this.chkOnlyActive);
            this.tabEmanetlerim.Controls.Add(this.btnRefreshBorrows);
            this.tabEmanetlerim.Controls.Add(this.pictureBox1);
            this.tabEmanetlerim.Location = new System.Drawing.Point(4, 32);
            this.tabEmanetlerim.Name = "tabEmanetlerim";
            this.tabEmanetlerim.Padding = new System.Windows.Forms.Padding(8);
            this.tabEmanetlerim.Size = new System.Drawing.Size(1002, 506);
            this.tabEmanetlerim.TabIndex = 0;
            this.tabEmanetlerim.Text = "Emanetlerim";
            this.tabEmanetlerim.UseVisualStyleBackColor = true;
            // 
            // dgvMyBorrows
            // 
            this.dgvMyBorrows.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMyBorrows.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyBorrows.Location = new System.Drawing.Point(34, 99);
            this.dgvMyBorrows.Name = "dgvMyBorrows";
            this.dgvMyBorrows.RowHeadersWidth = 51;
            this.dgvMyBorrows.RowTemplate.Height = 24;
            this.dgvMyBorrows.Size = new System.Drawing.Size(924, 381);
            this.dgvMyBorrows.TabIndex = 0;
            // 
            // chkOnlyActive
            // 
            this.chkOnlyActive.AutoSize = true;
            this.chkOnlyActive.BackColor = System.Drawing.Color.Linen;
            this.chkOnlyActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.chkOnlyActive.Location = new System.Drawing.Point(34, 51);
            this.chkOnlyActive.Name = "chkOnlyActive";
            this.chkOnlyActive.Size = new System.Drawing.Size(210, 27);
            this.chkOnlyActive.TabIndex = 1;
            this.chkOnlyActive.Text = "Sadece Aktif Emanetler";
            this.chkOnlyActive.UseVisualStyleBackColor = false;
            // 
            // btnRefreshBorrows
            // 
            this.btnRefreshBorrows.BackColor = System.Drawing.Color.Linen;
            this.btnRefreshBorrows.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnRefreshBorrows.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnRefreshBorrows.Location = new System.Drawing.Point(283, 43);
            this.btnRefreshBorrows.Name = "btnRefreshBorrows";
            this.btnRefreshBorrows.Size = new System.Drawing.Size(130, 42);
            this.btnRefreshBorrows.TabIndex = 2;
            this.btnRefreshBorrows.Text = "Yenile";
            this.btnRefreshBorrows.UseVisualStyleBackColor = false;
            // 
            // tabTaleplerim
            // 
            this.tabTaleplerim.Controls.Add(this.dgvMyRequests);
            this.tabTaleplerim.Controls.Add(this.btnRefreshRequests);
            this.tabTaleplerim.Controls.Add(this.pictureBox2);
            this.tabTaleplerim.Location = new System.Drawing.Point(4, 32);
            this.tabTaleplerim.Name = "tabTaleplerim";
            this.tabTaleplerim.Padding = new System.Windows.Forms.Padding(8);
            this.tabTaleplerim.Size = new System.Drawing.Size(1002, 506);
            this.tabTaleplerim.TabIndex = 1;
            this.tabTaleplerim.Text = "Taleplerim";
            this.tabTaleplerim.UseVisualStyleBackColor = true;
            // 
            // dgvMyRequests
            // 
            this.dgvMyRequests.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMyRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyRequests.Location = new System.Drawing.Point(35, 97);
            this.dgvMyRequests.Name = "dgvMyRequests";
            this.dgvMyRequests.RowHeadersWidth = 51;
            this.dgvMyRequests.RowTemplate.Height = 24;
            this.dgvMyRequests.Size = new System.Drawing.Size(926, 382);
            this.dgvMyRequests.TabIndex = 0;
            // 
            // btnRefreshRequests
            // 
            this.btnRefreshRequests.BackColor = System.Drawing.Color.Linen;
            this.btnRefreshRequests.Font = new System.Drawing.Font("Palatino Linotype", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnRefreshRequests.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnRefreshRequests.Location = new System.Drawing.Point(35, 40);
            this.btnRefreshRequests.Name = "btnRefreshRequests";
            this.btnRefreshRequests.Size = new System.Drawing.Size(130, 41);
            this.btnRefreshRequests.TabIndex = 1;
            this.btnRefreshRequests.Text = "Yenile";
            this.btnRefreshRequests.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Kutuphane.UI.Properties.Resources._2ae7ede073c1080482435f34f81ff66f2;
            this.pictureBox1.Location = new System.Drawing.Point(8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(986, 490);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Image = global::Kutuphane.UI.Properties.Resources._2ae7ede073c1080482435f34f81ff66f2;
            this.pictureBox2.Location = new System.Drawing.Point(8, 8);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(986, 490);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // UyePanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 542);
            this.Controls.Add(this.tabUye);
            this.Name = "UyePanelForm";
            this.Text = "Üye Paneli";
            this.tabUye.ResumeLayout(false);
            this.tabEmanetlerim.ResumeLayout(false);
            this.tabEmanetlerim.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyBorrows)).EndInit();
            this.tabTaleplerim.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyRequests)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
