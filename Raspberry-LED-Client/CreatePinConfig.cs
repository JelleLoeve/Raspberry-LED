using System;
using Raspberry.IO.GeneralPurpose;
using Microsoft.AspNet.SignalR.Client;

namespace Raspberry_LED_Client
{
    public static class CreatePinConfig
    {
//        public static HubConnection hubconnection;
//        public static IHubProxy raspberryHub;
//
//        public CreatePinConfig(HubConnection hubcon, IHubProxy hub)
//        {
//            hubconnection = hubcon;
//            raspberryHub = hub;
//        }

        public static OutputPinConfiguration CreateOutputPinConfiguration(ConnectorPin pin, Action<bool> Onstatuschanged, string type)
        {
            
            return pin.Output().Revert().OnStatusChanged(Onstatuschanged);
        }
    }
}
