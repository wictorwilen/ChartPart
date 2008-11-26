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
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Threading;
using Microsoft.SharePoint.Utilities;
using System.Web.UI.HtmlControls;


namespace ChartPart {
    /// <summary>
    /// Abstract helper class for creating EditorParts
    /// </summary>
    public abstract class BaseEditorPart : EditorPart {

        /// <summary>
        /// Name of the editor, required
        /// </summary>
        public abstract string EditorName {
            get;
        }

        /// <summary>
        /// The main table used in the editor part
        /// </summary>
        protected Table EditorTable {
            get;
            set;
        }

        /// <summary>
        /// The Panel used for the EditorPart
        /// </summary>
        protected Panel EditorPanel {
            get;
            set;
        }

        /// <summary>
        /// Create the actual content in the editor part here...
        /// </summary>
        protected abstract void FillEditorPanel();




        /// <summary>
        /// Overrides the CreateChildControls
        /// </summary>
        protected override void CreateChildControls() {
            this.EditorPanel = new Panel();
            this.EditorPanel.CssClass = "ms-ToolPartSpacing";

            this.Controls.Add(this.EditorPanel);

            FillEditorPanel();
            

            
            base.CreateChildControls();
            this.ChildControlsCreated = true;
        }

        protected void AddToolPaneRow(TableRow row) {
            if (this.EditorTable == null) {
                CreateToolPaneTable();
            }
            this.EditorTable.Rows.Add(row);
        }
        protected void AddToolPaneRowWithBuilder(TableRow row, TextBox textBox) {
            if (this.EditorTable == null) {
                CreateToolPaneTable();
            }
            this.EditorTable.Rows.Add(row);

            TableCell cell = row.Cells[0];
            
            cell.Controls.AddAt(cell.Controls.IndexOf(textBox), new LiteralControl("&nbsp;"));
            Button button = new Button();
            
            cell.Controls.AddAt(cell.Controls.IndexOf(textBox) + 1, button);
            button.EnableViewState = false;
            button.CssClass = "ms-PropGridBuilderButton";
            button.ToolTip = Properties.Resources.ToolBuilderToolTip;
            button.TabIndex = 0;
            button.Text = "...";
            button.OnClientClick = string.Format("javascript:MSOPGrid_doBuilder('{0}?culture={1}', {2}, 'dialogHeight:340px;dialogWidth:430px;help:no;status:no;resizable:yes');",
                SPHttpUtility.EcmaScriptStringLiteralEncode("/lt/_layouts/zoombldr.aspx"),
                Thread.CurrentThread.CurrentUICulture,
                SPHttpUtility.EcmaScriptStringLiteralEncode(textBox.ClientID));
            button.Style.Add("display", "none");
            button.Attributes.Add("onfocusout", "this.style.display='none';");

            textBox.Attributes.Add("onfocusin", string.Format("MSOPGrid_BuilderVisible({0});", button.ClientID));
            textBox.Attributes.Add("ondeactivate", string.Format("MSOTlPn_prevBuilder={0};", button.ClientID));
            textBox.Attributes.Add("ms-TlPnWiden", "true");


            
        }
        protected void CreateToolPaneTable() {
            this.EditorTable = new Table();
            this.EditorTable.CellPadding = 0;
            this.EditorTable.CellSpacing = 0;
            this.EditorTable.Style["border-collapse"] = "collapse";
            this.EditorTable.Attributes.Add("width", "100%");
            this.EditorPanel.Controls.Add(this.EditorTable);
        }

        protected TextBox CreateEditorPartTextBox() {
            TextBox textBox = new TextBox();
            textBox.CssClass = "UserInput";
            textBox.Width = new Unit("176px");
            return textBox;
        }
   

        protected TableRow CreateToolPaneRow(string title, Control[] controls) {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Controls.Add(new LiteralControl("<div class='UserSectionHead'>" + title + "</div>"));
            cell.Controls.Add(new LiteralControl("<div class='UserSectionBody'><div class='UserControlGroup'><nobr>"));
            foreach (Control control in controls) {
                cell.Controls.Add(control);
                WebControl wc = control as WebControl;
                if (wc != null) {
                    wc.Attributes.Add("onfocusin","MSOPGrid_HidePrevBuilder()");
                    wc.Attributes.Add("onclick", "MSOPGrid_HidePrevBuilder()");
                }
                
            }
            cell.Controls.Add(new LiteralControl("</nobr></div></div>"));
            row.Cells.Add(cell);
            return row;

        }
        protected TableRow CreateToolPaneRowWithBuilder(string title, TextBox textBox) {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            row.Cells.Add(cell);

            cell.Controls.Add(new LiteralControl("<div class='UserSectionHead'>" + title + "</div>"));
            cell.Controls.Add(new LiteralControl("<div class='UserSectionBody'><div class='UserControlGroup'><nobr>"));
            cell.Controls.Add(textBox);


            
            cell.Controls.Add(new LiteralControl("</nobr></div></div>"));
            
            return row;

        }
        protected TableRow CreateToolPaneSeparator() {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Controls.Add(new LiteralControl("<div style='width:100%' class='UserDottedLine'></div>"));
            row.Cells.Add(cell);
            return row;

        }
    }
}
