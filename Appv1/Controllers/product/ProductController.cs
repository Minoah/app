using Appv1.Common;
using Appv1.Entities;
using Appv1.Services.MAdmin;
using Appv1.Services.MAppUser;
using Appv1.Services.MCategory;
using Appv1.Services.MProduct;
using Appv1.Services.MProductStatus;
using Appv1.Services.MStatus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appv1.Controllers.product
{
    public class ProductController : ControllerBase
    {
        private IAppUserService AppUserService;
        private IStatusService StatusService;
        private ICategoryService CategoryService;
        private IProductStatusService ProductStatusService;
        private IProductService ProductService;
        private IAdminService AdminService;
        private ICurrentContext CurrentContext;
        public ProductController(
            IAppUserService AppUserService,
            IStatusService StatusService,
            ICategoryService CategoryService,
            IProductStatusService ProductStatusService, 
            IProductService ProductService,
            IAdminService AdminService,
            ICurrentContext CurrentContext
        )
        {
            this.CategoryService = CategoryService;
            this.AppUserService = AppUserService;
            this.StatusService = StatusService;
            this.ProductStatusService = ProductStatusService;
            this.ProductService = ProductService;
            this.AdminService = AdminService;
            this.CurrentContext = CurrentContext;
        }
        [Route(ProductRoute.Count), HttpPost]
        public async Task<ActionResult<long>> Count([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            //ProductFilter = AdminService.ToFilter(ProductFilter);
            long count = await ProductService.Count(ProductFilter);
            return count;
        }

        [Route(ProductRoute.List), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> List([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            //ProductFilter = ProductService.ToFilter(ProductFilter);
            List<Product> Products = await ProductService.List(ProductFilter);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs;
        }

        [Route(ProductRoute.Get), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Get([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);


            Product Product = await ProductService.Get(Product_ProductDTO.Id);
            return new Product_ProductDTO(Product);
        }
        private Product ConvertDTOToEntity(Product_ProductDTO Product_ProductDTO)
        {
            Product Product = new Product();
            Product.Id = Product_ProductDTO.Id;
            Product.Code = Product_ProductDTO.Code;
            Product.Description = Product_ProductDTO.Description;
            Product.Name = Product_ProductDTO.Name;
            Product.Address = Product_ProductDTO.Address;
            Product.CategoryId = Product_ProductDTO.CategoryId;
            Product.Price = Product_ProductDTO.Price;
            Product.ProductStatusId = Product_ProductDTO.ProductStatusId;
            Product.StatusId = Product_ProductDTO.StatusId;
            Product.AppUserId = Product_ProductDTO.AppUserId;
            Product.AdminId = Product_ProductDTO.AdminId;
            Product.Quantity = Product_ProductDTO.Quantity;
            Product.Longitude = Product_ProductDTO.Longitude;
            Product.Latitude = Product_ProductDTO.Latitude;
            Product.Address = Product_ProductDTO.Address;
            Product.CreatedAt = Product_ProductDTO.CreatedAt;
            Product.UpdatedAt = Product_ProductDTO.UpdatedAt;
            Product.RowId = Product_ProductDTO.RowId;
            Product.Distance = Product_ProductDTO.Distance;
            Product.AppUser = Product_ProductDTO.AppUser == null ? null : new AppUser
            {
                Id = Product_ProductDTO.AppUser.Id,
                Username = Product_ProductDTO.AppUser.Username,
                DisplayName = Product_ProductDTO.AppUser.DisplayName,
                Address = Product_ProductDTO.AppUser.Address,
                Birthday = Product_ProductDTO.AppUser.Birthday,
                Email = Product_ProductDTO.AppUser.Email,
                Phone = Product_ProductDTO.AppUser.Phone,
            }; 
            Product.Admin = Product_ProductDTO.Admin == null ? null : new Admin
            {
                Id = Product_ProductDTO.Admin.Id,
                Username = Product_ProductDTO.Admin.Username,
                DisplayName = Product_ProductDTO.Admin.DisplayName,
                Address = Product_ProductDTO.Admin.Address,
                Birthday = Product_ProductDTO.Admin.Birthday,
                Email = Product_ProductDTO.Admin.Email,
                Phone = Product_ProductDTO.Admin.Phone,
            };

            Product.Status = Product_ProductDTO.Status == null ? null : new Status
            {
                Id = Product_ProductDTO.Status.Id,
                Code = Product_ProductDTO.Status.Code,
                Name = Product_ProductDTO.Status.Name,
            };
            Product.ProductStatus = Product_ProductDTO.ProductStatus == null ? null : new ProductStatus
            {
                Id = Product_ProductDTO.ProductStatus.Id,
                Code = Product_ProductDTO.ProductStatus.Code,
                Name = Product_ProductDTO.ProductStatus.Name,
            };
            Product.Category = Product_ProductDTO.Category == null ? null : new Category
            {
                Id = Product_ProductDTO.Category.Id,
                Code = Product_ProductDTO.Category.Code,
                Name = Product_ProductDTO.Category.Name,
                Description = Product_ProductDTO.Category.Description,
                StatusId = Product_ProductDTO.Category.StatusId
            };


            return Product;
        }

        private ProductFilter ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter.Skip = Product_ProductFilterDTO.Skip;
            ProductFilter.Take = Product_ProductFilterDTO.Take;
            ProductFilter.OrderBy = Product_ProductFilterDTO.OrderBy;
            ProductFilter.OrderType = Product_ProductFilterDTO.OrderType;

            ProductFilter.Id = Product_ProductFilterDTO.Id;
            ProductFilter.Code = Product_ProductFilterDTO.Code;
            ProductFilter.Description = Product_ProductFilterDTO.Description;
            ProductFilter.Name = Product_ProductFilterDTO.Name;
            ProductFilter.Address = Product_ProductFilterDTO.Address;
            ProductFilter.Search = Product_ProductFilterDTO.Search;
            ProductFilter.AdminId = Product_ProductFilterDTO.AdminId;
            ProductFilter.AppUserId = Product_ProductFilterDTO.AppUserId;
            ProductFilter.Longitude = Product_ProductFilterDTO.Longitude;
            ProductFilter.Latitude = Product_ProductFilterDTO.Latitude;
            ProductFilter.Distance = Product_ProductFilterDTO.Distance;
            ProductFilter.StatusId = Product_ProductFilterDTO.StatusId;
            ProductFilter.ProductStatusId = Product_ProductFilterDTO.ProductStatusId;
            ProductFilter.Quantity = Product_ProductFilterDTO.Quantity;
            ProductFilter.Price = Product_ProductFilterDTO.Price;
            ProductFilter.RowId = Product_ProductFilterDTO.RowId;
            ProductFilter.CreatedAt = Product_ProductFilterDTO.CreatedAt;
            return ProductFilter;
        }
    }
}
