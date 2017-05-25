using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelleDruyts.Windows.Test
{
    [TestClass]
    public class SexagesimalTest
    {
        [TestMethod]
        public void TryParse()
        {
            double result;
            var succeeded = Sexagesimal.TryParse(null, out result);
            Assert.IsFalse(succeeded);
            Assert.AreEqual<double>(0, result);

            succeeded = Sexagesimal.TryParse(string.Empty, out result);
            Assert.IsFalse(succeeded);
            Assert.AreEqual<double>(0, result);

            succeeded = Sexagesimal.TryParse("12", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(12, result);

            succeeded = Sexagesimal.TryParse("12.345", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(12.345, result);

            succeeded = Sexagesimal.TryParse("12,345", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(12 + (345d / 60d), result);

            succeeded = Sexagesimal.TryParse("12,345,67", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(12 + (345d / 60d) + (67d / (60d * 60d)), result);

            succeeded = Sexagesimal.TryParse("12N", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(12, result);

            succeeded = Sexagesimal.TryParse("12E", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(12, result);

            succeeded = Sexagesimal.TryParse("12S", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(-12, result);

            succeeded = Sexagesimal.TryParse("12W", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(-12, result);

            succeeded = Sexagesimal.TryParse("12,345N", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(12 + (345d / 60d), result);

            succeeded = Sexagesimal.TryParse("12,345E", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(12 + (345d / 60d), result);

            succeeded = Sexagesimal.TryParse("12,345S", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(-(12 + (345d / 60d)), result);

            succeeded = Sexagesimal.TryParse("12,345W", out result);
            Assert.IsTrue(succeeded);
            Assert.AreEqual<double>(-(12 + (345d / 60d)), result);
        }
    }
}