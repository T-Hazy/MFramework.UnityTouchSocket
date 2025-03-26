﻿using TouchSocket.JsonRpc;
using TouchSocket.Rpc;

namespace Assets.UnityTouchSocket.RpcClientServer
{
    public partial class ReverseJsonRpcServer : RpcServer
    {
        [JsonRpc(MethodInvoke = true)]
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
