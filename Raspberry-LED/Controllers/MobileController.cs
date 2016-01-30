using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raspberry_LED.Models;
using Raspberry_LED.Helpers;

namespace Raspberry_LED.Controllers
{
    public class MobileController : Controller
    {
        private MobileConnectDBContext mobiledb = new MobileConnectDBContext();

        // GET: Mobile
        public ActionResult Index()
        {
            return View(mobiledb.MobileConnect.ToList());
        }
        [HttpPost]
        public ActionResult saveDevice(FormCollection device)
        {
            mobiledb.MobileConnect.Add(new MobileConnect
            {
                Name = device["name"],
                IPAddress = device["IPAddress"],
            });
            mobiledb.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}