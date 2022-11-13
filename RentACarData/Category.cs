using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RentACarData
{
    public class Category : EntityBase
    {
        [Display(Name = "Reyon")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public Guid MainMenuId { get; set; }

        [Display(Name = "Ad")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public virtual MainMenu? MainMenu { get; set; }

        public virtual ICollection<Car> Cars { get; set; } = new HashSet<Car>();
    }

    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder
                .Property(p => p.Name)
                .HasMaxLength(50);

            builder
                .HasIndex(p => new { p.Name, p.MainMenuId })
                .IsUnique();

        }
    }
}
