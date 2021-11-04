using Microsoft.EntityFrameworkCore;
using Appv1.Entities;
//using Appv1.Helpers;
using Appv1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appv1.Common;

namespace Appv1.Repositories
{
    public interface IAppUserRepository
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<int> CountAll(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(List<long> Ids);
        Task<AppUser> Get(long Id);
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkMerge(List<AppUser> AppUsers);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
    }
    public class AppUserRepository : IAppUserRepository
    {
        private DataContext DataContext;
        public AppUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<AppUserDAO> DynamicFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.DeletedAt == null);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Username.ToLower(), filter.Username?.ToLower());
            query = query.Where(q => q.Password, filter.Password);
            query = query.Where(q => q.DisplayName, filter.DisplayName);
            query = query.Where(q => q.Address, filter.Address);
            query = query.Where(q => q.Email, filter.Email);
            query = query.Where(q => q.Phone, filter.Phone);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.SexId, filter.SexId);
            query = query.Where(q => q.Birthday, filter.Birthday);
            return query;
        }
        private IQueryable<AppUserDAO> DynamicOrder(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderBy(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case AppUserOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case AppUserOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case AppUserOrder.Sex:
                            query = query.OrderBy(q => q.Sex);
                            break;
                        case AppUserOrder.Birthday:
                            query = query.OrderBy(q => q.Birthday);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderByDescending(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case AppUserOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case AppUserOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case AppUserOrder.Sex:
                            query = query.OrderByDescending(q => q.Sex);
                            break;
                        case AppUserOrder.Birthday:
                            query = query.OrderByDescending(q => q.Birthday);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<AppUser>> DynamicSelect(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            List<AppUser> AppUsers = await query.Select(q => new AppUser()
            {
                Id = filter.Selects.Contains(AppUserSelect.Id) ? q.Id : default(long),
                Username = filter.Selects.Contains(AppUserSelect.Username) ? q.Username : default(string),
                Password = filter.Selects.Contains(AppUserSelect.Password) ? q.Password : default(string),
                DisplayName = filter.Selects.Contains(AppUserSelect.DisplayName) ? q.DisplayName : default(string),
                Address = filter.Selects.Contains(AppUserSelect.Address) ? q.Address : default(string),
                Email = filter.Selects.Contains(AppUserSelect.Email) ? q.Email : default(string),
                Phone = filter.Selects.Contains(AppUserSelect.Phone) ? q.Phone : default(string),
                StatusId = filter.Selects.Contains(AppUserSelect.Status) ? q.StatusId : default(long),
                SexId = filter.Selects.Contains(AppUserSelect.Sex) ? q.SexId : default(long),
                Birthday = filter.Selects.Contains(AppUserSelect.Birthday) ? q.Birthday : null,
                Avatar = filter.Selects.Contains(AppUserSelect.Avatar) ? q.Avatar : default(string),
                Status = filter.Selects.Contains(AppUserSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Sex = filter.Selects.Contains(AppUserSelect.Sex) && q.Sex != null ? new Sex
                {
                    Id = q.Sex.Id,
                    Code = q.Sex.Code,
                    Name = q.Sex.Name,
                } : null,
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();

            //var Ids = AppUsers.Select(x => x.Id).ToList();
            //var AppUserEstateMappings = await DataContext.AppUserEstateMapping.Where(x => Ids.Contains(x.AppUserId)).Select(x => new AppUserEstateMapping
            //{
            //    AppUserId = x.AppUserId,
            //    EstateId = x.EstateId,
            //    Estate = x.Estate == null ? null : new Estate
            //    {
            //        Id = x.Estate.Id,
            //        Name = x.Estate.Name,
            //        AvatarId = x.Estate.AvatarId,
            //        Address = x.Estate.Address,
            //        ProvinceId = x.Estate.ProvinceId,
            //        DistrictId = x.Estate.DistrictId,
            //        WardId = x.Estate.WardId,
            //        Avatar = x.Estate.Avatar == null ? null : new Image
            //        {
            //            Id = x.Estate.Avatar.Id,
            //            Name = x.Estate.Avatar.Name,
            //            Url = x.Estate.Avatar.Url,
            //            ThumbnailUrl = x.Estate.Avatar.ThumbnailUrl,
            //            EstateId = x.Estate.Avatar.EstateId,
            //        },
            //        District = x.Estate.District == null ? null : new District
            //        {
            //            Id = x.Estate.District.Id,
            //            Code = x.Estate.District.Code,
            //            Name = x.Estate.District.Name,
            //            Priority = x.Estate.District.Priority,
            //            ProvinceId = x.Estate.District.ProvinceId,
            //            StatusId = x.Estate.District.StatusId,
            //            Used = x.Estate.District.Used,
            //        },
            //        Province = x.Estate.Province == null ? null : new Province
            //        {
            //            Id = x.Estate.Province.Id,
            //            Code = x.Estate.Province.Code,
            //            Name = x.Estate.Province.Name,
            //            Priority = x.Estate.Province.Priority,
            //            StatusId = x.Estate.Province.StatusId,
            //            Used = x.Estate.Province.Used,
            //        },
            //        Ward = x.Estate.Ward == null ? null : new Ward
            //        {
            //            Id = x.Estate.Ward.Id,
            //            Code = x.Estate.Ward.Code,
            //            Name = x.Estate.Ward.Name,
            //            Priority = x.Estate.Ward.Priority,
            //            DistrictId = x.Estate.Ward.DistrictId,
            //            StatusId = x.Estate.Ward.StatusId,
            //            Used = x.Estate.Ward.Used,
            //        },
            //    }
            //}).ToListAsync();

            //foreach (var AppUser in AppUsers)
            //{
            //    AppUser.AppUserEstateMappings = AppUserEstateMappings.Where(x => x.AppUserId == AppUser.Id).ToList();
            //}
            return AppUsers;
        }

        public async Task<int> Count(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUsers = DataContext.AppUsers.AsNoTracking();
            AppUsers = DynamicFilter(AppUsers, filter);
            return await AppUsers.CountAsync();
        }

        public async Task<int> CountAll(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUsers = DataContext.AppUsers.AsNoTracking();
            AppUsers = DynamicFilter(AppUsers, filter);
            return await AppUsers.CountAsync();
        }

        public async Task<List<AppUser>> List(AppUserFilter filter)
        {
            if (filter == null) return new List<AppUser>();
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUsers.AsNoTracking();
            AppUserDAOs = DynamicFilter(AppUserDAOs, filter);
            AppUserDAOs = DynamicOrder(AppUserDAOs, filter);
            List<AppUser> AppUsers = await DynamicSelect(AppUserDAOs, filter);
            return AppUsers;
        }

        public async Task<List<AppUser>> List(List<long> Ids)
        {
            List<AppUser> AppUsers = await DataContext.AppUsers.AsNoTracking()
                .Where(x => Ids.Contains(x.Id)).Select(x => new AppUser()
                {
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    Address = x.Address,
                    Avatar = x.Avatar,
                    Birthday = x.Birthday,
                    Email = x.Email,
                    Phone = x.Phone,
                    StatusId = x.StatusId,
                    SexId = x.SexId,
                    RowId = x.RowId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    Sex = x.Sex == null ? null : new Sex
                    {
                        Id = x.Sex.Id,
                        Code = x.Sex.Code,
                        Name = x.Sex.Name,
                    }
                }).ToListAsync();


            return AppUsers;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await DataContext.AppUsers.AsNoTracking()
                .Where(x => x.Id == Id)
                .Select(x => new AppUser()
                {
                    Id = x.Id,
                    Username = x.Username,
                    Password = x.Password,
                    DisplayName = x.DisplayName,
                    Address = x.Address,
                    Avatar = x.Avatar,
                    Birthday = x.Birthday,
                    Email = x.Email,
                    Phone = x.Phone,
                    StatusId = x.StatusId,
                    SexId = x.SexId,
                    RowId = x.RowId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    Sex = x.Sex == null ? null : new Sex
                    {
                        Id = x.Sex.Id,
                        Code = x.Sex.Code,
                        Name = x.Sex.Name,
                    }
                }).FirstOrDefaultAsync();

            if (AppUser == null)
                return null;
            //AppUser.AppUserEstateMappings = await DataContext.AppUserEstateMapping
            //    .Where(x => x.AppUserId == AppUser.Id)
            //    .OrderBy(x => x.EstateId)
            //    .Select(x => new AppUserEstateMapping
            //    {
            //        AppUserId = x.AppUserId,
            //        EstateId = x.EstateId,
            //        Estate = new Estate
            //        {
            //            Id = x.Estate.Id,
            //            Name = x.Estate.Name,
            //            AvatarId = x.Estate.AvatarId,
            //            Address = x.Estate.Address,
            //            ProvinceId = x.Estate.ProvinceId,
            //            DistrictId = x.Estate.DistrictId,
            //            WardId = x.Estate.WardId,
            //            District = x.Estate.District == null ? null : new District
            //            {
            //                Id = x.Estate.District.Id,
            //                Code = x.Estate.District.Code,
            //                Name = x.Estate.District.Name,
            //                Priority = x.Estate.District.Priority,
            //                ProvinceId = x.Estate.District.ProvinceId,
            //                StatusId = x.Estate.District.StatusId,
            //                Used = x.Estate.District.Used,
            //            },
            //            Province = x.Estate.Province == null ? null : new Province
            //            {
            //                Id = x.Estate.Province.Id,
            //                Code = x.Estate.Province.Code,
            //                Name = x.Estate.Province.Name,
            //                Priority = x.Estate.Province.Priority,
            //                StatusId = x.Estate.Province.StatusId,
            //                Used = x.Estate.Province.Used,
            //            },
            //            Ward = x.Estate.Ward == null ? null : new Ward
            //            {
            //                Id = x.Estate.Ward.Id,
            //                Code = x.Estate.Ward.Code,
            //                Name = x.Estate.Ward.Name,
            //                Priority = x.Estate.Ward.Priority,
            //                DistrictId = x.Estate.Ward.DistrictId,
            //                StatusId = x.Estate.Ward.StatusId,
            //                Used = x.Estate.Ward.Used,
            //            },
            //        }
            //    }).ToListAsync();
            return AppUser;
        }

        public async Task<bool> Create(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = new AppUserDAO();
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Address = AppUser.Address;
            AppUserDAO.Avatar = AppUser.Avatar;
            AppUserDAO.Birthday = AppUser.Birthday;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.StatusId = AppUser.StatusId;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.CreatedAt = DateTime.Now;
            AppUserDAO.UpdatedAt = DateTime.Now;
            AppUserDAO.RowId = Guid.NewGuid();
            DataContext.AppUsers.Add(AppUserDAO);
            await DataContext.SaveChangesAsync();
            AppUser.Id = AppUserDAO.Id;
            AppUser.RowId = AppUserDAO.RowId;
            //await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = DataContext.AppUsers.Where(x => x.Id == AppUser.Id).FirstOrDefault();
            if (AppUserDAO == null)
                return false;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Address = AppUser.Address;
            AppUserDAO.Avatar = AppUser.Avatar;
            AppUserDAO.Birthday = AppUser.Birthday;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.StatusId = AppUser.StatusId;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.UpdatedAt = DateTime.Now;
            await DataContext.SaveChangesAsync();
            AppUser.RowId = AppUserDAO.RowId;
            return true;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            await DataContext.AppUsers.Where(x => x.Id == AppUser.Id).UpdateFromQueryAsync(x => new AppUserDAO { DeletedAt = DateTime.Now });
            AppUser.RowId = DataContext.AppUsers.Where(x => x.Id == AppUser.Id).Select(a => a.RowId).FirstOrDefault();
            return true;
        }

        public async Task<bool> BulkMerge(List<AppUser> AppUsers)
        {
            List<AppUserDAO> AppUserDAOs = new List<AppUserDAO>();
            foreach (AppUser AppUser in AppUsers)
            {
                AppUserDAO AppUserDAO = new AppUserDAO();
                AppUserDAO.Id = AppUser.Id;
                AppUserDAO.Username = AppUser.Username;
                AppUserDAO.DisplayName = AppUser.DisplayName;
                AppUserDAO.Address = AppUser.Address;
                AppUserDAO.Avatar = AppUser.Avatar;
                AppUserDAO.Password = AppUser.Password;
                AppUserDAO.Phone = AppUser.Phone;
                AppUserDAO.Email = AppUser.Email;
                AppUserDAO.SexId = AppUser.SexId;
                AppUserDAO.Birthday = AppUser.Birthday;
                AppUserDAO.StatusId = AppUser.StatusId;
                AppUserDAO.CreatedAt = AppUser.CreatedAt;
                AppUserDAO.UpdatedAt = AppUser.UpdatedAt;
                AppUserDAO.RowId = AppUser.RowId;
                AppUserDAOs.Add(AppUserDAO);
            }
            await DataContext.BulkMergeAsync(AppUserDAOs);

            return true;
        }

        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            List<long> Ids = AppUsers.Select(x => x.Id).ToList();
            await DataContext.AppUsers
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new AppUserDAO { DeletedAt = DateTime.Now });
            return true;
        }

    }
}
