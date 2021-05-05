using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AutoPortal.Controllers;
using AutoPortal.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeControllerTest
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void MarkaValidationRequired()
        {
            Auto auto = new Auto();
            auto.Marka = null;

            var context = new ValidationContext(auto) { MemberName = "Marka" };
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateProperty(auto.Marka, context, results);

            Assert.IsFalse(valid);
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual("Marka je obavezna", results[0].ErrorMessage);
        }

        [TestMethod]
        public void MarkaValidationLenghtMin2()
        {
            Auto auto = new Auto();
            auto.Marka = "a";

            var context = new ValidationContext(auto) { MemberName = "Marka" };
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateProperty(auto.Marka, context, results);

            Assert.IsFalse(valid);
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual("Marka mora biti duljine minimalno 2 a maksimalno 25 znakova", results[0].ErrorMessage);
        }

        [TestMethod]
        public void MarkaValidationLenghtMax25()
        {
            Auto auto = new Auto();
            auto.Marka = new string('a', 26);

            var context = new ValidationContext(auto) { MemberName = "Marka" };
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateProperty(auto.Marka, context, results);

            Assert.IsFalse(valid);
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual("Marka mora biti duljine minimalno 2 a maksimalno 25 znakova", results[0].ErrorMessage);
        }


        [TestMethod]
        public void CijenaValidationRequired()
        {
            Auto auto = new Auto();
            auto.Cijena = null;

            var context = new ValidationContext(auto) { MemberName = "Cijena" };
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateProperty(auto.Cijena, context, results);

            Assert.IsFalse(valid);
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual("Cijena je obavezna", results[0].ErrorMessage);
        }

        [TestMethod]
        public void CijenaValidationLenghtMax20()
        {
            Auto auto = new Auto();
            auto.Cijena = new string('1', 21);

            var context = new ValidationContext(auto) { MemberName = "Cijena" };
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateProperty(auto.Cijena, context, results);

            Assert.IsFalse(valid);
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual("Cijena mora biti duljine minimalno 1 a maksimalno 20 znakova", results[0].ErrorMessage);
        }
    }
}