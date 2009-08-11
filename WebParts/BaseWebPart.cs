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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace ChartPart {
    /// <summary>
    /// Abstract class for creating a WebPart with an EditorPart
    /// </summary>
    /// <typeparam name="T">The EditorClass to use</typeparam>
    public abstract class BaseWebPart<T> : WebPart, IWebEditable where T : BaseEditorPartCollection, new() {
        protected BaseWebPart() {
            
        }

        /// <summary>
        /// Renders an error to the writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="control"></param>
        protected static void RenderError(HtmlTextWriter writer, Control control) {
            Table table = new Table();
            TableRow row = new TableRow();
            table.Rows.Add(row);
            row.Cells.Add(new TableCell());
            row.Cells[0].Controls.Add(control);
            table.RenderControl(writer);
        }

        /// <summary>
        /// Creates a control containing an error message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="useToolpane">Set to true if a link to the toolpane should be provided</param>
        /// <returns></returns>
        protected Label CreateErrorControl(string message, bool useToolpane) {
            Label label = new Label();
            LiteralControl content = new LiteralControl(message);
            label.Controls.Add(content);
            HyperLink hl = new HyperLink { 
                NavigateUrl = string.Format("javascript:MSOTlPn_ShowToolPane2Wrapper('Edit','129','{0}');", this.ID), 
                ID = string.Format("MsoFrameworkToolpartDefmsg_{0}", this.ID), 
                Text = Localization.Translate("OpenToolPane") };
            label.Controls.Add(new LiteralControl("<br/>"));
            label.Controls.Add(hl);
            return label;
        }

        #region IWebEditable Members

        EditorPartCollection IWebEditable.CreateEditorParts() {
            T editors = new T();
            List<BaseEditorPart> list = new List<BaseEditorPart>();
            foreach (BaseEditorPart editor in  editors) {
                // set the Id
                editor.ID = this.ID + editor.EditorName;
                // Only add the editors we want here
                if ((!editor.SharedModeOnly || editor.SharedModeOnly && this.WebPartManager.Personalization.Scope == PersonalizationScope.Shared) 
                    && editor.IsVisible(this)) {
                    list.Add(editor);
                }                
            }
            
            return new EditorPartCollection(list);
        }

        object IWebEditable.WebBrowsableObject {
            get { return this; }
        }
     
        #endregion
    }
}
