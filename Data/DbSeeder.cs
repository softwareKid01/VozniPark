using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VozniPark.Models;
using VozniPark.Data;

namespace VozniPark.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Korisnik>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. KREIRANJE ULOGA
            string[] uloge = { "ADMIN", "USER", "READONLY" };
            foreach (var uloga in uloge)
            {
                if (!await roleManager.RoleExistsAsync(uloga))
                {
                    await roleManager.CreateAsync(new IdentityRole(uloga));
                }
            }

            // 2. KREIRANJE ADMINA (1 Admin)
            string adminEmail = "admin@voznipark.ba";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var noviAdmin = new Korisnik { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, Ime = "Glavni", Prezime = "Admin" };
                var result = await userManager.CreateAsync(noviAdmin, "Admin123!");
                if (result.Succeeded) await userManager.AddToRoleAsync(noviAdmin, "ADMIN");
            }

            // 3. KREIRANJE 10 KORISNIKA (USER)
            var korisniciPodaci = new (string Ime, string Prezime)[]
            {
                ("Emir", "Hodzic"), ("Lejla", "Kovacevic"), ("Amar", "Delic"),
                ("Amina", "Spahic"), ("Kenan", "Basic"), ("Emina", "Karic"),
                ("Haris", "Halilovic"), ("Selma", "Muminovic"), ("Adnan", "Cengic"),
                ("Jasmina", "Hadzic")
            };

            for (int i = 0; i < korisniciPodaci.Length; i++)
            {
                // Automatsko kreiranje emaila iz imena i prezimena (npr. emir.hodzic@voznipark.ba)
                string email = $"{korisniciPodaci[i].Ime.ToLower()}.{korisniciPodaci[i].Prezime.ToLower()}@voznipark.ba";

                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new Korisnik { UserName = email, Email = email, EmailConfirmed = true, Ime = korisniciPodaci[i].Ime, Prezime = korisniciPodaci[i].Prezime };
                    var result = await userManager.CreateAsync(user, "Korisnik123!");
                    if (result.Succeeded) await userManager.AddToRoleAsync(user, "USER");
                }
            }

            // 4. KREIRANJE 20 POSMATRACA (READONLY)
            var posmatraciPodaci = new (string Ime, string Prezime)[]
            {
                ("Damir", "Avdic"), ("Nejra", "Alic"), ("Dino", "Smajic"),
                ("Merima", "Omerovic"), ("Mirza", "Begic"), ("Aida", "Huseinovic"),
                ("Denis", "Dizdarevic"), ("Amra", "Mujic"), ("Nermin", "Pasic"),
                ("Dalila", "Brkic"), ("Vedad", "Tomic"), ("Ajla", "Mesic"),
                ("Faris", "Kurtovic"), ("Ilhana", "Zukic"), ("Eldar", "Catic"),
                ("Naida", "Salihovic"), ("Armin", "Kulenovic"), ("Alisa", "Topalovic"),
                ("Samir", "Ramic"), ("Medina", "Imamovic")
            };

            for (int i = 0; i < posmatraciPodaci.Length; i++)
            {
                // Automatsko kreiranje emaila za posmatrače
                string email = $"{posmatraciPodaci[i].Ime.ToLower()}.{posmatraciPodaci[i].Prezime.ToLower()}@voznipark.ba";

                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new Korisnik { UserName = email, Email = email, EmailConfirmed = true, Ime = posmatraciPodaci[i].Ime, Prezime = posmatraciPodaci[i].Prezime };
                    var result = await userManager.CreateAsync(user, "Posmatrac123!");
                    if (result.Succeeded) await userManager.AddToRoleAsync(user, "READONLY");
                }
            }

            // 5. KREIRANJE SVIH VOZILA I SERVISA IZ BAZE
            if (!context.Vozila.Any())
            {
                var vozila = new List<Vozilo>
                {
                    new Vozilo { RegistracijskaOznaka = "SA-100-AA", Marka = "Audi", Model = "A4", GodinaProizvodnje = 2022, DatumRegistracije = new DateTime(2025, 9, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 2, 10), TipServisa = "Mali servis", OpisRadova = "Zamjena ulja i filtera", ServisnaKuca = "Auto Centar", Cijena = 250.00m, Napomena = "Sve OK." },
                            new Servis { DatumServisa = new DateTime(2026, 5, 15), TipServisa = "Provjera", OpisRadova = "Redovan pregled", ServisnaKuca = "Auto Centar", Cijena = 50.00m, Napomena = "Ispravno." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 20), TipServisa = "Klima", OpisRadova = "Dopuna freona", ServisnaKuca = "Frigo servis", Cijena = 80.00m, Napomena = "Hladi odlično." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-101-BB", Marka = "BMW", Model = "X5", GodinaProizvodnje = 2023, DatumRegistracije = new DateTime(2025, 9, 12), Napomena = "Direktor",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 3, 1), TipServisa = "Mali servis", OpisRadova = "Zamjena filtera", ServisnaKuca = "BMW Servis", Cijena = 400.00m, Napomena = "Koristeni originalni dijelovi." },
                            new Servis { DatumServisa = new DateTime(2026, 4, 10), TipServisa = "Gume", OpisRadova = "Montaža ljetnih guma", ServisnaKuca = "Vulco", Cijena = 120.00m, Napomena = "Balansirano." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 5), TipServisa = "Kočnice", OpisRadova = "Zamjena diskova", ServisnaKuca = "BMW Servis", Cijena = 600.00m, Napomena = "Hitna zamjena." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-102-CC", Marka = "Mercedes", Model = "C-Class", GodinaProizvodnje = 2021, DatumRegistracije = new DateTime(2026, 7, 13), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 1, 20), TipServisa = "Veliki servis", OpisRadova = "Zupčasti remen", ServisnaKuca = "MERC Servis", Cijena = 850.00m, Napomena = "Garancija 2g." },
                            new Servis { DatumServisa = new DateTime(2026, 4, 1), TipServisa = "Tehnički", OpisRadova = "Redovni tehnički", ServisnaKuca = "Stanica MUP", Cijena = 100.00m, Napomena = "Bez primjedbi." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 15), TipServisa = "Trap", OpisRadova = "Reglaža trapa", ServisnaKuca = "Auto centar", Cijena = 70.00m, Napomena = "Stabilno." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-103-DD", Marka = "Toyota", Model = "RAV4", GodinaProizvodnje = 2020, DatumRegistracije = new DateTime(2026, 2, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 2, 15), TipServisa = "Mali servis", OpisRadova = "Zamjena ulja", ServisnaKuca = "Toyota Servis", Cijena = 280.00m, Napomena = "Sintetika." },
                            new Servis { DatumServisa = new DateTime(2026, 5, 10), TipServisa = "Sijalice", OpisRadova = "Zamjena prednjih", ServisnaKuca = "Toyota Servis", Cijena = 40.00m, Napomena = "LED." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 25), TipServisa = "Akumulator", OpisRadova = "Zamjena akumulatora", ServisnaKuca = "Auto Elektrika", Cijena = 160.00m, Napomena = "Novi." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-104-EE", Marka = "Kia", Model = "Sportage", GodinaProizvodnje = 2024, DatumRegistracije = new DateTime(2026, 2, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 1, 5), TipServisa = "Mali servis", OpisRadova = "Redovan servis", ServisnaKuca = "Kia Centar", Cijena = 220.00m, Napomena = "Pregledano." },
                            new Servis { DatumServisa = new DateTime(2026, 3, 20), TipServisa = "Gume", OpisRadova = "Balansiranje", ServisnaKuca = "Vulco", Cijena = 60.00m, Napomena = "Zimske." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 10), TipServisa = "Kočnice", OpisRadova = "Nove pločice", ServisnaKuca = "Kia Centar", Cijena = 180.00m, Napomena = "Odlično." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-105-FF", Marka = "Ford", Model = "Focus", GodinaProizvodnje = 2018, DatumRegistracije = new DateTime(2026, 7, 15), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 2, 20), TipServisa = "Mali servis", OpisRadova = "Zamjena filtera", ServisnaKuca = "Ford Servis", Cijena = 240.00m, Napomena = "Sve čisto." },
                            new Servis { DatumServisa = new DateTime(2026, 4, 5), TipServisa = "Klima", OpisRadova = "Čišćenje klime", ServisnaKuca = "Frigo servis", Cijena = 90.00m, Napomena = "Dezinfekcija urađena." },
                            new Servis { DatumServisa = new DateTime(2026, 7, 1), TipServisa = "Pregled", OpisRadova = "Dijagnostika", ServisnaKuca = "Ford Servis", Cijena = 50.00m, Napomena = "Bez grešaka." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-106-GG", Marka = "Hyundai", Model = "i30", GodinaProizvodnje = 2022, DatumRegistracije = new DateTime(2026, 4, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 3, 10), TipServisa = "Mali servis", OpisRadova = "Zamjena ulja", ServisnaKuca = "Hyundai Servis", Cijena = 210.00m, Napomena = "Standard." },
                            new Servis { DatumServisa = new DateTime(2026, 5, 20), TipServisa = "Sijalice", OpisRadova = "Zamjena zadnjih", ServisnaKuca = "Auto Elektrika", Cijena = 30.00m, Napomena = "Zamijenjeno." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 30), TipServisa = "Trap", OpisRadova = "Centriranje", ServisnaKuca = "Auto centar", Cijena = 70.00m, Napomena = "Sređeno." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-107-HH", Marka = "Opel", Model = "Astra", GodinaProizvodnje = 2021, DatumRegistracije = new DateTime(2026, 4, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 1, 15), TipServisa = "Mali servis", OpisRadova = "Redovan servis", ServisnaKuca = "Opel Servis", Cijena = 230.00m, Napomena = "Sve OK." },
                            new Servis { DatumServisa = new DateTime(2026, 4, 12), TipServisa = "Gume", OpisRadova = "Montaža guma", ServisnaKuca = "Vulco", Cijena = 80.00m, Napomena = "Ljetne." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 18), TipServisa = "Kočnice", OpisRadova = "Zamjena diskova", ServisnaKuca = "Opel Servis", Cijena = 350.00m, Napomena = "Nova garnitura." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-108-II", Marka = "Peugeot", Model = "3008", GodinaProizvodnje = 2023, DatumRegistracije = new DateTime(2026, 4, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 2, 10), TipServisa = "Veliki servis", OpisRadova = "Zupčasti set", ServisnaKuca = "Peugeot Servis", Cijena = 700.00m, Napomena = "Pouzdan servis." },
                            new Servis { DatumServisa = new DateTime(2026, 5, 5), TipServisa = "Mali servis", OpisRadova = "Zamjena ulja", ServisnaKuca = "Peugeot Servis", Cijena = 260.00m, Napomena = "Redovno." },
                            new Servis { DatumServisa = new DateTime(2026, 7, 5), TipServisa = "Tehnički", OpisRadova = "Redovni tehnički", ServisnaKuca = "Stanica MUP", Cijena = 100.00m, Napomena = "Sve ispravno." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-109-JJ", Marka = "Mazda", Model = "CX-5", GodinaProizvodnje = 2020, DatumRegistracije = new DateTime(2025, 8, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 3, 20), TipServisa = "Mali servis", OpisRadova = "Zamjena ulja", ServisnaKuca = "Mazda Servis", Cijena = 320.00m, Napomena = "Preporučeno." },
                            new Servis { DatumServisa = new DateTime(2026, 5, 15), TipServisa = "Klima", OpisRadova = "Dopuna freona", ServisnaKuca = "Frigo servis", Cijena = 90.00m, Napomena = "Hladi." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 25), TipServisa = "Pregled", OpisRadova = "Provjera motora", ServisnaKuca = "Mazda Servis", Cijena = 50.00m, Napomena = "Rad miran." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-110-KK", Marka = "Volvo", Model = "XC60", GodinaProizvodnje = 2022, DatumRegistracije = new DateTime(2025, 8, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 1, 10), TipServisa = "Mali servis", OpisRadova = "Redovan servis", ServisnaKuca = "Volvo Servis", Cijena = 500.00m, Napomena = "Volvo standard." },
                            new Servis { DatumServisa = new DateTime(2026, 4, 20), TipServisa = "Kočnice", OpisRadova = "Nove pločice", ServisnaKuca = "Volvo Servis", Cijena = 400.00m, Napomena = "Sigurno." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 30), TipServisa = "Gume", OpisRadova = "Balansiranje", ServisnaKuca = "Vulco", Cijena = 100.00m, Napomena = "Sve OK." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-112-MM", Marka = "Dacia", Model = "Duster", GodinaProizvodnje = 2023, DatumRegistracije = new DateTime(2026, 5, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 2, 15), TipServisa = "Mali servis", OpisRadova = "Zamjena ulja", ServisnaKuca = "Fiat Servis", Cijena = 190.00m, Napomena = "Povoljno." },
                            new Servis { DatumServisa = new DateTime(2026, 5, 10), TipServisa = "Sijalice", OpisRadova = "Zamjena sijalica", ServisnaKuca = "Auto Elektrika", Cijena = 40.00m, Napomena = "Sređeno." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 25), TipServisa = "Trap", OpisRadova = "Reglaža", ServisnaKuca = "Auto centar", Cijena = 70.00m, Napomena = "Urađeno." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-113-NN", Marka = "Nissan", Model = "Qashqai", GodinaProizvodnje = 2021, DatumRegistracije = new DateTime(2025, 12, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 3, 5), TipServisa = "Mali servis", OpisRadova = "Zamjena filtera", ServisnaKuca = "Dacia Servis", Cijena = 180.00m, Napomena = "Sve OK." },
                            new Servis { DatumServisa = new DateTime(2026, 5, 25), TipServisa = "Pregled", OpisRadova = "Redovan servis", ServisnaKuca = "Dacia Servis", Cijena = 60.00m, Napomena = "Spreman." },
                            new Servis { DatumServisa = new DateTime(2026, 7, 2), TipServisa = "Klima", OpisRadova = "Čišćenje", ServisnaKuca = "Frigo servis", Cijena = 80.00m, Napomena = "Svježina." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-114-OO", Marka = "Citroen", Model = "C4", GodinaProizvodnje = 2022, DatumRegistracije = new DateTime(2026, 6, 12), Napomena = "Službeno",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 1, 25), TipServisa = "Mali servis", OpisRadova = "Zamjena ulja", ServisnaKuca = "Nissan Servis", Cijena = 270.00m, Napomena = "Standard." },
                            new Servis { DatumServisa = new DateTime(2026, 4, 15), TipServisa = "Kočnice", OpisRadova = "Zamjena pločica", ServisnaKuca = "Nissan Servis", Cijena = 200.00m, Napomena = "Nove." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 15), TipServisa = "Gume", OpisRadova = "Montaža guma", ServisnaKuca = "Vulco", Cijena = 90.00m, Napomena = "Ok." }
                        }
                    },
                    new Vozilo { RegistracijskaOznaka = "SA-188-FO", Marka = "Nissan", Model = "Sentar", GodinaProizvodnje = 2017, DatumRegistracije = new DateTime(2026, 3, 11), Napomena = "Brisaci",
                        Servisi = new List<Servis> {
                            new Servis { DatumServisa = new DateTime(2026, 2, 10), TipServisa = "Mali servis", OpisRadova = "Redovan servis", ServisnaKuca = "Citroen Servis", Cijena = 230.00m, Napomena = "Zadovoljan." },
                            new Servis { DatumServisa = new DateTime(2026, 5, 1), TipServisa = "Tehnički", OpisRadova = "Redovni tehnički", ServisnaKuca = "Stanica MUP", Cijena = 100.00m, Napomena = "Prošao." },
                            new Servis { DatumServisa = new DateTime(2026, 6, 20), TipServisa = "Trap", OpisRadova = "Centriranje", ServisnaKuca = "Auto centar", Cijena = 70.00m, Napomena = "Stabilan." }
                        }
                    }
                };

                await context.Vozila.AddRangeAsync(vozila);
                await context.SaveChangesAsync();
            }
        }
    }
}