﻿using System;
using Raspberry.IO.GeneralPurpose;

namespace Raspberry_LED_Client
{
    class Helpers
    {
        public static bool IsLinux
        {
            get
            {
                int p = (int) Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

    }
}
