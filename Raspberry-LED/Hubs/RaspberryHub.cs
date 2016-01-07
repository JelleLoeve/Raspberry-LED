using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Raspberry_LED.Helpers;

namespace Raspberry_LED.Hubs
{
    [HubName("Raspberry")]
    public class RaspberryHub : Hub
    {
        [HubMethodName("ChangeLed")]
        public void ChangeLed(string ledIdInDb)
        {
            Clients.Others.ChangePiLed(ledIdInDb);
        }
    }
}