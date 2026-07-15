using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VozniPark.Models;
using VozniPark.Services;
using ClosedXML.Excel;
using System.IO;

namespace VozniPark.Controllers
{
    [Authorize]
    public class VoziloController : Controller
    {
        private readonly IVoziloService _voziloService;

        public VoziloController(IVoziloService voziloService)
        {
            _voziloService = voziloService;
        }

        
        public async Task<IActionResult> Index(string searchReg, int? searchGodina, DateTime? searchDatum)
        {
            ViewData["FilterReg"] = searchReg;
            ViewData["FilterGodina"] = searchGodina;
            if (searchDatum.HasValue)
            {
                ViewData["FilterDatum"] = searchDatum.Value.ToString("yyyy-MM-dd");
            }

            var vozila = await _voziloService.GetAllVozilaAsync();

            if (!string.IsNullOrEmpty(searchReg))
            {
                vozila = vozila.Where(v => v.RegistracijskaOznaka.Contains(searchReg, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (searchGodina.HasValue)
            {
                vozila = vozila.Where(v => v.GodinaProizvodnje == searchGodina.Value).ToList();
            }

            if (searchDatum.HasValue)
            {
                vozila = vozila.Where(v => v.DatumRegistracije.Date >= searchDatum.Value.Date).ToList();
            }

            return View(vozila);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var vozilo = await _voziloService.GetVoziloDetailsAsync(id); 
            if (vozilo == null)
            {
                return NotFound();
            }
            return View(vozilo);
        }

        

        
        [Authorize(Roles = "USER")]
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> Create([Bind("Id,RegistracijskaOznaka,Marka,Model,GodinaProizvodnje,DatumRegistracije,Napomena")] Vozilo vozilo)
        {
            if (ModelState.IsValid)
            {
                await _voziloService.AddVoziloAsync(vozilo);
                return RedirectToAction(nameof(Index));
            }
            return View(vozilo);
        }

       
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> Edit(int id)
        {
            var vozilo = await _voziloService.GetVoziloByIdAsync(id);
            if (vozilo == null)
            {
                return NotFound();
            }
            return View(vozilo);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RegistracijskaOznaka,Marka,Model,GodinaProizvodnje,DatumRegistracije,Napomena")] Vozilo vozilo)
        {
            if (id != vozilo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _voziloService.UpdateVoziloAsync(vozilo);
                return RedirectToAction(nameof(Index));
            }
            return View(vozilo);
        }

       
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> Delete(int id)
        {
            var vozilo = await _voziloService.GetVoziloByIdAsync(id);
            if (vozilo == null)
            {
                return NotFound();
            }
            return View(vozilo);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _voziloService.DeleteVoziloAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Dashboard()
        {
            
            var kriticneRegistracije = await _voziloService.GetKriticneRegistracijeAsync();
            var kriticniServisi = await _voziloService.GetKriticniServisiAsync();

            
            ViewBag.KriticneRegistracije = kriticneRegistracije;
            ViewBag.KriticniServisi = kriticniServisi;

            
            ViewBag.BrojKriticnihRegistracija = kriticneRegistracije.Count();
            ViewBag.BrojKriticnihServisa = kriticniServisi.Count();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            
            var vozila = await _voziloService.GetAllVozilaAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Vozila i Statusi");
                var currentRow = 1;

                
                worksheet.Cell(currentRow, 1).Value = "Marka i Model";
                worksheet.Cell(currentRow, 2).Value = "Registracijska Oznaka";
                worksheet.Cell(currentRow, 3).Value = "Datum Isteka Reg.";
                worksheet.Cell(currentRow, 4).Value = "STATUS REGISTRACIJE";
                worksheet.Cell(currentRow, 5).Value = "Zadnji Servis (Datum)";
                worksheet.Cell(currentRow, 6).Value = "Zadnji Servis (Tip)";
                worksheet.Cell(currentRow, 7).Value = "STATUS SERVISA";

                
                var headerRange = worksheet.Range("A1:G1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 102, 204); 
                headerRange.Style.Font.FontColor = XLColor.White;

                
                foreach (var vozilo in vozila)
                {
                    currentRow++;

                    
                    DateTime datumIsteka = vozilo.DatumRegistracije.AddYears(1);
                    double daniDoIsteka = (datumIsteka - DateTime.Now).TotalDays;
                    bool kriticnaReg = daniDoIsteka <= 30;

                  
                    var zadnjiServis = vozilo.Servisi?.OrderByDescending(s => s.DatumServisa).FirstOrDefault();
                    
                    bool kriticanServis = zadnjiServis == null || (DateTime.Now - zadnjiServis.DatumServisa).TotalDays > 365;

                   
                    worksheet.Cell(currentRow, 1).Value = $"{vozilo.Marka} {vozilo.Model}";
                    worksheet.Cell(currentRow, 2).Value = vozilo.RegistracijskaOznaka;
                    worksheet.Cell(currentRow, 3).Value = datumIsteka.ToString("dd.MM.yyyy");

                    
                    var celijaReg = worksheet.Cell(currentRow, 4);
                    if (kriticnaReg)
                    {
                        celijaReg.Value = daniDoIsteka < 0 ? "ISTEKLO!" : $"Kritično ({Math.Ceiling(daniDoIsteka)} dana)";
                        celijaReg.Style.Fill.BackgroundColor = XLColor.LightCoral; 
                        celijaReg.Style.Font.Bold = true;
                    }
                    else
                    {
                        celijaReg.Value = "OK";
                        celijaReg.Style.Fill.BackgroundColor = XLColor.LightGreen; 
                    }

                    
                    worksheet.Cell(currentRow, 5).Value = zadnjiServis != null ? zadnjiServis.DatumServisa.ToString("dd.MM.yyyy") : "Nema podataka";
                    worksheet.Cell(currentRow, 6).Value = zadnjiServis != null ? zadnjiServis.TipServisa : "-";

                    
                    var celijaServis = worksheet.Cell(currentRow, 7);
                    if (kriticanServis)
                    {
                        celijaServis.Value = "KRITIČNO (Potreban servis)";
                        celijaServis.Style.Fill.BackgroundColor = XLColor.LightCoral; 
                        celijaServis.Style.Font.Bold = true;
                    }
                    else
                    {
                        celijaServis.Value = "OK (Uredno)";
                        celijaServis.Style.Fill.BackgroundColor = XLColor.LightGreen; 
                    }
                }

               
                worksheet.Columns().AdjustToContents();

                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Izvjestaj_Status_Vozila.xlsx"
                    );
                }
            }
        }
    }
}
