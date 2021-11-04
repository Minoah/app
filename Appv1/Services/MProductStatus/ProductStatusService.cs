using Appv1.Common;
using Appv1.Entities;
using Appv1.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appv1.Services.MProductStatus
{
    public interface IProductStatusService : IServiceScoped
    {
        Task<int> Count(ProductStatusFilter ProductStatusFilter);
        Task<List<ProductStatus>> List(ProductStatusFilter ProductStatusFilter);
    }

    public class ProductStatusService : BaseService, IProductStatusService
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProductStatusService(
            IUOW UOW,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(ProductStatusFilter ProductStatusFilter)
        {
            try
            {
                int result = await UOW.ProductStatusRepository.Count(ProductStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ProductStatus>> List(ProductStatusFilter ProductStatusFilter)
        {
            try
            {
                List<ProductStatus> ProductStatuss = await UOW.ProductStatusRepository.List(ProductStatusFilter);
                return ProductStatuss;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
