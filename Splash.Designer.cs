namespace MissionPlanner
{
    partial class Splash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            this.TXT_version = new System.Windows.Forms.Label();
            this.startupImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.startupImage)).BeginInit();
            this.SuspendLayout();
            // 
            // TXT_version
            // 
            this.TXT_version.BackColor = System.Drawing.Color.Transparent;
            this.TXT_version.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXT_version.ForeColor = System.Drawing.Color.White;
            this.TXT_version.Location = new System.Drawing.Point(467, 366);
            this.TXT_version.Name = "TXT_version";
            this.TXT_version.Size = new System.Drawing.Size(121, 25);
            this.TXT_version.TabIndex = 1;
            this.TXT_version.Text = "Version: ";
            this.TXT_version.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.TXT_version.Click += new System.EventHandler(this.TXT_version_Click);
            // 
            // startupImage
            // 
            this.startupImage.BackColor = System.Drawing.Color.Transparent;
            this.startupImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.startupImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startupImage.Location = new System.Drawing.Point(0, 0);
            this.startupImage.Name = "startupImage";
            this.startupImage.Size = new System.Drawing.Size(600, 400);
            this.startupImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.startupImage.TabIndex = 2;
            this.startupImage.TabStop = false;
            this.startupImage.Click += new System.EventHandler(this.startupImage_Click_1);
            // 
            // Splash
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.ControlBox = false;
            this.Controls.Add(this.TXT_version);
            this.Controls.Add(this.startupImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 1000);
            this.MinimizeBox = false;
            this.Name = "Splash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MissionPlanner-Plus";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.startupImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox startupImage;
        private System.Windows.Forms.Label TXT_version;
    }
}
