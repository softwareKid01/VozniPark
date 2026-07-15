using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // OVO JE NOVO I JAKO BITNO ZA [NotMapped]

namespace VozniPark.Models
{
    public class Vozilo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Registracijska oznaka je obavezna.")]
        [StringLength(20)]
        [Display(Name = "Registracijska Oznaka")]
        public string RegistracijskaOznaka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Marka vozila je obavezna.")]
        [StringLength(50)]
        public string Marka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model vozila je obavezan.")]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Godina proizvodnje je obavezna.")]
        [Range(1900, 2100, ErrorMessage = "Unesite validnu godinu.")]
        [Display(Name = "Godina Proizvodnje")]
        public int GodinaProizvodnje { get; set; }

        [Required(ErrorMessage = "Datum registracije je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum Registracije")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DatumRegistracije { get; set; }

        [StringLength(500)]
        public string? Napomena { get; set; }

        
        public ICollection<Servis> Servisi { get; set; } = new List<Servis>();


       
        [NotMapped]
        public DateTime DatumIstekaRegistracije => DatumRegistracije.AddYears(1);

        [NotMapped]
        public bool RegistracijaIstekla => DatumIstekaRegistracije.Date < DateTime.Now.Date;

        [NotMapped]
        public bool RegistracijaIsticeUskoro =>
            !RegistracijaIstekla && DatumIstekaRegistracije.Date <= DateTime.Now.AddDays(30).Date; // Ističe u narednih 30 dana

        
        [NotMapped]
        public DateTime? KrajnjiRokZaServis
        {
            get
            {
                if (Servisi == null || !Servisi.Any())
                    return DatumRegistracije.AddYears(1); 

               
                var zadnjiServis = Servisi.OrderByDescending(s => s.DatumServisa).First();
                return zadnjiServis.DatumServisa.AddYears(1); 
            }
        }

        [NotMapped]
        public bool ServisIstekao => KrajnjiRokZaServis.HasValue && KrajnjiRokZaServis.Value.Date < DateTime.Now.Date;

        [NotMapped]
        public bool ServisIsticeUskoro =>
            !ServisIstekao && KrajnjiRokZaServis.HasValue &&
            KrajnjiRokZaServis.Value.Date <= DateTime.Now.AddDays(30).Date; 
    }
}