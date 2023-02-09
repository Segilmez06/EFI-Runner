namespace EFI_Runner
{
    partial class VNCViewer
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
            this.Viewer = new VncSharp.RemoteDesktop();
            this.SuspendLayout();
            // 
            // Viewer
            // 
            this.Viewer.AutoScroll = true;
            this.Viewer.AutoScrollMinSize = new System.Drawing.Size(608, 427);
            this.Viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Viewer.Location = new System.Drawing.Point(0, 0);
            this.Viewer.Margin = new System.Windows.Forms.Padding(0);
            this.Viewer.Name = "Viewer";
            this.Viewer.Size = new System.Drawing.Size(801, 600);
            this.Viewer.TabIndex = 0;
            // 
            // VNCViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 600);
            this.Controls.Add(this.Viewer);
            this.Name = "VNCViewer";
            this.Text = "VNCViewer";
            this.Load += new System.EventHandler(this.VNCViewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private VncSharp.RemoteDesktop Viewer;
    }
}