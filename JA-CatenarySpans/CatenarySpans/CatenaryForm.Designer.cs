namespace JA.CatenarySpans
{
    partial class CatenaryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components=null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing&&(components!=null))
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CatenaryForm));
            this.horizSpitContainer = new System.Windows.Forms.SplitContainer();
            this.rulingspanDataGrid = new System.Windows.Forms.DataGridView();
            this.Dx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Dy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UnitWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AverageTension = new System.Windows.Forms.DataGridViewButtonColumn();
            this.TotalLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaximumSag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CenterY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CenterX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.catenaryConstantDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GeometricStrainPct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.etaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.catenaryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.topToolStrip = new System.Windows.Forms.ToolStrip();
            this.addSpanButton = new System.Windows.Forms.ToolStripButton();
            this.delSpanButton = new System.Windows.Forms.ToolStripButton();
            this.moveUpButton = new System.Windows.Forms.ToolStripButton();
            this.moveDnButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.printSpansButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSpansButton = new System.Windows.Forms.ToolStripButton();
            this.importSpansButton = new System.Windows.Forms.ToolStripButton();
            this.clearSpansButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.pic = new System.Windows.Forms.PictureBox();
            this.rulingspanPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.propertySplitter = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabelStatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.fileLabelStatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.unitsDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.horizSpitContainer.Panel1.SuspendLayout();
            this.horizSpitContainer.Panel2.SuspendLayout();
            this.horizSpitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rulingspanDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.catenaryBindingSource)).BeginInit();
            this.topToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.panel2.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // horizSpitContainer
            // 
            this.horizSpitContainer.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::JA.Properties.Settings.Default, "split", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.horizSpitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.horizSpitContainer.Location = new System.Drawing.Point(0, 0);
            this.horizSpitContainer.Name = "horizSpitContainer";
            this.horizSpitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // horizSpitContainer.Panel1
            // 
            this.horizSpitContainer.Panel1.Controls.Add(this.rulingspanDataGrid);
            this.horizSpitContainer.Panel1.Controls.Add(this.topToolStrip);
            // 
            // horizSpitContainer.Panel2
            // 
            this.horizSpitContainer.Panel2.Controls.Add(this.pic);
            this.horizSpitContainer.Panel2.Controls.Add(this.rulingspanPropertyGrid);
            this.horizSpitContainer.Panel2.Controls.Add(this.propertySplitter);
            this.horizSpitContainer.Panel2.Controls.Add(this.panel2);
            this.horizSpitContainer.Size = new System.Drawing.Size(940, 658);
            this.horizSpitContainer.SplitterDistance = global::JA.Properties.Settings.Default.split;
            this.horizSpitContainer.TabIndex = 0;
            // 
            // rulingspanDataGrid
            // 
            this.rulingspanDataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rulingspanDataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.rulingspanDataGrid.AutoGenerateColumns = false;
            this.rulingspanDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.rulingspanDataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.rulingspanDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.rulingspanDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.rulingspanDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Dx,
            this.Dy,
            this.UnitWeight,
            this.AverageTension,
            this.TotalLength,
            this.MaximumSag,
            this.CenterY,
            this.CenterX,
            this.catenaryConstantDataGridViewTextBoxColumn,
            this.GeometricStrainPct,
            this.etaDataGridViewTextBoxColumn});
            this.rulingspanDataGrid.DataSource = this.catenaryBindingSource;
            this.rulingspanDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rulingspanDataGrid.GridColor = System.Drawing.SystemColors.Highlight;
            this.rulingspanDataGrid.Location = new System.Drawing.Point(0, 25);
            this.rulingspanDataGrid.Name = "rulingspanDataGrid";
            this.rulingspanDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.rulingspanDataGrid.Size = new System.Drawing.Size(940, 367);
            this.rulingspanDataGrid.StandardTab = true;
            this.rulingspanDataGrid.TabIndex = 1;
            this.rulingspanDataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellContentClick);
            this.rulingspanDataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellValueChanged);
            this.rulingspanDataGrid.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            this.rulingspanDataGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.controls_KeyDown);
            // 
            // Dx
            // 
            this.Dx.DataPropertyName = "SpanX";
            dataGridViewCellStyle3.NullValue = null;
            this.Dx.DefaultCellStyle = dataGridViewCellStyle3;
            this.Dx.HeaderText = "Δx (ft)";
            this.Dx.MinimumWidth = 40;
            this.Dx.Name = "Dx";
            this.Dx.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Dy
            // 
            this.Dy.DataPropertyName = "SpanY";
            this.Dy.HeaderText = "Δy (ft)";
            this.Dy.MinimumWidth = 40;
            this.Dy.Name = "Dy";
            this.Dy.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // UnitWeight
            // 
            this.UnitWeight.DataPropertyName = "Weight";
            dataGridViewCellStyle4.Format = "0.00";
            dataGridViewCellStyle4.NullValue = null;
            this.UnitWeight.DefaultCellStyle = dataGridViewCellStyle4;
            this.UnitWeight.HeaderText = "Weight (lbf/ft)";
            this.UnitWeight.MinimumWidth = 40;
            this.UnitWeight.Name = "UnitWeight";
            this.UnitWeight.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // AverageTension
            // 
            this.AverageTension.DataPropertyName = "AverageTension";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.Format = "0.0";
            dataGridViewCellStyle5.NullValue = "Add";
            this.AverageTension.DefaultCellStyle = dataGridViewCellStyle5;
            this.AverageTension.HeaderText = "Average Tension (lbf)";
            this.AverageTension.MinimumWidth = 50;
            this.AverageTension.Name = "AverageTension";
            this.AverageTension.ReadOnly = true;
            this.AverageTension.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // TotalLength
            // 
            this.TotalLength.DataPropertyName = "TotalLength";
            dataGridViewCellStyle6.Format = "0.##";
            this.TotalLength.DefaultCellStyle = dataGridViewCellStyle6;
            this.TotalLength.HeaderText = "Total Length (ft)";
            this.TotalLength.MinimumWidth = 50;
            this.TotalLength.Name = "TotalLength";
            // 
            // MaximumSag
            // 
            this.MaximumSag.DataPropertyName = "MaximumSag";
            dataGridViewCellStyle7.Format = "0.00";
            this.MaximumSag.DefaultCellStyle = dataGridViewCellStyle7;
            this.MaximumSag.HeaderText = "Maximum Sag (ft)";
            this.MaximumSag.MinimumWidth = 50;
            this.MaximumSag.Name = "MaximumSag";
            // 
            // CenterY
            // 
            this.CenterY.DataPropertyName = "Clearance";
            dataGridViewCellStyle8.Format = "0.##";
            this.CenterY.DefaultCellStyle = dataGridViewCellStyle8;
            this.CenterY.HeaderText = "Clearance (ft)";
            this.CenterY.Name = "CenterY";
            // 
            // CenterX
            // 
            this.CenterX.DataPropertyName = "CenterX";
            dataGridViewCellStyle9.Format = "0.#";
            this.CenterX.DefaultCellStyle = dataGridViewCellStyle9;
            this.CenterX.HeaderText = "Clearance Position (ft)";
            this.CenterX.Name = "CenterX";
            this.CenterX.ReadOnly = true;
            // 
            // catenaryConstantDataGridViewTextBoxColumn
            // 
            this.catenaryConstantDataGridViewTextBoxColumn.DataPropertyName = "CatenaryConstant";
            dataGridViewCellStyle10.Format = "0.#";
            this.catenaryConstantDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle10;
            this.catenaryConstantDataGridViewTextBoxColumn.HeaderText = "Catenary Constant (ft)";
            this.catenaryConstantDataGridViewTextBoxColumn.Name = "catenaryConstantDataGridViewTextBoxColumn";
            // 
            // GeometricStrainPct
            // 
            this.GeometricStrainPct.DataPropertyName = "GeometricStrainPct";
            dataGridViewCellStyle11.Format = "0.####";
            this.GeometricStrainPct.DefaultCellStyle = dataGridViewCellStyle11;
            this.GeometricStrainPct.HeaderText = "Geometric Strain (%)";
            this.GeometricStrainPct.Name = "GeometricStrainPct";
            this.GeometricStrainPct.ReadOnly = true;
            // 
            // etaDataGridViewTextBoxColumn
            // 
            this.etaDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.etaDataGridViewTextBoxColumn.DataPropertyName = "IsUpliftCondition";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.Format = "0.000";
            dataGridViewCellStyle12.NullValue = false;
            this.etaDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle12;
            this.etaDataGridViewTextBoxColumn.HeaderText = "Uplift";
            this.etaDataGridViewTextBoxColumn.MinimumWidth = 50;
            this.etaDataGridViewTextBoxColumn.Name = "etaDataGridViewTextBoxColumn";
            this.etaDataGridViewTextBoxColumn.ReadOnly = true;
            this.etaDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.etaDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.etaDataGridViewTextBoxColumn.Width = 50;
            // 
            // catenaryBindingSource
            // 
            this.catenaryBindingSource.AllowNew = true;
            this.catenaryBindingSource.DataSource = typeof(JA.Engineering.Catenary);
            // 
            // topToolStrip
            // 
            this.topToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSpanButton,
            this.delSpanButton,
            this.moveUpButton,
            this.moveDnButton,
            this.toolStripSeparator2,
            this.printSpansButton,
            this.toolStripSeparator1,
            this.saveSpansButton,
            this.importSpansButton,
            this.clearSpansButton,
            this.toolStripLabel1});
            this.topToolStrip.Location = new System.Drawing.Point(0, 0);
            this.topToolStrip.Name = "topToolStrip";
            this.topToolStrip.Size = new System.Drawing.Size(940, 25);
            this.topToolStrip.TabIndex = 3;
            this.topToolStrip.Text = "toolStrip1";
            // 
            // addSpanButton
            // 
            this.addSpanButton.Image = global::JA.Properties.Resources.plus_white;
            this.addSpanButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addSpanButton.Name = "addSpanButton";
            this.addSpanButton.Size = new System.Drawing.Size(78, 22);
            this.addSpanButton.Text = "Add Span";
            this.addSpanButton.Click += new System.EventHandler(this.addSpanButton_Click);
            // 
            // delSpanButton
            // 
            this.delSpanButton.Image = global::JA.Properties.Resources.minus_white;
            this.delSpanButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.delSpanButton.Name = "delSpanButton";
            this.delSpanButton.Size = new System.Drawing.Size(89, 22);
            this.delSpanButton.Text = "Delete Span";
            this.delSpanButton.Click += new System.EventHandler(this.delSpanButton_Click);
            // 
            // moveUpButton
            // 
            this.moveUpButton.Image = global::JA.Properties.Resources.navigation_090_white;
            this.moveUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(75, 22);
            this.moveUpButton.Text = "Move Up";
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // moveDnButton
            // 
            this.moveDnButton.Image = global::JA.Properties.Resources.navigation_270_white;
            this.moveDnButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveDnButton.Name = "moveDnButton";
            this.moveDnButton.Size = new System.Drawing.Size(91, 22);
            this.moveDnButton.Text = "Move Down";
            this.moveDnButton.Click += new System.EventHandler(this.moveDnButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // printSpansButton
            // 
            this.printSpansButton.Image = global::JA.Properties.Resources.printer;
            this.printSpansButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printSpansButton.Name = "printSpansButton";
            this.printSpansButton.Size = new System.Drawing.Size(101, 22);
            this.printSpansButton.Text = "Print Graphics";
            this.printSpansButton.Click += new System.EventHandler(this.printSpansButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // saveSpansButton
            // 
            this.saveSpansButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.saveSpansButton.Enabled = false;
            this.saveSpansButton.Image = global::JA.Properties.Resources.disk_black;
            this.saveSpansButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveSpansButton.Name = "saveSpansButton";
            this.saveSpansButton.Size = new System.Drawing.Size(85, 22);
            this.saveSpansButton.Text = "Save Spans";
            this.saveSpansButton.Click += new System.EventHandler(this.saveSpansButton_Click);
            // 
            // importSpansButton
            // 
            this.importSpansButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.importSpansButton.Enabled = false;
            this.importSpansButton.Image = global::JA.Properties.Resources.blue_folder_open;
            this.importSpansButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importSpansButton.Name = "importSpansButton";
            this.importSpansButton.Size = new System.Drawing.Size(97, 22);
            this.importSpansButton.Text = "Import Spans";
            this.importSpansButton.Click += new System.EventHandler(this.importSpansButton_Click);
            // 
            // clearSpansButton
            // 
            this.clearSpansButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.clearSpansButton.Image = global::JA.Properties.Resources.flag_white;
            this.clearSpansButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearSpansButton.Name = "clearSpansButton";
            this.clearSpansButton.Size = new System.Drawing.Size(88, 22);
            this.clearSpansButton.Text = "Clear Spans";
            this.clearSpansButton.Click += new System.EventHandler(this.clearSpansButton_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(96, 22);
            this.toolStripLabel1.Text = "Span Definitions:";
            // 
            // pic
            // 
            this.pic.BackColor = System.Drawing.Color.White;
            this.pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic.Location = new System.Drawing.Point(0, 19);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(704, 243);
            this.pic.TabIndex = 0;
            this.pic.TabStop = false;
            this.pic.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_Paint);
            this.pic.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pic_MouseDoubleClick);
            this.pic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pic_MouseDown);
            this.pic.MouseLeave += new System.EventHandler(this.pic_MouseLeave);
            this.pic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_MouseMove);
            this.pic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_MouseUp);
            this.pic.Resize += new System.EventHandler(this.pic_Resize);
            // 
            // rulingspanPropertyGrid
            // 
            this.rulingspanPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rulingspanPropertyGrid.BackColor = System.Drawing.SystemColors.Control;
            this.rulingspanPropertyGrid.Location = new System.Drawing.Point(704, 20);
            this.rulingspanPropertyGrid.Name = "rulingspanPropertyGrid";
            this.rulingspanPropertyGrid.Size = new System.Drawing.Size(233, 239);
            this.rulingspanPropertyGrid.TabIndex = 4;
            this.rulingspanPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.rulingspanPropertyGrid_PropertyValueChanged);
            // 
            // propertySplitter
            // 
            this.propertySplitter.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertySplitter.Location = new System.Drawing.Point(704, 19);
            this.propertySplitter.Name = "propertySplitter";
            this.propertySplitter.Size = new System.Drawing.Size(236, 243);
            this.propertySplitter.TabIndex = 3;
            this.propertySplitter.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(940, 19);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(940, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Span Visualization (all dimensions in feet). Right click to measure clearances to" +
    " the catenary.";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelStatusBar,
            this.fileLabelStatusBar,
            this.unitsDropDown});
            this.statusStrip.Location = new System.Drawing.Point(0, 658);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(940, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabelStatusBar
            // 
            this.statusLabelStatusBar.Name = "statusLabelStatusBar";
            this.statusLabelStatusBar.Size = new System.Drawing.Size(215, 17);
            this.statusLabelStatusBar.Text = "Span Count = 0, Horizontal Tension = 0";
            // 
            // fileLabelStatusBar
            // 
            this.fileLabelStatusBar.Name = "fileLabelStatusBar";
            this.fileLabelStatusBar.Size = new System.Drawing.Size(627, 17);
            this.fileLabelStatusBar.Spring = true;
            this.fileLabelStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // unitsDropDown
            // 
            this.unitsDropDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.unitsDropDown.Name = "unitsDropDown";
            this.unitsDropDown.Size = new System.Drawing.Size(83, 20);
            this.unitsDropDown.Text = "Unit System";
            this.unitsDropDown.DropDownClosed += new System.EventHandler(this.unitsDropDown_DropDownClosed);
            this.unitsDropDown.DropDownOpening += new System.EventHandler(this.unitsDropDown_DropDownOpening);
            // 
            // CatenaryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 680);
            this.Controls.Add(this.horizSpitContainer);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CatenaryForm";
            this.Text = "Catenary Spans";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CatenaryForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.controls_KeyDown);
            this.horizSpitContainer.Panel1.ResumeLayout(false);
            this.horizSpitContainer.Panel1.PerformLayout();
            this.horizSpitContainer.Panel2.ResumeLayout(false);
            this.horizSpitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rulingspanDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.catenaryBindingSource)).EndInit();
            this.topToolStrip.ResumeLayout(false);
            this.topToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.panel2.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer horizSpitContainer;
        private System.Windows.Forms.DataGridView rulingspanDataGrid;
        private System.Windows.Forms.BindingSource catenaryBindingSource;
        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStrip topToolStrip;
        private System.Windows.Forms.ToolStripButton addSpanButton;
        private System.Windows.Forms.ToolStripButton delSpanButton;
        private System.Windows.Forms.ToolStripButton moveUpButton;
        private System.Windows.Forms.ToolStripButton moveDnButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton clearSpansButton;
        private System.Windows.Forms.ToolStripButton importSpansButton;
        private System.Windows.Forms.ToolStripButton saveSpansButton;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton printSpansButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelStatusBar;
        private System.Windows.Forms.ToolStripStatusLabel fileLabelStatusBar;
        private System.Windows.Forms.ToolStripDropDownButton unitsDropDown;
        private System.Windows.Forms.PropertyGrid rulingspanPropertyGrid;
        private System.Windows.Forms.Splitter propertySplitter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dx;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dy;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitWeight;
        private System.Windows.Forms.DataGridViewButtonColumn AverageTension;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaximumSag;
        private System.Windows.Forms.DataGridViewTextBoxColumn CenterY;
        private System.Windows.Forms.DataGridViewTextBoxColumn CenterX;
        private System.Windows.Forms.DataGridViewTextBoxColumn catenaryConstantDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn GeometricStrainPct;
        private System.Windows.Forms.DataGridViewCheckBoxColumn etaDataGridViewTextBoxColumn;
    }
}

