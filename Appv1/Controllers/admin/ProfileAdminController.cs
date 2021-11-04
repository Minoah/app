using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Appv1.Common;
using Appv1.Entities;
using Appv1.Enums;
using Appv1.Services.MAdmin;
using Appv1.Services.MSex;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Appv1.Controllers.admin
{
    public class ProfileAdminRoot
    {
        public const string Login = "controller/app/admin/account/login";
        public const string Logged = "controller/app/admin/account/logged";
        public const string GetForWeb = "controller/app/admin/profile-web/get";
        public const string Get = "controller/app/admin/profile/get";
        public const string GetDraft = "controller/app/admin/profile/get-draft";
        public const string Update = "controller/app/admin/profile/update";
        public const string SaveImage = "controller/app/admin/profile/save-image";
        public const string ChangePassword = "controller/app/admin/profile/change-password";
        public const string ForgotPassword = "controller/app/admin/profile/forgot-password";
        public const string VerifyOtpCode = "controller/app/admin/profile/verify-otp-code";
        public const string RecoveryPassword = "controller/app/admin/profile/recovery-password";
        public const string SingleListSex = "controller/app/admin/profile/single-list-sex";
        public const string SingleListProvince = "controller/app/admin/profile/single-list-province";
    }
    [Authorize]
    public class ProfileAdminControllers : ControllerBase
    {
        private IAdminService AdminService;
        private ISexService SexService;
        private ICurrentContext CurrentContext;
        public ProfileAdminControllers(
            IAdminService AdminService,
            ISexService SexService,
            ICurrentContext CurrentContext
            )
        {
            this.AdminService = AdminService;
            this.SexService = SexService;
            this.CurrentContext = CurrentContext;
        }

        [AllowAnonymous]
        [Route(ProfileAdminRoot.Login), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> Login([FromBody] Admin_LoginDTO Admin_LoginDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            Admin Admin = new Admin
            {
                Username = Admin_LoginDTO.Username,
                Password = Admin_LoginDTO.Password,
            };
            Admin = await AdminService.Login(Admin);
            Admin_AdminDTO Admin_AdminDTO = new Admin_AdminDTO(Admin);

            if (Admin.IsValidated)
            {
                Response.Cookies.Append("Token", Admin.Token);
                Admin_AdminDTO.Token = Admin.Token;
                return Admin_AdminDTO;
            }
            else
                return BadRequest(Admin_AdminDTO);
        }

        [Route(ProfileAdminRoot.Logged), HttpPost]
        public bool Logged()
        {
            return true;
        }
        [Route(ProfileAdminRoot.ChangePassword), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> ChangePassword([FromBody] Admin_ProfileChangePasswordDTO Admin_ProfileChangePasswordDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            this.CurrentContext.UserId = ExtractUserId();
            Admin Admin = new Admin
            {
                Id = CurrentContext.UserId,
                Password = Admin_ProfileChangePasswordDTO.OldPassword,
                NewPassword = Admin_ProfileChangePasswordDTO.NewPassword,
            };
            Admin = await AdminService.ChangePassword(Admin);
            Admin_AdminDTO Admin_AdminDTO = new Admin_AdminDTO(Admin);
            if (Admin.IsValidated)
                return Admin_AdminDTO;
            else
                return BadRequest(Admin_AdminDTO);
        }

        #region Forgot Password
        [AllowAnonymous]
        [Route(ProfileAdminRoot.ForgotPassword), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> ForgotPassword([FromBody] Admin_ForgotPassword Admin_ForgotPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Admin Admin = new Admin
            {
                Email = Admin_ForgotPassword.Email,
            };

            Admin = await AdminService.ForgotPassword(Admin);
            Admin_AdminDTO Admin_AdminDTO = new Admin_AdminDTO(Admin);
            if (Admin.IsValidated)
            {
                return Admin_AdminDTO;
            }
            else
                return BadRequest(Admin_AdminDTO);
        }

        [Route(ProfileAdminRoot.RecoveryPassword), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> RecoveryPassword([FromBody] Admin_RecoveryPassword Admin_RecoveryPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            Admin Admin = new Admin
            {
                Id = UserId,
                Password = Admin_RecoveryPassword.Password,
            };
            Admin = await AdminService.RecoveryPassword(Admin);
            if (Admin == null)
                return Unauthorized();
            Admin_AdminDTO Admin_AdminDTO = new Admin_AdminDTO(Admin);
            return Admin_AdminDTO;
        }
        #endregion

        [Route(ProfileAdminRoot.GetForWeb), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> GetForWeb()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            Admin Admin = await AdminService.Get(UserId);
            Admin_AdminDTO Admin_AdminDTO = new Admin_AdminDTO(Admin);
            return Admin_AdminDTO;
        }

        [Route(ProfileAdminRoot.Get), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> GetMe()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            Admin Admin = await AdminService.Get(UserId);
            Admin_AdminDTO Admin_AdminDTO = new Admin_AdminDTO(Admin);
            
            return Admin_AdminDTO;
        }

        [Route(ProfileAdminRoot.GetDraft), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> GetDraft()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            Admin Admin = await AdminService.Get(UserId);
            return new Admin_AdminDTO(Admin);
        }

        [Route(ProfileAdminRoot.Update), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> UpdateMe([FromBody] Admin_AdminDTO Admin_AdminDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            this.CurrentContext.UserId = ExtractUserId();
            Admin OldData = await AdminService.Get(this.CurrentContext.UserId);
            Admin Admin = ConvertDTOToEntity(Admin_AdminDTO);
            Admin.Id = CurrentContext.UserId;
            Admin = await AdminService.Update(Admin);
            Admin_AdminDTO = new Admin_AdminDTO(Admin);
            if (Admin.IsValidated)
                return Admin_AdminDTO;
            else
                return BadRequest(Admin_AdminDTO);
        }

        [Route(ProfileAdminRoot.SingleListSex), HttpPost]
        public async Task<List<Admin_SexDTO>> SingleListSex([FromBody] Admin_SexFilterDTO Admin_SexFilterDTO)
        {
            SexFilter SexFilter = new SexFilter();
            SexFilter.Skip = 0;
            SexFilter.Take = 20;
            SexFilter.OrderBy = SexOrder.Id;
            SexFilter.OrderType = OrderType.ASC;
            SexFilter.Selects = SexSelect.ALL;
            SexFilter.Id = Admin_SexFilterDTO.Id;
            SexFilter.Code = Admin_SexFilterDTO.Code;
            SexFilter.Name = Admin_SexFilterDTO.Name;
            List<Sex> Sexes = await SexService.List(SexFilter);
            List<Admin_SexDTO> Admin_SexDTOs = Sexes
                .Select(x => new Admin_SexDTO(x)).ToList();
            return Admin_SexDTOs;
        }

        private long ExtractUserId()
        {
            return long.TryParse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
        }
        private Admin ConvertDTOToEntity(Admin_AdminDTO Admin_AdminDTO)
        {
            Admin Admin = new Admin();
            Admin.Id = Admin_AdminDTO.Id;
            Admin.Username = Admin_AdminDTO.Username;
            Admin.Password = Admin_AdminDTO.Password;
            Admin.DisplayName = Admin_AdminDTO.DisplayName;
            Admin.Address = Admin_AdminDTO.Address;
            Admin.Avatar = Admin_AdminDTO.Avatar;
            Admin.Birthday = Admin_AdminDTO.Birthday;
            Admin.Email = Admin_AdminDTO.Email;
            Admin.Phone = Admin_AdminDTO.Phone;
            Admin.SexId = Admin_AdminDTO.SexId;
            Admin.StatusId = Admin_AdminDTO.StatusId;
            Admin.Sex = Admin_AdminDTO.Sex == null ? null : new Sex
            {
                Id = Admin_AdminDTO.Sex.Id,
                Code = Admin_AdminDTO.Sex.Code,
                Name = Admin_AdminDTO.Sex.Name,
            };
            Admin.Status = Admin_AdminDTO.Status == null ? null : new Status
            {
                Id = Admin_AdminDTO.Status.Id,
                Code = Admin_AdminDTO.Status.Code,
                Name = Admin_AdminDTO.Status.Name,
            };
            return Admin;
        }
    }
}
