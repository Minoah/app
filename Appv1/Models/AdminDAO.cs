using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class AdminDAO
    {
        public AdminDAO()
        {
            AdminNotifications = new HashSet<AdminNotificationDAO>();
            Products = new HashSet<ProductDAO>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? SexId { get; set; }
        public DateTime? Birthday { get; set; }
        public string Avatar { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }

        public virtual SexDAO Sex { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<AdminNotificationDAO> AdminNotifications { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
    }
}
