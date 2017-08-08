using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Drawing2D;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Diagnostics;
using Trionic5Tools;

namespace Trionic5Controls
{

    /*public enum ViewType : int
    {
        Hexadecimal = 0,
        Decimal,
        Easy,
        ASCII,
        Decimal3Bar,
        Easy3Bar,
        Decimal35Bar,
        Easy35Bar,
        Decimal4Bar,
        Easy4Bar
    }

    public enum ViewSize : int
    {
        NormalView = 0,
        SmallView,
        ExtraSmallView,
        TouchscreenView
    }*/

    public partial class SimpleMapViewer : IMapViewer // DevExpress.XtraEditors.XtraUserControl
    {
        
        private IECUFile m_trionic_file = new Trionic5File();
        private bool m_issixteenbit = false;
        private int m_TableWidth = 8;
        private bool m_datasourceMutated = false;
        private int m_MaxValueInTable = 0;
        private bool m_prohibitcellchange = false;
        private bool m_prohibitsplitchange = false;
        private bool m_prohibitgraphchange = false;
        private Trionic5Tools.ViewType m_viewtype = Trionic5Tools.ViewType.Hexadecimal;
        private Trionic5Tools.ViewType m_previousviewtype = Trionic5Tools.ViewType.Easy;
        private bool m_prohibit_viewchange = false;
        private bool m_trackbarBlocked = true;
        private ViewSize m_vs = ViewSize.NormalView;
        private bool m_OnlineMode = false;

        private bool m_clearData = false;

        public override bool ClearData
        {
            get { return m_clearData; }
            set { m_clearData = value; }
        }

        public override bool OnlineMode
        {
            get { return m_OnlineMode; }
            set
            {
                m_OnlineMode = value;
                if (m_OnlineMode)
                {
                    
                    ShowTable(m_TableWidth, m_issixteenbit);
                }
                else
                {
                    
                    ShowTable(m_TableWidth, m_issixteenbit);
                }
            }
        }

        private bool _isCompareViewer = false;

        public override bool IsCompareViewer
        {
            get { return _isCompareViewer; }
            set
            {
                _isCompareViewer = value;
                if (_isCompareViewer)
                {
                    gridView1.OptionsBehavior.Editable = false; // don't let the user edit a compare viewer
                    toolStripButton3.Enabled = false;
                    toolStripTextBox1.Enabled = false;
                    toolStripComboBox1.Enabled = false;
                    smoothSelectionToolStripMenuItem.Enabled = false;
                    pasteSelectedCellsToolStripMenuItem.Enabled = false;
                    exportMapToolStripMenuItem.Enabled = false;
                    simpleButton2.Enabled = false;
                    simpleButton3.Enabled = false;
                    btnSaveToRAM.Enabled = false;
                    btnReadFromRAM.Enabled = false;
                }
            }
        }

        public override void SetDataTable(DataTable dt)
        {
            gridControl1.DataSource = dt;
        }

        public override void SetViewSize(ViewSize vs)
        {
            m_vs = vs; 
            if (vs == ViewSize.SmallView)
            {
                gridView1.PaintStyleName = "UltraFlat";
                gridView1.Appearance.Row.Font = new Font("Tahoma", 8);
                this.Font = new Font("Tahoma", 8);
                gridView1.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                gridView1.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            else if (vs == ViewSize.ExtraSmallView)
            {
                gridView1.PaintStyleName = "UltraFlat";
                gridView1.Appearance.Row.Font = new Font("Tahoma", 7);
                this.Font = new Font("Tahoma", 7);
                gridView1.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                gridView1.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            else if (vs == ViewSize.TouchscreenView)
            {
                gridView1.PaintStyleName = "UltraFlat";
                gridView1.Appearance.Row.Font = new Font("Tahoma", 6);
                this.Font = new Font("Tahoma", 6);
                gridView1.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                gridView1.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
//                TryToLaunchInputScreen();
            }
        }

        private bool m_DirectSRAMWriteOnSymbolChange = false;

        public override bool DirectSRAMWriteOnSymbolChange
        {
            get { return m_DirectSRAMWriteOnSymbolChange; }
            set { m_DirectSRAMWriteOnSymbolChange = value; }
        }

        private SymbolCollection m_SymbolCollection = new SymbolCollection();

        public override SymbolCollection mapSymbolCollection
        {
            get { return m_SymbolCollection; }
            set { m_SymbolCollection = value; }
        }

        public override Trionic5Tools.ViewType Viewtype
        {
            get { return m_viewtype; }
            set
            {
                m_viewtype = value;
                m_prohibit_viewchange = true;
                toolStripComboBox3.SelectedIndex = (int)m_viewtype;
                m_prohibit_viewchange = false;
            }
        }

        public override int MaxValueInTable
        {
            get { return m_MaxValueInTable; }
            set { m_MaxValueInTable = value; }
        }

        public override bool AutoSizeColumns
        {
            set
            {
                gridView1.OptionsView.ColumnAutoWidth = value;
            }
        }

        private bool m_disablecolors = false;

        public override bool DisableColors
        {
            get
            {
                return m_disablecolors;
            }
            set
            {
                m_disablecolors = value;
                Invalidate();
            }
        }

        private string m_filename;
        //private bool m_isHexMode = true;
        private bool m_isRedWhite = false;
        private int m_textheight = 12;
        private string m_xformatstringforhex = "X4";
        private bool m_isDragging = false;
        private int _mouse_drag_x = 0;
        private int _mouse_drag_y = 0;
        private bool m_prohibitlock_change = false;

        private bool m_tableVisible = false;

        public override int SliderPosition
        {
            get { return 0; }
            set
            {
            }
        }

        public override bool TableVisible
        {
            get { return m_tableVisible; }
            set
            {
                m_tableVisible = value;
                
            }
        }

        public override int LockMode
        {
            get
            {
                return toolStripComboBox2.SelectedIndex;
            }
            set
            {
                m_prohibitlock_change = true;
                toolStripComboBox2.SelectedIndex = value;
                m_prohibitlock_change = false;
            }

        }

        private double _max_y_axis_value = 0;

        public override double Max_y_axis_value
        {
            get
            {
                
                return _max_y_axis_value;
            }
            set { 
                _max_y_axis_value = value;
            }
        }

        private bool m_isRAMViewer = false;

        public override bool IsRAMViewer
        {
            get { return m_isRAMViewer; }
            set { m_isRAMViewer = value; }
        }

        private bool m_isUpsideDown = false;

        public override bool IsUpsideDown
        {
            get { return m_isUpsideDown; }
            set { m_isUpsideDown = value; }
        }

        private double correction_factor = 1;

        public override double Correction_factor
        {
            get { return correction_factor; }
            set { correction_factor = value; }
        }
        private double correction_offset = 0;

        public override double Correction_offset
        {
            get { return correction_offset; }
            set { correction_offset = value; }
        }

        public override bool GraphVisible
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

        public override bool IsRedWhite
        {
            get { return m_isRedWhite; }
            set { m_isRedWhite = value; }
        }

        /*public bool IsHexMode
        {
            get { return m_isHexMode; }
            set { m_isHexMode = value; }
        }*/

        public override string Filename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }


        public override bool DatasourceMutated
        {
            get { return m_datasourceMutated; }
            set { m_datasourceMutated = value; }
        }

        private bool m_UseNewCompare = false;

        public override bool UseNewCompare
        {
            get { return m_UseNewCompare; }
            set { m_UseNewCompare = value; }
        }

        private bool m_SaveChanges = false;

        public override bool SaveChanges
        {
            get { return m_SaveChanges; }
            set { m_SaveChanges = value; }
        }
        private byte[] m_map_content;

        public override byte[] Map_content
        {
            get { return m_map_content; }
            set
            {
                m_map_content = value;
            }
        }

        public override void InitEditValues()
        {
            m_values_changed_highlight_ecu = new byte[m_map_content.Length];
            m_values_changed_highlight_ecu.Initialize();
            m_values_changed_highlight_user = new byte[m_map_content.Length];
            m_values_changed_highlight_user.Initialize();
        }

        private byte[] m_map_compare_content;

        public override byte[] Map_compare_content
        {
            get { return m_map_compare_content; }
            set { m_map_compare_content = value; }
        }

        private byte[] m_map_original_content;

        public override byte[] Map_original_content
        {
            get { return m_map_original_content; }
            set { m_map_original_content = value; }
        }

        private byte[] m_values_changed_highlight_user;

        public override byte[] Values_changed_highlight_user
        {
            get { return m_values_changed_highlight_user; }
            set { m_values_changed_highlight_user = value; }
        }
        private byte[] m_values_changed_highlight_ecu;

        public override byte[] Values_changed_highlight_ecu
        {
            get { return m_values_changed_highlight_ecu; }
            set { m_values_changed_highlight_ecu = value; }
        }

        private Int32 m_map_address = 0;

        public override Int32 Map_address
        {
            get { return m_map_address; }
            set { m_map_address = value; }
        }


        private Int32 m_map_sramaddress = 0;

        public override Int32 Map_sramaddress
        {
            get { return m_map_sramaddress; }
            set { m_map_sramaddress = value; }
        }
        private Int32 m_map_length = 0;

        public override Int32 Map_length
        {
            get { return m_map_length; }
            set { m_map_length = value; }
        }
        private string m_map_name = string.Empty;

        public override string Map_name
        {
            get { return m_map_name; }
            set
            {
                m_map_name = value;
                this.Text = "Table details [" + m_map_name + "]";
            }
        }

        private string m_map_descr = string.Empty;

        public override string Map_descr
        {
            get { return m_map_descr; }
            set
            {
                m_map_descr = value;
            }
        }

        private XDFCategories m_map_cat = XDFCategories.Undocumented;

        public override XDFCategories Map_cat
        {
            get { return m_map_cat; }
            set
            {
                m_map_cat = value;
            }
        }

        private string m_x_axis_name = string.Empty;

        public override string X_axis_name
        {
            get { return m_x_axis_name; }
            set { m_x_axis_name = value; }
        }
        private string m_y_axis_name = string.Empty;

        public override string Y_axis_name
        {
            get { return m_y_axis_name; }
            set { m_y_axis_name = value; }
        }
        private string m_z_axis_name = string.Empty;

        public override string Z_axis_name
        {
            get { return m_z_axis_name; }
            set { m_z_axis_name = value; }
        }

        private int[] x_axisvalues;

        public override int[] X_axisvalues
        {
            get { return x_axisvalues; }
            set { x_axisvalues = value; }
        }
        private int[] y_axisvalues;

        public override int[] Y_axisvalues
        {
            get { return y_axisvalues; }
            set { y_axisvalues = value; }
        }

        

        public override event IMapViewer.ViewerClose onClose;
        public override event IMapViewer.AxisEditorRequested onAxisEditorRequested;
        public override event IMapViewer.ReadDataFromSRAM onReadFromSRAM;
        public override event IMapViewer.WriteDataToSRAM onWriteToSRAM;
        public override event IMapViewer.ViewTypeChanged onViewTypeChanged;
        public override event IMapViewer.GraphSelectionChanged onGraphSelectionChanged;
        public override event IMapViewer.SurfaceGraphViewChanged onSurfaceGraphViewChanged;
        public override event IMapViewer.SurfaceGraphViewChangedEx onSurfaceGraphViewChangedEx;
        public override event IMapViewer.NotifySaveSymbol onSymbolSave;
        public override event IMapViewer.SplitterMoved onSplitterMoved;
        public override event IMapViewer.SelectionChanged onSelectionChanged;
        public override event IMapViewer.NotifyAxisLock onAxisLock;
        public override event IMapViewer.NotifySliderMove onSliderMove;
        public override event IMapViewer.CellLocked onCellLocked;


        private bool m_autoUpdateChecksum = false;
        public override bool AutoUpdateChecksum
        {
            get { return m_autoUpdateChecksum; }
            set { m_autoUpdateChecksum = value; }
        }
        private void AddLogItem(string item)
        {
           
        }


        public SimpleMapViewer()
        {
            try
            {
                InitializeComponent();
                toolStripComboBox1.SelectedIndex = 0;
                toolStripComboBox2.SelectedIndex = 0;
                string[] datamembers = new string[1];
                datamembers.SetValue("Y", 0);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }


        public override bool SaveData()
        {
            bool retval = false;
            if (simpleButton2.Enabled)
            {
                simpleButton2_Click(this, EventArgs.Empty);
                retval = true;
            }
            return retval;
        }

        private bool MapIsScalableFor3Bar(string symbolname)
        {
            bool retval = false;
            if (symbolname.StartsWith("Tryck_mat")) retval = true; // boost request map
            else if (symbolname.StartsWith("Regl_tryck")) retval = true; // 1st and second gear limiters
            else if (symbolname.StartsWith("Tryck_vakt_tab")) retval = true; // maximum boost
            else if (symbolname.StartsWith("Idle_tryck")) retval = true; // idle boost
            else if (symbolname.StartsWith("Limp_tryck_konst")) retval = true; // Maximum limp home boost
            else if (symbolname.StartsWith("Knock_press_tab")) retval = true; // Maximum limp home boost
            else if (symbolname.StartsWith("Knock_press")) retval = true; // Maximum limp home boost
            else if (symbolname.StartsWith("Turbo_knock_tab")) retval = true; // Maximum limp home boost
            else if (symbolname.StartsWith("Open_loop_knock")) retval = true; // Maximum limp home boost
            else if (symbolname.StartsWith("Open_loop")) retval = true; // Maximum limp home boost
            else if (symbolname.StartsWith("Sond_heat_tab")) retval = true; // Maximum limp home boost
            else if (symbolname.StartsWith("Reg_last!")) retval = true;
            else if (symbolname.StartsWith("Idle_st_last!")) retval = true;
            //else if (symbolname.StartsWith("Last_temp_st!")) retval = true;
            else if (symbolname.StartsWith("Lam_minlast!")) retval = true;
            else if (symbolname.StartsWith("Lam_laststeg!")) retval = true;
            else if (symbolname.StartsWith("Grund_last!")) retval = true;
            else if (symbolname.StartsWith("Max_ratio_aut!")) retval = true;
            else if (symbolname.StartsWith("Diag_speed_load!")) retval = true;
            else if (symbolname.StartsWith("Knock_press_lim")) retval = true;
            else if (symbolname.StartsWith("Turbo_knock_press")) retval = true;
            else if (symbolname.StartsWith("Kadapt_load_high!")) retval = true;
            else if (symbolname.StartsWith("Kadapt_load_low!")) retval = true;
            else if (symbolname.StartsWith("Iv_min_load!")) retval = true;
            else if (symbolname.StartsWith("Shift_load!")) retval = true;
            else if (symbolname.StartsWith("Shift_up_load_hyst!")) retval = true;


            return retval;
        }

        public override void ShowTable(int tablewidth, bool issixteenbits)
        {
            btnReadFromRAM.Enabled = true;
            m_MaxValueInTable = 0;
            //if (m_isHexMode)
            if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
            {
                int lenvals = m_map_length;
                if (issixteenbits) lenvals /= 2;
            }
            else
            {
                int lenvals = m_map_length;
                if (issixteenbits) lenvals /= 2;
            }


            m_TableWidth = tablewidth;
            m_issixteenbit = issixteenbits;
            DataTable dt = new DataTable();
            if (m_map_length != 0 && m_map_name != string.Empty)
            {

                if (tablewidth > 0)
                {
                    int numberrows = (int)(m_map_length / tablewidth);
                    if (issixteenbits) numberrows /= 2;
                    int map_offset = 0;
                    /* if (numberrows > 0)
                     {*/
                    // aantal kolommen = 8

                    dt.Columns.Clear();
                    for (int c = 0; c < tablewidth; c++)
                    {
                        dt.Columns.Add(c.ToString());
                    }

                    if (issixteenbits)
                    {

                        for (int i = 0; i < numberrows; i++)
                        {
                            //ListViewItem lvi = new ListViewItem();
                            //lvi.UseItemStyleForSubItems = false;
                            object[] objarr = new object[tablewidth];
                            int b;
                            for (int j = 0; j < tablewidth; j++)
                            {
                                b = (byte)m_map_content.GetValue(map_offset++);
                                b *= 256;
                                b += (byte)m_map_content.GetValue(map_offset++);
                                if (b > 32000)
                                {
                                   // b ^= 0xFFFF;
                                    b = 65536 - b;
                                    b = -b;
                                }
                                if (/*m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || */m_map_name == "FeedbackvsTargetAFR" || m_map_name == "IdleFeedbackvsTargetAFR")
                                {
                                    if (b > 200)
                                    {
                                        b = 256-b;
                                        b = -b;
                                    }

                                }
                                if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                {
                                    // correct with 1.2
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 120;
                                        b /= 100;
                                    }
                                }
                                if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                {
                                    // correct with 1.4
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 140;
                                        b /= 100;
                                    }
                                }
                                if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                {
                                    // correct with 1.6
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 160;
                                        b /= 100;
                                    }
                                }
                                if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                {
                                    // correct with 1.6
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 200;
                                        b /= 100;
                                    }
                                }
                                if (b > m_MaxValueInTable) m_MaxValueInTable = b;
                                // TEST
                                //b = (int)(correction_factor * (double)b);
                                //b = (int)(correction_offset + (double)b);
                                // TEST
                                //if (m_isHexMode)
                                if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                {
                                    objarr.SetValue(b.ToString("X4"), j);
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.ASCII)
                                {
                                    //objarr.SetValue(b.ToString("X4"), j);
                                    // show as ascii characters
                                    try
                                    {
                                        objarr.SetValue(Convert.ToChar(b), j);
                                    }
                                    catch (Exception E)
                                    {
                                        Console.WriteLine("Failed to convert to ascii: " +  E.Message);
                                        objarr.SetValue(Convert.ToChar(0x20), j);
                                    }
                                }
                                else
                                {
                                    objarr.SetValue(b.ToString(), j);
                                }
                                //lvi.SubItems.Add(b.ToString("X4"));
                                //lvi.SubItems[j + 1].BackColor = Color.FromArgb((int)(b / 256), 255 - (int)(b / 256), 0);
                            }
                            //listView2.Items.Add(lvi);
                            if (m_isUpsideDown)
                            {
                                System.Data.DataRow r = dt.NewRow();
                                r.ItemArray = objarr;
                                dt.Rows.InsertAt(r, 0);
                            }
                            else
                            {
                                dt.Rows.Add(objarr);
                            }
                        }
                        // en dan het restant nog in een nieuwe rij zetten
                        if (map_offset < m_map_length)
                        {
                            object[] objarr = new object[tablewidth];
                            int b;
                            int sicnt = 0;
                            for (int v = map_offset; v < m_map_length - 1; v++)
                            {
                                //b = (byte)m_map_content.GetValue(map_offset++);
                                if (map_offset <= m_map_content.Length-1)
                                {
                                    b = (byte)m_map_content.GetValue(map_offset++);
                                    b *= 256;
                                    b += (byte)m_map_content.GetValue(map_offset++);
                                    if (b > 32000)
                                    {
                                        //b ^= 0xFFFF;
                                        b = 65536 - b;

                                        b = -b;
                                    }
                                    if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                    {
                                        // correct with 1.2
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            b *= 120;
                                            b /= 100;
                                        }
                                    }
                                    if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                    {
                                        // correct with 1.4
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            b *= 140;
                                            b /= 100;
                                        }
                                    }
                                    if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                    {
                                        // correct with 1.6
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            b *= 160;
                                            b /= 100;
                                        }
                                    }
                                    if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                    {
                                        // correct with 1.6
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            b *= 200;
                                            b /= 100;
                                        }
                                    }
                                    if (b > m_MaxValueInTable) m_MaxValueInTable = b;
                                    // TEST
                                    //b = (int)(correction_factor * (double)b);
                                    //b = (int)(correction_offset + (double)b);

                                    // TEST

//                                    if (m_isHexMode)
                                    if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                    {
                                        objarr.SetValue(b.ToString("X4"), sicnt);
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.ASCII)
                                    {
                                        //objarr.SetValue(b.ToString("X4"), j);
                                        // show as ascii characters
                                        objarr.SetValue(Convert.ToChar(b), sicnt);
                                    }
                                    else
                                    {
                                        objarr.SetValue(b.ToString(), sicnt);
                                    }
                                }
                                sicnt++;
                                //lvi.SubItems[lvi.SubItems.Count - 1].BackColor = Color.FromArgb((int)(b / 256), 255 - (int)(b / 256), 0);
                            }
                            /*for (int t = 0; t < (tablewidth - 1) - sicnt; t++)
                            {
                                lvi.SubItems.Add("");
                            }*/
                            //listView2.Items.Add(lvi);
                            if (m_isUpsideDown)
                            {
                                System.Data.DataRow r = dt.NewRow();
                                r.ItemArray = objarr;
                                dt.Rows.InsertAt(r, 0);
                            }
                            else
                            {

                                dt.Rows.Add(objarr);
                            }
                        }

                    }
                    else
                    {

                        for (int i = 0; i < numberrows; i++)
                        {
                            object[] objarr = new object[tablewidth];
                            int b;
                            for (int j = 0; j < (tablewidth); j++)
                            {
                                b = (byte)m_map_content.GetValue(map_offset++);
                                if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                {
                                    // correct with 1.2
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 120;
                                        b /= 100;
                                    }
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                {
                                    // correct with 1.4
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 140;
                                        b /= 100;
                                    }
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                {
                                    // correct with 1.6
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 160;
                                        b /= 100;
                                    }
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                {
                                    // correct with 1.6
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 200;
                                        b /= 100;
                                    }
                                }
                                if (b > m_MaxValueInTable) m_MaxValueInTable = b;
                                // TEST
                                //b = (byte)(correction_factor * (double)b);
                                //b = (byte)(correction_offset + (double)b);

                                // TEST

                                //if (m_isHexMode)
                                if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                {
                                    objarr.SetValue(b.ToString("X2"), j);
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.ASCII)
                                {
                                    //objarr.SetValue(b.ToString("X4"), j);
                                    // show as ascii characters
                                    objarr.SetValue(Convert.ToChar(b), j);
                                }
                                else
                                {
                                    objarr.SetValue(b.ToString(), j);
                                }
                            }
                            if (m_isUpsideDown)
                            {
                                System.Data.DataRow r = dt.NewRow();
                                r.ItemArray = objarr;
                                dt.Rows.InsertAt(r, 0);
                            }
                            else
                            {

                                dt.Rows.Add(objarr);
                            }
                        }
                        // en dan het restant nog in een nieuwe rij zetten
                        if (map_offset < m_map_length)
                        {
                            object[] objarr = new object[tablewidth];
                            byte b;
                            int sicnt = 0;
                            for (int v = map_offset; v < m_map_length /*- 1*/; v++)
                            {
                                b = (byte)m_map_content.GetValue(map_offset++);
                                if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                {
                                    // correct with 1.2
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 120;
                                        b /= 100;
                                    }
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                {
                                    // correct with 1.4
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 140;
                                        b /= 100;
                                    }
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                {
                                    // correct with 1.6
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 160;
                                        b /= 100;
                                    }
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                {
                                    // correct with 1.6
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        b *= 200;
                                        b /= 100;
                                    }
                                }
                                if (b > m_MaxValueInTable) m_MaxValueInTable = b;
                                // TEST
                                //b = (byte)(correction_factor * (double)b);
                                //b = (byte)(correction_offset + (double)b);

                                // TEST

                                //if (m_isHexMode)
                                if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                {
                                    objarr.SetValue(b.ToString("X2"), sicnt);
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.ASCII)
                                {
                                    //objarr.SetValue(b.ToString("X4"), j);
                                    // show as ascii characters
                                    objarr.SetValue(Convert.ToChar(b), sicnt);
                                }
                                else
                                {
                                    objarr.SetValue(b.ToString(), sicnt);
                                }
                                sicnt++;
                            }
                            if (m_isUpsideDown)
                            {
                                System.Data.DataRow r = dt.NewRow();
                                r.ItemArray = objarr;
                                dt.Rows.InsertAt(r, 0);
                            }
                            else
                            {

                                dt.Rows.Add(objarr);
                            }
                        }
                    }
                }

                gridControl1.DataSource = dt;

                if (!gridView1.OptionsView.ColumnAutoWidth)
                {
                    for (int c = 0; c< gridView1.Columns.Count; c ++ )
                    {
                        gridView1.Columns[c].Width = 40;
                    }
                }


                // set axis
                // scale to 3 bar?
                int indicatorwidth = -1;
                for (int i = 0; i < y_axisvalues.Length; i++)
                {
                    string yval = Convert.ToInt32(y_axisvalues.GetValue(i)).ToString();
                    if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                    {
                        yval = Convert.ToInt32(y_axisvalues.GetValue(i)).ToString("X4");
                    }
                    if (m_y_axis_name == "MAP")
                    {
                        if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                        {
                            int tempval = Convert.ToInt32(y_axisvalues.GetValue(i));
                            tempval *= 120;
                            tempval /= 100;
                            //                                y_axisvalues.SetValue(tempval, i);
                            yval = tempval.ToString("X4");
                        }
                        else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                        {
                            int tempval = Convert.ToInt32(y_axisvalues.GetValue(i));
                            tempval *= 140;
                            tempval /= 100;
                            //                                y_axisvalues.SetValue(tempval, i);
                            yval = tempval.ToString("X4");
                        }
                        else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar || m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                        {
                            int tempval = Convert.ToInt32(y_axisvalues.GetValue(i));
                            tempval *= 160;
                            tempval /= 100;
                            //                                y_axisvalues.SetValue(tempval, i);
                            yval = tempval.ToString("X4");
                        }
                        else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar || m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                        {
                            int tempval = Convert.ToInt32(y_axisvalues.GetValue(i));
                            tempval *= 200;
                            tempval /= 100;
                            //                                y_axisvalues.SetValue(tempval, i);
                            yval = tempval.ToString("X4");
                        }
                    }

                    Graphics g = gridControl1.CreateGraphics();
                    SizeF size = g.MeasureString(yval, this.Font);
                    if(size.Width > indicatorwidth) indicatorwidth = (int)size.Width;
                    m_textheight = (int)size.Height;
                    g.Dispose();

                }
                if (indicatorwidth > 0)
                {
                    gridView1.IndicatorWidth = indicatorwidth + 6; // keep margin
                }

                int maxxval = 0;
                for (int i = 0; i < x_axisvalues.Length; i++)
                {
                    int xval = Convert.ToInt32(x_axisvalues.GetValue(i));

                    if (m_x_axis_name == "MAP" || m_x_axis_name == "Pressure error (bar)")
                    {
                        if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                        {
                            int tempval = Convert.ToInt32(x_axisvalues.GetValue(i));
                            tempval *= 120;
                            tempval /= 100;
                            //x_axisvalues.SetValue(tempval, i);
                            xval = tempval;
                        }
                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                        {
                            int tempval = Convert.ToInt32(x_axisvalues.GetValue(i));
                            tempval *= 140;
                            tempval /= 100;
                            //x_axisvalues.SetValue(tempval, i);
                            xval = tempval;
                        }
                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                        {
                            int tempval = Convert.ToInt32(x_axisvalues.GetValue(i));
                            tempval *= 160;
                            tempval /= 100;
                            //x_axisvalues.SetValue(tempval, i);
                            xval = tempval;
                        }
                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                        {
                            int tempval = Convert.ToInt32(x_axisvalues.GetValue(i));
                            tempval *= 200;
                            tempval /= 100;
                            //x_axisvalues.SetValue(tempval, i);
                            xval = tempval;
                        }
                    }

                    if (xval > maxxval) maxxval = xval;
                }
                if (maxxval <= 255) m_xformatstringforhex = "X2";
            }
            m_trackbarBlocked = false;

        }

    
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (!m_isRAMViewer)
            {
                if (m_datasourceMutated)
                {
                    DialogResult dr = MessageBox.Show("Data was mutated, do you want to save these changes in you binary?", "Warning", MessageBoxButtons.YesNoCancel);
                    if (dr == DialogResult.Yes)
                    {
                        m_SaveChanges = true;
                        CastSaveEvent();
                        CastCloseEvent();

                    }
                    else if (dr == DialogResult.No)
                    {
                        m_SaveChanges = false;
                        CastCloseEvent();
                    }
                    else
                    {
                        // cancel
                        // do nothing
                    }
                }
                else
                {
                    m_SaveChanges = false;
                    CastCloseEvent();
                }
            }
            else
            {
                m_SaveChanges = false;
                CastCloseEvent();
            }
        }

        int[] afr_counter;

        public override int[] Afr_counter
        {
            get { return afr_counter; }
            set { afr_counter = value; }
        }


        int _boostadaptrpmfrom = 0;

        public override int BoostAdaptionRpmFrom
        {
            get
            {
                return _boostadaptrpmfrom;
            }
            set
            {
                _boostadaptrpmfrom = value;
            }
        }

        int _boostadaptrpmupto = 0;

        public override int BoostAdaptionRpmUpto
        {
            get
            {
                return _boostadaptrpmupto;
            }
            set
            {
                _boostadaptrpmupto = value;
            }
        }

        int _knockadaptrpmfrom = 0;

        public override int KnockAdaptionRpmFrom
        {
            get
            {
                return _knockadaptrpmfrom;
            }
            set
            {
                _knockadaptrpmfrom = value;
            }
        }

        int _knockadaptrpmupto = 0;

        public override int KnockAdaptionRpmUpto
        {
            get
            {
                return _knockadaptrpmupto;
            }
            set
            {
                _knockadaptrpmupto = value;
            }
        }

        int _knockadaptloadfrom = 0;

        public override int KnockAdaptionLoadFrom
        {
            get
            {
                return _knockadaptloadfrom;
            }
            set
            {
                _knockadaptloadfrom = value;
            }
        }

        int _knockadaptloadupto = 0;

        public override int KnockAdaptionLoadUpto
        {
            get
            {
                return _knockadaptloadupto;
            }
            set
            {
                _knockadaptloadupto = value;
            }
        }

        byte[] open_loop_knock;

        public override byte[] Open_loop_knock
        {
            get { return open_loop_knock; }
            set { open_loop_knock = value; }
        }
        byte[] open_loop;

        public override byte[] Open_loop
        {
            get { return open_loop; }
            set { open_loop = value; }
        }

        int[] idle_afr_lock_map;

        public override int[] IdleAFR_lock_map
        {
            get
            {
                return idle_afr_lock_map;
            }
            set
            {
                idle_afr_lock_map = value;
            }
        }


        int[] afr_lock_map;

        public override int[] AFR_lock_map
        {
            get
            {
                return afr_lock_map;
            }
            set
            {
                afr_lock_map = value;
            }
        }

        int[] ignition_lock_map;

        public override int[] Ignition_lock_map
        {
            get
            {
                return ignition_lock_map;
            }
            set
            {
                ignition_lock_map = value;
            }
        }

        int[] turbo_press_tab;

        public override int[] Turbo_press_tab
        {
            get
            {
                return turbo_press_tab;
            }
            set
            {
                turbo_press_tab = value;
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            //  if mapname is Insp_mat or Fuel_knock_map indicate open/closed loop operation
            // the map indicating where the ecu switches to open loop should be loaded into... 
            // for Fuel_knock_map this is Open_loop_knock!
            // for Insp_mat this is Open_loop!
            try
            {
                if (e.CellValue != null)
                {
                    if (e.CellValue != DBNull.Value)
                    {
                        int b = 0;
                        int cellvalue = 0;
                        //if (m_isHexMode)
                        if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                        {
                            b = Convert.ToInt32(e.CellValue.ToString(), 16);
                            cellvalue = b;
                        }
                        else
                        {
                            b = Convert.ToInt32(e.CellValue.ToString());
                            cellvalue = b;
                        }
                        b *= 255;
                        if (m_MaxValueInTable != 0)
                        {
                            b /= m_MaxValueInTable;
                        }
                        int red = 128;
                        int green = 128;
                        int blue = 128;
                        Color c = Color.White;
                        if (m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR" || m_map_name == "IdleTargetAFR" || m_map_name == "IdleFeedbackAFR" || m_map_name == "IdleFeedbackvsTargetAFR" || m_OnlineMode)
                        {
                            b /= 2;
                            red = b;
                            if (red < 0) red = 0;
                            if (red > 255) red = 255;
                            if (b > 255) b = 255;
                            green = 255-red;
                            blue = 255 - red;
                            c = Color.FromArgb(red, green, blue);
                           // if (b == 0) c = Color.Transparent;

                        }
                        else if (!m_isRedWhite)
                        {
                            red = b;
                            if (red < 0) red = 0;
                            if (red > 255) red = 255;
                            if (b > 255) b = 255;
                            blue = 0;
                            green = 255 - red;
                            c = Color.FromArgb(red, green, blue);
                        }
                        else
                        {
                            if (b < 0) b = -b;
                            if (b > 255) b = 255;
                            c = Color.FromArgb(b, Color.Red);
                        }
                        if (!m_disablecolors)
                        {
                            SolidBrush sb = new SolidBrush(c);
                            e.Graphics.FillRectangle(sb, e.Bounds);
                        }
                        // draw indicator for changed by user
                        try
                        {
                            if (m_values_changed_highlight_user != null)
                            {
                                byte bchangeduser = Convert.ToByte(m_values_changed_highlight_user.GetValue((e.RowHandle * m_TableWidth) + e.Column.AbsoluteIndex));
                                if (bchangeduser > 0)
                                {
                                    // draw triangle
                                    Point[] pnts = new Point[4];
                                    pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 0);
                                    pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width - (e.Bounds.Height / 2), e.Bounds.Y), 1);
                                    pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y + (e.Bounds.Height / 2)), 2);
                                    pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 3);
                                    e.Graphics.FillPolygon(Brushes.Yellow, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                                }
                                
                            }
                            if(m_values_changed_highlight_ecu != null)
                            {
                                byte bchangedecu = Convert.ToByte(m_values_changed_highlight_ecu.GetValue((e.RowHandle * m_TableWidth) + e.Column.AbsoluteIndex));
                                if (bchangedecu > 0)
                                {
                                    // draw triangle
                                    Point[] pnts = new Point[4];
                                    pnts.SetValue(new Point(e.Bounds.X, e.Bounds.Y), 0);
                                    pnts.SetValue(new Point(e.Bounds.X + (e.Bounds.Height / 2), e.Bounds.Y), 1);
                                    pnts.SetValue(new Point(e.Bounds.X, e.Bounds.Y + (e.Bounds.Height / 2)), 2);
                                    pnts.SetValue(new Point(e.Bounds.X, e.Bounds.Y), 3);
                                    e.Graphics.FillPolygon(Brushes.Orange, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                                }
                            }
                        }
                        catch /*(Exception changedUserE)*/
                        {
                            //Console.WriteLine("Failed to draw changed by user indicator: " + changedUserE.Message);
                        }

                        if (m_viewtype == Trionic5Tools.ViewType.Easy || m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                        {
                            float dispvalue = 0;
                            dispvalue = (float)cellvalue;
                            if (correction_offset != 0 || correction_factor != 1)
                            {
                                dispvalue = (float)((float)cellvalue * (float)correction_factor) + (float)correction_offset;
                                if (m_viewtype != Trionic5Tools.ViewType.Hexadecimal)
                                //if (!m_isHexMode)
                                {
                                    if (m_map_name.StartsWith("Ign_map_0!") || m_map_name.StartsWith("Ign_map_4!"))
                                    {
                                        e.DisplayText = dispvalue.ToString("F1") + "\u00b0";
                                        /*if (dispvalue < 0)
                                        {
                                            Console.WriteLine("Negative value:  " + cellvalue.ToString());

                                        }*/
                                    }
                                    else if (m_map_name.StartsWith("Reg_kon_mat"))
                                    {
                                        e.DisplayText = dispvalue.ToString("F0") + @"%";
                                    }
                                    else if (m_map_name.StartsWith("FeedbackAFR") || m_map_name.StartsWith("FeedbackvsTargetAFR") || m_map_name.StartsWith("IdleFeedbackAFR") || m_map_name.StartsWith("IdleFeedbackvsTargetAFR"))
                                    {
                                        if (dispvalue == 0)
                                        {
                                            e.DisplayText = "";
                                        }
                                        else
                                        {
                                            e.DisplayText = dispvalue.ToString("F2");
                                        }
                                    }
                                    else
                                    {
                                        e.DisplayText = dispvalue.ToString("F2");
                                    }
                                }
                                else
                                {
                                    //e.DisplayText = dispvalue.ToString();
                                }
                            }
                            else if (m_map_name.StartsWith("Reg_kon_mat"))
                            {
                                e.DisplayText = dispvalue.ToString("F0") + @"%";
                            }
                        }

                        //  if mapname is Insp_mat or Fuel_knock_map indicate open/closed loop operation
                        // the map indicating where the ecu switches to open loop should be loaded into... 
                        // for Fuel_knock_map this is Open_loop_knock!
                        // for Insp_mat this is Open_loop!
                        //e.Column.AbsoluteIndex
                        bool lock_drawn = false;
                        if (m_map_name == "Ign_map_0!" || m_map_name == "Knock_count_map")
                        {
                            try
                            {
                                if (turbo_press_tab != null)
                                {
                                    int mapvalue = x_axisvalues[e.Column.AbsoluteIndex];
                                    int rpmvalue = y_axisvalues[e.RowHandle];
                                    if (turbo_press_tab.Length > 0)
                                    {
                                        int mapopenloop = (int)turbo_press_tab[(open_loop.Length - e.RowHandle) - 1];
                                        if (mapopenloop > mapvalue)
                                        {
                                            int pos = (e.Bounds.Width) - 12;
                                            int ypos = (e.Bounds.Height / 2) - 5;
                                            if (pos >= 0)
                                            {
                                                System.Drawing.Image cflag = (Image)global::Trionic5Controls.Properties.Resources.db_lock16_h;
                                                e.Graphics.DrawImage(cflag, e.Bounds.X + pos, e.Bounds.Y + ypos, 10, 10);
                                                lock_drawn = true;
                                            }
                                        }
                                    }
                                }
                                if (ignition_lock_map != null)
                                {
                                    int mapvalue = x_axisvalues[e.Column.AbsoluteIndex];
                                    int rpmvalue = y_axisvalues[e.RowHandle];
                                    if (ignition_lock_map.Length > 0)
                                    {
                                        int locked = (int)ignition_lock_map[(e.RowHandle * m_TableWidth) + e.Column.AbsoluteIndex];

                                        if (locked > 0 && !lock_drawn)
                                        {
                                            // draw lock icon
                                            int pos = (e.Bounds.Width) - 12;
                                            int ypos = (e.Bounds.Height / 2) - 5;
                                            if (pos >= 0)
                                            {
                                                System.Drawing.Image cflag = (Image)global::Trionic5Controls.Properties.Resources.db_lock16_h;
                                                e.Graphics.DrawImage(cflag, e.Bounds.X + pos, e.Bounds.Y + ypos, 10, 10);
                                                lock_drawn = true;
                                            }

                                        }
                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine("Failed to mark cell as locked: " + E.Message);
                            }
                        }
                        if (m_map_name == "Insp_mat!" || m_map_name == "Inj_map_0!" || m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR")
                        {
                            // show locked indicators from afr_lock_map
                            try
                            {
                                if (afr_lock_map != null)
                                {
                                    int mapvalue = x_axisvalues[e.Column.AbsoluteIndex];
                                    int rpmvalue = y_axisvalues[e.RowHandle];
                                    if (afr_lock_map.Length > 0)
                                    {
                                        int locked = (int)afr_lock_map[(e.RowHandle * m_TableWidth) + e.Column.AbsoluteIndex];

                                        if (locked > 0)
                                        {
                                            // draw lock icon
                                            int pos = (e.Bounds.Width) - 12;
                                            int ypos = (e.Bounds.Height / 2) - 5;
                                            if (pos >= 0)
                                            {
                                                System.Drawing.Image cflag = (Image)global::Trionic5Controls.Properties.Resources.db_lock16_h;
                                                e.Graphics.DrawImage(cflag, e.Bounds.X + pos, e.Bounds.Y + ypos, 10, 10);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine("Failed to mark cell as locked: " + E.Message);
                            }
                        }
                        if (m_map_name == "Idle_fuel_korr!" || m_map_name == "IdleTargetAFR" || m_map_name == "IdleFeedbackAFR" || m_map_name == "IdleFeedbackvsTargetAFR")
                        {
                            // show locked indicators from Idleafr_lock_map
                            try
                            {
                                if (idle_afr_lock_map != null)
                                {
                                    int mapvalue = x_axisvalues[e.Column.AbsoluteIndex];
                                    int rpmvalue = y_axisvalues[e.RowHandle];
                                    if (idle_afr_lock_map.Length > 0)
                                    {
                                        int locked = (int)idle_afr_lock_map[(e.RowHandle * m_TableWidth) + e.Column.AbsoluteIndex];

                                        if (locked > 0)
                                        {
                                            // draw lock icon
                                            int pos = (e.Bounds.Width) - 12;
                                            int ypos = (e.Bounds.Height / 2) - 5;
                                            if (pos >= 0)
                                            {
                                                System.Drawing.Image cflag = (Image)global::Trionic5Controls.Properties.Resources.db_lock16_h;
                                                e.Graphics.DrawImage(cflag, e.Bounds.X + pos, e.Bounds.Y + ypos, 10, 10);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine("Failed to mark cell as locked: " + E.Message);
                            }
                        }
                        if (m_map_name == "Insp_mat!" || m_map_name == "Inj_map_0!" || m_map_name == "Ign_map_0!" || m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR" || m_map_name == "IdleTargetAFR" || m_map_name == "IdleFeedbackAFR" || m_map_name == "IdleFeedbackvsTargetAFR")
                        {
                            try
                            {
                                if (open_loop != null)
                                {
                                    int mapvalue = x_axisvalues[e.Column.AbsoluteIndex];
                                    int rpmvalue = y_axisvalues[e.RowHandle];
                                    if (open_loop.Length > 0)
                                    {
                                        int mapopenloop = (int)open_loop[(open_loop.Length - e.RowHandle) - 1];
                                        if (mapopenloop > mapvalue)
                                        {
                                            //e.Graphics.FillEllipse(Brushes.Black, e.Bounds.X, e.Bounds.Y, e.Bounds.X + 10, e.Bounds.Y+10);
                                            //e.Graphics.FillEllipse(Brushes.Yellow, e.Bounds.X+2, e.Bounds.Y+2, 4,4);
                                            Pen p = new Pen(Brushes.Black, 2);
                                            e.Graphics.DrawRectangle(p, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width -2 , e.Bounds.Height -2);
                                            p.Dispose();
                                          //  DrawHighlight(e.Graphics, e.Bounds);
                                        }
                                    }
                                }
                                if (_knockadaptloadfrom != 0 || _knockadaptloadupto != 0 || _knockadaptrpmfrom != 0 || _knockadaptrpmupto != 0)
                                {
                                   /* int mapvalue = x_axisvalues[e.Column.AbsoluteIndex];
                                    int rpmvalue = y_axisvalues[e.RowHandle];
                                    if (rpmvalue >= _knockadaptrpmfrom && rpmvalue <= _knockadaptrpmupto && mapvalue >= _knockadaptloadfrom && mapvalue <= _knockadaptloadupto)
                                    {
                                        Pen p = new Pen(Brushes.Blue, 1);
                                        e.Graphics.DrawRectangle(p, e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 5, e.Bounds.Height - 5);
                                        p.Dispose();

                                    }*/
                                    int mapvalue = x_axisvalues[e.Column.AbsoluteIndex];
                                    int rpmvalue = y_axisvalues[y_axisvalues.Length - e.RowHandle - 1];
                                    if (rpmvalue >= _knockadaptrpmfrom && rpmvalue <= _knockadaptrpmupto && mapvalue >= _knockadaptloadfrom && mapvalue <= _knockadaptloadupto)
                                    {
                                        Pen p = new Pen(Brushes.Blue, 1);
                                        e.Graphics.DrawRectangle(p, e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 5, e.Bounds.Height - 5);
                                        p.Dispose();

                                    }

                                }
                                if (_boostadaptrpmfrom != 0 || _boostadaptrpmupto != 0)
                                {
                                    /*if (e.Column.AbsoluteIndex == x_axisvalues.Length - 1)
                                    {
                                        int rpmvalue = y_axisvalues[y_axisvalues.Length - e.RowHandle - 1];
                                        if (rpmvalue >= _boostadaptrpmfrom && rpmvalue <= _boostadaptrpmupto)
                                        {
                                            Pen p = new Pen(Brushes.White, 1);
                                            e.Graphics.DrawRectangle(p, e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 5, e.Bounds.Height - 5);
                                            p.Dispose();
                                        }
                                    }*/
                                    if (e.Column.AbsoluteIndex == x_axisvalues.Length - 1)
                                    {
                                        int rpmvalue = y_axisvalues[y_axisvalues.Length - e.RowHandle - 1];
                                        if (rpmvalue >= _boostadaptrpmfrom && rpmvalue <= _boostadaptrpmupto)
                                        {
                                            Pen p = new Pen(Brushes.White, 1);
                                            e.Graphics.DrawRectangle(p, e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 5, e.Bounds.Height - 5);
                                            p.Dispose();
                                        }
                                    }

                                }
                                if (m_map_name.StartsWith("Feedback"))
                                {
                                    if (afr_counter != null)
                                    {
                                        // fetch correct counter
                                        int current_afrcounter = (int)afr_counter[(afr_counter.Length - ((e.RowHandle +1) * m_TableWidth)) + e.Column.AbsoluteIndex ];
                                        //current_afrcounter /= 10;
                                        if(current_afrcounter > 255) current_afrcounter = 255;
                                        if (current_afrcounter != 0)
                                        {
                                            Color cc = Color.FromArgb(255 - current_afrcounter, current_afrcounter, 0);

                                            Pen p = new Pen(cc, 1);
                                            e.Graphics.DrawRectangle(p, e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 5, e.Bounds.Height - 5);
                                            p.Dispose();
                                        }
                                        /*if (current_afrcounter < 100)
                                        {
                                            // one star
                                        }
                                        else if (current_afrcounter < 200)
                                        {
                                            // two stars
                                        }
                                        else if (current_afrcounter < 300)
                                        {
                                            // three stars
                                        }
                                        else if (current_afrcounter < 400)
                                        {
                                            // four stars
                                        }
                                        else
                                        {
                                            // five stars
                                        }*/

                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }

                        }
                        else if (m_map_name == "Fuel_knock_mat!")
                        {
                            try
                            {
                                if (open_loop_knock != null)
                                {
                                    if (open_loop_knock.Length > 0)
                                    {
                                        int mapvalue = x_axisvalues[e.Column.AbsoluteIndex];
                                        int rpmvalue = y_axisvalues[e.RowHandle];
                                        int mapopenloop = (int)open_loop_knock[(open_loop_knock.Length - e.RowHandle) - 1];
                                        if (mapopenloop > mapvalue)
                                        {
                                            //e.Graphics.FillEllipse(Brushes.Black, e.Bounds.X, e.Bounds.Y, e.Bounds.X + 10, e.Bounds.Y+10);
                                            //e.Graphics.FillEllipse(Brushes.Yellow, e.Bounds.X+2, e.Bounds.Y+2, 4,4);
                                            Pen p = new Pen(Brushes.Black, 2);
                                            e.Graphics.DrawRectangle(p, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                                            p.Dispose();
                                            //  DrawHighlight(e.Graphics, e.Bounds);
                                        }
                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }

                        }
                        /*else if (m_map_name == "Ign_map_0!" ||m_map_name == "Ign_map_2!" ||m_map_name == "Ign_map_4!" )
                        {
                            try
                            {
                                if (open_loop != null)
                                {
                                    if (open_loop.Length > 0)
                                    {
                                        int mapvalue = x_axisvalues[e.Column.AbsoluteIndex];
                                        int rpmvalue = y_axisvalues[e.RowHandle];
                                        int mapopenloop = (int)open_loop[(open_loop.Length - e.RowHandle) - 1];
                                        if (mapopenloop > mapvalue)
                                        {
                                            //e.Graphics.FillEllipse(Brushes.Black, e.Bounds.X, e.Bounds.Y, e.Bounds.X + 10, e.Bounds.Y+10);
                                            //e.Graphics.FillEllipse(Brushes.Yellow, e.Bounds.X+2, e.Bounds.Y+2, 4,4);
                                            Pen p = new Pen(Brushes.Black, 2);
                                            e.Graphics.DrawRectangle(p, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                                            p.Dispose();
                                            //  DrawHighlight(e.Graphics, e.Bounds);
                                        }
                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }

                        }
                        else if (m_map_name == "Tryck_mat!" || m_map_name == "Tryck_mat_a!")
                        {
                            try
                            {
                                if (open_loop != null)
                                {
                                    if (open_loop.Length > 0)
                                    {
                                        int mapvalue = Convert.ToInt32(e.CellValue);
                                        int rpmvalue = y_axisvalues[e.RowHandle];
                                        int mapopenloop = (int)open_loop[(open_loop.Length - e.RowHandle) - 1];
                                        if (mapopenloop > mapvalue)
                                        {
                                            //e.Graphics.FillEllipse(Brushes.Black, e.Bounds.X, e.Bounds.Y, e.Bounds.X + 10, e.Bounds.Y+10);
                                            //e.Graphics.FillEllipse(Brushes.Yellow, e.Bounds.X+2, e.Bounds.Y+2, 4,4);
                                            Pen p = new Pen(Brushes.Black, 2);
                                            e.Graphics.DrawRectangle(p, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                                            p.Dispose();
                                            //  DrawHighlight(e.Graphics, e.Bounds);
                                        }
                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }

                        }*/

                    }
                }

                // draw realtime highlight!
                if (m_selectedrowhandle >= 0 && m_selectedcolumnindex >= 0)
                {
                    if (e.RowHandle == m_selectedrowhandle && e.Column.AbsoluteIndex == m_selectedcolumnindex)
                    {
                        SolidBrush sbsb = new SolidBrush(Color.Yellow);
                        e.Graphics.FillRectangle(sbsb, e.Bounds);
                    }
                }

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void DrawHighlight(Graphics g, Rectangle clientRectangle)
        {
            
            clientRectangle.Height = clientRectangle.Height >> 1;
            clientRectangle.Inflate(-2, -2);
            Color color = Color.FromArgb(100, 0xff, 0xff, 0xff);
            Color color2 = Color.FromArgb(30, 0xff, 0xff, 0xff);
            this.DrawRoundRect(g, clientRectangle, /*((this.m_nCornerRadius - 1) > 1) ? ((float)(this.m_nCornerRadius - 1)) :*/ ((float)1), color, color2, Color.Empty, 0, true, false);
        }

        private void DrawRoundRect(Graphics g, Rectangle rect, float radius, Color col1, Color col2, Color colBorder, int nBorderWidth, bool bGradient, bool bDrawBorder)
        {
            GraphicsPath path = new GraphicsPath();
            float width = radius + radius;
            RectangleF ef = new RectangleF(0f, 0f, width, width);
            Brush brush = null;
            ef.X = rect.Left;
            ef.Y = rect.Top;
            path.AddArc(ef, 180f, 90f);
            ef.X = (rect.Right - 1) - width;
            path.AddArc(ef, 270f, 90f);
            ef.Y = (rect.Bottom - 1) - width;
            path.AddArc(ef, 0f, 90f);
            ef.X = rect.Left;
            path.AddArc(ef, 90f, 90f);
            path.CloseFigure();
            if (bGradient)
            {
                brush = new LinearGradientBrush(rect, col1, col2, 90f, false);
            }
            else
            {
                brush = new SolidBrush(col1);
            }
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(brush, path);
            if (/*bDrawBorder*/ true)
            {
                Pen pen = new Pen(colBorder);
                pen.Width = nBorderWidth;
                g.DrawPath(pen, path);
                pen.Dispose();
            }
            g.SmoothingMode = SmoothingMode.None;
            brush.Dispose();
            path.Dispose();
        }

        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            m_datasourceMutated = true;
            simpleButton2.Enabled = true;
            simpleButton3.Enabled = true;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            ShowTable(m_TableWidth, m_issixteenbit);
            m_datasourceMutated = false;
            simpleButton2.Enabled = false;
            simpleButton3.Enabled = false;
            //m_values_changed_highlight_user.Initialize(); // reset changes made!
            InitEditValues();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (m_isRAMViewer) return;
            else
            {
                m_SaveChanges = true;
                m_datasourceMutated = false;
                CastSaveEvent();
            }
        }

        private byte[] GetDataFromGridView(bool upsidedown)
        {

            byte[] retval = new byte[m_map_length];
            try
            {
                DataTable gdt = (DataTable)gridControl1.DataSource;
                int cellcount = 0;
                if (upsidedown)
                {
                    for (int t = gdt.Rows.Count - 1; t >= 0; t--)
                    {
                        foreach (object o in gdt.Rows[t].ItemArray)
                        {
                            if (o != null)
                            {
                                if (o != DBNull.Value)
                                {
                                    if (cellcount < retval.Length)
                                    {
                                        if (m_issixteenbit)
                                        {
                                            // twee waarde toevoegen
                                            Int32 cellvalue = 0;
                                            string bstr1 = "0";
                                            string bstr2 = "0";
                                            //if (m_isHexMode)
                                            if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                            {
                                                cellvalue = Convert.ToInt32(o.ToString(), 16);
                                            }
                                            else
                                            {
                                                cellvalue = Convert.ToInt32(o.ToString());
                                                if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                                {
                                                    // convert back to normal value
                                                    if (MapIsScalableFor3Bar(m_map_name))
                                                    {
                                                        cellvalue *= 100;
                                                        cellvalue /= 120;
                                                    }
                                                }
                                                else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                                {
                                                    // convert back to normal value
                                                    if (MapIsScalableFor3Bar(m_map_name))
                                                    {
                                                        cellvalue *= 100;
                                                        cellvalue /= 140;
                                                    }
                                                }
                                                else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar || m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                                                {
                                                    // convert back to normal value
                                                    if (MapIsScalableFor3Bar(m_map_name))
                                                    {
                                                        cellvalue *= 100;
                                                        cellvalue /= 160;
                                                    }
                                                }
                                                else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar || m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                                                {
                                                    // convert back to normal value
                                                    if (MapIsScalableFor3Bar(m_map_name))
                                                    {
                                                        cellvalue *= 100;
                                                        cellvalue /= 200;
                                                    }
                                                }
                                            }
                                            /*if (cellvalue < 0)
                                            {
                                                Console.WriteLine("value < 0");
                                            }*/
                                            bstr1 = cellvalue.ToString("X8").Substring(4, 2);
                                            bstr2 = cellvalue.ToString("X8").Substring(6, 2);
                                            retval.SetValue(Convert.ToByte(bstr1, 16), cellcount++);
                                            retval.SetValue(Convert.ToByte(bstr2, 16), cellcount++);
                                        }
                                        else
                                        {
                                            //if (m_isHexMode)
                                            if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                            {
                                                //double v = Convert.ToDouble(o);
                                                int iv = Convert.ToInt32(o.ToString(), 16);//(int)Math.Floor(v);
                                                retval.SetValue(Convert.ToByte(iv), cellcount++);
                                            }
                                            else
                                            {
                                                double v = Convert.ToDouble(o);
                                                if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                                {
                                                    // convert back to normal value
                                                    if (MapIsScalableFor3Bar(m_map_name))
                                                    {
                                                        v *= 100;
                                                        v /= 120;
                                                    }
                                                }
                                                else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                                {
                                                    // convert back to normal value
                                                    if (MapIsScalableFor3Bar(m_map_name))
                                                    {
                                                        v *= 100;
                                                        v /= 140;
                                                    }
                                                }
                                                else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                                {
                                                    // convert back to normal value
                                                    if (MapIsScalableFor3Bar(m_map_name))
                                                    {
                                                        v *= 100;
                                                        v /= 160;
                                                    }
                                                }
                                                else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                                {
                                                    // convert back to normal value
                                                    if (MapIsScalableFor3Bar(m_map_name))
                                                    {
                                                        v *= 100;
                                                        v /= 200;
                                                    }
                                                }
                                                //retval.SetValue(Convert.ToByte((int)Math.Floor(v)), cellcount++);
                                                retval.SetValue(Convert.ToByte((int)Math.Ceiling(v)), cellcount++);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                else
                {

                    foreach (DataRow dr in gdt.Rows)
                    {
                        foreach (object o in dr.ItemArray)
                        {
                            if (o != null)
                            {
                                if (o != DBNull.Value)
                                {
                                    if (cellcount < retval.Length)
                                    {
                                        if (m_issixteenbit)
                                        {
                                            // twee waarde toevoegen
                                            Int32 cellvalue = 0;
                                            string bstr1 = "0";
                                            string bstr2 = "0";
                                            //if (m_isHexMode)
                                            if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                            {
                                                cellvalue = Convert.ToInt32(o.ToString(), 16);
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    cellvalue = Convert.ToInt32(o.ToString());
                                                    if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                                    {
                                                        // convert back to normal value
                                                        if (MapIsScalableFor3Bar(m_map_name))
                                                        {
                                                            cellvalue *= 100;
                                                            cellvalue /= 120;
                                                        }
                                                    }
                                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                                    {
                                                        // convert back to normal value
                                                        if (MapIsScalableFor3Bar(m_map_name))
                                                        {
                                                            cellvalue *= 100;
                                                            cellvalue /= 140;
                                                        }
                                                    }
                                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar || m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                                                    {
                                                        // convert back to normal value
                                                        if (MapIsScalableFor3Bar(m_map_name))
                                                        {
                                                            cellvalue *= 100;
                                                            cellvalue /= 160;
                                                        }
                                                    }
                                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar || m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                                                    {
                                                        // convert back to normal value
                                                        if (MapIsScalableFor3Bar(m_map_name))
                                                        {
                                                            cellvalue *= 100;
                                                            cellvalue /= 200;
                                                        }
                                                    }
                                                }
                                                catch (Exception cE)
                                                {
                                                    Console.WriteLine(cE.Message);
                                                }
                                            }
                                            bstr1 = cellvalue.ToString("X8").Substring(4, 2);
                                            bstr2 = cellvalue.ToString("X8").Substring(6, 2);
                                            retval.SetValue(Convert.ToByte(bstr1, 16), cellcount++);
                                            retval.SetValue(Convert.ToByte(bstr2, 16), cellcount++);
                                            /*                                        bstr1 = cellvalue.ToString("X4").Substring(0, 2);
                                                                                    bstr2 = cellvalue.ToString("X4").Substring(2, 2);
                                                                                    retval.SetValue(Convert.ToByte(bstr1, 16), cellcount++);
                                                                                    retval.SetValue(Convert.ToByte(bstr2, 16), cellcount++);*/
                                        }
                                        else
                                        {
                                            //                                        if (m_isHexMode)
                                            if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                            {

                                                //double v = Convert.ToDouble(o);
                                                try
                                                {
                                                    int iv = Convert.ToInt32(o.ToString(), 16);
                                                    //int iv = (int)Math.Floor(v);
                                                    retval.SetValue(Convert.ToByte(iv.ToString()), cellcount++);
                                                }
                                                catch (Exception cE)
                                                {
                                                    Console.WriteLine(cE.Message);
                                                }

                                            }
                                            else
                                            {

                                                try
                                                {
                                                    double v = Convert.ToDouble(o);
                                                    if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                                    {
                                                        // convert back to normal value
                                                        if (MapIsScalableFor3Bar(m_map_name))
                                                        {
                                                            v *= 100;
                                                            v /= 120;
                                                        }
                                                    }
                                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                                    {
                                                        // convert back to normal value
                                                        if (MapIsScalableFor3Bar(m_map_name))
                                                        {
                                                            v *= 100;
                                                            v /= 140;
                                                        }
                                                    }
                                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                                    {
                                                        // convert back to normal value
                                                        if (MapIsScalableFor3Bar(m_map_name))
                                                        {
                                                            v *= 100;
                                                            v /= 160;
                                                        }
                                                    }
                                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                                    {
                                                        // convert back to normal value
                                                        if (MapIsScalableFor3Bar(m_map_name))
                                                        {
                                                            v *= 100;
                                                            v /= 200;
                                                        }
                                                    }
                                                    if (v >= 0)
                                                    {
                                                        //retval.SetValue(Convert.ToByte((int)Math.Floor(v)), cellcount++);
                                                        retval.SetValue(Convert.ToByte((int)Math.Ceiling(v)), cellcount++);
                                                    }
                                                }
                                                catch (Exception sE)
                                                {
                                                    Console.WriteLine(sE.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }

       

        private void CastLockEvent(int mode)
        {
            if (onAxisLock != null)
            {
                switch (mode)
                {
                    case 0: // autoscale
                        onAxisLock(this, new AxisLockEventArgs(-1, mode, m_map_name, m_filename));
                        break;
                    case 1: // peak value
                        onAxisLock(this, new AxisLockEventArgs(m_MaxValueInTable, mode, m_map_name, m_filename));
                        break;
                    case 2: // max value

                        int max_value = 0xFF;
                        if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                        {
                            if (MapIsScalableFor3Bar(m_map_name))
                            {
                                max_value = 306;
                            }
                        }
                        else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                        {
                            if (MapIsScalableFor3Bar(m_map_name))
                            {
                                max_value = 357;
                            }
                        }
                        else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar || m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                        {
                            if (MapIsScalableFor3Bar(m_map_name))
                            {
                                max_value = 408;
                            }
                        }
                        else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar || m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                        {
                            if (MapIsScalableFor3Bar(m_map_name))
                            {
                                max_value = 510;
                            }
                        }
                        if (m_issixteenbit) max_value = 0xFFFF;
                        onAxisLock(this, new AxisLockEventArgs(max_value, mode, m_map_name, m_filename));
                        break;
                }
            }
            else
            {
                Console.WriteLine("onAxisLock not registered");
            }
        }

        private void CastSelectEvent(int rowhandle, int colindex)
        {
            if (onSelectionChanged != null)
            {
                // haal eerst de data uit de tabel van de gridview
                onSelectionChanged(this, new CellSelectionChangedEventArgs(rowhandle, colindex, m_map_name));
                // set the row and column index in the surfacegraphviewer
                int numberofrows = m_map_content.Length / m_TableWidth;
                if (m_issixteenbit)
                {
                    numberofrows /= 2;
                }
                rowhandle = (numberofrows - 1) - rowhandle;
            }
            else
            {
                //Console.WriteLine("onSelectionChanged not registered!");
            }

        }

        private int LookUpIndexAxisRPMMap(double value, int[] axisvalues)
        {
            int return_index = -1;
            int multiplywith = 1;
            double min_difference = 10000000;
            for (int t = 0; t < axisvalues.Length; t ++)
            {
                int b = (int)axisvalues.GetValue(t);
                b *= multiplywith;
                double diff = Math.Abs((double)b - value);
                if (min_difference > diff)
                {
                    min_difference = diff;
                    return_index = t;
                }
            }
            return return_index;
        }

        private int LookUpIndexAxisMAPMap(double value, int[] axisvalues)
        {
            int return_index = -1;
            double multiplywith = 1;
            if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
            {
                multiplywith = 1.2;
            }
            else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
            {
                multiplywith = 1.4;
            }
            else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
            {
                multiplywith = 1.6;
            }
            else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
            {
                multiplywith = 2.0;
            }
            double min_difference = 10000000;
            for (int t = 0; t < axisvalues.Length; t++)
            {
                double b = Convert.ToDouble((int)axisvalues.GetValue(t));
                b *= multiplywith;

                b -= 100;
                b /= 100;
                double diff = Math.Abs(b - value);
                if (min_difference > diff)
                {
                    min_difference = diff;
                    return_index = t;
                }
            }
            return return_index;
        }
        private int LookUpIndexAxisMAPMapRegLast(double value, int[] axisvalues)
        {
            int return_index = -1;
            double multiplywith = 1;
            if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
            {
                multiplywith = 1.2;
            }
            else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
            {
                multiplywith = 1.4;
            }
            else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
            {
                multiplywith = 1.6;
            }
            else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
            {
                multiplywith = 2.0;
            }
            double min_difference = 10000000;
            for (int t = 0; t < axisvalues.Length; t++)
            {
                double b = Convert.ToDouble((int)axisvalues.GetValue(t));
                b *= multiplywith;

                // b -= 100;// no offsett for reg_last
                b /= 100;
                double diff = Math.Abs(b - value);
                if (min_difference > diff)
                {
                    min_difference = diff;
                    return_index = t;
                }
            }
            return return_index;
        }


        private int LookUpIndexAxisTPSMap(double value, int[] axisvalues)
        {
            int multiplywith = 1;
            int return_index = -1;
            double min_difference = 10000000;
            for (int t = 0; t < axisvalues.Length; t++)
            {
                int b = (int)axisvalues.GetValue(t);
                b *= multiplywith;
                double diff = Math.Abs((double)b - value);
                if (min_difference > diff)
                {
                    min_difference = diff;
                    return_index = t;
                }
            }
            return return_index;
        }

        private void UpdateLiveView()
        {
            int index_x = -1;
            int index_y = -1;
            // depends on the axis descriptions in the current map
            //Finish this method!
            if (m_x_axis_name == "MAP")
            {
                // get the index in the xaxis based on boost level
                index_x = LookUpIndexAxisMAPMap(_boost, x_axisvalues);
            }
            if (m_x_axis_name == "Pressure error (bar)")
            {
                // get the index in the xaxis based on boost level
                double boost_error = Math.Abs(_boost - _boostTarget);
                index_x = LookUpIndexAxisMAPMapRegLast(boost_error, x_axisvalues);
            }
            else if (m_x_axis_name == "RPM")
            {
                index_x = LookUpIndexAxisRPMMap(_rpm, x_axisvalues);
            }
            else if (m_x_axis_name == "TPS" || m_x_axis_name == "Throttle position" || m_x_axis_name == "Relative throttle position")
            {
                index_x = LookUpIndexAxisTPSMap(_tps, x_axisvalues);
            }
            if (m_y_axis_name == "MAP")
            {
                index_y = LookUpIndexAxisMAPMap(_boost, y_axisvalues);
            }
            else if (m_y_axis_name == "RPM")
            {
                index_y = LookUpIndexAxisRPMMap(_rpm, y_axisvalues);
            }
            else if (m_y_axis_name == "TPS" || m_x_axis_name == "Throttle position" || m_x_axis_name == "Relative throttle position")
            {
                index_y = LookUpIndexAxisTPSMap(_tps, y_axisvalues);
            }
            if (index_x >= 0 && index_y == -1) index_y = 0; // 2d ook weer kunnen geven
            else if (index_x == -1 && index_y >= 0) index_x = 0; // 2d ook weer kunnen geven
                 
            if (index_x >= 0 && index_y >= 0)
            {
                HighlightCell(index_x, index_y);
            }
        }

        private double _rpm = 0;

        public override double Rpm
        {
            get { return _rpm; }
            set
            {
                if (_rpm != value)
                {
                    _rpm = value;
                    UpdateLiveView();
                }
            }
        }
        private double _boost = 0;

        public override double Boost
        {
            get { return _boost; }
            set
            {
                if (_boost != value)
                {
                    _boost = value;
                    UpdateLiveView();
                }
            }
        }

        private double _boostTarget = 0;

        public override double BoostTarget
        {
            get { return _boostTarget; }
            set
            {
                if (_boostTarget != value)
                {
                    _boostTarget = value;
                    UpdateLiveView();
                }
            }
        }

        private double _coolant = 0;

        public override double Coolant
        {
            get { return _coolant; }
            set
            {
                if (_coolant != value)
                {
                    _coolant = value;
                    UpdateLiveView();
                }
            }
        }
        private double _iat = 0;

        public override double Iat
        {
            get { return _iat; }
            set
            {
                if (_iat != value)
                {
                    _iat = value;
                    UpdateLiveView();
                }
            }
        }
        private double _tps = 0;

        public override double Tps
        {
            get { return _tps; }
            set
            {
                if (_tps != value)
                {
                    _tps = value;
                    UpdateLiveView();
                }
            }
        }

        public override void SelectCell(int rowhandle, int colindex)
        {
            try
            {
                m_prohibitcellchange = true;
                gridView1.ClearSelection();
                gridView1.SelectCell(rowhandle, gridView1.Columns[colindex]);
                m_prohibitcellchange = false;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        public override void SetSplitter(int panel1height, int panel2height, int splitdistance, bool panel1collapsed, bool panel2collapsed)
        {
        }


        private void CastSaveEvent()
        {
            if (onSymbolSave != null)
            {
                // haal eerst de data uit de tabel van de gridview
                byte[] mutateddata = GetDataFromGridView(m_isUpsideDown);
                onSymbolSave(this, new SaveSymbolEventArgs(m_map_address, m_map_length, mutateddata, m_map_name, Filename));
                m_datasourceMutated = false;
                simpleButton2.Enabled = false;
                simpleButton3.Enabled = false;
            }
            else
            {
                Console.WriteLine("onSymbolSave not registered!");
            }

        }

        private void CastSplitterMovedEvent()
        {

        }



        private void CastCloseEvent()
        {
            if (onClose != null)
            {
                onClose(this, EventArgs.Empty);
            }
        }

        private void groupControl2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UpdateValueChangedByUser(int column, int row)
        {
            try
            {
                m_values_changed_highlight_user.SetValue((byte)1, (row * m_TableWidth) + column);
            }
            catch (Exception E)
            {
                Console.WriteLine("UpdateValueChangedByUser: " + E.Message);
            }
        }

        private void UpdateValueChangedByECU(int column, int row)
        {
            try
            {
                m_values_changed_highlight_ecu.SetValue((byte)1, (row * m_TableWidth) + column);
            }
            catch (Exception E)
            {
                Console.WriteLine("UpdateValueChangedByUser: " + E.Message);
            }
        }


        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            // update value in changed map
            UpdateValueChangedByUser(e.Column.AbsoluteIndex, e.RowHandle);
            m_datasourceMutated = true;
            simpleButton2.Enabled = true;
            simpleButton3.Enabled = true;
            if (m_DirectSRAMWriteOnSymbolChange)
            {
                // calculate address to update!
                //int addresstoupdate = m_map_sramaddress;
                //addresstoupdate += e.row
                CastWriteToSRAM();
            }
        }

        private void StartSingleLineGraphTimer()
        {
            timer3.Stop();
            timer3.Start();
        }

        private void StartChartUpdateTimer()
        {
            timer1.Stop();
            timer1.Start();
        }

        private void StartSurfaceChartUpdateTimer()
        {
            timer2.Stop();
            timer2.Start();
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            if (cellcollection.Length > 0)
            {
                if (e.KeyCode == Keys.Add)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if(m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value++;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                if (m_viewtype != Trionic5Tools.ViewType.Easy3Bar && m_viewtype != Trionic5Tools.ViewType.Decimal3Bar)
                                {
                                    if (value > 0xFF) value = 0xFF;
                                }
                                else
                                {
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                        {
                                            if (value > 306) value = 306;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                        {
                                            if (value > 357) value = 357;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                        {
                                            if (value > 408) value = 408;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                        {
                                            if (value > 510) value = 510;
                                        }
                                    }
                                    else
                                    {
                                        if (value > 0xFF) value = 0xFF;
                                    }
                                }
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value++;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                if (m_viewtype != Trionic5Tools.ViewType.Easy3Bar && m_viewtype != Trionic5Tools.ViewType.Decimal3Bar)
                                {
                                    if (value > 0xFF) value = 0xFF;
                                }
                                else
                                {
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                        {
                                            if (value > 306) value = 306;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                        {
                                            if (value > 357) value = 357;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                        {
                                            if (value > 408) value = 408;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                        {
                                            if (value > 510) value = 510;
                                        }
                                    }
                                    else
                                    {
                                        if (value > 0xFF) value = 0xFF;
                                    }
                                }
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.Subtract)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value--;
                            if (!m_issixteenbit)
                            {
                                if (value < 0) value = 0;
                            }
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value--;
                            if (!m_issixteenbit)
                            {
                                if (value < 0) value = 0;
                            }
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value += 0x10;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                if (m_viewtype != Trionic5Tools.ViewType.Easy3Bar && m_viewtype != Trionic5Tools.ViewType.Decimal3Bar)
                                {
                                    if (value > 0xFF) value = 0xFF;
                                }
                                else
                                {
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                        {
                                            if (value > 306) value = 306;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                        {
                                            if (value > 357) value = 357;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                        {
                                            if (value > 408) value = 408;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                        {
                                            if (value > 510) value = 510;
                                        }
                                    }
                                    else
                                    {
                                        if (value > 0xFF) value = 0xFF;
                                    }
                                }
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value+=10;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                if (m_viewtype != Trionic5Tools.ViewType.Easy3Bar && m_viewtype != Trionic5Tools.ViewType.Decimal3Bar)
                                {
                                    if (value > 0xFF) value = 0xFF;
                                }
                                else
                                {
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                        {
                                            if (value > 306) value = 306;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                        {
                                            if (value > 357) value = 357;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                        {
                                            if (value > 408) value = 408;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                        {
                                            if (value > 510) value = 510;
                                        }
                                    }
                                    else
                                    {
                                        if (value > 0xFF) value = 0xFF;
                                    }
                                }
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value-=0x10;
                            if (!m_issixteenbit)
                            {
                                if (value < 0) value = 0;
                            }
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value-=10;
                            if (!m_issixteenbit)
                            {
                                if (value < 0) value = 0;
                            }
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.Home)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                        {

                            int value = 0xFFFF;
                            if (m_issixteenbit)
                            {
                                value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                if (m_viewtype != Trionic5Tools.ViewType.Easy3Bar && m_viewtype != Trionic5Tools.ViewType.Decimal3Bar)
                                {
                                    value = 0xFF;
                                }
                                else
                                {
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                        {
                                            if (value > 306) value = 306;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                        {
                                            if (value > 357) value = 357;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                        {
                                            if (value > 408) value = 408;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                        {
                                            if (value > 510) value = 510;
                                        }
                                    }
                                    else
                                    {
                                        value = 0xFF;
                                    }
                                }
                                
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;

                        }
                        else
                        {
                            int value = 0xFFFF;
                            if (m_issixteenbit)
                            {
                                value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                if (m_viewtype != Trionic5Tools.ViewType.Easy3Bar && m_viewtype != Trionic5Tools.ViewType.Decimal3Bar)
                                {
                                    value = 0xFF;
                                }
                                else
                                {
                                    if (MapIsScalableFor3Bar(m_map_name))
                                    {
                                        if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                        {
                                            if (value > 306) value = 306;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                        {
                                            if (value > 357) value = 357;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                        {
                                            if (value > 408) value = 408;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                        {
                                            if (value > 510) value = 510;
                                        }
                                    }
                                    else
                                    {
                                        value = 0xFF;
                                    }
                                }
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;

                        }
                    }
                }
                else if (e.KeyCode == Keys.End)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                        {
                            int value = 0;
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = 0;
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }

            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                
              //  e.Painter.DrawCaption(new DevExpress.Utils.Drawing.ObjectInfoArgs(new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics)), "As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, null);
               // e.Cache.DrawString("As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, new StringFormat());
                try
                {
                    if (y_axisvalues.Length > 0)
                    {
                        if (y_axisvalues.Length > e.RowHandle)
                        {
                            string yvalue = y_axisvalues.GetValue((y_axisvalues.Length-1) - e.RowHandle).ToString();
                            if (!m_isUpsideDown)
                            {
                                // dan andere waarde nemen
                                yvalue = y_axisvalues.GetValue(e.RowHandle).ToString();
                            }
                            if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                            {
                                yvalue = Convert.ToInt32(/*y_axisvalues.GetValue(e.RowHandle)*/y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle)).ToString("X4");
                            }
                            if (m_y_axis_name == "MAP" )
                            {
                                if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                {
                                    int tempval = Convert.ToInt32(y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle));
                                    if (!m_isUpsideDown)
                                    {
                                        tempval = Convert.ToInt32(y_axisvalues.GetValue(e.RowHandle));
                                    }
                                    tempval *= 120;
                                    tempval /= 100;
                                    yvalue = tempval.ToString();
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                {
                                    int tempval = Convert.ToInt32(y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle));
                                    if (!m_isUpsideDown)
                                    {
                                        tempval = Convert.ToInt32(y_axisvalues.GetValue(e.RowHandle));
                                    }
                                    tempval *= 140;
                                    tempval /= 100;
                                    yvalue = tempval.ToString();
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar || m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                                {
                                    int tempval = Convert.ToInt32(y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle));
                                    if (!m_isUpsideDown)
                                    {
                                        tempval = Convert.ToInt32(y_axisvalues.GetValue(e.RowHandle));
                                    }
                                    tempval *= 160;
                                    tempval /= 100;
                                    yvalue = tempval.ToString();
                                }
                                else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar || m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                                {
                                    int tempval = Convert.ToInt32(y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle));
                                    if (!m_isUpsideDown)
                                    {
                                        tempval = Convert.ToInt32(y_axisvalues.GetValue(e.RowHandle));
                                    }
                                    tempval *= 200;
                                    tempval /= 100;
                                    yvalue = tempval.ToString();
                                }
                            }

                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            e.Graphics.DrawString(yvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 4, e.Bounds.Y + 1 + (e.Bounds.Height - m_textheight) / 2));
                            e.Handled = true;
                        }
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (x_axisvalues.Length > 0)
                {
                    if (e.Column != null)
                    {
                        if (x_axisvalues.Length > e.Column.VisibleIndex)
                        {
                            string xvalue = x_axisvalues.GetValue(e.Column.VisibleIndex).ToString();
                            if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                            {
                                xvalue = Convert.ToInt32(x_axisvalues.GetValue(e.Column.VisibleIndex)).ToString(m_xformatstringforhex);
                            }
                            else if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Decimal || m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                            {
                                if (m_x_axis_name == "MAP" || m_x_axis_name == "Pressure error (bar)")
                                {
                                    int tempvalue = Convert.ToInt32(x_axisvalues.GetValue(e.Column.VisibleIndex));
                                    if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                    {
                                        tempvalue *= 120;
                                        tempvalue /= 100;
                                    }
                                    if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                    {
                                        tempvalue *= 140;
                                        tempvalue /= 100;
                                    }
                                    if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                                    {
                                        tempvalue *= 160;
                                        tempvalue /= 100;
                                    }
                                    if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                                    {
                                        tempvalue *= 200;
                                        tempvalue /= 100;
                                    }
                                    xvalue = tempvalue.ToString();
                                }
                            }
                            if (m_viewtype == Trionic5Tools.ViewType.Easy || m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                            {
                                if (m_x_axis_name == "MAP" || m_x_axis_name == "Pressure error (bar)")
                                {
                                    try
                                    {
                                        float v = (float)Convert.ToDouble(xvalue);
                                        if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                        {
                                            v *= 1.2F;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                        {
                                            v *= 1.4F;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                        {
                                            v *= 1.6F;
                                        }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                        {
                                            v *= 2.0F;
                                        }
                                        v *= (float)0.01F;
                                        if (m_x_axis_name == "MAP")
                                        {
                                            v -= 1;
                                        }
                                        xvalue = v.ToString("F2");
                                    }
                                    catch (Exception cE)
                                    {
                                        Console.WriteLine(cE.Message);
                                    }
                                }
                                /*if (m_map_name.StartsWith("P_fors") || m_map_name.StartsWith("I_fors") || m_map_name.StartsWith("D_fors"))
                                {
                                    try
                                    {
                                        float v = (float)Convert.ToDouble(xvalue);
                                        v *= (float)0.01F;
                                        xvalue = v.ToString("F2");
                                    }
                                    catch (Exception cE)
                                    {
                                        Console.WriteLine(cE.Message);
                                    }
                                }*/
                            }

                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            //e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, r);
                            e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 3, e.Bounds.Y + 1 + (e.Bounds.Height - m_textheight) / 2));
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }


        internal void ReShowTable()
        {
            ShowTable(m_TableWidth, m_issixteenbit);
        }

        private void chartControl1_Click(object sender, EventArgs e)
        {

        }

       

        private SeriesPoint _sp_dragging;

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Enabled = false;
        }



        

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
        }

        public override void SetSelectedTabPageIndex(int tabpageindex)
        {
            Invalidate();
        }

        private void CastViewTypeChangedEvent()
        {
            if (onViewTypeChanged != null)
            {
                onViewTypeChanged(this, new ViewTypeChangedEventArgs(m_viewtype, m_map_name));
                m_previousviewtype = m_viewtype;
            }
        }

       


        private int GetRowForAxisValue(string axisvalue)
        {
            int retval = -1;
            for (int t = 0; t < y_axisvalues.Length; t++)
            {
                if (m_y_axis_name == "MAP")
                {
                    // <GS-10112009> hier afmaken?
                    if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy)
                    {
                        int tempvalue = Convert.ToInt32(axisvalue);
                        tempvalue *= 100;
                        tempvalue /= 120;
                        axisvalue = tempvalue.ToString();
                    }
                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                    {
                        int tempvalue = Convert.ToInt32(axisvalue);
                        tempvalue *= 100;
                        tempvalue /= 140;
                        axisvalue = tempvalue.ToString();
                    }
                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                    {
                        int tempvalue = Convert.ToInt32(axisvalue);
                        tempvalue *= 100;
                        tempvalue /= 160;
                        axisvalue = tempvalue.ToString();
                    }
                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                    {
                        int tempvalue = Convert.ToInt32(axisvalue);
                        tempvalue *= 100;
                        tempvalue /= 200;
                        axisvalue = tempvalue.ToString();
                    }
                }
                if (y_axisvalues.GetValue(t).ToString() == axisvalue)
                {
                    retval = (y_axisvalues.Length - 1) - t;
                }
            }
            return retval;
        }



        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
           
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
        }

        private void MapViewerCellEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextEdit)
            {
                TextEdit txtedit = (TextEdit)sender;
                if (e.KeyCode == Keys.Add)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    //if (IsHexMode)
                    if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                    {
                        int value = Convert.ToInt32(txtedit.Text, 16);
                        value++;
                        if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                        if (m_issixteenbit)
                        {
                            if (value > 0xFFFF) value = 0xFFFF;
                            txtedit.Text = value.ToString("X4");
                        }
                        else
                        {
                            if (m_viewtype != Trionic5Tools.ViewType.Easy3Bar && m_viewtype != Trionic5Tools.ViewType.Decimal3Bar)
                            {
                                if (value > 0xFF) value = 0xFF;
                            }
                            else
                            {
                                if (MapIsScalableFor3Bar(m_map_name))
                                {
                                    if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                    {
                                        if (value > 306) value = 306;
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                    {
                                        if (value > 357) value = 357;
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                    {
                                        if (value > 408) value = 408;
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                    {
                                        if (value > 510) value = 510;
                                    }
                                }
                                else
                                {
                                    if (value > 0xFF) value = 0xFF;
                                }
                            }
                            txtedit.Text = value.ToString("X2");
                        }
                    }
                    else
                    {
                        int value = Convert.ToInt32(txtedit.Text);
                        value++;
                        if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                        if (m_issixteenbit)
                        {
                            if (value > 0xFFFF) value = 0xFFFF;
                            txtedit.Text = value.ToString();
                        }
                        else
                        {
                            if (value > 0xFF) value = 0xFF;
                            txtedit.Text = value.ToString();
                        }

                    }

                }
                else if (e.KeyCode == Keys.Subtract)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    //if (IsHexMode)
                    if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                    {
                        int value = Convert.ToInt32(txtedit.Text, 16);
                        value--;
                        if (value < 0) value = 0;
                        if (m_issixteenbit)
                        {
                            txtedit.Text = value.ToString("X4");
                        }
                        else
                        {
                            txtedit.Text = value.ToString("X2");
                        }
                    }
                    else
                    {
                        int value = Convert.ToInt32(txtedit.Text);
                        value--;
                        if (value < 0) value = 0;
                        if (m_issixteenbit)
                        {
                            txtedit.Text = value.ToString();
                        }
                        else
                        {
                            txtedit.Text = value.ToString();
                        }

                    }

                }
            }

        }

        private void CopySelectionToClipboard()
        {
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            CellHelperCollection chc = new CellHelperCollection();
            foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
            {
                CellHelper ch = new CellHelper();
                ch.Rowhandle = gc.RowHandle;
                ch.Columnindex = gc.Column.AbsoluteIndex;
                object o = gridView1.GetRowCellValue(gc.RowHandle, gc.Column);
                if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                {
                    ch.Value = Convert.ToInt32(o.ToString(), 16);
                }
                else
                {
                    ch.Value = Convert.ToInt32(o);
                }
                chc.Add(ch);
            }
            string serialized = ((int)m_viewtype).ToString();//string.Empty;
            foreach (CellHelper ch in chc)
            {
                serialized += ch.Columnindex.ToString() + ":" + ch.Rowhandle.ToString() + ":" + ch.Value.ToString() + ":~";
            }
            
            Clipboard.SetText(serialized);
        }

        private void CopyMapToClipboard()
        {
            gridView1.SelectAll();
            CopySelectionToClipboard();
            gridView1.ClearSelection();
        }

        private void copySelectedCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            if (cellcollection.Length > 0)
            {
                CopySelectionToClipboard();
            }
            else
            {
                if (MessageBox.Show("No selection, copy the entire map?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CopyMapToClipboard();
                }
            }
        }

        private void atCurrentlySelectedLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // Clipboard.ContainsData("System.Object");
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            if (cellcollection.Length >= 1)
            {
                try
                {
                    int rowhandlefrom = cellcollection[0].RowHandle;
                    int colindexfrom = cellcollection[0].Column.AbsoluteIndex;
                    int originalrowoffset = -1;
                    int originalcolumnoffset = -1;
                    if (Clipboard.ContainsText())
                    {
                        string serialized = Clipboard.GetText();
                        //   Console.WriteLine(serialized);
                        int viewtypeinclipboard = Convert.ToInt32(serialized.Substring(0, 1));
                        Trionic5Tools.ViewType vtclip = (Trionic5Tools.ViewType)viewtypeinclipboard;
                        serialized = serialized.Substring(1);
                        char[] sep = new char[1];
                        sep.SetValue('~', 0);
                        string[] cells = serialized.Split(sep);
                        foreach (string cell in cells)
                        {
                            char[] sep2 = new char[1];
                            sep2.SetValue(':', 0);
                            string[] vals = cell.Split(sep2);
                            if (vals.Length >= 3)
                            {

                                int rowhandle = Convert.ToInt32(vals.GetValue(1));
                                int colindex = Convert.ToInt32(vals.GetValue(0));
                                int ivalue = 0;
                                double dvalue = 0;
                                if (vtclip == Trionic5Tools.ViewType.Hexadecimal)
                                {
                                    ivalue = Convert.ToInt32(vals.GetValue(2).ToString());
                                    dvalue = ivalue;
                                }
                                else if (vtclip == Trionic5Tools.ViewType.Decimal || vtclip == Trionic5Tools.ViewType.Decimal3Bar || vtclip == Trionic5Tools.ViewType.Decimal35Bar || vtclip == Trionic5Tools.ViewType.Decimal4Bar || vtclip == Trionic5Tools.ViewType.Decimal5Bar)
                                {
                                    ivalue = Convert.ToInt32(vals.GetValue(2));
                                    dvalue = ivalue;
                                }
                                else if (vtclip == Trionic5Tools.ViewType.Easy || vtclip == Trionic5Tools.ViewType.Easy3Bar || vtclip == Trionic5Tools.ViewType.Easy35Bar || vtclip == Trionic5Tools.ViewType.Easy4Bar || vtclip == Trionic5Tools.ViewType.Easy5Bar)
                                {
                                    dvalue = Convert.ToDouble(vals.GetValue(2));
                                }

                                if (originalrowoffset == -1) originalrowoffset = rowhandle;
                                if (originalcolumnoffset == -1) originalcolumnoffset = colindex;
                                if (rowhandle >= 0 && colindex >= 0)
                                {
                                    try
                                    {
                                        if (vtclip == Trionic5Tools.ViewType.Hexadecimal)
                                        {
                                            //gridView1.SetRowCellValue(rowhandle, gridView1.Columns[colindex], ivalue.ToString("X"));
                                            gridView1.SetRowCellValue(rowhandlefrom + (rowhandle - originalrowoffset), gridView1.Columns[colindexfrom + (colindex - originalcolumnoffset)], ivalue.ToString("X"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(rowhandlefrom + (rowhandle - originalrowoffset), gridView1.Columns[colindexfrom + (colindex - originalcolumnoffset)], dvalue);
                                        }

                                    }
                                    catch (Exception E)
                                    {
                                        Console.WriteLine(E.Message);
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception pasteE)
                {
                    Console.WriteLine(pasteE.Message);
                }
            }
        }

        private void inOrgininalPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string serialized = Clipboard.GetText();
                try
                {
                    //   Console.WriteLine(serialized);
                    int viewtypeinclipboard = Convert.ToInt32(serialized.Substring(0, 1));
                    Trionic5Tools.ViewType vtclip = (Trionic5Tools.ViewType)viewtypeinclipboard;
                    serialized = serialized.Substring(1);

                    char[] sep = new char[1];
                    sep.SetValue('~', 0);
                    string[] cells = serialized.Split(sep);
                    foreach (string cell in cells)
                    {
                        char[] sep2 = new char[1];
                        sep2.SetValue(':', 0);
                        string[] vals = cell.Split(sep2);
                        if (vals.Length >= 3)
                        {
                            int rowhandle = Convert.ToInt32(vals.GetValue(1));
                            int colindex = Convert.ToInt32(vals.GetValue(0));
                            //int value = Convert.ToInt32(vals.GetValue(2));
                            int ivalue = 0;
                            double dvalue = 0;
                            if (vtclip == Trionic5Tools.ViewType.Hexadecimal)
                            {
                                ivalue = Convert.ToInt32(vals.GetValue(2).ToString());
                                dvalue = ivalue;
                            }
                            else if (vtclip == Trionic5Tools.ViewType.Decimal || vtclip == Trionic5Tools.ViewType.Decimal3Bar || vtclip == Trionic5Tools.ViewType.Decimal35Bar || vtclip == Trionic5Tools.ViewType.Decimal4Bar || vtclip == Trionic5Tools.ViewType.Decimal5Bar)
                            {
                                ivalue = Convert.ToInt32(vals.GetValue(2));
                                dvalue = ivalue;
                            }
                            else if (vtclip == Trionic5Tools.ViewType.Easy || vtclip == Trionic5Tools.ViewType.Easy3Bar || vtclip == Trionic5Tools.ViewType.Easy35Bar || vtclip == Trionic5Tools.ViewType.Easy4Bar || vtclip == Trionic5Tools.ViewType.Easy5Bar)
                            {
                                dvalue = Convert.ToDouble(vals.GetValue(2));
                            }
                            if (rowhandle >= 0 && colindex >= 0)
                            {
                                try
                                {
                                    if (vtclip == Trionic5Tools.ViewType.Hexadecimal)
                                    {
                                        gridView1.SetRowCellValue(rowhandle, gridView1.Columns[colindex], ivalue.ToString("X"));
                                    }
                                    else
                                    {
                                        gridView1.SetRowCellValue(rowhandle, gridView1.Columns[colindex], dvalue);
                                    }
                                }
                                catch (Exception E)
                                {
                                    Console.WriteLine(E.Message);
                                }
                            }
                        }

                    }
                }
                catch (Exception pasteE)
                {
                    Console.WriteLine(pasteE.Message);
                }
            }
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
//            m_isHexMode = !m_isHexMode;
            if (m_viewtype != Trionic5Tools.ViewType.Hexadecimal) m_viewtype = Trionic5Tools.ViewType.Hexadecimal;
            else m_viewtype = m_previousviewtype;
            ShowTable(m_TableWidth, m_issixteenbit);
        }

        private double ConvertToDouble(string v)
        {
            double d = 0;
            if (v == "") return d;
            string vs = "";
            vs = v.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
        }



        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                
                double _workvalue = ConvertToDouble(toolStripTextBox1.Text);
                DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
                if (cellcollection.Length > 0)
                {
                    switch (toolStripComboBox1.SelectedIndex)
                    {
                        case 0: // addition
                            foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                            {
                                try
                                {
                                    int value = 0;
                                    if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString(), 16);
                                        value += (int)Math.Round(_workvalue);
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        if (value > 0xFF)
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X4"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X2"));
                                        }
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value += (int)Math.Round(_workvalue);
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value += (int)Math.Round(_workvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 78642) value = 78642;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 306) value = 306;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value += (int)Math.Round(_workvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 91749) value = 91749;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 357) value = 357;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value += (int)Math.Round(_workvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 104856) value = 104856;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 408) value = 408;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value += (int)Math.Round(_workvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 131070) value = 131070;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 510) value = 510;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue += _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);

                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue += _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 78642) value = 78642;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 306) value = 306;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                               // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue += _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 91749) value = 91749;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 357) value = 357;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue += _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 104856) value = 104856;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 408) value = 408;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue += _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 131070) value = 131070;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 510) value = 510;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                }
                                catch (Exception cE)
                                {
                                    Console.WriteLine(cE.Message);
                                }
                            }
                            break;
                        case 1: // multiply
                            foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                            {
                                try
                                {
                                    int value = 0;
                                    if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString(), 16);
                                        value *= (int)(100 * _workvalue); // was (int)Math.Round(_workvalue)
                                        value /= 100;
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        if (value > 0xFF)
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X4"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X2"));
                                        }
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value *= (int)(100 * _workvalue); // was (int)Math.Round(_workvalue)
                                        value /= 100;
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value *= (int)(100 * _workvalue); // was (int)Math.Round(_workvalue)
                                        value /= 100;
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 78642) value = 78642;
                                               // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 306) value = 306;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value *= (int)(100 * _workvalue); // was (int)Math.Round(_workvalue)
                                        value /= 100;
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 91749) value = 91749;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 357) value = 357;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value *= (int)(100 * _workvalue); // was (int)Math.Round(_workvalue)
                                        value /= 100;
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 104856) value = 104856;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 408) value = 408;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value *= (int)(100 * _workvalue); // was (int)Math.Round(_workvalue)
                                        value /= 100;
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 131070) value = 131070;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 510) value = 510;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue *= _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);

                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue *= _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 78642) value = 78642;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 306) value = 306;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue *= _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 91749) value = 91749;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 357) value = 357;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue *= _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 104856) value = 104856;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 408) value = 408;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue *= _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 131070) value = 131070;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 510) value = 510;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                //if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                }
                                catch (Exception cE)
                                {
                                    Console.WriteLine(cE.Message);
                                }

                            }
                            break;
                        case 2: // divide
                            foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                            {
                                if (_workvalue == 0) _workvalue = 1;
                                try
                                {
                                    int value = 0;
                                    if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString(), 16);
                                        if (_workvalue != 0)
                                        {
                                            
                                            double tempvalue = value;
                                            tempvalue /= _workvalue; // was (int)Math.Round(_workvalue)
                                            value = (int)tempvalue;

                                        }
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        if (value > 0xFF)
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X4"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X2"));
                                        }
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        if (_workvalue != 0)
                                        {
                                            double tempvalue = value;
                                            tempvalue /= _workvalue; // was (int)Math.Round(_workvalue)
                                            value = (int)tempvalue;
                                        }
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        if (_workvalue != 0)
                                        {
                                            double tempvalue = value;
                                            tempvalue /= _workvalue; // was (int)Math.Round(_workvalue)
                                            value = (int)tempvalue;
                                        }
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 78642) value = 78642;
                                               // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 306) value = 306;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                               // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        if (_workvalue != 0)
                                        {
                                            double tempvalue = value;
                                            tempvalue /= _workvalue; // was (int)Math.Round(_workvalue)
                                            value = (int)tempvalue;
                                        }
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 91749) value = 91749;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 357) value = 357;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        if (_workvalue != 0)
                                        {
                                            double tempvalue = value;
                                            tempvalue /= _workvalue; // was (int)Math.Round(_workvalue)
                                            value = (int)tempvalue;
                                        }
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 104856) value = 104856;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 408) value = 408;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        if (_workvalue != 0)
                                        {
                                            double tempvalue = value;
                                            tempvalue /= _workvalue; // was (int)Math.Round(_workvalue)
                                            value = (int)tempvalue;
                                        }
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 131070) value = 131070;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 510) value = 510;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        if (_workvalue != 0)
                                        {
                                            dvalue /= _workvalue;
                                        }
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);

                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                           // if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        if (_workvalue != 0)
                                        {
                                            dvalue /= _workvalue;
                                        }
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 78642) value = 78642;
                                               // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 306) value = 306;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                               // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        if (_workvalue != 0)
                                        {
                                            dvalue /= _workvalue;
                                        }
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 91749) value = 91749;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 357) value = 357;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        if (_workvalue != 0)
                                        {
                                            dvalue /= _workvalue;
                                        }
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 104856) value = 104856;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 408) value = 408;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                    {
                                        double dvalue = ConvertToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString());
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        if (_workvalue != 0)
                                        {
                                            dvalue /= _workvalue;
                                        }
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //value = (int)dvalue;
                                        value = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 131070) value = 131070;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 510) value = 510;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                }
                                catch (Exception cE)
                                {
                                    Console.WriteLine(cE.Message);
                                }
                            }
                            break;
                        case 3: // fill
                            foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                            {
                                try
                                {
                                    double value = _workvalue;
                                    if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                                    {
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                           // if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        int dvalue = (int)value;
                                        if (value > 0xFF)
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString("X4"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString("X2"));
                                        }
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal)
                                    {
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        int dvalue = (int)value;
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar)
                                    {
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 78642) value = 78642;
                                              //  if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 306) value = 306;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                               // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        int dvalue = (int)value;
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString());
                                    }
                                        else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar)
                                    {
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 91749) value = 91749;
                                                //  if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 357) value = 357;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        int dvalue = (int)value;
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar)
                                    {
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 104856) value = 104856;
                                                //  if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 408) value = 408;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        int dvalue = (int)value;
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar)
                                    {
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 131070) value = 131070;
                                                //  if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 510) value = 510;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (value > 0xFFFF) value = 0xFFFF;
                                                // if (value < 0) value = 0;
                                            }
                                            else
                                            {
                                                if (value > 0xFF) value = 0xFF;
                                                if (value < 0) value = 0;
                                            }
                                        }
                                        int dvalue = (int)value;
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy)
                                    {
                                        double dvalue = _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //int decvalue = (int)dvalue;
                                        int decvalue = (int)Math.Round(dvalue);
                                        if (m_issixteenbit)
                                        {
                                            if (decvalue > 0xFFFF) decvalue = 0xFFFF;
                                          //  if (decvalue < 0) decvalue = 0;
                                        }
                                        else
                                        {
                                            if (decvalue > 0xFF) decvalue = 0xFF;
                                            if (decvalue < 0) decvalue = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, decvalue.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                                    {
                                        double dvalue = _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //int decvalue = (int)dvalue;
                                        int decvalue = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (decvalue > 78642) decvalue = 78642;
                                              //  if (decvalue < 0) decvalue = 0;
                                            }
                                            else
                                            {
                                                if (decvalue > 306) decvalue = 306;
                                                if (decvalue < 0) decvalue = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (decvalue > 0xFFFF) decvalue = 0xFFFF;
                                               // if (decvalue < 0) decvalue = 0;
                                            }
                                            else
                                            {
                                                if (decvalue > 0xFF) decvalue = 0xFF;
                                                if (decvalue < 0) decvalue = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, decvalue.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                                    {
                                        double dvalue = _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //int decvalue = (int)dvalue;
                                        int decvalue = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (decvalue > 91749) decvalue = 91749;
                                                //  if (decvalue < 0) decvalue = 0;
                                            }
                                            else
                                            {
                                                if (decvalue > 306) decvalue = 306;
                                                if (decvalue < 0) decvalue = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (decvalue > 0xFFFF) decvalue = 0xFFFF;
                                                // if (decvalue < 0) decvalue = 0;
                                            }
                                            else
                                            {
                                                if (decvalue > 0xFF) decvalue = 0xFF;
                                                if (decvalue < 0) decvalue = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, decvalue.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                                    {
                                        double dvalue = _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //int decvalue = (int)dvalue;
                                        int decvalue = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (decvalue > 104856) decvalue = 104856;
                                                //  if (decvalue < 0) decvalue = 0;
                                            }
                                            else
                                            {
                                                if (decvalue > 408) decvalue = 408;
                                                if (decvalue < 0) decvalue = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (decvalue > 0xFFFF) decvalue = 0xFFFF;
                                                // if (decvalue < 0) decvalue = 0;
                                            }
                                            else
                                            {
                                                if (decvalue > 0xFF) decvalue = 0xFF;
                                                if (decvalue < 0) decvalue = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, decvalue.ToString());
                                    }
                                    else if (m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                                    {
                                        double dvalue = _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        //int decvalue = (int)dvalue;
                                        int decvalue = (int)Math.Round(dvalue);
                                        if (MapIsScalableFor3Bar(m_map_name))
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (decvalue > 131070) decvalue = 131070;
                                                //  if (decvalue < 0) decvalue = 0;
                                            }
                                            else
                                            {
                                                if (decvalue > 510) decvalue = 510;
                                                if (decvalue < 0) decvalue = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (m_issixteenbit)
                                            {
                                                if (decvalue > 0xFFFF) decvalue = 0xFFFF;
                                                // if (decvalue < 0) decvalue = 0;
                                            }
                                            else
                                            {
                                                if (decvalue > 0xFF) decvalue = 0xFF;
                                                if (decvalue < 0) decvalue = 0;
                                            }
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, decvalue.ToString());
                                    }

                                }
                                catch (Exception cE)
                                {
                                    Console.WriteLine(cE.Message);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_map_name != string.Empty && !m_prohibitlock_change)
            {
                switch (toolStripComboBox2.SelectedIndex)
                {
                    case 0: // autoscale
                        CastLockEvent(0);
                        break;
                    case 1: // peak values
                        CastLockEvent(1);
                        break;
                    case 2: // max possible
                        CastLockEvent(2);
                        break;
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (this.Parent is DevExpress.XtraBars.Docking.DockPanel)
            {
                DevExpress.XtraBars.Docking.DockPanel pnl = (DevExpress.XtraBars.Docking.DockPanel)this.Parent;
                if (pnl.FloatForm == null)
                {
                    pnl.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    pnl.FloatLocation = new System.Drawing.Point(1, 1);
                    pnl.MakeFloat();
                    // alleen grafiek
                }
                else
                {
                    pnl.Restore();
                }
            }
            else if (this.Parent is DevExpress.XtraBars.Docking.ControlContainer)
            {
                DevExpress.XtraBars.Docking.ControlContainer container = (DevExpress.XtraBars.Docking.ControlContainer)this.Parent;
                if (container.Panel.FloatForm == null)
                {
                    container.Panel.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    container.Panel.FloatLocation = new System.Drawing.Point(1, 1);
                    container.Panel.MakeFloat();
                }
                else
                {
                    container.Panel.Restore();
                }
                
            }
            CastSplitterMovedEvent();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (this.Parent is DevExpress.XtraBars.Docking.DockPanel)
            {
                DevExpress.XtraBars.Docking.DockPanel pnl = (DevExpress.XtraBars.Docking.DockPanel)this.Parent;
                if (pnl.FloatForm == null)
                {
                    pnl.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    pnl.FloatLocation = new System.Drawing.Point(1, 1);
                    pnl.MakeFloat();
                    // alleen grafiek
                }
                else
                {
                    pnl.Restore();
                }
            }
            else if (this.Parent is DevExpress.XtraBars.Docking.ControlContainer)
            {
                DevExpress.XtraBars.Docking.ControlContainer container = (DevExpress.XtraBars.Docking.ControlContainer)this.Parent;
                if (container.Panel.FloatForm == null)
                {
                    container.Panel.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    container.Panel.FloatLocation = new System.Drawing.Point(1, 1);
                    container.Panel.MakeFloat();
                }
                else
                {
                    container.Panel.Restore();
                }

            }
            CastSplitterMovedEvent();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (this.Parent is DevExpress.XtraBars.Docking.DockPanel)
            {
                DevExpress.XtraBars.Docking.DockPanel pnl = (DevExpress.XtraBars.Docking.DockPanel)this.Parent;
                if (pnl.FloatForm == null)
                {
                    pnl.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    pnl.FloatLocation = new System.Drawing.Point(1, 1);
                    pnl.MakeFloat();
                    // alleen grafiek
                }
                else
                {
                    pnl.Restore();
                }
            }
            else if (this.Parent is DevExpress.XtraBars.Docking.ControlContainer)
            {
                DevExpress.XtraBars.Docking.ControlContainer container = (DevExpress.XtraBars.Docking.ControlContainer)this.Parent;
                if (container.Panel.FloatForm == null)
                {
                    container.Panel.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    container.Panel.FloatLocation = new System.Drawing.Point(1, 1);
                    container.Panel.MakeFloat();
                }
                else
                {
                    container.Panel.Restore();
                }

            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            CastSplitterMovedEvent();
        }

        bool tmr_toggle = false;

        private void timer4_Tick(object sender, EventArgs e)
        {
            
        }

        private void popupContainerEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
//            e.Value = System.IO.Path.GetFileName(m_filename) + " : " + m_map_name + " flash address : " + m_map_address.ToString("X6") + " sram address : " + m_map_sramaddress.ToString("X4");
        }

        private void gridView1_SelectionChanged_1(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (!m_prohibitcellchange)
            {
                DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
                if (cellcollection.Length == 1)
                {
                    object o = cellcollection.GetValue(0);
                    if (o is DevExpress.XtraGrid.Views.Base.GridCell)
                    {
                        DevExpress.XtraGrid.Views.Base.GridCell cell = (DevExpress.XtraGrid.Views.Base.GridCell)o;
                        CastSelectEvent(cell.RowHandle, cell.Column.AbsoluteIndex);
                    }

                }
            }
            
        }

        private bool m_split_dragging = false;

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (m_split_dragging)
            {
                m_split_dragging = false;
                CastSplitterMovedEvent();
            }
        }

        private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        {
            m_split_dragging = true;
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
         
        }

        private void splitContainer1_MouseLeave(object sender, EventArgs e)
        {

        }

        
        public override void SetSurfaceGraphViewEx(float depthx, float depthy, float zoom, float rotation, float elevation)
        {
            
        }

        public override void SetSurfaceGraphView(int pov_x, int pov_y, int pov_z, int pan_x, int pan_y, double pov_d)
        {

        }

        private void toolStripComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // refresh the mapviewer with the values like selected
            if (m_prohibit_viewchange) return;
            
            m_viewtype = (Trionic5Tools.ViewType)toolStripComboBox3.SelectedIndex;
            ReShowTable();
            CastViewTypeChangedEvent();
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
           
        }

        private bool MapSupportsNegativeValues(string mapname)
        {
            bool retval = true;
            if (mapname == "Insp_mat!") retval = false;
            else if (mapname == "Inj_map_0!") retval = false;
            else if (mapname == "Fuel_knock_mat!") retval = false;
            return retval;
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (m_issixteenbit)
            {
                if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                {
                    try
                    {
                        int value = Convert.ToInt32(Convert.ToString(e.Value), 16);
                        if (value > 0xFFFF)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                    }
                    catch (Exception hE)
                    {
                        e.Valid = false;
                        e.ErrorText = hE.Message;
                    }
                    /*value = 0xFFFF;
                     e.Value = value.ToString("X4");*/
                }
                else if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                {
                    try
                    {
                        double dvalue = Convert.ToDouble(e.Value);
                        int value = 0;
                        if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                        {
                            if (gridView1.ActiveEditor != null)
                            {
                                if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                                {
                                    Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                    dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                    value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                                }
                                else
                                {
                                    value = Convert.ToInt32(Convert.ToString(e.Value));
                                }
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }

                        if (MapIsScalableFor3Bar(m_map_name))
                        {
                            if (value > 78643)
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }
                            else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }

                            else
                            {
                                e.Value = value;
                            }
                        }
                        else
                        {
                            if (value > 0xFFFF)
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }
                            else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }

                            else
                            {
                                e.Value = value;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    /*value = 78643;
                e.Value = value;*/
                }
                else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                {
                    try
                    {
                        double dvalue = Convert.ToDouble(e.Value);
                        int value = 0;
                        if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                        {
                            if (gridView1.ActiveEditor != null)
                            {
                                if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                                {
                                    Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                    dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                    value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                                }
                                else
                                {
                                    value = Convert.ToInt32(Convert.ToString(e.Value));
                                }
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }

                        if (MapIsScalableFor3Bar(m_map_name))
                        {
                            if (value > 91749)
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }
                            else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }

                            else
                            {
                                e.Value = value;
                            }
                        }
                        else
                        {
                            if (value > 0xFFFF)
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }
                            else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }

                            else
                            {
                                e.Value = value;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    /*value = 78643;
                e.Value = value;*/
                }
                else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                {
                    try
                    {
                        double dvalue = Convert.ToDouble(e.Value);
                        int value = 0;
                        if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                        {
                            if (gridView1.ActiveEditor != null)
                            {
                                if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                                {
                                    Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                    dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                    value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                                }
                                else
                                {
                                    value = Convert.ToInt32(Convert.ToString(e.Value));
                                }
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }

                        if (MapIsScalableFor3Bar(m_map_name))
                        {
                            if (value > 104856)
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }
                            else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }

                            else
                            {
                                e.Value = value;
                            }
                        }
                        else
                        {
                            if (value > 0xFFFF)
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }
                            else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }

                            else
                            {
                                e.Value = value;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    /*value = 78643;
                e.Value = value;*/
                }
                else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                {
                    try
                    {
                        double dvalue = Convert.ToDouble(e.Value);
                        int value = 0;
                        if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                        {
                            if (gridView1.ActiveEditor != null)
                            {
                                if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                                {
                                    Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                    dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                    value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                                }
                                else
                                {
                                    value = Convert.ToInt32(Convert.ToString(e.Value));
                                }
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }

                        if (MapIsScalableFor3Bar(m_map_name))
                        {
                            if (value > 131070)
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }
                            else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }

                            else
                            {
                                e.Value = value;
                            }
                        }
                        else
                        {
                            if (value > 0xFFFF)
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }
                            else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                            {
                                e.Valid = false;
                                e.ErrorText = "Value not valid...";
                            }

                            else
                            {
                                e.Value = value;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    /*value = 78643;
                e.Value = value;*/
                }
                else
                {
                    double dvalue = Convert.ToDouble(e.Value);
                    int value = 0;
                    if (m_viewtype == Trionic5Tools.ViewType.Easy)
                    {
                        if (gridView1.ActiveEditor != null)
                        {
                            if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                            {
                                Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
/*                                if(value < 0)
                                {
                                    value ^= 0xffffff;
                                    value = -value;
                                }*/
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }
                    }
                    else
                    {
                        value = Convert.ToInt32(Convert.ToString(e.Value));
                    }
                    if (Math.Abs(value) > 78643)
                    {
                        e.Valid = false;
                        e.ErrorText = "Value not valid...";
                    }
                    else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                    {
                        e.Valid = false;
                        e.ErrorText = "Value not valid...";
                    }

                    else
                    {
                        e.Value = value;
                    }
                }

            }
            else
            {
                if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
                {
                    try
                    {
                        int value = Convert.ToInt32(Convert.ToString(e.Value), 16);
                        if (value > 0xFF)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                    }
                    catch(Exception hE)
                    {
                        e.Valid = false;
                        e.ErrorText = hE.Message;
                    }
                }
                else if (m_viewtype == Trionic5Tools.ViewType.Decimal3Bar || m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                {
                    //int value = Convert.ToInt32(Convert.ToString(e.Value));
                    double dvalue = Convert.ToDouble(e.Value);
                    int value = 0;
                    if (m_viewtype == Trionic5Tools.ViewType.Easy3Bar)
                    {
                        if (gridView1.ActiveEditor != null)
                        {
                            if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                            {
                                Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }
                    }
                    else
                    {
                        value = Convert.ToInt32(Convert.ToString(e.Value));
                    }

                    if (MapIsScalableFor3Bar(m_map_name))
                    {
                        if (value > 306)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                        else
                        {
                            e.Value = value;
                        }
                    }
                    else
                    {
                        if (value > 0xFF)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                        else
                        {
                            e.Value = value;
                        }
                    }
                }
                else if (m_viewtype == Trionic5Tools.ViewType.Decimal35Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                {
                    //int value = Convert.ToInt32(Convert.ToString(e.Value));
                    double dvalue = Convert.ToDouble(e.Value);
                    int value = 0;
                    if (m_viewtype == Trionic5Tools.ViewType.Easy35Bar)
                    {
                        if (gridView1.ActiveEditor != null)
                        {
                            if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                            {
                                Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }
                    }
                    else
                    {
                        value = Convert.ToInt32(Convert.ToString(e.Value));
                    }

                    if (MapIsScalableFor3Bar(m_map_name))
                    {
                        if (value > 357)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                        else
                        {
                            e.Value = value;
                        }
                    }
                    else
                    {
                        if (value > 0xFF)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                        else
                        {
                            e.Value = value;
                        }
                    }
                }
                else if (m_viewtype == Trionic5Tools.ViewType.Decimal4Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                {
                    //int value = Convert.ToInt32(Convert.ToString(e.Value));
                    double dvalue = Convert.ToDouble(e.Value);
                    int value = 0;
                    if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                    {
                        if (gridView1.ActiveEditor != null)
                        {
                            if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                            {
                                Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }
                    }
                    else
                    {
                        value = Convert.ToInt32(Convert.ToString(e.Value));
                    }

                    if (MapIsScalableFor3Bar(m_map_name))
                    {
                        if (value > 408)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                        else
                        {
                            e.Value = value;
                        }
                    }
                    else
                    {
                        if (value > 0xFF)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                        else
                        {
                            e.Value = value;
                        }
                    }
                }
                else if (m_viewtype == Trionic5Tools.ViewType.Decimal5Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                {
                    //int value = Convert.ToInt32(Convert.ToString(e.Value));
                    double dvalue = Convert.ToDouble(e.Value);
                    int value = 0;
                    if (m_viewtype == Trionic5Tools.ViewType.Easy4Bar)
                    {
                        if (gridView1.ActiveEditor != null)
                        {
                            if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                            {
                                Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }
                    }
                    else
                    {
                        value = Convert.ToInt32(Convert.ToString(e.Value));
                    }

                    if (MapIsScalableFor3Bar(m_map_name))
                    {
                        if (value > 510)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                        else
                        {
                            e.Value = value;
                        }
                    }
                    else
                    {
                        if (value > 0xFF)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                        else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }

                        else
                        {
                            e.Value = value;
                        }
                    }
                }
                else
                {
                    double dvalue = Convert.ToDouble(e.Value);
                    int value = 0;
                    if (m_viewtype == Trionic5Tools.ViewType.Easy)
                    {
                        if (gridView1.ActiveEditor != null)
                        {
                            if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                            {
                                Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }
                    }
                    else
                    {
                        value = Convert.ToInt32(Convert.ToString(e.Value));
                    }

                    if (value > 255)
                    {
                        e.Valid = false;
                        e.ErrorText = "Value not valid...";
                    }
                    else if (value < 0 && !MapSupportsNegativeValues(m_map_name))
                    {
                        e.Valid = false;
                        e.ErrorText = "Value not valid...";
                    }

                    else
                    {
                        e.Value = value;
                    }
                }
            }
        }

        private void popupContainerEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            e.DisplayText = System.IO.Path.GetFileName(m_filename) + " : " + m_map_name + " flash address : " + m_map_address.ToString("X6") + " sram address : " + m_map_sramaddress.ToString("X4");
        }

        private float ConvertToEasyValue(float editorvalue)
        {
            float retval = editorvalue;
            if (m_viewtype == Trionic5Tools.ViewType.Easy || m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
            {
                retval = (float)((float)editorvalue * (float)correction_factor) + (float)correction_offset;
            }
            return retval;
        }

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {
            /*
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName == "CityCode" && view.ActiveEditor is LookUpEdit)
            {
                Text = view.ActiveEditor.Parent.Name;
                DevExpress.XtraEditors.LookUpEdit edit;
                edit = (LookUpEdit)view.ActiveEditor;

                DataTable table = edit.Properties.LookUpData.DataSource as DataTable;
                clone = new DataView(table);
                DataRow row = view.GetDataRow(view.FocusedRowHandle);
                clone.RowFilter = "[CountryCode] = " + row["CountryCode"].ToString();
                edit.Properties.LookUpData.DataSource = clone;
            }
            */
            try
            {
                if (m_viewtype == Trionic5Tools.ViewType.Easy || m_viewtype == Trionic5Tools.ViewType.Easy3Bar || m_viewtype == Trionic5Tools.ViewType.Easy35Bar || m_viewtype == Trionic5Tools.ViewType.Easy4Bar || m_viewtype == Trionic5Tools.ViewType.Easy5Bar)
                {
                    gridView1.ActiveEditor.EditValue = ConvertToEasyValue((float)Convert.ToDouble(gridView1.ActiveEditor.EditValue)).ToString("F2");
                    Console.WriteLine("Started editor with value: " + gridView1.ActiveEditor.EditValue.ToString());
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
        }

        private void gridView1_HiddenEditor(object sender, EventArgs e)
        {
            Console.WriteLine("Hidden editor with value: " + gridView1.GetFocusedRowCellDisplayText(gridView1.FocusedColumn));
        }

        private void MapViewer_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                
            }
        }

        internal void ClearSelection()
        {
            gridView1.ClearSelection();
            
        }

        private int m_selectedrowhandle = -1;
        private int m_selectedcolumnindex = -1;

        internal void HighlightCell(int tpsindex, int rpmindex)
        {
            
          //  gridView1.BeginUpdate();
            try
            {
                int numberofrows = m_map_content.Length / m_TableWidth;
                if (m_issixteenbit)
                {
                    numberofrows /= 2;
                }

                // controleer of het verandert is
/*                DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
                if (cellcollection.Length > 1)
                {
                    gridView1.ClearSelection();
                }
                else if (cellcollection.Length == 1)
                {
                    // normal situation
                    DevExpress.XtraGrid.Views.Base.GridCell cell = (DevExpress.XtraGrid.Views.Base.GridCell)cellcollection.GetValue(0);

                    if (cell.RowHandle != ( (numberofrows - 1) - rpmindex) || cell.Column.AbsoluteIndex != tpsindex)
                    {
                        gridView1.ClearSelection();
                        gridView1.SelectCell((numberofrows - 1) - rpmindex, gridView1.Columns[tpsindex]);
                    }
                }
                else
                {
                    gridView1.SelectCell((numberofrows-1) - rpmindex, gridView1.Columns[tpsindex]);
                }*/
                m_selectedrowhandle = (numberofrows - 1) - rpmindex;
                m_selectedcolumnindex = tpsindex;

                if (m_selectedrowhandle > numberofrows) m_selectedrowhandle = numberofrows;
                if (m_selectedcolumnindex > m_TableWidth) m_selectedcolumnindex = m_TableWidth;

                gridControl1.Invalidate();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
          //  gridView1.EndUpdate();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            SymbolAxesTranslator sat = new SymbolAxesTranslator();
            string x = sat.GetXaxisSymbol(m_map_name);
            string y = sat.GetYaxisSymbol(m_map_name);
            if (x != string.Empty)
            {
                editXaxisSymbolToolStripMenuItem.Enabled = true;
                editXaxisSymbolToolStripMenuItem.Text = "Edit x-axis (" + x + ")";
            }
            else
            {
                editXaxisSymbolToolStripMenuItem.Enabled = false;
                editXaxisSymbolToolStripMenuItem.Text = "Edit x-axis";
            }
            if (y != string.Empty)
            {
                editYaxisSymbolToolStripMenuItem.Enabled = true;
                editYaxisSymbolToolStripMenuItem.Text = "Edit y-axis (" + y + ")";
            }
            else
            {
                editYaxisSymbolToolStripMenuItem.Enabled = false;
                editYaxisSymbolToolStripMenuItem.Text = "Edit y-axis";
            }
            if (m_map_name == "Tryck_mat!" || m_map_name == "Tryck_mat_a!" || m_map_name == "Ign_map_0!" || m_map_name == "Insp_mat!" || m_map_name == "Fuel_knock_mat!" || m_map_name == "Reg_kon_mat!" || m_map_name == "Knock_ref_matrix!")
            {
                exportMapToolStripMenuItem.Visible = true;
            }
            else
            {
                exportMapToolStripMenuItem.Visible = false;
            }
            if (m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR" || m_map_name == "IdleFeedbackAFR" || m_map_name == "IdleFeedbackvsTargetAFR")
            {
                clearDataToolStripMenuItem.Visible = true;
            }
            else
            {
                clearDataToolStripMenuItem.Visible = false;
            }
            if (m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR" || m_map_name == "TargetAFR" || m_map_name == "Insp_mat!" || m_map_name == "Inj_map_0!" || m_map_name == "Ign_map_0!" || m_map_name == "Knock_count_map" || m_map_name == "IdleFeedbackAFR" || m_map_name == "IdleFeedbackvsTargetAFR" || m_map_name == "IdleTargetAFR" || m_map_name == "Idle_fuel_korr!")
            {
                // allow locking/unlocking of cells
                // <GS-29032011>
                lockCellsToolStripMenuItem.Visible = true;
                unlockCellsToolStripMenuItem.Visible = true;
            }
            else
            {
                lockCellsToolStripMenuItem.Visible = false;
                unlockCellsToolStripMenuItem.Visible = false;
            }
        }

        private void editXaxisSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CastEditXaxisEditorRequested();
        }

        private void CastEditXaxisEditorRequested()
        {
            if (onAxisEditorRequested != null)
            {
                onAxisEditorRequested(this, new AxisEditorRequestedEventArgs(AxisIdent.X_Axis, m_map_name, m_filename));
            }
        }

        private void editYaxisSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CastEditYaxisEditorRequested();
        }

        private void CastEditYaxisEditorRequested()
        {
            if (onAxisEditorRequested != null)
            {
                onAxisEditorRequested(this, new AxisEditorRequestedEventArgs(AxisIdent.Y_Axis, m_map_name, m_filename));
            }
        }

        private void CastReadFromSRAM()
        {
            if (onReadFromSRAM != null)
            {
                onReadFromSRAM(this, new ReadFromSRAMEventArgs(m_map_name));
            }
        }

        private void CastWriteToSRAM()
        {
            if (onWriteToSRAM != null)
            {
                onWriteToSRAM(this, new WriteToSRAMEventArgs(m_map_name, GetDataFromGridView(m_isUpsideDown)));
            }
        }

        private void btnSaveToRAM_Click(object sender, EventArgs e)
        {
            // save available data to SRAM
            CastWriteToSRAM();
        }

        private void btnReadFromRAM_Click(object sender, EventArgs e)
        {
            // Reload data from SRAM
            btnReadFromRAM.Enabled = false;
            CastReadFromSRAM();

        }

        private void smoothSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_viewtype == Trionic5Tools.ViewType.Hexadecimal)
            {
                MessageBox.Show("Smoothing cannot be done in Hex view!");
                return;
            }
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            if (cellcollection.Length > 2)
            {
                // get boundaries for this selection
                // we need 4 corners 
                int max_column = 0;
                int min_column = 0xFFFF;
                int max_row = 0;
                int min_row = 0xFFFF;
                foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                {
                    if (cell.Column.AbsoluteIndex > max_column) max_column = cell.Column.AbsoluteIndex;
                    if (cell.Column.AbsoluteIndex < min_column) min_column = cell.Column.AbsoluteIndex;
                    if (cell.RowHandle > max_row) max_row = cell.RowHandle;
                    if (cell.RowHandle < min_row) min_row = cell.RowHandle;
                }
                if (max_column == min_column)
                {
                    // one column selected only
                    int top_value = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[max_column]));
                    int bottom_value = Convert.ToInt32(gridView1.GetRowCellValue(min_row, gridView1.Columns[max_column]));
                    double diffvalue = (top_value - bottom_value) / (cellcollection.Length-1);
                    for (int t = 1; t < cellcollection.Length-1; t++)
                    {
                        double newvalue = bottom_value + (t * diffvalue);
                        gridView1.SetRowCellValue(min_row + t, gridView1.Columns[max_column], newvalue);
                    }

                }
                else if (max_row == min_row)
                {
                    // one row selected only
                    int top_value = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[max_column]));
                    int bottom_value = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[min_column]));
                    double diffvalue = (top_value - bottom_value) / (cellcollection.Length - 1);
                    for (int t = 1; t < cellcollection.Length - 1; t++)
                    {
                        double newvalue = bottom_value + (t * diffvalue);
                        gridView1.SetRowCellValue(min_row, gridView1.Columns[min_column + t], newvalue);
                    }
                }
                else
                {
                    // block selected
                    // interpolation on 4 points!!!
                    int top_leftvalue = Convert.ToInt32(gridView1.GetRowCellValue(min_row, gridView1.Columns[min_column]));
                    int top_rightvalue = Convert.ToInt32(gridView1.GetRowCellValue(min_row, gridView1.Columns[max_column]));
                    int bottom_leftvalue = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[min_column]));
                    int bottom_rightvalue = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[max_column]));

                    for (int tely = 1; tely < max_row - min_row; tely++)
                    {
                        for (int telx = 1; telx < max_column - min_column; telx++)
                        {
                            // get values 
                            double valx1 = 0;
                            double valx2 = 0;
                            double valy1 = 0;
                            double valy2 = 0;

                            if (telx+min_column > 0)
                            {
                                valx1 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[telx + min_column -1]));
                            }
                            else
                            {
                                valx1 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[min_column]));
                            }
                            if ((telx+min_column) < gridView1.Columns.Count - 1)
                            {
                                valx2 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[telx + min_column + 1]));
                            }
                            else
                            {
                                valx2 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[telx + min_column]));
                            }

                            if (tely+min_row > 0)
                            {
                                valy1 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row - 1, gridView1.Columns[telx + min_column]));
                            }
                            else
                            {
                                valy1 = Convert.ToDouble(gridView1.GetRowCellValue(min_row, gridView1.Columns[telx + min_column]));
                            }
                            if ((tely+min_row) < gridView1.RowCount - 1)
                            {
                                valy2 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row + 1, gridView1.Columns[telx + min_column]));
                            }
                            else
                            {
                                valy2 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[telx + min_column]));
                            }
                            //Console.WriteLine("valx1 = " + valx1.ToString() + " valx2 = " + valx2.ToString() + " valy1 = " + valy1.ToString() + " valy2 = " + valy2.ToString());
                            // x as 
                            double valuex = (valx2 + valx1) / 2;
                            double valuey = (valy2 + valy1) / 2;
                            float newvalue = (float)((valuex + valuey) / 2);

                            gridView1.SetRowCellValue(min_row + tely, gridView1.Columns[min_column + telx], newvalue.ToString("F0"));

                            m_map_content = GetDataFromGridView(m_isUpsideDown);
                            
                            

                            //double diffvaluex = (top_rightvalue - top_leftvalue) / (max_column - min_column - 1);
                            //double diffvaluey = (top_rightvalue - top_leftvalue) / (max_column - min_column - 1);
                        }
                    }

                }
                simpleButton3.Enabled = false;
            }
        }

        private void groupControl1_MouseHover(object sender, EventArgs e)
        {
            
            
        }


        private void ShowHitInfo(GridHitInfo hi)
        {
            if (hi.InRowCell)
            {

                if (afr_counter != null)
                {
                    // fetch correct counter
                    int current_afrcounter = (int)afr_counter[(afr_counter.Length - ((hi.RowHandle + 1) * m_TableWidth)) + hi.Column.AbsoluteIndex];
                    // show number of measurements in balloon
                    string detailline = "# measurements: " + current_afrcounter.ToString();
                    toolTipController1.ShowHint(detailline, "Information", Cursor.Position);
                }
            }
            else
            {
                toolTipController1.HideHint();
            }
        }

        private void groupControl1_MouseMove(object sender, MouseEventArgs e)
        {
            //gridControl1.GetViewAt(MousePosition);
            /*if (m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR")
            {
                ShowHitInfo(gridView1.CalcHitInfo(new Point(e.X, e.Y)));
            }*/
        }

        private void gridView1_MouseMove(object sender, MouseEventArgs e)
        {
            //gridControl1.GetViewAt(MousePosition);
            if (m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR" || m_map_name == "IdleTargetAFR" || m_map_name == "IdleFeedbackAFR" || m_map_name == "IdleFeedbackvsTargetAFR")
            {
                ShowHitInfo(gridView1.CalcHitInfo(new Point(e.X, e.Y)));
            }
        }


        internal void UpdateSelectedCells(int value)
        {
            if(value == 1) gridView1_KeyDown(this, new KeyEventArgs(Keys.Add));        
            else if(value == 10) gridView1_KeyDown(this, new KeyEventArgs(Keys.PageUp));        
            else if(value == -1) gridView1_KeyDown(this, new KeyEventArgs(Keys.Subtract));        
            else if(value == -10) gridView1_KeyDown(this, new KeyEventArgs(Keys.PageDown));        
        }

        private void ExportMapToDashBoard()
        {
            // export data with a setting name
            frmMapName mapdescr = new frmMapName();
            if (mapdescr.ShowDialog() == DialogResult.OK)
            {
                // export the map
                SaveFileDialog sfd = new SaveFileDialog();
                if (m_map_name == "Ign_map_0!")
                {
                    sfd.Filter = "Ignition map settings|*.ims";
                    sfd.DefaultExt = "ims";
                }
                else if (m_map_name == "Knock_ref_matrix!")
                {
                    sfd.Filter = "Knock sensitivity maps|*.krm";
                    sfd.DefaultExt = "krm";
                }
                else if (m_map_name == "Insp_mat!")
                {
                    sfd.Filter = "Fuel map settings|*.fms";
                    sfd.DefaultExt = "fms";
                }
                else if (m_map_name == "Fuel_knock_mat!")
                {
                    sfd.Filter = "Fuel knock map settings|*.kms";
                    sfd.DefaultExt = "kms";
                }
                else if (m_map_name == "Reg_kon_mat!")
                {
                    sfd.Filter = "Regulation map settings|*.rms";
                    sfd.DefaultExt = "rms";
                }
                else
                {
                    sfd.Filter = "Boost map settings|*.bms";
                    sfd.DefaultExt = "bms";
                }
                sfd.AddExtension = true;
                sfd.FileName = mapdescr.MapDescription;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // write the mapdata to the given filename
                    byte[] mutateddata = GetDataFromGridView(m_isUpsideDown);
                    int numberrows = (int)(m_map_length / m_TableWidth);

                    if (m_issixteenbit)
                    {
                        numberrows /= 2;
                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false))
                        {
                            // first write mapname into file
                            sw.WriteLine(m_map_name);
                            // write description into file
                            sw.WriteLine(mapdescr.MapDescription);
                            // next a line with max boost request?
                            sw.WriteLine("00");
                            // then all data
                            for (int rowcount = 0; rowcount < numberrows; rowcount++)
                            {
                                for (int colcount = 0; colcount < m_TableWidth * 2; colcount += 2)
                                {
                                    byte b1 = (byte)mutateddata.GetValue((rowcount * m_TableWidth * 2) + colcount);
                                    byte b2 = (byte)mutateddata.GetValue((rowcount * m_TableWidth * 2) + colcount + 1);
                                    int value = (int)b1 * 256 + (int)b2;
                                    sw.Write(value.ToString("X4") + ",");
                                }
                                sw.WriteLine("");
                            }
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false))
                        {
                            // first write mapname into file
                            sw.WriteLine(m_map_name);
                            // write description into file
                            sw.WriteLine(mapdescr.MapDescription);
                            // next a line with max boost request?
                            byte bmax = 0;
                            foreach (byte testb in mutateddata)
                            {
                                if (testb > bmax) bmax = testb;
                            }
                            sw.WriteLine(bmax.ToString("X2"));
                            // then all data
                            for (int rowcount = 0; rowcount < numberrows; rowcount++)
                            {
                                for (int colcount = 0; colcount < m_TableWidth; colcount++)
                                {
                                    byte b = (byte)mutateddata.GetValue((rowcount * m_TableWidth) + colcount);
                                    sw.Write(b.ToString("X2") + ",");
                                }
                                sw.WriteLine("");
                            }
                        }
                    }

                }
            }
        }

        private void asPreferredSettingInT5DashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportMapToDashBoard();   
        }

        /*private void TryToLaunchInputScreen()
        {
            if (File.Exists(Application.StartupPath + "\\T5Softkeys.exe"))
            {
                // launch it
                Process.Start(Application.StartupPath + "\\T5Softkeys.exe");

            }
        }*/
        public override void LoadSymbol(string symbolname, IECUFile trionic_file, string sramfile)
        {
            m_trionic_file = trionic_file;
            //Set correctly
            this.IsUpsideDown = true; // always?
            foreach (SymbolHelper sh in m_trionic_file.GetFileInfo().SymbolCollection)
            {
                if (sh.Varname == symbolname)
                {
                    // get data from it
                    IECUFile file = new Trionic5File();
                    file.SetAutoUpdateChecksum(m_autoUpdateChecksum);
                    file.SelectFile(m_trionic_file.GetFileInfo().Filename);
                    //byte[] symboldata = file.ReadData((uint)sh.Flash_start_address, (uint)sh.Length);
                    byte[] symboldata = file.ReadDataFromFile(sramfile, (uint)sh.Start_address, (uint)sh.Length);

                    //byte[] symboldata = file.readdatafromfile(m_trionic_file.GetFileInfo().Filename, sh.Flash_start_address, sh.Length);
                    this.Map_content = symboldata;
                    this.Map_length = symboldata.Length;
                    this.Filename = sramfile;// m_trionic_file.GetFileInfo().Filename;
                    if (m_trionic_file.IsTableSixteenBits(symbolname))
                    {
                        //this.Map_length /= 2;
                    }
                    this.Map_name = symbolname;
                    this.Correction_factor = m_trionic_file.GetCorrectionFactorForMap(symbolname);
                    this.correction_offset = m_trionic_file.GetOffsetForMap(symbolname);
                    //Set correctly
                    this.SetViewSize(ViewSize.NormalView);
                    //this.Viewtype = Trionic5Tools.ViewType.Easy;
                    // set axis information
                    SymbolAxesTranslator sat = new SymbolAxesTranslator();
                    sat.GetXaxisSymbol(symbolname);
                    sat.GetYaxisSymbol(symbolname);
                    this.X_axisvalues = m_trionic_file.GetMapXaxisValues(symbolname);
                    this.Y_axisvalues = m_trionic_file.GetMapYaxisValues(symbolname);
                    string x = string.Empty;
                    string y = string.Empty;
                    string z = string.Empty;

                    m_trionic_file.GetMapAxisDescriptions(symbolname, out x, out y, out z);

                    this.X_axis_name = x;
                    this.Y_axis_name = y;
                    this.Z_axis_name = z;
                    int columns = 1;
                    int rows = 1;
                    m_trionic_file.GetMapMatrixWitdhByName(symbolname, out columns, out rows);
                    this.ShowTable(columns, m_trionic_file.IsTableSixteenBits(symbolname));

                    break;
                }
            }
        }

        public override void LoadSymbol(string symbolname, IECUFile trionic_file)
        {
            // autonomous
            m_trionic_file = trionic_file;
            m_trionic_file.LibraryPath = Application.StartupPath + "\\Binaries";
            this.IsUpsideDown = true; // always?
            foreach (SymbolHelper sh in m_trionic_file.GetFileInfo().SymbolCollection)
            {
                if (sh.Varname == symbolname)
                {
                    // get data from it
                    IECUFile file = new Trionic5File();
                    file.LibraryPath = Application.StartupPath + "\\Binaries";
                    file.SetAutoUpdateChecksum(m_autoUpdateChecksum);
                    file.SelectFile(m_trionic_file.GetFileInfo().Filename);
                    byte[] symboldata = file.ReadData((uint)sh.Flash_start_address, (uint)sh.Length);
                    //byte[] symboldata = file.readdatafromfile(m_trionic_file.GetFileInfo().Filename, sh.Flash_start_address, sh.Length);
                    this.Map_content = symboldata;
                    this.Map_length = symboldata.Length;
                    this.Filename = m_trionic_file.GetFileInfo().Filename;
                    if (m_trionic_file.IsTableSixteenBits(symbolname))
                    {
                        //this.Map_length /= 2;
                    }
                    this.Map_name = symbolname;
                    this.Correction_factor = m_trionic_file.GetCorrectionFactorForMap(symbolname);
                    this.correction_offset = m_trionic_file.GetOffsetForMap(symbolname);
                    this.SetViewSize(ViewSize.NormalView);
                    //this.Viewtype = Trionic5Tools.ViewType.Easy;
                    // set axis information
                    SymbolAxesTranslator sat = new SymbolAxesTranslator();
                    sat.GetXaxisSymbol(symbolname);
                    sat.GetYaxisSymbol(symbolname);
                    this.X_axisvalues = m_trionic_file.GetMapXaxisValues(symbolname);
                    this.Y_axisvalues = m_trionic_file.GetMapYaxisValues(symbolname);
                    string x = string.Empty;
                    string y = string.Empty;
                    string z = string.Empty;

                    m_trionic_file.GetMapAxisDescriptions(symbolname, out x, out y, out z);

                    this.X_axis_name = x;
                    this.Y_axis_name = y;
                    this.Z_axis_name = z;
                    int columns = 1;
                    int rows = 1;
                    m_trionic_file.GetMapMatrixWitdhByName(symbolname, out columns, out rows);
                    this.ShowTable(columns, m_trionic_file.IsTableSixteenBits(symbolname));

                    break;
                }
            }
            
        }

        public override int DetermineWidth()
        {
            return 600;
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            CastCloseEvent();
        }

        private void toolStripComboBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // see if there's a selection being made
                gridView1.ClearSelection();
                string strValue = toolStripComboBox3.Text;
                char[] sep = new char[1];
                sep.SetValue(' ', 0);
                string[] strValues = strValue.Split(sep);
                foreach (string strval in strValues)
                {
                    double dblres;
                    if (Double.TryParse(strval, out dblres))
                    {
                        SelectCellsWithValue(dblres);
                    }
                }
                gridView1.Focus();

            }
        }

        private void SelectCellsWithValue(double value)
        {
            for (int rh = 0; rh < gridView1.RowCount; rh++)
            {
                for (int ch = 0; ch < gridView1.Columns.Count; ch++)
                {
                    try
                    {
                        object ov = gridView1.GetRowCellValue(rh, gridView1.Columns[ch]);


                        double val = Convert.ToDouble(ov);
                        val *= correction_factor;
                        val += correction_offset;
                        double diff = Math.Abs(val - value);
                        if (diff < 0.009)
                        {
                            gridView1.SelectCell(rh, gridView1.Columns[ch]);
                        }

                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to select cell: " + E.Message);
                    }

                }
            }
        }

        private void clearDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // clear data if it is a feedback afr map
            m_clearData = true;
            CastSaveEvent();
        }

        private void lockCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // lock cells (depends on type of map loaded)
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();

            foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
            {
                // cast cell locked event
                CastCellLockEvent(gc.RowHandle, gc.Column.AbsoluteIndex, true);
            }
        }

        private void CastCellLockEvent(int rowindex, int columnindex, bool locked)
        {
            if (onCellLocked != null)
            {
                onCellLocked(this, new CellLockedEventArgs(rowindex, columnindex, locked));
            }
        }

        private void unlockCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // unlock cells (depends on type of map loaded)
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();

            foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
            {
                // cast cell unlocked event
                CastCellLockEvent(gc.RowHandle, gc.Column.AbsoluteIndex, false);
            }
        }
    }
}
