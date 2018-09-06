namespace Laser_Build_1._0
{
    partial class Display_Dxf
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Display_Dxf));
            this.axMxDrawX1 = new AxMxDrawXLib.AxMxDrawX();
            ((System.ComponentModel.ISupportInitialize)(this.axMxDrawX1)).BeginInit();
            this.SuspendLayout();
            // 
            // axMxDrawX1
            // 
            axMxDrawX1.Enabled = true;
            this.axMxDrawX1.Location = new System.Drawing.Point(8, 10);
            this.axMxDrawX1.Name = "axMxDrawX1";
            this.axMxDrawX1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMxDrawX1.OcxState")));
            this.axMxDrawX1.Size = new System.Drawing.Size(900, 600);
            this.axMxDrawX1.TabIndex = 0;
            // 
            // Display_Dxf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new System.Drawing.Size(1380, 930);
            this.Controls.Add(this.axMxDrawX1);
            this.Name = "Display_Dxf";
            this.Text = "Display_Dxf";
            this.Load += new System.EventHandler(this.Display_Dxf_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axMxDrawX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public AxMxDrawXLib.AxMxDrawX axMxDrawX1=new AxMxDrawXLib.AxMxDrawX();
    }
}