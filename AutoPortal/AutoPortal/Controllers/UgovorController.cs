using AutoPortal.Misc;
using AutoPortal.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AutoPortal.Controllers
{
    [Authorize(Roles = OvlastiKorisnik.User)]
    public class UgovorController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();

        // GET: Ugovor
        [OverrideAuthorization]
        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult Pretrazivanje()
        {
            return View();
        }

        [OverrideAuthorization]
        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult PretrazivanjePartial(string Mail_Ug, string Model, DateTime? Datum, int? page, string sort)
        {
            var ugovor = bazaPodataka.PopisUgovor.ToList();

            ViewBag.Sortiranje = sort;
            ViewBag.MailSort = string.IsNullOrEmpty(sort) ? "mail_desc" : "";
            ViewBag.ModelSort = sort == "model" ? "model_desc" : "model";
            ViewBag.DatumSort = sort == "datum_desc" ? "datum" : "datum_desc";

            ViewBag.Mail = Mail_Ug;
            ViewBag.Model = Model;
            ViewBag.Datum = Datum;

            //filtriranje
            if (!String.IsNullOrWhiteSpace(Mail_Ug))
                ugovor = ugovor.Where(x => x.Mail_Ug.ToUpper().Contains(Mail_Ug.ToUpper())).ToList();

            if (!String.IsNullOrWhiteSpace(Model))
                ugovor = ugovor.Where(x => x.Model.ToUpper().Contains(Model.ToUpper())).ToList();

            if (Datum != null)
                ugovor = ugovor.Where(x => x.Datum >= Datum).ToList();

            switch (sort)
            {
                case "mail_desc":
                    ugovor = ugovor.OrderByDescending(x => x.Mail_Ug).ToList();
                    break;
                case "model":
                    ugovor = ugovor.OrderBy(x => x.Model).ToList();
                    break;
                case "model_desc":
                    ugovor = ugovor.OrderByDescending(x => x.Model).ToList();
                    break;
                case "datum":
                    ugovor = ugovor.OrderBy(x => x.Datum).ToList();
                    break;
                case "datum_desc":
                    ugovor = ugovor.OrderByDescending(x => x.Datum).ToList();
                    break;
                default:
                    ugovor = ugovor.OrderBy(x => x.Mail_Ug).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            return PartialView("_PartialPretrazivanje", ugovor.ToPagedList(pageNumber, pageSize));
        }

        [OverrideAuthorization]
        [Authorize]
        public ActionResult Create(int? id, string ime)
        {
            var auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == id);
            var korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == ime);
            Ugovor ugovor = new Ugovor();
            ugovor.Model = auto.Model;
            ugovor.Mail_Ug = korisnik.Email;
            return View(ugovor);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [OverrideAuthorization]
        [Authorize]
        public ActionResult Create(Ugovor ugovor)
        {

            if (ugovor.Datum < DateTime.Now || ugovor.Datum < DateTime.Now.AddDays(6))
            {
                ModelState.AddModelError("Datum", "Mogućnost ugovora sastanka najranije 7 dana od današnjeg dana");
            }
            else if (ugovor.Datum.DayOfWeek == DayOfWeek.Saturday || ugovor.Datum.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError("Datum", "Saloni ne rade vikendom");
            }

            if (String.IsNullOrEmpty(ugovor.Vrijeme_Od))
            {
                ModelState.AddModelError("Vrijeme_Od", "Vrijeme je obavezno");
            }
            else if ((ugovor.Vrijeme_Od.Length < 3) || (!(ugovor.Vrijeme_Od[2] == ':')))
            {
                ModelState.AddModelError("Vrijeme_Od", "Molimo unesite vrijeme u formatu {HH:mm} od 24h sa ':'");
            }
            else if (!((Convert.ToInt32(ugovor.Vrijeme_Od.Substring(0, 2)) > 0) && (Convert.ToInt32(ugovor.Vrijeme_Od.Substring(0, 2)) <= 24) && (Convert.ToInt32(ugovor.Vrijeme_Od.Substring(3, 2)) >= 0) && (Convert.ToInt32(ugovor.Vrijeme_Od.Substring(3, 2)) <= 59) && (Convert.ToInt32(ugovor.Vrijeme_Od.Substring(0, 2)) < 24 || (Convert.ToInt32(ugovor.Vrijeme_Od.Substring(0, 2)) == 24 && Convert.ToInt32(ugovor.Vrijeme_Od.Substring(3, 2)) == 0))))
            {
                ModelState.AddModelError("Vrijeme_Od", "Molimo unesite vrijeme u formatu {HH:mm} od 24h");
            }

            if (String.IsNullOrEmpty(ugovor.Vrijeme_Do))
            {
                ModelState.AddModelError("Vrijeme_Do", "Vrijeme je obavezno");
            }
            else if ((ugovor.Vrijeme_Do.Length < 3) || (!(ugovor.Vrijeme_Do[2] == ':')))
            {
                ModelState.AddModelError("Vrijeme_Do", "Molimo unesite vrijeme u formatu {HH:mm} od 24h sa ':'");
            }
            else if (!((Convert.ToInt32(ugovor.Vrijeme_Do.Substring(0, 2)) > 0) && (Convert.ToInt32(ugovor.Vrijeme_Do.Substring(0, 2)) <= 24) && (Convert.ToInt32(ugovor.Vrijeme_Do.Substring(3, 2)) >= 0) && (Convert.ToInt32(ugovor.Vrijeme_Do.Substring(3, 2)) <= 59) && (Convert.ToInt32(ugovor.Vrijeme_Do.Substring(0, 2)) < 24 || (Convert.ToInt32(ugovor.Vrijeme_Do.Substring(0, 2)) == 24 && Convert.ToInt32(ugovor.Vrijeme_Do.Substring(3, 2)) == 0))))
            {
                ModelState.AddModelError("Vrijeme_Do", "Molimo unesite vrijeme u formatu {HH:mm} od 24h");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bazaPodataka.PopisUgovor.Add(ugovor);
                    bazaPodataka.SaveChanges();

                    if (Request.IsAjaxRequest())
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }

                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Došlo je do greške!!");
                }

            }

            return View(ugovor);
        }

        [OverrideAuthorization]
        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult Delete(int? id)
        {
            Ugovor ugovor = bazaPodataka.PopisUgovor.FirstOrDefault(x => x.IdUgovor == id);
            if (ugovor == null)
            {
                return RedirectToAction("Pretrazivanje");
            }
            return View(ugovor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [OverrideAuthorization]
        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult DeleteConfirmed(int? id)
        {
            try
            {
                Ugovor ugovor = bazaPodataka.PopisUgovor.FirstOrDefault(x => x.IdUgovor == id);
                bazaPodataka.PopisUgovor.Remove(ugovor);
                bazaPodataka.SaveChanges();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Došlo je do greške!!");
            }

            if (Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            return RedirectToAction("Pretrazivanje");

        }
    }
}