using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Raspberry_LED.Helpers
{
    public class CommonHelpers
    {
        public string FTPUpload() // TODO: Add the file to the class so that we can upload it
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1"); // TODO: Check if we can store files to a folder structure
            request.Method = WebRequestMethods.Ftp.UploadFile;

            var ftpww = File.ReadAllLines(@"F:\ww.txt"); // I don't want everyone to know my personal password.
            

            request.Credentials = new NetworkCredential("Starlight", ftpww[0]);

            // Copy the contents of the file to the request stream.
            // TODO: File that is temporary stored on the server
            StreamReader sourceStream = new StreamReader("");
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            // TODO: Check if the upload was successfull. If so then store the uploaded file name to the db for easy access when user wants to play it again later. If not then just return with error msg

            response.Close();
            //TODO: Delete the temporary file

            return response.StatusDescription;
        }
        /// <summary>
        /// Helper to check if the current user is me(Starlight) or not
        /// </summary>
        /// <returns>True if user = Starlight, false if user is someone else</returns>
        public bool isStarlight() // Some code only works on my pc so I used a helper class to see if the user running the code is me or not
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return userName == "Starlight";
        }

        // The command types for the socket so the rpi server knows what to do
        public class COMMANDTYPES
        {
            public const string TOGGLELED = "LED";
            public const string LOG = "LOG";
        }
    }
}