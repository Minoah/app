using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class ImageDAO
    {
        public ImageDAO()
        {
            ProductImageMappings = new HashSet<ProductImageMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public long StatusId { get; set; }

        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<ProductImageMappingDAO> ProductImageMappings { get; set; }
    }
}
