using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedulr.Infrastructure;

namespace Schedulr.Test
{
    [TestClass]
    public class SchedulrExtensionsTest
    {
        [TestMethod]
        public void ValueOr()
        {
            Assert.AreEqual<bool>(true, new bool?(true).ValueOr(false));
            Assert.AreEqual<bool>(false, new bool?(false).ValueOr(false));
            Assert.AreEqual<bool>(true, new bool?(true).ValueOr(true));
            Assert.AreEqual<bool>(false, new bool?(false).ValueOr(true));
            Assert.AreEqual<bool>(true, new bool?().ValueOr(true));
            Assert.AreEqual<bool>(false, new bool?().ValueOr(false));
        }
    }
}