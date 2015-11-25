using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            return View();
        }
    }
}