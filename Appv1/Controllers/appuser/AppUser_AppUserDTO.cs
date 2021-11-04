using Appv1.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Appv1.Common;

namespace Appv1.Controllers.appuser
{
    public class AppUser_AppUserDTO : DataDTO
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
        public AppUser_SexDTO Sex { get; set; }
        public AppUser_StatusDTO Status { get; set; }
        public long STT;
        public AppUser_AppUserDTO() { }
        public AppUser_AppUserDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.Password = AppUser.Password;
            this.DisplayName = AppUser.DisplayName;
            this.Address = AppUser.Address;
            this.Avatar = AppUser.Avatar;
            this.Birthday = AppUser.Birthday;
            this.Email = AppUser.Email;
            this.Phone = AppUser.Phone;
            this.SexId = AppUser.SexId;
            this.StatusId = AppUser.StatusId;
            this.RowId = AppUser.RowId;
            this.Sex = AppUser.Sex == null ? null : new AppUser_SexDTO(AppUser.Sex);
            this.Status = AppUser.Status == null ? null : new AppUser_StatusDTO(AppUser.Status);
            this.Errors = AppUser.Errors;
        }
    }

    public class AppUser_AppUserFilterDTO : FilterDTO
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
        public AppUserOrder OrderBy { get; set; }
        public DateFilter CreatedAt { get; set; }
    }
}
