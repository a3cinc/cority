using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace BillSplitter
{
    public class BillSplitter
    {
        #region enum
        //class level enum
        enum TripStage
        {
            Start = 0,
            BillCount = 1,
            IndividualBill = 2,
            End = 3
        }
        #endregion enum

        #region [Public Methods]

        /// <summary>
        /// Validate the input file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public (bool, string) IsValidFile(string filename, string path, out string errmsg)
        {
            bool flag;

            // check if input has extension, if so validate its correctness
            (flag, filename) = IsValidFileName(filename, out errmsg);

            // check if the file exists in the folder. if true output the file ( filename + path)
            if (flag)
                (flag, filename) = IsFileExists(filename, path, out errmsg);

            return (flag, filename);
        }

        /// <summary>
        /// Reads a txt file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public (bool, string[]) ReadInputFile(string file, out string errmsg)
        {
            bool flag = true;
            errmsg = "";

            string[] arrayBill = null;

            try
            {
                arrayBill =
                    System.IO.File.ReadAllLines(file)
                            .Where(line => !string.IsNullOrWhiteSpace(line) && line.Trim() != string.Empty)
                            .Select(line => line.Trim()).ToArray();
            }
            catch (Exception ex)
            {
                // IO exception.
                errmsg = ex.Message;
                flag = false;
            }

            // check for end of record/file value (= 0)
            int length = arrayBill.Length;

            if (length > 0)
            {
                try
                {
                    decimal lastdigit = decimal.Parse(arrayBill[length - 1]);

                    if (lastdigit != 0)
                    {
                        errmsg = $"Last camping entry is missing in file : the last line entry should be 0. current value is {lastdigit}";
                        flag = false;
                    }
                }
                catch (Exception ex)
                {
                    errmsg = $"Invalid data in file : the last line entry should be 0. \n {ex.Message}";
                    flag = false;
                }
            }
            else
            {
                flag = false;
                errmsg = $"Coun't find camping details : file is empty or has got incorrect entries.";
            }

            return (flag, arrayBill);
        }

        /// <summary>
        /// Creates a txt file
        /// </summary>
        /// <param name="arrayBill"></param>
        /// <param name="file"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public bool WriteOutPutFile(string[] arrayBill, string file, out string errmsg)
        {
            bool flag = true;
            errmsg = "";

          
            // convert string array to decimal 
            decimal[] tmpArrayBill = null;
            try
            {
                tmpArrayBill = arrayBill.Select(decimal.Parse).ToArray();
            }
            catch (Exception ex)
            {
                errmsg = "Invalid data/characters in input file " + Environment.NewLine + ex.Message;
                flag = false;
            }

            string outputText = String.Empty;
            if(flag)
                (flag, outputText) = GetOutText(tmpArrayBill, out errmsg);

            if (flag)
            {
                try
                {
                    // new file name for out file. 
                    file = Path.ChangeExtension(file, ".txt.out");
                    
                    File.WriteAllText(file, outputText);
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                    flag = false;
                }
            }
            return flag;
        }

        #endregion

        #region [Private Methods]
        /// <summary>
        /// check the correctness of file extension if its part of the user input text.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public (bool, string) IsValidFileName(string filename, out string errmsg)
        {
            bool flag = true;
            errmsg = "";

            // check the correctness of file extension if its part of the user input text.
            if (Path.HasExtension(filename))
            {
                string extension = Path.GetExtension(filename);

                if (!Path.GetExtension(filename).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    flag = false;
                    errmsg = $"incorrect file extension ({extension}), '.txt' is expected ";
                }
            }
            else
            {
                //suffix the .txt extension.
                filename = String.Concat(filename, ".txt");
            }

            return (flag, filename);
        }

        /// <summary>
        /// Check the file exists in the working directory
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public (bool, string) IsFileExists(string filename, string path, out string errmsg)
        {
            bool flag = true;
            errmsg = "";
            string file = Path.Combine(path, filename);

            if (!File.Exists(file))
            {
                flag = false;
                errmsg = $"The File '{filename}' not present in the folder '{path}' ";
            }
            return (flag, file);
        }

        /// <summary>
        /// Generates output data format
        /// </summary>
        /// <param name="tmpArrayBill"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private (bool, string) GetOutText(decimal[] tmpArrayBill, out string errmsg)
        {
            bool flag = true;
            errmsg = "";

            int arrlength = tmpArrayBill.Length;
            TripStage trip = TripStage.Start;
            int totalParticipants = 0;
            int participantBillCount = 0;
            List<decimal> lstBillamount = new List<decimal>();
            int jgdIndex = 0;

            decimal[][] jgdArray = null;

            StringBuilder outputText = new StringBuilder();

            if (arrlength > 0)
            {
                for (int i = 0; i < arrlength; i++)
                {
                    if (trip == TripStage.Start)
                    {
                        totalParticipants = (int)Math.Floor(tmpArrayBill[i]);


                        jgdArray = new decimal[totalParticipants][];

                        trip = TripStage.BillCount;// move to next phase

                        if (i == arrlength - 1 && totalParticipants == 0)
                        {
                            trip = TripStage.End; // end the execution.
                        }
                    }
                    else if (trip == TripStage.BillCount)
                    {
                        participantBillCount = (int)Math.Floor(tmpArrayBill[i]);

                        trip = TripStage.IndividualBill; // move to next phase

                        lstBillamount.Clear();
                    }
                    else if (trip == TripStage.IndividualBill)
                    {
                        lstBillamount.Add(tmpArrayBill[i]);

                        if (lstBillamount.Count == participantBillCount)
                        {
                            // move to jagged array.
                            decimal[] arrybill = lstBillamount.ToArray();
                            jgdArray[jgdIndex] = arrybill;

                            jgdIndex += 1;// increment the index

                            trip = TripStage.BillCount;//process next person bill set in the same trip
                        }

                        if (totalParticipants == jgdIndex) // Process completed for A trip.
                        {
                            // Write o/p for processed trip.
                            outputText.Append(GenerateTripAmountSplit(jgdArray));

                            // reset jagged array 
                            trip = TripStage.Start;
                            jgdArray = null;
                            jgdIndex = 0;
                        }
                    }
                }
            }
            else
            {
                errmsg = "Data error: Invalid entries in input file";
                flag = false;
            }

            if (trip != TripStage.End)
            {
                errmsg = "Data error: Incorrect trip entries/ are not in order !! ";
                flag = false;
            }
            return (flag, outputText.ToString());
        }

        /// <summary>
        /// Calculates the split bill amount per trip
        /// </summary>
        /// <param name="jgdarray"></param>
        /// <returns></returns>
        private string GenerateTripAmountSplit(decimal[][] jgdarray)
        {
            StringBuilder sbout = new StringBuilder();

            List<decimal> lstbillamount = new List<decimal>();
            int jgdlength = jgdarray.Length;

            for (int row = 0; row < jgdlength; row++)
            {
                decimal[] tmpcol = jgdarray[row];
                lstbillamount.Add(tmpcol.Sum());
            }

            decimal perheadamount = lstbillamount.Sum() / jgdlength;
            // individual contribution
            string stramount = String.Empty;

            for (int i = 0; i < jgdlength; i++)
            {
                decimal amount = perheadamount - lstbillamount[i];

                if (amount >= 0)
                {
                    stramount = $"${amount:0.00}";
                }
                else
                {
                    stramount = $"(${(amount * -1):0.00})";
                }
                sbout.AppendLine(stramount);
            }

            sbout.AppendLine();
            sbout.AppendLine();

            return sbout.ToString();
        }

        #endregion
    }
}
