using Appv1.Common;
using Appv1.Models;
using System;
using Appv1.Common;

namespace Appv1.Repositories
{
    public interface IUOW : IServiceScoped, IDisposable
    {
        IAdminRepository AdminRepository { get; }
        IAppUserRepository AppUserRepository { get; }
        ISexRepository SexRepository { get; }
        IStatusRepository StatusRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductStatusRepository ProductStatusRepository { get; }
        IProductRepository ProductRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;

        public IAdminRepository AdminRepository { get; private set; }
        public IAppUserRepository AppUserRepository { get; private set; }
        public ISexRepository SexRepository { get; private set; }
        public IStatusRepository StatusRepository { get; private set; }
        public IProductStatusRepository ProductStatusRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }

        public UOW(DataContext DataContext, ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;

            AdminRepository = new AdminRepository(DataContext);
            AppUserRepository = new AppUserRepository(DataContext);
            SexRepository = new SexRepository(DataContext);
            StatusRepository = new StatusRepository(DataContext);
            ProductStatusRepository = new ProductStatusRepository(DataContext);
            ProductRepository = new ProductRepository(DataContext);
            CategoryRepository = new CategoryRepository(DataContext);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.DataContext == null)
            {
                return;
            }
            this.DataContext.Dispose();
            this.DataContext = null;
        }
    }
}