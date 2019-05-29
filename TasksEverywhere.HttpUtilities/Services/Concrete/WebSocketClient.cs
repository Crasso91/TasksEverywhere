using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.HttpUtilities.HttpModel;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace TasksEverywhere.HttpUtilities.Services.Concrete
{
    public class WebSocketClient
    {
        public static ILog logger = log4net.LogManager.GetLogger("LoggerInternal");
        private static WebSocket clientWebSocket;
        public static void Init(string url)
        {
            clientWebSocket = new WebSocket(url);
            clientWebSocket.Connect();
            

            logger.Info("WebSocket client initialized for url: " + url);
        }

        public static void SendMessage(SocketMessage message)
        {
            try
            {

                if (clientWebSocket.ReadyState != WebSocketState.Open)
                {
                    clientWebSocket.Close();
                    clientWebSocket.Connect();
                }
                logger.Info("WebSocket sending message: " + message.Stringify());
                clientWebSocket.SendAsync(message.Stringify(), (x) => { logger.Info("WebSocket message sent"); });
                logger.Info("WebSocket Sending message async");
            }
            catch (Exception e)
            {
                logger.Error("WebSocketClient.SendMessage -> ", e);
            }
        }

        public static void Dispose()
        {
            clientWebSocket.Close();
            logger.Info("WebSocket Disposed");
        }

    }
}
