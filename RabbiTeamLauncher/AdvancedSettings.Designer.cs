namespace RabbiTeamLauncher
{
    partial class AdvancedSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedSettings));
            this.LauncherPathLabel = new System.Windows.Forms.Label();
            this.LauncherPath = new System.Windows.Forms.TextBox();
            this.MigrateButton = new System.Windows.Forms.Button();
            this.JvmArgsLabel = new System.Windows.Forms.Label();
            this.JavaArgs = new System.Windows.Forms.TextBox();
            this.ResetArgsButton = new System.Windows.Forms.Button();
            this.DevToolsButton = new System.Windows.Forms.Button();
            this.ShowConsoleCheck = new System.Windows.Forms.CheckBox();
            this.CloseAfterStart = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // LauncherPathLabel
            // 
            this.LauncherPathLabel.AutoSize = true;
            this.LauncherPathLabel.Location = new System.Drawing.Point(12, 15);
            this.LauncherPathLabel.Name = "LauncherPathLabel";
            this.LauncherPathLabel.Size = new System.Drawing.Size(80, 13);
            this.LauncherPathLabel.TabIndex = 0;
            this.LauncherPathLabel.Text = "Launcher Path:";
            // 
            // LauncherPath
            // 
            this.LauncherPath.Location = new System.Drawing.Point(98, 12);
            this.LauncherPath.Name = "LauncherPath";
            this.LauncherPath.ReadOnly = true;
            this.LauncherPath.Size = new System.Drawing.Size(623, 20);
            this.LauncherPath.TabIndex = 1;
            // 
            // MigrateButton
            // 
            this.MigrateButton.Location = new System.Drawing.Point(727, 10);
            this.MigrateButton.Name = "MigrateButton";
            this.MigrateButton.Size = new System.Drawing.Size(88, 23);
            this.MigrateButton.TabIndex = 2;
            this.MigrateButton.Text = "Migrate";
            this.MigrateButton.UseVisualStyleBackColor = true;
            this.MigrateButton.Click += new System.EventHandler(this.MigrateButtonClick);
            // 
            // JvmArgsLabel
            // 
            this.JvmArgsLabel.AutoSize = true;
            this.JvmArgsLabel.Location = new System.Drawing.Point(12, 42);
            this.JvmArgsLabel.Name = "JvmArgsLabel";
            this.JvmArgsLabel.Size = new System.Drawing.Size(84, 13);
            this.JvmArgsLabel.TabIndex = 3;
            this.JvmArgsLabel.Text = "JVM Arguments:";
            // 
            // JavaArgs
            // 
            this.JavaArgs.Location = new System.Drawing.Point(98, 39);
            this.JavaArgs.Name = "JavaArgs";
            this.JavaArgs.Size = new System.Drawing.Size(623, 20);
            this.JavaArgs.TabIndex = 4;
            this.JavaArgs.Text = resources.GetString("JavaArgs.Text");
            this.JavaArgs.TextChanged += new System.EventHandler(this.JavaArgsTextChanged);
            // 
            // ResetArgsButton
            // 
            this.ResetArgsButton.Location = new System.Drawing.Point(728, 37);
            this.ResetArgsButton.Name = "ResetArgsButton";
            this.ResetArgsButton.Size = new System.Drawing.Size(87, 23);
            this.ResetArgsButton.TabIndex = 5;
            this.ResetArgsButton.Text = "Reset";
            this.ResetArgsButton.UseVisualStyleBackColor = true;
            this.ResetArgsButton.Click += new System.EventHandler(this.ResetArgsButtonClick);
            // 
            // DevToolsButton
            // 
            this.DevToolsButton.Location = new System.Drawing.Point(728, 66);
            this.DevToolsButton.Name = "DevToolsButton";
            this.DevToolsButton.Size = new System.Drawing.Size(87, 23);
            this.DevToolsButton.TabIndex = 6;
            this.DevToolsButton.Text = "DevTools";
            this.DevToolsButton.UseVisualStyleBackColor = true;
            this.DevToolsButton.Click += new System.EventHandler(this.DevButtonClick);
            // 
            // ShowConsoleCheck
            // 
            this.ShowConsoleCheck.AutoSize = true;
            this.ShowConsoleCheck.Location = new System.Drawing.Point(15, 66);
            this.ShowConsoleCheck.Name = "ShowConsoleCheck";
            this.ShowConsoleCheck.Size = new System.Drawing.Size(94, 17);
            this.ShowConsoleCheck.TabIndex = 7;
            this.ShowConsoleCheck.Text = "Show Console";
            this.ShowConsoleCheck.UseVisualStyleBackColor = true;
            this.ShowConsoleCheck.CheckedChanged += new System.EventHandler(this.ShowConsoleCheckChanged);
            // 
            // CloseAfterStart
            // 
            this.CloseAfterStart.AutoSize = true;
            this.CloseAfterStart.Location = new System.Drawing.Point(15, 89);
            this.CloseAfterStart.Name = "CloseAfterStart";
            this.CloseAfterStart.Size = new System.Drawing.Size(77, 17);
            this.CloseAfterStart.TabIndex = 8;
            this.CloseAfterStart.Text = "Auto-Close";
            this.CloseAfterStart.UseVisualStyleBackColor = true;
            this.CloseAfterStart.CheckedChanged += new System.EventHandler(this.CloseAfterStartCheckChanged);
            // 
            // AdvancedSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 406);
            this.Controls.Add(this.CloseAfterStart);
            this.Controls.Add(this.ShowConsoleCheck);
            this.Controls.Add(this.DevToolsButton);
            this.Controls.Add(this.ResetArgsButton);
            this.Controls.Add(this.JavaArgs);
            this.Controls.Add(this.JvmArgsLabel);
            this.Controls.Add(this.MigrateButton);
            this.Controls.Add(this.LauncherPath);
            this.Controls.Add(this.LauncherPathLabel);
            this.Name = "AdvancedSettings";
            this.Text = "advSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LauncherPathLabel;
        private System.Windows.Forms.TextBox LauncherPath;
        private System.Windows.Forms.Button MigrateButton;
        private System.Windows.Forms.Label JvmArgsLabel;
        private System.Windows.Forms.TextBox JavaArgs;
        private System.Windows.Forms.Button ResetArgsButton;
        private System.Windows.Forms.Button DevToolsButton;
        private System.Windows.Forms.CheckBox ShowConsoleCheck;
        private System.Windows.Forms.CheckBox CloseAfterStart;
    }
}