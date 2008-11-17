using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace ChartPart {
    public abstract class BaseWebPart : System.Web.UI.WebControls.WebParts.WebPart {

        protected void RenderError(HtmlTextWriter writer, Control control) {
            Table table = new Table();
            TableRow row = new TableRow();
            table.Rows.Add(row);
            row.Cells.Add(new TableCell());
            row.Cells[0].Controls.Add(control);
            table.RenderControl(writer);
        }

        protected Label CreateErrorControl(string message, bool useToolpane) {
            Label label = new Label();
            LiteralControl content = new LiteralControl();
            content.Text = message;
            label.Controls.Add(content);
            HyperLink hl = new HyperLink();
            hl.NavigateUrl = string.Format("javascript:MSOTlPn_ShowToolPane2Wrapper('Edit','129','{0}');", this.ID);
            hl.ID = string.Format("MsoFrameworkToolpartDefmsg_{0}", this.ID);
            hl.Text = "Click here to open the Tool Pane...";
            label.Controls.Add(new LiteralControl("<br/>"));
            label.Controls.Add(hl);
            return label;
        }
    }
}
