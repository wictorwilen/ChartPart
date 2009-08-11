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

namespace ChartPart {
    [Flags]
    public enum LockDownModes {
        None = 0x00,
        Colors = 0x01,
        ChartType = 0x02,
        ThreeD = 0x04,
        Full = 0xFF
    }
}
