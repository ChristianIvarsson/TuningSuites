namespace Trionic5Controls
{
    partial class frmPartnumberLookup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPartnumberLookup));
            this.buttonEdit1 = new DevExpress.XtraEditors.ButtonEdit();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.lblStageIII = new DevExpress.XtraEditors.LabelControl();
            this.lblStageII = new DevExpress.XtraEditors.LabelControl();
            this.lblStageI = new DevExpress.XtraEditors.LabelControl();
            this.lblTorque = new DevExpress.XtraEditors.LabelControl();
            this.lblPower = new DevExpress.XtraEditors.LabelControl();
            this.lblMaxBoostAUT = new DevExpress.XtraEditors.LabelControl();
            this.lblMaxBoostManual = new DevExpress.XtraEditors.LabelControl();
            this.lblBaseBoost = new DevExpress.XtraEditors.LabelControl();
            this.lblEngineType = new DevExpress.XtraEditors.LabelControl();
            this.lblCarModel = new DevExpress.XtraEditors.LabelControl();
            this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.checkEdit5 = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.checkEdit4 = new DevExpress.XtraEditors.CheckEdit();
            this.checkEdit3 = new DevExpress.XtraEditors.CheckEdit();
            this.checkEdit2 = new DevExpress.XtraEditors.CheckEdit();
            this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.buttonEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit5.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonEdit1
            // 
            resources.ApplyResources(this.buttonEdit1, "buttonEdit1");
            this.buttonEdit1.BackgroundImage = null;
            this.buttonEdit1.EditValue = null;
            this.buttonEdit1.Name = "buttonEdit1";
            this.buttonEdit1.Properties.AccessibleDescription = null;
            this.buttonEdit1.Properties.AccessibleName = null;
            this.buttonEdit1.Properties.AutoHeight = ((bool)(resources.GetObject("buttonEdit1.Properties.AutoHeight")));
            this.buttonEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("buttonEdit1.Properties.Buttons"))), ((System.Drawing.Image)(resources.GetObject("buttonEdit1.Properties.Buttons1"))), null)});
            this.buttonEdit1.Properties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("buttonEdit1.Properties.Mask.AutoComplete")));
            this.buttonEdit1.Properties.Mask.BeepOnError = ((bool)(resources.GetObject("buttonEdit1.Properties.Mask.BeepOnError")));
            this.buttonEdit1.Properties.Mask.EditMask = resources.GetString("buttonEdit1.Properties.Mask.EditMask");
            this.buttonEdit1.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("buttonEdit1.Properties.Mask.IgnoreMaskBlank")));
            this.buttonEdit1.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("buttonEdit1.Properties.Mask.MaskType")));
            this.buttonEdit1.Properties.Mask.PlaceHolder = ((char)(resources.GetObject("buttonEdit1.Properties.Mask.PlaceHolder")));
            this.buttonEdit1.Properties.Mask.SaveLiteral = ((bool)(resources.GetObject("buttonEdit1.Properties.Mask.SaveLiteral")));
            this.buttonEdit1.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("buttonEdit1.Properties.Mask.ShowPlaceHolders")));
            this.buttonEdit1.Properties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("buttonEdit1.Properties.Mask.UseMaskAsDisplayFormat")));
            this.buttonEdit1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit1_ButtonClick);
            this.buttonEdit1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.buttonEdit1_KeyDown);
            // 
            // groupControl1
            // 
            this.groupControl1.AccessibleDescription = null;
            this.groupControl1.AccessibleName = null;
            resources.ApplyResources(this.groupControl1, "groupControl1");
            this.groupControl1.Controls.Add(this.lblStageIII);
            this.groupControl1.Controls.Add(this.lblStageII);
            this.groupControl1.Controls.Add(this.lblStageI);
            this.groupControl1.Controls.Add(this.lblTorque);
            this.groupControl1.Controls.Add(this.lblPower);
            this.groupControl1.Controls.Add(this.lblMaxBoostAUT);
            this.groupControl1.Controls.Add(this.lblMaxBoostManual);
            this.groupControl1.Controls.Add(this.lblBaseBoost);
            this.groupControl1.Controls.Add(this.lblEngineType);
            this.groupControl1.Controls.Add(this.lblCarModel);
            this.groupControl1.Controls.Add(this.labelControl11);
            this.groupControl1.Controls.Add(this.labelControl10);
            this.groupControl1.Controls.Add(this.labelControl9);
            this.groupControl1.Controls.Add(this.labelControl8);
            this.groupControl1.Controls.Add(this.labelControl7);
            this.groupControl1.Controls.Add(this.labelControl6);
            this.groupControl1.Controls.Add(this.labelControl5);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.checkEdit5);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.checkEdit4);
            this.groupControl1.Controls.Add(this.checkEdit3);
            this.groupControl1.Controls.Add(this.checkEdit2);
            this.groupControl1.Controls.Add(this.checkEdit1);
            this.groupControl1.Name = "groupControl1";
            // 
            // lblStageIII
            // 
            this.lblStageIII.AccessibleDescription = null;
            this.lblStageIII.AccessibleName = null;
            resources.ApplyResources(this.lblStageIII, "lblStageIII");
            this.lblStageIII.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblStageIII.Appearance.Options.UseForeColor = true;
            this.lblStageIII.Name = "lblStageIII";
            // 
            // lblStageII
            // 
            this.lblStageII.AccessibleDescription = null;
            this.lblStageII.AccessibleName = null;
            resources.ApplyResources(this.lblStageII, "lblStageII");
            this.lblStageII.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblStageII.Appearance.Options.UseForeColor = true;
            this.lblStageII.Name = "lblStageII";
            // 
            // lblStageI
            // 
            this.lblStageI.AccessibleDescription = null;
            this.lblStageI.AccessibleName = null;
            resources.ApplyResources(this.lblStageI, "lblStageI");
            this.lblStageI.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblStageI.Appearance.Options.UseForeColor = true;
            this.lblStageI.Name = "lblStageI";
            // 
            // lblTorque
            // 
            this.lblTorque.AccessibleDescription = null;
            this.lblTorque.AccessibleName = null;
            resources.ApplyResources(this.lblTorque, "lblTorque");
            this.lblTorque.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblTorque.Appearance.Options.UseForeColor = true;
            this.lblTorque.Name = "lblTorque";
            // 
            // lblPower
            // 
            this.lblPower.AccessibleDescription = null;
            this.lblPower.AccessibleName = null;
            resources.ApplyResources(this.lblPower, "lblPower");
            this.lblPower.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPower.Appearance.Options.UseForeColor = true;
            this.lblPower.Name = "lblPower";
            // 
            // lblMaxBoostAUT
            // 
            this.lblMaxBoostAUT.AccessibleDescription = null;
            this.lblMaxBoostAUT.AccessibleName = null;
            resources.ApplyResources(this.lblMaxBoostAUT, "lblMaxBoostAUT");
            this.lblMaxBoostAUT.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblMaxBoostAUT.Appearance.Options.UseForeColor = true;
            this.lblMaxBoostAUT.Name = "lblMaxBoostAUT";
            // 
            // lblMaxBoostManual
            // 
            this.lblMaxBoostManual.AccessibleDescription = null;
            this.lblMaxBoostManual.AccessibleName = null;
            resources.ApplyResources(this.lblMaxBoostManual, "lblMaxBoostManual");
            this.lblMaxBoostManual.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblMaxBoostManual.Appearance.Options.UseForeColor = true;
            this.lblMaxBoostManual.Name = "lblMaxBoostManual";
            // 
            // lblBaseBoost
            // 
            this.lblBaseBoost.AccessibleDescription = null;
            this.lblBaseBoost.AccessibleName = null;
            resources.ApplyResources(this.lblBaseBoost, "lblBaseBoost");
            this.lblBaseBoost.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblBaseBoost.Appearance.Options.UseForeColor = true;
            this.lblBaseBoost.Name = "lblBaseBoost";
            // 
            // lblEngineType
            // 
            this.lblEngineType.AccessibleDescription = null;
            this.lblEngineType.AccessibleName = null;
            resources.ApplyResources(this.lblEngineType, "lblEngineType");
            this.lblEngineType.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblEngineType.Appearance.Options.UseForeColor = true;
            this.lblEngineType.Name = "lblEngineType";
            // 
            // lblCarModel
            // 
            this.lblCarModel.AccessibleDescription = null;
            this.lblCarModel.AccessibleName = null;
            resources.ApplyResources(this.lblCarModel, "lblCarModel");
            this.lblCarModel.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblCarModel.Appearance.Options.UseForeColor = true;
            this.lblCarModel.Name = "lblCarModel";
            // 
            // labelControl11
            // 
            this.labelControl11.AccessibleDescription = null;
            this.labelControl11.AccessibleName = null;
            resources.ApplyResources(this.labelControl11, "labelControl11");
            this.labelControl11.Name = "labelControl11";
            // 
            // labelControl10
            // 
            this.labelControl10.AccessibleDescription = null;
            this.labelControl10.AccessibleName = null;
            resources.ApplyResources(this.labelControl10, "labelControl10");
            this.labelControl10.Name = "labelControl10";
            // 
            // labelControl9
            // 
            this.labelControl9.AccessibleDescription = null;
            this.labelControl9.AccessibleName = null;
            resources.ApplyResources(this.labelControl9, "labelControl9");
            this.labelControl9.Name = "labelControl9";
            // 
            // labelControl8
            // 
            this.labelControl8.AccessibleDescription = null;
            this.labelControl8.AccessibleName = null;
            resources.ApplyResources(this.labelControl8, "labelControl8");
            this.labelControl8.Name = "labelControl8";
            // 
            // labelControl7
            // 
            this.labelControl7.AccessibleDescription = null;
            this.labelControl7.AccessibleName = null;
            resources.ApplyResources(this.labelControl7, "labelControl7");
            this.labelControl7.Name = "labelControl7";
            // 
            // labelControl6
            // 
            this.labelControl6.AccessibleDescription = null;
            this.labelControl6.AccessibleName = null;
            resources.ApplyResources(this.labelControl6, "labelControl6");
            this.labelControl6.Name = "labelControl6";
            // 
            // labelControl5
            // 
            this.labelControl5.AccessibleDescription = null;
            this.labelControl5.AccessibleName = null;
            resources.ApplyResources(this.labelControl5, "labelControl5");
            this.labelControl5.Name = "labelControl5";
            // 
            // labelControl4
            // 
            this.labelControl4.AccessibleDescription = null;
            this.labelControl4.AccessibleName = null;
            resources.ApplyResources(this.labelControl4, "labelControl4");
            this.labelControl4.Name = "labelControl4";
            // 
            // checkEdit5
            // 
            resources.ApplyResources(this.checkEdit5, "checkEdit5");
            this.checkEdit5.BackgroundImage = null;
            this.checkEdit5.Name = "checkEdit5";
            this.checkEdit5.Properties.AccessibleDescription = null;
            this.checkEdit5.Properties.AccessibleName = null;
            this.checkEdit5.Properties.AutoHeight = ((bool)(resources.GetObject("checkEdit5.Properties.AutoHeight")));
            this.checkEdit5.Properties.Caption = resources.GetString("checkEdit5.Properties.Caption");
            // 
            // labelControl3
            // 
            this.labelControl3.AccessibleDescription = null;
            this.labelControl3.AccessibleName = null;
            resources.ApplyResources(this.labelControl3, "labelControl3");
            this.labelControl3.Name = "labelControl3";
            // 
            // labelControl2
            // 
            this.labelControl2.AccessibleDescription = null;
            this.labelControl2.AccessibleName = null;
            resources.ApplyResources(this.labelControl2, "labelControl2");
            this.labelControl2.Name = "labelControl2";
            // 
            // checkEdit4
            // 
            resources.ApplyResources(this.checkEdit4, "checkEdit4");
            this.checkEdit4.BackgroundImage = null;
            this.checkEdit4.Name = "checkEdit4";
            this.checkEdit4.Properties.AccessibleDescription = null;
            this.checkEdit4.Properties.AccessibleName = null;
            this.checkEdit4.Properties.AutoHeight = ((bool)(resources.GetObject("checkEdit4.Properties.AutoHeight")));
            this.checkEdit4.Properties.Caption = resources.GetString("checkEdit4.Properties.Caption");
            // 
            // checkEdit3
            // 
            resources.ApplyResources(this.checkEdit3, "checkEdit3");
            this.checkEdit3.BackgroundImage = null;
            this.checkEdit3.Name = "checkEdit3";
            this.checkEdit3.Properties.AccessibleDescription = null;
            this.checkEdit3.Properties.AccessibleName = null;
            this.checkEdit3.Properties.AutoHeight = ((bool)(resources.GetObject("checkEdit3.Properties.AutoHeight")));
            this.checkEdit3.Properties.Caption = resources.GetString("checkEdit3.Properties.Caption");
            // 
            // checkEdit2
            // 
            resources.ApplyResources(this.checkEdit2, "checkEdit2");
            this.checkEdit2.BackgroundImage = null;
            this.checkEdit2.Name = "checkEdit2";
            this.checkEdit2.Properties.AccessibleDescription = null;
            this.checkEdit2.Properties.AccessibleName = null;
            this.checkEdit2.Properties.AutoHeight = ((bool)(resources.GetObject("checkEdit2.Properties.AutoHeight")));
            this.checkEdit2.Properties.Caption = resources.GetString("checkEdit2.Properties.Caption");
            // 
            // checkEdit1
            // 
            resources.ApplyResources(this.checkEdit1, "checkEdit1");
            this.checkEdit1.BackgroundImage = null;
            this.checkEdit1.Name = "checkEdit1";
            this.checkEdit1.Properties.AccessibleDescription = null;
            this.checkEdit1.Properties.AccessibleName = null;
            this.checkEdit1.Properties.AutoHeight = ((bool)(resources.GetObject("checkEdit1.Properties.AutoHeight")));
            this.checkEdit1.Properties.Caption = resources.GetString("checkEdit1.Properties.Caption");
            // 
            // labelControl1
            // 
            this.labelControl1.AccessibleDescription = null;
            this.labelControl1.AccessibleName = null;
            resources.ApplyResources(this.labelControl1, "labelControl1");
            this.labelControl1.Name = "labelControl1";
            // 
            // simpleButton1
            // 
            this.simpleButton1.AccessibleDescription = null;
            this.simpleButton1.AccessibleName = null;
            resources.ApplyResources(this.simpleButton1, "simpleButton1");
            this.simpleButton1.BackgroundImage = null;
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.AccessibleDescription = null;
            this.simpleButton2.AccessibleName = null;
            resources.ApplyResources(this.simpleButton2, "simpleButton2");
            this.simpleButton2.BackgroundImage = null;
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton3
            // 
            this.simpleButton3.AccessibleDescription = null;
            this.simpleButton3.AccessibleName = null;
            resources.ApplyResources(this.simpleButton3, "simpleButton3");
            this.simpleButton3.BackgroundImage = null;
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
            // 
            // frmPartnumberLookup
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.simpleButton3);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.buttonEdit1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPartnumberLookup";
            ((System.ComponentModel.ISupportInitialize)(this.buttonEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit5.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ButtonEdit buttonEdit1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit checkEdit4;
        private DevExpress.XtraEditors.CheckEdit checkEdit3;
        private DevExpress.XtraEditors.CheckEdit checkEdit2;
        private DevExpress.XtraEditors.CheckEdit checkEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.CheckEdit checkEdit5;
        private DevExpress.XtraEditors.LabelControl labelControl11;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl lblStageIII;
        private DevExpress.XtraEditors.LabelControl lblStageII;
        private DevExpress.XtraEditors.LabelControl lblStageI;
        private DevExpress.XtraEditors.LabelControl lblTorque;
        private DevExpress.XtraEditors.LabelControl lblPower;
        private DevExpress.XtraEditors.LabelControl lblMaxBoostAUT;
        private DevExpress.XtraEditors.LabelControl lblMaxBoostManual;
        private DevExpress.XtraEditors.LabelControl lblBaseBoost;
        private DevExpress.XtraEditors.LabelControl lblEngineType;
        private DevExpress.XtraEditors.LabelControl lblCarModel;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
    }
}