using Akka.Actor;
using System;
using System.Net;

namespace akkkaSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("echo-server-system"))
            {
                var port = 9001;
                var actor = system.ActorOf(Props.Create(() => new EchoService(new IPEndPoint(IPAddress.Any, port))), "echo-service");
                Console.WriteLine("TCP server is listening on *:{0}", port);
                Console.WriteLine("ENTER to exit...");
                Console.ReadLine();
            }
        }
    }
}
