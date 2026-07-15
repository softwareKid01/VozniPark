# VozniPark - Sistem za upravljanje voznim parkom

Sistem "VozniPark" je web aplikacija razvijena za profesionalno upravljanje vozilima, praćenje servisnih intervala i kontrolu isteka registracija.

## Korištene tehnologije i arhitektura

Aplikacija je izgrađena prateći **MVC (Model-View-Controller)** arhitekturu, čime je osigurano jasno odvajanje poslovne logike, korisničkog interfejsa i interakcije sa podacima.

*   **Backend:** C# / ASP.NET Core MVC (.NET)
*   **Baza podataka:** Microsoft SQL Server
*   **Frontend:** Razor Pages, HTML5, CSS3, Bootstrap 5
*   **Autentifikacija i Autorizacija:** ASP.NET Core Identity
*   **Kontejnerizacija:** Docker i Docker Compose

## Funkcionalnosti i uputstvo za korištenje

Aplikacija nudi jednostavno praćenje i upravljanje stanjem voznog parka kroz sljedeće module:
*   **Dashboard (Kontrolna tabla):** Dinamički prikaz kritičnih upozorenja. Sistem automatski prepoznaje vozila kojima registracija ističe za manje od 30 dana i vozila kojima je prošlo više od 1 godine od posljednjeg servisa.
*   **Upravljanje vozilima:** Dodavanje, uređivanje, detaljni pregled i brisanje vozila iz sistema.
*   **Sistem obavještenja:** Globalne notifikacije u navigaciji koje upozoravaju na hitne akcije i isticanje rokova.

## Testni korisnički nalozi

Sistem implementira kontrolu pristupa baziranu na ulogama (Role-Based Access Control). Baza se pri prvom pokretanju automatski popunjava inicijalnim (seed) podacima. Za testiranje aplikacije, možete koristiti sljedeće predefinisane naloge:

*   **Administrator** (Pun pristup: upravljanje vozilima, servisima i administracija korisnika)
    *   Email: `admin@voznipark.ba`
    *   Lozinka: `Admin123!`

*   **Korisnici (USER)** (Operativni pristup: dodavanje i ažuriranje vozila i servisa)
    *   Sistem automatski kreira 10 korisničkih naloga. Format emaila je `ime.prezime@voznipark.ba`.
    *   *Primjeri dostupnih naloga:* `emir.hodzic@voznipark.ba`, `lejla.kovacevic@voznipark.ba`, `amar.delic@voznipark.ba`
    *   Lozinka za sve korisnike: `Korisnik123!`

*   **Posmatrači (READONLY)** (Samo pregled (Read-only): uvid u Dashboard i listu vozila)
    *   Sistem automatski kreira 20 posmatračkih naloga. Format emaila je `ime.prezime@voznipark.ba`.
    *   *Primjeri dostupnih naloga:* `damir.avdic@voznipark.ba`, `nejra.alic@voznipark.ba`, `dino.smajic@voznipark.ba`
    *   Lozinka za sve posmatrače: `Posmatrac123!`

## Instalacija i pokretanje aplikacije (Docker)

Aplikacija i baza podataka su u potpunosti kontejnerizovane. Da biste pokrenuli sistem, **nije potrebno** instalirati SQL Server, .NET SDK ili bilo koje druge lokalne ovisnosti osim Dockera.

### Preduslovi
*   Instaliran i pokrenut [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Koraci za pokretanje

1. Klonirajte repozitorij na vašu lokalnu mašinu:
   ```bash
   git clone https://github.com/TarikDedic/VozniPark.git
   cd VozniPark
   ```

2. Pokrenite aplikaciju i bazu podataka koristeći Docker Compose komandu:
   ```bash
   docker-compose up -d --build
   ```

3. Aplikacija će nakon nekoliko sekundi biti dostupna u vašem pretraživaču na adresi:
   `http://localhost:8080` *(ili na portu koji je definisan u postavkama)*

Docker Compose će automatski podići SQL Server instancu, pokrenuti web aplikaciju, primijeniti sve potrebne migracije i popuniti bazu inicijalnim (seed) podacima kako bi aplikacija bila odmah spremna za testiranje.