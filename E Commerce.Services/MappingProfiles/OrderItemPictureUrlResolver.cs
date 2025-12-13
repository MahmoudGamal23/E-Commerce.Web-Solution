using AutoMapper;
using AutoMapper.Execution;
using E_Commerce.Domain.Entities.OrderModule;
using E_Commerce.Shared.DTOs.OrderDTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services.MappingProfiles
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDTO, string>
    {
        private readonly IConfiguration configuration;

        public OrderItemPictureUrlResolver(IConfiguration configuration) 
        {
            this.configuration = configuration;
        }
        public string Resolve(OrderItem source, OrderItemDTO destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.ProductItemOrdered.PictureUrl)) return string.Empty;

            if (source.ProductItemOrdered.PictureUrl.StartsWith("http")) return source.ProductItemOrdered.PictureUrl;

            var BaseUrl = configuration.GetSection("URLs")["BaseUrl"];
            if (string.IsNullOrEmpty(BaseUrl))
                return string.Empty;

            var PicUrl = $"{BaseUrl}{source.ProductItemOrdered.PictureUrl}";

            return PicUrl;
        }
    }
}
