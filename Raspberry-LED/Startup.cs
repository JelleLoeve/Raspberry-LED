using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

using Raspberry_LED.Helpers;
using Raspberry_LED.Models;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

[assembly: OwinStartupAttribute(typeof(Raspberry_LED.Startup))]
namespace Raspberry_LED
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            app.MapSignalR();
            var pingMobileThreat = new Thread(pingMobile);
            pingMobileThreat.Start();
        }
        public void pingMobile()
        {
            Thread.Sleep(5000);
            MobileConnectDBContext mobiledb = new MobileConnectDBContext();
            while (true)
            {
                List<string> devices = new List<string>();
                var result = mobiledb.MobileConnect.SqlQuery("SELECT * FROM MobileConnects").ToList();
                foreach (var item in result)
                {
                    if (!devices.Contains(item.IPAddress))
                    {
                        devices.Add(item.IPAddress);
                    }
                }
                foreach (var item in devices)
                {
                    PingHelper.MobilePing(item);
                }
                Thread.Sleep(30 * 1000);
            }
        }
    }
}
