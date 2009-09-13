using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Security;
using System.Security.Permissions;
using System.Threading;

namespace ChartPart {
    static class Localization {
        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        internal static string Translate(string key) {
            uint lcid = 1033;
            if (SPContext.Current != null) {
                if (SPContext.Current.Web != null) {
                    lcid = SPContext.Current.Web.Language;
                }
            }
            return Translate(key, lcid);
        }
        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        internal static string Translate(string key, uint lcid) {
            return SPUtility.GetLocalizedString(string.Format(Thread.CurrentThread.CurrentUICulture,"$Resources:ChartPart,{0}",key), "ChartPart", lcid);
        }
    }
}
