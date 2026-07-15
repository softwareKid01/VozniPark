using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VozniPark.Models
{
    public class Servis
    {
        public int Id { get; set; }

        [Required]
        public int VoziloId { get; set; }

        [Required(ErrorMessage = "Datum servisa je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum Servisa")]
        public DateTime DatumServisa { get; set; }

        [Required(ErrorMessage = "Tip servisa je obavezan.")]
        [StringLength(100)]
        [Display(Name = "Tip Servisa")]
        public string TipServisa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Opis radova je obavezan.")]
        [StringLength(1000)]
        [Display(Name = "Opis Radova")]
        public string OpisRadova { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Servisna Kuća")]
        public string? ServisnaKuca { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Unesite validnu cijenu.")]
        public decimal? Cijena { get; set; }

        [StringLength(500)]
        public string? Napomena { get; set; }

        
        [ForeignKey("VoziloId")]
        public Vozilo? Vozilo { get; set; }
    }
}
