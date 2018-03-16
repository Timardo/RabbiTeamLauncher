namespace RabbiTeamLauncher
{
    partial class testLauncher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(testLauncher));
            this.nick = new System.Windows.Forms.TextBox();
            this.nickDesc = new System.Windows.Forms.Label();
            this.playButt = new System.Windows.Forms.Button();
            this.showConsole = new System.Windows.Forms.CheckBox();
            this.memoryAllocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // nick
            // 
            this.nick.BackColor = System.Drawing.SystemColors.Window;
            this.nick.Location = new System.Drawing.Point(12, 34);
            this.nick.Name = "nick";
            this.nick.Size = new System.Drawing.Size(100, 23);
            this.nick.TabIndex = 0;
            this.nick.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.nick.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.testLauncher_KeyPress);
            // 
            // nickDesc
            // 
            this.nickDesc.AutoSize = true;
            this.nickDesc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nickDesc.Location = new System.Drawing.Point(12, 9);
            this.nickDesc.Name = "nickDesc";
            this.nickDesc.Size = new System.Drawing.Size(32, 15);
            this.nickDesc.TabIndex = 1;
            this.nickDesc.Text = "Nick";
            this.nickDesc.Click += new System.EventHandler(this.label1_Click);
            // 
            // playButt
            // 
            this.playButt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.playButt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.playButt.Location = new System.Drawing.Point(12, 63);
            this.playButt.Name = "playButt";
            this.playButt.Size = new System.Drawing.Size(75, 23);
            this.playButt.TabIndex = 2;
            this.playButt.Text = "Play";
            this.playButt.UseVisualStyleBackColor = false;
            this.playButt.Click += new System.EventHandler(this.playButt_Click);
            // 
            // showConsole
            // 
            this.showConsole.AutoSize = true;
            this.showConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.showConsole.Checked = true;
            this.showConsole.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showConsole.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.showConsole.Location = new System.Drawing.Point(12, 92);
            this.showConsole.Name = "showConsole";
            this.showConsole.Size = new System.Drawing.Size(96, 19);
            this.showConsole.TabIndex = 3;
            this.showConsole.Text = "Show Console";
            this.showConsole.UseVisualStyleBackColor = false;
            this.showConsole.CheckedChanged += new System.EventHandler(this.showConsole_CheckedChanged);
            // 
            // memoryAllocation
            // 
            this.memoryAllocation.Location = new System.Drawing.Point(161, 34);
            this.memoryAllocation.Name = "memoryAllocation";
            this.memoryAllocation.Size = new System.Drawing.Size(100, 23);
            this.memoryAllocation.TabIndex = 4;
            this.memoryAllocation.Text = "3000";
            this.memoryAllocation.TextChanged += new System.EventHandler(this.memoryAllocation_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(161, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "RAM (MB)";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(161, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "*3 GB recommended";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // testLauncher
            // 
            this.AccessibleDescription = "RabbiTeam Launcher";
            this.AccessibleName = "RabbiTeam Launcher";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(304, 141);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.memoryAllocation);
            this.Controls.Add(this.showConsole);
            this.Controls.Add(this.playButt);
            this.Controls.Add(this.nickDesc);
            this.Controls.Add(this.nick);
            this.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "testLauncher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RabbiTeam Launcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nick;
        private System.Windows.Forms.Label nickDesc;
        private System.Windows.Forms.Button playButt;
        private System.Windows.Forms.CheckBox showConsole;
        private System.Windows.Forms.TextBox memoryAllocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

