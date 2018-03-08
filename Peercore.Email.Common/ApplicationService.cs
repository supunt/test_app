using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peercore.Email.Common
{
    public sealed class ApplicationService
    {
        private static ApplicationService instance;
        public static ApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApplicationService();
                }

                return instance;
            }

        }

        private ApplicationService()
        {
            PopulateConfigSettings();
        }

        private void PopulateConfigSettings()
        {
            this.ConnectionString = ConfigurationManager.ConnectionStrings["IngresConnection"].ConnectionString;
            this.DbProvider = ConfigurationManager.ConnectionStrings["IngresConnection"].ProviderName;
        }

        public string ConnectionString { get; set; }

        public string DbProvider { get; set; }


    }
}
