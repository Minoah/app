using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Appv1.Common;
using Appv1.Entities;
using Appv1.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
//using Appv1.Handlers.Configuration;

namespace Appv1.Services.MAppUser
{
    public interface IAppUserService : IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<AppUser> Create(AppUser AppUser);
        Task<AppUser> Update(AppUser AppUser);
        Task<AppUser> Login(AppUser AppUser);
        Task<AppUser> ChangePassword(AppUser AppUser);
        Task<AppUser> ForgotPassword(AppUser AppUser);
        Task<AppUser> RecoveryPassword(AppUser AppUser);
        Task<AppUser> Delete(AppUser AppUser);
        Task<AppUser> AppUserChangePassword(AppUser AppUser);
        Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers);
        Task<List<AppUser>> Import(List<AppUser> AppUsers);
        AppUserFilter ToFilter(AppUserFilter AppUserFilter);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private IAppUserValidator AppUserValidator;
        private IConfiguration Configuration;

        public AppUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IAppUserValidator AppUserValidator,
            IConfiguration Configuration
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.AppUserValidator = AppUserValidator;
            this.Configuration = Configuration;
        }
        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            if (AppUser == null)
                return null;
            
            return AppUser;
        }

        public async Task<AppUser> Create(AppUser AppUser)
        {
            if (!await AppUserValidator.Create(AppUser))
                return AppUser;

            try
            {
                AppUser.Id = 0;
                var Password = GeneratePassword();
                AppUser.Password = HashPassword(Password);

                await UOW.AppUserRepository.Create(AppUser);

                AppUser = await Get(AppUser.Id);

                return AppUser;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> Update(AppUser AppUser)
        {
            if (!await AppUserValidator.Update(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                AppUser.Password = oldData.Password;

                await UOW.AppUserRepository.Update(AppUser);


                AppUser = await Get(AppUser.Id);
                return AppUser;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }


        public async Task<AppUser> Login(AppUser AppUser)
        {
            if (!await AppUserValidator.Login(AppUser))
                return AppUser;
            AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
            CurrentContext.UserId = AppUser.Id;
            AppUser.Token = CreateToken(AppUser.Id, AppUser.Username, AppUser.RowId);

            return AppUser;
        }
        public async Task<AppUser> ChangePassword(AppUser AppUser)
        {
            if (!await AppUserValidator.ChangePassword(AppUser))
                return AppUser;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                oldData.Password = HashPassword(AppUser.NewPassword);

                await UOW.AppUserRepository.Update(oldData);

                var newData = await UOW.AppUserRepository.Get(AppUser.Id);
                return newData;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> ForgotPassword(AppUser AppUser)
        {
            if (!await AppUserValidator.ForgotPassword(AppUser))
                return AppUser;
            try
            {
                AppUser oldData = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = 1,
                    Email = new StringFilter { Equal = AppUser.Email },
                    Selects = AppUserSelect.ALL
                })).FirstOrDefault();

                CurrentContext.UserId = oldData.Id;
                await UOW.AppUserRepository.Update(oldData);


                var newData = await UOW.AppUserRepository.Get(oldData.Id);
                return newData;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> RecoveryPassword(AppUser AppUser)
        {
            if (AppUser.Id == 0)
                return null;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                CurrentContext.UserId = AppUser.Id;
                oldData.Password = HashPassword(AppUser.Password);

                await UOW.AppUserRepository.Update(oldData);


                var newData = await UOW.AppUserRepository.Get(oldData.Id);
                return newData;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> AppUserChangePassword(AppUser AppUser)
        {
            if (!await AppUserValidator.AppUserChangePassword(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                oldData.Password = HashPassword(AppUser.NewPassword);

                await UOW.AppUserRepository.Update(oldData);

                return AppUser;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> Delete(AppUser AppUser)
        {
            if (!await AppUserValidator.Delete(AppUser))
                return AppUser;

            try
            {
                await UOW.AppUserRepository.Delete(AppUser);

                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                AppUser = await Get(AppUser.Id);
                return AppUser;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.BulkDelete(AppUsers))
                return AppUsers;

            try
            {

                await UOW.AppUserRepository.BulkDelete(AppUsers);

                var Ids = AppUsers.Select(x => x.Id).ToList();
                AppUsers = await UOW.AppUserRepository.List(Ids);
                return AppUsers;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<List<AppUser>> Import(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.Import(AppUsers))
                return AppUsers;
            try
            {
                var listAppUserInDB = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.ALL
                }));
                foreach (var AppUser in AppUsers)
                {
                    var appUser = listAppUserInDB.Where(a => a.Username == AppUser.Username).FirstOrDefault();
                    if (AppUser != null)
                    {
                        AppUser.Id = appUser.Id;
                        AppUser.RowId = appUser.RowId;
                        AppUser.CreatedAt = appUser.CreatedAt;
                        AppUser.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        var Password = GeneratePassword();
                        AppUser.Password = HashPassword(Password);
                        AppUser.RowId = Guid.NewGuid();
                        AppUser.CreatedAt = DateTime.Now;
                        AppUser.UpdatedAt = DateTime.Now;

                    }
                }
                await UOW.AppUserRepository.BulkMerge(AppUsers);

                var Ids = AppUsers.Select(x => x.Id).ToList();
                AppUsers = await UOW.AppUserRepository.List(Ids);

                return AppUsers;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public AppUserFilter ToFilter(AppUserFilter filter)
        {

            return filter;
        }

        private string CreateToken(long id, string userName, Guid rowId, double? expiredTime = null)
        {
            if (expiredTime == null)
                expiredTime = double.TryParse(Configuration["Config:ExpiredTime"], out double time) ? time : 0;

            string PrivateRSAKeyBase64 = Configuration["Config:PrivateRSAKey"];
            byte[] PrivateRSAKeyBytes = Convert.FromBase64String(PrivateRSAKeyBase64);
            string PrivateRSAKey = Encoding.Default.GetString(PrivateRSAKeyBytes);

            RSAParameters rsaParams;
            using (var tr = new StringReader(PrivateRSAKey))
            {
                var pemReader = new PemReader(tr);
                var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
                if (keyPair == null)
                {
                    throw new Exception("Could not read RSA private key");
                }
                var privateRsaParams = keyPair.Private as RsaPrivateCrtKeyParameters;
                rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
            }

            RSA rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);

            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
            var jwt = new JwtSecurityToken(
                claims: new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.PrimarySid, rowId.ToString()),
                },
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(expiredTime.Value),
                signingCredentials: signingCredentials
            );

            string Token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Token;
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        private string GeneratePassword()
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            const string special = "!@#$%^&*_-=+";

            Random _rand = new Random();
            var bytes = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(bytes);

            var res = new StringBuilder();
            foreach (byte b in bytes)
            {
                switch (_rand.Next(4))
                {
                    case 0:
                        res.Append(lower[b % lower.Count()]);
                        break;
                    case 1:
                        res.Append(upper[b % upper.Count()]);
                        break;
                    case 2:
                        res.Append(number[b % number.Count()]);
                        break;
                    case 3:
                        res.Append(special[b % special.Count()]);
                        break;
                }
            }
            return res.ToString();
        }

        private string GenerateOTPCode()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }

    }

}
