using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace JA.UI
{
    public partial class PrintPreviewAndSettings : Form
    {
        public PrintPreviewAndSettings()
        {
            InitializeComponent();
        }

        public PrintDoc Document
        {
            get { return doc; }
            set
            {
                doc = value;
                pp.Document = doc;
            }
        }

        private void PrintPreviewAndSettings_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;            
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog
            {
                Document = doc,
                ShowNetwork = true,
                UseEXDialog = true,
                AllowCurrentPage = true,
                AllowPrintToFile = true,
                AllowSelection = true,
                AllowSomePages = true,
                ShowHelp = true,
                PrinterSettings = doc.PrinterSettings
            };
            if (pd.ShowDialog() == DialogResult.OK)
            {                
                doc.Print();
                pp.InvalidatePreview();
            }
        }

        private void btPrinterSettings_Click(object sender, EventArgs e)
        {
            PageSetupDialog psd = new PageSetupDialog
            {
                Document = doc,
                ShowNetwork = true,
                AllowMargins = true,
                AllowOrientation = true,
                AllowPaper = true,
                AllowPrinter = true,
                PageSettings = doc.DefaultPageSettings,
                PrinterSettings = doc.PrinterSettings
            };
            if (psd.ShowDialog() == DialogResult.OK)
            {
                doc.DefaultPageSettings = psd.PageSettings;
                doc.PrinterSettings = psd.PrinterSettings;

                pp.InvalidatePreview();
            }
        }


        private void btAuto_Click(object sender, EventArgs e)
        {
            pp.AutoZoom = true;
        }

        private void bt50_Click(object sender, EventArgs e)
        {
            pp.Zoom = 0.5;
        }

        private void bt100_Click(object sender, EventArgs e)
        {
            pp.Zoom = 1;
        }

        private void bt150_Click(object sender, EventArgs e)
        {
            pp.Zoom = 1.5;
        }


        private void txtZoom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double z = double.Parse(txtZoom.Text);
                if (z > 0 && z < 5)
                {
                    pp.Zoom = z;
                    //pp.InvalidatePreview();
                }
                else if (z >= 5)
                {
                    pp.Zoom = z / 100;
                }
            }
        }
    }
}
