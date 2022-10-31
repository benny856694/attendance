using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace huaanClient
{
    internal class BroadcastBehavior : WebSocketBehavior
    {
       

        protected override void OnMessage(MessageEventArgs e)
        {
            Sessions.Broadcast(e.Data);
        }

        public static BroadcastBehavior Instance = new Lazy<BroadcastBehavior>(() => new BroadcastBehavior()).Value;
        

        public void Broadcast(string json)
        {
            Sessions?.Broadcast(json);
        }
    }
}
