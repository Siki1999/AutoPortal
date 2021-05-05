using AutoPortal.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AutoPortal.PDF
{
    public class PDFgeneriraj
    {
        public byte[] Podatci { get; private set; }

        public void GenerirajPdf(Auto auto)
        {
            var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var Header = FontFactory.GetFont("Helvetica", 15, Font.BOLDITALIC);
            var razmak = FontFactory.GetFont("Arial", 10, Font.BOLD);
            BaseFont bfontFooter = BaseFont.CreateFont(BaseFont.TIMES_ITALIC, BaseFont.CP1250, false);
            var naslov = auto.Marka + " " + auto.Model;

            using (MemoryStream memo = new MemoryStream())
            {
                // Create a Document object
                using (Document document = new Document(PageSize.A4, 50, 50, 20, 50))
                {
                    PdfWriter.GetInstance(document, memo).CloseStream = false;
                    document.Open();

                    Paragraph header = new Paragraph("Auto - Portal", Header);
                    header.Alignment = Element.ALIGN_LEFT;
                    document.Add(header);

                    document.Add(new Paragraph(" ", razmak));

                    Paragraph preface = new Paragraph(naslov, titleFont);
                    preface.Alignment = Element.ALIGN_CENTER;
                    document.Add(preface);

                    try
                    {
                        var path = @"~/Slike/Marke/"  + auto.Marka +  "/"  + auto.Marka + " " + auto.Model + ".jpg";
                        var imagepath = System.Web.Hosting.HostingEnvironment.MapPath(path);
                        Image img = Image.GetInstance(imagepath);
                        if (img.Height > img.Width)
                        {
                            //Maximum height is 800 pixels.
                            float percentage = 0.0f;
                            percentage = 700 / img.Height;
                            img.ScalePercent(percentage * 100);
                        }
                        else
                        {
                            //Maximum width is 600 pixels.
                            float percentage = 0.0f;
                            percentage = 540 / img.Width;
                            img.ScalePercent(percentage * 100);
                        }
                        img.Alignment = Element.ALIGN_CENTER;
                        document.Add(img);
                    }
                    catch
                    {
                        var path = @"~/Slike/Default/noimage.jpg";
                        var imagepath = System.Web.Hosting.HostingEnvironment.MapPath(path);
                        Image img = Image.GetInstance(imagepath);
                        if (img.Height > img.Width)
                        {
                            //Maximum height is 800 pixels.
                            float percentage = 0.0f;
                            percentage = 700 / img.Height;
                            img.ScalePercent(percentage * 100);
                        }
                        else
                        {
                            //Maximum width is 600 pixels.
                            float percentage = 0.0f;
                            percentage = 540 / img.Width;
                            img.ScalePercent(percentage * 100);
                        }
                        img.Alignment = Element.ALIGN_CENTER;
                        document.Add(img);
                    }


                    document.Add(new Paragraph(" ", razmak));

                    PdfPTable table = new PdfPTable(2);
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.WidthPercentage = 100f;
                    table.DefaultCell.Border = 0;

                    PdfPCell marka = new PdfPCell(new Phrase("Marka : ", titleFont));
                    marka.Border = 0;
                    marka.VerticalAlignment = Element.ALIGN_MIDDLE;
                    marka.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(marka);

                    PdfPCell marka1 = new PdfPCell(new Phrase(auto.Marka.ToString(), titleFont));
                    marka1.Border = 0;
                    marka1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    marka1.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(marka1);

                    PdfPCell model = new PdfPCell(new Phrase("Model : ", titleFont));
                    model.Border = 0;
                    model.VerticalAlignment = Element.ALIGN_MIDDLE;
                    model.HorizontalAlignment = Element.ALIGN_CENTER;
                    model.PaddingTop = 10;
                    table.AddCell(model);

                    PdfPCell model1 = new PdfPCell(new Phrase(auto.Model.ToString(), titleFont));
                    model1.Border = 0;
                    model1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    model1.HorizontalAlignment = Element.ALIGN_CENTER;
                    model1.PaddingTop = 10;
                    table.AddCell(model1);

                    PdfPCell obujam = new PdfPCell(new Phrase("Obujam : ", titleFont));
                    obujam.Border = 0;
                    obujam.VerticalAlignment = Element.ALIGN_MIDDLE;
                    obujam.HorizontalAlignment = Element.ALIGN_CENTER;
                    obujam.PaddingTop = 10;
                    table.AddCell(obujam);

                    PdfPCell obujam1 = new PdfPCell(new Phrase((auto.Obujam.ToString() + " cm3"), titleFont));
                    obujam1.Border = 0;
                    obujam1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    obujam1.HorizontalAlignment = Element.ALIGN_CENTER;
                    obujam1.PaddingTop = 10;
                    table.AddCell(obujam1);

                    PdfPCell snaga = new PdfPCell(new Phrase("Snaga : ", titleFont));
                    snaga.Border = 0;
                    snaga.VerticalAlignment = Element.ALIGN_MIDDLE;
                    snaga.HorizontalAlignment = Element.ALIGN_CENTER;
                    snaga.PaddingTop = 10;
                    table.AddCell(snaga);

                    PdfPCell snaga1 = new PdfPCell(new Phrase((auto.Snaga.ToString() + " KS"), titleFont));
                    snaga1.Border = 0;
                    snaga1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    snaga1.HorizontalAlignment = Element.ALIGN_CENTER;
                    snaga1.PaddingTop = 10;
                    table.AddCell(snaga1);

                    PdfPCell kilaza = new PdfPCell(new Phrase("Kilaža : ", titleFont));
                    kilaza.Border = 0;
                    kilaza.VerticalAlignment = Element.ALIGN_MIDDLE;
                    kilaza.HorizontalAlignment = Element.ALIGN_CENTER;
                    kilaza.PaddingTop = 10;
                    table.AddCell(kilaza);

                    PdfPCell kilaza1 = new PdfPCell(new Phrase((auto.Kilaza.ToString() + " kg"), titleFont));
                    kilaza1.Border = 0;
                    kilaza1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    kilaza1.HorizontalAlignment = Element.ALIGN_CENTER;
                    kilaza1.PaddingTop = 10;
                    table.AddCell(kilaza1);

                    PdfPCell brzina = new PdfPCell(new Phrase("Maksimalna brzina : ", titleFont));
                    brzina.Border = 0;
                    brzina.VerticalAlignment = Element.ALIGN_MIDDLE;
                    brzina.HorizontalAlignment = Element.ALIGN_CENTER;
                    brzina.PaddingTop = 10;
                    table.AddCell(brzina);

                    PdfPCell brzina1 = new PdfPCell(new Phrase((auto.Max_Brzina.ToString() + " kmh"), titleFont));
                    brzina1.Border = 0;
                    brzina1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    brzina1.HorizontalAlignment = Element.ALIGN_CENTER;
                    brzina1.PaddingTop = 10;
                    table.AddCell(brzina1);

                    PdfPCell ubrzanje = new PdfPCell(new Phrase("Ubrzanje : ", titleFont));
                    ubrzanje.Border = 0;
                    ubrzanje.VerticalAlignment = Element.ALIGN_MIDDLE;
                    ubrzanje.HorizontalAlignment = Element.ALIGN_CENTER;
                    ubrzanje.PaddingTop = 10;
                    table.AddCell(ubrzanje);

                    PdfPCell ubrzanje1 = new PdfPCell(new Phrase((auto.Ubrzanje.ToString() + " s"), titleFont));
                    ubrzanje1.Border = 0;
                    ubrzanje1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    ubrzanje1.HorizontalAlignment = Element.ALIGN_CENTER;
                    ubrzanje1.PaddingTop = 10;
                    table.AddCell(ubrzanje1);

                    PdfPCell cijena = new PdfPCell(new Phrase("Cijena : ", titleFont));
                    cijena.Border = 0;
                    cijena.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cijena.HorizontalAlignment = Element.ALIGN_CENTER;
                    cijena.PaddingTop = 10;
                    table.AddCell(cijena);

                    PdfPCell cijena1 = new PdfPCell(new Phrase((auto.Cijena.ToString() + " kn"), titleFont));
                    cijena1.Border = 0;
                    cijena1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cijena1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cijena1.PaddingTop = 10;
                    table.AddCell(cijena1);

                    document.Add(table);
                }

                Podatci = memo.ToArray();

                using (var reader = new PdfReader(Podatci))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var stamper = new PdfStamper(reader, ms))
                        {
                            int pageCount = reader.NumberOfPages;
                            for (int i = 1; i <= pageCount; i++)
                            {
                                Rectangle pageSize = reader.GetPageSize(i);
                                PdfContentByte canvas = stamper.GetOverContent(i);

                                canvas.BeginText();
                                canvas.SetFontAndSize(bfontFooter, 10);

                                canvas.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, $"Stranica {i} / {pageCount}", pageSize.Right - 50, 30, 0);
                                canvas.EndText();
                            }
                        }
                        Podatci = ms.ToArray();
                    }
                }
            }
        }
    }
}