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
using System.Collections;

namespace ChartPart {
    public class BaseEditorPartCollection : ReadOnlyCollectionBase{
        public BaseEditorPartCollection(params BaseEditorPart[] args) {
            this.InnerList.AddRange(args);
        }
     
     
    }
}
