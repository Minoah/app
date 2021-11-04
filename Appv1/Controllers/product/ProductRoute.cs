using Appv1.Common;
using Appv1.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Appv1.Controllers.product
{
    [DisplayName("Tài khoản")]
    public class ProductRoute : Root
    {
        public const string Parent = Module + "/product";
        public const string Master = Module + "/product/product-master";
        public const string Detail = Module + "/product/product-detail";
        private const string Default = Controller + Module + "/product";
        public const string Count = Default + "/count";
        public const string CountDistance = Default + "/count-distance";
        public const string List = Default + "/list";
        public const string ListDistance = Default + "/list-distance";
        public const string Get = Default + "/get";
        public const string Create = Default + "/app-user/create";
        public const string Update = Default + "/app-user/update";
        public const string AdminUpdate = Default + "/admin/update";
        public const string Delete = Default + "/app-user/delete";
        public const string AdminDelete = Default + "/admin/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListSex = Default + "/filter-list-sex";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListSex = Default + "/single-list-sex";
        public const string SingleListStatus = Default + "/single-list-status";

    }
}

