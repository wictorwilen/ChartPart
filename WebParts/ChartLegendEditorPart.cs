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
    public class ChartLegendEditorPart : BaseEditorPart {
        CheckBox m_legend;
        DropDownList m_legendPos;
        DropDownList m_legendStyle;
        TextBox m_title;
        TextBox m_titleFontSize;
        TextBox m_legendFontSize;
        CheckBox m_showValue;

        public ChartLegendEditorPart()
            : base(false) {
            this.Title = Localization.Translate("Legend");
        }
        public override string EditorName {
            get { return "_ChartLegendEditorPart"; }
        }

        public override bool IsVisible(WebPart webPart) {
            if (webPart != null) {
                ChartPartWebPart wp = webPart as ChartPartWebPart;
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
            m_showValue = new CheckBox();
            m_title = CreateEditorPartTextBox();
            m_legendPos = new DropDownList();
            foreach (string s in Enum.GetNames(typeof(Docking))) {
                m_legendPos.Items.Add(s);
            }
            m_legendStyle = new DropDownList();
            foreach (string s in Enum.GetNames(typeof(LegendStyle))) {
                m_legendStyle.Items.Add(s);
            }
            m_legend = new CheckBox();
            m_legend.AutoPostBack = true;

            m_titleFontSize = CreateEditorPartTextBox(70);
            m_titleFontSize.ID = "titleFontSize";
            RangeValidator rv1 = new RangeValidator();
            rv1.ControlToValidate = m_titleFontSize.ID;
            rv1.Type = ValidationDataType.Integer;
            rv1.MinimumValue = "1";
            rv1.MaximumValue = "100";
            rv1.ErrorMessage = String.Format(" {0}", Localization.Translate("InvalidValue"));

            m_legendFontSize= CreateEditorPartTextBox(70);
            m_legendFontSize.ID = "legendFontSize";
            RangeValidator rv2 = new RangeValidator();
            rv2.ControlToValidate = m_legendFontSize.ID;
            rv2.Type = ValidationDataType.Integer;
            rv2.MinimumValue = "1";
            rv2.MaximumValue = "100";
            rv2.ErrorMessage = String.Format(" {0}", Localization.Translate("InvalidValue"));

            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_showValue, Localization.Translate("ShowValueLabel"))));
            AddToolPaneRow(CreateToolPaneSeparator());
            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_legend, Localization.Translate("ShowLegend"))));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("Title"), new Control[] { m_title }));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("LegendPosition"), new Control[] { m_legendPos }));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("LegendStyle"), new Control[] { m_legendStyle }));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("TitleFontSz"), new Control[] { m_titleFontSize, new LiteralControl("pt "), rv1 }));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("FontSz"), new Control[] { m_legendFontSize, new LiteralControl("pt "), rv2 }));


        }

        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
            m_legendPos.Enabled = m_legend.Checked;
            m_legendStyle.Enabled = m_legend.Checked;
            m_title.Enabled = m_legend.Checked;
        }

        public override void SyncChanges() {
            EnsureChildControls();
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {
                m_legend.Checked = chartPart.ShowLegend;
                m_legendPos.SelectedValue = chartPart.LegendPosition.ToString();
                m_legendStyle.SelectedValue = chartPart.LegendStyle.ToString();
                m_title.Text = chartPart.LegendTitle;
                m_legendFontSize.Text = chartPart.LegendFontSize.ToString();
                m_titleFontSize.Text = chartPart.LegendTitleFontSize.ToString();
                m_showValue.Checked = chartPart.ShowValueAsLabel;
            }
        }
        public override bool ApplyChanges() {
            EnsureChildControls();
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {
                chartPart.ShowLegend = m_legend.Checked;
                chartPart.LegendPosition = (Docking)Enum.Parse(typeof(Docking), m_legendPos.SelectedValue);
                chartPart.LegendStyle = (LegendStyle)Enum.Parse(typeof(LegendStyle), m_legendStyle.SelectedValue);
                chartPart.LegendTitle = m_title.Text;
                chartPart.LegendTitleFontSize = int.Parse(m_titleFontSize.Text);
                chartPart.LegendFontSize = int.Parse(m_legendFontSize.Text);
                chartPart.ShowValueAsLabel = m_showValue.Checked;
            }
            return true;
        }

    }
}
