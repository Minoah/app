using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class ProductStatusDAO
    {
        public ProductStatusDAO()
        {
            Products = new HashSet<ProductDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProductDAO> Products { get; set; }
    }
}
