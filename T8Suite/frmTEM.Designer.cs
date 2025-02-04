﻿namespace T8SuitePro
{
    partial class frmTEM
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
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cIdx = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cTEM = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cSymbolSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cSymbolAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cSymbolType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cSymbol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(926, 456);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.cIdx,
            this.cTEM,
            this.cSymbolSize,
            this.cSymbolAddress,
            this.cSymbolType,
            this.cSymbol,
            this.cDescription});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsFind.AlwaysVisible = true;
            this.gridView1.OptionsFind.ShowCloseButton = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            // 
            // cIdx
            // 
            this.cIdx.Caption = " ";
            this.cIdx.FieldName = "Index";
            this.cIdx.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.cIdx.Name = "cIdx";
            this.cIdx.OptionsColumn.AllowEdit = false;
            this.cIdx.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.cIdx.OptionsColumn.ReadOnly = true;
            this.cIdx.Visible = true;
            this.cIdx.VisibleIndex = 0;
            this.cIdx.Width = 26;
            // 
            // cTEM
            // 
            this.cTEM.Caption = "Label";
            this.cTEM.FieldName = "PID";
            this.cTEM.Name = "cTEM";
            this.cTEM.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.cTEM.Visible = true;
            this.cTEM.VisibleIndex = 1;
            this.cTEM.Width = 59;
            // 
            // cSymbolSize
            // 
            this.cSymbolSize.Caption = "Size";
            this.cSymbolSize.FieldName = "SymbolSize";
            this.cSymbolSize.Name = "cSymbolSize";
            this.cSymbolSize.OptionsColumn.AllowEdit = false;
            this.cSymbolSize.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.cSymbolSize.OptionsColumn.ReadOnly = true;
            this.cSymbolSize.Visible = true;
            this.cSymbolSize.VisibleIndex = 6;
            this.cSymbolSize.Width = 45;
            // 
            // cSymbolAddress
            // 
            this.cSymbolAddress.Caption = "Address";
            this.cSymbolAddress.FieldName = "SymbolAddress";
            this.cSymbolAddress.Name = "cSymbolAddress";
            this.cSymbolAddress.OptionsColumn.AllowEdit = false;
            this.cSymbolAddress.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.cSymbolAddress.OptionsColumn.ReadOnly = true;
            this.cSymbolAddress.Visible = true;
            this.cSymbolAddress.VisibleIndex = 5;
            this.cSymbolAddress.Width = 66;
            // 
            // cSymbolType
            // 
            this.cSymbolType.Caption = "Type";
            this.cSymbolType.FieldName = "SymbolType";
            this.cSymbolType.Name = "cSymbolType";
            this.cSymbolType.OptionsColumn.AllowEdit = false;
            this.cSymbolType.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.cSymbolType.OptionsColumn.ReadOnly = true;
            this.cSymbolType.Visible = true;
            this.cSymbolType.VisibleIndex = 4;
            this.cSymbolType.Width = 41;
            // 
            // cSymbol
            // 
            this.cSymbol.Caption = "Symbol";
            this.cSymbol.FieldName = "SymbolIndex";
            this.cSymbol.Name = "cSymbol";
            this.cSymbol.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.cSymbol.Visible = true;
            this.cSymbol.VisibleIndex = 2;
            this.cSymbol.Width = 187;
            // 
            // cDescription
            // 
            this.cDescription.Caption = "Description";
            this.cDescription.FieldName = "SymbolDescription";
            this.cDescription.Name = "cDescription";
            this.cDescription.OptionsColumn.AllowEdit = false;
            this.cDescription.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.cDescription.OptionsColumn.ReadOnly = true;
            this.cDescription.Visible = true;
            this.cDescription.VisibleIndex = 3;
            this.cDescription.Width = 518;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton1.Location = new System.Drawing.Point(842, 462);
            this.simpleButton1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "Ok";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.simpleButton2.Location = new System.Drawing.Point(761, 462);
            this.simpleButton2.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 2;
            this.simpleButton2.Text = "Cancel";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // frmTEM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.simpleButton2;
            this.ClientSize = new System.Drawing.Size(926, 494);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.gridControl1);
            this.Name = "frmTEM";
            this.ShowIcon = false;
            this.Text = "TEM editor";
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraGrid.Columns.GridColumn cIdx;
        private DevExpress.XtraGrid.Columns.GridColumn cTEM;
        private DevExpress.XtraGrid.Columns.GridColumn cSymbol;
        private DevExpress.XtraGrid.Columns.GridColumn cDescription;
        private DevExpress.XtraGrid.Columns.GridColumn cSymbolSize;
        private DevExpress.XtraGrid.Columns.GridColumn cSymbolAddress;
        private DevExpress.XtraGrid.Columns.GridColumn cSymbolType;
    }
}