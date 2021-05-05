using AutoPortal.Misc;
using AutoPortal.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace AutoPortal.Controllers
{
    [Authorize(Roles = OvlastiKorisnik.Administrator)]
    public class KorisnikController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();

        // GET: Korisnik
        public ActionResult Popis()
        {
            var listaKorisnika = bazaPodataka.PopisKorisnika.OrderBy(x => x.SifraOvlasti).ThenBy(x => x.Prezime).ToList();
            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;
            return View(listaKorisnika);
        }

        public ActionResult PretrazivanjePartial(string KorisnickoIme, string Email, string Ime, string Prezime, string Naziv, int? page, string sort)
        {
            var korisnik = bazaPodataka.PopisKorisnika.ToList();
            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            ViewBag.Sortiranje = sort;
            ViewBag.KorisnickoImeSort = string.IsNullOrEmpty(sort) ? "kori_desc" : "";
            ViewBag.EmailSort = sort == "Email" ? "Email_desc" : "Email";
            ViewBag.ImeSort = sort == "Ime" ? "Ime_desc" : "Ime";
            ViewBag.PrezimeSort = sort == "Prezime" ? "Prezime_desc" : "Prezime";

            ViewBag.KorisnickoIme = KorisnickoIme;
            ViewBag.Email = Email;
            ViewBag.Ime = Ime;
            ViewBag.Prezime = Prezime;
            ViewBag.Naziv = Naziv;

            //filtriranje
            if (!String.IsNullOrWhiteSpace(KorisnickoIme))
                korisnik = korisnik.Where(x => x.KorisnickoIme.ToUpper().Contains(KorisnickoIme.ToUpper())).ToList();

            if (!String.IsNullOrWhiteSpace(Email))
                korisnik = korisnik.Where(x => x.Email.ToUpper().Contains(Email.ToUpper())).ToList();

            if (!String.IsNullOrWhiteSpace(Ime))
                korisnik = korisnik.Where(x => x.Ime.ToUpper().Contains(Ime.ToUpper())).ToList();
            
            if (!String.IsNullOrWhiteSpace(Prezime))
                korisnik = korisnik.Where(x => x.Prezime.ToUpper().Contains(Prezime.ToUpper())).ToList();

            if (!String.IsNullOrWhiteSpace(Naziv))
                korisnik = korisnik.Where(x => x.Ovlast.Naziv.ToUpper().Contains(Naziv.ToUpper())).ToList();

            switch (sort)
            {
                case "kori_desc":
                    korisnik = korisnik.OrderByDescending(x => x.KorisnickoIme).ToList();
                    break;
                case "Email":
                    korisnik = korisnik.OrderBy(x => x.Email).ToList();
                    break;
                case "Email_desc":
                    korisnik = korisnik.OrderByDescending(x => x.Email).ToList();
                    break;
                case "Ime":
                    korisnik = korisnik.OrderBy(x => x.Ime).ToList();
                    break;
                case "Ime_desc":
                    korisnik = korisnik.OrderByDescending(x => x.Ime).ToList();
                    break;
                case "Prezime":
                    korisnik = korisnik.OrderBy(x => x.Prezime).ToList();
                    break;
                case "Prezime_desc":
                    korisnik = korisnik.OrderByDescending(x => x.Prezime).ToList();
                    break;
                default:
                    korisnik = korisnik.OrderBy(x => x.KorisnickoIme).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            return PartialView("_PartialPretrazivanje", korisnik.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Registracija()
        {
            Korisnik model = new Korisnik();

            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Registracija(Korisnik model)
        {
            if (!String.IsNullOrWhiteSpace(model.KorisnickoIme))
            {
                var korImeZauzeto = bazaPodataka.PopisKorisnika.Any(x => x.KorisnickoIme == model.KorisnickoIme);
                if (korImeZauzeto)
                {
                    ModelState.AddModelError("KorisnickoIme", "Korisničko ime je već zauzeto");
                }
            }
            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var emailZauzet = bazaPodataka.PopisKorisnika.Any(x => x.Email == model.Email);
                if (emailZauzet)
                {
                    ModelState.AddModelError("Email", "Email je već zauzet");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var hashLozinke = Misc.PasswordHelper.HashPassword(model.LozinkaUnos);
                    model.Salt = hashLozinke.Item1;
                    model.Lozinka = hashLozinke.Item2;

                    bazaPodataka.PopisKorisnika.Add(model);
                    bazaPodataka.SaveChanges();
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Došlo je do greške!!");
                }

                return RedirectToAction("Index", "Home");
            }

            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            return View(model);
        }

        [HttpGet]
        [OverrideAuthorization]
        [Authorize]
        public ActionResult Azuriraj(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == id);
            if (korisnik == null)
            {
                return HttpNotFound();
            }

            KorisnikAzuriranje model = new KorisnikAzuriranje();
            model.KorisnickoIme = korisnik.KorisnickoIme;
            model.Email = korisnik.Email;
            model.Prezime = korisnik.Prezime;
            model.Ime = korisnik.Ime;
            model.Ovlast = korisnik.SifraOvlasti;

            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            return View(model);
        }

        [HttpPost]
        [OverrideAuthorization]
        [Authorize]
        public ActionResult Azuriraj(KorisnikAzuriranje model)
        {
            var korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == model.KorisnickoIme);

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var emailZauzet = bazaPodataka.PopisKorisnika.Any(x => x.Email == model.Email && x.KorisnickoIme != model.KorisnickoIme);
                if (emailZauzet)
                {
                    ModelState.AddModelError("Email", "Email je već zauzet");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    korisnik.Email = model.Email;
                    korisnik.Prezime = model.Prezime;
                    korisnik.Ime = model.Ime;
                    korisnik.SifraOvlasti = model.Ovlast;

                    bazaPodataka.Entry(korisnik).State = EntityState.Modified;
                    bazaPodataka.Configuration.ValidateOnSaveEnabled = false;
                    bazaPodataka.SaveChanges();
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Došlo je do greške!!");
                }

                return RedirectToAction("Index", "Home");
            }

            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            return View(model);
        }

        [HttpGet]
        [OverrideAuthorization]
        [Authorize]
        public ActionResult ResetLozinke(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == id);
            if (korisnik == null)
            {
                return HttpNotFound();
            }

            KorisnikResetLozinke model = new KorisnikResetLozinke();
            model.KorisnickoIme = korisnik.KorisnickoIme;

            return View(model);
        }

        [HttpPost]
        [OverrideAuthorization]
        [Authorize]
        public ActionResult ResetLozinke(KorisnikResetLozinke model)
        {
            var korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == model.KorisnickoIme);

            if (ModelState.IsValid)
            {
                try
                {
                    var pass = Misc.PasswordHelper.HashPassword(model.Lozinka);
                    korisnik.Salt = pass.Item1;
                    korisnik.Lozinka = pass.Item2;

                    bazaPodataka.Entry(korisnik).State = EntityState.Modified;
                    bazaPodataka.Configuration.ValidateOnSaveEnabled = false;
                    bazaPodataka.SaveChanges();
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Došlo je do greške!!");
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Brisi(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == id);
            if (korisnik == null)
            {
                return HttpNotFound();
            }

            return View(korisnik);
        }

        [HttpPost, ActionName("Brisi")]
        [ValidateAntiForgeryToken]
        public ActionResult BrisiPotvrda(string id)
        {
            var korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == id);
            if (korisnik == null)
            {
                return HttpNotFound();
            }

            try
            {
                bazaPodataka.PopisKorisnika.Remove(korisnik);
                bazaPodataka.SaveChanges();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Došlo je do greške!!");
            }

            return RedirectToAction("Popis");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Prijava(string returnUrl)
        {
            KorisnikPrijava model = new KorisnikPrijava();
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Prijava(KorisnikPrijava model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var korisnikBaza = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == model.KorisnickoIme);
                if (korisnikBaza != null)
                {
                    var passwordOK = Misc.PasswordHelper.ValidatePassword(model.Lozinka, korisnikBaza.Lozinka, korisnikBaza.Salt);

                    if (passwordOK)
                    {
                        LogiraniKorisnik prijavljeniKorisnik = new LogiraniKorisnik(korisnikBaza);
                        LogiraniKorisnikSerializeModel serializeModel = new LogiraniKorisnikSerializeModel();
                        serializeModel.CopyFromUser(prijavljeniKorisnik);
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        string korisnickiPodaci = serializer.Serialize(serializeModel);

                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1,
                            prijavljeniKorisnik.Identity.Name,
                            DateTime.Now,
                            DateTime.Now.AddDays(1),
                            false,
                            korisnickiPodaci);

                        string ticketEncrypted = FormsAuthentication.Encrypt(authTicket);

                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncrypted);
                        Response.Cookies.Add(cookie);

                        if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError("", "Neispravno korisničko ime ili lozinka");
            return View(model);
        }

        [OverrideAuthorization]
        [Authorize]
        public ActionResult Odjava()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult DodajAdmin()
        {
            Korisnik model = new Korisnik();

            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            return View(model);
        }

        [HttpPost]
        public ActionResult DodajAdmin(Korisnik model)
        {
            if (!String.IsNullOrWhiteSpace(model.KorisnickoIme))
            {
                var korImeZauzeto = bazaPodataka.PopisKorisnika.Any(x => x.KorisnickoIme == model.KorisnickoIme);
                if (korImeZauzeto)
                {
                    ModelState.AddModelError("KorisnickoIme", "Korisničko ime je već zauzeto");
                }
            }
            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var emailZauzet = bazaPodataka.PopisKorisnika.Any(x => x.Email == model.Email);
                if (emailZauzet)
                {
                    ModelState.AddModelError("Email", "Email je već zauzet");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var hashLozinke = Misc.PasswordHelper.HashPassword(model.LozinkaUnos);
                    model.Salt = hashLozinke.Item1;
                    model.Lozinka = hashLozinke.Item2;

                    bazaPodataka.PopisKorisnika.Add(model);
                    bazaPodataka.SaveChanges();
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Došlo je do greške!!");
                }

                return RedirectToAction("Index", "Home");
            }

            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            return View(model);
        }
    }
}