namespace Kutuphane.UI
{
    partial class MyRequestsForm
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
            this.dgvMyRequests = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyRequests)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMyRequests
            // 
            this.dgvMyRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyRequests.Location = new System.Drawing.Point(12, 94);
            this.dgvMyRequests.Name = "dgvMyRequests";
            this.dgvMyRequests.RowHeadersWidth = 51;
            this.dgvMyRequests.RowTemplate.Height = 24;
            this.dgvMyRequests.Size = new System.Drawing.Size(571, 356);
            this.dgvMyRequests.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(685, 239);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(143, 53);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "button1";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // MyRequestsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 479);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.dgvMyRequests);
            this.Name = "MyRequestsForm";
            this.Text = "MyRequestsForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyRequests)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMyRequests;
        private System.Windows.Forms.Button btnRefresh;
    }
}