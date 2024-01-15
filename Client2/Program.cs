using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new UDPClient();
            client.Run();
            Console.ReadKey();
        }
    }
}
