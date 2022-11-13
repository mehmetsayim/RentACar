using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace RentACarData
{
    public class MainMenu : EntityBase
    {

        [Display(Name = "Ad")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    }

    public class MainMenuEntityTypeConfiguration : IEntityTypeConfiguration<MainMenu>
    {
        public void Configure(EntityTypeBuilder<MainMenu> builder)
        {
            builder
                .Property(p => p.Name)
                .HasMaxLength(50);

            builder
                .HasIndex(p => new { p.Name })
                .IsUnique();

            builder
                .HasMany(p => p.Categories)
                .WithOne(p => p.MainMenu)
                .HasForeignKey(P => P.MainMenuId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
