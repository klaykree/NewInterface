namespace NewUI
{
    partial class Clickable
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
            this.components = new System.ComponentModel.Container();
            this.MouseoverName = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // Clickable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Clickable";
            this.ShowInTaskbar = false;
            this.Text = "Clickable";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Clickable_MouseClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip MouseoverName;
    }
}