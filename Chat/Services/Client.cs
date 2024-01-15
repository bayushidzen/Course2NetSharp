using Course2NetSharp.Abstract;
using Course2NetSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Course2NetSharp.Services
{
    public readonly string _name;
    public IMessageSource _messageSource;
    public IPEndPoint remoteEndPoint;
    public Client(string name, string address, int port)
    {
        this._name = name;
        _messageSource = new UdpMessageSource();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
    }
    public UdpClient udpClientClient = new UdpClient();
    public async Task ClientListener()
    {
        while (true)
        {
            try
            {
                var messageReceived = _messageSource.Receive(ref remoteEndPoint);
                Console.WriteLine($"Получено сообщение от {messageReceived.NickNameFrom}:");
                Console.WriteLine(messageReceived.Text);
                await Confirm(messageReceived, remoteEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении сообщения: " + ex.Message);
            }
        }
    }

    async Task Confirm(NetMessage message, IPEndPoint remoteEndPoint)
    {
        message.Command = Command.Confirmation;
        await _messageSource.SendAsync(message, remoteEndPoint);
    }

    public void Register(IPEndPoint remoteEndPoint)
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
        var message = new NetMessage() { NickNameFrom = _name, NickNameTo = null, Text = null, Command = Command.Register, EndPoint = ep };
        _messageSource.SendAsync(message, remoteEndPoint);
    }

    async Task ClientSender()
    {
        Register(remoteEndPoint);
        while (true)
        {
            try
            {
                Console.Write("Введите  имя получателя: ");
                var nameTo = Console.ReadLine();
                Console.Write("Введите сообщение и нажмите Enter: ");
                var messageText = Console.ReadLine();

                var message = new NetMessage() { Command = Command.Message, NickNameFrom = _name, NickNameTo = nameTo, Text = messageText };
                await _messageSource.SendAsync(message, remoteEndPoint);
                Console.WriteLine("Сообщение отправлено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
            }
        }
    }

    public async Task Start()
    {
        await ClientListener();
        await ClientSender();
    }
}


