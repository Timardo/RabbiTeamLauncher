using System.Windows.Forms;

namespace RabbiTeamLauncher
{
    partial class Launcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            this.Size = new System.Drawing.Size(304, 242);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.StubLabel = new System.Windows.Forms.Label();
            this.NyanCat = new System.Windows.Forms.PictureBox();
            this.StubLabel = new System.Windows.Forms.Label();
            this.NyanCat = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.NyanCat)).BeginInit();
            this.SuspendLayout();
            this.StubLabel.AutoEllipsis = true;
            this.StubLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.StubLabel.Location = new System.Drawing.Point(0, 0);
            this.StubLabel.Name = "stubLabel";
            this.StubLabel.Size = new System.Drawing.Size(288, 28);
            this.StubLabel.TabIndex = 0;
            this.StubLabel.Text = "Since the Launcher cannot start until all dependency libs are downloaded, enjoy this Nyan Cat (so mainstream)";
            this.NyanCat.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.ErrorImage")));
            this.NyanCat.Image = global::RabbiTeamLauncher.Properties.Resources.original;
            this.NyanCat.InitialImage = ((System.Drawing.Image)(resources.GetObject("nyanCat.InitialImage")));
            this.NyanCat.Location = new System.Drawing.Point(12, 31);
            this.NyanCat.Name = "nyanCat";
            this.NyanCat.Size = new System.Drawing.Size(272, 168);
            this.NyanCat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.NyanCat.TabIndex = 1;
            this.NyanCat.TabStop = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 203);
            this.Controls.Add(this.NyanCat);
            this.Controls.Add(this.StubLabel);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "LauncherBody";
            this.Text = "NyanCat";
            ((System.ComponentModel.ISupportInitialize)(this.NyanCat)).EndInit();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            this.NickBox = new System.Windows.Forms.TextBox();
            this.NickLabel = new System.Windows.Forms.Label();
            this.PlayButton = new System.Windows.Forms.Button();
            this.MemoryAllocationBox = new System.Windows.Forms.TextBox();
            this.RamLabel = new System.Windows.Forms.Label();
            this.RecommendedRamLabel = new System.Windows.Forms.Label();
            this.ClickableStuff = new System.Windows.Forms.Panel();
            this.PackList = new System.Windows.Forms.ComboBox();
            this.PackLabel = new System.Windows.Forms.Label();
            this.CreditsButton = new System.Windows.Forms.Button();
            this.AdvancedSettingsButton = new System.Windows.Forms.Button();
            this.OfficialModpackButton = new System.Windows.Forms.Button();
            this.Browser = new System.Windows.Forms.Panel();
            this.ClickableStuff.SuspendLayout();
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
            this.MemoryAllocationBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MemoryAllocationBoxKeyPressed);
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
            // ClickableStuff
            // 
            this.ClickableStuff.BackgroundImage = global::RabbiTeamLauncher.Properties.Resources.dirt;
            this.ClickableStuff.Controls.Add(this.PackList);
            this.ClickableStuff.Controls.Add(this.PackLabel);
            this.ClickableStuff.Controls.Add(this.CreditsButton);
            this.ClickableStuff.Controls.Add(this.PlayButton);
            this.ClickableStuff.Controls.Add(this.AdvancedSettingsButton);
            this.ClickableStuff.Controls.Add(this.RecommendedRamLabel);
            this.ClickableStuff.Controls.Add(this.MemoryAllocationBox);
            this.ClickableStuff.Controls.Add(this.OfficialModpackButton);
            this.ClickableStuff.Controls.Add(this.NickBox);
            this.ClickableStuff.Controls.Add(this.NickLabel);
            this.ClickableStuff.Controls.Add(this.RamLabel);
            this.ClickableStuff.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ClickableStuff.Location = new System.Drawing.Point(0, 411);
            this.ClickableStuff.Name = "ClickableStuff";
            this.ClickableStuff.Size = new System.Drawing.Size(1000, 125);
            this.ClickableStuff.TabIndex = 8;
            // 
            // PackList
            // 
            this.PackList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PackList.FormattingEnabled = true;
            this.PackList.Location = new System.Drawing.Point(50, 73);
            this.PackList.Name = "PackList";
            this.PackList.Size = new System.Drawing.Size(100, 23);
            this.PackList.TabIndex = 11;
            this.PackList.SelectedIndexChanged += new System.EventHandler(this.PackListSelect);
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
            // Launcher
            // 
            this.AcceptButton = this.PlayButton;
            this.AccessibleDescription = "RabbiTeam Launcher";
            this.AccessibleName = "RabbiTeam Launcher";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 536);
            this.Controls.Add(this.ClickableStuff);
            this.Controls.Add(this.Browser);
            this.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1016, 575);
            this.Name = "Launcher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RabbiTeam Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LauncherBodyClosing);
            this.Resize += new System.EventHandler(this.LauncherBodyResize);
            this.ClickableStuff.ResumeLayout(false);
            this.ClickableStuff.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox NickBox;
        private System.Windows.Forms.Label NickLabel;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.TextBox MemoryAllocationBox;
        private System.Windows.Forms.Label RamLabel;
        private System.Windows.Forms.Label RecommendedRamLabel;
        private System.Windows.Forms.Panel ClickableStuff;
        private System.Windows.Forms.Button OfficialModpackButton;
        private System.Windows.Forms.Panel Browser;
        private System.Windows.Forms.Button CreditsButton;
        private System.Windows.Forms.Button AdvancedSettingsButton;
        private System.Windows.Forms.Label StubLabel;
        private System.Windows.Forms.PictureBox NyanCat;
        private System.Windows.Forms.Label PackLabel;
        private System.Windows.Forms.ComboBox PackList;
    }
}

