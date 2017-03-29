using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBSMVC.Common
{
    public class utils
    {
        // ===== Retrieve the API URL USed for the application based on the current Environment's Server Name ==========
        public static string getCurrentEnvironment()
        {            
            //Returns the Environment ("DEV", "QA", "PROD" or "LOCAL" Vased on the Server Machine Name)
            string serverName = Environment.MachineName.ToUpper();
            string environment = String.Empty;
            switch (serverName)
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
        //-------------------------------------------------------------------------------------------------------------------------


    }
}