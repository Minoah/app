using Appv1.Common;
using Appv1.Entities;
using Appv1.Enums;
using Appv1.Helpers;
using Appv1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace Appv1.Repositories
{
    public interface IProductRepository
    {
        Task<long> Count(ProductFilter ProductFilter);
        Task<long> CountAll(ProductFilter ProductFilter);
        Task<List<Product>> List(ProductFilter ProductFilter);
        Task<List<Product>> List(List<long> Ids);
        Task<Product> Get(long Id);
        Task<bool> Create(Product Product);
        Task<bool> Update(Product Product);
        Task<bool> Delete(Product Product);
        Task<bool> BulkMerge(List<Product> Products);
        Task<bool> BulkDelete(List<Product> Products);
    }
    public class ProductRepository : IProductRepository
    {
        private DataContext DataContext;
        public ProductRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ProductDAO>> DynamicFilter(IQueryable<ProductDAO> query, ProductFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.DeletedAt == null);

            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.CategoryId, filter.CategoryId);
            query = query.Where(q => q.ProductStatusId, filter.ProductStatusId);
            query = query.Where(q => q.Description, filter.Description);
            query = query.Where(q => q.Address, filter.Address);
            query = query.Where(q => q.Price, filter.Price);
            query = query.Where(q => q.AdminId, filter.AdminId);
            query = query.Where(q => q.AppUserId, filter.AppUserId);
            query = query.Where(q => q.Quantity, filter.Quantity);
            query = query.Where(q => q.Latitude, filter.Latitude);
            query = query.Where(q => q.Longitude, filter.Longitude);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.StatusId, filter.StatusId);

            if (filter.Search != null)
                query = query.Where(q =>
                q.Code.ToLower().Contains(filter.Search.ToLower()) ||
                q.Address.ToLower().Contains(filter.Search.ToLower()) ||
                q.Description.ToLower().Contains(filter.Search.ToLower()) ||
                q.Name.ToLower().Contains(filter.Search.ToLower()));
            return query;
        }

        private IQueryable<ProductDAO> OrFilter(IQueryable<ProductDAO> query, ProductFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProductDAO> initQuery = query.Where(q => false);
            foreach (ProductFilter ProductFilter in filter.OrFilter)
            {
                IQueryable<ProductDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ProductFilter.Id);
                queryable = queryable.Where(q => q.Code, ProductFilter.Code);
                queryable = queryable.Where(q => q.Description, ProductFilter.Description);
                queryable = queryable.Where(q => q.Name, ProductFilter.Name);
                queryable = queryable.Where(q => q.Quantity, ProductFilter.Quantity);
                queryable = queryable.Where(q => q.Price, ProductFilter.Price);
                queryable = queryable.Where(q => q.CreatedAt, ProductFilter.CreatedAt);
                queryable = queryable.Where(q => q.UpdatedAt, ProductFilter.UpdatedAt);
                queryable = queryable.Where(q => q.CategoryId, ProductFilter.CategoryId);
                queryable = queryable.Where(q => q.Address, ProductFilter.Address);
                queryable = queryable.Where(q => q.Latitude, ProductFilter.Latitude);
                queryable = queryable.Where(q => q.Longitude, ProductFilter.Longitude);
                queryable = queryable.Where(q => q.StatusId, ProductFilter.StatusId);
                queryable = queryable.Where(q => q.AppUserId, ProductFilter.AppUserId);
                queryable = queryable.Where(q => q.ProductStatusId, ProductFilter.ProductStatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ProductDAO> DynamicOrder(IQueryable<ProductDAO> query, ProductFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProductOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProductOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ProductOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case ProductOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ProductOrder.Category:
                            query = query.OrderBy(q => q.CategoryId);
                            break;
                        case ProductOrder.Price:
                            query = query.OrderBy(q => q.Price);
                            break;
                        case ProductOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ProductOrder.ProductStatus:
                            query = query.OrderBy(q => q.ProductStatusId);
                            break;
                        case ProductOrder.Quantity:
                            query = query.OrderBy(q => q.Quantity);
                            break;
                        case ProductOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case ProductOrder.Longitude:
                            query = query.OrderBy(q => q.Longitude);
                            break;
                        case ProductOrder.Latitude:
                            query = query.OrderBy(q => q.Latitude);
                            break;
                        case ProductOrder.CreatedAt:
                            query = query.OrderBy(q => q.CreatedAt);
                            break;
                        case ProductOrder.UpdatedAt:
                            query = query.OrderBy(q => q.UpdatedAt);
                            break;
                        case ProductOrder.Admin:
                            query = query.OrderBy(q => q.AdminId);
                            break;
                        case ProductOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProductOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProductOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ProductOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case ProductOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ProductOrder.Category:
                            query = query.OrderByDescending(q => q.CategoryId);
                            break;
                        case ProductOrder.Price:
                            query = query.OrderByDescending(q => q.Price);
                            break;
                        case ProductOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ProductOrder.ProductStatus:
                            query = query.OrderByDescending(q => q.ProductStatusId);
                            break;
                        case ProductOrder.Quantity:
                            query = query.OrderByDescending(q => q.Quantity);
                            break;
                        case ProductOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case ProductOrder.Longitude:
                            query = query.OrderByDescending(q => q.Longitude);
                            break;
                        case ProductOrder.Latitude:
                            query = query.OrderByDescending(q => q.Latitude);
                            break;
                        case ProductOrder.CreatedAt:
                            query = query.OrderByDescending(q => q.CreatedAt);
                            break;
                        case ProductOrder.UpdatedAt:
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                        case ProductOrder.Admin:
                            query = query.OrderByDescending(q => q.AdminId);
                            break;
                        case ProductOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Product>> DynamicSelect(IQueryable<ProductDAO> query, ProductFilter filter)
        {
            List<long> Ids = await query.Select(q => q.Id).ToListAsync();
            IdFilter IdFilter = new IdFilter { In = Ids };
            query = DataContext.Products.AsNoTracking()
                .Where(x => x.Id, IdFilter);
            filter.Skip = 0;
            query = DynamicOrder(query, filter);
            List<Product> Products = await query.Select(q => new Product()
            {
                Id = filter.Selects.Contains(ProductSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ProductSelect.Code) ? q.Code : default(string),
                Description = filter.Selects.Contains(ProductSelect.Description) ? q.Description : default(string),
                Name = filter.Selects.Contains(ProductSelect.Name) ? q.Name : default(string),
                CategoryId = filter.Selects.Contains(ProductSelect.Category) ? q.CategoryId : default(long),
                AdminId = filter.Selects.Contains(ProductSelect.Admin) ? q.AdminId : default(long?),
                ProductStatusId = filter.Selects.Contains(ProductSelect.ProductStatus) ? q.ProductStatusId : default(long),
                Address = filter.Selects.Contains(ProductSelect.Address) ? q.Address : default(string),
                Latitude = filter.Selects.Contains(ProductSelect.Latitude) ? q.Latitude : default(decimal),
                Longitude = filter.Selects.Contains(ProductSelect.Longitude) ? q.Longitude : default(decimal),
                Price = filter.Selects.Contains(ProductSelect.Price) ? q.Price : default(decimal?),
                Quantity = filter.Selects.Contains(ProductSelect.Quantity) ? q.Quantity : default(long),
                StatusId = filter.Selects.Contains(ProductSelect.Status) ? q.StatusId : default(long),
                AppUserId = filter.Selects.Contains(ProductSelect.AppUser) ? q.AppUserId : default(long),
                CreatedAt = filter.Selects.Contains(ProductSelect.CreatedAt) ? q.CreatedAt : default(DateTime),
                UpdatedAt = filter.Selects.Contains(ProductSelect.UpdatedAt) ? q.UpdatedAt : default(DateTime),
                RowId = filter.Selects.Contains(ProductSelect.RowId) ? q.RowId : default(Guid),
                AppUser = filter.Selects.Contains(ProductSelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    DisplayName = q.AppUser.DisplayName,
                } : null,
                Admin = filter.Selects.Contains(ProductSelect.Admin) && q.Admin != null ? new Admin
                {
                    Id = q.Admin.Id,
                    Username = q.Admin.Username,
                    DisplayName = q.Admin.DisplayName,
                } : null,
                Status = filter.Selects.Contains(ProductSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Category = filter.Selects.Contains(ProductSelect.Category) && q.Category != null ? new Category
                {
                    Id = q.Category.Id,
                    Code = q.Category.Code,
                    Name = q.Category.Name,

                } : null,
                ProductStatus = filter.Selects.Contains(ProductSelect.ProductStatus) && q.ProductStatus != null ? new ProductStatus
                {
                    Id = q.ProductStatus.Id,
                    Code = q.ProductStatus.Code,
                    Name = q.ProductStatus.Name,
                } : null,

            }).ToListAsync();

            return Products;
        }

        public async Task<long> Count(ProductFilter filter)
        {
            if (filter == null) return 0;
            IQueryable<ProductDAO> ProductDAOs = DataContext.Products;
            ProductDAOs = await DynamicFilter(ProductDAOs, filter);
            ProductDAOs = OrFilter(ProductDAOs, filter);
            long count = await ProductDAOs.CountAsync();
            return count;
        }

        public async Task<long> CountAll(ProductFilter filter)
        {
            IQueryable<ProductDAO> Products = DataContext.Products;
            Products = await DynamicFilter(Products, filter);
            long count = await Products.CountAsync();
            return count;
        }

        public async Task<List<Product>> List(ProductFilter filter)
        {
            if (filter == null) return new List<Product>();
            IQueryable<ProductDAO> ProductDAOs = DataContext.Products.AsNoTracking();
            ProductDAOs = await DynamicFilter(ProductDAOs, filter);
            ProductDAOs = OrFilter(ProductDAOs, filter);
            ProductDAOs = DynamicOrder(ProductDAOs, filter);
            List<Product> Products = await DynamicSelect(ProductDAOs, filter);
            return Products;
        }

        public async Task<List<Product>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };
            var query = DataContext.Products.AsNoTracking()
                .Where(x => x.Id, IdFilter);
            List<Product> Products = await query
                .Select(x => new Product
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Address = x.Address,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    StatusId = x.StatusId,
                    RowId = x.RowId,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    AppUserId = x.AppUserId,
                    AdminId = x.AdminId,
                    ProductStatusId = x.ProductStatusId,
                    Description = x.Description,
                    AppUser = x.AppUser == null ? null : new AppUser
                    {
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        DisplayName = x.AppUser.DisplayName,
                        Address = x.AppUser.Address,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
                        StatusId = x.AppUser.StatusId,
                        Avatar = x.AppUser.Avatar,
                        SexId = x.AppUser.SexId,
                        Birthday = x.AppUser.Birthday,
                    },
                    Admin = x.Admin == null ? null : new Admin
                    {
                        Id = x.Admin.Id,
                        Username = x.Admin.Username,
                        DisplayName = x.Admin.DisplayName,
                        Address = x.Admin.Address,
                        Email = x.Admin.Email,
                        Phone = x.Admin.Phone,
                        StatusId = x.Admin.StatusId,
                        Avatar = x.Admin.Avatar,
                        SexId = x.Admin.SexId,
                        Birthday = x.Admin.Birthday,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    Category = x.Category == null ? null : new Category
                    {
                        Id = x.Category.Id,
                        Code = x.Category.Code,
                        Name = x.Category.Name,
                        StatusId = x.Category.StatusId,
                        RowId = x.Category.RowId,
                    },
                    ProductStatus = x.ProductStatus == null ? null : new ProductStatus
                    {
                        Id = x.ProductStatus.Id,
                        Code = x.ProductStatus.Code,
                        Name = x.ProductStatus.Name,
                    },
                }).ToListAsync();

            return Products;
        }
        public async Task<Product> Get(long Id)
        {
            Product Product = await DataContext.Products.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Product()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    CategoryId = x.CategoryId,
                    Address = x.Address,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    StatusId = x.StatusId,
                    RowId = x.RowId,
                    AppUserId = x.AppUserId,
                    AdminId = x.AdminId,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ProductStatusId = x.ProductStatusId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Description = x.Description,
                    AppUser = x.AppUser == null ? null : new AppUser
                    {
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        DisplayName = x.AppUser.DisplayName,
                    },
                    Admin = x.Admin == null ? null : new Admin
                    {
                        Id = x.Admin.Id,
                        Username = x.Admin.Username,
                        DisplayName = x.Admin.DisplayName,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    Category = x.Category == null ? null : new Category
                    {
                        Id = x.Category.Id,
                        Code = x.Category.Code,
                        Name = x.Category.Name,
                        StatusId = x.Category.StatusId,
                    },
                    ProductStatus = x.ProductStatus == null ? null : new ProductStatus
                    {
                        Id = x.ProductStatus.Id,
                        Code = x.ProductStatus.Code,
                        Name = x.ProductStatus.Name,
                    },
                }).FirstOrDefaultAsync();

            if (Product == null)
                return null;

            return Product;
        }
        public async Task<bool> Create(Product Product)
        {
            try
            {
                ProductDAO ProductDAO = new ProductDAO();
                ProductDAO.Id = Product.Id;
                ProductDAO.Code = Product.Code;
                ProductDAO.Name = Product.Name;
                ProductDAO.CategoryId = Product.CategoryId;
                ProductDAO.Address = Product.Address;
                ProductDAO.Latitude = Product.Latitude;
                ProductDAO.Longitude = Product.Longitude;
                ProductDAO.StatusId = Product.StatusId;
                ProductDAO.AppUserId = Product.AppUserId;
                ProductDAO.AdminId = Product.AdminId;
                ProductDAO.Price = Product.Price;
                ProductDAO.Quantity = Product.Quantity;
                ProductDAO.ProductStatusId = Product.ProductStatusId;
                ProductDAO.Description = Product.Description;
                ProductDAO.RowId = Guid.NewGuid();
                ProductDAO.CreatedAt = DateTime.Now;
                ProductDAO.UpdatedAt = DateTime.Now;
                DataContext.Products.Add(ProductDAO);
                await DataContext.SaveChangesAsync();
                Product.Id = ProductDAO.Id;
                Product.RowId = ProductDAO.RowId;
                ProductDAO.Code = $"PRODUCT.{Product.Category.Code}.{(10000000 + ProductDAO.Id).ToString().Substring(1)}";
                await DataContext.SaveChangesAsync();
               // await SaveReference(Product);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<bool> Update(Product Product)
        {
            ProductDAO ProductDAO = DataContext.Products.Where(x => x.Id == Product.Id).FirstOrDefault();
            if (ProductDAO == null)
                return false;

            ProductDAO.Code = $"PRODUCT.{Product.Category.Code}.{(10000000 + ProductDAO.Id).ToString().Substring(1)}";
            ProductDAO.Name = Product.Name;
            ProductDAO.CategoryId = Product.CategoryId;
            ProductDAO.Address = Product.Address;
            ProductDAO.Price = Product.Price;
            ProductDAO.Quantity = Product.Quantity;
            ProductDAO.Latitude = Product.Latitude;
            ProductDAO.Longitude = Product.Longitude;
            ProductDAO.StatusId = Product.StatusId;
            ProductDAO.AppUserId = Product.AppUserId;
            ProductDAO.ProductStatusId = Product.ProductStatusId;
            ProductDAO.Description = Product.Description;
            ProductDAO.AdminId = Product.AdminId;
            ProductDAO.UpdatedAt = DateTime.Now;
            await DataContext.SaveChangesAsync();
            //await SaveReference(Product);
            return true;
        }

        public async Task<bool> Delete(Product Product)
        {
            //await DataContext.ProductProductGroupingMapping.Where(x => x.ProductId == Product.Id).DeleteFromQueryAsync();
            //await DataContext.AppUserProductMapping.Where(x => x.ProductId == Product.Id).DeleteFromQueryAsync();
            //await DataContext.Product.Where(x => x.ParentProductId == Product.Id).UpdateFromQueryAsync(x => new ProductDAO { ParentProductId = null });
            await DataContext.Products.Where(x => x.Id == Product.Id).UpdateFromQueryAsync(x => new ProductDAO { DeletedAt = DateTime.Now });
            return true;
        }

        public async Task<bool> BulkMerge(List<Product> Products)
        {
            List<ProductDAO> ProductDAOs = new List<ProductDAO>();
            foreach (Product Product in Products)
            {
                ProductDAO ProductDAO = new ProductDAO();
                ProductDAO.Id = Product.Id;
                ProductDAO.Code = Guid.NewGuid().ToString();
                ProductDAO.Name = Product.Name;
                ProductDAO.CategoryId = Product.CategoryId;
                ProductDAO.Address = Product.Address;
                ProductDAO.Latitude = Product.Latitude;
                ProductDAO.Longitude = Product.Longitude;
                ProductDAO.StatusId = Product.StatusId;
                ProductDAO.AppUserId = Product.AppUserId;
                ProductDAO.AdminId = Product.AdminId;
                ProductDAO.Price = Product.Price;
                ProductDAO.Quantity = Product.Quantity;
                ProductDAO.ProductStatusId = Product.ProductStatusId;
                ProductDAO.RowId = Guid.NewGuid();
                ProductDAO.CreatedAt = Product.CreatedAt == DateTime.MinValue ? DateTime.Now : Product.CreatedAt;
                ProductDAO.UpdatedAt = DateTime.Now;
                ProductDAOs.Add(ProductDAO);
                Product.RowId = ProductDAO.RowId;
            }
            await DataContext.BulkMergeAsync(ProductDAOs);
            foreach (ProductDAO ProductDAO in ProductDAOs)
            {
                Product Product = Products.Where(x => x.RowId == ProductDAO.RowId).FirstOrDefault();
                Product.Id = ProductDAO.Id;
                ProductDAO.Code = $"PRODUCT.{Product.Category.Code}.{(10000000 + ProductDAO.Id).ToString().Substring(1)}";
            }
            await DataContext.BulkMergeAsync(ProductDAOs);

            return true;
        }

        public async Task<bool> BulkDelete(List<Product> Products)
        {
            List<long> Ids = Products.Select(x => x.Id).ToList();
            await DataContext.Products
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ProductDAO { DeletedAt = DateTime.Now });
            return true;
        }
        //private async Task SaveReference(Product Product)
        //{
        //    List<AlbumImageMappingDAO> AlbumImageMappingDAOs = await DataContext.AlbumImageMapping.Where(x => x.ProductId == Product.Id).ToListAsync();
        //    AlbumImageMappingDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
        //    if (Product.AlbumImageMappings != null)
        //    {
        //        foreach (var AlbumImageMapping in Product.AlbumImageMappings)
        //        {
        //            AlbumImageMappingDAO AlbumImageMappingDAO = AlbumImageMappingDAOs.Where(x => x.AlbumId == AlbumImageMapping.AlbumId && x.ImageId == AlbumImageMapping.ImageId).FirstOrDefault();
        //            if (AlbumImageMappingDAO == null)
        //            {
        //                AlbumImageMappingDAO = new AlbumImageMappingDAO();
        //                AlbumImageMappingDAO.AlbumId = AlbumImageMapping.AlbumId;
        //                AlbumImageMappingDAO.ImageId = AlbumImageMapping.ImageId;
        //                AlbumImageMappingDAO.ProductId = Product.Id;
        //                AlbumImageMappingDAO.SaleEmployeeId = AlbumImageMapping.SaleEmployeeId;
        //                AlbumImageMappingDAO.ShootingAt = StaticParams.DateTimeNow;
        //                AlbumImageMappingDAO.DeletedAt = null;
        //                AlbumImageMappingDAOs.Add(AlbumImageMappingDAO);
        //            }
        //            else
        //            {
        //                AlbumImageMappingDAO.AlbumId = AlbumImageMapping.AlbumId;
        //                AlbumImageMappingDAO.ShootingAt = AlbumImageMapping.ShootingAt;
        //                AlbumImageMappingDAO.DeletedAt = null;
        //            }
        //        }
        //    }
        //    await DataContext.AlbumImageMapping.BulkMergeAsync(AlbumImageMappingDAOs);

        //    await DataContext.ProductImageMapping.Where(x => x.ProductId == Product.Id).DeleteFromQueryAsync();
        //    List<ProductImageMappingDAO> ProductImageMappingDAOs = new List<ProductImageMappingDAO>();
        //    if (Product.ProductImageMappings != null)
        //    {
        //        foreach (ProductImageMapping ProductImageMapping in Product.ProductImageMappings)
        //        {
        //            ProductImageMappingDAO ProductImageMappingDAO = new ProductImageMappingDAO();
        //            ProductImageMappingDAO.ProductId = Product.Id;
        //            ProductImageMappingDAO.ImageId = ProductImageMapping.ImageId;
        //            ProductImageMappingDAOs.Add(ProductImageMappingDAO);
        //        }
        //        await DataContext.ProductImageMapping.BulkMergeAsync(ProductImageMappingDAOs);
        //    }

        //    await DataContext.BrandInProductProductGroupingMapping.Where(x => x.BrandInProduct.ProductId == Product.Id).DeleteFromQueryAsync();
        //    await DataContext.BrandInProductShowingCategoryMapping.Where(x => x.BrandInProduct.ProductId == Product.Id).DeleteFromQueryAsync();
        //    List<BrandInProductDAO> BrandInProductDAOs = await DataContext.BrandInProduct.Where(x => x.ProductId == Product.Id).ToListAsync();
        //    BrandInProductDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
        //    if (Product.BrandInProducts != null)
        //    {
        //        foreach (var BrandInProduct in Product.BrandInProducts)
        //        {
        //            BrandInProductDAO BrandInProductDAO = BrandInProductDAOs.Where(x => x.BrandId == BrandInProduct.BrandId).FirstOrDefault();
        //            if (BrandInProductDAO == null)
        //            {
        //                BrandInProductDAO = new BrandInProductDAO
        //                {
        //                    BrandId = BrandInProduct.BrandId,
        //                    ProductId = Product.Id,
        //                    Top = BrandInProduct.Top,
        //                    AdminId = BrandInProduct.AdminId,
        //                    CreatedAt = StaticParams.DateTimeNow,
        //                    UpdatedAt = StaticParams.DateTimeNow,
        //                    RowId = Guid.NewGuid()
        //                };
        //                BrandInProductDAOs.Add(BrandInProductDAO);
        //                BrandInProduct.RowId = BrandInProductDAO.RowId;
        //            }
        //            else
        //            {
        //                BrandInProductDAO.Top = BrandInProduct.Top;
        //                BrandInProductDAO.UpdatedAt = StaticParams.DateTimeNow;
        //                BrandInProductDAO.DeletedAt = null;
        //                BrandInProduct.RowId = BrandInProductDAO.RowId;
        //            }
        //        }
        //        await DataContext.BulkMergeAsync(BrandInProductDAOs);

        //        List<BrandInProductHistoryDAO> BrandInProductHistoryDAOs = new List<BrandInProductHistoryDAO>();

        //        foreach (var BrandInProduct in Product.BrandInProducts)
        //        {
        //            BrandInProductDAO BrandInProductDAO = DataContext.BrandInProduct.Where(x => x.RowId == BrandInProduct.RowId).FirstOrDefault();
        //            BrandInProductHistoryDAO BrandInProductHistoryDAO = DataContext.BrandInProductHistory.Where(x => x.BrandInProductId == BrandInProductDAO.Id).FirstOrDefault();
        //            if (BrandInProductHistoryDAO == null)
        //            {
        //                BrandInProductHistoryDAOs.Add(new BrandInProductHistoryDAO
        //                {
        //                    BrandInProductId = BrandInProductDAO.Id,
        //                    CreatedAt = StaticParams.DateTimeNow,
        //                    AdminId = Product.AdminId,
        //                    Top = BrandInProductDAO.Top,
        //                    BrandId = BrandInProductDAO.BrandId,
        //                    ProductId = BrandInProductDAO.ProductId
        //                });
        //            }
        //            else if (BrandInProductHistoryDAO.Top != BrandInProductDAO.Top)
        //            {
        //                BrandInProductHistoryDAOs.Add(new BrandInProductHistoryDAO
        //                {
        //                    BrandInProductId = BrandInProductDAO.Id,
        //                    CreatedAt = StaticParams.DateTimeNow,
        //                    AdminId = Product.AdminId,
        //                    BrandId = BrandInProductDAO.BrandId,
        //                    ProductId = BrandInProductDAO.ProductId,
        //                    Top = BrandInProductDAO.Top,
        //                });
        //            }
        //        }
        //        await DataContext.BulkMergeAsync(BrandInProductHistoryDAOs);

        //        List<BrandInProductProductGroupingMappingDAO> BrandInProductProductGroupingMappingDAOs = new List<BrandInProductProductGroupingMappingDAO>();
        //        foreach (var BrandInProduct in Product.BrandInProducts)
        //        {
        //            BrandInProduct.Id = BrandInProductDAOs.Where(x => x.RowId == BrandInProduct.RowId).Select(x => x.Id).FirstOrDefault();
        //            if (BrandInProduct.BrandInProductProductGroupingMappings != null)
        //            {
        //                foreach (var BrandInProductProductGroupingMapping in BrandInProduct.BrandInProductProductGroupingMappings)
        //                {
        //                    BrandInProductProductGroupingMappingDAO BrandInProductProductGroupingMappingDAO = new BrandInProductProductGroupingMappingDAO
        //                    {
        //                        BrandInProductId = BrandInProduct.Id,
        //                        ProductGroupingId = BrandInProductProductGroupingMapping.ProductGroupingId
        //                    };
        //                    BrandInProductProductGroupingMappingDAOs.Add(BrandInProductProductGroupingMappingDAO);
        //                }
        //            }
        //        }
        //        await DataContext.BulkMergeAsync(BrandInProductProductGroupingMappingDAOs);

        //        List<BrandInProductShowingCategoryMappingDAO> BrandInProductShowingCategoryMappingDAOs = new List<BrandInProductShowingCategoryMappingDAO>();
        //        foreach (var BrandInProduct in Product.BrandInProducts)
        //        {
        //            BrandInProduct.Id = BrandInProductDAOs.Where(x => x.RowId == BrandInProduct.RowId).Select(x => x.Id).FirstOrDefault();
        //            if (BrandInProduct.BrandInProductShowingCategoryMappings != null)
        //            {
        //                foreach (var BrandInProductShowingCategoryMapping in BrandInProduct.BrandInProductShowingCategoryMappings)
        //                {
        //                    BrandInProductShowingCategoryMappingDAO BrandInProductShowingCategoryMappingDAO = new BrandInProductShowingCategoryMappingDAO
        //                    {
        //                        BrandInProductId = BrandInProduct.Id,
        //                        ShowingCategoryId = BrandInProductShowingCategoryMapping.ShowingCategoryId
        //                    };
        //                    BrandInProductShowingCategoryMappingDAOs.Add(BrandInProductShowingCategoryMappingDAO);
        //                }
        //            }
        //        }
        //        await DataContext.BulkMergeAsync(BrandInProductShowingCategoryMappingDAOs);
        //    }

        //    await DataContext.AppUserProductMapping.Where(x => x.ProductId == Product.Id).DeleteFromQueryAsync();
        //    List<AppUserProductMappingDAO> AppUserProductMappingDAOs = new List<AppUserProductMappingDAO>();
        //    if (Product.AppUserProductMappings != null)
        //    {
        //        foreach (AppUserProductMapping AppUserProductMapping in Product.AppUserProductMappings)
        //        {
        //            AppUserProductMappingDAO AppUserProductMappingDAO = new AppUserProductMappingDAO();
        //            AppUserProductMappingDAO.AppUserId = AppUserProductMapping.AppUserId;
        //            AppUserProductMappingDAO.ProductId = Product.Id;
        //            AppUserProductMappingDAOs.Add(AppUserProductMappingDAO);
        //        }
        //        await DataContext.AppUserProductMapping.BulkMergeAsync(AppUserProductMappingDAOs);
        //    }

        //    await DataContext.ProductProductGroupingMapping
        //        .Where(x => x.ProductId == Product.Id)
        //        .DeleteFromQueryAsync();
        //    List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
        //    if (Product.ProductProductGroupingMappings != null)
        //    {
        //        foreach (ProductProductGroupingMapping ProductProductGroupingMapping in Product.ProductProductGroupingMappings)
        //        {
        //            ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO()
        //            {
        //                ProductId = Product.Id,
        //                ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId
        //            };
        //            ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
        //        }
        //        await DataContext.ProductProductGroupingMapping.BulkMergeAsync(ProductProductGroupingMappingDAOs);
        //    }
        //}
    }
}
