using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orders.Grpc;

namespace Orders.Services
{
    public class OrderService : Orders.Grpc.OrderService.OrderServiceBase
    { 
        private static Dictionary<long, CoffeeOrder> _orders = new Dictionary<long, CoffeeOrder>();
        private static List<IServerStreamWriter<CoffeeOrder>> _subscribers = new List<IServerStreamWriter<CoffeeOrder>>();
        private ILogger _logger;

        public OrderService(ILogger<OrderService> logger)
        {
            this._logger = logger;
        }

        public override Task<CoffeeOrder> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
        {
            this._logger.LogInformation($"Updating order {request.Order.Id}...");

            // setup what we're allowed to update.
            var updateMask = FieldMask.FromFieldNumbers<CoffeeOrder>(CoffeeOrder.SizeFieldNumber);
            var mask = request.OrderMask.Intersection(updateMask);

            // mergce
            mask.Merge(request.Order, _orders[request.Order.Id]);

            _subscribers.ForEach(async s =>
            {
                await s.WriteAsync(_orders[request.Order.Id]);

            });

            return Task.FromResult(_orders[request.Order.Id]);
        }

        public override async Task SubscribeOrders(SubscribeOrdersRequest request, IServerStreamWriter<CoffeeOrder> responseStream, ServerCallContext context)
        {
            this._logger.LogInformation($"Subscribing to order...");
            _subscribers.Add(responseStream);

            context.CancellationToken.Register(() =>
            {
                this._logger.LogInformation($"Cancelling subscription.");
                _subscribers.Remove(responseStream);
            });

            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
            
        }

        public override Task<CoffeeOrder> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            this._logger.LogInformation($"Creating order: {request.Size} {request.CoffeeType}");
            
            var coffeeOrder = new CoffeeOrder
            {
                Id = _orders.Count() + 1,
                CoffeeType = request.CoffeeType,
                Size = request.Size
            };

            _orders[coffeeOrder.Id] = coffeeOrder;

            _subscribers.ForEach(async s =>
            {
                await s.WriteAsync(coffeeOrder);

            });
          
            return Task.FromResult(coffeeOrder);
        }
    }
}
