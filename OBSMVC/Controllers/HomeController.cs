﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;


namespace OBSMVC.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            string environment = getCurrentEnvironment();
            
            ////Verify whether we are running in DEV environemnt or not
            //if (!(ConfigurationManager.AppSettings["ServerType"] == null))
            //{
            //    environment = ConfigurationManager.AppSettings["ServerType"].ToString();
            //    if (environment.Equals("Development") || environment.Equals("QA")) { return RedirectToAction("AppSelection", "Home"); }
            //}

            if (environment.Equals("DEV") || environment.Equals("QA")) { return RedirectToAction("AppSelection", "Home"); }
            else { return RedirectToAction("Home", "Home"); }
        }

        [AllowAnonymous]
        public ActionResult AppSelection()
        {
            string environment = "";
            switch (Environment.MachineName.ToUpper())
            {
                case "DSCAPPSQA1":                //QA Server    192.168.43.192
                    environment = "QA";
                    break; 
                case "DSCAPPSPROD1":              //PROD Server  192.168.1.181,  192.168.1.183 and 192.168.1.184
                    environment = "PROD";
                    break;
                case "L-9L28F12":
                    environment = "DEV";
                    break;
                default:                          //Default to the Development Server   192.168.43.43
                    environment = "DEV";
                    break;
            }
            return View("AppSelection", (object)environment);
        }

        // GET: Home/_AppSelectionEnvironment
        [AllowAnonymous] [ChildActionOnly]
        public PartialViewResult _AppSelectionEnvironment(string environmentRow)
        { //This controller will display the Application selections Row for the selected Environment

            environmentApps prodApps = new environmentApps(environmentRow);

            return PartialView(prodApps);
        }


        public ActionResult Home()
        {
            //if (!Request.IsAuthenticated) { return RedirectToAction("Login", "Login"); }

            return View();
        }

        private string getCurrentEnvironment()
        {
            //Returns the Environment ("DEV", "QA", "PROD" or "LOCAL" Vased on the Server Machine Name)
            string environment = String.Empty;
            switch (Environment.MachineName.ToUpper())
            {
                case "DSCAPPSQA1":              //QA Server 192.168.43.192
                    environment = "QA";
                    break;
                case "DSCAPPSPROD1":            //PROD Server  192.168.1.181,  192.168.1.183 and 192.168.1.184
                    environment = "PROD";
                    break;
                case "L-9L28F12":               // FD Local Host
                    environment = "LOCAL";
                    break;
                default:                        //Default to the Development Server   192.168.43.43
                    environment = "DEV";
                    break;
            }

            return environment;
        }


    }

    public class environmentApps {
        public string envName { get; set; }
        public string envDIVid { get; set; }
        public string envColorScheme { get; set; }
        public List<environmentApp> envAppList = new List<environmentApp>();

        public environmentApps(string environment) {
            environment = environment.ToUpper();
            switch (environment) { 
                case "PROD":
                    envName = "Production";
                    envDIVid = "divPROD";     //divDEV  //divQA
                    envColorScheme = "black"; //green   //darkred
                    envAppList.Add(new environmentApp("http://OBSDM/Home/Home", "http://OBSDM/Home/Index", "OBS-DM"));
                    envAppList.Add(new environmentApp("http://OBSPRO/Home/Index", "http://OBSPRO/Home/Index", "OBS-PRO"));
                    envAppList.Add(new environmentApp("http://RedZone/Home/Index", "http://RedZone/Home/Index", "RED-ZONE"));
                    envAppList.Add(new environmentApp("http://METRICDM/Home/Index", "http://METRICDM/Home/Index", "METRIC-DM"));
                    break;
                default:
                case "DEV":
                    envName = "Development";
                    envDIVid = "divDEV";
                    envColorScheme = "green";
                    envAppList.Add(new environmentApp("http://DSCAPPDEV/Home/Home", "http://DSCAPPDEV/Home/Index", "OBS-DM"));
                    envAppList.Add(new environmentApp("http://DSCAPPDEV:81/Home/Index", "http://DSCAPPDEV:81/Home/Index", "OBS-PRO"));
                    envAppList.Add(new environmentApp("http://DSCAPPDEV:82/Home/Index", "http://DSCAPPDEV:82/Home/Index", "RED-ZONE"));
                    envAppList.Add(new environmentApp("http://DSCAPPDEV:83/Home/Index", "http://DSCAPPDEV:83/Home/Index", "METRIC-DM"));
                    break;
                case "QA":
                    envName = "QA";
                    envDIVid = "divQA";
                    envColorScheme = "darkred";
                    envAppList.Add(new environmentApp("http://DSCAPPQA/Home/Home", "http://DSCAPPQA/Home/Dashboard", "OBS-DM"));
                    envAppList.Add(new environmentApp("http://DSCAPPQA:81/Home/Index", "http://DSCAPPQA:81/Home/Index", "OBS-PRO"));
                    envAppList.Add(new environmentApp("http://DSCAPPQA:82/Home/Index", "http://DSCAPPQA:82/Home/Index", "RED-ZONE"));
                    envAppList.Add(new environmentApp("http://DSCAPPQA:83/Home/Index", "http://DSCAPPQA:83/Home/Index", "METRIC-DM"));
                    break;
            }

        }
        // End of Constructors Section
    
    }// End of the "EnvironemntApps" Class

    public class environmentApp
    {
        public string appURL { get; set; }
        public string appURL_display { get; set; }
        public string appName { get; set; }
        public string imageURL { get; set; }
        public environmentApp(string url, string urlName, string app_Name)
        {
            appURL = url;
            appURL_display = urlName;

            switch (app_Name)
            {
                case "OBS-DM":
                    appName = "OBS DM TOOLS";
                    imageURL = "/Images/Resources_Tools_icon-250x250.jpg";
                    break;
                default:
                case "OBS-PRO":
                    appName = "OBS PRO";
                    imageURL = "/Images/Observations_icon.jpg";
                    break;
                case "RED-ZONE":
                    appName = "Red Zone";
                    imageURL = "/Images/Red-Zone_Icon.png";
                    break;
                case "METRIC-DM":
                    appName = "Metric DM Tools";
                    imageURL = "/Images/MetricDM_icon.jpg";
                    break;
            }
        
        } // End of Constructors Section

    }// End of the "EnvironmentApp" Class


}