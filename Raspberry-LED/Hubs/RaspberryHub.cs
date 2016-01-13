using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public void ChangeLedWeb(string ledIdInDb, string ison)
        {
            var dbData = db.PinConfigs.Find(int.Parse(ledIdInDb));
            dbData.isOn = !dbData.isOn;
            db.Entry(dbData).State = EntityState.Modified;
            db.SaveChanges();
            Clients.Others.ChangePiLed(ledIdInDb);
            Clients.Others.ChangedLeds(ledIdInDb, ison);
        }
    }
}