using ChatCommonLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatCommonLib.Abstractions
{
    public interface IMessageSourceClient<T>
    {
        Task SendAsync(NetMessage message, T ep);
        NetMessage Receive(ref T ep);
        T CreateEndpoint();
        T GetServer();
    }
}
