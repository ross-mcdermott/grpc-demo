using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moonbucks.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonbucks.Order.Services
{
    public class OrderService : Moonbucks.Services.OrderService.OrderServiceBase
    {
        private static Dictionary<long, CoffeeOrder> _orders = new Dictionary<long, CoffeeOrder>();
        private readonly ILogger<OrderService> _logger;

        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }



        public override Task<CoffeeOrder> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            var coffeeOrder = new CoffeeOrder
            {
                Id = _orders.Count() + 1,
                CoffeeType = request.CoffeeType,
                Size = request.Size
            };

            _orders[coffeeOrder.Id] = coffeeOrder;

            return Task.FromResult(coffeeOrder);
        }

        public override Task<CoffeeOrder> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
        {
            var mask = request.OrderMask.Intersection(FieldMask.FromFieldNumbers<CoffeeOrder>(CoffeeOrder.SizeFieldNumber));

            request.OrderMask.Merge(request.Order, _orders[request.Order.Id]);

            return Task.FromResult(_orders[request.Order.Id]);
        }

    }
}
