using Appv1.Common;
using Appv1.Entities;
using Appv1.Enums;
using Appv1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Appv1.Services.MAppUser
{
    public interface IAppUserValidator : IServiceScoped
    {
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Login(AppUser AppUser);
        Task<bool> ChangePassword(AppUser AppUser);
        Task<bool> ForgotPassword(AppUser AppUser);
        //Task<bool> VerifyOptCode(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> AppUserChangePassword(AppUser AppUser);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
        Task<bool> Import(List<AppUser> AppUsers);
    }

    public class AppUserValidator : IAppUserValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            AppUserInUsed,
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

        public async Task<bool> ValidateId(AppUser AppUser)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = AppUser.Id },
                Selects = AppUserSelect.Id
            };

            int count = await UOW.AppUserRepository.Count(AppUserFilter);
            if (count == 0)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Id), ErrorCode.IdNotExisted);
            return AppUser.IsValidated;
        }

        public async Task<bool> ValidateUsername(AppUser AppUser)
        {
            if (string.IsNullOrWhiteSpace(AppUser.Username))
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameEmpty);
            else
            {
                var Code = AppUser.Username;
                if (AppUser.Username.Contains(" ") || !Code.ChangeToEnglishChar().Equals(AppUser.Username))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameHasSpecialCharacter);
                }
                else if (AppUser.Username.Length > 255)
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameOverLength);
                }
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = AppUser.Id },
                    Username = new StringFilter { Equal = AppUser.Username },
                    Selects = AppUserSelect.Username
                };

                int count = await UOW.AppUserRepository.Count(AppUserFilter);
                if (count != 0)
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameExisted);
            }

            return AppUser.IsValidated;
        }

        public async Task<bool> ValidateDisplayName(AppUser AppUser)
        {
            if (string.IsNullOrWhiteSpace(AppUser.DisplayName))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), ErrorCode.DisplayNameEmpty);
            }
            else if (AppUser.DisplayName.Length > 255)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), ErrorCode.DisplayNameOverLength);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> ValidateEmail(AppUser AppUser)
        {
            if (string.IsNullOrWhiteSpace(AppUser.Email))
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailEmpty);
            else if (!IsValidEmail(AppUser.Email))
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailInvalid);
            else
            {
                if (AppUser.Email.Length > 255)
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailOverLength);
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = AppUser.Id },
                    Email = new StringFilter { Equal = AppUser.Email },
                    Selects = AppUserSelect.Email
                };

                int count = await UOW.AppUserRepository.Count(AppUserFilter);
                if (count != 0)
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailExisted);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> ValidatePhone(AppUser AppUser)
        {
            if (string.IsNullOrEmpty(AppUser.Phone))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Phone), ErrorCode.PhoneEmpty);
            }
            else if (AppUser.Phone.Length > 255)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Phone), ErrorCode.PhoneOverLength);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> ValidateAddress(AppUser AppUser)
        {
            if (!string.IsNullOrEmpty(AppUser.Address) && AppUser.Address.Length > 255)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Address), ErrorCode.AddressOverLength);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> ValidateStatus(AppUser AppUser)
        {
            if (StatusEnum.ACTIVE.Id != AppUser.StatusId && StatusEnum.INACTIVE.Id != AppUser.StatusId)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Status), ErrorCode.StatusNotExisted);
            return true;
        }

        private async Task<bool> ValidateSex(AppUser AppUser)
        {
            List<long> SexIds = (await UOW.SexRepository.List(new SexFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SexSelect.Id
            })).Select(x => x.Id).ToList();

            if (AppUser.SexId.HasValue && !SexIds.Contains(AppUser.SexId.Value))
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Sex), ErrorCode.SexNotExisted);
            return AppUser.IsValidated;

        }

        public async Task<bool> Create(AppUser AppUser)
        {
            await ValidateUsername(AppUser);
            await ValidateDisplayName(AppUser);
            await ValidateEmail(AppUser);
            await ValidatePhone(AppUser);
            await ValidateAddress(AppUser);
            await ValidateStatus(AppUser);
            await ValidateSex(AppUser);
            return AppUser.IsValidated;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            if (await ValidateId(AppUser))
            {
                await ValidateUsername(AppUser);
                await ValidateDisplayName(AppUser);
                await ValidateEmail(AppUser);
                await ValidatePhone(AppUser);
                await ValidateAddress(AppUser);
                await ValidateStatus(AppUser);
                await ValidateSex(AppUser);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> Login(AppUser AppUser)
        {
            if (string.IsNullOrWhiteSpace(AppUser.Username))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameNotExisted);
                return false;
            }
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                Username = new StringFilter { Equal = AppUser.Username },
                Selects = AppUserSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };
            List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
            if (AppUsers.Count == 0)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameNotExisted);
            }
            else
            {
                AppUser appUser = AppUsers.FirstOrDefault();
                bool verify = VerifyPassword(appUser.Password, AppUser.Password);
                if (verify == false)
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Password), ErrorCode.PasswordNotMatch);
                }
                else
                {
                    AppUser.Id = AppUser.Id;
                    AppUser.RowId = AppUser.RowId;

                }
            }
            return AppUser.IsValidated;
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

        public async Task<bool> ChangePassword(AppUser AppUser)
        {
            List<AppUser> AppUsers = await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                Id = new IdFilter { Equal = AppUser.Id },
                Selects = AppUserSelect.ALL,
            });
            if (AppUsers.Count == 0)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.IdNotExisted);
            }
            else
            {
                AppUser appUser = AppUsers.FirstOrDefault();
                bool verify = VerifyPassword(AppUser.Password, AppUser.Password);
                if (verify == false)
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Password), ErrorCode.PasswordNotMatch);
                }
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> ForgotPassword(AppUser AppUser)
        {
            if (AppUser != null && !string.IsNullOrWhiteSpace(AppUser.Email))
            {
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Email = new StringFilter { Equal = AppUser.Email },
                };

                int count = await UOW.AppUserRepository.Count(AppUserFilter);
                if (count == 0)
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailNotExisted);
            }

            return AppUser.IsValidated;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            await ValidateId(AppUser);
            return AppUser.IsValidated;
        }

        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            foreach (var AppUser in AppUsers)
            {
                await Delete(AppUser);
            }
            return AppUsers.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<AppUser> AppUsers)
        {
            var listEmailInDB = (await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Email,
            })).Select(e => e.Email);
            var listUserNameInDB = (await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Username,
            })).Select(e => e.Username);
            List<long> SexIds = (await UOW.SexRepository.List(new SexFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SexSelect.Id
            })).Select(x => x.Id).ToList();
            foreach (var AppUser in AppUsers)
            {
                await ValidateDisplayName(AppUser);
                await ValidatePhone(AppUser);
                await ValidateAddress(AppUser);
                await ValidateStatus(AppUser);
                if (string.IsNullOrWhiteSpace(AppUser.Email))
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailEmpty);
                else if (!IsValidEmail(AppUser.Email))
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailInvalid);
                else if (listEmailInDB.Contains(AppUser.Email))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailExisted);
                }

                if (string.IsNullOrWhiteSpace(AppUser.Username))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.UsernameEmpty);
                }
                else if (listUserNameInDB.Contains(AppUser.Username))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameExisted);
                }

                if (AppUser.SexId.HasValue && !SexIds.Contains(AppUser.SexId.Value))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Sex), ErrorCode.SexNotExisted);
                }
            }

            foreach (var AppUser in AppUsers)
            {
                if (AppUser.SexId != Enums.SexEnum.MALE.Id && AppUser.SexId != Enums.SexEnum.FEMALE.Id && AppUser.SexId != Enums.SexEnum.OTHER.Id)
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Sex), ErrorCode.SexNotExisted);
            }
            return AppUsers.All(a => a.IsValidated);
        }

        public async Task<bool> AppUserChangePassword(AppUser AppUser)
        {
            await ValidateId(AppUser);
            return AppUser.IsValidated;
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
