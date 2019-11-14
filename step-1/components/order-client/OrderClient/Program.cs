using Grpc.Net.Client;
using Orders.Grpc;
using System;
using System.Threading.Tasks;

namespace OrderClient
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
                    
                    var order = await client.CreateOrderAsync(
                               new CreateOrderRequest { CoffeeType = "latte", Size = Size.Small });
                    Console.WriteLine($"Added order...{order.Id}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Waiting for server");
                }
                

                await Task.Delay(7000);
            }

            
         

        }
    }
}
