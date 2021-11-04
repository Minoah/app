using System.Collections.Generic;
using Appv1.Common;

namespace Appv1.Common
{
    public interface ICurrentContext : IServiceScoped
    {
        long UserId { get; set; }
        string UserName { get; set; }
        int TimeZone { get; set; }
        string Token { get; set; }
        decimal? Latitude { get; set; }
        decimal? Longitude { get; set; }
        //Dictionary<long, List<FilterPermissionDefinition>> Filters { get; set; }
    }
    public class CurrentContext : ICurrentContext
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int TimeZone { get; set; }
        public string Language { get; set; } = "vi";
        public string Token { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        //public Dictionary<long, List<FilterPermissionDefinition>> Filters { get; set; }
    }
}
