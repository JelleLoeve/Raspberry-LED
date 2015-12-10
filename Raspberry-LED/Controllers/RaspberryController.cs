using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Provider;
using Raspberry_LED.Helpers;
using Raspberry_LED.Models;

namespace Raspberry_LED.Controllers
{
    public class RaspberryController : Controller
    {
        private PinConfigDBContext db = new PinConfigDBContext();
        // GET: Raspberry
        public ActionResult Index()
        {
            return View(db.PinConfigs.ToList());
        }
        public ActionResult Ping()
        {
            // Uncomment this code to test the socket connection
            SocketHelper _socketHelper = new SocketHelper("127.0.0.1");
            if (_socketHelper.ConnectToSocket())
            {
                ViewBag.PingResults = PingHelper.Ping("127.0.0.1");
                return View();
            }
            else
            {
                return View("Error");
            }
            
            //ViewBag.PingResults = PingHelper.Ping("127.0.0.1");
            return View();

        }

        public ActionResult Config()
        {
            return View(db.PinConfigs.ToList());
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
        public ActionResult saveConfig(FormCollection pinData)
        {
            //ViewBag.ErrorMessage = "Not yet fully working";
            //return View("_Error");
            var i = 1;
            foreach (var key in pinData.AllKeys)
            {
                if (key == "saveConfig") continue;
                string inputedValue = pinData[key];
                var test = db.PinConfigs.Find(i);
                test.color = "";
                test.isSet = false;
                if (inputedValue != string.Empty && inputedValue != "saveConfig")
                {
                    test.color = pinData[key];
                    test.isSet = true;
                    Debug.WriteLine(pinData[key]);
                }
                i++;
            }
            db.SaveChanges();
            return null;
        }

        public ActionResult ChangeLed(int? id)
        {

            return RedirectToAction("Index");
        }
    }
}