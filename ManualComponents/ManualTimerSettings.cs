﻿using LiveSplit.UI;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Voxif.AutoSplitter {
    public partial class ManualTimerSettings : UserControl {
        public float TimerHeight { get; set; }
        public float TimerWidth { get; set; }

        public float DecimalsSize { get; set; }

        public string DigitsFormat { get; set; }
        public string Accuracy { get; set; }
        private string TimerFormat {
            get => DigitsFormat + Accuracy;
            set {
                var decimalIndex = value.IndexOf('.');
                if(decimalIndex < 0) {
                    DigitsFormat = value;
                    Accuracy = "";
                } else {
                    DigitsFormat = value.Substring(0, decimalIndex);
                    Accuracy = value.Substring(decimalIndex);
                }
            }
        }

        public LayoutMode Mode { get; set; }

        public Color TimerColor { get; set; }
        public bool OverrideSplitColors { get; set; }

        public bool CenterTimer { get; set; }

        public bool ShowGradient { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public string GradientString {
            get { return GetBackgroundTypeString(BackgroundGradient); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value.Replace(" ", "")); }
        }

        public ManualTimerSettings() {
            InitializeComponent();

            TimerWidth = 225;
            TimerHeight = 50;
            TimerFormat = "1.23";
            TimerColor = Color.FromArgb(170, 170, 170);
            OverrideSplitColors = false;
            ShowGradient = true;
            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = GradientType.Plain;
            CenterTimer = false;
            DecimalsSize = 35f;

            btnTimerColor.DataBindings.Add("BackColor", this, "TimerColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTimerColors.DataBindings.Add("Checked", this, "OverrideSplitColors", false, DataSourceUpdateMode.OnPropertyChanged);
            chkGradient.DataBindings.Add("Checked", this, "ShowGradient", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkCenterTimer.DataBindings.Add("Checked", this, "CenterTimer", false, DataSourceUpdateMode.OnPropertyChanged);
            trkDecimalsSize.DataBindings.Add("Value", this, "DecimalsSize", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbDigitsFormat.DataBindings.Add("SelectedItem", this, "DigitsFormat", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbAccuracy.DataBindings.Add("SelectedItem", this, "Accuracy", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void CmbTimerFormat_SelectedIndexChanged(object sender, EventArgs e) {
            DigitsFormat = cmbDigitsFormat.SelectedItem.ToString();
        }

        private void CmbAccuracy_SelectedIndexChanged(object sender, EventArgs e) {
            Accuracy = cmbAccuracy.SelectedItem.ToString();
        }

        void ChkOverrideTimerColors_CheckedChanged(object sender, EventArgs e) {
            label1.Enabled = btnTimerColor.Enabled = chkOverrideTimerColors.Checked;
        }

        void CmbGradientType_SelectedIndexChanged(object sender, EventArgs e) {
            var selectedText = cmbGradientType.SelectedItem.ToString();
            btnColor1.Visible = selectedText != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        public static string GetBackgroundTypeString(GradientType type) {
            switch(type) {
                case GradientType.Horizontal:
                    return "Horizontal";
                case GradientType.Vertical:
                    return "Vertical";
                case GradientType.Plain:
                default:
                    return "Plain";
            }
        }

        void TimerSettings_Load(object sender, EventArgs e) {
            ChkOverrideTimerColors_CheckedChanged(null, null);

            if(Mode == LayoutMode.Horizontal) {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 50;
                trkSize.Maximum = 500;
                trkSize.DataBindings.Add("Value", this, "TimerWidth", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSize.Text = "Width:";
            } else {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 20;
                trkSize.Maximum = 150;
                trkSize.DataBindings.Add("Value", this, "TimerHeight", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSize.Text = "Height:";
            }
        }

        public void SetSettings(XmlNode node) {
            var element = (XmlElement)node;
            TimerHeight = SettingsHelper.ParseFloat(element["TimerHeight"]);
            TimerWidth = SettingsHelper.ParseFloat(element["TimerWidth"]);
            ShowGradient = SettingsHelper.ParseBool(element["ShowGradient"], true);
            TimerColor = SettingsHelper.ParseColor(element["TimerColor"], Color.FromArgb(170, 170, 170));
            DecimalsSize = SettingsHelper.ParseFloat(element["DecimalsSize"], 35f);
            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"], Color.Transparent);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"], Color.Transparent);
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"], GradientType.Plain.ToString());
            CenterTimer = SettingsHelper.ParseBool(element["CenterTimer"], false);
            OverrideSplitColors = SettingsHelper.ParseBool(element["OverrideSplitColors"]);
            TimerFormat = SettingsHelper.ParseString(element["TimerFormat"]);
        }

        public XmlNode GetSettings(XmlDocument document) {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent) {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.0") ^
            SettingsHelper.CreateSetting(document, parent, "TimerHeight", TimerHeight) ^
            SettingsHelper.CreateSetting(document, parent, "TimerWidth", TimerWidth) ^
            SettingsHelper.CreateSetting(document, parent, "TimerFormat", TimerFormat) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideSplitColors", OverrideSplitColors) ^
            SettingsHelper.CreateSetting(document, parent, "ShowGradient", ShowGradient) ^
            SettingsHelper.CreateSetting(document, parent, "TimerColor", TimerColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
            SettingsHelper.CreateSetting(document, parent, "CenterTimer", CenterTimer) ^
            SettingsHelper.CreateSetting(document, parent, "DecimalsSize", DecimalsSize);
        }

        public int GetSettingsHashCode() => CreateSettingsNode(null, null);

        private void ColorButtonClick(object sender, EventArgs e) => SettingsHelper.ColorButtonClick((Button)sender, this);
    }
}