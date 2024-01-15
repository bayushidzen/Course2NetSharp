using Course2NetSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Course2NetSharp.Abstract
{
    internal interface IMessageSource
    {
        Task SendAsync(NetMessage message, IPEndPoint ep);
        NetMessage Receive(ref IPEndPoint ep);
    }
}
