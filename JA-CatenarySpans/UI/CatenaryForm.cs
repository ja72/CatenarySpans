using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace JA.UI
{
    using JA.Engineering;

    [LicenseProvider(typeof(ComponentLicenseProvider))]
    public partial class CatenaryForm : Form
    {
        CatenaryGraphics vis;
        RulingSpan rs;
        bool _isDemo=true;

        #region Initializations
        public CatenaryForm()
        {
            InitializeComponent();

            RulingSpan=new RulingSpan(DefaultProjectUnits);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ReadLicense())
            {
                CheckLicense();

                if (!_isDemo)
                {
                    saveSpansButton.Enabled=true;
                    importSpansButton.Enabled=true;
                }
            }

            
            string path=Properties.Settings.Default.lastSave;
            if (File.Exists(path))
            {
                try
                {
                    RulingSpan=RulingSpan.OpenFile(path);
                    fileLabelStatusBar.Text=Path.GetFileName(path);
                }
                catch (System.IO.FileLoadException ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            if (rs.Spans.Count==0)
            {
                Catenary A = new Catenary(150*Vector2.UnitY, 400, 12, 0.75, 3000);
                Catenary B = new Catenary(A.EndPosition, 1400, -10, 0.75, 3000);
                Catenary C = new Catenary(B.EndPosition, 700, -20, 0.75, 3000);
                rs.AddSpans(A, B, C);
            }


            rs.Spans.RaiseListChangedEvents=true;
            var icon_green=Properties.Resources.flag_green;

            foreach (var item in ProjectUnits.PredefinedUnits)
            {
                var mi=item.ToMenuItem(icon_green, (tsi, pev) =>
                    {
                        rs.SetProjectUnits(pev.NewProjectUnits);
                        unitsDropDown.Text=rs.Units.ToString();
                        RefreshAndPaintGrid();
                    });
                unitsDropDown.DropDownItems.Add(mi);
            }

            unitsDropDown.Text=rs.Units.ToString();
        }

        private bool ReadLicense()
        {
            try
            {
                string keys=File.ReadAllText(this.GetType().Name+".lic");
                DeveloperKey=keys;
                return true;
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return false;
        }

        private void CheckLicense()
        {
            bool CheckLic()
            {
                if (LicenseManager.Validate(typeof(CatenaryForm), this) is ComponentLicense lic)
                {
                    return lic.IsDemo;
                }
                return false;
            }
#if DESIGN_ONLY
            if (Site != null && Site.DesignMode)
            {
                //Check license here
                _isDemo = CheckLic();
            }
            else
            {
                _isDemo = false;
                //Allow runtime usage.
            }
#else
            //Check license here
            _isDemo = CheckLic();
#endif

        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets the developer key for use on this control.
        /// </summary>
        [Category("License"),
          Description("Sets the license key for use on the control.")]
        public string LicenseKey
        {
            get { return DeveloperKey; }
            set
            {
                // d9d359be-040a-435a-ab21-69e6aa91d7d9-74afb16077bd116e
                DeveloperKey = value;
                CheckLicense();
                this.Invalidate();
            }
        }
        public bool IsDemo { get { return _isDemo; } }
        [Browsable(false)]
        public static string DeveloperKey { get; set; }

        public ProjectUnits DefaultProjectUnits
        {
            get
            {
                if (Enum.TryParse(Properties.Settings.Default.projectUnits, true, out ProjectUnitSystem system))
                {
                    return new ProjectUnits(system);
                }
                return new ProjectUnits(ProjectUnitSystem.FeetPoundSecond);
            }
        }

        public RulingSpan RulingSpan
        {
            get { return rs; }
            set
            {


                vis=null;

                if (rs!=null)
                {
                    rs.RulingSpanChanged-=new EventHandler<RulingSpan.ItemChangeEventArgs>(spans_RulingSpanChanged);
                    rs.ProjectUnitsChanged-=new EventHandler<ProjectUnits.ChangeEventArgs>(rs_ProjectUnitsChanged);
                }
                rs=value;
                if (value==null)
                {
                    rulingspanDataGrid.DataSource=typeof(Catenary);
                    rulingspanPropertyGrid.SelectedObject=typeof(RulingSpan);
                    SetupGridColumnText();
                    RefreshAndPaintGrid();
                    return;
                }

                rulingspanDataGrid.DataSource=rs.Spans;
                rulingspanPropertyGrid.SelectedObject=rs;
                rs.RulingSpanChanged+=new EventHandler<RulingSpan.ItemChangeEventArgs>(spans_RulingSpanChanged);
                rs.ProjectUnitsChanged+=new EventHandler<ProjectUnits.ChangeEventArgs>(rs_ProjectUnitsChanged);
                SetupGridColumnText();
                RefreshAndPaintGrid();
            }
        }


        public CurrencyManager DataPosition { get { return BindingContext[rulingspanDataGrid.DataSource] as CurrencyManager; } }
        #endregion

        #region Ruling Span Events
        protected void spans_RulingSpanChanged(object sender, RulingSpan.ItemChangeEventArgs e)
        {
            RefreshAndPaintGrid();
        }
        protected void rs_ProjectUnitsChanged(object sender, ProjectUnits.ChangeEventArgs e)
        {
            SetupGridColumnText();
        }


        void RefreshAndPaintGrid()
        {
            if (rs==null)
            {
                return;
            }
            DataPosition.EndCurrentEdit();
            DataPosition.Refresh();
            rulingspanDataGrid.Refresh();
            rulingspanDataGrid.Parent.Refresh();
            rulingspanPropertyGrid.Refresh();
            pic.Refresh();

            if (rs.Last!=null)
            {
                statusLabelStatusBar.Text = 
                    $"Span Count = {rs.Spans.Count}, Horizontal Tension = {rs.Last.HorizontalTension:G4} {rs.Units.Force}";
            }
            else
            {
                statusLabelStatusBar.Text= 
                    $"Span Count = {rs.Spans.Count}";
            }

        }

        protected void SetupGridColumnText()
        {
            if (rs==null)
            {
                return;
            }

            string lu=rs.Units.Length;
            string fu=rs.Units.Force;
            string wu=rs.Units.LinearWeight;

            rulingspanDataGrid.Columns[ 0].HeaderText= $"Δx ({lu})";
            rulingspanDataGrid.Columns[ 1].HeaderText= $"Δy ({lu})";
            rulingspanDataGrid.Columns[ 2].HeaderText= $"Weight ({wu})";
            rulingspanDataGrid.Columns[ 3].HeaderText= $"Average Tension ({fu})";
            rulingspanDataGrid.Columns[ 4].HeaderText= $"Total Length ({lu})";
            rulingspanDataGrid.Columns[ 5].HeaderText= $"Maximum Sag ({lu})";
            rulingspanDataGrid.Columns[ 6].HeaderText= $"Ground Clearance ({lu})";
            rulingspanDataGrid.Columns[ 7].HeaderText= $"Clearance Position ({lu})";
            rulingspanDataGrid.Columns[ 8].HeaderText= $"Catenary Constant ({lu})";
            rulingspanDataGrid.Columns[ 9].HeaderText= $"Geometric Strain ({"%"})";
            rulingspanDataGrid.Columns[10].HeaderText= $"Uplift Condition ({"y/n"})";
        }

        #endregion

        #region Grid Events
        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex>=0&&rulingspanDataGrid.Columns[e.ColumnIndex]==AverageTension)
            {
                if (e.RowIndex>=0)
                {
                    TensionForm dlg = new TensionForm
                    {
                        Units = rs.Units,
                        Catenary = rs[e.RowIndex].Clone()
                    };

                    if (dlg.ShowDialog()==DialogResult.OK)
                    {
                        rs[e.RowIndex]=dlg.Catenary;
                        
                    }
                }
            }
        }

        private void grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
            if (rs!=null&&e.RowIndex>=0)
            {
                rs.UpdateAllFromFrom(e.RowIndex);
            }
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            delSpanButton.Enabled=rulingspanDataGrid.SelectedRows.Count>0;
            moveDnButton.Enabled=rulingspanDataGrid.SelectedRows.Count>0;
            moveUpButton.Enabled=rulingspanDataGrid.SelectedRows.Count>0;
        }

        private void rulingspanPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            RefreshAndPaintGrid();
        }


        #endregion

        #region Links & Actions

        private void CatenaryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
        private void unitsDropDown_DropDownOpening(object sender, EventArgs e)
        {
            var pu=this.rs.Units;
            foreach (var item in unitsDropDown.DropDownItems)
            {
                var tsi=(item as ToolStripItem);
                if (pu.Equals(tsi.Tag))
                {
                    tsi.Image=Properties.Resources.flag_blue;
                }
                else
                {
                    tsi.Image=Properties.Resources.flag_green;
                }
            }
        }
        private void unitsDropDown_DropDownClosed(object sender, EventArgs e)
        {
            // Make all icons gray
            foreach (var item in unitsDropDown.DropDownItems)
            {
                var tsi=(item as ToolStripItem);
                tsi.Image=Properties.Resources.flag_gray;
            }
        }



        private void addSpanButton_Click(object sender, EventArgs e)
        {
            TensionForm dlg = new TensionForm
            {
                Units = rs.Units,
                Catenary = rs.NewCatenary()
            };

            if (dlg.ShowDialog()==DialogResult.OK)
            {
                rs.Spans.Add(dlg.Catenary);
                rs.SetHorizontalTensionFrom(rs.Spans.Count-1);
            }
            RefreshAndPaintGrid();
        }

        private void delSpanButton_Click(object sender, EventArgs e)
        {
            int N=rulingspanDataGrid.SelectedRows.Count;
            int[] goners=new int[N];
            for (int j=0; j<N; j++)
            {
                goners[j]=rulingspanDataGrid.SelectedRows[j].Index;
            }
            Array.Sort(goners);

            for (int k=goners.Length-1; k>=0; k--)
            {
                rs.Spans.RemoveAt(goners[k]);
            }
            rs.UpdateAllCatenary();
            rs.UpdateSpanEnds();
            
            RefreshAndPaintGrid();
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            int N=rulingspanDataGrid.SelectedRows.Count;
            int[] movers=new int[N];
            for (int j=0; j<N; j++)
            {
                movers[j]=rulingspanDataGrid.SelectedRows[j].Index;
            }
            Array.Sort(movers);


            rs.Spans.RaiseListChangedEvents=false;
            for (int i=0; i<movers.Length; i++)
            {
                if (movers[i]>0)
                {
                    Catenary cat=rs[movers[i]];
                    rs.Spans.RemoveAt(movers[i]);
                    rs.Spans.Insert(movers[i]-1, cat);
                }
            }
            
            rs.UpdateAllCatenary();
            rs.UpdateSpanEnds();
            RefreshAndPaintGrid();
            rs.Spans.RaiseListChangedEvents=true;
            rulingspanDataGrid.ClearSelection();
            for (int i=0; i<movers.Length; i++)
            {
                if (movers[i]>0)
                {
                    rulingspanDataGrid.Rows[movers[i]-1].Selected=true;
                }
            }

        }

        private void moveDnButton_Click(object sender, EventArgs e)
        {
            int N=rulingspanDataGrid.SelectedRows.Count;
            int[] movers=new int[N];
            for (int j=0; j<N; j++)
            {
                movers[j]=rulingspanDataGrid.SelectedRows[j].Index;
            }
            Array.Sort(movers);
            rs.Spans.RaiseListChangedEvents=false;
            for (int i=movers.Length-1; i>=0; i--)
            {
                if (movers[i]<rs.Spans.Count-1)
                {
                    Catenary cat=rs[movers[i]];
                    rs.Spans.RemoveAt(movers[i]);
                    rs.Spans.Insert(movers[i]+1, cat);
                }
            }
            rs.UpdateAllCatenary();
            rs.UpdateSpanEnds();
            RefreshAndPaintGrid();
            
            rs.Spans.RaiseListChangedEvents=true;
            rulingspanDataGrid.ClearSelection();
            for (int i=0; i<movers.Length; i++)
            {
                if (movers[i]<rs.Spans.Count-1)
                {
                    rulingspanDataGrid.Rows[movers[i]+1].Selected=true;
                }
            }

        }

        private void printSpansButton_Click(object sender, EventArgs e)
        {
            PrintDocument();
        }

        private void clearSpansButton_Click(object sender, EventArgs e)
        {
            rs.Spans.Clear();
            fileLabelStatusBar.Text=string.Empty;
            RefreshAndPaintGrid();
        }

        private void importSpansButton_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Open Spans",
                Filter = "Span Xml File (*.spanx)|*.spanx|All Files|*.*"
            };
            string path=Properties.Settings.Default.lastSave;
            if (File.Exists(path))
            {
                dlg.InitialDirectory=Path.GetDirectoryName(path);
                dlg.FileName=Path.GetFileName(path);
            }
            if (dlg.ShowDialog()==DialogResult.OK)
            {
                RulingSpan=RulingSpan.OpenFile(dlg.FileName);
                Properties.Settings.Default.lastSave=dlg.FileName;
                fileLabelStatusBar.Text=Path.GetFileName(dlg.FileName);
            }

        }

        private void saveSpansButton_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title = "Save Spans",
                Filter = "Span Xml File (*.spanx)|*.spanx|All Files|*.*"
            };
            string path=Properties.Settings.Default.lastSave;
            if (File.Exists(path))
            {
                dlg.InitialDirectory=Path.GetDirectoryName(path);
                dlg.FileName=Path.GetFileName(path);
            }
            if (dlg.ShowDialog()==DialogResult.OK)
            {
                RulingSpan.SaveFile(dlg.FileName);
                Properties.Settings.Default.lastSave=dlg.FileName;
                fileLabelStatusBar.Text=Path.GetFileName(dlg.FileName);
            }
        }


        #endregion

        #region Graphics & Printing
        private void pic_Resize(object sender, EventArgs e)
        {
            pic.Refresh();
        }


        private void pic_Paint(object sender, PaintEventArgs e)
        {
            if (vis==null)
            {
                this.vis=new CatenaryGraphics(rs);
            }
            vis.Render(e.Graphics, vis.GetDrawArea(pic));
        }

        void PrintDocument()
        {
            var doc = new PrintDoc
            {
                Landscape = true
            };
            if (vis==null)
            {
                vis=new CatenaryGraphics(rs);
            }
            var tbl=new CatenaryTable(rs, vis.MousePosition);

            using (Font font=new Font(SystemFonts.DialogFont.FontFamily, 16f, FontStyle.Bold))
            {
                doc.AddText("Catenary List", font);
                doc.AddHorizLine();
                doc.Add(vis);
                doc.AddLine();
                doc.Add(tbl);

                var dlg = new PrintPreviewAndSettings
                {
                    Document = doc,
                    StartPosition = FormStartPosition.CenterParent
                };
                dlg.ShowDialog();
            }
        }
        #endregion

        #region Mouse & Keys
        private void controls_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Enter:
                    e.SuppressKeyPress=true;
                    break;
                default:
                    break;
            }
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            if (vis!=null)
            {
                vis.MouseUp(pic, e.Location, e.Button);
                pic.Refresh();
            }
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (vis!=null)
            {
                vis.MouseMove(pic, e.Location, e.Button);
                if (e.Button==MouseButtons.Right)
                {
                    vis.MouseDown(pic, e.Location, e.Button);
                }
                pic.Refresh();
            }
        }

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (vis!=null)
            {
                vis.MouseDown(pic, e.Location, e.Button);
                pic.Refresh();
            }
        }

        private void pic_MouseLeave(object sender, EventArgs e)
        {
            if (vis!=null)
            {
                vis.MouseLeave(pic);
                pic.Refresh();
            }
        }

        private void pic_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (vis!=null)
            {
                RectangleF target=vis.GetDrawArea(sender as Control);
                Canvas canvas=vis.GetCanvasFor(target);
                var mouse_pt=canvas.InvMap(e.Location);
                var cat=rs.FindCatenaryFromX(mouse_pt.X);
                int index=rs.Spans.IndexOf(cat);
                if (index>=0)
                {
                    if (e.Button==MouseButtons.Left)
                    {
                        bool sel=rulingspanDataGrid.Rows[index].Selected;
                        rulingspanDataGrid.Rows[index].Selected=true;
                        TensionForm dlg = new TensionForm
                        {
                            Units = rs.Units,
                            Catenary = rs[index].Clone()
                        };

                        if (dlg.ShowDialog()==DialogResult.OK)
                        {
                            rs[index]=dlg.Catenary;
                        }
                        rulingspanDataGrid.Rows[index].Selected=sel;
                    }
                    else if (e.Button==MouseButtons.Right)
                    {
                        bool sel=rulingspanDataGrid.Rows[index].Selected;
                        rulingspanDataGrid.Rows[index].Selected=true;
                        Cursor cur=pic.Cursor;
                        pic.Cursor=Cursors.WaitCursor;
                        var rs_temp=RulingSpan.Clone();
                        this.Enabled=false;
                        rs_temp[index].SetClearancePoint(mouse_pt);
                        this.Enabled=true;
                        pic.Cursor=cur;
                        vis.ClearMousePosition();
                        RulingSpan=rs_temp;
                        rulingspanDataGrid.Rows[index].Selected=sel;
                    }
                }
            }

        }
        #endregion




    }
}
