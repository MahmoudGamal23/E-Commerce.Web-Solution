using AutoMapper;
using E_Commerce.Domain.Entities.OrderModule;
using E_Commerce.Shared.DTOs.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile() 
        { 
          CreateMap<AddressDTO, OrderAddress>().ReverseMap();
            CreateMap<Order, OrderToReturnDTO>()
                  .ForMember(dest => dest.DeliveryMethod, O => O.MapFrom(src => src.DeliveryMethod.ShortName))
                  .ForMember(dest => dest.OrderStatus, O => O.MapFrom(src => src.Status.ToString()));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, O => O.MapFrom(src => src.ProductItemOrdered.ProductName))
                .ForMember(dest => dest.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());



        }

    }
}
