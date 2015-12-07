using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raspberry_LED.Helpers;

namespace Raspberry_LED.Controllers
{
    public class RaspberryController : Controller
    {
        // GET: Raspberry
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

        public ActionResult Config()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                file.SaveAs(path);
                CommonHelpers.FTPUpload(path, fileName);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public string saveConfig(FormCollection pinData)
        {
            foreach (string key in pinData.AllKeys)
            {
                Console.WriteLine("Key" + key);
                Console.WriteLine(pinData[key]);


            }
            //return RedirectToAction("Index");
            return pinData.ToString();
        }
    }
}