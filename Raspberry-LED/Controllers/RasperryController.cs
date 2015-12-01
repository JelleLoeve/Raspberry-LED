using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raspberry_LED.Helpers;

namespace Raspberry_LED.Controllers
{
    public class RasperryController : Controller
    {
        // GET: Rasperry
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Ping()
        {
            // Uncomment this code to test the socket connection
            /* SocketHelper _socketHelper = new SocketHelper("127.0.0.1");
            if (_socketHelper.connectToSocket())
            {
                ViewBag.PingResults = PingHelper.Ping("127.0.0.1");
                return View();
            }
            else
            {
                return View("Error");
            } */

            ViewBag.PingResults = PingHelper.Ping("127.0.0.1");
            return View();

        }
    }
}