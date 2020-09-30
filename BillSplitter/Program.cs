using System;
using System.IO;
using System.Reflection;

namespace BillSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine("Enter the file name");
            string inputFileName = Console.ReadLine();
            inputFileName = inputFileName.ToLower().Trim();

            BillSplitter BillSplit = new BillSplitter();
            (bool flag, string file) = BillSplit.IsValidFile(inputFileName, currentDirectory, out string errmsg);
            
            string[] arrayBill = null;

            if (flag)
            {
                (flag, arrayBill) = BillSplit.ReadInputFile(file, out errmsg);
            }

            if (flag)
            {
                flag = BillSplit.WriteOutPutFile(arrayBill, file, out errmsg);
            }

            if (flag)
            {
                Console.WriteLine($"Output file created in folder ({currentDirectory}");
            }
            else
            {
                Console.WriteLine(" **** ERROR while proccesing the input file **** "+ Environment.NewLine +errmsg);
            }

            Console.ReadKey();
        }
    }
}
