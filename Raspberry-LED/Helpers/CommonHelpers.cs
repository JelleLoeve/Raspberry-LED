using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Web;

namespace Raspberry_LED.Helpers
{
    public static class CommonHelpers
    {

        public static string FTPUpload(string filepath, string filename) // TODO: Add the file to the class so that we can upload it
        {
            //FtpWebRequest request = =WebRequest.Create("ftp://127.0.0.1/" + filename);
            //request.Method = WebRequestMethods.Ftp.UploadFile;

            //var ftpww = File.ReadAllLines(@"F:\ww.txt"); // I don't want everyone to know my personal password.

            using (WebClient request = new WebClient())
            {
               // request.Credentials = new NetworkCredential("Starlight", ftpww[0]);
                request.UploadFile("ftp://127.0.0.1/" + filename, "STOR", filepath);
                var response = request.ResponseHeaders;
                SocketHelper _socketHelper = new SocketHelper("127.0.0.1");
                _socketHelper.SendToServer(COMMANDTYPES.MUSIC, filename);
            }
            
            return null;
        }

        // The command types for the socket so the rpi server knows what to do
        public class COMMANDTYPES
        {
            public const string MUSIC = "PLAY";
            public const string TOGGLELED = "LED";
            public const string LOG = "LOG";
        }
    }
}