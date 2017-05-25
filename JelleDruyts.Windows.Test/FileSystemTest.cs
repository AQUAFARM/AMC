using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelleDruyts.Windows.Test
{
    [TestClass]
    public class FileSystemTest
    {
        [TestMethod]
        public void GetTempFileShouldCreateFile()
        {
            var fileName = FileSystem.GetTempFileName();

            Assert.IsNotNull(fileName);
            Assert.IsTrue(File.Exists(fileName));
            Assert.AreEqual<string>(Path.GetDirectoryName(Path.GetTempPath()), Path.GetDirectoryName(fileName));

            File.Delete(fileName);
        }

        [TestMethod]
        public void GetTempFileShouldCreateFileWithExtension()
        {
            var fileName = FileSystem.GetTempFileName(".txt");

            Assert.IsNotNull(fileName);
            Assert.IsTrue(File.Exists(fileName));
            Assert.AreEqual<string>(Path.GetDirectoryName(Path.GetTempPath()), Path.GetDirectoryName(fileName));
            Assert.AreEqual<string>(".txt", Path.GetExtension(fileName));

            File.Delete(fileName);
        }

        [TestMethod]
        public void GetTempFileShouldCreateFileWithExtensionWithoutDot()
        {
            var fileName = FileSystem.GetTempFileName("txt");

            Assert.IsNotNull(fileName);
            Assert.IsTrue(File.Exists(fileName));
            Assert.AreEqual<string>(Path.GetDirectoryName(Path.GetTempPath()), Path.GetDirectoryName(fileName));
            Assert.AreEqual<string>(".txt", Path.GetExtension(fileName));

            File.Delete(fileName);
        }

        [TestMethod]
        public void GetTempFileShouldCreateFileWithNullExtension()
        {
            var fileName = FileSystem.GetTempFileName(null);

            Assert.IsNotNull(fileName);
            Assert.IsTrue(File.Exists(fileName));
            Assert.AreEqual<string>(Path.GetDirectoryName(Path.GetTempPath()), Path.GetDirectoryName(fileName));
            Assert.AreEqual<string>(string.Empty, Path.GetExtension(fileName));

            File.Delete(fileName);
        }

        [TestMethod]
        public void GetTempFileShouldCreateFileWithEmptyExtension()
        {
            var fileName = FileSystem.GetTempFileName(string.Empty);

            Assert.IsNotNull(fileName);
            Assert.IsTrue(File.Exists(fileName));
            Assert.AreEqual<string>(Path.GetDirectoryName(Path.GetTempPath()), Path.GetDirectoryName(fileName));
            Assert.AreEqual<string>(string.Empty, Path.GetExtension(fileName));

            File.Delete(fileName);
        }

        [TestMethod]
        public void GetValidFileExtensionShouldReturnValidFileExtension()
        {
            Assert.AreEqual<string>(null, FileSystem.EnsureValidFileExtension(null));
            Assert.AreEqual<string>(string.Empty, FileSystem.EnsureValidFileExtension(string.Empty));
            Assert.AreEqual<string>(".txt", FileSystem.EnsureValidFileExtension("txt"));
            Assert.AreEqual<string>(".txt", FileSystem.EnsureValidFileExtension(".txt"));
            Assert.AreEqual<string>("..txt", FileSystem.EnsureValidFileExtension("..txt"));
        }
    }
}