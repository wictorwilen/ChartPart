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
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Globalization;

namespace ChartPart {
    /// <summary>
    /// Abstract class for creating a WebPart with an EditorPart
    /// </summary>
    /// <typeparam name="T">The EditorClass to use</typeparam>
    public abstract class BaseWebPart<T> : System.Web.UI.WebControls.WebParts.WebPart, IWebEditable where T : BaseEditorPart, new() {

        protected Panel panel;


        protected override void CreateChildControls() {
            base.CreateChildControls();
            panel = new Panel();
            this.Controls.Add(panel);
        }

   
        protected void RenderError(Control control) {
            Table table = new Table();
            TableRow row = new TableRow();
            table.Rows.Add(row);
            row.Cells.Add(new TableCell());
            row.Cells[0].Controls.Add(control);
            this.panel.Controls.Add(table);
        }
        /// <summary>
        /// Renders an error to the target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="control"></param>
        protected static void RenderError(Control target, Control control) {
            Table table = new Table();
            TableRow row = new TableRow();
            table.Rows.Add(row);
            row.Cells.Add(new TableCell());
            row.Cells[0].Controls.Add(control);
            target.Controls.Add(table);
        }

        /// <summary>
        /// Creates a control containing an error message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Label CreateErrorControl(string message, bool showToolPane) {
            Label label = new Label();
            LiteralControl content = new LiteralControl();
            content.Text = message;
            label.Controls.Add(content);
            if (showToolPane) {
                HyperLink hl = new HyperLink();
                hl.NavigateUrl = string.Format(CultureInfo.InvariantCulture, "javascript:MSOTlPn_ShowToolPane2Wrapper('Edit','129','{0}');", this.ID);
                hl.ID = string.Format(CultureInfo.InvariantCulture, "MsoFrameworkToolpartDefmsg_{0}", this.ID);
                hl.Text = Properties.Resources.OpenToolPane;
                label.Controls.Add(new LiteralControl("<br/>"));
                label.Controls.Add(hl);
            }
            return label;
        }



        
        #region IWebEditable Members
        

        EditorPartCollection IWebEditable.CreateEditorParts() {
            if (this.WebPartManager.Personalization.Scope == PersonalizationScope.Shared) {
                List<EditorPart> editors = new List<EditorPart>();
                T editor = new T();
                editor.ID = this.ID + editor.EditorName;
                editors.Add(editor);
                return new EditorPartCollection(editors);
            }
            else {
                return null;
            }
        }

        object IWebEditable.WebBrowsableObject {
            get { return this; }
        }
     
        #endregion
    }
}
