using System;
using System.Collections.Generic;
using System.Text;
using Iterum.Logs;
using NativeWebSocket;

namespace NCV.Network.WS
{
    public class WSNetworkClient
    {
        private WebSocketState State => net?.State ?? WebSocketState.Closed;
        private WSNetworkState wsState = WSNetworkState.NONE;

        public WSNetworkState WsNetworkState
        {
            get => wsState;
            set
            {
                if (value != wsState)
                {
                    wsState = value;
                    OnStateChanged?.Invoke(wsState);
                }
            }
        }
        private WebSocket net;

        public event Action Connected;
        public event Action<WebSocketCloseCode> Disconnected;
        public event Action<byte[]> Received;
        public event WSStateCliEventHandler OnStateChanged;

        private DispatcherTimer clientHeartTimer;

        private static byte[] ping = Encoding.UTF8.GetBytes("ping");

        public void Connect(string url, Dictionary<string, string> headers)
        {
            headers ??= new Dictionary<string, string>();

            net = new WebSocket(url, headers);
            net.OnOpen += Client_Connected;
            net.OnClose += Client_Disconnected;
            net.OnError += Client_Error;

            net.OnMessage += Client_Received;

            Log.Info("WS", $"Connecting '{url}'...  ");

            net?.Connect();
        }

        public void Disconnect()
        {
            net?.Close();
        }

        public void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            net?.DispatchMessageQueue();
#endif
        }

        public void Send(byte[] data)
        {
            // Log.Debug("Full DEC", string.Join(" ", data));
            // Log.Debug("Full HEX", BitConverter.ToString(data).Replace("-", " "));
            net?.Send(data);
        }


        private void Client_Error(string errorMsg)
        {
            Log.Error("WS", $"{errorMsg}");
            WsNetworkState = WSNetworkState.ERROR;
        }

        private void Client_Received(byte[] data)
        {
            Received?.Invoke(data);
        }

        private void Client_Connected()
        {

            Log.Info("WS", "Connected");
            WsNetworkState = WSNetworkState.OK;
            Connected?.Invoke();

        }

        private void Client_Disconnected(WebSocketCloseCode closeCode)
        {
            Log.Info("WS", $"Disconnected {closeCode.ToString()}");
            if (closeCode != WebSocketCloseCode.Normal)
            {
                WsNetworkState = WSNetworkState.DISCONNECT;
                Disconnected?.Invoke(closeCode);
            }
        }

        private void Client_SendClientHeart(object sender, EventArgs e)
        {
            Log.Debug($"heartbeat {State}");

            if (State == WebSocketState.Open)
            {
                Send(ping);
            }
            else
            {
                WsNetworkState = WSNetworkState.ERROR;
            }
        }


    }
}
