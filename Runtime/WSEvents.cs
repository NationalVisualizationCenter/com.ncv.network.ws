namespace NCV.Network.WS
{
    public enum WSNetworkState
    {
        NONE = 0,
        OK,
        DISCONNECT,
        ERROR,
    }
    public delegate void WSStateCliEventHandler(WSNetworkState state);
    public delegate void NetworkErrorEventHandler(NetworkErrorType state);

    public enum NetworkErrorType
    {
        NONE = 0,
        WS,
        HTTP
    }
}
