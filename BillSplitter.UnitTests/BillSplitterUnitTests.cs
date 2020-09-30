using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BillSplitter.UnitTests
{
    [TestClass]
    public class BillSplitterUnitTests
    {
        [TestMethod]
        public void IsValidFileName_ValidFileNameWithOutExtension_ReturnsTrue()
        {
            BillSplitter Obj = new BillSplitter();

            string filename = "avalidfilenamewithoutextension";
            var result = Obj.IsValidFileName(filename, out string strmsg);

            Assert.IsTrue(result.Item1);

        }

        [TestMethod]
        public void IsValidFileName_ValidFileNameWithExtension_ReturnsTrue()
        {
            BillSplitter Obj = new BillSplitter();

            string filename = "avalidfilenamewithextension.txt";
            var result = Obj.IsValidFileName(filename, out string strmsg);

            Assert.IsTrue(result.Item1);

        }

        [TestMethod]
        public void IsValidFileName_ValidFileNameWithInvalidExtension_ReturnsFalse()
        {
            BillSplitter Obj = new BillSplitter();

            string filename = "avalidfilenamewithInvalidextension.pdf";
            var result = Obj.IsValidFileName(filename, out string strmsg);

            Assert.IsFalse(result.Item1);

        }

        [TestMethod]
        public void IsFileExists_ValidFileName_NoFileInFolder_ReturnsFalse()
        {
            BillSplitter Obj = new BillSplitter();

            string filename = "avalidfilename";
            string path = "";
            var result = Obj.IsFileExists(filename, path, out string strmsg);

            Assert.IsFalse(result.Item1);

        }

        [TestMethod]
        public void ReadInputFile_ValidFileName_FileInFolder_ReturnsTrue()
        {
            BillSplitter Obj = new BillSplitter();

            string filename = @"TestInput\avalidfilename.txt";
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string file = Path.Combine(currentDirectory, filename);

            var result = Obj.ReadInputFile(file, out string strmsg);

            Assert.IsTrue(result.Item1);
        }

        [TestMethod]
        public void WriteFile_ValidFileName_FileInFolder_ReturnsTrue()
        {
            BillSplitter Obj = new BillSplitter();

            string filename = @"TestInput\avalidfilename.txt";
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string file = Path.Combine(currentDirectory, filename);
        
            string[]arrayBill = new string[] { "1","1","3","0" };

            var result = Obj.WriteOutPutFile(arrayBill, file, out string errmsg);

            Assert.IsTrue(result);
        }
    }
}
