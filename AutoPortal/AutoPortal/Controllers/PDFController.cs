using AutoPortal.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoPortal.PDF;

namespace AutoPortal.Controllers
{
    public class PDFController : Controller
    {

        BazaDbContext bazaPodataka = new BazaDbContext();
        // GET: PDF
        public ActionResult Generiraj(string car)
        {
            Auto auto = bazaPodataka.PopisAuto.FirstOrDefault(x => x.Model == car);
            if (car == null)
            {
                return RedirectToAction("Index");
            }
            if (auto == null)
            {
                return RedirectToAction("Index");
            }

            PDFgeneriraj pdf = new PDFgeneriraj();
            pdf.GenerirajPdf(auto);

            return File(pdf.Podatci, System.Net.Mime.MediaTypeNames.Application.Pdf, auto.Marka.ToString() + " " + auto.Model.ToString() + ".pdf");
        }
    }
}