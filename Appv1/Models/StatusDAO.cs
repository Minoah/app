using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class StatusDAO
    {
        public StatusDAO()
        {
            Admins = new HashSet<AdminDAO>();
            AppUsers = new HashSet<AppUserDAO>();
            Comments = new HashSet<CommentDAO>();
            Images = new HashSet<ImageDAO>();
            Products = new HashSet<ProductDAO>();
            Categories = new HashSet<CategoryDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AdminDAO> Admins { get; set; }
        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
        public virtual ICollection<CommentDAO> Comments { get; set; }
        public virtual ICollection<ImageDAO> Images { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
        public virtual ICollection<CategoryDAO> Categories { get; set; }
    }
}
