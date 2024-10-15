using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsDapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetGenerateFile
    {
        private static GetNSWL GetNSWL = new GetNSWL();
        public static string TextFileContent(string data, int length, string emptyReplceData, bool isLeft)
        {
            string result = "";

            if (data.Length > length)
            {
                result = data.Substring(0, length);
            }
            else
            {
                if (isLeft)
                {
                    result = data.PadLeft(length, Convert.ToChar(emptyReplceData));
                }
                else
                {
                    result = data.PadRight(length, Convert.ToChar(emptyReplceData));
                }
            }

            return result;
        }

        public string CreateTextFile(string filename, string fileContent, string subFolderName)
        {
            string folderPath = "~/TextFile/" + subFolderName + "/";
            string path = HttpContext.Current.Server.MapPath(folderPath);
            string filecreation = path + filename;

            TryToDelete(filecreation);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter writer = new StreamWriter(filecreation, true))
            {
                writer.WriteLine(fileContent);
                writer.Close();
            }

            return folderPath;
        }

        static bool TryToDelete(string f)
        {
            try
            {
                // A.
                // Try to delete the file.
                File.Delete(f);
                return true;
            }
            catch (IOException)
            {
                // B.
                // We could not delete the file.
                return false;
            }
        }

        public static string GenerateFileMaybankTax(List<sp_TaxCP39_Result> maybankrcmsList, tbl_Ladang tbl_Ladang, string bulan, string tahun, int? NegaraID, int? SyarikatID, int? Region, int? Estate, string filter, DateTime PaymentDate, out string filename)
        {
            decimal? TotalMTDAmount = 0;
            decimal? TotalCP38Amount = 0;
            decimal? TotalAllAmount = 0;
            int CountAllData = 0;
            int CountMTDData = 0;
            int CountCP38Data = 0;
            int rowno = 1;
            long TotalHash = 0;
            long SumAllTotalHash = 0;
            long TaxNoFirst8Digit = 0;
            long TaxNoLast2Digit = 0;
            int WifeCodeInt = 0;
            int MTDAmountInt = 0;
            int CP38AmountInt = 0;
            string statusmsg = "";
            string CorpID = "";
            string ClientID = "";
            string AccNo = "";
            string InitialName = "";
            string AccNoWorker = "";
            string ClientIDText = "";
            string EmployerTaxNo = "";
            string day = PaymentDate.ToString("dd");
            string month = PaymentDate.ToString("MM");
            string year = PaymentDate.ToString("yyyy");
            string WorkerTaxNo = "";
            DateTime? PaymentDateFormat = new DateTime(PaymentDate.Year, PaymentDate.Month, PaymentDate.Day);

            GetNSWL.GetSyarikatRCMSDetail(Region, Estate, out CorpID, out ClientID, out AccNo, out InitialName);
            string filePath = "~/MaybankFile/" + tahun + "/" + bulan + "/" + NegaraID.ToString() + "_" + SyarikatID.ToString() + "/" /*+ WilayahID.ToString() + "/"*/;
            string path = HttpContext.Current.Server.MapPath(filePath);

            EmployerTaxNo = tbl_Ladang.fld_EmployerTaxNo.ToUpper();
            filename = "M2E MTD (" + tbl_Ladang.fld_LdgCode.ToUpper() + ") " + " " + bulan + tahun + ".txt";
            string filecreation = path + filename;

            try
            {
                TryToDelete(filecreation);
                if (!Directory.Exists(path))
                {
                    //If No any such directory then creates the new one
                    Directory.CreateDirectory(path);
                }

                if (maybankrcmsList.Count() != 0)
                {
                    TotalMTDAmount = maybankrcmsList.Sum(s => s.fld_CarumanPekerja);
                    TotalCP38Amount = maybankrcmsList.Sum(s => s.fld_CP38Amount);
                    TotalAllAmount = TotalMTDAmount + TotalCP38Amount;
                    CountMTDData = maybankrcmsList.Where(x => x.fld_CarumanPekerja > 0).Count();
                    CountCP38Data = maybankrcmsList.Where(x => x.fld_CP38Amount > 0).Count();
                    CountAllData = maybankrcmsList.Count();
                }

                using (StreamWriter writer = new StreamWriter(filecreation, true))
                {
                    //header
                    int HeaderLoop = 41;
                    ArrayList Header = new ArrayList();
                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == 0)
                        {
                            Header.Insert(i, "00|");
                        }
                        else if (i == 1)
                        {
                            Header.Insert(i, "Statutory Body|");
                        }
                        else if (i == 2) //Statutory body type
                        {
                            Header.Insert(i, "4|");
                        }
                        else if (i == 3) //payment indicator
                        {
                            Header.Insert(i, "02|");
                        }
                        else if (i == 4) //Employer Reference No.
                        {
                            Header.Insert(i, EmployerTaxNo + "|");
                        }
                        else if (i == 5) //Employer Name
                        {
                            Header.Insert(i, tbl_Ladang.fld_LdgName.ToUpper() + "|");
                        }
                        else if (i == 6) //contribution month year
                        {
                            Header.Insert(i, bulan + tahun + "|");
                        }
                        else if (i == 7) //account no
                        {
                            Header.Insert(i, AccNo + "|");
                        }
                        else if (i == 8) //value date
                        {
                            Header.Insert(i, PaymentDate.ToString("dd") + PaymentDate.ToString("MM") + PaymentDate.ToString("yyyy") + "|");
                        }
                        else if (i == 9) //tax payment option
                        {
                            Header.Insert(i, "2|");
                        }
                        else if (i == 12) //reference
                        {
                            Header.Insert(i, "RCMS Tax " + bulan + "/" + tahun + "|");
                        }
                        else if (i == 13) //reference
                        {
                            Header.Insert(i, tbl_Ladang.fld_LdgCode + "-Tax " + bulan + "/" + tahun + "|");
                        }
                        else if (i == 20) //client id
                        {
                            if (filter == "" || filter == null)
                            {
                                Header.Insert(i, ClientID + "|");
                            }
                            else
                            {
                                Header.Insert(i, filter + "|");
                            }
                        }
                        else if (i == 21) //corporate id
                        {
                            Header.Insert(i, CorpID + "|");
                        }
                        else
                        {
                            Header.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == HeaderLoop)
                        {
                            writer.WriteLine(Header[i]);
                        }
                        else
                        {
                            writer.Write(Header[i]);
                        }
                    }

                    //body                
                    foreach (var maybankrcms in maybankrcmsList)
                    {
                        int TaxNoLength = 0;
                        int WorkerNameLength = 0;
                        string WorkerName1 = "";

                        WorkerNameLength = maybankrcms.fld_Nama.Length;
                        if (WorkerNameLength <= 80)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, WorkerNameLength);
                        }

                        //CountryCode = CountryCodeDetail.Where(x => x.fldOptConfFlag2 == maybankrcms.fld_kdrkyt).Select(s => s.fldOptConfValue).FirstOrDefault();

                        //***Hashing***
                        TaxNoLength = maybankrcms.fld_TaxNo.Length;

                        if (TaxNoLength < 11 || maybankrcms.fld_TaxNo == null || maybankrcms.fld_TaxNo == "")
                        {
                            WorkerTaxNo = "IG00000000000";
                        }
                        else
                        {
                            WorkerTaxNo = maybankrcms.fld_TaxNo;
                        }

                        TaxNoFirst8Digit = Int64.Parse(WorkerTaxNo.Substring(2, 8));
                        TaxNoLast2Digit = Int64.Parse(WorkerTaxNo.Substring(WorkerTaxNo.Length - 2, 2));

                        WifeCodeInt = Int32.Parse(maybankrcms.fld_WifeCode);

                        MTDAmountInt = (int)(maybankrcms.fld_CarumanPekerja * 100);
                        CP38AmountInt = (int)(maybankrcms.fld_CP38Amount * 100);

                        //**TotalHash***
                        TotalHash = TaxNoFirst8Digit + TaxNoLast2Digit + WifeCodeInt + MTDAmountInt + CP38AmountInt;

                        //if (TotalHash < 0)
                        //{
                        //    int s = 0;
                        //}

                        SumAllTotalHash = SumAllTotalHash + TotalHash;

                        //if(SumAllTotalHash < 0)
                        //{
                        //    int s = 0;
                        //}

                        //start write body
                        int BodyLoop = 136;
                        ArrayList Body = new ArrayList();
                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == 0) //1
                            {
                                Body.Insert(i, "01|");
                            }
                            else if (i == 2) //3
                            {
                                Body.Insert(i, "092|");
                            }
                            else if (i == 4)
                            {
                                Body.Insert(i, WorkerName1.ToUpper() + "|");
                            }
                            else if (i == 5)
                            {
                                if (maybankrcms.fld_CountryCode == "MY")
                                {
                                    Body.Insert(i, "Y|");
                                }
                                else
                                {
                                    Body.Insert(i, "N|");
                                }

                            }
                            else if (i == 6)//country code
                            {
                                Body.Insert(i, maybankrcms.fld_CountryCode + "|");
                            }
                            else if (i == 7)
                            {
                                Body.Insert(i, WorkerTaxNo + "|");
                            }
                            else if (i == 8) //nokp
                            {
                                Body.Insert(i, maybankrcms.fld_Nokp + "|");
                            }
                            else if (i == 10) //passport
                            {
                                Body.Insert(i, maybankrcms.fld_PassportNo + "|");
                            }
                            else if (i == 11)
                            {
                                Body.Insert(i, maybankrcms.fld_CarumanPekerja + "|");
                            }
                            else if (i == 12) //11
                            {
                                Body.Insert(i, maybankrcms.fld_CP38Amount + "|");
                            }
                            else if (i == 13) //wife code
                            {
                                Body.Insert(i, maybankrcms.fld_WifeCode + "|");
                            }
                            else if (i == 14) //worker no
                            {
                                Body.Insert(i, maybankrcms.fld_Nopkj + "|");
                            }
                            else
                            {
                                Body.Insert(i, "|");
                            }
                        }

                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == BodyLoop)
                            {
                                writer.WriteLine(Body[i]);
                            }
                            else
                            {
                                writer.Write(Body[i]);
                            }
                        }
                        rowno++;
                    }//close foreach

                    //footer
                    int FooterLoop = 28;
                    ArrayList Footer = new ArrayList();
                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == 0)//1
                        {
                            Footer.Insert(i, "99|");
                        }
                        else if (i == 1)//2
                        {
                            Footer.Insert(i, TotalMTDAmount + "|");
                        }
                        else if (i == 2)//3
                        {
                            Footer.Insert(i, TotalCP38Amount + "|");
                        }
                        else if (i == 3)//4
                        {
                            Footer.Insert(i, CountMTDData + "|");
                        }
                        else if (i == 4)//5
                        {
                            Footer.Insert(i, CountCP38Data + "|");
                        }
                        else if (i == 5)//6
                        {
                            Footer.Insert(i, CountAllData + "|");
                        }
                        else if (i == 6)//7
                        {
                            Footer.Insert(i, TotalAllAmount + "|");
                        }
                        else if (i == 7)//8
                        {
                            Footer.Insert(i, SumAllTotalHash + "|");
                        }
                        else
                        {
                            Footer.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == FooterLoop)
                        {
                            writer.WriteLine(Footer[i]);
                        }
                        else
                        {
                            writer.Write(Footer[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                //msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = ex.Message;
            }
            return filePath;
        }

        public static string GenerateFileMaybankKwsp(List<sp_MaybankRcmsKwsp_Result> maybankrcmsList, tbl_Ladang tbl_Ladang, string bulan, string tahun, int? NegaraID, int? SyarikatID, int? Region, int? Estate, string filter, DateTime PaymentDate, out string filename)
        {
            decimal? TotalEmployeeCont = 0;
            decimal? TotalEmployerCont = 0;
            decimal? TotalAllAmount = 0;
            int CountAllData = 0;
            int CountEmployeeContData = 0;
            int CountEmployerContData = 0;
            int rowno = 1;
            int AmountHash = 0;
            int AccountHash = 0;
            long TotalRowHash = 0;
            long SumAllTotalHash = 0;
            long TaxNoFirst8Digit = 0;
            long TaxNoLast2Digit = 0;
            int WifeCodeInt = 0;
            string statusmsg = "";
            string CorpID = "";
            string ClientID = "";
            string AccNo = "";
            string InitialName = "";
            string AccNoWorker = "";
            string ClientIDText = "";
            string EmployerKwspNo = "";
            string day = PaymentDate.ToString("dd");
            string month = PaymentDate.ToString("MM");
            string year = PaymentDate.ToString("yyyy");
            string WorkerTaxNo = "";
            int SixDigitAccNoInt = 0;
            int reminder = 0;
            char onechar;
            int onedigit = 0;
            DateTime? PaymentDateFormat = new DateTime(PaymentDate.Year, PaymentDate.Month, PaymentDate.Day);

            GetNSWL.GetSyarikatRCMSDetail(Region, Estate, out CorpID, out ClientID, out AccNo, out InitialName);
            string filePath = "~/MaybankFile/" + tahun + "/" + bulan + "/" + NegaraID.ToString() + "_" + SyarikatID.ToString() + "/" /*+ WilayahID.ToString() + "/"*/;
            string path = HttpContext.Current.Server.MapPath(filePath);

            EmployerKwspNo = tbl_Ladang.fld_EmployerEPFNo.ToUpper();
            filename = "M2E EPF (" + tbl_Ladang.fld_LdgCode.ToUpper() + ") " + " " + bulan + tahun + ".txt";
            string filecreation = path + filename;

            try
            {
                TryToDelete(filecreation);
                if (!Directory.Exists(path))
                {
                    //If No any such directory then creates the new one
                    Directory.CreateDirectory(path);
                }

                if (maybankrcmsList.Count() != 0)
                {
                    TotalEmployeeCont = decimal.Truncate(maybankrcmsList.Sum(s => s.fld_KWSPPkj));
                    TotalEmployerCont = decimal.Truncate(maybankrcmsList.Sum(s => s.fld_KWSPMjk));
                    TotalAllAmount = TotalEmployeeCont + TotalEmployerCont;
                    CountEmployeeContData = maybankrcmsList.Where(x => x.fld_KWSPPkj > 0).Count();
                    CountEmployerContData = maybankrcmsList.Where(x => x.fld_KWSPMjk > 0).Count();
                    CountAllData = maybankrcmsList.Count();
                }

                using (StreamWriter writer = new StreamWriter(filecreation, true))
                {
                    //header
                    int HeaderLoop = 41;
                    ArrayList Header = new ArrayList();
                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == 0)
                        {
                            Header.Insert(i, "00|");
                        }
                        else if (i == 1)
                        {
                            Header.Insert(i, "Statutory Body|");
                        }
                        else if (i == 2) //Statutory body type
                        {
                            Header.Insert(i, "5|");
                        }
                        else if (i == 3) //payment indicator
                        {
                            Header.Insert(i, "02|");
                        }
                        else if (i == 4) //Employer Reference No.
                        {
                            Header.Insert(i, EmployerKwspNo + "|");
                        }
                        else if (i == 5) //Employer Name
                        {
                            Header.Insert(i, tbl_Ladang.fld_LdgName.ToUpper() + "|");
                        }
                        else if (i == 6) //contribution month year
                        {
                            Header.Insert(i, bulan + tahun + "|");
                        }
                        else if (i == 7) //account no
                        {
                            Header.Insert(i, AccNo + "|");
                        }
                        else if (i == 8) //value date
                        {
                            Header.Insert(i, PaymentDate.ToString("dd") + PaymentDate.ToString("MM") + PaymentDate.ToString("yyyy") + "|");
                        }
                        else if (i == 10) //payment option
                        {
                            Header.Insert(i, "A|");
                        }
                        else if (i == 11) //company state code
                        {
                            Header.Insert(i, "14|");
                        }
                        else if (i == 12) //reference
                        {
                            Header.Insert(i, "RCMS EPF " + bulan + "/" + tahun + "|");
                        }
                        else if (i == 13) //reference
                        {
                            Header.Insert(i, tbl_Ladang.fld_LdgCode + "-EPF " + bulan + "/" + tahun + "|");
                        }
                        else if (i == 16) //contact person name
                        {
                            Header.Insert(i, "NURUL AINI BINTI BAHARUDDIN|");
                        }
                        else if (i == 17) //contact phone number
                        {
                            Header.Insert(i, "03-27890334|");
                        }
                        else if (i == 20) //client id
                        {
                            if (filter == "" || filter == null)
                            {
                                Header.Insert(i, ClientID + "|");
                            }
                            else
                            {
                                Header.Insert(i, filter + "|");
                            }
                        }
                        else if (i == 21) //corporate id
                        {
                            Header.Insert(i, CorpID + "|");
                        }
                        else
                        {
                            Header.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == HeaderLoop)
                        {
                            writer.WriteLine(Header[i]);
                        }
                        else
                        {
                            writer.Write(Header[i]);
                        }
                    }

                    //body                
                    foreach (var maybankrcms in maybankrcmsList)
                    {
                        //int TaxNoLength = 0;
                        int WorkerNameLength = 0;
                        string WorkerName1 = "";

                        WorkerNameLength = maybankrcms.fld_Nama.Length;
                        if (WorkerNameLength <= 80)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, WorkerNameLength);
                        }

                        //CountryCode = CountryCodeDetail.Where(x => x.fldOptConfFlag2 == maybankrcms.fld_kdrkyt).Select(s => s.fldOptConfValue).FirstOrDefault();

                        //***Amount Hashing***
                        AmountHash = (((int)(maybankrcms.fld_KWSPPkj * 100) + (int)(maybankrcms.fld_KWSPMjk * 100)) % 2000) + rowno;


                        //***Employee Reference No / Hashing***
                        AccountHash = 0;
                        AccNoWorker = maybankrcms.fld_Nokwsp;
                        if (AccNoWorker == "" || AccNoWorker == null) //space (ASCII) = 32
                        {
                            AccountHash = ((32 * 6) * 2) + rowno;
                        }
                        else if (AccNoWorker == "0")
                        {
                            AccountHash = ((0 * 6) * 2) + rowno;
                        }
                        else
                        {
                            if (AccNoWorker.Length < 6)
                            {
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                            }
                            else
                            {
                                AccNoWorker = AccNoWorker.Substring(AccNoWorker.Length - 6, 6);
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                            }
                        }


                        //**Total Row Hash***
                        TotalRowHash = AmountHash + AccountHash;

                        SumAllTotalHash = SumAllTotalHash + TotalRowHash;

                        //start write body
                        int BodyLoop = 136;
                        ArrayList Body = new ArrayList();
                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == 0) //1
                            {
                                Body.Insert(i, "01|");
                            }
                            else if (i == 4)
                            {
                                Body.Insert(i, WorkerName1.ToUpper() + "|");
                            }
                            else if (i == 5)
                            {
                                if (maybankrcms.fld_Kdrkyt == "MA")
                                {
                                    Body.Insert(i, "Y|");
                                }
                                else
                                {
                                    Body.Insert(i, "N|");
                                }
                            }
                            else if (i == 6)//country code
                            {
                                if (maybankrcms.fld_Kdrkyt != "MA")
                                {
                                    //Body.Insert(i, maybankrcms.fld_CountryCode + "|");
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, "|");
                                }
                            }
                            else if (i == 7)
                            {
                                Body.Insert(i, maybankrcms.fld_Nokwsp + "|");
                            }
                            else if (i == 8) //nokp
                            {
                                Body.Insert(i, maybankrcms.fld_Nokp + "|");
                            }
                            else if (i == 11)
                            {
                                //Body.Insert(i, decimal.Truncate(maybankrcms.fld_KWSPPkj) + "|");
                                Body.Insert(i, Math.Floor(maybankrcms.fld_KWSPPkj).ToString("0.00") + "|");
                            }
                            else if (i == 12) //11
                            {
                                Body.Insert(i, Math.Floor(maybankrcms.fld_KWSPMjk).ToString("0.00") + "|");
                            }
                            else if (i == 14) //worker no
                            {
                                Body.Insert(i, maybankrcms.fld_Nopkj + "|");
                            }
                            else if (i == 15) //gross wages
                            {
                                Body.Insert(i, Math.Floor(maybankrcms.fld_GajiKasar).ToString("0.00") + "|");
                            }
                            else if (i == 16) //worker no
                            {
                                Body.Insert(i, maybankrcms.fld_Nopkj + "|");
                            }
                            else
                            {
                                Body.Insert(i, "|");
                            }
                        }

                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == BodyLoop)
                            {
                                writer.WriteLine(Body[i]);
                            }
                            else
                            {
                                writer.Write(Body[i]);
                            }
                        }
                        rowno++;
                    }//close foreach

                    //footer
                    int FooterLoop = 28;
                    ArrayList Footer = new ArrayList();
                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == 0)//1
                        {
                            Footer.Insert(i, "99|");
                        }
                        else if (i == 1)//2
                        {
                            Footer.Insert(i, TotalEmployeeCont + "|");
                        }
                        else if (i == 2)//3
                        {
                            Footer.Insert(i, TotalEmployerCont + "|");
                        }
                        else if (i == 3)//4
                        {
                            Footer.Insert(i, CountEmployeeContData + "|");
                        }
                        else if (i == 4)//5
                        {
                            Footer.Insert(i, CountEmployerContData + "|");
                        }
                        else if (i == 5)//6
                        {
                            Footer.Insert(i, CountAllData + "|");
                        }
                        else if (i == 6)//7
                        {
                            Footer.Insert(i, TotalAllAmount + "|");
                        }
                        else if (i == 7)//8
                        {
                            Footer.Insert(i, SumAllTotalHash + "|");
                        }
                        else
                        {
                            Footer.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == FooterLoop)
                        {
                            writer.WriteLine(Footer[i]);
                        }
                        else
                        {
                            writer.Write(Footer[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                //msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = ex.Message;
            }
            return filePath;
        }

        public static string GenerateFileMaybankSocso(List<sp_MaybankRcmsSocso_Result> maybankrcmsList, tbl_Ladang tbl_Ladang, string bulan, string tahun, int? NegaraID, int? SyarikatID, int? Region, int? Estate, string filter, DateTime PaymentDate, out string filename)
        {
            decimal? TotalEmployeeCont = 0;
            decimal? TotalEmployerCont = 0;
            decimal? TotalAllAmount = 0;
            int CountAllData = 0;
            int CountEmployeeContData = 0;
            int CountEmployerContData = 0;
            int rowno = 1;
            int AmountHash = 0;
            int AccountHash = 0;
            long TotalRowHash = 0;
            long SumAllTotalHash = 0;
            int WifeCodeInt = 0;
            string statusmsg = "";
            string CorpID = "";
            string ClientID = "";
            string AccNo = "";
            string InitialName = "";
            string AccNoWorker = "";
            string ClientIDText = "";
            string EmployerSocsoNo = "";
            string day = PaymentDate.ToString("dd");
            string month = PaymentDate.ToString("MM");
            string year = PaymentDate.ToString("yyyy");
            int SixDigitAccNoInt = 0;
            int reminder = 0;
            char onechar;
            int onedigit = 0;
            DateTime? PaymentDateFormat = new DateTime(PaymentDate.Year, PaymentDate.Month, PaymentDate.Day);

            GetNSWL.GetSyarikatRCMSDetail(Region, Estate, out CorpID, out ClientID, out AccNo, out InitialName);
            string filePath = "~/MaybankFile/" + tahun + "/" + bulan + "/" + NegaraID.ToString() + "_" + SyarikatID.ToString() + "/" /*+ WilayahID.ToString() + "/"*/;
            string path = HttpContext.Current.Server.MapPath(filePath);

            EmployerSocsoNo = tbl_Ladang.fld_EmployerSocsoNo.ToUpper();
            filename = "M2E SOCSO (" + tbl_Ladang.fld_LdgCode.ToUpper() + ") " + " " + bulan + tahun + ".txt";
            string filecreation = path + filename;

            try
            {
                TryToDelete(filecreation);
                if (!Directory.Exists(path))
                {
                    //If No any such directory then creates the new one
                    Directory.CreateDirectory(path);
                }

                if (maybankrcmsList.Count() != 0)
                {
                    TotalEmployeeCont = maybankrcmsList.Sum(s => s.fld_SocsoPkj);
                    TotalEmployerCont = maybankrcmsList.Sum(s => s.fld_SocsoMjk);
                    TotalAllAmount = TotalEmployeeCont + TotalEmployerCont;
                    CountEmployeeContData = maybankrcmsList.Where(x => x.fld_SocsoMjk > 0).Count(); //count caruman majikan sbb caruman majikan adalah wajib
                    CountEmployerContData = maybankrcmsList.Where(x => x.fld_SocsoMjk > 0).Count();
                    CountAllData = maybankrcmsList.Count();
                }

                using (StreamWriter writer = new StreamWriter(filecreation, true))
                {
                    //header
                    int HeaderLoop = 41;
                    ArrayList Header = new ArrayList();
                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == 0)
                        {
                            Header.Insert(i, "00|");
                        }
                        else if (i == 1)
                        {
                            Header.Insert(i, "Statutory Body|");
                        }
                        else if (i == 2) //Statutory body type
                        {
                            Header.Insert(i, "1|");
                        }
                        else if (i == 3) //Payment indicator
                        {
                            Header.Insert(i, "02|");
                        }
                        else if (i == 4) //Employer Reference No.
                        {
                            Header.Insert(i, EmployerSocsoNo + "|");
                        }
                        else if (i == 5) //Employer Name
                        {
                            Header.Insert(i, tbl_Ladang.fld_LdgName.ToUpper() + "|");
                        }
                        else if (i == 6) //contribution month year
                        {
                            Header.Insert(i, bulan + tahun + "|");
                        }
                        else if (i == 7) //account no
                        {
                            Header.Insert(i, AccNo + "|");
                        }
                        else if (i == 8) //value date
                        {
                            Header.Insert(i, PaymentDate.ToString("dd") + PaymentDate.ToString("MM") + PaymentDate.ToString("yyyy") + "|");
                        }
                        else if (i == 12) //reference
                        {
                            Header.Insert(i, "RCMS SOCSO " + bulan + "/" + tahun + "|");
                        }
                        else if (i == 13) //reference
                        {
                            Header.Insert(i, tbl_Ladang.fld_LdgCode + "-SOCSO " + bulan + "/" + tahun + "|");
                        }
                        else if (i == 20) //client id
                        {
                            if (filter == "" || filter == null)
                            {
                                Header.Insert(i, ClientID + "|");
                            }
                            else
                            {
                                Header.Insert(i, filter + "|");
                            }
                        }
                        else if (i == 21) //corporate id
                        {
                            Header.Insert(i, CorpID + "|");
                        }
                        else
                        {
                            Header.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == HeaderLoop)
                        {
                            writer.WriteLine(Header[i]);
                        }
                        else
                        {
                            writer.Write(Header[i]);
                        }
                    }

                    //body                
                    foreach (var maybankrcms in maybankrcmsList)
                    {
                        int WorkerNameLength = 0;
                        string WorkerName1 = "";

                        WorkerNameLength = maybankrcms.fld_Nama.Length;
                        if (WorkerNameLength <= 80)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, WorkerNameLength);
                        }

                        //CountryCode = CountryCodeDetail.Where(x => x.fldOptConfFlag2 == maybankrcms.fld_kdrkyt).Select(s => s.fldOptConfValue).FirstOrDefault();

                        //***Amount Hashing***
                        AmountHash = (((int)(maybankrcms.fld_SocsoPkj * 100) + (int)(maybankrcms.fld_SocsoMjk * 100)) % 2000) + rowno;


                        //***Employee Reference No / Hashing***
                        AccountHash = 0;
                        AccNoWorker = maybankrcms.fld_Noperkeso;
                        if (AccNoWorker == "" || AccNoWorker == null) //space (ASCII) = 32
                        {
                            AccountHash = ((32 * 6) * 2) + rowno;
                        }
                        else if (AccNoWorker == "0")
                        {
                            AccountHash = ((0 * 6) * 2) + rowno;
                        }
                        else
                        {
                            if (AccNoWorker.Length < 6)
                            {
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                            }
                            else
                            {
                                AccNoWorker = AccNoWorker.Substring(AccNoWorker.Length - 6, 6);
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                            }
                        }


                        //**Total Row Hash***
                        TotalRowHash = AmountHash + AccountHash;

                        SumAllTotalHash = SumAllTotalHash + TotalRowHash;

                        //start write body
                        int BodyLoop = 136;
                        ArrayList Body = new ArrayList();
                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == 0) //1
                            {
                                Body.Insert(i, "01|");
                            }
                            else if (i == 4)
                            {
                                Body.Insert(i, WorkerName1.ToUpper() + "|");
                            }
                            //else if (i == 5)
                            //{
                            //    if (maybankrcms.fld_Kdrkyt == "MA")
                            //    {
                            //        Body.Insert(i, "Y|");
                            //    }
                            //    else
                            //    {
                            //        Body.Insert(i, "N|");
                            //    }
                            //}
                            //else if (i == 6)//country code
                            //{
                            //    if (maybankrcms.fld_Kdrkyt != "MA")
                            //    {
                            //        //Body.Insert(i, maybankrcms.fld_CountryCode + "|");
                            //        Body.Insert(i, "|");
                            //    }
                            //    else
                            //    {
                            //        Body.Insert(i, "|");
                            //    }
                            //}
                            else if (i == 7)
                            {
                                Body.Insert(i, maybankrcms.fld_Noperkeso + "|");
                            }
                            else if (i == 8) //nokp
                            {
                                if (maybankrcms.fld_Kdrkyt == "MA")
                                {
                                    Body.Insert(i, maybankrcms.fld_Nokp + "|");
                                }
                                else
                                {
                                    Body.Insert(i, "|");
                                }
                            }
                            else if (i == 10) //passport
                            {
                                if (maybankrcms.fld_Kdrkyt != "MA")
                                {
                                    Body.Insert(i, maybankrcms.fld_Nokp + "|");
                                }
                                else
                                {
                                    Body.Insert(i, "|");
                                }
                            }
                            else if (i == 11)
                            {
                                if (maybankrcms.fld_SocsoPkj == 0)
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, maybankrcms.fld_SocsoPkj + "|");
                                }
                            }
                            else if (i == 12) 
                            {
                                Body.Insert(i, maybankrcms.fld_SocsoMjk + "|");
                            }
                            else if (i == 14)
                            {
                                Body.Insert(i, maybankrcms.fld_Nopkj + "|");
                            }
                            else
                            {
                                Body.Insert(i, "|");
                            }
                        }

                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == BodyLoop)
                            {
                                writer.WriteLine(Body[i]);
                            }
                            else
                            {
                                writer.Write(Body[i]);
                            }
                        }
                        rowno++;
                    }//close foreach

                    //footer
                    int FooterLoop = 28;
                    ArrayList Footer = new ArrayList();
                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == 0)//1
                        {
                            Footer.Insert(i, "99|");
                        }
                        else if (i == 1)//2
                        {
                            Footer.Insert(i, TotalEmployeeCont + "|");
                        }
                        else if (i == 2)//3
                        {
                            Footer.Insert(i, TotalEmployerCont + "|");
                        }
                        else if (i == 3)//4
                        {
                            Footer.Insert(i, CountEmployeeContData + "|");
                        }
                        else if (i == 4)//5
                        {
                            Footer.Insert(i, CountEmployerContData + "|");
                        }
                        else if (i == 5)//6
                        {
                            Footer.Insert(i, CountAllData + "|");
                        }
                        else if (i == 6)//7
                        {
                            Footer.Insert(i, TotalAllAmount + "|");
                        }
                        else if (i == 7)//8
                        {
                            Footer.Insert(i, SumAllTotalHash + "|");
                        }
                        else
                        {
                            Footer.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == FooterLoop)
                        {
                            writer.WriteLine(Footer[i]);
                        }
                        else
                        {
                            writer.Write(Footer[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                //msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = ex.Message;
            }
            return filePath;
        }

        public static string GenerateFileMaybankSip(List<sp_MaybankRcmsSip_Result> maybankrcmsList, tbl_Ladang tbl_Ladang, string bulan, string tahun, int? NegaraID, int? SyarikatID, int? Region, int? Estate, string filter, DateTime PaymentDate, out string filename)
        {
            decimal? TotalEmployeeCont = 0;
            decimal? TotalEmployerCont = 0;
            decimal? TotalAllAmount = 0;
            int CountAllData = 0;
            int CountEmployeeContData = 0;
            int CountEmployerContData = 0;
            int rowno = 1;
            int AmountHash = 0;
            int AccountHash = 0;
            long TotalRowHash = 0;
            long SumAllTotalHash = 0;
            string statusmsg = "";
            string CorpID = "";
            string ClientID = "";
            string AccNo = "";
            string InitialName = "";
            string AccNoWorker = "";
            string ClientIDText = "";
            string EmployerSocsoNo = "";
            string day = PaymentDate.ToString("dd");
            string month = PaymentDate.ToString("MM");
            string year = PaymentDate.ToString("yyyy");
            string WorkerTaxNo = "";
            int SixDigitAccNoInt = 0;
            int reminder = 0;
            char onechar;
            int onedigit = 0;
            DateTime? PaymentDateFormat = new DateTime(PaymentDate.Year, PaymentDate.Month, PaymentDate.Day);

            GetNSWL.GetSyarikatRCMSDetail(Region, Estate, out CorpID, out ClientID, out AccNo, out InitialName);
            string filePath = "~/MaybankFile/" + tahun + "/" + bulan + "/" + NegaraID.ToString() + "_" + SyarikatID.ToString() + "/" /*+ WilayahID.ToString() + "/"*/;
            string path = HttpContext.Current.Server.MapPath(filePath);

            EmployerSocsoNo = tbl_Ladang.fld_EmployerSocsoNo.ToUpper();
            filename = "M2E EIS (" + tbl_Ladang.fld_LdgCode.ToUpper() + ") " + " " + bulan + tahun + ".txt";
            string filecreation = path + filename;

            try
            {
                TryToDelete(filecreation);
                if (!Directory.Exists(path))
                {
                    //If No any such directory then creates the new one
                    Directory.CreateDirectory(path);
                }

                if (maybankrcmsList.Count() != 0)
                {
                    TotalEmployeeCont = maybankrcmsList.Sum(s => s.fld_CarumanPekerja);
                    TotalEmployerCont = maybankrcmsList.Sum(s => s.fld_CarumanMajikan);
                    TotalAllAmount = TotalEmployeeCont + TotalEmployerCont;
                    CountEmployeeContData = maybankrcmsList.Where(x => x.fld_CarumanPekerja > 0).Count();
                    CountEmployerContData = maybankrcmsList.Where(x => x.fld_CarumanMajikan > 0).Count();
                    CountAllData = maybankrcmsList.Count();
                }

                using (StreamWriter writer = new StreamWriter(filecreation, true))
                {
                    //header
                    int HeaderLoop = 41;
                    ArrayList Header = new ArrayList();
                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == 0)
                        {
                            Header.Insert(i, "00|");
                        }
                        else if (i == 1)
                        {
                            Header.Insert(i, "Statutory Body|");
                        }
                        else if (i == 2) //Statutory body type
                        {
                            Header.Insert(i, "9|");
                        }
                        else if (i == 3) //Payment indicator
                        {
                            Header.Insert(i, "02|");
                        }
                        else if (i == 4) //Employer Reference No.
                        {
                            Header.Insert(i, EmployerSocsoNo + "|");
                        }
                        else if (i == 5) //Employer Name
                        {
                            Header.Insert(i, tbl_Ladang.fld_LdgName.ToUpper() + "|");
                        }
                        else if (i == 6) //contribution month year
                        {
                            Header.Insert(i, bulan + tahun + "|");
                        }
                        else if (i == 7) //account no
                        {
                            Header.Insert(i, AccNo + "|");
                        }
                        else if (i == 8) //value date
                        {
                            Header.Insert(i, PaymentDate.ToString("dd") + PaymentDate.ToString("MM") + PaymentDate.ToString("yyyy") + "|");
                        }
                        else if (i == 12) //reference
                        {
                            Header.Insert(i, "RCMS SOCSO " + bulan + "/" + tahun + "|");
                        }
                        else if (i == 13) //reference
                        {
                            Header.Insert(i, tbl_Ladang.fld_LdgCode + "-SOCSO " + bulan + "/" + tahun + "|");
                        }
                        else if (i == 20) //client id
                        {
                            if (filter == "" || filter == null)
                            {
                                Header.Insert(i, ClientID + "|");
                            }
                            else
                            {
                                Header.Insert(i, filter + "|");
                            }
                        }
                        else if (i == 21) //corporate id
                        {
                            Header.Insert(i, CorpID + "|");
                        }
                        else
                        {
                            Header.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == HeaderLoop)
                        {
                            writer.WriteLine(Header[i]);
                        }
                        else
                        {
                            writer.Write(Header[i]);
                        }
                    }

                    //body                
                    foreach (var maybankrcms in maybankrcmsList)
                    {
                        int WorkerNameLength = 0;
                        string WorkerName1 = "";

                        WorkerNameLength = maybankrcms.fld_Nama.Length;
                        if (WorkerNameLength <= 80)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, WorkerNameLength);
                        }

                        //CountryCode = CountryCodeDetail.Where(x => x.fldOptConfFlag2 == maybankrcms.fld_kdrkyt).Select(s => s.fldOptConfValue).FirstOrDefault();

                        //***Amount Hashing***
                        AmountHash = (((int)(maybankrcms.fld_CarumanPekerja * 100) + (int)(maybankrcms.fld_CarumanMajikan * 100)) % 2000) + rowno;


                        //***Employee Reference No / Hashing***
                        AccountHash = 0;
                        AccNoWorker = maybankrcms.fld_Noperkeso;
                        if (AccNoWorker == "" || AccNoWorker == null) //space (ASCII) = 32
                        {
                            AccountHash = ((32 * 6) * 2) + rowno;
                        }
                        else if (AccNoWorker == "0")
                        {
                            AccountHash = ((0 * 6) * 2) + rowno;
                        }
                        else
                        {
                            if (AccNoWorker.Length < 6)
                            {
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                            }
                            else
                            {
                                AccNoWorker = AccNoWorker.Substring(AccNoWorker.Length - 6, 6);
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                            }
                        }


                        //**Total Row Hash***
                        TotalRowHash = AmountHash + AccountHash;

                        SumAllTotalHash = SumAllTotalHash + TotalRowHash;

                        //start write body
                        int BodyLoop = 136;
                        ArrayList Body = new ArrayList();
                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == 0) //1
                            {
                                Body.Insert(i, "01|");
                            }
                            else if (i == 4)
                            {
                                Body.Insert(i, WorkerName1.ToUpper() + "|");
                            }
                            else if (i == 7)
                            {
                                Body.Insert(i, maybankrcms.fld_Noperkeso + "|");
                            }
                            else if (i == 8) //nokp
                            {
                                if (maybankrcms.fld_Kdrkyt == "MA")
                                {
                                    Body.Insert(i, maybankrcms.fld_Nokp + "|");
                                }
                                else
                                {
                                    Body.Insert(i, "|");
                                }
                            }
                            else if (i == 10) //passport
                            {
                                if (maybankrcms.fld_Kdrkyt != "MA")
                                {
                                    Body.Insert(i, maybankrcms.fld_Nokp + "|");
                                }
                                else
                                {
                                    Body.Insert(i, "|");
                                }
                            }
                            else if (i == 11)
                            {
                                Body.Insert(i, maybankrcms.fld_CarumanPekerja + "|");
                            }
                            else if (i == 12) 
                            {
                                if (maybankrcms.fld_CarumanMajikan == 0.00m || maybankrcms.fld_CarumanMajikan == 0)
                                {
                                    Body.Insert(i, "0|");
                                }
                                else
                                {
                                    Body.Insert(i, maybankrcms.fld_CarumanMajikan + "|");
                                }
                            }
                            else if (i == 14)
                            {
                                Body.Insert(i, maybankrcms.fld_Nopkj + "|");
                            }
                            else
                            {
                                Body.Insert(i, "|");
                            }
                        }

                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == BodyLoop)
                            {
                                writer.WriteLine(Body[i]);
                            }
                            else
                            {
                                writer.Write(Body[i]);
                            }
                        }
                        rowno++;
                    }//close foreach

                    //footer
                    int FooterLoop = 28;
                    ArrayList Footer = new ArrayList();
                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == 0)//1
                        {
                            Footer.Insert(i, "99|");
                        }
                        else if (i == 1)//2
                        {
                            Footer.Insert(i, TotalEmployeeCont + "|");
                        }
                        else if (i == 2)//3
                        {
                            Footer.Insert(i, TotalEmployerCont + "|");
                        }
                        else if (i == 3)//4
                        {
                            Footer.Insert(i, CountEmployeeContData + "|");
                        }
                        else if (i == 4)//5
                        {
                            Footer.Insert(i, CountEmployerContData + "|");
                        }
                        else if (i == 5)//6
                        {
                            Footer.Insert(i, CountAllData + "|");
                        }
                        else if (i == 6)//7
                        {
                            Footer.Insert(i, TotalAllAmount + "|");
                        }
                        else if (i == 7)//8
                        {
                            Footer.Insert(i, SumAllTotalHash + "|");
                        }
                        else
                        {
                            Footer.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == FooterLoop)
                        {
                            writer.WriteLine(Footer[i]);
                        }
                        else
                        {
                            writer.Write(Footer[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                //msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = ex.Message;
            }
            return filePath;
        }

    }
}