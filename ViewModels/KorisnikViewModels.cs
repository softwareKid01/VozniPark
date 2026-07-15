using System.ComponentModel.DataAnnotations;

namespace VozniPark.ViewModels
{
    public class KorisnikCreateViewModel
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ime je obavezno.")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Odaberite ulogu.")]
        public string Uloga { get; set; } = string.Empty;
    }

    public class KorisnikEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ime je obavezno.")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Odaberite ulogu.")]
        public string Uloga { get; set; } = string.Empty;
    }
}
