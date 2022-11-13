using RentACarData;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RentACarWeb.Models
{
    public class RegisterViewModel
    {
        [Display(Name = "E-Posta")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi yazınız!")]
        public string UserName { get; set; }


        [Display(Name = "Ad Soyad")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }

        [Display(Name = "Parola")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Password { get; set; }

        [Display(Name = "Parola Tekrar")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [Compare("Password", ErrorMessage = "{0} alanı ile {1} alanı aynı olmalıdır!")]
        public string PasswordVerify { get; set; }

        [Display(Name = "Cinsiyet")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public Genders Gender { get; set; }

        [Display(Name = "Doğum T.")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public DateTime BirthDate { get; set; }
    }
}
