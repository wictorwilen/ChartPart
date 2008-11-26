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
using System.Linq;
using System.Drawing;

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

            m_chart.Page = this.Page;

            
            

            m_chart.ImageType = ChartImageType.Png;
            m_chart.ImageStorageMode = ImageStorageMode.UseHttpHandler;
            m_chart.RenderType = RenderType.ImageTag;
            this.Controls.Add(m_chart);
            base.CreateChildControls();

           
            
        }

        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        protected override void Render(HtmlTextWriter writer) {
            if (string.IsNullOrEmpty(this.SiteUrl)) {
                RenderError(writer, CreateErrorControl(Properties.Resources.MissingSite, true));
                return;
            }
            if (this.ListId == Guid.Empty) {
                RenderError(writer, CreateErrorControl(Properties.Resources.MissingList, true));
                return;
            }
            if (this.ViewId == Guid.Empty) {
                RenderError(writer, CreateErrorControl(Properties.Resources.MissingView, true));
                return;
            }
            if (this.XAxisSourceColumns.Count == 0) {
                RenderError(writer, CreateErrorControl(Properties.Resources.MissingXAxis, true));
                return;
            }
            if (this.YAxisSourceColumns.Count == 0) {
                RenderError(writer, CreateErrorControl(Properties.Resources.MissingYAxis, true));
                return;
            }
            try {

                GenerateChart();
                
                if (this.CustomPalette) {
                    m_chart.Palette = ChartColorPalette.None;
                   
                    List<string> sColors = new List<string>(this.CustomPaletteValues.Split(','));
                    try {
                        List<Color> colors = sColors.ConvertAll<Color>(new Converter<string, Color>((s) => (Color)(new ColorConverter().ConvertFromString(s))));
                        m_chart.PaletteCustomColors = colors.ToArray();
                    }
                    catch (Exception) {
                        RenderError(writer, CreateErrorControl("One or more colors in the custom colors could not be parsed", true));
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
            catch (Exception ex) {
                writer.WriteEncodedText(Properties.Resources.ExceptionOccurred + ex.ToString());
            }
        }


        protected abstract void GenerateChart();

        [WebBrowsable]
        [LocalizedWebDisplayNameAttribute("Width")]
        [LocalizedWebDescriptionAttribute("WidthDesc")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Chart")]
        [DefaultValue(300)]
        public int ChartWidth {
            get;
            set;
        }
        [WebBrowsable]
        [Personalizable(PersonalizationScope.Shared)]
        [LocalizedWebDisplayNameAttribute("Height")]
        [LocalizedWebDescriptionAttribute("HeightDesc")]
        [Category("Chart")]
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
        [Category("Chart")]
        [LocalizedWebDisplayName("Palette")]
        [LocalizedWebDescription("PaletteDesc")]
        [Personalizable(PersonalizationScope.Shared)]
        public ChartColorPalette Palette {
            get;
            set;
        }
        [WebBrowsable]
        [Category("Chart")]
        [LocalizedWebDisplayName("CustomPalette")]
        [LocalizedWebDescription("CustomPaletteDesc")]
        [Personalizable(PersonalizationScope.Shared)]
        public bool CustomPalette {
            get;
            set;
        }

        [WebBrowsable]
        [Category("Chart")]
        [LocalizedWebDisplayName("CustomPaletteValues")]
        [LocalizedWebDescription("CustomPaletteValuesDesc")]
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
        public List<string> YAxisSourceColumns {
            get;
            set;
        }

    }
}
