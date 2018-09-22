namespace RabbiTeamLauncher
{
    partial class LauncherBody
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

        private void InitializeStub()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LauncherBody));
            this.Size = new System.Drawing.Size(304, 242);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.stubLabel = new System.Windows.Forms.Label();
            this.nyanCat = new System.Windows.Forms.PictureBox();
            this.stubLabel = new System.Windows.Forms.Label();
            this.nyanCat = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nyanCat)).BeginInit();
            this.SuspendLayout();
            this.stubLabel.AutoEllipsis = true;
            this.stubLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.stubLabel.Location = new System.Drawing.Point(0, 0);
            this.stubLabel.Name = "stubLabel";
            this.stubLabel.Size = new System.Drawing.Size(288, 28);
            this.stubLabel.TabIndex = 0;
            this.stubLabel.Text = "Since the Launcher cannot start until all dependency libs are downloaded, enjoy this Nyan Cat (so mainstream)";
            this.nyanCat.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.ErrorImage")));
            this.nyanCat.Image = global::RabbiTeamLauncher.Properties.Resources.original;
            this.nyanCat.InitialImage = ((System.Drawing.Image)(resources.GetObject("nyanCat.InitialImage")));
            this.nyanCat.Location = new System.Drawing.Point(12, 31);
            this.nyanCat.Name = "nyanCat";
            this.nyanCat.Size = new System.Drawing.Size(272, 168);
            this.nyanCat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.nyanCat.TabIndex = 1;
            this.nyanCat.TabStop = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 203);
            this.Controls.Add(this.nyanCat);
            this.Controls.Add(this.stubLabel);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "LauncherBody";
            this.Text = "NyanCat";
            ((System.ComponentModel.ISupportInitialize)(this.nyanCat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LauncherBody));
            this.NickBox = new System.Windows.Forms.TextBox();
            this.NickLabel = new System.Windows.Forms.Label();
            this.PlayButton = new System.Windows.Forms.Button();
            this.MemoryAllocationBox = new System.Windows.Forms.TextBox();
            this.RamLabel = new System.Windows.Forms.Label();
            this.RecommendedRamLabel = new System.Windows.Forms.Label();
            this.clickableStuff = new System.Windows.Forms.Panel();
            this.PackLabel = new System.Windows.Forms.Label();
            this.CreditsButton = new System.Windows.Forms.Button();
            this.AdvancedSettingsButton = new System.Windows.Forms.Button();
            this.OfficialModpackButton = new System.Windows.Forms.Button();
            this.Browser = new System.Windows.Forms.Panel();
            this.clickableStuff.SuspendLayout();
            this.SuspendLayout();
            // 
            // NickBox
            // 
            this.NickBox.BackColor = System.Drawing.SystemColors.Window;
            this.NickBox.Location = new System.Drawing.Point(50, 23);
            this.NickBox.Name = "NickBox";
            this.NickBox.Size = new System.Drawing.Size(100, 23);
            this.NickBox.TabIndex = 0;
            this.NickBox.TextChanged += new System.EventHandler(this.NickBoxTextChanged);
            // 
            // NickLabel
            // 
            this.NickLabel.AutoSize = true;
            this.NickLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NickLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.NickLabel.Location = new System.Drawing.Point(12, 23);
            this.NickLabel.Name = "NickLabel";
            this.NickLabel.Size = new System.Drawing.Size(32, 15);
            this.NickLabel.TabIndex = 1;
            this.NickLabel.Text = "Nick";
            // 
            // PlayButton
            // 
            this.PlayButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PlayButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.PlayButton.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PlayButton.Location = new System.Drawing.Point(339, 34);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(322, 56);
            this.PlayButton.TabIndex = 2;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = false;
            this.PlayButton.Click += new System.EventHandler(this.PlayButtonClick);
            // 
            // MemoryAllocationBox
            // 
            this.MemoryAllocationBox.Location = new System.Drawing.Point(194, 20);
            this.MemoryAllocationBox.Name = "MemoryAllocationBox";
            this.MemoryAllocationBox.Size = new System.Drawing.Size(100, 23);
            this.MemoryAllocationBox.TabIndex = 4;
            this.MemoryAllocationBox.Text = "3000";
            this.MemoryAllocationBox.TextChanged += new System.EventHandler(this.MemoryAllocationBoxTextChanged);
            // 
            // RamLabel
            // 
            this.RamLabel.AutoSize = true;
            this.RamLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.RamLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.RamLabel.Location = new System.Drawing.Point(156, 26);
            this.RamLabel.Name = "RamLabel";
            this.RamLabel.Size = new System.Drawing.Size(32, 15);
            this.RamLabel.TabIndex = 5;
            this.RamLabel.Text = "RAM";
            // 
            // RecommendedRamLabel
            // 
            this.RecommendedRamLabel.AutoSize = true;
            this.RecommendedRamLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.RecommendedRamLabel.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.RecommendedRamLabel.Location = new System.Drawing.Point(189, 2);
            this.RecommendedRamLabel.Name = "RecommendedRamLabel";
            this.RecommendedRamLabel.Size = new System.Drawing.Size(110, 15);
            this.RecommendedRamLabel.TabIndex = 6;
            this.RecommendedRamLabel.Text = "*3 GB recommended";
            // 
            // clickableStuff
            // 
            this.clickableStuff.BackgroundImage = global::RabbiTeamLauncher.Properties.Resources.dirt;
            this.clickableStuff.Controls.Add(this.PackLabel);
            this.clickableStuff.Controls.Add(this.CreditsButton);
            this.clickableStuff.Controls.Add(this.PlayButton);
            this.clickableStuff.Controls.Add(this.AdvancedSettingsButton);
            this.clickableStuff.Controls.Add(this.RecommendedRamLabel);
            this.clickableStuff.Controls.Add(this.MemoryAllocationBox);
            this.clickableStuff.Controls.Add(this.OfficialModpackButton);
            this.clickableStuff.Controls.Add(this.NickBox);
            this.clickableStuff.Controls.Add(this.NickLabel);
            this.clickableStuff.Controls.Add(this.RamLabel);
            this.clickableStuff.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.clickableStuff.Location = new System.Drawing.Point(0, 411);
            this.clickableStuff.Name = "clickableStuff";
            this.clickableStuff.Size = new System.Drawing.Size(1000, 125);
            this.clickableStuff.TabIndex = 8;
            // 
            // PackLabel
            // 
            this.PackLabel.AutoSize = true;
            this.PackLabel.BackColor = System.Drawing.Color.Transparent;
            this.PackLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.PackLabel.Location = new System.Drawing.Point(12, 76);
            this.PackLabel.Name = "PackLabel";
            this.PackLabel.Size = new System.Drawing.Size(31, 15);
            this.PackLabel.TabIndex = 12;
            this.PackLabel.Text = "Pack";
            // 
            // CreditsButton
            // 
            this.CreditsButton.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CreditsButton.Location = new System.Drawing.Point(877, 86);
            this.CreditsButton.Name = "CreditsButton";
            this.CreditsButton.Size = new System.Drawing.Size(111, 33);
            this.CreditsButton.TabIndex = 10;
            this.CreditsButton.Text = "Credits";
            this.CreditsButton.UseVisualStyleBackColor = true;
            // 
            // AdvancedSettingsButton
            // 
            this.AdvancedSettingsButton.Location = new System.Drawing.Point(877, 46);
            this.AdvancedSettingsButton.Name = "AdvancedSettingsButton";
            this.AdvancedSettingsButton.Size = new System.Drawing.Size(111, 33);
            this.AdvancedSettingsButton.TabIndex = 9;
            this.AdvancedSettingsButton.Text = "Advanced Settings";
            this.AdvancedSettingsButton.UseVisualStyleBackColor = true;
            this.AdvancedSettingsButton.Click += new System.EventHandler(this.AdvancedSettingsButtonClick);
            // 
            // OfficialModpackButton
            // 
            this.OfficialModpackButton.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.OfficialModpackButton.Location = new System.Drawing.Point(877, 6);
            this.OfficialModpackButton.Name = "OfficialModpackButton";
            this.OfficialModpackButton.Size = new System.Drawing.Size(111, 33);
            this.OfficialModpackButton.TabIndex = 8;
            this.OfficialModpackButton.Text = "RabbitPack";
            this.OfficialModpackButton.UseVisualStyleBackColor = true;
            this.OfficialModpackButton.Click += new System.EventHandler(this.OfficialPackButtonClick);
            // 
            // Browser
            // 
            this.Browser.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Browser.BackColor = System.Drawing.SystemColors.Control;
            this.Browser.Location = new System.Drawing.Point(0, 0);
            this.Browser.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(1000, 408);
            this.Browser.TabIndex = 10;
            // 
            // LauncherBody
            // 
            this.AcceptButton = this.PlayButton;
            this.AccessibleDescription = "RabbiTeam Launcher";
            this.AccessibleName = "RabbiTeam Launcher";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 536);
            this.Controls.Add(this.clickableStuff);
            this.Controls.Add(this.Browser);
            this.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1016, 575);
            this.Name = "LauncherBody";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RabbiTeam Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LauncherBodyClosing);
            this.Resize += new System.EventHandler(this.LauncherBodyResize);
            this.clickableStuff.ResumeLayout(false);
            this.clickableStuff.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox NickBox;
        private System.Windows.Forms.Label NickLabel;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.TextBox MemoryAllocationBox;
        private System.Windows.Forms.Label RamLabel;
        private System.Windows.Forms.Label RecommendedRamLabel;
        private System.Windows.Forms.Panel clickableStuff;
        private System.Windows.Forms.Button OfficialModpackButton;
        private System.Windows.Forms.Panel Browser;
        private System.Windows.Forms.Button CreditsButton;
        private System.Windows.Forms.Button AdvancedSettingsButton;
        private System.Windows.Forms.Label stubLabel;
        private System.Windows.Forms.PictureBox nyanCat;
        private System.Windows.Forms.Label PackLabel;
    }
}

