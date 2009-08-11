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
using System.Linq;
using System.Text;
using System.Web.UI.WebControls.WebParts;

namespace ChartPart {
    public class ChartPartEditorParts: BaseEditorPartCollection {
       
        /// <summary>
        /// Initializes a new instance of the ChartPartEditorParts class.
        /// </summary>
        public  ChartPartEditorParts(): base(
            new ChartPartEditorPart(),
            new ChartStyleEditorPart(),
            new ChartLegendEditorPart(),
            new Chart3DEditorPart(),
            new ChartAdvancedEditorPart()) {
        }
    }
}
