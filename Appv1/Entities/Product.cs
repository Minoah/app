using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Appv1.Common;

namespace Appv1.Entities
{
    public class Product : DataEntity, IEquatable<Category>
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
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public double Distance { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual Category Category { get; set; }
        public virtual ProductStatus ProductStatus { get; set; }
        public virtual Status Status { get; set; }
        public bool Equals(Category other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
    public class ProductFilter : FilterEntity
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProductOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Description = 3,
        Category = 4,
        Status = 5,
        ProductStatus = 6,
        Price = 7,
        AppUser = 8,
        Admin = 9,
        Quantity = 10,
        Address = 11,
        Longitude = 12,
        Latitude = 13,
        CreatedAt = 14,
        UpdatedAt = 15,
    }

    [Flags]
    public enum ProductSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Description = E._3,
        Category = E._4,
        Status = E._5,
        ProductStatus = E._6,
        Price = E._7,
        AppUser = E._8,
        Admin = E._9,
        Quantity = E._10,
        Adderss = E._11,
        Longitude = E._12,
        Latitude = E._13,
        CreatedAt = E._14,
        UpdatedAt = E._15,
        Address = E._16,
        RowId = E._17,
    }
}
