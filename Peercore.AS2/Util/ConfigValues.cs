using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Peercore.AS2.Util
{
    public class ConfigValues
    {
        public static string DropLocation { get { return WebConfigurationManager.AppSettings["DropLocation"]; } }
        public static Uri DestinationUri { get {
            Uri uri = new Uri(WebConfigurationManager.AppSettings["Uri"]);
            return uri;
        }
        }
        public static string PickLocation { get { return WebConfigurationManager.AppSettings["PickLocation"]; } }
        public static string LocalFrom { get { return WebConfigurationManager.AppSettings["LocalFrom"]; } }
        public static string LocalTo { get { return WebConfigurationManager.AppSettings["LocalTo"]; } }
        public static string SigningCertFilename { get { return WebConfigurationManager.AppSettings["SigningCertFilename"]; } }
        public static string SigningCertPassword { get { return WebConfigurationManager.AppSettings["SigningCertPassword"]; } }
        public static string RecipientCertFilename { get { return WebConfigurationManager.AppSettings["RecipientCertFilename"]; } }
        public static string RecipientPubCertFilename { get { return WebConfigurationManager.AppSettings["RecipientPubCertFilename"]; } }
        public static string RecipientCertPassword { get { return WebConfigurationManager.AppSettings["RecipientCertPassword"]; } }
        public static string LoggerID { get { return WebConfigurationManager.AppSettings["LggerID"]; } }
        public static string CertificateFilePath { get { return WebConfigurationManager.AppSettings["CertificateFilePath"]; } }


        public static string SendAcknowledgement { get { return WebConfigurationManager.AppSettings["SendAcknowledgement"]; } }
        public static string AcknowledgeInterval { get { return WebConfigurationManager.AppSettings["AcknowledgeInterval"]; } }
        public static Uri BidvestUri{ get { Uri uri = new Uri(WebConfigurationManager.AppSettings["BidvestUri"]); return uri;}}
        
    }
}