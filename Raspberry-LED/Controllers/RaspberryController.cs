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
        private PingResultsDBContext pingdb = new PingResultsDBContext();
        SocketHelper _socketHelper = new SocketHelper("192.168.1.3");
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
                pingdb.PingResults.Add(new PingResult
                {
                    Date = DateTime.Now.ToString("dd-MM-yyyy"),
                    Time = DateTime.Now.ToString("HH:mm:ss"),
                    IP = "127.0.0.1",
                    Ping = "Couldn't execute ping"
                });
                pingdb.SaveChanges();
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
                    file.SaveAs(path);
                    CommonHelpers.FTPUpload(path, fileName);
                    uploaddb.Uploads.Add(new Upload
                    {
                        Alias = postData["alias"],
                        FileName = fileName,
                        Type = fileType
                    });
                    uploaddb.SaveChanges();
                }
                SocketSendAndRecieve thread1 = new SocketSendAndRecieve(CommonHelpers.COMMANDTYPES.MUSIC, fileName);
                thread1.SendCommand();
                return RedirectToAction("Index");
            }
            else if (postData["SelectedFileID"] != null)
            {
                int id = int.Parse(postData["SelectedFileID"]);
                var result = uploaddb.Uploads.Find(id);
                //_socketHelper.SendToServer(CommonHelpers.COMMANDTYPES.MUSIC, result.FileName);
                SocketSendAndRecieve thread1 = new SocketSendAndRecieve(CommonHelpers.COMMANDTYPES.MUSIC, result.FileName);
                thread1.SendCommand();
                if (thread1.GetRecievedData() == "NoFile")
                {
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), result.FileName);
                    uploaddb.Uploads.SqlQuery($"DELETE Uploads WHERE ID={id}");
                    if (System.IO.File.Exists(path))
                    {
                        CommonHelpers.FTPUpload(path, result.FileName);
                        thread1.SendCommand();
                    }
                    else
                    {
                        ViewBag.ErrorType = "NoFileFound";
                        return View("_Error");
                    }
                }
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
                    test.Name = "";
                    test.isSet = false;
                    if (inputedValue != string.Empty && inputedValue != "saveConfig")
                    {
                        test.Name = pinData[key];
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