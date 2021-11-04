using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class NotificationStatusDAO
    {
        public NotificationStatusDAO()
        {
            AdminNotifications = new HashSet<AdminNotificationDAO>();
            AppUserNotifications = new HashSet<AppUserNotificationDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AdminNotificationDAO> AdminNotifications { get; set; }
        public virtual ICollection<AppUserNotificationDAO> AppUserNotifications { get; set; }
    }
}
