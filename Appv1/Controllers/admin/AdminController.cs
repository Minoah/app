using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Appv1.Common;
using Appv1.Entities;
using Appv1.Services.MAdmin;
using Appv1.Services.MSex;
using Appv1.Services.MStatus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Appv1.Enums;

namespace Appv1.Controllers.admin
{
    public class AdminControllers : RpcController
    {
        private ISexService SexService;
        private IStatusService StatusService;
        private IAdminService AdminService;
        private ICurrentContext CurrentContext;
        public AdminControllers(
            ISexService SexService,
            IStatusService StatusService,
            IAdminService AdminService,
            ICurrentContext CurrentContext
        )
        {
            this.SexService = SexService;
            this.StatusService = StatusService;
            this.AdminService = AdminService;
            this.CurrentContext = CurrentContext;
        }

        [Route(AdminRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Admin_AdminFilterDTO Admin_AdminFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AdminFilter AdminFilter = ConvertFilterDTOToFilterEntity(Admin_AdminFilterDTO);
            //AdminFilter = AdminService.ToFilter(AdminFilter);
            int count = await AdminService.Count(AdminFilter);
            return count;
        }

        [Route(AdminRoute.List), HttpPost]
        public async Task<ActionResult<List<Admin_AdminDTO>>> List([FromBody] Admin_AdminFilterDTO Admin_AdminFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AdminFilter AdminFilter = ConvertFilterDTOToFilterEntity(Admin_AdminFilterDTO);
            AdminFilter = AdminService.ToFilter(AdminFilter);
            List<Admin> Admins = await AdminService.List(AdminFilter);
            List<Admin_AdminDTO> Admin_AdminDTOs = Admins
                .Select(c => new Admin_AdminDTO(c)).ToList();
            return Admin_AdminDTOs;
        }

        [Route(AdminRoute.Get), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> Get([FromBody] Admin_AdminDTO Admin_AdminDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);


            Admin Admin = await AdminService.Get(Admin_AdminDTO.Id);
            return new Admin_AdminDTO(Admin);
        }

        [Route(AdminRoute.Create), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> Create([FromBody] Admin_AdminDTO Admin_AdminDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);


            Admin Admin = ConvertDTOToEntity(Admin_AdminDTO);
            Admin = await AdminService.Create(Admin);
            Admin_AdminDTO = new Admin_AdminDTO(Admin);
            if (Admin.IsValidated)
                return Admin_AdminDTO;
            else
                return BadRequest(Admin_AdminDTO);
        }

        [Route(AdminRoute.Update), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> Update([FromBody] Admin_AdminDTO Admin_AdminDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);


            Admin Admin = ConvertDTOToEntity(Admin_AdminDTO);
            Admin = await AdminService.Update(Admin);
            Admin_AdminDTO = new Admin_AdminDTO(Admin);
            if (Admin.IsValidated)
                return Admin_AdminDTO;
            else
                return BadRequest(Admin_AdminDTO);
        }

        [Route(AdminRoute.Delete), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> Delete([FromBody] Admin_AdminDTO Admin_AdminDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);


            Admin Admin = ConvertDTOToEntity(Admin_AdminDTO);
            Admin = await AdminService.Delete(Admin);
            Admin_AdminDTO = new Admin_AdminDTO(Admin);
            if (Admin.IsValidated)
                return Admin_AdminDTO;
            else
                return BadRequest(Admin_AdminDTO);
        }

        [Route(AdminRoute.ChangePassword), HttpPost]
        public async Task<ActionResult<Admin_AdminDTO>> ChangePassword([FromBody] Admin_ChangePasswordDTO Admin_ChangePasswordDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            Admin Admin = new Admin
            {
                Id = Admin_ChangePasswordDTO.Id,
                NewPassword = Admin_ChangePasswordDTO.NewPassword,
            };
            Admin = await AdminService.AdminChangePassword(Admin);
            Admin_AdminDTO Admin_AdminDTO = new Admin_AdminDTO(Admin);
            if (Admin.IsValidated)
                return Admin_AdminDTO;
            else
                return BadRequest(Admin_AdminDTO);
        }

        [Route(AdminRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AdminFilter AdminFilter = new AdminFilter();
            AdminFilter = AdminService.ToFilter(AdminFilter);
            AdminFilter.Id = new IdFilter { In = Ids };
            AdminFilter.Selects = AdminSelect.Id | AdminSelect.RowId;
            AdminFilter.Skip = 0;
            AdminFilter.Take = int.MaxValue;

            List<Admin> Admins = await AdminService.List(AdminFilter);
            Admins = await AdminService.BulkDelete(Admins);
            if (Admins.Any(x => !x.IsValidated))
                return BadRequest(Admins.Where(x => !x.IsValidated));
            return true;
        }
        private Admin ConvertDTOToEntity(Admin_AdminDTO Admin_AdminDTO)
        {
            Admin Admin = new Admin();
            Admin.Id = Admin_AdminDTO.Id;
            Admin.Username = Admin_AdminDTO.Username;
            Admin.Password = Admin_AdminDTO.Password;
            Admin.DisplayName = Admin_AdminDTO.DisplayName;
            Admin.Address = Admin_AdminDTO.Address;
            Admin.Avatar = Admin_AdminDTO.Avatar;
            Admin.Birthday = Admin_AdminDTO.Birthday;
            Admin.Email = Admin_AdminDTO.Email;
            Admin.Phone = Admin_AdminDTO.Phone;
            Admin.SexId = Admin_AdminDTO.SexId;
            Admin.StatusId = Admin_AdminDTO.StatusId;
            Admin.Sex = Admin_AdminDTO.Sex == null ? null : new Sex
            {
                Id = Admin_AdminDTO.Sex.Id,
                Code = Admin_AdminDTO.Sex.Code,
                Name = Admin_AdminDTO.Sex.Name,
            };
            Admin.Status = Admin_AdminDTO.Status == null ? null : new Status
            {
                Id = Admin_AdminDTO.Status.Id,
                Code = Admin_AdminDTO.Status.Code,
                Name = Admin_AdminDTO.Status.Name,
            };
            return Admin;
        }

        private AdminFilter ConvertFilterDTOToFilterEntity(Admin_AdminFilterDTO Admin_AdminFilterDTO)
        {
            AdminFilter AdminFilter = new AdminFilter();
            AdminFilter.Selects = AdminSelect.ALL;
            AdminFilter.Skip = Admin_AdminFilterDTO.Skip;
            AdminFilter.Take = Admin_AdminFilterDTO.Take;
            AdminFilter.OrderBy = Admin_AdminFilterDTO.OrderBy;
            AdminFilter.OrderType = Admin_AdminFilterDTO.OrderType;

            AdminFilter.Id = Admin_AdminFilterDTO.Id;
            AdminFilter.Username = Admin_AdminFilterDTO.Username;
            AdminFilter.Password = Admin_AdminFilterDTO.Password;
            AdminFilter.DisplayName = Admin_AdminFilterDTO.DisplayName;
            AdminFilter.Address = Admin_AdminFilterDTO.Address;
            AdminFilter.Email = Admin_AdminFilterDTO.Email;
            AdminFilter.Phone = Admin_AdminFilterDTO.Phone;
            AdminFilter.SexId = Admin_AdminFilterDTO.SexId;
            AdminFilter.StatusId = Admin_AdminFilterDTO.StatusId;
            return AdminFilter;
        }

        [Route(AdminRoute.FilterListSex), HttpPost]
        public async Task<List<Admin_SexDTO>> FilterListSex([FromBody] Admin_SexFilterDTO Admin_SexFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SexFilter SexFilter = new SexFilter();
            SexFilter.Skip = 0;
            SexFilter.Take = 20;
            SexFilter.OrderBy = SexOrder.Id;
            SexFilter.OrderType = OrderType.ASC;
            SexFilter.Selects = SexSelect.ALL;
            SexFilter.Id = Admin_SexFilterDTO.Id;
            SexFilter.Code = Admin_SexFilterDTO.Code;
            SexFilter.Name = Admin_SexFilterDTO.Name;

            List<Sex> Sexes = await SexService.List(SexFilter);
            List<Admin_SexDTO> Admin_SexDTOs = Sexes
                .Select(x => new Admin_SexDTO(x)).ToList();
            return Admin_SexDTOs;
        }
        [Route(AdminRoute.FilterListStatus), HttpPost]
        public async Task<List<Admin_StatusDTO>> FilterListStatus([FromBody] Admin_StatusFilterDTO Admin_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Admin_StatusFilterDTO.Id;
            StatusFilter.Code = Admin_StatusFilterDTO.Code;
            StatusFilter.Name = Admin_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Admin_StatusDTO> Admin_StatusDTOs = Statuses
                .Select(x => new Admin_StatusDTO(x)).ToList();
            return Admin_StatusDTOs;
        }
        [Route(AdminRoute.SingleListSex), HttpPost]
        public async Task<List<Admin_SexDTO>> SingleListSex([FromBody] Admin_SexFilterDTO Admin_SexFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SexFilter SexFilter = new SexFilter();
            SexFilter.Skip = 0;
            SexFilter.Take = 20;
            SexFilter.OrderBy = SexOrder.Id;
            SexFilter.OrderType = OrderType.ASC;
            SexFilter.Selects = SexSelect.ALL;
            SexFilter.Id = Admin_SexFilterDTO.Id;
            SexFilter.Code = Admin_SexFilterDTO.Code;
            SexFilter.Name = Admin_SexFilterDTO.Name;

            List<Sex> Sexes = await SexService.List(SexFilter);
            List<Admin_SexDTO> Admin_SexDTOs = Sexes
                .Select(x => new Admin_SexDTO(x)).ToList();
            return Admin_SexDTOs;
        }
        [Route(AdminRoute.SingleListStatus), HttpPost]
        public async Task<List<Admin_StatusDTO>> SingleListStatus([FromBody] Admin_StatusFilterDTO Admin_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Admin_StatusFilterDTO.Id;
            StatusFilter.Code = Admin_StatusFilterDTO.Code;
            StatusFilter.Name = Admin_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Admin_StatusDTO> Admin_StatusDTOs = Statuses
                .Select(x => new Admin_StatusDTO(x)).ToList();
            return Admin_StatusDTOs;
        }
    }
}

