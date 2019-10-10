using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using Moonbucks.Services;

namespace Moonbucks.SampleClient
{
    class Program
    {
       static async Task Main(string[] args)
        {
            Console.WriteLine("== Welcome to Moonbucks! ==");

            do
            {
                Console.WriteLine("Press 'Y' to order a coffee.");
            }
            while (Console.ReadKey().Key != ConsoleKey.Y);
            Console.WriteLine();
            // for macos
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // The port number(5001) must match the port of the gRPC server.
            var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions { });
            var client = new Moonbucks.Services.OrderService.OrderServiceClient(channel);
            var reply = await client.CreateOrderAsync(
                              new CreateOrderRequest { CoffeeType = "latte", Size = Size.Small });

            Console.WriteLine($"Ordered you a *{reply.Size}* {reply.CoffeeType} - your order is order no {reply.Id}");

            do
            {
                Console.WriteLine("Press 'Y' to upsize to a large coffee.");
            }
            while (Console.ReadKey().Key != ConsoleKey.Y);
            Console.WriteLine();

            // assign our mask (what we will update)
            var mask = FieldMask.FromFieldNumbers<CoffeeOrder>(CoffeeOrder.SizeFieldNumber);    
            reply.Size = Size.Large;
            reply.CoffeeType = "magic"; // wont update since we didnt update the mask.

            // create a request
            var request = new UpdateOrderRequest {
                Order = reply,
                OrderMask = mask
            };

            // call the client
            var updated = await client.UpdateOrderAsync(request);

            Console.WriteLine($"Upsized to a *{updated.Size}* {updated.CoffeeType} - for order order no {updated.Id}");
            Console.WriteLine();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
