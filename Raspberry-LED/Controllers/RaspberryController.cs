using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public ActionResult Upload(HttpPostedFileBase[] files, FormCollection postData)
        {
            
            if ((files != null) && (files.Length == 1 || files.Length == 2))
            {
                var fileToPlay = string.Empty;
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fileSort = Path.GetExtension(file.FileName);
                    var fileType = string.Empty;
                    switch (fileSort)
                    {
                        case ".mp3":
                        case ".ogg":
                        case ".wav":
                        case ".flac":
                            fileType = "audio";
                            break;
                        case ".mp4":
                        case ".mkv":
                        case ".avi":
                        case ".wmv":
                        case ".mov":
                        case ".mpg":
                            fileType = "video";
                            break;
                        case ".srt":
                        case ".ass":
                        case ".sub":
                            fileType = "subtitles";
                            break;

                        default:
                            fileType = "unknown";
                            break;
                    }
                    if (fileType == "video" || fileType == "audio")
                    {
                        fileToPlay = fileName;
                    }

                    var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    var result = uploaddb.Uploads.SqlQuery("SELECT * FROM Uploads WHERE FileName='" + fileName + "'");
                    Task<List<Upload>> resultslist = result.ToListAsync();
                    var results = resultslist.Result;
                    if (!System.IO.File.Exists(path) || results.Count == 0)
                    {
                        file.SaveAs(path);
                        CommonHelpers.FTPUpload(path, fileName);
                        uploaddb.Uploads.Add(new Upload
                        {
                            Alias = postData["alias"],
                            FileName = fileName,
                            Type = fileSort
                        });
                        uploaddb.SaveChanges();
                    }
                }
                SocketSendAndRecieve thread1 = new SocketSendAndRecieve(CommonHelpers.COMMANDTYPES.MUSIC, fileToPlay);
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
                if (key != "saveConfig" && !key.Contains("t")) // This is a text input so it is a name
                {
                    string inputedValue = pinData[key];
                    if (i%2 == 0)
                    {
                        i++;
                    }
                    var test = pindb.PinConfigs.Find(i / 2);
                    test.Name = "";
                    test.isSet = false;
                    if (inputedValue != string.Empty && inputedValue != "saveConfig")
                    {
                        test.Name = pinData[key];
                        test.Type = pinData[key + "t"];
                        test.isSet = true;
                        Debug.WriteLine(pinData[key]);
                    }
                    i++;
                }
                else // This is a type so it is either LED or Button
                {
                    i++;
                }
            }
            pindb.SaveChanges();
            return RedirectToAction("Config");
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