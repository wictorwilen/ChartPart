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
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Globalization;

namespace ChartPart {
    public class FeatureReceiver : SPFeatureReceiver {

        public override void FeatureInstalled(SPFeatureReceiverProperties properties) {
            
        }
        public override void FeatureUninstalling(SPFeatureReceiverProperties properties) {
            
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties) {
            SPSite site = properties.Feature.Parent as SPSite;
            SPWebApplication webApplication = site.WebApplication;
            AddorRemoveChartSettingsToWebConfig(webApplication, false);
            AddorRemoveChartHandlerToWebConfig(webApplication, false);
            webApplication.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
            webApplication.Update();
            
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties) {
            SPSite site = properties.Feature.Parent as SPSite;
            SPWebApplication webApplication = site.WebApplication;
            AddorRemoveChartSettingsToWebConfig(webApplication, true);
            AddorRemoveChartHandlerToWebConfig(webApplication, true);
            webApplication.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
            webApplication.Update();
        }

        // idea from Tony Bierman 
        private static void AddorRemoveChartHandlerToWebConfig(SPWebApplication webApplication, bool removeModification) {
            
            string asmDetails = string.Format(CultureInfo.InvariantCulture,
                "System.Web.UI.DataVisualization.Charting.ChartHttpHandler, System.Web.DataVisualization, Version={0}, Culture=neutral,PublicKeyToken={1}",
                new object[] { "3.5.0.0", "31bf3856ad364e35" });

            SPWebConfigModification modification = new SPWebConfigModification(
                "add[@path='ChartImg.axd']",
                "configuration/system.web/httpHandlers");

            modification.Owner = "ChartPart";
            modification.Sequence = 0;
            modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;

            modification.Value = string.Format(
                CultureInfo.InvariantCulture,
                "<add verb=\"{0}\" path=\"{1}\" type=\"{2}\" validate=\"{3}\"/>",
                new object[] { "GET,HEAD,POST", "ChartImg.axd", asmDetails, "false" });            

            if (removeModification) {
                webApplication.WebConfigModifications.Remove(modification);
            }
            else {
                webApplication.WebConfigModifications.Add(modification);
            }
            
        }

        private static void AddorRemoveChartSettingsToWebConfig(SPWebApplication webApplication, bool removeModification) {


            // this is to make sure that we have the appSettings, not there by default in WSS 3.0
            // and I don't care removing it afterwards
            SPWebConfigModification appSettingsMod = new SPWebConfigModification("appSettings", "configuration");
            appSettingsMod.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureSection;
            appSettingsMod.Owner = "ChartPart";
            appSettingsMod.Sequence = 0;
            webApplication.WebConfigModifications.Add(appSettingsMod);
            

            string keyValue = string.Format(CultureInfo.InvariantCulture,
                "storage={0};timeout={1};",
                new object[] { "memory", "20" });

            SPWebConfigModification modification = new SPWebConfigModification(
                "add[@key='ChartImageHandler']",
                "configuration/appSettings");

            modification.Owner = "ChartPart";
            modification.Sequence = 1;
            modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;

            modification.Value = string.Format(
                CultureInfo.InvariantCulture,
                "<add key=\"{0}\" value=\"{1}\"/>",
                new object[] { "ChartImageHandler", keyValue});

            if (removeModification) {
                webApplication.WebConfigModifications.Remove(modification);
            }
            else {                
                webApplication.WebConfigModifications.Add(modification);
            }
            
            
        }
    }
}











