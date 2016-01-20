using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Raspberry_LED.Helpers;
using Raspberry_LED.Models;


namespace Raspberry_LED.Hubs
{
    [HubName("Raspberry")]
    public class RaspberryHub : Hub
    {
        PinConfigDBContext db = new PinConfigDBContext();
        [HubMethodName("ChangeLedWeb")]
        public void ChangeLedWeb(string DBID, string ison)
        {
            var DBData = db.PinConfigs.Find(int.Parse(DBID));
            var PinNumber = DBData.PinNumber;
            DBData.isOn = !DBData.isOn;
            db.Entry(DBData).State = EntityState.Modified;
            db.SaveChanges();
            Clients.Others.ChangePiLed(PinNumber);
            Clients.All.ChangedValue(PinNumber, ison == "On" ? "Off" : "On");
        }

        [HubMethodName("SendChangedValue")]
        public void SendChangedValueToClients(string pinnumber, string ison)
        {
            Clients.All.ChangedValue(pinnumber, ison);
        }
    }
}