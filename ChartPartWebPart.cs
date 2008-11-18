/*
 * 
 * ChartPart for SharePoint
 * ------------------------------------------
 * Copyright (c) 2008, Wictor Wilén
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
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.DataVisualization.Charting;
using System.ComponentModel;
using System.Web;
using Microsoft.SharePoint.Security;
using System.Security.Permissions;
using System.Collections.Generic;

namespace ChartPart {
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ChartPart runat=server></{0}:ChartPart>")]
    [XmlRoot(Namespace = "ChartPart")]
    [Guid("16da55e8-106a-49d9-b807-30544fa41f56")]
    [SupportsAttributeMarkup(true)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    public class ChartPartWebPart : BaseChartPart<ChartPartEditorPart> {


        public ChartPartWebPart() {
        }



        protected override void GenerateChart() {





            if (!string.IsNullOrEmpty(this.ChartTitle)) {
                Title title = new Title(this.ChartTitle, Docking.Top);
                m_chart.Titles.Add(title);
            }

            Dictionary<string, double> data = new Dictionary<string, double>();



            m_chart.ChartAreas.Add("ChartArea1");
            using (SPSite site = new SPSite(this.SiteUrl)) {
                using (SPWeb web = site.OpenWeb(this.SiteUrl.Substring(site.Url.Length))) {

                    SPList list = web.Lists[this.ListId];
                    SPView view = list.Views[this.ViewId];


                    for (int x = 0; x < XAxisSourceColumns.Count; x++) {
                        Series series = new Series();
                        series["DrawingStyle"] = "Cylinder";
                        if (this.LinkToSourceList) {
                            series.Url = web.Url + "/" + view.Url;
                        }
                        if (this.XAxisSourceColumns[x] == "**count**") {
                            series.ToolTip = "Count";
                            
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

                        // TODO Axis label

                        



                        foreach (SPListItem item in list.GetItems(view)) {
                            if (item[this.YAxisSourceColumns[0]] == null)
                                continue;

                            //set initial value to 0
                            if (!data.ContainsKey(item[this.YAxisSourceColumns[0]].ToString())) {
                                data.Add(item[this.YAxisSourceColumns[0]].ToString(), 0);
                            }

                            if (this.XAxisSourceColumns[x] == "**count**") {
                                data[item[this.YAxisSourceColumns[0]].ToString()] += 1;
                            }
                            else {
                                if (item[this.XAxisSourceColumns[x]] == null && !this.TreatAsZero) {
                                    continue;
                                }


                                if (!(item[this.XAxisSourceColumns[x]] == null && this.TreatAsZero)) {
                                    data[item[this.YAxisSourceColumns[0]].ToString()] += double.Parse(item[this.XAxisSourceColumns[x]].ToString());
                                }
                            }
                        }

                        foreach (string key in data.Keys) {
                            if (yField.FieldValueType == typeof(DateTime)) {
                                series.XValueType = ChartValueType.DateTime;
                                series.Points.AddXY(DateTime.Parse(key), data[key]);
                            } 
                            else if (yField.FieldValueType == typeof(SPFieldUserValue)) {
                                series.XValueType = ChartValueType.String;
                                
                                series.Points.AddXY(key.Substring(key.IndexOf(";#")+2), data[key]);
                                
                            }
                            else if (yField.FieldValueType == typeof(string)) {
                                series.XValueType = ChartValueType.String;
                                series.Points.AddXY(key, data[key]);
                            } 
                            else if(yField.FieldValueType == null) {
                                series.Points.AddXY(key, data[key]);
                            }
                            else {
                                series.Points.Add(data[key]);
                            }
                        }
                        data.Clear();
                    }
                }
            }
           

            if (this.ChartBorder) {
                m_chart.BorderSkin.SkinStyle = BorderSkinStyle.FrameTitle1;
                m_chart.BorderColor = System.Drawing.Color.FromArgb(26, 59, 105);
                m_chart.BorderlineDashStyle = ChartDashStyle.Solid;
                m_chart.BorderWidth = 2;
            }


            m_chart.Legends.Add("Legend1");
            m_chart.Legends["Legend1"].Title = Properties.Resources.Legend;
            m_chart.Legends["Legend1"].Enabled = this.ShowLegend;



        }





        [WebBrowsable]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Show legend")]
        public bool ShowLegend {
            get;
            set;
        }



        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(SeriesChartType.Line)]
        public SeriesChartType ChartType {
            get;
            set;
        }



        [WebBrowsable]
        [WebDisplayName("Treat missing values as zero")]
        [WebDescription("Treat null values as zero, otherwise don't include those rows at all")]
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(false)]
        public bool TreatAsZero {
            get;
            set;
        }

        [WebBrowsable]
        [WebDisplayName("Chart Border")]
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(false)]
        public bool ChartBorder {
            get;
            set;
        }

        [WebBrowsable]
        [WebDescription("Link to source list")]
        [DefaultValue(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public bool LinkToSourceList {
            get;
            set;
        }




    }
}
