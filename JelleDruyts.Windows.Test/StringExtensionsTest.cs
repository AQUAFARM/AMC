using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelleDruyts.Windows.Test
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void BytesToDisplayString()
        {
            Assert.AreEqual<string>((4.0).ToString("f2", CultureInfo.CurrentCulture) + " KB", (4096L).ToDisplayString());
            Assert.AreEqual<string>((4.1).ToString("f2", CultureInfo.CurrentCulture) + " KB", (4200L).ToDisplayString());
            Assert.AreEqual<string>((4.39).ToString("f2", CultureInfo.CurrentCulture) + " KB", (4500L).ToDisplayString());
            Assert.AreEqual<string>((4.0).ToString("f2", CultureInfo.CurrentCulture) + " MB", (1024L * 4096L).ToDisplayString());
            Assert.AreEqual<string>((4.1).ToString("f2", CultureInfo.CurrentCulture) + " MB", (1024L * 4200L).ToDisplayString());
            Assert.AreEqual<string>((4.39).ToString("f2", CultureInfo.CurrentCulture) + " MB", (1024L * 4500L).ToDisplayString());
        }

        [TestMethod]
        public void PercentageString()
        {
            Assert.AreEqual<string>("100%", (1.0).ToPercentageString());
            Assert.AreEqual<string>("200%", (2.0).ToPercentageString());
            Assert.AreEqual<string>("10%", (0.1).ToPercentageString());
            Assert.AreEqual<string>("33%", (1.0 / 3.0).ToPercentageString());
            Assert.AreEqual<string>("67%", (2.0 / 3.0).ToPercentageString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PluralizeThrowsOnNullSingular()
        {
            string singular = null;
            singular.Pluralize(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PluralizeThrowsOnNegative()
        {
            "account".Pluralize(-1);
        }

        [TestMethod]
        public void Pluralize()
        {
            Assert.AreEqual<string>("account", "account".Pluralize(1));
            Assert.AreEqual<string>("accounts", "account".Pluralize(0));
            Assert.AreEqual<string>("accounts", "account".Pluralize(2));
            Assert.AreEqual<string>("entries", "entry".Pluralize(2));
        }

        [TestMethod]
        public void ToCountString()
        {
            // No prefix or postfix.
            Assert.AreEqual<string>("1 account", 1.ToCountString("account"));
            Assert.AreEqual<string>("0 accounts", 0.ToCountString("account"));
            Assert.AreEqual<string>("2 accounts", 2.ToCountString("account"));

            // Prefix only.
            Assert.AreEqual<string>("Updating 1 account", 1.ToCountString("account", "Updating "));
            Assert.AreEqual<string>("Updating 0 accounts", 0.ToCountString("account", "Updating "));
            Assert.AreEqual<string>("Updating 2 accounts", 2.ToCountString("account", "Updating "));

            // Postfix only.
            Assert.AreEqual<string>("1 account updated", 1.ToCountString("account", null, " updated"));
            Assert.AreEqual<string>("0 accounts updated", 0.ToCountString("account", null, " updated"));
            Assert.AreEqual<string>("2 accounts updated", 2.ToCountString("account", null, " updated"));

            // Prefix and postfix.
            Assert.AreEqual<string>("Updating 1 account now", 1.ToCountString("account", "Updating ", " now"));
            Assert.AreEqual<string>("Updating 0 accounts now", 0.ToCountString("account", "Updating ", " now"));
            Assert.AreEqual<string>("Updating 2 accounts now", 2.ToCountString("account", "Updating ", " now"));
        }
    }
}