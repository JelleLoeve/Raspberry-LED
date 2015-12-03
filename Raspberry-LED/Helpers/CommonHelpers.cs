using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var ftpww = File.ReadAllLines(@"F:\ww.txt"); // I don't want everyone to know my personal password.

            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential("Starlight", ftpww[0]);
                request.UploadFile("ftp://127.0.0.1/" + filename, "STOR", filepath);
            }
            //            // Copy the contents of the file to the request stream.
            //            // TODO: File that is temporary stored on the server
            //            StreamReader sourceStream = new StreamReader(filepath);
            //            byte[] fileContents = Encoding.ASCII.GetBytes(sourceStream.ReadToEnd());
            //            sourceStream.Close();
            //            request.ContentLength = fileContents.Length;
            //
            //            Stream requestStream = request.GetRequestStream();
            //            requestStream.Write(fileContents, 0, fileContents.Length);
            //            requestStream.Close();
            //
            //            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            //
            //            // TODO: Check if the upload was successfull. If so then store the uploaded file name to the db for easy access when user wants to play it again later. If not then just return with error msg
            //
            //            response.Close();
            //            //TODO: Delete the temporary file
            //
            //            return response.StatusDescription;
            return null;
        }
        /// <summary>
        /// Helper to check if the current user is me(Starlight) or not
        /// </summary>
        /// <returns>True if user = Starlight, false if user is someone else</returns>
        public static bool isStarlight() // Some code only works on my pc so I used a helper class to see if the user running the code is me or not
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