using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static byte[] result = new byte[1024];
        static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("192.168.0.101");
            Thread.Sleep(5000);
            socket.Connect(new IPEndPoint(ip, 9001));
            Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    Thread.Sleep(2000);
                    int receiveLength = socket.Receive(result);
                    Console.WriteLine("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));
                    Thread.ResetAbort();
                }
            });
            Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    Thread.Sleep(2000);
                    string sendMessage = "client send Message Hellp " + DateTime.Now;
                    socket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    Thread.ResetAbort();
                }
            });

           
            Console.ReadLine();

        }
    }
}
