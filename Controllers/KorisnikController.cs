using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VozniPark.Models;
using VozniPark.ViewModels;
using VozniPark.ViewModels;

namespace VozniPark.Controllers
{
    [Authorize(Roles = "ADMIN")] 
    public class KorisnikController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public KorisnikController(UserManager<Korisnik> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

       
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var query = _userManager.Users;
            var totalKorisnika = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalKorisnika / (double)pageSize);

            var paginiraniKorisnici = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginiraniKorisnici);
        }

       
        [HttpGet]
        public IActionResult Create()
        {
            PripremiUlogeZaDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KorisnikCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Korisnik
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Ime = model.Ime,
                    Prezime = model.Prezime,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Lozinka);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Uloga);
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            PripremiUlogeZaDropdown();
            return View(model);
        }

       
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "USER"; // Default fallback

            var model = new KorisnikEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Ime = user.Ime,
                Prezime = user.Prezime,
                Uloga = userRole
            };

            PripremiUlogeZaDropdown();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KorisnikEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                // Ažuriranje ličnih podataka
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Ime = model.Ime;
                user.Prezime = model.Prezime;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Ažuriranje uloge
                    var trenutneUloge = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, trenutneUloge);
                    await _userManager.AddToRoleAsync(user, model.Uloga);

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            PripremiUlogeZaDropdown();
            return View(model);
        }

       
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user); // Prikazuje formu za potvrdu brisanja
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        
        private void PripremiUlogeZaDropdown()
        {
            ViewBag.Uloge = new SelectList(new[] { "ADMIN", "USER", "READONLY" });
        }
    }
}