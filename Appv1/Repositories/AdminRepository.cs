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
    public interface IAdminRepository
    {
        Task<int> Count(AdminFilter AdminFilter);
        Task<int> CountAll(AdminFilter AdminFilter);
        Task<List<Admin>> List(AdminFilter AdminFilter);
        Task<List<Admin>> List(List<long> Ids);
        Task<Admin> Get(long Id);
        Task<bool> Create(Admin Admin);
        Task<bool> Update(Admin Admin);
        Task<bool> Delete(Admin Admin);
        Task<bool> BulkMerge(List<Admin> Admins);
        Task<bool> BulkDelete(List<Admin> Admins);
    }
    public class AdminRepository : IAdminRepository
    {
        private DataContext DataContext;
        public AdminRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<AdminDAO> DynamicFilter(IQueryable<AdminDAO> query, AdminFilter filter)
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
        private IQueryable<AdminDAO> DynamicOrder(IQueryable<AdminDAO> query, AdminFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AdminOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case AdminOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case AdminOrder.Password:
                            query = query.OrderBy(q => q.Password);
                            break;
                        case AdminOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case AdminOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case AdminOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case AdminOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case AdminOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case AdminOrder.Sex:
                            query = query.OrderBy(q => q.Sex);
                            break;
                        case AdminOrder.Birthday:
                            query = query.OrderBy(q => q.Birthday);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AdminOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case AdminOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case AdminOrder.Password:
                            query = query.OrderByDescending(q => q.Password);
                            break;
                        case AdminOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case AdminOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case AdminOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case AdminOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case AdminOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case AdminOrder.Sex:
                            query = query.OrderByDescending(q => q.Sex);
                            break;
                        case AdminOrder.Birthday:
                            query = query.OrderByDescending(q => q.Birthday);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Admin>> DynamicSelect(IQueryable<AdminDAO> query, AdminFilter filter)
        {
            List<Admin> Admins = await query.Select(q => new Admin()
            {
                Id = filter.Selects.Contains(AdminSelect.Id) ? q.Id : default(long),
                Username = filter.Selects.Contains(AdminSelect.Username) ? q.Username : default(string),
                Password = filter.Selects.Contains(AdminSelect.Password) ? q.Password : default(string),
                DisplayName = filter.Selects.Contains(AdminSelect.DisplayName) ? q.DisplayName : default(string),
                Address = filter.Selects.Contains(AdminSelect.Address) ? q.Address : default(string),
                Email = filter.Selects.Contains(AdminSelect.Email) ? q.Email : default(string),
                Phone = filter.Selects.Contains(AdminSelect.Phone) ? q.Phone : default(string),
                StatusId = filter.Selects.Contains(AdminSelect.Status) ? q.StatusId : default(long),
                SexId = filter.Selects.Contains(AdminSelect.Sex) ? q.SexId : default(long),
                Birthday = filter.Selects.Contains(AdminSelect.Birthday) ? q.Birthday : null,
                Avatar = filter.Selects.Contains(AdminSelect.Avatar) ? q.Avatar: default(string),
                Status = filter.Selects.Contains(AdminSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Sex = filter.Selects.Contains(AdminSelect.Sex) && q.Sex != null ? new Sex
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

            //var Ids = Admins.Select(x => x.Id).ToList();
            //var AdminEstateMappings = await DataContext.AdminEstateMapping.Where(x => Ids.Contains(x.AdminId)).Select(x => new AdminEstateMapping
            //{
            //    AdminId = x.AdminId,
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

            //foreach (var Admin in Admins)
            //{
            //    Admin.AdminEstateMappings = AdminEstateMappings.Where(x => x.AdminId == Admin.Id).ToList();
            //}
            return Admins;
        }

        public async Task<int> Count(AdminFilter filter)
        {
            IQueryable<AdminDAO> Admins = DataContext.Admins.AsNoTracking();
            Admins = DynamicFilter(Admins, filter);
            return await Admins.CountAsync();
        }

        public async Task<int> CountAll(AdminFilter filter)
        {
            IQueryable<AdminDAO> Admins = DataContext.Admins.AsNoTracking();
            Admins = DynamicFilter(Admins, filter);
            return await Admins.CountAsync();
        }

        public async Task<List<Admin>> List(AdminFilter filter)
        {
            if (filter == null) return new List<Admin>();
            IQueryable<AdminDAO> AdminDAOs = DataContext.Admins.AsNoTracking();
            AdminDAOs = DynamicFilter(AdminDAOs, filter);
            AdminDAOs = DynamicOrder(AdminDAOs, filter);
            List<Admin> Admins = await DynamicSelect(AdminDAOs, filter);
            return Admins;
        }

        public async Task<List<Admin>> List(List<long> Ids)
        {
            List<Admin> Admins = await DataContext.Admins.AsNoTracking()
                .Where(x => Ids.Contains(x.Id)).Select(x => new Admin()
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


            return Admins;
        }

        public async Task<Admin> Get(long Id)
        {
            Admin Admin = await DataContext.Admins.AsNoTracking()
                .Where(x => x.Id == Id)
                .Select(x => new Admin()
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

            if (Admin == null)
                return null;
            //Admin.AdminEstateMappings = await DataContext.AdminEstateMapping
            //    .Where(x => x.AdminId == Admin.Id)
            //    .OrderBy(x => x.EstateId)
            //    .Select(x => new AdminEstateMapping
            //    {
            //        AdminId = x.AdminId,
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
            return Admin;
        }

        public async Task<bool> Create(Admin Admin)
        {
            AdminDAO AdminDAO = new AdminDAO();
            AdminDAO.Id = Admin.Id;
            AdminDAO.Username = Admin.Username;
            AdminDAO.Password = Admin.Password;
            AdminDAO.DisplayName = Admin.DisplayName;
            AdminDAO.Address = Admin.Address;
            AdminDAO.Avatar = Admin.Avatar;
            AdminDAO.Birthday = Admin.Birthday;
            AdminDAO.Email = Admin.Email;
            AdminDAO.Phone = Admin.Phone;
            AdminDAO.StatusId = Admin.StatusId;
            AdminDAO.SexId = Admin.SexId;
            AdminDAO.CreatedAt = DateTime.Now;
            AdminDAO.UpdatedAt = DateTime.Now;
            AdminDAO.RowId = Guid.NewGuid();
            DataContext.Admins.Add(AdminDAO);
            await DataContext.SaveChangesAsync();
            Admin.Id = AdminDAO.Id;
            Admin.RowId = AdminDAO.RowId;
            //await SaveReference(Admin);
            return true;
        }

        public async Task<bool> Update(Admin Admin)
        {
            AdminDAO AdminDAO = DataContext.Admins.Where(x => x.Id == Admin.Id).FirstOrDefault();
            if (AdminDAO == null)
                return false;
            AdminDAO.Username = Admin.Username;
            AdminDAO.Password = Admin.Password;
            AdminDAO.DisplayName = Admin.DisplayName;
            AdminDAO.Address = Admin.Address;
            AdminDAO.Avatar = Admin.Avatar;
            AdminDAO.Birthday = Admin.Birthday;
            AdminDAO.Email = Admin.Email;
            AdminDAO.Phone = Admin.Phone;
            AdminDAO.StatusId = Admin.StatusId;
            AdminDAO.SexId = Admin.SexId;
            AdminDAO.UpdatedAt = DateTime.Now;
            await DataContext.SaveChangesAsync();
            Admin.RowId = AdminDAO.RowId;
            return true;
        }

        public async Task<bool> Delete(Admin Admin)
        {
            await DataContext.Admins.Where(x => x.Id == Admin.Id).UpdateFromQueryAsync(x => new AdminDAO { DeletedAt = DateTime.Now });
            Admin.RowId = DataContext.Admins.Where(x => x.Id == Admin.Id).Select(a => a.RowId).FirstOrDefault();
            return true;
        }

        public async Task<bool> BulkMerge(List<Admin> Admins)
        {
            List<AdminDAO> AdminDAOs = new List<AdminDAO>();
            foreach (Admin Admin in Admins)
            {
                AdminDAO AdminDAO = new AdminDAO();
                AdminDAO.Id = Admin.Id;
                AdminDAO.Username = Admin.Username;
                AdminDAO.DisplayName = Admin.DisplayName;
                AdminDAO.Address = Admin.Address;
                AdminDAO.Avatar = Admin.Avatar;
                AdminDAO.Password = Admin.Password;
                AdminDAO.Phone = Admin.Phone;
                AdminDAO.Email = Admin.Email;
                AdminDAO.SexId = Admin.SexId;
                AdminDAO.Birthday = Admin.Birthday;
                AdminDAO.StatusId = Admin.StatusId;
                AdminDAO.CreatedAt = Admin.CreatedAt;
                AdminDAO.UpdatedAt = Admin.UpdatedAt;
                AdminDAO.RowId = Admin.RowId;
                AdminDAOs.Add(AdminDAO);
            }
            await DataContext.BulkMergeAsync(AdminDAOs);

            return true;
        }

        public async Task<bool> BulkDelete(List<Admin> Admins)
        {
            List<long> Ids = Admins.Select(x => x.Id).ToList();
            await DataContext.Admins
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new AdminDAO { DeletedAt = DateTime.Now });
            return true;
        }

    }
}
