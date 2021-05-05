using AutoPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.Data.Entity.Infrastructure;
using MySql.Data.MySqlClient;
using System.Data.Entity;
using System.Web;
using System.IO;
using AutoPortal.Misc;

namespace AutoPortal.Controllers
{
    public class HomeController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();
        int[] broj = new int[2];

        public ActionResult Index()
        {
            var auti = bazaPodataka.PopisAuto.OrderBy(x => x.Marka).ToList();
            List<Auto> marke = new List<Auto>();
            marke.Add(auti[0]);
            for (int i = 1; i < auti.Count(); i++)
            {
                if (auti[i].Marka != auti[i - 1].Marka)
                {
                    marke.Add(auti[i]);
                }
            }

            return View(marke);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Popis(string car)
        {
            if (car == null)
            {
                return RedirectToAction("Index");
            }

            var auti = bazaPodataka.PopisAuto.OrderBy(x => x.Model).ToList();
            List<Auto> model = new List<Auto>();
            foreach (var auto in auti)
            {
                if(auto.Marka == car)
                {
                    model.Add(auto);
                }
            }

            if(model.Count() == 0)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }


        public ActionResult Detalji(int? id)
        {
            Auto auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == id);

            if (auto == null)
            {
                return RedirectToAction("Index");
            }

            return View(auto);
        }

        [Authorize]
        public ActionResult Pretrazivanje()
        {
            return View();
        }
        [Authorize]
        public ActionResult PretrazivanjePartial(string marka,string model,int? page, string sort)
        {
            var auto = bazaPodataka.PopisAuto.ToList();

            ViewBag.Sortiranje = sort;
            ViewBag.MarkaSort = string.IsNullOrEmpty(sort) ? "marka_desc" : "";
            ViewBag.ModelSort = sort == "model" ? "model_desc" : "model";
            ViewBag.ObujamSort = sort == "obujam_desc" ? "obujam" : "obujam_desc";
            ViewBag.SnagaSort = sort == "snaga_desc" ? "snaga" : "snaga_desc";
            ViewBag.KilazaSort = sort == "kilaza_desc" ? "kilaza" : "kilaza_desc";
            ViewBag.BrzinaSort = sort == "brzina_desc" ? "brzina" : "brzina_desc";
            ViewBag.UbrzanjeSort = sort == "ubrzanje" ? "ubrzanje_desc" : "ubrzanje";

            ViewBag.Marka = marka;
            ViewBag.Model = model;

            //filtriranje
            if (!String.IsNullOrWhiteSpace(marka))
                auto = auto.Where(x => x.Marka.ToUpper().Contains(marka.ToUpper())).ToList();

            if (!String.IsNullOrWhiteSpace(model))
                auto = auto.Where(x => x.Model.ToUpper().Contains(model.ToUpper())).ToList();

            switch (sort)
            {
                case "marka_desc":
                    auto = auto.OrderByDescending(x => x.Marka).ToList();
                    break;
                case "model":
                    auto = auto.OrderBy(x => x.Model).ToList();
                    break;
                case "model_desc":
                    auto = auto.OrderByDescending(x => x.Model).ToList();
                    break;
                case "obujam":
                    auto = auto.OrderBy(x => x.Obujam).ToList();
                    break;
                case "obujam_desc":
                    auto = auto.OrderByDescending(x => x.Obujam).ToList();
                    break;
                case "snaga":
                    auto = auto.OrderBy(x => x.Snaga).ToList();
                    break;
                case "snaga_desc":
                    auto = auto.OrderByDescending(x => x.Snaga).ToList();
                    break;
                case "kilaza":
                    auto = auto.OrderBy(x => x.Kilaza).ToList();
                    break;
                case "kilaza_desc":
                    auto = auto.OrderByDescending(x => x.Kilaza).ToList();
                    break;
                case "brzina":
                    auto = auto.OrderBy(x => x.Max_Brzina).ToList();
                    break;
                case "brzina_desc":
                    auto = auto.OrderByDescending(x => x.Max_Brzina).ToList();
                    break;
                case "ubrzanje":
                    auto = auto.OrderBy(x => x.Ubrzanje).ToList();
                    break;
                case "ubrzanje_desc":
                    auto = auto.OrderByDescending(x => x.Ubrzanje).ToList();
                    break;
                default:
                    auto = auto.OrderBy(x => x.Marka).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            return PartialView("_PartialPretrazivanje", auto.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult Create()
        {
            Auto auto = new Auto();

            return View(auto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult Create(Auto auto)
        {
            if (auto.Obujam < 0)
            {
                ModelState.AddModelError("Obujam", "Obujam ne može biti negativan");
            }

            if (auto.Snaga < 0)
            {
                ModelState.AddModelError("Snaga", "Snaga ne može biti negativna");
            }

            if (auto.Kilaza < 0)
            {
                ModelState.AddModelError("Kilaza", "Kilaža ne može biti negativna");
            }

            if (auto.Max_Brzina < 0)
            {
                ModelState.AddModelError("Max_Brzina", "Max brzina ne može biti negativna");
            }

            if (auto.Ubrzanje < 0)
            {
                ModelState.AddModelError("Ubrzanje", "Ubrzanje ne može biti negativno");
            }

            for (int i = 0; i < auto.Cijena.Length; i++)
            {
                if (auto.Cijena[i] >= 65 && auto.Cijena[i] <= 119)
                {
                    ModelState.AddModelError("Cijena", "Molimo unesite cijenu a ne text");
                    break;
                }
            }

            for (int i = 0; i < auto.Cijena.Length; i++)
            {
                if (!((auto.Cijena[i] == 46) || (auto.Cijena[i] >= 48 && auto.Cijena[i] <= 57)))
                {
                    ModelState.AddModelError("Cijena", "Molimo unesite valjanu cijenu");
                    break;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bazaPodataka.PopisAuto.Add(auto);
                    bazaPodataka.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Došlo je do greške!!");
                }

            }

            return View(auto);
        }

        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult Edit(int? id)
        {
            Auto auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == id);
            if (auto == null)
            {
                return RedirectToAction("Index");
            }
            return View(auto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult Edit(Auto auto)
        {
            if (auto.Obujam < 0)
            {
                ModelState.AddModelError("Obujam", "Obujam ne može biti negativan");
            }

            if (auto.Snaga < 0)
            {
                ModelState.AddModelError("Snaga", "Snaga ne može biti negativna");
            }

            if (auto.Kilaza < 0)
            {
                ModelState.AddModelError("Kilaza", "Kilaža ne može biti negativna");
            }

            if (auto.Max_Brzina < 0)
            {
                ModelState.AddModelError("Max_Brzina", "Max brzina ne može biti negativna");
            }

            if (auto.Ubrzanje < 0)
            {
                ModelState.AddModelError("Ubrzanje", "Ubrzanje ne može biti negativno");
            }

            for (int i = 0; i < auto.Cijena.Length; i++)
            {
                if (auto.Cijena[i] >= 65 && auto.Cijena[i] <= 119)
                {
                    ModelState.AddModelError("Cijena", "Molimo unesite cijenu a ne text");
                    break;
                }
            }

            for (int i = 0; i < auto.Cijena.Length; i++)
            {
                if (!((auto.Cijena[i] == 46) || (auto.Cijena[i] >= 48 && auto.Cijena[i] <= 57)))
                {
                    ModelState.AddModelError("Cijena", "Molimo unesite valjanu cijenu");
                    break;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bazaPodataka.Entry(auto).State = EntityState.Modified;
                    bazaPodataka.SaveChanges();

                    if (Request.IsAjaxRequest())
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Došlo je do greške!!");
                }
                
            }

            return View(auto);
        }

        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult Delete(int? id)
        {
            Auto auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == id);
            if (auto == null)
            {
                return RedirectToAction("Index");
            }
            return View(auto);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult DeleteConfirmed(int? id)
        {
            try
            {
                Auto auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == id);
                var folder = Server.MapPath("~/Slike/Marke/") + auto.Marka;
                var logo = Server.MapPath("~/Slike/logo/") + auto.Marka + ".png";
                string filename = Server.MapPath("~/Slike/Marke/") + auto.Marka + "\\" + auto.Marka + " " + auto.Model + ".jpg";
                var Slike = Directory.GetFiles(Server.MapPath("~/Slike/logo/"));
                var dirfol = Directory.GetFiles(Server.MapPath("~/Slike/Marke/"));
                if (Directory.Exists(folder))
                {
                    if (Directory.GetFiles(folder).Count() < 2)
                    {
                        string path2 = Path.Combine(Server.MapPath("~/Slike/Marke/"), auto.Marka + "\\" + auto.Marka + " " + auto.Model + ".jpg");
                        System.IO.File.Delete(path2);
                        Directory.Delete(folder);
                        string path = Path.Combine(Server.MapPath("~/Slike/logo/"), auto.Marka + ".png");
                        System.IO.File.Delete(path);
                    }
                    else
                    {
                        foreach (var dir in dirfol)
                        {
                            if (dir == filename)
                            {
                                string path = Path.Combine(Server.MapPath("~/Slike/Marke/"), auto.Marka + "\\" + auto.Marka + " " + auto.Model + ".jpg");
                                System.IO.File.Delete(path);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var slike in Slike)
                    {
                        if (slike == logo)
                        {
                            string path = Path.Combine(Server.MapPath("~/Slike/logo/"), auto.Marka + ".png");
                            System.IO.File.Delete(path);
                        }
                    }
                }

                bazaPodataka.PopisAuto.Remove(auto);
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

            return RedirectToAction("Index");

        }

        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult DodajLogo(int? id)
        {
            Auto auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == id);
            if (auto == null)
            {
                return RedirectToAction("Index");
            }
            return View(auto);
        }

        [HttpPost]
        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult DodajLogo(HttpPostedFileBase file, string marka)
        {
            try
            {
                string filename = marka + ".png";
                string filepath = Path.Combine(Server.MapPath("~/Slike/logo/"), filename);
                file.SaveAs(filepath);
                return RedirectToAction("Popis", new { car = marka });
            }
            catch
            {
                return View(file);
            }
        }

        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult DodajSliku(int? id)
        {
            Auto auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == id);
            if (auto == null)
            {
                return RedirectToAction("Index");
            }
            return View(auto);
        }

        [HttpPost]
        [Authorize(Roles = OvlastiKorisnik.Administrator)]
        public ActionResult DodajSliku(HttpPostedFileBase file, string marka, string model)
        {
            try
            {
                var folder = Server.MapPath("~/Slike/Marke/") + marka;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filename = marka + " " + model + ".jpg";
                string filepath = Path.Combine(folder, filename);
                file.SaveAs(filepath);
                return RedirectToAction("Popis", new { car = marka });
            }
            catch
            {
                return View(file);
            }
        }

        [Authorize]
        public ActionResult Usporedi(int? id)
        {
            Auto auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == id);
            if (auto == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.auto1 = Convert.ToInt32(id);
            return View();
        }

        [Authorize]
        public ActionResult PretrazivanjePartial2(string marka, string model, int? page, string sort, int? id)
        {
            var auto = bazaPodataka.PopisAuto.ToList();

            ViewBag.Sortiranje = sort;
            ViewBag.MarkaSort = string.IsNullOrEmpty(sort) ? "marka_desc" : "";
            ViewBag.ModelSort = sort == "model" ? "model_desc" : "model";

            ViewBag.auto1 = Convert.ToInt32(id);

            ViewBag.Marka = marka;
            ViewBag.Model = model;

            //filtriranje
            if (!String.IsNullOrWhiteSpace(marka))
                auto = auto.Where(x => x.Marka.ToUpper().Contains(marka.ToUpper())).ToList();

            if (!String.IsNullOrWhiteSpace(model))
                auto = auto.Where(x => x.Model.ToUpper().Contains(model.ToUpper())).ToList();

            switch (sort)
            {
                case "marka_desc":
                    auto = auto.OrderByDescending(x => x.Marka).ToList();
                    break;
                case "model":
                    auto = auto.OrderBy(x => x.Model).ToList();
                    break;
                case "model_desc":
                    auto = auto.OrderByDescending(x => x.Model).ToList();
                    break;
                default:
                    auto = auto.OrderBy(x => x.Marka).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            return PartialView("_PartialPretrazivanje2", auto.ToPagedList(pageNumber, pageSize));
        }

        [Authorize]
        public ActionResult Usporedi2(int? id, int? id2)
        {
            List<Auto> mojalista = new List<Auto>();
            Auto auti = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == id);
            if (auti == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                broj[0] = Convert.ToInt32(id2);
                broj[1] = Convert.ToInt32(id);
            }

            foreach (var kljuc in broj)
            {
                Auto auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.IdAuto == kljuc);
                mojalista.Add(auto);
            }

            return View(mojalista);
        }
    }
}