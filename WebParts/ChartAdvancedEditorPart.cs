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
using System;
using System.Web.UI.WebControls;


namespace ChartPart {
    public class ChartAdvancedEditorPart: BaseEditorPart {
        CheckBox m_linkToSourceList;
        CheckBox m_treatAsZero;
        CheckBox m_logarithmic;
        CheckBox m_multipleCharts;
        CheckBox m_includeColumnInTooltip;
        CheckBox m_displayColumnValueInToolTip;

        public ChartAdvancedEditorPart():base(false) {
            this.Title = Localization.Translate("AdvancedChartSettings");
            this.ChromeState = System.Web.UI.WebControls.WebParts.PartChromeState.Minimized;
        }
        public override string EditorName {
            get { return "_ChartAdvEditorPart"; }
        }

        protected override void FillEditorPanel() {
            CreateToolPaneTable();
            m_linkToSourceList = new CheckBox();
            m_treatAsZero = new CheckBox();
            m_logarithmic = new CheckBox();
            m_multipleCharts = new CheckBox();
            m_includeColumnInTooltip = new CheckBox();
            m_displayColumnValueInToolTip = new CheckBox();

            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_linkToSourceList, Localization.Translate("LinkToSourceList"), Localization.Translate("LinkToSourceListDesc"))));
            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_treatAsZero, Localization.Translate("TreatAsZero"), Localization.Translate("TreatAsZeroDesc"))));
            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_logarithmic, Localization.Translate("Logarithmic"), Localization.Translate("UseLogarithmic"))));
            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_multipleCharts, Localization.Translate("MultipleCharts"), Localization.Translate("MultipleChartsDesc"))));
            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_includeColumnInTooltip, Localization.Translate("ColNameTooltip"), Localization.Translate("ColNameTooltipDesc"))));
            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_displayColumnValueInToolTip, Localization.Translate("ColValueTooltip"), Localization.Translate("ColValueTooltipDesc"))));
        }
        public override void SyncChanges() {
            EnsureChildControls();
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {
                m_linkToSourceList.Checked = chartPart.LinkToSourceList;                
                m_treatAsZero.Checked = chartPart.TreatAsZero;
                m_logarithmic.Checked = chartPart.Logarithmic;
                m_multipleCharts.Checked = chartPart.MultipleCharts;
                m_includeColumnInTooltip.Checked = chartPart.ColumnNameInTooltip;
                m_displayColumnValueInToolTip.Checked = chartPart.ColumnValueInTooltip;
            }
        }
        public override bool ApplyChanges() {
            EnsureChildControls();
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {
                chartPart.LinkToSourceList =m_linkToSourceList.Checked;
                chartPart.TreatAsZero = m_treatAsZero.Checked;
                chartPart.Logarithmic = m_logarithmic.Checked;
                chartPart.MultipleCharts = m_multipleCharts.Checked;
                chartPart.ColumnNameInTooltip = m_includeColumnInTooltip.Checked;
                chartPart.ColumnValueInTooltip = m_displayColumnValueInToolTip.Checked;
            }
            return true; 
        }
    }
}
