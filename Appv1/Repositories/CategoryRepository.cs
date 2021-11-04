using Appv1.Entities;
using Appv1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appv1.Common;

namespace Appv1.Repositories
{
    public interface ICategoryRepository
    {
        Task<int> Count(CategoryFilter CategoryFilter);
        Task<int> CountAll(CategoryFilter CategoryFilter);
        Task<List<Category>> List(CategoryFilter CategoryFilter);
        Task<Category> Get(long Id);
        Task<bool> BulkMerge(List<Category> Categories);
    }
    public class CategoryRepository : ICategoryRepository
    {
        private DataContext DataContext;
        public CategoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<CategoryDAO> DynamicFilter(IQueryable<CategoryDAO> query, CategoryFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.RowId, filter.RowId);
            return query;
        }

        private IQueryable<CategoryDAO> DynamicOrder(IQueryable<CategoryDAO> query, CategoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CategoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CategoryOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case CategoryOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case CategoryOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CategoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CategoryOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case CategoryOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case CategoryOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Category>> DynamicSelect(IQueryable<CategoryDAO> query, CategoryFilter filter)
        {
            List<Category> Categories = await query.Select(q => new Category()
            {
                Id = filter.Selects.Contains(CategorySelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(CategorySelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(CategorySelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(CategorySelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(CategorySelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();

            return Categories;
        }

        public async Task<int> Count(CategoryFilter filter)
        {
            IQueryable<CategoryDAO> Categories = DataContext.Categories.AsNoTracking();
            Categories = DynamicFilter(Categories, filter);
            return await Categories.CountAsync();
        }
        public async Task<int> CountAll(CategoryFilter filter)
        {
            IQueryable<CategoryDAO> Categories = DataContext.Categories.AsNoTracking();
            Categories = DynamicFilter(Categories, filter);
            return await Categories.CountAsync();
        }
        public async Task<List<Category>> List(CategoryFilter filter)
        {
            if (filter == null) return new List<Category>();
            IQueryable<CategoryDAO> CategoryDAOs = DataContext.Categories.AsNoTracking();
            CategoryDAOs = DynamicFilter(CategoryDAOs, filter);
            CategoryDAOs = DynamicOrder(CategoryDAOs, filter);
            List<Category> Categories = await DynamicSelect(CategoryDAOs, filter);
            return Categories;
        }

        public async Task<Category> Get(long Id)
        {
            Category Category = await DataContext.Categories.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new Category()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                RowId = x.RowId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Category == null)
                return null;

            return Category;
        }

        public async Task<bool> BulkMerge(List<Category> Categories)
        {
            List<CategoryDAO> CategoryDAOs = new List<CategoryDAO>();
            //List<ImageDAO> ImageDAOs = new List<ImageDAO>();
            foreach (Category Category in Categories)
            {
                CategoryDAO CategoryDAO = new CategoryDAO();
                CategoryDAO.Id = Category.Id;
                CategoryDAO.Code = Category.Code;
                CategoryDAO.CreatedAt = Category.CreatedAt;
                CategoryDAO.UpdatedAt = Category.UpdatedAt;
                CategoryDAO.DeletedAt = Category.DeletedAt;
                CategoryDAO.Id = Category.Id;
                CategoryDAO.Name = Category.Name;
                CategoryDAO.RowId = Category.RowId;
                CategoryDAO.StatusId = Category.StatusId;
                CategoryDAOs.Add(CategoryDAO);

            }
            //await DataContext.BulkMergeAsync(ImageDAOs);
            await DataContext.BulkMergeAsync(CategoryDAOs);
            return true;
        }
    }
}
