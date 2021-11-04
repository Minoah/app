using Microsoft.EntityFrameworkCore;
using Appv1.Entities;
using Appv1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appv1.Common;

namespace Appv1.Repositories
{
    public interface IProductStatusRepository
    {
        Task<int> Count(ProductStatusFilter ProductStatusFilter);
        Task<List<ProductStatus>> List(ProductStatusFilter ProductStatusFilter);
        Task<ProductStatus> Get(long Id);
        Task<bool> BulkMerge(List<ProductStatus> ProductStatuses);
    }
    public class ProductStatusRepository : IProductStatusRepository
    {
        private DataContext DataContext;
        public ProductStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProductStatusDAO> DynamicFilter(IQueryable<ProductStatusDAO> query, ProductStatusFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ProductStatusDAO> OrFilter(IQueryable<ProductStatusDAO> query, ProductStatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProductStatusDAO> initQuery = query.Where(q => false);
            foreach (ProductStatusFilter ProductStatusFilter in filter.OrFilter)
            {
                IQueryable<ProductStatusDAO> queryable = query;
                if (ProductStatusFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ProductStatusFilter.Id);
                if (ProductStatusFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, ProductStatusFilter.Code);
                if (ProductStatusFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ProductStatusFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ProductStatusDAO> DynamicOrder(IQueryable<ProductStatusDAO> query, ProductStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProductStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProductStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ProductStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProductStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProductStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ProductStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ProductStatus>> DynamicSelect(IQueryable<ProductStatusDAO> query, ProductStatusFilter filter)
        {
            List<ProductStatus> ProductStatuses = await query.Select(q => new ProductStatus()
            {
                Id = filter.Selects.Contains(ProductStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ProductStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ProductStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return ProductStatuses;
        }

        public async Task<int> Count(ProductStatusFilter filter)
        {
            IQueryable<ProductStatusDAO> ProductStatuss = DataContext.ProductStatuses;
            ProductStatuss = DynamicFilter(ProductStatuss, filter);
            return await ProductStatuss.CountAsync();
        }

        public async Task<List<ProductStatus>> List(ProductStatusFilter filter)
        {
            if (filter == null) return new List<ProductStatus>();
            IQueryable<ProductStatusDAO> ProductStatusDAOs = DataContext.ProductStatuses;
            ProductStatusDAOs = DynamicFilter(ProductStatusDAOs, filter);
            ProductStatusDAOs = DynamicOrder(ProductStatusDAOs, filter);
            List<ProductStatus> ProductStatuses = await DynamicSelect(ProductStatusDAOs, filter);
            return ProductStatuses;
        }

        public async Task<ProductStatus> Get(long Id)
        {
            ProductStatus ProductStatus = await DataContext.ProductStatuses.Where(x => x.Id == Id).Select(x => new ProductStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            return ProductStatus;
        }
        public async Task<bool> BulkMerge(List<ProductStatus> ProductStatuses)
        {
            List<ProductStatusDAO> ProductStatusDAOs = ProductStatuses.Select(x => new ProductStatusDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            await DataContext.BulkMergeAsync(ProductStatusDAOs);
            return true;
        }

    }
}
