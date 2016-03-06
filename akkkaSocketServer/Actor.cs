/************
** auth:wuxiaoyan
** date:2016/3/5 23:14:35
** desc:
** Ver:V1.0.0
************/
using Akka.Actor;
using Akka.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace akkkaSocketServer
{
    public class EchoService : ReceiveActor
    {
        private readonly IActorRef _manager = Context.System.Tcp();

        public EchoService(EndPoint endpoint)
        {
            _manager.Tell(new Tcp.Bind(Self, endpoint));

            // To behave as TCP listener, actor should be able to handle Tcp.Connected messages
            Receive<Tcp.Connected>(connected =>
            {
                Console.WriteLine("Remote address {0} connected", connected.RemoteAddress);
                Sender.Tell(new Tcp.Register(Context.ActorOf(Props.Create(() => new EchoConnectionHandler(connected.RemoteAddress, Sender)))));
            });
        }
    }

    public class EchoConnectionHandler : ReceiveActor
    {
        public EchoConnectionHandler(EndPoint remote, IActorRef connection)
        {
            // we want to know when the connection dies (without using Tcp.ConnectionClosed)
            Context.Watch(connection);
            var byteString = new ByteStringBuilder();
            byteString.Append(Encoding.Default.GetBytes("tian am"));
            var sendStr=byteString.Result();
            Receive<Tcp.Received>(received =>
            {
                var text = Encoding.UTF8.GetString(received.Data.ToArray()).Trim();
                Console.WriteLine("Received '{0}' from remote address [{1}]", text, remote);
                if (text == "exit")
                    Context.Stop(Self);
                else
                    Sender.Tell(Tcp.Write.Create(sendStr));
            });
            Receive<Tcp.ConnectionClosed>(closed =>
            {
                Console.WriteLine("Stopped, remote connection [{0}] closed", remote);
                Context.Stop(Self);
            });
            Receive<Terminated>(terminated =>
            {
                Console.WriteLine("Stopped, remote connection [{0}] died", remote);
                Context.Stop(Self);
            });
        }
    }

}
