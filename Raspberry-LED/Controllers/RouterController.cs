using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

using Raspberry_LED.Helpers;
using Raspberry_LED.Models;

namespace Raspberry_LED.Controllers
{
    public class RouterController : Controller
    {
        private static PingResultsDBContext db = new PingResultsDBContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Status()
        {
            return View(db.PingResults.ToList());
        }

        [HttpPost]
        public ActionResult Ping(FormCollection postcollect)
        {
            IPAddress ipToPing;
            if (IPAddress.TryParse(postcollect["IP"], out ipToPing))
            {
                //ip address if on dvc
                ViewBag.PingResults1 = "Pinged to: " + ipToPing.ToString();
                ViewBag.PingResults2 = "Result is: " + PingHelper.Ping(ipToPing);
                return View();
            }
            else
            {
                ViewBag.IPAddress = postcollect["IP"];
                ViewBag.ErrorType = "InvalidIP";
                return View("_Error");
            }
        }

        public ActionResult Ping()
        {
            return View();
        }
    }
}