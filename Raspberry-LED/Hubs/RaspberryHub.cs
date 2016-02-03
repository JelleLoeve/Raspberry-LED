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
        public void ChangeLedWeb(string dbid, string ison)
        {
            var DBData = db.PinConfigs.Find(int.Parse(dbid));
            var pinNumber = DBData.PinNumber;
            Clients.Others.ChangePiLed(pinNumber);
        }

        [HubMethodName("SendChangedValue")]
        public void SendChangedValueToClients(string pinnumber, string ison)
        {
            var DBData = db.PinConfigs.Find(int.Parse(pinnumber) + 1);
            if (ison.Equals("On") || ison.Equals("Off"))
            {
                DBData.isOn = ison.Equals("On");
            }
            db.Entry(DBData).State = EntityState.Modified;
            db.SaveChanges();
            Clients.Others.ChangedValue(pinnumber, ison);
        }

        [HubMethodName("GetPinStatusServer")]
        public void GetPinStatusFromPi(int pinnumber, string type)
        {
            Clients.Others.GetPinStatus(pinnumber, type);
        }
    }
}