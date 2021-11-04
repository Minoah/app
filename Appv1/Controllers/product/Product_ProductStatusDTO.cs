using Appv1.Entities;
using Appv1.Common;

namespace Appv1.Controllers.product
{
    public class Product_ProductStatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public Product_ProductStatusDTO() { }
        public Product_ProductStatusDTO(ProductStatus ProductStatus)
        {

            this.Id = ProductStatus.Id;

            this.Code = ProductStatus.Code;

            this.Name = ProductStatus.Name;

        }
    }

    public class Product_ProductStatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public ProductStatusOrder OrderBy { get; set; }
    }
}