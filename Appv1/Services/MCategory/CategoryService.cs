using Appv1.Common;
using Appv1.Entities;
using Appv1.Enums;
using Appv1.Helpers;
using Appv1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appv1.Services.MCategory
{
    public interface ICategoryService : IServiceScoped
    {
        Task<int> Count(CategoryFilter CategoryFilter);
        Task<List<Category>> List(CategoryFilter CategoryFilter);
        Task<Category> Get(long Id);
        //Task<CategoryFilter> ToFilter(CategoryFilter CategoryFilter);
    }

    public class CategoryService : BaseService, ICategoryService
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ICategoryValidator CategoryValidator;

        public CategoryService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ICategoryValidator CategoryValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.CategoryValidator = CategoryValidator;
        }
        public async Task<int> Count(CategoryFilter CategoryFilter)
        {
            try
            {
                List<Category> Categories = await UOW.CategoryRepository.List(CategoryFilter); // lay ra cac categories thoa man
                
                int result = Categories.Count;
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
            return 0;
        }

        public async Task<List<Category>> List(CategoryFilter CategoryFilter)
        {
            try
            {
                List<Category> Categories = await UOW.CategoryRepository.List(CategoryFilter); // lay ra cac categories thoa man
               
                return Categories;
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
            return null;
        }
        public async Task<Category> Get(long Id)
        {
            Category Category = await UOW.CategoryRepository.Get(Id);
            if (Category == null)
                return null;
            return Category;
        }

    }
}
