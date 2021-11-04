using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class ProductDAO
    {
        public ProductDAO()
        {
            AppUserFavoriteProductMappings = new HashSet<AppUserFavoriteProductMappingDAO>();
            Comments = new HashSet<CommentDAO>();
            Orders = new HashSet<OrderDAO>();
            ProductImageMappings = new HashSet<ProductImageMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public decimal? Price { get; set; }
        public long ProductStatusId { get; set; }
        public long StatusId { get; set; }
        public long AppUserId { get; set; }
        public long? AdminId { get; set; }
        public long Quantity { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual AdminDAO Admin { get; set; }
        public virtual AppUserDAO AppUser { get; set; }
        public virtual CategoryDAO Category { get; set; }
        public virtual ProductStatusDAO ProductStatus { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<AppUserFavoriteProductMappingDAO> AppUserFavoriteProductMappings { get; set; }
        public virtual ICollection<CommentDAO> Comments { get; set; }
        public virtual ICollection<OrderDAO> Orders { get; set; }
        public virtual ICollection<ProductImageMappingDAO> ProductImageMappings { get; set; }
    }
}
