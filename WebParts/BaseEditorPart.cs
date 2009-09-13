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
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Threading;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Security;
using System.Security.Permissions;
using System.Globalization;


namespace ChartPart {
    /// <summary>
    /// Abstract helper class for creating EditorParts
    /// </summary>
    public abstract class BaseEditorPart : EditorPart {

        /// <summary>
        /// Initializes a new instance of the BaseEditorPart class.
        /// </summary>
        protected BaseEditorPart(string id) :this(false) {
            
        }
        /// <summary>
        /// Initializes a new instance of the BaseEditorPart class.
        /// </summary>
        protected BaseEditorPart(bool sharedModeOnly) {
            this.SharedModeOnly = sharedModeOnly;
        }

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
        public bool SharedModeOnly { get; private set; }

        public virtual bool IsVisible(WebPart webPart) {
            return true;
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
        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
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
            button.ToolTip = Localization.Translate("ToolBuilderToolTip");
            button.TabIndex = 0;
            button.Text = "...";
            button.OnClientClick = string.Format(CultureInfo.InvariantCulture, "javascript:MSOPGrid_doBuilder('{0}?culture={1}', {2}, 'dialogHeight:340px;dialogWidth:430px;help:no;status:no;resizable:yes');",
                SPHttpUtility.EcmaScriptStringLiteralEncode("/_layouts/zoombldr.aspx"),
                Thread.CurrentThread.CurrentUICulture,
                SPHttpUtility.EcmaScriptStringLiteralEncode(textBox.ClientID));
            button.Style.Add("display", "none");
            button.Attributes.Add("onfocusout", "this.style.display='none';");

            textBox.Attributes.Add("onfocusin", string.Format(CultureInfo.InvariantCulture,"MSOPGrid_BuilderVisible({0});", button.ClientID));
            textBox.Attributes.Add("ondeactivate", string.Format(CultureInfo.InvariantCulture, "MSOTlPn_prevBuilder={0};", button.ClientID));
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

        protected static TextBox CreateEditorPartTextBox(int width) {
            TextBox textBox = new TextBox();
            textBox.CssClass = "UserInput";
            textBox.Width = new Unit(string.Format(CultureInfo.InvariantCulture, "{0}px", width),CultureInfo.InvariantCulture);
            return textBox;
        }
        protected static TextBox CreateEditorPartTextBox() {
            return CreateEditorPartTextBox(176);
        }
        protected static WebControl[] CreateCheckBoxControls(CheckBox checkBox, string title) {
            return CreateCheckBoxControls(checkBox, title, null);
        }
        protected static WebControl[] CreateCheckBoxControls(CheckBox checkBox, string title, string description) {
            WebControl[] controls = new WebControl[2];
            
            controls[0] = new Label();
            controls[0].Controls.Add(checkBox);

            controls[1] = new WebControl(HtmlTextWriterTag.Label);
            if(description != null){
                controls[1].ToolTip = description;
            }
            controls[1].Attributes.Add("for", checkBox.ID);
            controls[1].Controls.Add(new LiteralControl(title));

            return controls;

        }

        protected static TableRow CreateToolPaneRow(Control[] controls) {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Controls.Add(new LiteralControl("<div class='UserSectionHead'>"));
            
            foreach (Control control in controls) {
                cell.Controls.Add(control);
                WebControl wc = control as WebControl;

                if (wc != null) {
                    wc.Attributes.Add("onfocusin", "MSOPGrid_HidePrevBuilder()");
                    wc.Attributes.Add("onclick", "MSOPGrid_HidePrevBuilder()");
                }
            }
            cell.Controls.Add(new LiteralControl("</div>"));
            row.Cells.Add(cell);
            return row;

        }
        protected static TableRow CreateToolPaneRow(string title, string description, Control[] controls) {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Controls.Add(new LiteralControl(String.Format(CultureInfo.InvariantCulture, "<div class='UserSectionHead'>{0}</div>", title)));
            cell.Controls.Add(new LiteralControl("<div class='UserSectionBody'><div class='UserControlGroup'><nobr>"));
            foreach (Control control in controls) {
                cell.Controls.Add(control);
                WebControl wc = control as WebControl;
                
                if (wc != null) {
                    if (!string.IsNullOrEmpty(description)) {
                        wc.ToolTip = description;
                    }
                    wc.Attributes.Add("onfocusin", "MSOPGrid_HidePrevBuilder()");
                    wc.Attributes.Add("onclick", "MSOPGrid_HidePrevBuilder()");
                }

            }
            cell.Controls.Add(new LiteralControl("</nobr></div></div>"));
            row.Cells.Add(cell);
            return row;

        }

        protected static TableRow CreateToolPaneRow(string title, Control[] controls) {
            return CreateToolPaneRow(title, null, controls);

        }
        protected static TableRow CreateToolPaneRowWithBuilder(string title, TextBox textBox) {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            row.Cells.Add(cell);

            cell.Controls.Add(new LiteralControl(String.Format(CultureInfo.InvariantCulture, "<div class='UserSectionHead'>{0}</div>", title)));
            cell.Controls.Add(new LiteralControl("<div class='UserSectionBody'><div class='UserControlGroup'><nobr>"));
            cell.Controls.Add(textBox);


            
            cell.Controls.Add(new LiteralControl("</nobr></div></div>"));
            
            return row;

        }
        protected static TableRow CreateToolPaneSeparator() {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Controls.Add(new LiteralControl("<div style='width:100%' class='UserDottedLine'></div>"));
            row.Cells.Add(cell);
            return row;

        }


     
    }
}
