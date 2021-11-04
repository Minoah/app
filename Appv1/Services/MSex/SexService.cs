using Appv1.Common;
using Appv1.Entities;
//sing Appv1.Helpers;
using Appv1.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appv1.Services.MSex
{
    public interface ISexService : IServiceScoped
    {
        Task<int> Count(SexFilter SexFilter);
        Task<List<Sex>> List(SexFilter SexFilter);
    }

    public class SexService : BaseService, ISexService
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ISexValidator SexValidator;

        public SexService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ISexValidator SexValidator
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.SexValidator = SexValidator;
        }
        public async Task<int> Count(SexFilter SexFilter)
        {
            try
            {
                int result = await UOW.SexRepository.Count(SexFilter);
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

        public async Task<List<Sex>> List(SexFilter SexFilter)
        {
            try
            {
                List<Sex> Sexs = await UOW.SexRepository.List(SexFilter);
                return Sexs;
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
