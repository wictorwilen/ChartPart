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
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls.WebParts;

namespace ChartPart {
    public class Chart3DEditorPart : BaseEditorPart {

        CheckBox m_3Denabled;
        DropDownList m_lightstyle;
        CheckBox m_isometric;
        TextBox m_perspective;
        TextBox m_rotation;
        TextBox m_inclination;
        RangeValidator m_perspectiveValidator;
        RangeValidator m_rotationValidator;
        RangeValidator m_inclinationValidator;

        public Chart3DEditorPart()
            : base(false) {
            this.Title = Localization.Translate("ThreeDCharts");
            this.ChromeState = PartChromeState.Minimized;
        }
        /// <summary>
        /// Initializes a new instance of the Chart3DEditorPart class.
        /// </summary>
        public Chart3DEditorPart(string id)
            : base(id) {
            
        }
        /// <summary>
        /// Initializes a new instance of the Chart3DEditorPart class.
        /// </summary>
        public Chart3DEditorPart(bool sharedModeOnly)
            : base(sharedModeOnly) {
            
        }
         
        public override string EditorName {
            get { return "_Chart3DEditorPart"; }
        }

        public override bool IsVisible(WebPart webPart) {
            if (webPart != null) {
                ChartPartWebPart wp = webPart as ChartPartWebPart;
                if (wp != null) {
                    if ((wp.LockDownMode & LockDownModes.ThreeD )== LockDownModes.ThreeD) {
                        return false;
                    }
                }
            }
            return true;
        }

        protected override void FillEditorPanel() {
            CreateToolPaneTable();
            m_3Denabled = new CheckBox();            
            m_3Denabled.AutoPostBack = true;
            m_lightstyle = new DropDownList();
            Array.ForEach(
                Enum.GetNames(typeof(LightStyle)), 
                s => m_lightstyle.Items.Add(new ListItem(s))
            );
            m_isometric = new CheckBox();
            m_isometric.AutoPostBack = true;

            m_perspective = CreateEditorPartTextBox(90);
            m_perspective.Text = "0";
            m_perspectiveValidator = new RangeValidator();
            m_perspectiveValidator.MinimumValue = "0";
            m_perspectiveValidator.MaximumValue = "100";
            m_perspectiveValidator.Type = ValidationDataType.Integer;
            m_perspective.ID = "perspective";   
            m_perspectiveValidator.Text = String.Format(" {0}", Localization.Translate("InvalidValue"));
            m_perspectiveValidator.ControlToValidate = m_perspective.ID;

            m_rotation = CreateEditorPartTextBox(90);
            m_rotation.Text = "0";
            m_rotationValidator = new RangeValidator();
            m_rotationValidator.MinimumValue = "-360";
            m_rotationValidator.MaximumValue = "360";
            m_rotationValidator.Type = ValidationDataType.Integer;
            m_rotation.ID = "rotation";
            m_rotationValidator.Text = String.Format(" {0}", Localization.Translate("InvalidValue"));
            m_rotationValidator.ControlToValidate = m_rotation.ID;

            m_inclination = CreateEditorPartTextBox(90);
            m_inclination.Text = "0";
            m_inclinationValidator = new RangeValidator();
            m_inclinationValidator.MinimumValue = "-90";
            m_inclinationValidator.MaximumValue = "90";
            m_inclinationValidator.Type = ValidationDataType.Integer;
            m_inclination.ID = "inclination";
            m_inclinationValidator.Text = String.Format(" {0}", Localization.Translate("InvalidValue"));
            m_inclinationValidator.ControlToValidate = m_inclination.ID;

            
            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_3Denabled,Localization.Translate("Enable3DMode"))));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("LightStyle"), new Control[] { m_lightstyle }));
            AddToolPaneRow(CreateToolPaneRow(CreateCheckBoxControls(m_isometric, Localization.Translate("Isometric"))));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("Perspective"), new Control[] { m_perspective, new LiteralControl("%"), m_perspectiveValidator }));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("Rotation"), new Control[] { m_rotation, new LiteralControl("&deg;"), m_rotationValidator }));
            AddToolPaneRow(CreateToolPaneRow(Localization.Translate("Inclination"), new Control[] { m_inclination, new LiteralControl("&deg;"), m_inclinationValidator }));
            
            
        }
        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
            m_lightstyle.Enabled = m_3Denabled.Checked;
            m_isometric.Enabled = m_3Denabled.Checked;
            m_rotation.Enabled = m_3Denabled.Checked;
            m_inclination.Enabled = m_3Denabled.Checked;
            m_perspective.Enabled = m_3Denabled.Checked && !m_isometric.Checked;
        }
        
        public override void SyncChanges() {
            EnsureChildControls();
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {
                m_3Denabled.Checked =chartPart.Enable3DMode;
                m_lightstyle.SelectedValue = chartPart.ThreeDLightStyle.ToString();
                m_lightstyle.Enabled = m_3Denabled.Checked;
                m_isometric.Enabled = m_3Denabled.Checked;
                m_isometric.Checked = chartPart.ThreeDIsometric;
                m_perspective.Text = chartPart.ThreeDPerspective.ToString();
                m_rotation.Text = chartPart.ThreeDRotation.ToString();
                m_inclination.Text = chartPart.ThreeDInclination.ToString();
                
            }
        }
        public override bool ApplyChanges() {
            EnsureChildControls();
            ChartPartWebPart chartPart = (ChartPartWebPart)this.WebPartToEdit;
            if (chartPart != null) {
                chartPart.Enable3DMode = m_3Denabled.Checked;
                chartPart.ThreeDLightStyle = (LightStyle)Enum.Parse(typeof(LightStyle), m_lightstyle.SelectedValue);
                chartPart.ThreeDIsometric = m_isometric.Checked;
                chartPart.ThreeDPerspective = int.Parse(m_perspective.Text);
                chartPart.ThreeDRotation = int.Parse(m_rotation.Text);
                chartPart.ThreeDInclination = int.Parse(m_inclination.Text);
            }

            return true;
        }

       
    }
}
