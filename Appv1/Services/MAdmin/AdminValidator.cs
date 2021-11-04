using Appv1.Common;
using Appv1.Entities;
using Appv1.Enums;
using Appv1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Appv1.Services.MAdmin
{
    public interface IAdminValidator : IServiceScoped
    {
        Task<bool> Create(Admin Admin);
        Task<bool> Update(Admin Admin);
        Task<bool> Login(Admin Admin);
        Task<bool> ChangePassword(Admin Admin);
        Task<bool> ForgotPassword(Admin Admin);
        //Task<bool> VerifyOptCode(Admin Admin);
        Task<bool> Delete(Admin Admin);
        Task<bool> AdminChangePassword(Admin Admin);
        Task<bool> BulkDelete(List<Admin> Admins);
        Task<bool> Import(List<Admin> Admins);
    }

    public class AppUserValidator : IAdminValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            AdminInUsed,
            UsernameExisted,
            UsernameEmpty,
            UsernameOverLength,
            UsernameHasSpecialCharacter,
            DisplayNameEmpty,
            DisplayNameOverLength,
            EmailExisted,
            EmailEmpty,
            EmailOverLength,
            EmailInvalid,
            PhoneEmpty,
            PhoneOverLength,
            AddressOverLength,
            StatusNotExisted,
            SexNotExisted,
            OrganizationNotExisted,
            OrganizationEmpty,
            UsernameNotExisted,
            PasswordNotMatch,
            ProvinceNotExisted,
            ProvinceEmpty,
            EmailNotExisted,
            OtpCodeInvalid,
            OtpExpired,
            PositionIdNotExisted,
            PositionEmpty,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public AppUserValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Admin Admin)
        {
            AdminFilter AdminFilter = new AdminFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Admin.Id },
                Selects = AdminSelect.Id
            };

            int count = await UOW.AdminRepository.Count(AdminFilter);
            if (count == 0)
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Id), ErrorCode.IdNotExisted);
            return Admin.IsValidated;
        }

        public async Task<bool> ValidateUsername(Admin Admin)
        {
            if (string.IsNullOrWhiteSpace(Admin.Username))
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Username), ErrorCode.UsernameEmpty);
            else
            {
                var Code = Admin.Username;
                if (Admin.Username.Contains(" ") || !Code.ChangeToEnglishChar().Equals(Admin.Username))
                {
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Username), ErrorCode.UsernameHasSpecialCharacter);
                }
                else if (Admin.Username.Length > 255)
                {
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Username), ErrorCode.UsernameOverLength);
                }
                AdminFilter AdminFilter = new AdminFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Admin.Id },
                    Username = new StringFilter { Equal = Admin.Username },
                    Selects = AdminSelect.Username
                };

                int count = await UOW.AdminRepository.Count(AdminFilter);
                if (count != 0)
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Username), ErrorCode.UsernameExisted);
            }

            return Admin.IsValidated;
        }

        public async Task<bool> ValidateDisplayName(Admin Admin)
        {
            if (string.IsNullOrWhiteSpace(Admin.DisplayName))
            {
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.DisplayName), ErrorCode.DisplayNameEmpty);
            }
            else if (Admin.DisplayName.Length > 255)
            {
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.DisplayName), ErrorCode.DisplayNameOverLength);
            }
            return Admin.IsValidated;
        }

        public async Task<bool> ValidateEmail(Admin Admin)
        {
            if (string.IsNullOrWhiteSpace(Admin.Email))
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Email), ErrorCode.EmailEmpty);
            else if (!IsValidEmail(Admin.Email))
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Email), ErrorCode.EmailInvalid);
            else
            {
                if (Admin.Email.Length > 255)
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Email), ErrorCode.EmailOverLength);
                AdminFilter AdminFilter = new AdminFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Admin.Id },
                    Email = new StringFilter { Equal = Admin.Email },
                    Selects = AdminSelect.Email
                };

                int count = await UOW.AdminRepository.Count(AdminFilter);
                if (count != 0)
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Email), ErrorCode.EmailExisted);
            }
            return Admin.IsValidated;
        }

        public async Task<bool> ValidatePhone(Admin Admin)
        {
            if (string.IsNullOrEmpty(Admin.Phone))
            {
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Phone), ErrorCode.PhoneEmpty);
            }
            else if (Admin.Phone.Length > 255)
            {
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Phone), ErrorCode.PhoneOverLength);
            }
            return Admin.IsValidated;
        }

        public async Task<bool> ValidateAddress(Admin Admin)
        {
            if (!string.IsNullOrEmpty(Admin.Address) && Admin.Address.Length > 255)
            {
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Address), ErrorCode.AddressOverLength);
            }
            return Admin.IsValidated;
        }

        public async Task<bool> ValidateStatus(Admin Admin)
        {
            if (StatusEnum.ACTIVE.Id != Admin.StatusId && StatusEnum.INACTIVE.Id != Admin.StatusId)
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Status), ErrorCode.StatusNotExisted);
            return true;
        }

        private async Task<bool> ValidateSex(Admin Admin)
        {
            List<long> SexIds = (await UOW.SexRepository.List(new SexFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SexSelect.Id
            })).Select(x => x.Id).ToList();

            if (Admin.SexId.HasValue && !SexIds.Contains(Admin.SexId.Value))
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Sex), ErrorCode.SexNotExisted);
            return Admin.IsValidated;

        }

        public async Task<bool> Create(Admin Admin)
        {
            await ValidateUsername(Admin);
            await ValidateDisplayName(Admin);
            await ValidateEmail(Admin);
            await ValidatePhone(Admin);
            await ValidateAddress(Admin);
            await ValidateStatus(Admin);
            await ValidateSex(Admin);
            return Admin.IsValidated;
        }

        public async Task<bool> Update(Admin Admin)
        {
            if (await ValidateId(Admin))
            {
                await ValidateUsername(Admin);
                await ValidateDisplayName(Admin);
                await ValidateEmail(Admin);
                await ValidatePhone(Admin);
                await ValidateAddress(Admin);
                await ValidateStatus(Admin);
                await ValidateSex(Admin);
            }
            return Admin.IsValidated;
        }

        public async Task<bool> Login(Admin Admin)
        {
            if (string.IsNullOrWhiteSpace(Admin.Username))
            {
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Username), ErrorCode.UsernameNotExisted);
                return false;
            }
            AdminFilter adminFilter = new AdminFilter
            {
                Skip = 0,
                Take = 1,
                Username = new StringFilter { Equal = Admin.Username },
                Selects = AdminSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };
            List<Admin> Admins = await UOW.AdminRepository.List(adminFilter);
            if (Admins.Count == 0)
            {
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Username), ErrorCode.UsernameNotExisted);
            }
            else
            {
                Admin admin = Admins.FirstOrDefault();
                bool verify = VerifyPassword(admin.Password, Admin.Password);
                if (verify == false)
                {
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Password), ErrorCode.PasswordNotMatch);
                }
                else
                {
                    Admin.Id = admin.Id;
                    Admin.RowId = admin.RowId;

                }
            }
            return Admin.IsValidated;
        }

        private bool VerifyPassword(string oldPassword, string newPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(oldPassword);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(newPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }

        public async Task<bool> ChangePassword(Admin Admin)
        {
            List<Admin> Admins = await UOW.AdminRepository.List(new AdminFilter
            {
                Skip = 0,
                Take = 1,
                Id = new IdFilter { Equal = Admin.Id },
                Selects = AdminSelect.ALL,
            });
            if (Admins.Count == 0)
            {
                Admin.AddError(nameof(AppUserValidator), nameof(Admin.Username), ErrorCode.IdNotExisted);
            }
            else
            {
                Admin admin = Admins.FirstOrDefault();
                bool verify = VerifyPassword(admin.Password, Admin.Password);
                if (verify == false)
                {
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Password), ErrorCode.PasswordNotMatch);
                }
            }
            return Admin.IsValidated;
        }

        public async Task<bool> ForgotPassword(Admin Admin)
        {
            if (Admin != null && !string.IsNullOrWhiteSpace(Admin.Email))
            {
                AdminFilter AdminFilter = new AdminFilter
                {
                    Email = new StringFilter { Equal = Admin.Email },
                };

                int count = await UOW.AdminRepository.Count(AdminFilter);
                if (count == 0)
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Email), ErrorCode.EmailNotExisted);
            }

            return Admin.IsValidated;
        }

        public async Task<bool> Delete(Admin Admin)
        {
            await ValidateId(Admin);
            return Admin.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Admin> Admins)
        {
            foreach (var Admin in Admins)
            {
                await Delete(Admin);
            }
            return Admins.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<Admin> Admins)
        {
            var listEmailInDB = (await UOW.AdminRepository.List(new AdminFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AdminSelect.Email,
            })).Select(e => e.Email);
            var listUserNameInDB = (await UOW.AdminRepository.List(new AdminFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AdminSelect.Username,
            })).Select(e => e.Username);
            List<long> SexIds = (await UOW.SexRepository.List(new SexFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SexSelect.Id
            })).Select(x => x.Id).ToList();
            foreach (var Admin in Admins)
            {
                await ValidateDisplayName(Admin);
                await ValidatePhone(Admin);
                await ValidateAddress(Admin);
                await ValidateStatus(Admin);
                if (string.IsNullOrWhiteSpace(Admin.Email))
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Email), ErrorCode.EmailEmpty);
                else if (!IsValidEmail(Admin.Email))
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Email), ErrorCode.EmailInvalid);
                else if (listEmailInDB.Contains(Admin.Email))
                {
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Email), ErrorCode.EmailExisted);
                }

                if (string.IsNullOrWhiteSpace(Admin.Username))
                {
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Email), ErrorCode.UsernameEmpty);
                }
                else if (listUserNameInDB.Contains(Admin.Username))
                {
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Username), ErrorCode.UsernameExisted);
                }

                if (Admin.SexId.HasValue && !SexIds.Contains(Admin.SexId.Value))
                {
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Sex), ErrorCode.SexNotExisted);
                }
            }

            foreach (var Admin in Admins)
            {
                if (Admin.SexId != Enums.SexEnum.MALE.Id && Admin.SexId != Enums.SexEnum.FEMALE.Id && Admin.SexId != Enums.SexEnum.OTHER.Id)
                    Admin.AddError(nameof(AppUserValidator), nameof(Admin.Sex), ErrorCode.SexNotExisted);
            }
            return Admins.All(a => a.IsValidated);
        }

        public async Task<bool> AdminChangePassword(Admin Admin)
        {
            await ValidateId(Admin);
            return Admin.IsValidated;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
