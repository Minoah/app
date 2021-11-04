using Appv1.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Appv1.Common;

namespace Appv1.Controllers.admin
{
    public class Admin_AdminDTO : DataDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Phone { get; set; }
        public long? SexId { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public Admin_SexDTO Sex { get; set; }
        public Admin_StatusDTO Status { get; set; }
        public long STT;
        public Admin_AdminDTO() { }
        public Admin_AdminDTO(Admin Admin)
        {
            this.Id = Admin.Id;
            this.Username = Admin.Username;
            this.Password = Admin.Password;
            this.DisplayName = Admin.DisplayName;
            this.Address = Admin.Address;
            this.Avatar = Admin.Avatar;
            this.Birthday = Admin.Birthday;
            this.Email = Admin.Email;
            this.Phone = Admin.Phone;
            this.SexId = Admin.SexId;
            this.StatusId = Admin.StatusId;
            this.RowId = Admin.RowId;
            this.Sex = Admin.Sex == null ? null : new Admin_SexDTO(Admin.Sex);
            this.Status = Admin.Status == null ? null : new Admin_StatusDTO(Admin.Status);
            this.Errors = Admin.Errors;
        }
    }

    public class Admin_AdminFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Address { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Phone { get; set; }
        public DateFilter Birthday { get; set; }
        public IdFilter SexId { get; set; }
        public IdFilter StatusId { get; set; }
        public AdminOrder OrderBy { get; set; }
        public DateFilter CreatedAt { get; set; }
    }
}
