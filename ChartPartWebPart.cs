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

/*
 1:  <appSettings>   2:      ...   3:      <add key="ChartImageHandler" value="storage=file;timeout=20;dir=C:\TempImages\;" />   4:  </appSettings>   5:      6:  <httpHandlers>   7:      ...   8:      <add path="ChartImg.axd" verb="GET,HEAD" type="System.Web.UI.DataVisualization.Charting.ChartHttpHandler, System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" validate="false" />   9:  </httpHandlers>  10:     11:  <handlers>  12:      ...  13:      <remove name="ChartImageHandler"/>  14:      ...  15:      <add name="ChartImageHandler" preCondition="integratedMode" verb="GET,HEAD" path="ChartImg.axd" type="System.Web.UI.DataVisualization.Charting.ChartHttpHandler, System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" />  16:  </handlers>
 */
namespace ChartPart {
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ChartPart runat=server></{0}:ChartPart>")]
    [XmlRoot(Namespace = "ChartPart")]
    [Guid("16da55e8-106a-49d9-b807-30544fa41f56")]
    [SupportsAttributeMarkup(true)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    public class ChartPartWebPart : BaseWebPart, IWebEditable {
        Chart m_chart;


        public ChartPartWebPart() {
        }

        protected override void CreateChildControls() {


            //  Height="296px" Width="412px" ImageLocation="~/TempImages/ChartPic_#SEQ(300,3)" Palette="BrightPastel" imagetype="Png" BorderDashStyle="Solid" BackSecondaryColor="White" BackGradientStyle="TopBottom" BorderWidth="2" backcolor="#D3DFF0" BorderColor="26, 59, 105"
            m_chart = new Chart();
            if (this.ChartHeight > 0) {
                m_chart.Height = this.ChartHeight > 600 ? 600 : this.ChartHeight;
            }

            if (this.ChartWidth > 0) {
                m_chart.Width = this.ChartWidth > 600 ? 600 : this.ChartWidth;
            }
            m_chart.Palette = this.Palette;

            m_chart.ImageType = ChartImageType.Png;
            //m_chart.ImageLocation = "~/TempImages/ChartPic_#SEQ(300,3)";
            m_chart.ImageStorageMode = ImageStorageMode.UseHttpHandler;
            m_chart.RenderType = RenderType.ImageTag;
            /*m_chart.ImageStorageMode = ImageStorageMode.UseHttpHandler;
            m_chart.RenderType = RenderType.BinaryStreaming;*/

            base.CreateChildControls();
        }



        // http://ibpcww/lt/Lists/Offerter/AllItems.aspx
        // Företag värde

        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        protected override void Render(HtmlTextWriter writer) {
            if (string.IsNullOrEmpty(this.SiteUrl)) {
                RenderError(writer, CreateErrorControl("No <b>Site</b> selected.", true));
                return;
            }
            if (this.ListId == Guid.Empty) {
                RenderError(writer, CreateErrorControl("No <b>List</b> selected.", true));
                return;
            }
            if (this.ViewId == Guid.Empty) {
                RenderError(writer, CreateErrorControl("No <b>View</b> selected.", true));
                return;
            }
            if (this.XAxisSourceColumns.Count == 0) {
                RenderError(writer, CreateErrorControl("No <b>Columns for X-axis</b> selected.", true));
                return;
            }
            if (this.YAxisSourceColumns.Count == 0) {
                RenderError(writer, CreateErrorControl("No <b>Column for Y-axis</b> selected.", true));
                return;
            }
            try {





                if (!string.IsNullOrEmpty(this.ChartTitle)) {
                    Title title = new Title(this.ChartTitle, Docking.Top);
                    m_chart.Titles.Add(title);
                }

                Dictionary<string, double> data = new Dictionary<string, double>();



                m_chart.ChartAreas.Add("ChartArea1");
                //m_chart.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
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
                            series.ToolTip = list.Fields.GetFieldByInternalName(this.XAxisSourceColumns[x]).Title; 
                            
                            series.ChartType = this.ChartType;
                            series.BorderWidth = 3;
                            series.ShadowOffset = 2;
                            series.Name = list.Fields.GetFieldByInternalName(this.XAxisSourceColumns[x]).Title;
                            SPField yField = list.Fields.GetFieldByInternalName(this.YAxisSourceColumns[0]);
                            m_chart.Series.Add(series);
                            

                            foreach (SPListItem item in list.GetItems(view)) {
                                if (item[this.XAxisSourceColumns[x]] == null && !this.TreatAsZero) {
                                    continue;
                                }

                                if (!data.ContainsKey(item[this.YAxisSourceColumns[0]].ToString())) {
                                    data.Add(item[this.YAxisSourceColumns[0]].ToString(), 0);
                                }
                                if (!(item[this.XAxisSourceColumns[x]] == null && this.TreatAsZero)) {
                                    data[item[this.YAxisSourceColumns[0]].ToString()] += double.Parse(item[this.XAxisSourceColumns[x]].ToString());
                                }
                            }

                            foreach (string key in data.Keys) {
                                if (yField.FieldValueType == typeof(DateTime)) {
                                    series.XValueType = ChartValueType.DateTime;
                                    series.Points.AddXY(DateTime.Parse(key), data[key]);
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
                //m_chart.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
                m_chart.Legends["Legend1"].Enabled = this.ShowLegend;
                //m_chart.ChartAreas[0].AxisX.CustomLabels



                m_chart.Page = this.Page;
                m_chart.RenderControl(writer);
            }
            catch (Exception ex) {
                writer.WriteEncodedText("An exception occurred: " + ex.ToString());
            }
        }

        EditorPartCollection IWebEditable.CreateEditorParts() {
            List<EditorPart> editors = new List<EditorPart>();
            ChartPartEditorPart editor = new ChartPartEditorPart();
            editor.ID = this.ID + "_CharPartEditor";
            editors.Add(editor);
            return new EditorPartCollection(editors);
        }
        object IWebEditable.WebBrowsableObject {
            get { return this; }
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
        [WebBrowsable]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Show legend")]
        public bool ShowLegend {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared)]
        public List<string> XAxisSourceColumns {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared)]
        public List<string> YAxisSourceColumns {
            get;
            set;
        }

        [WebBrowsable]
        [Personalizable(PersonalizationScope.Shared)]
        public ChartColorPalette Palette {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(SeriesChartType.Line)]
        public SeriesChartType ChartType{
            get;
            set;
        }

        [WebBrowsable]
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(300)]
        public int ChartWidth {
            get;
            set;
        }
        [WebBrowsable]
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue(300)]
        public int ChartHeight {
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
        public bool LinkToSourceList { get; set;
        }




    }
}
