using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderSubscriber
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // for macos
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // The port number(5001) must match the port of the gRPC server.
            var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions { });
            var client = new Orders.Grpc.OrderService.OrderServiceClient(channel);

            while (true)
            {
                try
                {
                    Console.WriteLine("Subscribing order...");

                    var cts = new CancellationTokenSource();
                    

                    var replies = client.SubscribeOrders(new Orders.Grpc.SubscribeOrdersRequest  {}, cancellationToken: cts.Token);
                    try
                    {
                        while (await replies.ResponseStream.MoveNext(cts.Token))
                        {
                            Console.WriteLine($"Got an order - {replies.ResponseStream.Current.CoffeeType} {replies.ResponseStream.Current.Size} [{replies.ResponseStream.Current.Id}]");
                        }
                    }
                    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                    {
                        Console.WriteLine("Stream cancelled.");
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine("Waiting for server");
                }


                await Task.Delay(5000);
            }



        }
    }
}
