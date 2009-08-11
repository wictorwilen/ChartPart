using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace ChartPart {
    static class Localization {
        internal static string Translate(string key) {
            uint lcid = 1033;
            if (SPContext.Current != null) {
                if (SPContext.Current.Web != null) {
                    lcid = SPContext.Current.Web.Language;
                }
            }
            return Translate(key, lcid);
        }
        internal static string Translate(string key, uint lcid) {
            return SPUtility.GetLocalizedString(string.Format("$Resources:ChartPart,{0}",key), "ChartPart", lcid);
        }
    }
}
