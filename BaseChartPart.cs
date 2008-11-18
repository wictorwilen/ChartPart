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
using System.Collections.Generic;
using System.Text;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;
using Microsoft.SharePoint.Security;
using System.Security.Permissions;
using System.Web.UI;

namespace ChartPart {
    public abstract class BaseChartPart<T> : BaseWebPart<T> where T: BaseEditorPart, new(){
        protected Chart m_chart;

        protected override void CreateChildControls() {
            m_chart = new Chart();
            if (this.ChartHeight > 0) {
                m_chart.Height = this.ChartHeight > 600 ? 600 : this.ChartHeight;
            }

            if (this.ChartWidth > 0) {
                m_chart.Width = this.ChartWidth > 600 ? 600 : this.ChartWidth;
            }
            m_chart.Palette = this.Palette;

            m_chart.ImageType = ChartImageType.Png;
            m_chart.ImageStorageMode = ImageStorageMode.UseHttpHandler;
            m_chart.RenderType = RenderType.ImageTag;

            base.CreateChildControls();
        }

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

                GenerateChart();

                m_chart.Page = this.Page;
                m_chart.RenderControl(writer);
            }
            catch (Exception ex) {
                writer.WriteEncodedText("An exception occurred: " + ex.ToString());
            }
        }


        protected abstract void GenerateChart();

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
        public ChartColorPalette Palette {
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

    }
}
