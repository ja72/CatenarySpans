using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JA.Engineering;
using JA.Gdi;

namespace JA.CatenarySpans
{
    public partial class TensionForm : Form
    {
        public TensionForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public Catenary Catenary
        {
            get { return catenaryBindingSource.DataSource as Catenary; }
            set
            {
                catenaryBindingSource.DataSource=value;
                CatenaryCurrecnyManager.Refresh();
            }
        }

        public ProjectUnits Units
        {
            get { return projectUnitsBindingSource.DataSource as ProjectUnits; }
            set
            {
                projectUnitsBindingSource.DataSource=value;
                ProjectUnitsCurrencyManager.Refresh();
            }
        }

        public CurrencyManager CatenaryCurrecnyManager
        {
            get { return BindingContext[catenaryBindingSource] as CurrencyManager; }
        }
        public CurrencyManager ProjectUnitsCurrencyManager
        {
            get { return BindingContext[projectUnitsBindingSource] as CurrencyManager; }
        }

        private void TrueFalseLabel_TextChanged(object sender, EventArgs e)
        {
            Control control=sender as Control;
            string value=control.Text;

            control.BackColor=SystemColors.Control;
            control.ForeColor=SystemColors.HighlightText;
            if (value.Equals(true.ToString())) { control.BackColor=Color.Red; }
            if (value.Equals(false.ToString())) { control.BackColor=Color.Green; }
        }

        private void catenaryPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Style style=Style.Default;
            style.SetGraphicsQuality(e.Graphics);
            Vector2 min=Catenary.StartPosition, max=Catenary.EndPosition;
            Catenary.GetBounds(ref min, ref max);
            Canvas canvas=new Canvas(pictureBox.ClientRectangle, min, max);
            CatenaryGraphics.RenderOne(e.Graphics, canvas, Catenary, style);
        }

        private void catenaryPictureBox_Resize(object sender, EventArgs e)
        {
            pictureBox.Refresh();
        }

        private void catenaryBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            pictureBox.Refresh();
        }

        private void catenaryBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            pictureBox.Refresh();
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            var img=new Bitmap(Size.Width, Size.Height);
            this.DrawToBitmap(img, new Rectangle(Point.Empty, Size));
            var doc=new JA.Printing.PrintDoc();
            var draw=new JA.Printing.Picture(img) { FitToPage=true, Caption="Tension Properties" };
            doc.Add(draw);
            var dlg = new JA.Printing.PrintPreviewAndSettings
            {
                Document = doc,
                StartPosition = FormStartPosition.CenterParent
            };
            dlg.ShowDialog();

        }

        private void projectUnitsBindingSource_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            if (e.BindingCompleteState!=BindingCompleteState.Success)
            {
                System.Diagnostics.Debug.WriteLine("Units Binding Error:");
            }
        }

    }
}
