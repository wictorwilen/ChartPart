using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace ChartPart {
    public abstract class BaseEditorPart: EditorPart {

        /// <summary>
        /// The main table used in the editor part
        /// </summary>
        protected Table EditorTable{
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
        protected abstract Control FillEditorPanel();

       


        /// <summary>
        /// Overrides the CreateChildControls
        /// </summary>
        protected override void CreateChildControls() {
            this.EditorPanel= new Panel();
            this.EditorPanel.CssClass = "ms-ToolPartSpacing";
            this.EditorPanel.Controls.Add(this.FillEditorPanel());

            this.Controls.Add(this.EditorPanel);
            base.CreateChildControls();
            this.ChildControlsCreated = true;
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
            }
            cell.Controls.Add(new LiteralControl("</nobr></div></div>"));
            row.Cells.Add(cell);
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
