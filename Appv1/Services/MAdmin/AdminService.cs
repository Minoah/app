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

namespace Appv1.Services.MAdmin
{
    public interface IAdminService : IServiceScoped
    {
        Task<int> Count(AdminFilter AdminFilter);
        Task<List<Admin>> List(AdminFilter AdminFilter);
        Task<Admin> Get(long Id);
        Task<Admin> Create(Admin Admin);
        Task<Admin> Update(Admin Admin);
        Task<Admin> Login(Admin Admin);
        Task<Admin> ChangePassword(Admin Admin);
        Task<Admin> ForgotPassword(Admin Admin);
        Task<Admin> RecoveryPassword(Admin Admin);
        Task<Admin> Delete(Admin Admin);
        Task<Admin> AdminChangePassword(Admin Admin);
        Task<List<Admin>> BulkDelete(List<Admin> Admins);
        Task<List<Admin>> Import(List<Admin> Admins);
        AdminFilter ToFilter(AdminFilter AdminFilter);
    }

    public class AppUserService : BaseService, IAdminService
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private IAdminValidator AdminValidator;
        private IConfiguration Configuration;

        public AppUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IAdminValidator AdminValidator,
            IConfiguration Configuration
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.AdminValidator = AdminValidator;
            this.Configuration = Configuration;
        }
        public async Task<int> Count(AdminFilter AdminFilter)
        {
            try
            {
                int result = await UOW.AdminRepository.Count(AdminFilter);
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

        public async Task<List<Admin>> List(AdminFilter AdminFilter)
        {
            try
            {
                List<Admin> Admins = await UOW.AdminRepository.List(AdminFilter);
                return Admins;
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

        public async Task<Admin> Get(long Id)
        {
            Admin Admin = await UOW.AdminRepository.Get(Id);
            if (Admin == null)
                return null;
            
            return Admin;
        }

        public async Task<Admin> Create(Admin Admin)
        {
            if (!await AdminValidator.Create(Admin))
                return Admin;

            try
            {
                Admin.Id = 0;
                var Password = GeneratePassword();
                Admin.Password = HashPassword(Password);

                await UOW.AdminRepository.Create(Admin);

                Admin = await Get(Admin.Id);

                return Admin;
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

        public async Task<Admin> Update(Admin Admin)
        {
            if (!await AdminValidator.Update(Admin))
                return Admin;
            try
            {
                var oldData = await UOW.AdminRepository.Get(Admin.Id);
                Admin.Password = oldData.Password;

                await UOW.AdminRepository.Update(Admin);


                Admin = await Get(Admin.Id);
                return Admin;
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


        public async Task<Admin> Login(Admin Admin)
        {
            if (!await AdminValidator.Login(Admin))
                return Admin;
            Admin = await UOW.AdminRepository.Get(Admin.Id);
            CurrentContext.UserId = Admin.Id;
            Admin.Token = CreateToken(Admin.Id, Admin.Username, Admin.RowId);

            return Admin;
        }
        public async Task<Admin> ChangePassword(Admin Admin)
        {
            if (!await AdminValidator.ChangePassword(Admin))
                return Admin;
            try
            {
                Admin oldData = await UOW.AdminRepository.Get(Admin.Id);
                oldData.Password = HashPassword(Admin.NewPassword);

                await UOW.AdminRepository.Update(oldData);

                var newData = await UOW.AdminRepository.Get(Admin.Id);
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

        public async Task<Admin> ForgotPassword(Admin Admin)
        {
            if (!await AdminValidator.ForgotPassword(Admin))
                return Admin;
            try
            {
                Admin oldData = (await UOW.AdminRepository.List(new AdminFilter
                {
                    Skip = 0,
                    Take = 1,
                    Email = new StringFilter { Equal = Admin.Email },
                    Selects = AdminSelect.ALL
                })).FirstOrDefault();

                CurrentContext.UserId = oldData.Id;
                await UOW.AdminRepository.Update(oldData);


                var newData = await UOW.AdminRepository.Get(oldData.Id);
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

        public async Task<Admin> RecoveryPassword(Admin Admin)
        {
            if (Admin.Id == 0)
                return null;
            try
            {
                Admin oldData = await UOW.AdminRepository.Get(Admin.Id);
                CurrentContext.UserId = Admin.Id;
                oldData.Password = HashPassword(Admin.Password);

                await UOW.AdminRepository.Update(oldData);


                var newData = await UOW.AdminRepository.Get(oldData.Id);
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

        public async Task<Admin> AdminChangePassword(Admin Admin)
        {
            if (!await AdminValidator.AdminChangePassword(Admin))
                return Admin;
            try
            {
                var oldData = await UOW.AdminRepository.Get(Admin.Id);
                oldData.Password = HashPassword(Admin.NewPassword);

                await UOW.AdminRepository.Update(oldData);

                return Admin;
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

        public async Task<Admin> Delete(Admin Admin)
        {
            if (!await AdminValidator.Delete(Admin))
                return Admin;

            try
            {
                await UOW.AdminRepository.Delete(Admin);

                Admin = await UOW.AdminRepository.Get(Admin.Id);
                Admin = await Get(Admin.Id);
                return Admin;
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

        public async Task<List<Admin>> BulkDelete(List<Admin> Admins)
        {
            if (!await AdminValidator.BulkDelete(Admins))
                return Admins;

            try
            {

                await UOW.AdminRepository.BulkDelete(Admins);

                var Ids = Admins.Select(x => x.Id).ToList();
                Admins = await UOW.AdminRepository.List(Ids);
                return Admins;
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
        public async Task<List<Admin>> Import(List<Admin> Admins)
        {
            if (!await AdminValidator.Import(Admins))
                return Admins;
            try
            {
                var listAdminInDB = (await UOW.AdminRepository.List(new AdminFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AdminSelect.ALL
                }));
                foreach (var Admin in Admins)
                {
                    var admin = listAdminInDB.Where(a => a.Username == Admin.Username).FirstOrDefault();
                    if (admin != null)
                    {
                        Admin.Id = admin.Id;
                        Admin.RowId = admin.RowId;
                        Admin.CreatedAt = admin.CreatedAt;
                        Admin.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        var Password = GeneratePassword();
                        Admin.Password = HashPassword(Password);
                        Admin.RowId = Guid.NewGuid();
                        Admin.CreatedAt = DateTime.Now;
                        Admin.UpdatedAt = DateTime.Now;

                    }
                }
                await UOW.AdminRepository.BulkMerge(Admins);

                var Ids = Admins.Select(x => x.Id).ToList();
                Admins = await UOW.AdminRepository.List(Ids);

                return Admins;
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

        public AdminFilter ToFilter(AdminFilter filter)
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
