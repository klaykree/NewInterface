using System;

namespace NewUI
{
    partial class UI
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
            if (ptrKbdHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(ptrKbdHook);
                ptrKbdHook = IntPtr.Zero;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI));
            this.StartBtn = new System.Windows.Forms.Button();
            this.AppHotKey = new System.Windows.Forms.Button();
            this.AppHotkeyLabel = new System.Windows.Forms.Label();
            this.TimeHotKeyLabel = new System.Windows.Forms.Label();
            this.TimeHotKey = new System.Windows.Forms.Button();
            this.DisableAppKey = new System.Windows.Forms.CheckBox();
            this.DisableTimeKey = new System.Windows.Forms.CheckBox();
            this.Reset = new System.Windows.Forms.Button();
            this.HelpBtn = new System.Windows.Forms.Button();
            this.DisabledBox = new System.Windows.Forms.CheckBox();
            this.Startup = new System.Windows.Forms.CheckBox();
            this.ApplicationList = new System.Windows.Forms.ListBox();
            this.AddAppBtn = new System.Windows.Forms.Button();
            this.RemoveAppBtn = new System.Windows.Forms.Button();
            this.ListBoxDesc = new System.Windows.Forms.Label();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.TimeFont = new System.Windows.Forms.Button();
            this.AddSiteBtn = new System.Windows.Forms.Button();
            this.SettingsFolderBtn = new System.Windows.Forms.Button();
            this.Diamond = new System.Windows.Forms.RadioButton();
            this.Circle = new System.Windows.Forms.RadioButton();
            this.Speed = new System.Windows.Forms.TrackBar();
            this.SpeedLabel = new System.Windows.Forms.Label();
            this.Distance = new System.Windows.Forms.TrackBar();
            this.DistanceLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Speed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Distance)).BeginInit();
            this.SuspendLayout();
            // 
            // StartBtn
            // 
            this.StartBtn.Location = new System.Drawing.Point(110, 198);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(105, 36);
            this.StartBtn.TabIndex = 0;
            this.StartBtn.Text = "Hide";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // AppHotKey
            // 
            this.AppHotKey.Location = new System.Drawing.Point(2, 29);
            this.AppHotKey.Name = "AppHotKey";
            this.AppHotKey.Size = new System.Drawing.Size(83, 23);
            this.AppHotKey.TabIndex = 8;
            this.AppHotKey.Text = "None";
            this.AppHotKey.UseVisualStyleBackColor = true;
            this.AppHotKey.Click += new System.EventHandler(this.AppHotKey_Click);
            // 
            // AppHotkeyLabel
            // 
            this.AppHotkeyLabel.AutoSize = true;
            this.AppHotkeyLabel.Location = new System.Drawing.Point(10, 13);
            this.AppHotkeyLabel.Name = "AppHotkeyLabel";
            this.AppHotkeyLabel.Size = new System.Drawing.Size(64, 13);
            this.AppHotkeyLabel.TabIndex = 9;
            this.AppHotkeyLabel.Text = "App hot key";
            // 
            // TimeHotKeyLabel
            // 
            this.TimeHotKeyLabel.AutoSize = true;
            this.TimeHotKeyLabel.Location = new System.Drawing.Point(121, 13);
            this.TimeHotKeyLabel.Name = "TimeHotKeyLabel";
            this.TimeHotKeyLabel.Size = new System.Drawing.Size(68, 13);
            this.TimeHotKeyLabel.TabIndex = 10;
            this.TimeHotKeyLabel.Text = "Date hot key";
            // 
            // TimeHotKey
            // 
            this.TimeHotKey.Location = new System.Drawing.Point(115, 29);
            this.TimeHotKey.Name = "TimeHotKey";
            this.TimeHotKey.Size = new System.Drawing.Size(86, 23);
            this.TimeHotKey.TabIndex = 11;
            this.TimeHotKey.Text = "None";
            this.TimeHotKey.UseVisualStyleBackColor = true;
            this.TimeHotKey.Click += new System.EventHandler(this.TimeHotkey_Click);
            // 
            // DisableAppKey
            // 
            this.DisableAppKey.AutoSize = true;
            this.DisableAppKey.Location = new System.Drawing.Point(2, 55);
            this.DisableAppKey.Name = "DisableAppKey";
            this.DisableAppKey.Size = new System.Drawing.Size(81, 17);
            this.DisableAppKey.TabIndex = 12;
            this.DisableAppKey.Text = "Disable key";
            this.DisableAppKey.UseVisualStyleBackColor = true;
            this.DisableAppKey.CheckedChanged += new System.EventHandler(this.DisableAppKey_CheckedChanged);
            // 
            // DisableTimeKey
            // 
            this.DisableTimeKey.AutoSize = true;
            this.DisableTimeKey.Location = new System.Drawing.Point(115, 58);
            this.DisableTimeKey.Name = "DisableTimeKey";
            this.DisableTimeKey.Size = new System.Drawing.Size(81, 17);
            this.DisableTimeKey.TabIndex = 13;
            this.DisableTimeKey.Text = "Disable key";
            this.DisableTimeKey.UseVisualStyleBackColor = true;
            this.DisableTimeKey.CheckedChanged += new System.EventHandler(this.DisableTimeKey_CheckedChanged);
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(2, 214);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(48, 20);
            this.Reset.TabIndex = 14;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // HelpBtn
            // 
            this.HelpBtn.Location = new System.Drawing.Point(89, 29);
            this.HelpBtn.Name = "HelpBtn";
            this.HelpBtn.Size = new System.Drawing.Size(22, 22);
            this.HelpBtn.TabIndex = 15;
            this.HelpBtn.Text = "?";
            this.HelpBtn.UseVisualStyleBackColor = true;
            this.HelpBtn.Click += new System.EventHandler(this.HelpBtn_Click);
            // 
            // DisabledBox
            // 
            this.DisabledBox.AutoSize = true;
            this.DisabledBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DisabledBox.Location = new System.Drawing.Point(242, 217);
            this.DisabledBox.Name = "DisabledBox";
            this.DisabledBox.Size = new System.Drawing.Size(67, 17);
            this.DisabledBox.TabIndex = 16;
            this.DisabledBox.Text = "Disabled";
            this.DisabledBox.UseVisualStyleBackColor = true;
            this.DisabledBox.CheckedChanged += new System.EventHandler(this.DisabledBox_CheckedChanged);
            // 
            // Startup
            // 
            this.Startup.AutoSize = true;
            this.Startup.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Startup.Location = new System.Drawing.Point(214, 198);
            this.Startup.Name = "Startup";
            this.Startup.Size = new System.Drawing.Size(95, 17);
            this.Startup.TabIndex = 17;
            this.Startup.Text = "Start at startup";
            this.Startup.UseVisualStyleBackColor = true;
            this.Startup.CheckedChanged += new System.EventHandler(this.Startup_CheckedChanged);
            // 
            // ApplicationList
            // 
            this.ApplicationList.AllowDrop = true;
            this.ApplicationList.FormattingEnabled = true;
            this.ApplicationList.HorizontalScrollbar = true;
            this.ApplicationList.Location = new System.Drawing.Point(5, 89);
            this.ApplicationList.Name = "ApplicationList";
            this.ApplicationList.Size = new System.Drawing.Size(178, 108);
            this.ApplicationList.TabIndex = 18;
            this.ApplicationList.DragDrop += new System.Windows.Forms.DragEventHandler(this.ApplicationList_DragDrop);
            this.ApplicationList.DragEnter += new System.Windows.Forms.DragEventHandler(this.ApplicationList_DragEnter);
            // 
            // AddAppBtn
            // 
            this.AddAppBtn.Location = new System.Drawing.Point(197, 103);
            this.AddAppBtn.Name = "AddAppBtn";
            this.AddAppBtn.Size = new System.Drawing.Size(75, 23);
            this.AddAppBtn.TabIndex = 19;
            this.AddAppBtn.Text = "Add";
            this.AddAppBtn.UseVisualStyleBackColor = true;
            this.AddAppBtn.Click += new System.EventHandler(this.AddAppBtn_Click);
            // 
            // RemoveAppBtn
            // 
            this.RemoveAppBtn.Location = new System.Drawing.Point(197, 151);
            this.RemoveAppBtn.Name = "RemoveAppBtn";
            this.RemoveAppBtn.Size = new System.Drawing.Size(75, 23);
            this.RemoveAppBtn.TabIndex = 20;
            this.RemoveAppBtn.Text = "Remove";
            this.RemoveAppBtn.UseVisualStyleBackColor = true;
            this.RemoveAppBtn.Click += new System.EventHandler(this.RemoveAppBtn_Click);
            // 
            // ListBoxDesc
            // 
            this.ListBoxDesc.AutoSize = true;
            this.ListBoxDesc.Location = new System.Drawing.Point(12, 75);
            this.ListBoxDesc.Name = "ListBoxDesc";
            this.ListBoxDesc.Size = new System.Drawing.Size(155, 13);
            this.ListBoxDesc.TabIndex = 21;
            this.ListBoxDesc.Text = "Drag files or folders into the box";
            // 
            // fontDialog1
            // 
            this.fontDialog1.Color = System.Drawing.SystemColors.ControlText;
            // 
            // TimeFont
            // 
            this.TimeFont.Location = new System.Drawing.Point(197, 175);
            this.TimeFont.Name = "TimeFont";
            this.TimeFont.Size = new System.Drawing.Size(75, 23);
            this.TimeFont.TabIndex = 22;
            this.TimeFont.Text = "Date font";
            this.TimeFont.UseVisualStyleBackColor = true;
            this.TimeFont.Click += new System.EventHandler(this.TimeFont_Click);
            // 
            // AddSiteBtn
            // 
            this.AddSiteBtn.Location = new System.Drawing.Point(197, 127);
            this.AddSiteBtn.Name = "AddSiteBtn";
            this.AddSiteBtn.Size = new System.Drawing.Size(75, 23);
            this.AddSiteBtn.TabIndex = 23;
            this.AddSiteBtn.Text = "Add site";
            this.AddSiteBtn.UseVisualStyleBackColor = true;
            this.AddSiteBtn.Click += new System.EventHandler(this.AddSiteBtn_Click);
            // 
            // SettingsFolderBtn
            // 
            this.SettingsFolderBtn.Location = new System.Drawing.Point(51, 198);
            this.SettingsFolderBtn.Name = "SettingsFolderBtn";
            this.SettingsFolderBtn.Size = new System.Drawing.Size(58, 36);
            this.SettingsFolderBtn.TabIndex = 24;
            this.SettingsFolderBtn.Text = "Settings folder";
            this.SettingsFolderBtn.UseVisualStyleBackColor = true;
            this.SettingsFolderBtn.Click += new System.EventHandler(this.SettingsFolderBtn_Click);
            // 
            // Diamond
            // 
            this.Diamond.AutoSize = true;
            this.Diamond.Checked = true;
            this.Diamond.Location = new System.Drawing.Point(194, 2);
            this.Diamond.Name = "Diamond";
            this.Diamond.Size = new System.Drawing.Size(67, 17);
            this.Diamond.TabIndex = 25;
            this.Diamond.TabStop = true;
            this.Diamond.Text = "Diamond";
            this.Diamond.UseVisualStyleBackColor = true;
            this.Diamond.CheckedChanged += new System.EventHandler(this.Diamond_CheckedChanged);
            // 
            // Circle
            // 
            this.Circle.AutoSize = true;
            this.Circle.Location = new System.Drawing.Point(262, 2);
            this.Circle.Name = "Circle";
            this.Circle.Size = new System.Drawing.Size(51, 17);
            this.Circle.TabIndex = 26;
            this.Circle.Text = "Circle";
            this.Circle.UseVisualStyleBackColor = true;
            this.Circle.CheckedChanged += new System.EventHandler(this.Circle_CheckedChanged);
            // 
            // Speed
            // 
            this.Speed.LargeChange = 1;
            this.Speed.Location = new System.Drawing.Point(209, 17);
            this.Speed.Maximum = 20;
            this.Speed.Minimum = 1;
            this.Speed.Name = "Speed";
            this.Speed.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Speed.Size = new System.Drawing.Size(104, 45);
            this.Speed.TabIndex = 27;
            this.Speed.Value = 10;
            this.Speed.Scroll += new System.EventHandler(this.SpeedScale_Scroll);
            // 
            // SpeedLabel
            // 
            this.SpeedLabel.AutoSize = true;
            this.SpeedLabel.Location = new System.Drawing.Point(231, 44);
            this.SpeedLabel.Name = "SpeedLabel";
            this.SpeedLabel.Size = new System.Drawing.Size(56, 13);
            this.SpeedLabel.TabIndex = 28;
            this.SpeedLabel.Text = "Speed: 10";
            // 
            // Distance
            // 
            this.Distance.LargeChange = 1;
            this.Distance.Location = new System.Drawing.Point(209, 56);
            this.Distance.Maximum = 70;
            this.Distance.Minimum = 40;
            this.Distance.Name = "Distance";
            this.Distance.Size = new System.Drawing.Size(104, 45);
            this.Distance.TabIndex = 29;
            this.Distance.Value = 50;
            this.Distance.Scroll += new System.EventHandler(this.Distance_Scroll);
            // 
            // DistanceLabel
            // 
            this.DistanceLabel.AutoSize = true;
            this.DistanceLabel.Location = new System.Drawing.Point(231, 84);
            this.DistanceLabel.Name = "DistanceLabel";
            this.DistanceLabel.Size = new System.Drawing.Size(67, 13);
            this.DistanceLabel.TabIndex = 30;
            this.DistanceLabel.Text = "Distance: 50";
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(309, 235);
            this.Controls.Add(this.DistanceLabel);
            this.Controls.Add(this.Distance);
            this.Controls.Add(this.SpeedLabel);
            this.Controls.Add(this.Speed);
            this.Controls.Add(this.Circle);
            this.Controls.Add(this.Diamond);
            this.Controls.Add(this.SettingsFolderBtn);
            this.Controls.Add(this.AddSiteBtn);
            this.Controls.Add(this.TimeFont);
            this.Controls.Add(this.ListBoxDesc);
            this.Controls.Add(this.RemoveAppBtn);
            this.Controls.Add(this.AddAppBtn);
            this.Controls.Add(this.ApplicationList);
            this.Controls.Add(this.Startup);
            this.Controls.Add(this.DisabledBox);
            this.Controls.Add(this.HelpBtn);
            this.Controls.Add(this.Reset);
            this.Controls.Add(this.DisableTimeKey);
            this.Controls.Add(this.DisableAppKey);
            this.Controls.Add(this.TimeHotKey);
            this.Controls.Add(this.TimeHotKeyLabel);
            this.Controls.Add(this.AppHotkeyLabel);
            this.Controls.Add(this.AppHotKey);
            this.Controls.Add(this.StartBtn);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UI";
            this.Text = "NI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UI_FormClosing);
            this.Load += new System.EventHandler(this.UI_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UI_MouseClick);
            ((System.ComponentModel.ISupportInitialize)(this.Speed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Distance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Button AppHotKey;
        private System.Windows.Forms.Label AppHotkeyLabel;
        private System.Windows.Forms.Label TimeHotKeyLabel;
        private System.Windows.Forms.Button TimeHotKey;
        private System.Windows.Forms.CheckBox DisableAppKey;
        private System.Windows.Forms.CheckBox DisableTimeKey;
        private System.Windows.Forms.Button Reset;
        private System.Windows.Forms.Button HelpBtn;
        private System.Windows.Forms.CheckBox DisabledBox;
        private System.Windows.Forms.CheckBox Startup;
        private System.Windows.Forms.ListBox ApplicationList;
        private System.Windows.Forms.Button AddAppBtn;
        private System.Windows.Forms.Button RemoveAppBtn;
        private System.Windows.Forms.Label ListBoxDesc;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button TimeFont;
        private System.Windows.Forms.Button AddSiteBtn;
        private System.Windows.Forms.Button SettingsFolderBtn;
        private System.Windows.Forms.RadioButton Diamond;
        private System.Windows.Forms.RadioButton Circle;
        private System.Windows.Forms.TrackBar Speed;
        private System.Windows.Forms.Label SpeedLabel;
        private System.Windows.Forms.TrackBar Distance;
        private System.Windows.Forms.Label DistanceLabel;
    }
}

