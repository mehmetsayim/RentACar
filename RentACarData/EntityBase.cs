using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RentACarData
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }

        [Display(Name = "Kayıt T.")]
        public DateTime DateCreated { get; set; }

        //Soft Delete
        [Display(Name = "Durum")]
        public bool Enabled { get; set; }
    }
}
