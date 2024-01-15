using Client;
internal class Program
{
    static void Main(string[] args)
    {
        var client = new UDPClient();
        client.Run();
        Console.ReadKey();
    }
}
