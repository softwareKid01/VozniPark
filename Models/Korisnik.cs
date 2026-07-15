using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VozniPark.Models
{
    public class Korisnik : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string Ime { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Prezime { get; set; } = string.Empty;
    }
}
