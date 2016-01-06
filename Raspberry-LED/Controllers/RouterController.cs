using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Ping()
        {
            return View();
        }
    }
}