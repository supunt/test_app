using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peercore.Email.Common
{
    public class Util
    {
        public static int ExchnageVersion
        {
            get { return int.Parse(ConfigurationManager.AppSettings["ExchnageVersion"].ToString().Trim()); }
        }
        public static string UserName
        {
            get { return ConfigurationManager.AppSettings["Username"].ToString().Trim(); }
        }
        public static string Password
        {
            get { return ConfigurationManager.AppSettings["Password"].ToString().Trim(); }
        }
        public static string Domain
        {
            get { return ConfigurationManager.AppSettings["Domain"].ToString().Trim(); }
        }
        public static string Email
        {
            get { return ConfigurationManager.AppSettings["Email"].ToString().Trim(); }
        }
        public static string PFDOrderSaveLocation
        {
            get { return ConfigurationManager.AppSettings["PFDOrderSaveLocation"].ToString().Trim(); }
        }
        public static string PFDOrderEmail
        {
            get { return ConfigurationManager.AppSettings["PFDOrderEmail"].ToString().Trim(); }
        }
        public static string PFDOrderInbox
        {
            get { return ConfigurationManager.AppSettings["PFDOrderInbox"].ToString().Trim(); }
        }
        public static string PFDOrderArchiveFolder
        {
            get { return ConfigurationManager.AppSettings["PFDOrderArchiveFolder"].ToString().Trim(); }
        }
        public static string PFDOrderNonRelativeMailFolder
        {
            get { return ConfigurationManager.AppSettings["PFDOrderNonRelativeMailFolder"].ToString().Trim(); }
        }
        public static string PFDOrderArchiveLocation
        {
            get { return ConfigurationManager.AppSettings["PFDOrderArchiveLocation"].ToString().Trim(); }
        }

        public static int LoopInterval
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["LoopInterval"]); }
        }
        public static int InitialSleep
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["InitialSleep"]); }
        }
        public static int SyncTimeout
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["SyncTimeout"]); }
        }
        
    }
}
