/*
 * 
 * ChartPart for SharePoint
 * ------------------------------------------
 * Copyright (c) 2008-2009, Wictor Wilén
 * http://www.codeplex.com/ChartPart/
 * http://www.wictorwilen.se/
 * ------------------------------------------
 * Licensed under the Microsoft Public License (Ms-PL) 
 * http://www.opensource.org/licenses/ms-pl.html
 * 
 */
using System.Web.UI.WebControls;
using System;
using System.Web.UI;
using System.Globalization;

using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls.WebParts;
namespace ChartPart {
    public class ChartStyleEditorPart: BaseEditorPart {
        DropDownList m_styles;
        CheckBox m_border;
        TextBox m_width;
        TextBox m_height;
        DropDownList m_borderstyle;
        DropDownList m_borderlinestyle;
        TextBox m_borderwidth;
        TextBox m_bordecolor;
        DropDownList m_palette;
        CheckBox m_useCustomPalette;
        TextBox m_customColors;
        TextBox m_titleFontSize;


        bool m_lockDown;
        
        
        public ChartStyleEditorPart(): base(false) {
            this.Title = Localization.Translate("Style");
        }
        /// <summary>
        /// Initializes a new instance of the ChartStyleEditorPart class.
        /// </summary>
        protected ChartStyleEditorPart(string id)
            : base(id) {
            
        }
        /// <summary>
        /// Initializes a new instance of the ChartStyleEditorPart class.
        /// </summary>
        protected ChartStyleEditorPart(bool sharedModeOnly)
            : base(sharedModeOnly) {
            
        }
         
        public override string EditorName {
            get { return "_ChartStyleEditorPart"; }
        }

        public override bool IsVisible(WebPart webpart) {
            if (webpart != null) {
                ChartPartWebPart wp = webpart as ChartPartWebPart;
                if (wp != null) {
                    if ((wp.LockDownMode & LockDownModes.ThreeD) == LockDownModes.ThreeD) {
                        return false;
                    }
                }
            }
            return true;
        }


        protected override void FillEditorPanel() {
            CreateToolPaneTable();
            m_styles = new DropDownList();
            Array.ForEach(Enum.GetNames(typeof(DrawingStyle)), m_styles.Items.Add);

            m_border = new CheckBox();
            m_border.AutoPostBack = true;
            m_borderstyle = new DropDownList();
            Array.ForEach(Enum.GetNames(typeof(BorderSkinStyle)), m_borderstyle.Items.Add);
            m_borderlinestyle = new DropDownList();
            Array.ForEach(Enum.GetNames(typeof(ChartDashStyle)), m_borderlinestyle.Items.Add);
            m_borderwidth = CreateEditorPartTextBox();
            m_bordecolor = CreateEditorPartTextBox();
            m_width = CreateEditorPartTextBox();
            m_height = CreateEditorPartTextBox();

            m_palette = new DropDownList();
            Array.ForEach(Enum.GetNames(typeof(ChartColorPalette)), m_palette.Items.Add);
            m_useCustomPalette = new CheckBox();
            m_useCustomPalette.AutoPostBack = true;

            m_customColors = new TextBox();

            m_titleFontSize = CreateEditorPartTextBox(70);
            m_titleFontSize.ID = "titleFontSize";
            RangeValidator rv2 = new RangeValidator { ControlToValidate = m_titleFontSize.ID, Type = ValidationDataType.Integer, MinimumValue = "1", MaximumValue = "100", ErrorMessage = "Invalid value" };

            
            
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("Style"), new Control[] { m_styles }));
            if (!m_lockDown) {
                AddToolPaneRow(CreateToolPaneSeparator());
                AddToolPaneRow(CreateToolPaneRow(Localization.Translate("TitleFontSz"), new Control[] { m_titleFontSize, new LiteralControl("pt "), rv2 }));
                AddToolPaneRow(CreateToolPaneSeparator());
                AddToolPaneRow(CreateToolPaneRow(Localization.Translate("Palette"), Localization.Translate("PaletteDesc"), new Control[] { m_palette }));
                AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_useCustomPalette, Localization.Translate("CustomPalette"), Localization.Translate("CustomPaletteDesc"))));
                AddToolPaneRow(CreateToolPaneRow(Localization.Translate("CustomPaletteValues"), Localization.Translate("CustomPaletteValuesDesc"), new Control[] { m_customColors }));
                
            }
            AddToolPaneRow(CreateToolPaneSeparator());
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("Height"), Localization.Translate("HeightDesc"), new Control[] { m_height }));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("Width"), Localization.Translate("WidthDesc"), new Control[] { m_width }));
            if (!m_lockDown) {
                AddToolPaneRow(CreateToolPaneSeparator());
                AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_border, Localization.Translate("ChartBorder"), Localization.Translate("ChartBorderDesc"))));
                AddToolPaneRow(CreateToolPaneRow(Localization.Translate("BorderStyle"), new Control[] { m_borderstyle }));
                AddToolPaneRow(CreateToolPaneRow(Localization.Translate("BorderLine"), new Control[] { m_borderlinestyle }));
                AddToolPaneRow(CreateToolPaneRow(Localization.Translate("BorderWidth"), new Control[] { m_borderwidth }));
                AddToolPaneRow(CreateToolPaneRow(Localization.Translate("BorderColor"), new Control[] { m_bordecolor }));
            }


        }

        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
            m_borderstyle.Enabled = m_border.Checked;
            m_borderwidth.Enabled = m_border.Checked;
            m_borderstyle.Enabled = m_border.Checked;
            m_bordecolor.Enabled = m_border.Checked;
            m_borderlinestyle.Enabled = m_border.Checked;
            m_customColors.Enabled = m_useCustomPalette.Checked;
            m_palette.Enabled = !m_useCustomPalette.Checked;
        }

        public override void SyncChanges() {
          EnsureChildControls();
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {
                m_width.Text = chartPart.ChartWidth.ToString(CultureInfo.CurrentCulture);
                m_height.Text = chartPart.ChartHeight.ToString(CultureInfo.CurrentCulture);
                m_border.Checked = chartPart.ChartBorder;
                m_styles.SelectedValue = chartPart.DrawingStyle.ToString();
                m_borderstyle.SelectedValue = chartPart.ChartBorderStyle.ToString();
                m_bordecolor.Text = chartPart.ChartBorderColor;
                m_borderlinestyle.SelectedValue = chartPart.ChartBorderLineStyle.ToString();
                m_borderlinestyle.Enabled = m_border.Checked;
                m_borderwidth.Text = chartPart.ChartBorderWidth.ToString();
                m_palette.SelectedValue = chartPart.Palette.ToString();
                m_useCustomPalette.Checked = chartPart.CustomPalette;
                m_customColors.Text = chartPart.CustomPaletteValues;
                m_titleFontSize.Text = chartPart.TitleFontSize.ToString();
            }
        }
        public override bool ApplyChanges() {
            EnsureChildControls();
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {
                chartPart.ChartWidth = Convert.ToInt32(m_width.Text);
                chartPart.ChartHeight = Convert.ToInt32(m_height.Text);
                chartPart.ChartBorder = m_border.Checked;
                chartPart.ChartBorderColor = m_bordecolor.Text;
                chartPart.ChartBorderWidth = Convert.ToInt32(m_borderwidth.Text);
                chartPart.ChartBorderLineStyle = (ChartDashStyle)Enum.Parse(typeof(ChartDashStyle), m_borderlinestyle.SelectedValue);
                chartPart.ChartBorderStyle = (BorderSkinStyle)Enum.Parse(typeof(BorderSkinStyle), m_borderstyle.SelectedValue);
                chartPart.DrawingStyle = (DrawingStyle)Enum.Parse(typeof(DrawingStyle), m_styles.SelectedValue);
                chartPart.Palette = (ChartColorPalette)Enum.Parse(typeof(ChartColorPalette), m_palette.SelectedValue);
                chartPart.CustomPalette = m_useCustomPalette.Checked;
                chartPart.CustomPaletteValues = m_customColors.Text;
                chartPart.TitleFontSize = Convert.ToInt32(m_titleFontSize.Text);
            }
            return true;
        }

    }
}
