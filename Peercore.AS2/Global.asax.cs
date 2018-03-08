using log4net;
using Peercore.AS2.Util;
using Peercore.Email.DataService;
using System;
using System.IO;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Peercore.AS2
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected OrderDataService OrderDataServ;
        private static System.Timers.Timer aTimer;

        protected void Application_Start()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                OrderDataServ = new OrderDataService();
                HttpContext.Current.Application["OrderDataServ"] = OrderDataServ;

                if(ConfigValues.SendAcknowledgement == "On")
                {
                    SetTimer();

                    //Util.AS2Acknowledge acknowledgeClass = new Util.AS2Acknowledge();
                    //acknowledgeClass.SendPOAcknowledgements();
                }
                
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Error($"Application Init failed. {ex.Message}");
                }
            }

            
            Log.Info("Application Init.");
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(Convert.ToInt32(ConfigValues.AcknowledgeInterval));
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            //aTimer.Start();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            ////Timer Testing Purposes

            //string path = @"c:\temp\MyTest.txt";
            //string str = "The Elapsed event was raised at " + DateTime.Now;
            //using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            //using (StreamWriter sw = new StreamWriter(fs))
            //{
            //    sw.WriteLine(str);
            //}

            Util.AS2Acknowledge acknowledgeClass = new Util.AS2Acknowledge();
            acknowledgeClass.SendPOAcknowledgements();
        }
    }
}
