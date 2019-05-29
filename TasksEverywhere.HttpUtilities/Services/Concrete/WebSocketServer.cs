using Fleck;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.HttpUtilities.HttpModel;
using log4net;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TasksEverywhere.HttpUtilities.Services.Concrete
{
    public class WebSocketServer
    {
        private static List<IWebSocketConnection> connections = new List<IWebSocketConnection>();
        private static Fleck.WebSocketServer socket;
        public static ILog logger = log4net.LogManager.GetLogger("LoggerInternal");

        public static void Init(string url)
        {
            if (socket == null)
            {
                socket = new Fleck.WebSocketServer(url);
                socket.RestartAfterListenError = true;

                FleckLog.LogAction = (level, message, ex) =>
                {
                    switch (level)
                    {
                        case LogLevel.Debug:
                            logger.Debug("WebSocket -> " + message, ex);
                            break;
                        case LogLevel.Error:
                            logger.Error("WebSocket -> " + message, ex);
                            break;
                        case LogLevel.Warn:
                            logger.Warn("WebSocket -> " + message, ex);
                            break;
                        default:
                            logger.Info("WebSocket -> " + message, ex);
                            break;
                    }
                };

                socket.Start(conn =>
                {
                    conn.OnOpen = () =>
                    {
                        if (connections.FirstOrDefault(x => x.ConnectionInfo.Id == conn.ConnectionInfo.Id) == null)
                            connections.Add(conn);
                        logger.Info("WebSocket -> new connection from: " + conn.ConnectionInfo.ClientIpAddress + " with GUID: " + conn.ConnectionInfo.Id);
                    };

                    conn.OnMessage = message =>
                    {
                        logger.Info("WebSocket -> Message received from: " + conn.ConnectionInfo.ClientIpAddress + " with GUID: " + conn.ConnectionInfo.Id);
                        WebSocketServer.SendMessage(message, conn.ConnectionInfo.Id);
                    };

                    conn.OnClose = () =>
                    {
                        connections.Remove(connections.First(x => x.ConnectionInfo.Id == conn.ConnectionInfo.Id));
                        logger.Info("WebSocket -> close connection from: " + conn.ConnectionInfo.ClientIpAddress + " with GUID: " + conn.ConnectionInfo.Id);
                    };

                    conn.OnBinary = binary =>
                    {
                        WebSocketServer.SendMessage(Encoding.UTF8.GetString(binary), conn.ConnectionInfo.Id);
                    };

                    conn.OnError = error =>
                    {
                        logger.Error("WebSocket -> Error with connection: " + conn.ConnectionInfo.ClientIpAddress + " and GUID: " + conn.ConnectionInfo.Id, error);
                    };

                });
                logger.Info("WebSocket server initialized for url: " + url);
            }
        }

        public static void SendMessage(SocketMessage message)
        {
            logger.Info("WebSocket sending message: " + message.Stringify());
            connections.ForEach(x =>
            {
                logger.Info("To: " + x.ConnectionInfo.ClientIpAddress);
                x.Send(message.Stringify());
            });
        }

        public static void SendMessage(string message, Guid idTofilter)
        {
            logger.Info("WebSocket sending message: " + message.Stringify());
            connections.Where(x=>x.ConnectionInfo.Id != idTofilter).ToList().ForEach(x =>
            {
                logger.Info("To: " + x.ConnectionInfo.ClientIpAddress + " with GUID: " + x.ConnectionInfo.Id);
                x.Send(message);
            });
        }

        public static void Dispose()
        {
            connections.Clear();
            socket.Dispose();
        }
    }
}