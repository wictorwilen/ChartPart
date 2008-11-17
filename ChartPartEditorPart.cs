using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Utilities;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Collections.Specialized;
using System.Web.UI.DataVisualization.Charting;


namespace ChartPart {
    public class ChartPartEditorPart : BaseEditorPart {

        public ChartPartEditorPart() {
            this.Title = "ChartPart Settings";

        }


        TextBox m_siteUrl;
        TextBox m_title;
        DropDownList m_list;
        DropDownList m_view;
        DropDownList m_chartType;
        CheckBoxList m_xcols;
        DropDownList m_ycols;


        protected override Control FillEditorPanel() {
            this.EditorTable = new Table();
            this.EditorTable.CellPadding = 0;
            this.EditorTable.CellSpacing = 0;
            this.EditorTable.Style["border-collapse"] = "collapse";
            this.EditorTable.Attributes.Add("width", "100%");

            // add the rows

            m_siteUrl = CreateEditorPartTextBox();
            m_siteUrl.TextChanged += new EventHandler(m_siteUrl_TextChanged);
            m_siteUrl.AutoPostBack = true;
            m_list = new DropDownList();
            m_view = new DropDownList();
            m_list.Items.Add(new ListItem("-- Select --", Guid.Empty.ToString()));
            m_list.SelectedIndexChanged += new EventHandler(m_list_SelectedIndexChanged);
            m_list.AutoPostBack = true;
            m_view.Items.Add(new ListItem("-- Select --", Guid.Empty.ToString()));
            m_view.SelectedIndexChanged += new EventHandler(m_view_SelectedIndexChanged);
            m_view.AutoPostBack = true;
            m_xcols = new CheckBoxList();
            m_ycols = new DropDownList();
            m_chartType = new DropDownList();
            m_chartType.Items.Add(SeriesChartType.Point.ToString());
            m_chartType.Items.Add(SeriesChartType.Line.ToString());
            m_chartType.Items.Add(SeriesChartType.Spline.ToString());
            m_chartType.Items.Add(SeriesChartType.Column.ToString());
            m_chartType.Items.Add(SeriesChartType.StackedColumn.ToString());
            m_chartType.Items.Add(SeriesChartType.StackedColumn100.ToString());
            m_chartType.Items.Add(SeriesChartType.Bar.ToString());
            m_chartType.Items.Add(SeriesChartType.StackedBar.ToString());
            m_chartType.Items.Add(SeriesChartType.StackedBar100.ToString());
            m_chartType.Items.Add(SeriesChartType.Area.ToString());
            m_chartType.Items.Add(SeriesChartType.SplineArea.ToString());
            m_chartType.Items.Add(SeriesChartType.StackedArea.ToString());
            m_chartType.Items.Add(SeriesChartType.StackedArea100.ToString());


            m_title = CreateEditorPartTextBox();
            this.EditorTable.Rows.Add(CreateToolPaneRow("Title", new Control[] { m_title }));
            this.EditorTable.Rows.Add(CreateToolPaneRow("Site", new Control[] { m_siteUrl }));
            this.EditorTable.Rows.Add(CreateToolPaneRow("List", new Control[] { m_list }));
            this.EditorTable.Rows.Add(CreateToolPaneRow("View", new Control[] { m_view }));
            this.EditorTable.Rows.Add(CreateToolPaneSeparator());
            this.EditorTable.Rows.Add(CreateToolPaneRow("X-Series column(s)", new Control[] { m_xcols }));
            this.EditorTable.Rows.Add(CreateToolPaneSeparator());
            this.EditorTable.Rows.Add(CreateToolPaneRow("Y-Series column", new Control[] { m_ycols }));
            this.EditorTable.Rows.Add(CreateToolPaneSeparator());
            this.EditorTable.Rows.Add(CreateToolPaneRow("Chart Type", new Control[] { m_chartType }));



            return this.EditorTable;
        }



        void m_siteUrl_TextChanged(object sender, EventArgs e) {
            m_view.Items.Clear();
            m_xcols.Items.Clear();
            m_ycols.Items.Clear();
            if (!string.IsNullOrEmpty(m_siteUrl.Text)) {
                using (SPSite site = new SPSite(m_siteUrl.Text)) {
                    using (SPWeb web = site.OpenWeb(m_siteUrl.Text.Substring(site.Url.Length))) {
                        fillLists(web);
                    }
                }
            }
        }

        void m_view_SelectedIndexChanged(object sender, EventArgs e) {
            m_xcols.Items.Clear();
            m_ycols.Items.Clear();
            if (!string.IsNullOrEmpty(m_siteUrl.Text)) {
                using (SPSite site = new SPSite(m_siteUrl.Text)) {
                    using (SPWeb web = site.OpenWeb(m_siteUrl.Text.Substring(site.Url.Length))) {
                        SPList sellist = web.Lists[new Guid(m_list.SelectedValue)];
                        if (sellist != null) {
                            try {
                                SPView selview = sellist.Views[new Guid(m_view.SelectedValue)];
                                if (selview != null) {
                                    fillColumns(sellist, selview);
                                }
                            }
                            catch (ArgumentException) {
                            }
                        }
                    }
                }
            }
        }

        void m_list_SelectedIndexChanged(object sender, EventArgs e) {
            m_view.Items.Clear();
            m_xcols.Items.Clear();
            m_ycols.Items.Clear();
            if (!string.IsNullOrEmpty(m_siteUrl.Text)) {
                using (SPSite site = new SPSite(m_siteUrl.Text)) {
                    using (SPWeb web = site.OpenWeb(m_siteUrl.Text.Substring(site.Url.Length))) {
                        SPList sellist = web.Lists[new Guid(m_list.SelectedValue)];
                        if (sellist != null) {
                            fillViews(sellist);
                        }
                    }
                }
            }
        }





        public override bool ApplyChanges() {
            EnsureChildControls();
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {
                chartPart.SiteUrl = m_siteUrl.Text;
                chartPart.ChartTitle = m_title.Text;
                if (m_list.SelectedValue != null) {
                    chartPart.ListId = new Guid(m_list.SelectedValue);
                }
                if (m_view.SelectedValue != null) {
                    chartPart.ViewId = new Guid(m_view.SelectedValue);
                }
                chartPart.XAxisSourceColumns = new List<string>();
                foreach (ListItem li in m_xcols.Items) {
                    if (li.Selected) {
                        chartPart.XAxisSourceColumns.Add(li.Value);
                    }
                }
                chartPart.YAxisSourceColumns = new List<string>();
                foreach (ListItem li in m_ycols.Items) {
                    if (li.Selected) {
                        chartPart.YAxisSourceColumns.Add(li.Value);
                    }
                }
                chartPart.ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), m_chartType.SelectedValue);
            }
            // Send the custom text to the Web Part.
            //chart.Text = Page.Request.Form[inputname];
            return true;
        }

        public override void SyncChanges() {
            EnsureChildControls();
            // sync with the new property changes here
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {

                if (string.IsNullOrEmpty(chartPart.SiteUrl)) {
                    chartPart.SiteUrl = SPContext.Current.Web.Url;
                }
                m_title.Text = chartPart.ChartTitle;
                m_siteUrl.Text = chartPart.SiteUrl;
                m_chartType.SelectedValue = chartPart.ChartType.ToString();
                
                using (SPSite site = new SPSite(chartPart.SiteUrl)) {
                    using (SPWeb web = site.OpenWeb(chartPart.SiteUrl.Substring(site.Url.Length))) {
                        fillLists(web);
                        m_list.SelectedValue = chartPart.ListId.ToString();
                        if (chartPart.ListId != Guid.Empty) {
                            SPList sellist = web.Lists[chartPart.ListId];

                            if (sellist != null) {
                                fillViews(sellist);
                                m_view.SelectedValue = chartPart.ViewId.ToString();
                                if (chartPart.ViewId != Guid.Empty) {
                                    try {
                                        SPView selview = sellist.Views[chartPart.ViewId];
                                        if (selview != null) {


                                            fillColumns(sellist, selview);
                                            if (chartPart.XAxisSourceColumns != null) {
                                                foreach (string s in chartPart.XAxisSourceColumns) {
                                                    m_xcols.Items.FindByValue(s).Selected = true;
                                                }
                                            }
                                            if (chartPart.YAxisSourceColumns != null) {
                                                foreach (string s in chartPart.YAxisSourceColumns) {
                                                    m_ycols.Items.FindByValue(s).Selected = true;
                                                }
                                            }
                                        }
                                    }
                                    catch (ArgumentException) {
                                    }
                                }
                            }
                        }
                    }
                }



                m_view.SelectedValue = chartPart.ViewId.ToString();
            }
        }

        private void fillLists(SPWeb web) {
            m_list.Items.Clear();
            m_list.Items.Add(new ListItem("-- Select --", Guid.Empty.ToString()));
            foreach (SPList list in web.Lists) {
                m_list.Items.Add(new ListItem(list.Title, list.ID.ToString()));
            }
        }

        private void fillViews(SPList sellist) {
            m_view.Items.Clear();
            m_view.Items.Add(new ListItem("-- Select --", Guid.Empty.ToString()));
            foreach (SPView view in sellist.Views) {
                m_view.Items.Add(new ListItem(view.Title, view.ID.ToString()));
            }
        }

        private void fillColumns(SPList sellist, SPView selview) {
            m_xcols.Items.Clear();
            m_ycols.Items.Clear();
            foreach (SPField field in sellist.Fields) {
                if (selview.ViewFields.Exists(field.InternalName)) {
                    if (isNumericField(field)) {
                        m_xcols.Items.Add(new ListItem(field.Title, field.InternalName));
                    }
                    m_ycols.Items.Add(new ListItem(field.Title, field.InternalName));
                }
            }
        }

        private bool isNumericField(SPField field) {
            if (field.FieldValueType == typeof(System.Double)) {
                return true;
            }
            return false;
        }



    }
}
