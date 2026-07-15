using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VozniPark.Data; 

namespace VozniPark.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NotificationViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var upozorenja = new List<string>();

            
            var vozila = await _context.Vozila.Include(v => v.Servisi).ToListAsync();

            foreach (var vozilo in vozila)
            {
                // 1. Provjera registracije (< 30 dana)
                var datumIsteka = vozilo.DatumRegistracije.AddYears(1);
                var daniDoIsteka = (datumIsteka - DateTime.Now).TotalDays;

                if (daniDoIsteka <= 30 && daniDoIsteka > 0)
                {
                    upozorenja.Add($"Registracija ističe za {Math.Ceiling(daniDoIsteka)} dana: {vozilo.RegistracijskaOznaka}");
                }
                else if (daniDoIsteka <= 0)
                {
                    upozorenja.Add($"ISTEKLA REGISTRACIJA: {vozilo.RegistracijskaOznaka}");
                }

                // 2. Provjera servisa (> 365 dana ili nema servisa)
                var zadnjiServis = vozilo.Servisi?.OrderByDescending(s => s.DatumServisa).FirstOrDefault();
                if (zadnjiServis == null)
                {
                    upozorenja.Add($"Nema evidentiranih servisa: {vozilo.RegistracijskaOznaka}");
                }
                else if ((DateTime.Now - zadnjiServis.DatumServisa).TotalDays > 365)
                {
                    upozorenja.Add($"Hitan servis (prošlo > godinu dana): {vozilo.RegistracijskaOznaka}");
                }
            }

            // Šaljemo listu upozorenja u HTML
            return View(upozorenja);
        }
    }
}