using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedulr.Models;

namespace Schedulr.Test
{
    [TestClass]
    public class PictureTest
    {
        [TestMethod]
        public void FileSizeIsNegativeOnInexistentFile()
        {
            var sut = new Picture();
            Assert.IsTrue(sut.FileSize < 0);
            sut.FileName = Util.GetDummyFileName();
            Assert.IsTrue(sut.FileSize < 0);
        }
    }
}