using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;
using NetSharp;

namespace Client2
{
    internal class UDPClient
    {
        static UdpClient _udpClient = new UdpClient();
        static IPEndPoint _iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.2"), 12345);
        static string _from = RandomNameGenerator.GenerateRandomName();
        internal static void Reply()
        {
            byte[] buffer = _udpClient.Receive(ref _iPEndPoint);
            var answer = Encoding.UTF8.GetString(buffer);
            Message message = Message.DeserializeFromJson(answer);
            message.Print();
        }
        internal static Message CreateMessage()
        {
            Console.Write("Введите сообщение: ");
            string messageText = Console.ReadLine();
            Console.Write("Кому Вы хотите отправить сообщение? ");
            string messageTo = Console.ReadLine();
            Message message = new() { Text = messageText, DateTime = DateTime.Now, NicknameFrom = _from, NicknameTo = messageTo };
            return message;
        }

        internal static void Send(Message message)
        {
            string json = message.SerializeMessageToJson();
            var data = Encoding.UTF8.GetBytes(json);
            _udpClient.Send(data, data.Length, _iPEndPoint);
            Console.WriteLine($"Отправлено {data.Length} байт.");
        }
        internal static void SendCopyTo(Message message)
        {
            Console.Write("Введите имя получателя: ");
            var sendTo = Console.ReadLine();
            message.Clone(sendTo);
            string copyJson = message.SerializeMessageToJson();

            var data = Encoding.UTF8.GetBytes(copyJson);
            _udpClient.Send(data, data.Length, _iPEndPoint);
            Console.WriteLine($"Отправлено {data.Length} байт.");
        }
        internal static Message Check()
        {
            byte[] buffer = _udpClient.Receive(ref _iPEndPoint);
            var messageText = Encoding.UTF8.GetString(buffer);
            Message message = Message.DeserializeFromJson(messageText);
            message.Print();
            return message;
        }
        internal async Task Run()
        {
            bool run = true;
            Console.WriteLine("Добро пожаловать в чат.");
            while (run)
            {
                Console.Write("Если Вы хотите отправить сообщение -" +
                    " введите go. Проверить сообщения - check. Если хотите выйти - введите exit: ");
                string command = Console.ReadLine().ToLower();

                switch (command)
                {
                    case "go":
                        var message = CreateMessage();
                        Send(message);
                        Reply();
                        Console.Write("Отправить это сообщение еще кому-то? Yes/no ");
                        var chose = Console.ReadLine().ToLower();
                        if (chose == "yes")
                            SendCopyTo(message);
                        break;
                    case "check":
                        Check();
                        break;
                    case "exit":
                        run = false;
                        Message exit = new() { Text = command, DateTime = DateTime.Now, NicknameFrom = _from, NicknameTo = "Server-1" };
                        string jsonEx = exit.SerializeMessageToJson();
                        var data1 = Encoding.UTF8.GetBytes(jsonEx);
                        _udpClient.Send(data1, data1.Length, _iPEndPoint);
                        Console.WriteLine("Чат завершает работу...");
                        break;
                    default:
                        Console.WriteLine("Неправильная команда. Попробуйте снова.");
                        break;
                }
            }
        }
    }
}
