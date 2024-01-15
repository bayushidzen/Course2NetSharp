using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppLib
{
    public class Server<T>
    {
        Dictionary<string, T> clients = [];
        private readonly IMessageSourceServer<T> _messageSource;
        private T ep;
        public Server(IMessageSourceServer<T> messageSource)
        {
            _messageSource = messageSource;
            ep = _messageSource.CreateEndpoint();
        }

        bool work = true;
        public void Stop()
        {
            work = false;
        }

        private async Task Register(NetMessage message)
        {
            Console.WriteLine($"Регистрирация нового пользователя = {message.NickNameFrom}");

            if (clients.TryAdd(message.NickNameFrom, _messageSource.CopyEndpoint(message.EndPoint)))
            {
                using (ChatContext context = new ChatContext())
                {
                    context.Users.Add(new User() { FullName = message.NickNameFrom });
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task RelyMessage(NetMessage message)
        {
            if (clients.TryGetValue(message.NickNameTo, out T ep))
            {
                int? id = 0;
                using (var ctx = new ChatContext())
                {
                    var fromUser = ctx.Users.First(x => x.FullName == message.NickNameFrom);
                    var toUser = ctx.Users.First(x => x.FullName == message.NickNameTo);
                    var msg = new Message { UserFrom = fromUser, UserTo = toUser, IsSent = false, Text = message.Text };
                    ctx.Messages.Add(msg);
                    ctx.SaveChanges();
                    id = msg.MessageId;
                }
                message.Id = id;
                await _messageSource.SendAsync(message, ep);
                Console.WriteLine($"Сообщение переслано от = {message.NickNameFrom} to = {message.NickNameTo}");
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }
        }

        static async Task ConfirmMessageReceived(int? id)
        {
            Console.WriteLine("Сообщение получено, id = " + id);
            using (var ctx = new ChatContext())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.MessageId == id);
                if (msg != null)
                {
                    msg.IsSent = true;
                    await ctx.SaveChangesAsync();
                }
            }
        }

        private async Task ProcessMessage(NetMessage message)
        {
            switch (message.Command)
            {
                case Command.Register: await Register(message); break;
                case Command.Message: await RelyMessage(message); break;
                case Command.Confirmation: await Server<T>.ConfirmMessageReceived(message.Id); break;
            }
        }

        public async Task Start()
        {
            Console.WriteLine("Сервер ожидает сообщения ");
            while (work)
            {
                try
                {
                    var message = _messageSource.Receive(ref ep);
                    Console.WriteLine(message.ToString());
                    await ProcessMessage(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
