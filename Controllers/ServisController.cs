using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VozniPark.Models;
using VozniPark.Services;
namespace VozniPark.Controllers
{
    [Authorize]
    public class ServisController : Controller
    {
        private readonly IServisService _servisService;
        public ServisController(IServisService servisService)
        {
            _servisService = servisService;
        }

        
        public async Task<IActionResult> IndexByVozilo(int voziloId)
        {
            var servisi = await _servisService.GetServisiByVoziloIdAsync(voziloId);

            
            ViewBag.VoziloId = voziloId;

            return View("Index", servisi);
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var servis = await _servisService.GetServisByIdAsync(id);
            if (servis == null)
            {
                return NotFound();
            }
            return View(servis); 
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Servis servis)
        {
            
            ModelState.Remove("Vozilo");

            if (ModelState.IsValid)
            {
                await _servisService.UpdateServisAsync(servis);

                
                return RedirectToAction("Details", "Vozilo", new { id = servis.VoziloId });
            }

           
            return View(servis);
        }



    }
}
