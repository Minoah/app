using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class AppUserFavoriteProductMappingDAO
    {
        public long AppUserId { get; set; }
        public long ProductId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual ProductDAO Product { get; set; }
    }
}
