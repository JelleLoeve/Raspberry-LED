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
            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential("ftp","ftp");
                request.UploadFile("ftp://192.168.1.3/" + filename, "STOR", filepath);
            }
            return null;
        }

        public enum KAKU
        {
            OneOneOn      = 1381717,
            OneOneOff     = 1381716,
            OneTwoOn      = 1394004,
            OneTwoOff     = 1394005,
            OneThreeOn    = 1397077,
            OneThreeOff   = 1397076,
            OneFourOn     = 1397845,
            OneFourOff    = 1397844
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