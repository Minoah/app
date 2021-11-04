using Appv1.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Appv1.Common;

namespace Appv1.Controllers.product
{
    public class Product_ProductDTO : DataDTO
    {
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
        public Guid RowId { get; set; }
        public double Distance { get; set; }

        public virtual Product_AdminDTO Admin { get; set; }
        public virtual Product_AppUserDTO AppUser { get; set; }
        public virtual Product_CategoryDTO Category { get; set; }
        public virtual Product_ProductStatusDTO ProductStatus { get; set; }
        public virtual Product_StatusDTO Status { get; set; }
        public Product_ProductDTO() { }
        public Product_ProductDTO(Product Product)
        {
            this.Id = Product.Id;
            this.Code = Product.Code;
            this.Name = Product.Name;
            this.Description = Product.Description;
            this.CategoryId = Product.CategoryId;
            this.Price = Product.Price;
            this.ProductStatusId = Product.ProductStatusId;
            this.StatusId = Product.StatusId;
            this.AdminId = Product.AdminId;
            this.Quantity = Product.Quantity;
            this.Latitude = Product.Latitude;
            this.Longitude = Product.Longitude;
            this.Address = Product.Address;
            this.CreatedAt = Product.CreatedAt;
            this.UpdatedAt = Product.UpdatedAt;
            this.RowId = Product.RowId;
            this.Distance = Product.Distance;
            this.Admin = Product.Admin == null ? null : new Product_AdminDTO(Product.Admin);
            this.AppUser = Product.AppUser == null ? null : new Product_AppUserDTO(Product.AppUser);
            this.Category = Product.Category == null ? null : new Product_CategoryDTO(Product.Category);
            this.ProductStatus = Product.ProductStatus == null ? null : new Product_ProductStatusDTO(Product.ProductStatus);
            this.Status = Product.Status == null ? null : new Product_StatusDTO(Product.Status);
            this.Errors = Product.Errors;
        }
    }
    public class Product_ProductFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public StringFilter Address { get; set; }
        public string Search { get; set; }
        public IdFilter AdminId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter CategoryId { get; set; }
        public IdFilter ProductStatusId { get; set; }
        public LongFilter Quantity { get; set; }
        public DecimalFilter Longitude { get; set; }
        public DecimalFilter Latitude { get; set; }
        public double Distance { get; set; }
        public DecimalFilter Price { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public List<ProductFilter> OrFilter { get; set; }
        public ProductOrder OrderBy { get; set; }
        public ProductSelect Selects { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public decimal CurrentLongitude { get; set; }
        public decimal CurrentLatitude { get; set; }
    }
}
