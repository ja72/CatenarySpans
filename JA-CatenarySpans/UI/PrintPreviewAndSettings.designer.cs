namespace JA.UI
{
    partial class PrintPreviewAndSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <arg name="disposing">true if managed resources should be disposed; otherwise, false.</arg>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintPreviewAndSettings));
            this.pp = new System.Windows.Forms.PrintPreviewControl();
            this.ts = new System.Windows.Forms.ToolStrip();
            this.btPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btPrinterSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.btAuto = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bt50 = new System.Windows.Forms.ToolStripMenuItem();
            this.bt100 = new System.Windows.Forms.ToolStripMenuItem();
            this.bt150 = new System.Windows.Forms.ToolStripMenuItem();
            this.txtZoom = new System.Windows.Forms.ToolStripTextBox();
            this.doc = new PrintDoc();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.ts.SuspendLayout();
            this.SuspendLayout();
            // 
            // pp
            // 
            this.pp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pp.Location = new System.Drawing.Point(0, 25);
            this.pp.Name = "pp";
            this.pp.Size = new System.Drawing.Size(615, 461);
            this.pp.TabIndex = 0;
            // 
            // ts
            // 
            this.ts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btPrint,
            this.toolStripSeparator1,
            this.btPrinterSettings,
            this.toolStripLabel1,
            this.toolStripLabel2,
            this.toolStripDropDownButton1});
            this.ts.Location = new System.Drawing.Point(0, 0);
            this.ts.Name = "ts";
            this.ts.Size = new System.Drawing.Size(615, 25);
            this.ts.TabIndex = 1;
            this.ts.Text = "toolStrip1";
            // 
            // btPrint
            // 
            this.btPrint.Image = ((System.Drawing.Image)(resources.GetObject("btPrint.Image")));
            this.btPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btPrint.Name = "btPrint";
            this.btPrint.Size = new System.Drawing.Size(73, 22);
            this.btPrint.Text = "Print Now";
            this.btPrint.Click += new System.EventHandler(this.btPrint_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btPrinterSettings
            // 
            this.btPrinterSettings.Image = ((System.Drawing.Image)(resources.GetObject("btPrinterSettings.Image")));
            this.btPrinterSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btPrinterSettings.Name = "btPrinterSettings";
            this.btPrinterSettings.Size = new System.Drawing.Size(101, 22);
            this.btPrinterSettings.Text = "Printer Settings";
            this.btPrinterSettings.Click += new System.EventHandler(this.btPrinterSettings_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btAuto,
            this.toolStripSeparator2,
            this.bt50,
            this.bt100,
            this.bt150,
            this.txtZoom});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(62, 22);
            this.toolStripDropDownButton1.Text = "Zoom";
            // 
            // btAuto
            // 
            this.btAuto.Name = "btAuto";
            this.btAuto.Size = new System.Drawing.Size(152, 22);
            this.btAuto.Text = "Auto";
            this.btAuto.Click += new System.EventHandler(this.btAuto_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // bt50
            // 
            this.bt50.Name = "bt50";
            this.bt50.Size = new System.Drawing.Size(152, 22);
            this.bt50.Text = "50%";
            this.bt50.Click += new System.EventHandler(this.bt50_Click);
            // 
            // bt100
            // 
            this.bt100.Name = "bt100";
            this.bt100.Size = new System.Drawing.Size(152, 22);
            this.bt100.Text = "100%";
            this.bt100.Click += new System.EventHandler(this.bt100_Click);
            // 
            // bt150
            // 
            this.bt150.Name = "bt150";
            this.bt150.Size = new System.Drawing.Size(152, 22);
            this.bt150.Text = "150%";
            this.bt150.Click += new System.EventHandler(this.bt150_Click);
            // 
            // txtZoom
            // 
            this.txtZoom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtZoom.Name = "txtZoom";
            this.txtZoom.Size = new System.Drawing.Size(80, 21);
            this.txtZoom.Text = "133";
            this.txtZoom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtZoom_KeyDown);
            // 
            // doc
            // 
            this.doc.Landscape = false;
            this.doc.PrintInColor = true;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(47, 22);
            this.toolStripLabel1.Text = "Page Up";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(61, 22);
            this.toolStripLabel2.Text = "Page Down";
            // 
            // PrintPreviewAndSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 486);
            this.Controls.Add(this.pp);
            this.Controls.Add(this.ts);
            this.Name = "PrintPreviewAndSettings";
            this.Text = "Print Preview and Settings";
            this.Load += new System.EventHandler(this.PrintPreviewAndSettings_Load);
            this.ts.ResumeLayout(false);
            this.ts.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PrintPreviewControl pp;
        private System.Windows.Forms.ToolStrip ts;
        private System.Windows.Forms.ToolStripButton btPrint;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btPrinterSettings;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private PrintDoc doc;
        private System.Windows.Forms.ToolStripMenuItem btAuto;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem bt50;
        private System.Windows.Forms.ToolStripMenuItem bt100;
        private System.Windows.Forms.ToolStripMenuItem bt150;
        private System.Windows.Forms.ToolStripTextBox txtZoom;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
    }
}