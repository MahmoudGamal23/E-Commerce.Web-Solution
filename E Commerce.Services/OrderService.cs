using AutoMapper;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.OrderModule;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOs.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IBasketRepository basketRepository;

        public OrderService(IUnitOfWork unitOfWork,IMapper mapper,IBasketRepository basketRepository) 
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.basketRepository = basketRepository;
        }
        public async Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string Email)
        {
            var OrderAddress = mapper.Map<OrderAddress>(orderDTO.Address);
            var Basket = await basketRepository.GetBasketAsync(orderDTO.BasketId);
            if (Basket == null) return Error.NotFound("Basket.NotFound", $"Basket with Id {orderDTO.BasketId} is not found");

            List<OrderItem> OrderItems = new List<OrderItem>();

            foreach (var item in Basket.Items)
            {
                var Product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (Product == null) return Error.NotFound("Product.NotFound", $"Product with Id {item.Id} is not found");
                OrderItems.Add(CreateOrderItem(item, Product));
            }

            var DeliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDTO.DeliveryMethodId);
            if (DeliveryMethod == null) return Error.NotFound("DeliveryMethod.NotFound", $"DeliveryMethod with Id {orderDTO.DeliveryMethodId} is not found");

            var SubTotal = OrderItems.Sum(I => I.Price * I.Quantity);

            var Order = new Order()
            {
                Address = OrderAddress,
                DeliveryMethod = DeliveryMethod,
                SubTotal = SubTotal,
                Items = OrderItems,
                UserEmail = Email
            };

            await unitOfWork.GetRepository<Order, int>().AddAsync(Order);
            var Result = await unitOfWork.SaveChangesAsync() > 0;
            if (!Result) return Error.Failure("Order.Failure", "Order Can Not Be Created");
            return mapper.Map<OrderToReturnDTO>(Order);

        }

        private static OrderItem CreateOrderItem(Domain.Entities.BasketModule.BasketItem item, Product Product)
        {
            return new OrderItem()
            { 
                ProductItemOrdered = new ProductItemOrdered() { ProductId = Product.Id, PictureUrl = Product.PictureUrl, ProductName = Product.Name },
                Price = Product.Price,
                Quantity = item.Quantity

            };
        }
    }
}
