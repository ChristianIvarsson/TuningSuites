using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;

namespace T5Suite2
{
    public partial class AxisBrowser : DevExpress.XtraEditors.XtraUserControl
    {
        public delegate void StartSymbolViewer(object sender, SymbolViewerRequestedEventArgs e);
        public event AxisBrowser.StartSymbolViewer onStartSymbolViewer;

        public class SymbolViewerRequestedEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }


            public SymbolViewerRequestedEventArgs(string mapname)
            {
                this._mapname = mapname;
            }
        }


        private int m_globalLanguageID = 44;

        public int GlobalLanguageID
        {
            get { return m_globalLanguageID; }
            set { m_globalLanguageID = value; }
        }

        public AxisBrowser()
        {
            InitializeComponent();
        }

        public void ShowSymbolCollection(SymbolCollection sc)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SYMBOLNAME");
            dt.Columns.Add("DESCRIPTION");
            dt.Columns.Add("XAXIS");
            dt.Columns.Add("XAXISDESCRIPTION");
            dt.Columns.Add("YAXIS");
            dt.Columns.Add("YAXISDESCRIPTION");
            SymbolAxesTranslator sat = new SymbolAxesTranslator();
            SymbolTranslator symtrans = new SymbolTranslator();
            string helptext = string.Empty;
            XDFCategories cat = XDFCategories.Undocumented;
            XDFSubCategory subcat = XDFSubCategory.Undocumented;
            foreach (SymbolHelper sh in sc)
            {
                string xaxis = sat.GetXaxisSymbol(sh.Varname);
                string yaxis = sat.GetYaxisSymbol(sh.Varname);
                string symboldescr = symtrans.TranslateSymbolToHelpText(sh.Varname, out helptext, out cat, out subcat);
                string xaxisdescr = "";
                string yaxisdescr = "";
                if (xaxis != "")
                {
                    xaxisdescr = symtrans.TranslateSymbolToHelpText(xaxis, out helptext, out cat, out subcat);
                }
                if (yaxis != "")
                {
                    yaxisdescr = symtrans.TranslateSymbolToHelpText(yaxis, out helptext, out cat, out subcat);
                }
                if (xaxis != "" || yaxis != "")
                {
                    dt.Rows.Add(sh.Varname, symboldescr, xaxis, xaxisdescr, yaxis, yaxisdescr);
                }
            }
            gridControl1.DataSource = dt;
        }

        public void SetCurrentSymbol(string symbolname)
        {
            if (symbolname == "")
            {
                ClearFilters();
            }
            else
            {
                SetDefaultFilters(symbolname);
            }
        }
        private void ClearFilters()
        {
            gridView1.ActiveFilterEnabled = false;
        }

        private void SetDefaultFilters(string symbolname)
        {
            DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(@"([SYMBOLNAME] = '" + symbolname + "')", "Symbol:" + symbolname);
            gridView1.ActiveFilter.Clear();
            gridView1.ActiveFilter.Add(gcBrowseSymbolName, fltr);
            gridView1.ActiveFilterEnabled = true;
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView obj = gridControl1.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = obj.CalcHitInfo(gridControl1.PointToClient(Cursor.Position));

            // valid info?
            if (!(hi.IsValid && hi.InRowCell)) return;

            // is symbol (symbol or axis) column?
            if (!(obj.FocusedColumn.FieldName == "SYMBOLNAME" | obj.FocusedColumn.FieldName == "XAXIS" | obj.FocusedColumn.FieldName == "YAXIS")) return;

            // check for null ref.
            if (obj.FocusedValue == null | string.IsNullOrEmpty(obj.FocusedValue.ToString())) return;

            //TheMainForm.StartTableViewer(obj.FocusedValue.ToString().Trim());
            //Cast and event to start the viewer
            if (onStartSymbolViewer != null)
            {
                onStartSymbolViewer(this, new SymbolViewerRequestedEventArgs(obj.FocusedValue.ToString().Trim()));
            }

        }

    }
}
