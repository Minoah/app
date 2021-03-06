using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class AppUserNotificationDAO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long OrganizationId { get; set; }
        public long NotificationStatusId { get; set; }
        public long AppUserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual NotificationStatusDAO NotificationStatus { get; set; }
    }
}
