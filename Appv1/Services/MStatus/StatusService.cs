using Appv1.Common;
using Appv1.Entities;
using Appv1.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appv1.Services.MStatus
{
    public interface IStatusService : IServiceScoped
    {
        Task<int> Count(StatusFilter StatusFilter);
        Task<List<Status>> List(StatusFilter StatusFilter);
    }

    public class StatusService : BaseService, IStatusService
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public StatusService(
            IUOW UOW,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(StatusFilter StatusFilter)
        {
            try
            {
                int result = await UOW.StatusRepository.Count(StatusFilter);
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

        public async Task<List<Status>> List(StatusFilter StatusFilter)
        {
            try
            {
                List<Status> Statuss = await UOW.StatusRepository.List(StatusFilter);
                return Statuss;
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
