namespace Kutuphane.UI
{
    partial class BookRequestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.lblPublisher = new System.Windows.Forms.Label();
            this.lblNote = new System.Windows.Forms.Label();
            this.btnSendRequest = new System.Windows.Forms.Button();
            this.txtRequestedTitle = new System.Windows.Forms.TextBox();
            this.txtRequestedAuthor = new System.Windows.Forms.TextBox();
            this.txtRequestedPublisher = new System.Windows.Forms.TextBox();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.picBgRequest = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBgRequest)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Linen;
            this.lblTitle.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(136, 120);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(105, 27);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Kitap Adı:";
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.BackColor = System.Drawing.Color.Linen;
            this.lblAuthor.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAuthor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblAuthor.Location = new System.Drawing.Point(136, 171);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(67, 27);
            this.lblAuthor.TabIndex = 1;
            this.lblAuthor.Text = "Yazar:";
            // 
            // lblPublisher
            // 
            this.lblPublisher.AutoSize = true;
            this.lblPublisher.BackColor = System.Drawing.Color.Linen;
            this.lblPublisher.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblPublisher.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblPublisher.Location = new System.Drawing.Point(136, 220);
            this.lblPublisher.Name = "lblPublisher";
            this.lblPublisher.Size = new System.Drawing.Size(95, 27);
            this.lblPublisher.TabIndex = 2;
            this.lblPublisher.Text = "Yayınevi:";
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.BackColor = System.Drawing.Color.Linen;
            this.lblNote.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblNote.Location = new System.Drawing.Point(136, 267);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(52, 27);
            this.lblNote.TabIndex = 3;
            this.lblNote.Text = "Not:";
            // 
            // btnSendRequest
            // 
            this.btnSendRequest.BackColor = System.Drawing.Color.Linen;
            this.btnSendRequest.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSendRequest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnSendRequest.Location = new System.Drawing.Point(619, 220);
            this.btnSendRequest.Name = "btnSendRequest";
            this.btnSendRequest.Size = new System.Drawing.Size(162, 59);
            this.btnSendRequest.TabIndex = 4;
            this.btnSendRequest.Text = "Talep Gönder";
            this.btnSendRequest.UseVisualStyleBackColor = false;
            // 
            // txtRequestedTitle
            // 
            this.txtRequestedTitle.BackColor = System.Drawing.Color.Linen;
            this.txtRequestedTitle.Location = new System.Drawing.Point(328, 120);
            this.txtRequestedTitle.Name = "txtRequestedTitle";
            this.txtRequestedTitle.Size = new System.Drawing.Size(198, 22);
            this.txtRequestedTitle.TabIndex = 5;
            // 
            // txtRequestedAuthor
            // 
            this.txtRequestedAuthor.BackColor = System.Drawing.Color.Linen;
            this.txtRequestedAuthor.Location = new System.Drawing.Point(328, 171);
            this.txtRequestedAuthor.Name = "txtRequestedAuthor";
            this.txtRequestedAuthor.Size = new System.Drawing.Size(198, 22);
            this.txtRequestedAuthor.TabIndex = 6;
            // 
            // txtRequestedPublisher
            // 
            this.txtRequestedPublisher.BackColor = System.Drawing.Color.Linen;
            this.txtRequestedPublisher.Location = new System.Drawing.Point(328, 220);
            this.txtRequestedPublisher.Name = "txtRequestedPublisher";
            this.txtRequestedPublisher.Size = new System.Drawing.Size(198, 22);
            this.txtRequestedPublisher.TabIndex = 7;
            // 
            // txtNote
            // 
            this.txtNote.BackColor = System.Drawing.Color.Linen;
            this.txtNote.Location = new System.Drawing.Point(328, 267);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNote.Size = new System.Drawing.Size(198, 107);
            this.txtNote.TabIndex = 8;
            // 
            // picBgRequest
            // 
            this.picBgRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBgRequest.Image = global::Kutuphane.UI.Properties.Resources.a_peaceful_library_with_a_variety_of_books_on_the_shelves_wallpaper_2880x1800_8;
            this.picBgRequest.Location = new System.Drawing.Point(0, 0);
            this.picBgRequest.Name = "picBgRequest";
            this.picBgRequest.Size = new System.Drawing.Size(957, 487);
            this.picBgRequest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picBgRequest.TabIndex = 9;
            this.picBgRequest.TabStop = false;
            // 
            // BookRequestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 487);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.txtRequestedPublisher);
            this.Controls.Add(this.txtRequestedAuthor);
            this.Controls.Add(this.txtRequestedTitle);
            this.Controls.Add(this.btnSendRequest);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.lblPublisher);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.picBgRequest);
            this.Name = "BookRequestForm";
            this.Text = "BookRequestForm";
            ((System.ComponentModel.ISupportInitialize)(this.picBgRequest)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Label lblPublisher;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Button btnSendRequest;
        private System.Windows.Forms.TextBox txtRequestedTitle;
        private System.Windows.Forms.TextBox txtRequestedAuthor;
        private System.Windows.Forms.TextBox txtRequestedPublisher;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.PictureBox picBgRequest;
    }
}