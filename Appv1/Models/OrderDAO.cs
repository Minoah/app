using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class OrderDAO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long AppUserId { get; set; }
        public long ProductId { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal? LongtitudeDeliver { get; set; }
        public decimal? LatitudeDeliver { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long OrderStatusId { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public Guid RowId { get; set; }
        public long Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual OrderStatusDAO OrderStatus { get; set; }
        public virtual ProductDAO Product { get; set; }
    }
}
