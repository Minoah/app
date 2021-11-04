using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class SexDAO
    {
        public SexDAO()
        {
            Admins = new HashSet<AdminDAO>();
            AppUsers = new HashSet<AppUserDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AdminDAO> Admins { get; set; }
        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
    }
}
