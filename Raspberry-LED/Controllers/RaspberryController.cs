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
        private PinConfigDBContext pindb = new PinConfigDBContext();
        private UploadDBContext uploaddb = new UploadDBContext();
        SocketHelper _socketHelper = new SocketHelper("127.0.0.1");
        private string _errorMessage;
        // GET: Raspberry
        public ActionResult Index()
        {
            ViewBag.ErrorType = _errorMessage;
            _errorMessage = "";
            List<object> list = new List<object> {pindb.PinConfigs.ToList(), uploaddb.Uploads.ToList()};

            return View(list);
        }
        public ActionResult Ping()
        {
            // Uncomment this code to test the socket connection
            
            if (_socketHelper.ConnectToSocket())
            {
                ViewBag.PingResults = PingHelper.Ping("127.0.0.1");
                return View();
            }
            else
            {
                ViewBag.ErrorType = "NoConnection";
                return View("_Error");
            }

        }

        public ActionResult Config()
        {
            return View(pindb.PinConfigs.ToList());
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, FormCollection postData)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileType = file.ContentType;
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                var results = uploaddb.Uploads.SqlQuery("SELECT * FROM Uploads WHERE FileName='" + fileName + "'");
                if (!System.IO.File.Exists(path) || results == null)
                {
                    var uploads = new Upload
                    {
                        Alias = postData["alias"],
                        FileName = fileName,
                        Type = fileType
                    };
                    uploaddb.Uploads.Add(uploads);
                    uploaddb.SaveChanges();
                    file.SaveAs(path);
                    CommonHelpers.FTPUpload(path, fileName);
                }
                _socketHelper.SendToServer(CommonHelpers.COMMANDTYPES.MUSIC, fileName);
            }
            else if (postData["SelectedFileID"] != null)
            {
                int id = int.Parse(postData["SelectedFileID"]);
                var result = uploaddb.Uploads.Find(id);
                _socketHelper.SendToServer(CommonHelpers.COMMANDTYPES.MUSIC, result.FileName);
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult saveConfig(FormCollection pinData)
        {
            var i = 1;
            foreach (var key in pinData.AllKeys)
            {
                if (key != "saveConfig")
                {
                    string inputedValue = pinData[key];
                    var test = pindb.PinConfigs.Find(i);
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
            }
            pindb.SaveChanges();
            return null;
        }

        public ActionResult ChangeLed(int? id)
        {
            if (_socketHelper.ConnectToSocket())
            {
                _socketHelper.SendToServer(CommonHelpers.COMMANDTYPES.TOGGLELED, id);
                _errorMessage = "Toggled led";
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }
            
        }
    }
}