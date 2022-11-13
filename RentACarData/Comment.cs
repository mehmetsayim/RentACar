using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RentACarData
{
    public class Comment : EntityBase
    {
        public Guid CarId { get; set; }

        public Guid ApplicationUserId { get; set; }

        public string Text { get; set; }

        public int Rate { get; set; }

        public virtual Car? Car { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }

    public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {

        }
    }
}
