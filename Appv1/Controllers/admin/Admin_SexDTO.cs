using Appv1.Entities;
using Appv1.Common;

namespace Appv1.Controllers.admin
{
    public class Admin_SexDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public Admin_SexDTO() { }
        public Admin_SexDTO(Sex Sex)
        {

            this.Id = Sex.Id;

            this.Code = Sex.Code;

            this.Name = Sex.Name;

        }
    }

    public class Admin_SexFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public SexOrder OrderBy { get; set; }
    }
}