using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.DTOs.OrderDTOs
{
    public record OrderToReturnDTO (Guid Id, string UserEmail,ICollection<OrderItemDTO>Item,AddressDTO Address,
        string DeliveryMethod,string OrderStatus,DateTimeOffset OrderData , decimal SubTotal,decimal Total);
    
}
