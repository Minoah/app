using System;
using System.Collections.Generic;

#nullable disable

namespace Appv1.Models
{
    public partial class CommentDAO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long AppUserId { get; set; }
        public long ProductId { get; set; }
        public long? Star { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual ProductDAO Product { get; set; }
        public virtual StatusDAO Status { get; set; }
    }
}
