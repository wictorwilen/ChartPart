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
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.DataVisualization.Charting;
using System.ComponentModel;
using Microsoft.SharePoint.Security;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;

namespace ChartPart {
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ChartPart runat=server></{0}:ChartPart>")]
    [XmlRoot(Namespace = "ChartPart")]
    [Guid("16da55e8-106a-49d9-b807-30544fa41f56")]
    [SupportsAttributeMarkup(true)]
    public class ChartPartWebPart : BaseWebPart<ChartPartEditorParts>{


        public ChartPartWebPart() {
        }

        protected Chart m_chart;

        protected override void CreateChildControls() {
            m_chart = new Chart();
            if (this.ChartHeight > 0) {
                m_chart.Height = this.ChartHeight > 1024 ? 1024 : this.ChartHeight;
            }

            if (this.ChartWidth > 0) {
                m_chart.Width = this.ChartWidth > 1024 ? 1024 : this.ChartWidth;
            }

            m_chart.Page = this.Page;


            m_chart.ImageType = ChartImageType.Png;
            m_chart.ImageStorageMode = ImageStorageMode.UseHttpHandler;
            m_chart.RenderType = RenderType.ImageTag;
            
            
            this.Controls.Add(m_chart);
            base.CreateChildControls();



        }

        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        protected override void Render(HtmlTextWriter writer) {
            if (string.IsNullOrEmpty(SiteUrl)) {
                RenderError(writer, CreateErrorControl(Localization.Translate("MissingSite"), true));
                return;
            }
            if (this.ListId == Guid.Empty) {
                RenderError(writer, CreateErrorControl(Localization.Translate("MissingList"), true));
                return;
            }
            if (this.ViewId == Guid.Empty) {
                RenderError(writer, CreateErrorControl(Localization.Translate("MissingView"), true));
                return;
            }
            if (this.XAxisSourceColumns.Count == 0) {
                RenderError(writer, CreateErrorControl(Localization.Translate("MissingXAxis"), true));
                return;
            }
            if (this.YAxisSourceColumns.Count == 0) {
                RenderError(writer, CreateErrorControl(Localization.Translate("MissingYAxis"), true));
                return;
            }
            try {

                GenerateChart();

                if (this.CustomPalette) {
                    m_chart.Palette = ChartColorPalette.None;

                    List<string> sColors = new List<string>(this.CustomPaletteValues.Split(','));
                    try {
                        List<Color> colors = sColors.ConvertAll<Color>(s => (Color)(new ColorConverter().ConvertFromString(s)));
                        m_chart.PaletteCustomColors = colors.ToArray();
                    }
                    catch (Exception) {
                        RenderError(writer, CreateErrorControl(Localization.Translate("ColorParseError"), true));
                    }


                }
                else {
                    m_chart.Palette = this.Palette;
                }

                m_chart.RenderControl(writer);
            }
#if !DEBUG
            catch (System.Web.HttpException) {
                writer.WriteEncodedText("Could not generate the chart, please reload the page");
            }
#endif
            catch (FileNotFoundException)
            {
                RenderError(writer, CreateErrorControl(Localization.Translate("MissingSite"), true));
                return;
            }
            catch (Exception ex) {
                writer.WriteEncodedText(Localization.Translate("ExceptionOccurred") + ex.ToString());
            }
        }


        private static void addPoints(Dictionary<string, double> data, ChartArea chartArea, Series series, SPField yField, string key) {
            if (yField.FieldValueType == typeof(DateTime)) {
                series.XValueType = ChartValueType.DateTime;
                series.Points.AddXY(DateTime.Parse(key, CultureInfo.CurrentCulture), data[key]);
            }
            else if (yField.FieldValueType == typeof(SPFieldUserValue)) {
                series.XValueType = ChartValueType.String;

                series.Points.AddXY(key.Substring(key.IndexOf(";#") + 2), data[key]);

            }
            else if (yField.FieldValueType == typeof(SPFieldCalculated)) {
                series.XValueType = ChartValueType.String;
                series.Points.AddXY(key.Remove(0, (key.IndexOf("#") + 1)), data[key]);
            }

            else if (yField.FieldValueType == typeof(string) || yField.Type == SPFieldType.Computed) {
                series.XValueType = ChartValueType.String;
                chartArea.AxisX.Interval = 1;
                series.Points.AddXY(key, data[key]);
            }
            else if (yField.FieldValueType == null) {
                series.Points.AddXY(key, data[key]);
            }

            else {
                series.Points.Add(data[key]);
            }
        }
        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        protected void GenerateChart() {
            if (!string.IsNullOrEmpty(this.ChartTitle)) {
                m_chart.Titles.Add(new Title(this.ChartTitle, 
                                       Docking.Top, 
                                       new Font("Microsoft Sans Serif",(float)
                                                                               this.TitleFontSize <= 0 ? 12 : this.TitleFontSize, 
                                                                               FontStyle.Bold), Color.Black));
            }

            Dictionary<string, double> data = new Dictionary<string, double>();

            ChartArea chartArea;
            if (!createMultipleCharts())
                chartArea = CreateChartArea("Default");
            else
                chartArea = null;

            m_chart.AntiAliasing = AntiAliasingStyles.All;

            using (SPSite site = new SPSite(this.SiteUrl)) {
                using (SPWeb web = site.OpenWeb()) {

                    SPList list = web.Lists[this.ListId];
                    SPView view = list.Views[this.ViewId];

                    for (int x = 0; x < XAxisSourceColumns.Count; x++) {
                        Series series = new Series(String.Format(Thread.CurrentThread.CurrentUICulture,"series_{0}", x));

                        if (createMultipleCharts()) {
                            chartArea = CreateChartArea(x.ToString());
                           
                            /*if (x != 0) {
                                chartArea.AlignmentOrientation = AreaAlignmentOrientations.Horizontal;
                                chartArea.AlignmentStyle = AreaAlignmentStyles.Position;
                                chartArea.AlignWithChartArea = (x - 1).ToString();
                            }*/
                            series.ChartArea = x.ToString();
                            
                        }
                        else {
                            series.ChartArea = "Default";
                        }
                        
                        
                        series["DrawingStyle"] = this.DrawingStyle.ToString();
                        if (this.LinkToSourceList) {
                            series.Url = String.Format(Thread.CurrentThread.CurrentUICulture,"{0}/{1}", web.Url, view.Url);
                        }
                        if (this.XAxisSourceColumns[x] == "**count**") {
                            series.ToolTip = Localization.Translate("Count");
                        }
                        else {
                            series.ToolTip = list.Fields.GetFieldByInternalName(this.XAxisSourceColumns[x]).Title;
                        }

                        series.ChartType = this.ChartType;
                        series.BorderWidth = 3;
                        series.ShadowOffset = 2;
                        series.Name = series.ToolTip;
                        SPField yField = list.Fields.GetFieldByInternalName(this.YAxisSourceColumns[0]);
                        m_chart.Series.Add(series);
                        switch (this.ChartType) {
                            case SeriesChartType.Funnel:
                                series["FunnelStyle"] = "YIsHeight";
                                series["FunnelLabelStyle"] = "OutsideInColumn";
                                series["FunnelOutsideLabelPlacement"] = "Right";
                                series["FunnelPointGap"] = "0";
                                series["FunnelMinPointHeight"] = "1";                                
                                break;
                            case SeriesChartType.Pyramid:
                                series["PyramidLabelStyle"] = "OutsideInColumn";
                                series["PyramidOutsideLabelPlacement"] = "Right";
                                series["PyramidPointGap"] = "0";
                                series["PyramidMinPointHeight"] = "0";

                                break;
                            default:
                                break;
                        }
                        series.IsValueShownAsLabel = this.ShowValueAsLabel;
                        if (_provider != null && _table != null) {
                            if (_table.Columns[this.YAxisSourceColumns[0]] == null) {
                                throw new ApplicationException(Localization.Translate("YAxisMissing"));
                            }
                            
                            foreach (DataRow row in _table.Rows) {

                                if (row.IsNull(this.YAxisSourceColumns[0])) {
                                    continue;
                                }

                                string yName = row[this.YAxisSourceColumns[0]].ToString();
                                switch (yField.Type) {
                                    case SPFieldType.Calculated:
                                        yName = yName.Remove(0, (yName.IndexOf("#") + 1));
                                        break;

                                    default:
                                        break;
                                }

                                if (!data.ContainsKey(row[this.YAxisSourceColumns[0]].ToString())) {
                                    data.Add(yName, 0);
                                }

                                if (this.XAxisSourceColumns[x] == "**count**") {
                                    data[yName] += 1;
                                }
                                else {

                                    if (row[this.XAxisSourceColumns[x]] == null) {
                                        continue;
                                    }
                                    else {
                                        // value is not null
                                        SPField xField = list.Fields.GetFieldByInternalName(this.XAxisSourceColumns[x]);
                                        if (xField.Type == SPFieldType.Calculated) {
                                            string tmp = row[this.XAxisSourceColumns[x]].ToString();
                                            tmp = tmp.Remove(0, (tmp.IndexOf("#") + 1));
                                            data[yName] += float.Parse(tmp, new CultureInfo("en-us"));

                                        }
                                        else {
                                            data[yName] += double.Parse(row[this.XAxisSourceColumns[x]].ToString(), CultureInfo.CurrentCulture);
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            foreach (SPListItem item in list.GetItems(view)) {
                                if (item[this.YAxisSourceColumns[0]] == null)
                                    continue;
                                string yName = item[this.YAxisSourceColumns[0]].ToString();
                                switch(yField.Type){
                                    case SPFieldType.Calculated:
                                        yName = yName.Remove(0,(yName.IndexOf("#")+1));
                                        break;

                                    default:
                                        break;
                                }
                                

                                if(yField.Type == SPFieldType.Calculated) {

                                }
                                //set initial value to 0
                                if (!data.ContainsKey(yName)) {
                                    data.Add(yName, 0);
                                }

                                if (this.XAxisSourceColumns[x] == "**count**") {
                                    data[yName] += 1;
                                }
                                else {

                                    if (item[this.XAxisSourceColumns[x]] == null) {
                                        continue;
                                    }
                                    else {
                                        // value is not null
                                        SPField xField = list.Fields.GetFieldByInternalName(this.XAxisSourceColumns[x]);
                                        if (xField.Type == SPFieldType.Calculated) {
                                            string tmp = item[this.XAxisSourceColumns[x]].ToString();
                                            tmp = tmp.Remove(0, (tmp.IndexOf("#") + 1));
                                            data[yName] += float.Parse(tmp, new CultureInfo("en-us"));

                                        }
                                        else {
                                            data[yName] += double.Parse(item[this.XAxisSourceColumns[x]].ToString(), CultureInfo.CurrentCulture);
                                        }
                                    }
                                }
                            }
                        }

                        foreach (string key in data.Keys) {
                            addPoints(data, chartArea, series, yField, key);
                        }
                        foreach (DataPoint point in series.Points) {
                            if (this.ColumnNameInTooltip) {
                                if (this.ColumnValueInTooltip) {
                                    switch (series.XValueType) {
                                            // todo: detailed tooltip instead...
                                        case ChartValueType.DateTime:
                                        case ChartValueType.Date:
                                            point.ToolTip = String.Format(Thread.CurrentThread.CurrentUICulture,"{0}: {1}: {2}", series.ToolTip, DateTime.FromOADate(point.XValue).ToShortDateString(), point.YValues[0]);
                                            break;
                                        default:
                                            point.ToolTip = String.Format(Thread.CurrentThread.CurrentUICulture,"{0}: {1}", series.ToolTip,  point.YValues[0]);
                                            break;
                                    }                                    
                                }
                                else {
                                    point.ToolTip = String.Format(Thread.CurrentThread.CurrentUICulture,"{0}: {1}", series.ToolTip, point.YValues[0]);
                                }
                            }
                            else {
                                if (this.ColumnValueInTooltip) {
                                    switch (series.XValueType) {
                                        case ChartValueType.DateTime:
                                        case ChartValueType.Date:
                                            point.ToolTip = String.Format(Thread.CurrentThread.CurrentUICulture,"{0}: {1}", DateTime.FromOADate(point.XValue).ToShortDateString(), point.YValues[0]);
                                            break;
                                        default:
                                            point.ToolTip = String.Format(Thread.CurrentThread.CurrentUICulture,"{0}", point.YValues[0]);
                                            break;
                                    }
                                }
                                else {
                                    point.ToolTip = String.Format(Thread.CurrentThread.CurrentUICulture,"{0}", point.YValues[0]);
                                }
                            }
                            

                        }
                        data.Clear();
                    } //for (int x = 0; x < XAxisSourceColumns.Count; x++) {
                }
            }

           

            if (this.ChartBorder) {
                m_chart.BorderSkin.SkinStyle = this.ChartBorderStyle;
                
                try {
                    m_chart.BorderlineColor = (Color)(new ColorConverter().ConvertFromString(this.ChartBorderColor));
                    m_chart.BorderSkin.BackColor = m_chart.BorderlineColor;
                }
                catch (NotSupportedException) {
                    m_chart.BorderlineColor = Color.Silver;
                    m_chart.BorderSkin.BackColor = Color.Silver;
                }
                m_chart.BorderlineDashStyle = this.ChartBorderLineStyle;
                m_chart.BorderlineWidth = this.ChartBorderWidth;
            }


            // Legend
            if (this.ShowLegend) {
                drawLegend();
            }
           
        }



        private void drawLegend() {
            Legend legend = m_chart.Legends.Add("Legend1");
            legend.Title = String.IsNullOrEmpty(LegendTitle) ? Localization.Translate("Legend") : this.LegendTitle;
            legend.Enabled = this.ShowLegend;
            legend.Docking = this.LegendPosition;
            legend.LegendStyle = this.LegendStyle;
            legend.TitleFont = new Font("Microsoft Sans Serif", this.LegendTitleFontSize, FontStyle.Bold);
            legend.Font = new Font("Microsoft Sans Serif", this.LegendFontSize, FontStyle.Regular);
        }
        private ChartArea CreateChartArea(string name) {
            ChartArea chartArea = m_chart.ChartAreas.Add(name);

            chartArea.Area3DStyle.Enable3D = this.Enable3DMode;
            if (this.Enable3DMode) {

                chartArea.Area3DStyle.LightStyle = this.ThreeDLightStyle;
                chartArea.Area3DStyle.IsRightAngleAxes = this.ThreeDIsometric;
                chartArea.Area3DStyle.Perspective = this.ThreeDPerspective;
                chartArea.Area3DStyle.Rotation = this.ThreeDRotation;
                chartArea.Area3DStyle.Inclination = this.ThreeDInclination;


            }
            chartArea.AxisY.IsLogarithmic = this.Logarithmic;
            return chartArea;
        }
        private bool createMultipleCharts() {
            if (this.MultipleCharts) {
                return true;
            }
            switch (this.ChartType) {
                case SeriesChartType.Pie:
                case SeriesChartType.Doughnut:
                case SeriesChartType.Pyramid:
                case SeriesChartType.Funnel:
                    return true;
            }
            return false;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowLegend {
            get;
            set;
        }



        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(Docking.Right)]
        public Docking LegendPosition { get; 
            set; }
        
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(LegendStyle.Column)]
        public LegendStyle LegendStyle {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(SeriesChartType.Line)]
        public SeriesChartType ChartType {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public string LegendTitle {
            get;
            set;
        }

        private int m_LegendFontSize;
        [Personalizable(PersonalizationScope.Shared)]
        public int LegendFontSize {
            get {
                return m_LegendFontSize == 0 ? 8 : m_LegendFontSize;
            }
            set {
                m_LegendFontSize = value;
            }
        }
        private int m_LegendTitleFontSize;
        [Personalizable(PersonalizationScope.Shared)]
        public int LegendTitleFontSize {
            get {
                return m_LegendTitleFontSize == 0 ? 8 : m_LegendTitleFontSize;
            }
            set {
                m_LegendTitleFontSize = value;
            }
        }


        
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(false)]
        public bool TreatAsZero {
            get;
            set;
        }

        
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(false)]
        public bool ChartBorder {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public BorderSkinStyle ChartBorderStyle {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public int ChartBorderWidth {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public string ChartBorderColor {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public ChartDashStyle ChartBorderLineStyle {
            get;
            set;
        }

        [DefaultValue(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public bool LinkToSourceList {
            get;
            set;
        }
        [DefaultValue(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public bool ColumnNameInTooltip { get; set; }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ColumnValueInTooltip { get; set; }

        [Personalizable(PersonalizationScope.Shared)]
        public bool Logarithmic { get; set; }

        [Personalizable(PersonalizationScope.Shared)]
        public DrawingStyle DrawingStyle {
            get;
            set;
        }

       
        [DefaultValue(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public bool Enable3DMode {
            get;
            set;
        }


        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(300)]
        public int ChartWidth {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(300)]
        public int ChartHeight {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string ChartTitle {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string SiteUrl {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public Guid ListId {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public Guid ViewId {
            get;
            set;
        }


        [Personalizable(PersonalizationScope.Shared)]
        public ChartColorPalette Palette {
            get;
            set;
        }
        
        [Personalizable(PersonalizationScope.Shared)]
        public bool CustomPalette {
            get;
            set;
        }


        [Personalizable(PersonalizationScope.Shared)]
        public string CustomPaletteValues {
            get;
            set;
        }


        [Personalizable(PersonalizationScope.Shared)]
        public List<string> XAxisSourceColumns {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public List<string> X2AxisSourceColumns {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public List<string> YAxisSourceColumns {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public LightStyle ThreeDLightStyle {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public bool ThreeDIsometric {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public int ThreeDPerspective {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public int ThreeDRotation {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public int ThreeDInclination {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public bool MultipleCharts {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared)]
        public LockDownModes LockDownMode {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(12)]
        public int TitleFontSize { get; set; }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowValueAsLabel {
            get;
            set;
        }
        
        #region Connection
        private IWebPartTable _provider;
        DataTable _table;
   
        [ConnectionConsumer("Table Data")]
        public void SetConnectionInterface(IWebPartTable provider) {
            _provider = provider;
        }

        protected override void OnPreRender(EventArgs e) {
            if (_provider != null) {
                _provider.GetTableData(GetTableData);
            }
            base.OnPreRender(e);
        }
        private void GetTableData(object tableData) {
            if (tableData != null) {
                DataView view = tableData as DataView;
                if (view != null) {
                    _table = view.Table;
                }
            }
        }
        
        
        #endregion
    }
}
