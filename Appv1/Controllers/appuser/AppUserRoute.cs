using Appv1.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Appv1.Controllers.appuser
{
    [DisplayName("Tài khoản")]
    public class AppUserRoute : Root
    {
        public const string Parent = Module + "/account";
        public const string Master = Module + "/appuser/appuser-master";
        public const string Detail = Module + "/appuser/appuser-detail";
        public const string Mobile = Module + "/appuser/master-data.profile";
        private const string Default = Controller + Module + "/appuser";
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

