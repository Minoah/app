using Appv1.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Appv1.Controllers.admin
{
    [DisplayName("Tài khoản")]
    public class AdminRoute : Root
    {
        public const string Parent = Module + "/account";
        public const string Master = Module + "/admin/admin-master";
        public const string Detail = Module + "/admin/admin-detail";
        public const string Mobile = Module + "/admin/master-data.profile";
        private const string Default = Controller + Module + "/admin";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string ChangePassword = Default + "/change-password";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListSex = Default + "/filter-list-sex";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListSex = Default + "/single-list-sex";
        public const string SingleListStatus = Default + "/single-list-status";
        
    }
}

