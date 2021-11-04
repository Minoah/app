using Appv1.Common;
using Appv1.Entities;
using Appv1.Enums;
using Appv1.Helpers;
using Appv1.Repositories;
using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Appv1.Services.MProduct
{
    public interface IProductService : IServiceScoped
    {
        Task<long> Count(ProductFilter ProductFilter);
        Task<List<Product>> List(ProductFilter ProductFilter);
        Task<Product> Get(long Id);
        Task<Product> Create(Product Product);
        Task<Product> Update(Product Product);
        Task<Product> Delete(Product Product);

        Task<List<Product>> BulkDelete(List<Product> Products);
    }

    public class ProductService : BaseService, IProductService
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        //private IProductValidator ProductValidator;
        public ProductService(
            IUOW UOW,
            ICurrentContext CurrentContext
            //IProductValidator ProductValidator,
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
           // this.ProductValidator = ProductValidator;
        }
        public async Task<long> Count(ProductFilter ProductFilter)
        {
            try
            {
                List<Product> Products = await UOW.ProductRepository.List(ProductFilter);
                GeoCoordinate cCoord = new GeoCoordinate((double)ProductFilter.CurrentLatitude, (double)ProductFilter.CurrentLatitude);
                Products.ForEach(p => {
                    GeoCoordinate pCoord = new GeoCoordinate((double)p.Latitude, (double)p.Latitude);
                    p.Distance = cCoord.GetDistanceTo(pCoord);
                });

                long result = await UOW.ProductRepository.Count(ProductFilter);
                if (ProductFilter.Distance > 0)
                {
                    double Distance = ProductFilter.Distance;
                    result = Products.Where(p => p.Distance <= Distance).Count();
                }
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

        public async Task<List<Product>> List(ProductFilter ProductFilter)
        {
            try
            {
                List<Product> Products = await UOW.ProductRepository.List(ProductFilter);
                if(ProductFilter.Distance > 0)
                {
                    GeoCoordinate cCoord = new GeoCoordinate((double)ProductFilter.CurrentLatitude, (double)ProductFilter.CurrentLatitude);
                    Products.ForEach(p => {
                        GeoCoordinate pCoord = new GeoCoordinate((double)p.Latitude, (double)p.Latitude);
                        p.Distance = cCoord.GetDistanceTo(pCoord);
                    });
                    double Distance = ProductFilter.Distance;
                    Products = Products.Where(p => p.Distance <= Distance).ToList();
                }
                return Products;
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

        public async Task<Product> Get(long Id)
        {
            Product Product = await UOW.ProductRepository.Get(Id);
            if (Product == null)
                return null;
           
            return Product;
        }

        public async Task<Product> Create(Product Product)
        {
            //if (!await ProductValidator.Create(Product))
            //    return Product;

            try
            {
                await UOW.ProductRepository.Create(Product);

                Product = await UOW.ProductRepository.Get(Product.Id);
                return Product;
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

        public async Task<Product> Update(Product Product)
        {
            //if (!await ProductValidator.Update(Product))
            //    return Product;
            try
            {
                var oldData = await UOW.ProductRepository.Get(Product.Id);


                await UOW.ProductRepository.Update(Product);
                return Product;
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

        public async Task<Product> Delete(Product Product)
        {
            //if (!await ProductValidator.Delete(Product))
            //    return Product;

            try
            {
                var oldData = await UOW.ProductRepository.Get(Product.Id);

                await UOW.ProductRepository.Delete(Product);


                Product = await UOW.ProductRepository.Get(Product.Id);
                return Product;
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

        public async Task<List<Product>> BulkDelete(List<Product> Products)
        {
            //if (!await ProductValidator.BulkDelete(Products))
            //    return Products;

            try
            {
                await UOW.ProductRepository.BulkDelete(Products);

                var Ids = Products.Select(x => x.Id).ToList();
                Products = await UOW.ProductRepository.List(Ids);
                return Products;
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
    }
}
