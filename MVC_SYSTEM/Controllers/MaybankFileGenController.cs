﻿using Dapper;
using iTextSharp.text.pdf;
using iTextSharp.text;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsDapper;
using MVC_SYSTEM.ModelsSP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rectangle = iTextSharp.text.Rectangle;
using static System.Net.Mime.MediaTypeNames;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class MaybankFileGenController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        private GetIdentity getidentity = new GetIdentity();
        private GetTriager GetTriager = new GetTriager();
        private GetNSWL GetNSWL = new GetNSWL();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private errorlog geterror = new errorlog();
        private GetConfig GetConfig = new GetConfig();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetWilayah getWilayah = new GetWilayah();
        private Connection Connection = new Connection();
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();
        GetWilayah getwilyah = new GetWilayah();
        // GET: MaybankFileGen
        public ActionResult Index()
        {
            ViewBag.MaybankFileGen = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.MenuSubList = sublist;
            ViewBag.MenuList = new SelectList(dbC.tblMenuLists.Where(x => x.fld_Flag == "m2e" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MenuList, string MenuSubList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (MenuSubList != null)
            {
                if (MenuSubList.Contains("|"))
                {
                    string[] split = MenuSubList.Split('|');
                    return RedirectToAction(split[1], split[0]);
                }
                else
                    return RedirectToAction(MenuSubList, "MaybankFileGen");
            }
            else
            {
                int menulist = int.Parse(MenuList);
                var action = dbC.tblMenuLists.Where(x => x.fld_ID == menulist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "MaybankFileGen");
            }
        }

        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var findsub = dbC.tblMenuLists.Where(x => x.fld_ID == ListID).Select(s => s.fld_Sub).FirstOrDefault();
            List<SelectListItem> sublist = new List<SelectListItem>();
            if (findsub != null)
            {
                sublist = new SelectList(dbC.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }
            db.Dispose();
            return Json(sublist);
        }

        public ActionResult TaxCP39()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int[] wlyhid = new int[] { };
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            int? wilayahselection = 0;
            int? ladangselection = 0;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            return View();
        }

        public ViewResult _TaxCP39(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string NamaSyarikat = "";
            string ClientId = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_TaxCP39_Result> taxCP39 = new List<sp_TaxCP39_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            var syarikat = dbC.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = syarikat.fld_NamaSyarikat;
            ViewBag.NamaPendekSyarikat = syarikat.fld_NamaPndkSyarikat;
            ViewBag.NoSyarikat = syarikat.fld_NoSyarikat;
            var ladang = dbC.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).FirstOrDefault();
            if (ClientId == null || ClientId == "")
            {
                ViewBag.clientid = syarikat.fld_NamaSyarikat + MonthList + YearList;
            }
            else
            {
                ViewBag.clientid = ClientId;
            }

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;

            ViewBag.Description = "Region " + NamaSyarikat + " - CP 39 for " + MonthList + "/" + YearList;

            if (WilayahIDList != null && LadangIDList != null)
            {
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", MonthList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Region", WilayahIDList);
                    parameters.Add("Estate", LadangIDList);
                    con.Open();
                    taxCP39 = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            ViewBag.RecordNo = taxCP39.Count();

            if (taxCP39.Count() == 0)
            {
                ViewBag.Message = GlobalResCorp.msgNoRecord;
            }
            return View(taxCP39);
        }

        public JsonResult TaxCP39Detail(int? Region, int? Estate, int Month, int Year)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            string stringyear = "";
            string stringmonth = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalMTDAmt = 0;
            int TotalMTDRec = 0;
            decimal? TotalCP38Amt = 0;
            int TotalCP38Rec = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_TaxCP39_Result> taxCP39 = new List<sp_TaxCP39_Result>();

            var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            string filename = "Tax CP39 (" + SyarikatDetail.fld_NamaPndkSyarikat.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";
            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Month", Month);
                parameters.Add("Year", Year);
                parameters.Add("Region", Region);
                parameters.Add("Estate", Estate);
                con.Open();
                taxCP39 = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            if (taxCP39.Count() != 0)
            {
                TotalMTDAmt = taxCP39.Sum(s => s.fld_CarumanPekerja);
                TotalMTDRec = taxCP39.Count();
                TotalCP38Amt = taxCP39.Sum(s => s.fld_CP38Amount);
                TotalCP38Rec = taxCP39.Where(x => x.fld_CP38Amount > 0).Count();
                msg = GlobalResCorp.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResCorp.msgDataNotFound;
                statusmsg = "warning";
            }

            dbSP.Dispose();
            dbC.Dispose();
            return Json(new { msg, statusmsg, file = filename, TotalMTDAmt, TotalMTDRec, TotalCP38Amt, TotalCP38Rec });
        }

        [HttpPost]
        public ActionResult DownloadCP39TextFile(int Month, int Year, int? Region, int? Estate)
        {
            string msg = "";
            string statusmsg = "";
            string link = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetGenerateFile getGenerateFile = new GetGenerateFile();
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            var SyarikatDetail = dbC.tbl_Ladang.Where(x => x.fld_ID == Estate).FirstOrDefault();
            List<sp_TaxCP39_Result> taxCP39 = new List<sp_TaxCP39_Result>();
            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Month", Month);
                parameters.Add("Year", Year);
                parameters.Add("Region", Region);
                parameters.Add("Estate", Estate);
                con.Open();
                taxCP39 = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            var TotalMTDAmt = taxCP39.Sum(s => s.fld_CarumanPekerja) * 100;
            var TotalMTDRec = taxCP39.Count();
            var TotalCP38Amt = taxCP39.Sum(s => s.fld_CP38Amount) * 100;
            var TotalCP38Rec = taxCP39.Where(x => x.fld_CP38Amount > 0).Count();
            var TotalMTDAmtStr = TotalMTDAmt.ToString("0");
            var TotalMTDRecStr = TotalMTDRec.ToString("0");
            var TotalCP38AmtStr = TotalCP38Amt.ToString("0");
            var TotalCP38RecStr = TotalCP38Rec.ToString("0");
            string fileContent = "";

            #region Header
            fileContent = GetGenerateFile.TextFileContent("H", 1, " ", true);
            fileContent += GetGenerateFile.TextFileContent(SyarikatDetail.fld_EmployerTaxNo, 10, "0", true);
            fileContent += GetGenerateFile.TextFileContent(SyarikatDetail.fld_EmployerTaxNo, 10, "0", true);
            fileContent += GetGenerateFile.TextFileContent(Year.ToString(), 4, "0", true);
            fileContent += GetGenerateFile.TextFileContent(Month.ToString(), 2, "0", true);
            fileContent += GetGenerateFile.TextFileContent(TotalMTDAmtStr, 10, "0", true);
            fileContent += GetGenerateFile.TextFileContent(TotalMTDRecStr, 5, "0", true);
            fileContent += GetGenerateFile.TextFileContent(TotalCP38AmtStr, 10, "0", true);
            fileContent += GetGenerateFile.TextFileContent(TotalCP38RecStr, 5, "0", true);
            fileContent += Environment.NewLine;
            #endregion Header

            #region Body
            foreach (var item in taxCP39)
            {
                fileContent += GetGenerateFile.TextFileContent("D", 1, " ", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_TaxNo, 10, "0", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_WifeCode, 1, " ", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_Nama, 60, " ", false);
                fileContent += GetGenerateFile.TextFileContent("", 12, " ", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_Nokp, 12, " ", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_PassportNo, 12, " ", false);
                fileContent += GetGenerateFile.TextFileContent(item.fld_CountryCode, 2, " ", true);
                fileContent += GetGenerateFile.TextFileContent((item.fld_CarumanPekerja * 100).ToString("0"), 8, "0", true);
                fileContent += GetGenerateFile.TextFileContent((item.fld_CP38Amount * 100).ToString("0"), 8, "0", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_Nopkj, 10, " ", false);
                if (taxCP39.IndexOf(item) != taxCP39.Count - 1)
                {
                    fileContent += Environment.NewLine;
                }
            }
            #endregion Body
            var filename = SyarikatDetail.fld_EmployerTaxNo + GetGenerateFile.TextFileContent(Month.ToString(), 2, "0", true) + "_" + GetGenerateFile.TextFileContent(Year.ToString(), 4, "0", true) + ".txt";
            var filePath = getGenerateFile.CreateTextFile(filename, fileContent, "CP39");

            link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

            msg = GlobalResCorp.msgGenerateSuccess;
            statusmsg = "success";

            return Json(new { msg, statusmsg, link });
        }

        public ActionResult TaxCP39Form()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int[] wlyhid = new int[] { };
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            int? wilayahselection = 0;
            int? ladangselection = 0;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            return View();
        }

        public FileStreamResult TaxCP39FormPdf(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string NamaSyarikat = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_TaxCP39_Result> taxCP39 = new List<sp_TaxCP39_Result>();

            var monthName = ((Constans.Month)MonthList).ToString().ToUpper();
            var SyarikatDetail = dbC.tbl_Ladang.Where(x => x.fld_ID == LadangIDList).FirstOrDefault();

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;

            ViewBag.Description = "Region " + NamaSyarikat + " - CP 39 for " + MonthList + "/" + YearList;

            if (WilayahIDList != null && LadangIDList != null)
            {
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", MonthList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Region", WilayahIDList);
                    parameters.Add("Estate", LadangIDList);
                    con.Open();
                    taxCP39 = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            if (taxCP39.Count() == 0)
            {
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 5);
                MemoryStream ms = new MemoryStream();
                MemoryStream output = new MemoryStream();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                Chunk chunk = new Chunk();
                Paragraph para = new Paragraph();
                pdfDoc.Open();
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
                pdfWriter.CloseStream = false;
                pdfDoc.Close();

                ms.Close();

                byte[] file = ms.ToArray();
                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
            else
            {
                MemoryStream output = new MemoryStream();

                string cp39Form = GetConfig.PdfPathFile("CP39 FORM-1.pdf");

                // open the reader
                PdfReader reader = new PdfReader(cp39Form);
                Rectangle size = reader.GetPageSizeWithRotation(1);
                Document document = new Document(size);

                // open the writer
                MemoryStream ms = new MemoryStream();
                //FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                PdfContentByte cb = writer.DirectContent;

                // front page content
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
                cb.SetColorFill(BaseColor.BLACK);
                cb.SetFontAndSize(bf, 8);

                string text = "";

                cb.BeginText();
                text = monthName; //Month name
                cb.ShowTextAligned(0, text, 358, 506, 0);
                cb.EndText();

                cb.BeginText();
                text = YearList.ToString(); //Year
                cb.ShowTextAligned(0, text, 450, 506, 0);
                cb.EndText();

                var noEmployerTax = SyarikatDetail.fld_EmployerTaxNo;
                char[] noEmployerTaxArr = noEmployerTax.ToCharArray();
                int arrCountNoEmployerTaxArr = noEmployerTaxArr.Count() - 1;
                float noEmployerXPosition = 0;

                for (int i = 0; i <= arrCountNoEmployerTaxArr; i++)
                {
                    try
                    {
                        text = noEmployerTaxArr[i].ToString();
                    }
                    catch (Exception ex)
                    {
                        text = "";
                    }
                    if (text != "")
                    {
                        switch (i)
                        {
                            case 0:
                                noEmployerXPosition = 100;
                                break;
                            case 1:
                                noEmployerXPosition = 113;
                                break;
                            case 2:
                                noEmployerXPosition = 126;
                                break;
                            case 3:
                                noEmployerXPosition = 140;
                                break;
                            case 4:
                                noEmployerXPosition = 152;
                                break;
                            case 5:
                                noEmployerXPosition = 167;
                                break;
                            case 6:
                                noEmployerXPosition = 180;
                                break;
                            case 7:
                                noEmployerXPosition = 192;
                                break;
                            case 8:
                                noEmployerXPosition = 205;
                                break;
                            case 9:
                                noEmployerXPosition = 230;
                                break;
                            case 10:
                                noEmployerXPosition = 243;
                                break;
                        }
                        cb.BeginText();
                        cb.ShowTextAligned(0, text, noEmployerXPosition, 451, 0);
                        cb.EndText();
                    }
                }

                var TotalMTDAmt = taxCP39.Sum(s => s.fld_CarumanPekerja);
                var TotalMTDRec = taxCP39.Count();
                var TotalCP38Amt = taxCP39.Sum(s => s.fld_CP38Amount);
                var TotalCP38Rec = taxCP39.Where(x => x.fld_CP38Amount > 0).Count();

                cb.BeginText();
                text = "RM   " + TotalMTDAmt.ToString("N"); //MTD Amt
                cb.ShowTextAligned(2, text, 413, 450, 0);
                cb.EndText();

                cb.BeginText();
                text = "RM   " + TotalCP38Amt.ToString("N"); //CP38
                cb.ShowTextAligned(2, text, 503, 450, 0);
                cb.EndText();

                cb.BeginText();
                text = TotalMTDRec.ToString(); //MTD Amt
                cb.ShowTextAligned(1, text, 373, 432, 0);
                cb.EndText();

                cb.BeginText();
                text = TotalCP38Rec.ToString(); //CP38
                cb.ShowTextAligned(1, text, 466, 432, 0);
                cb.EndText();

                var totalAmount = TotalMTDAmt + TotalCP38Amt;

                cb.BeginText();
                text = "RM   " + totalAmount.ToString("N"); //Total Amt
                cb.ShowTextAligned(1, text, 446, 409, 0);
                cb.EndText();

                cb.BeginText();
                text = SyarikatDetail.fld_LdgName; //Total Amt
                cb.ShowTextAligned(0, text, 98, 390, 0);
                cb.EndText();

                cb.BeginText();
                text = DateTime.Now.ToString("dd.MM.yyyy"); //Total Amt
                cb.ShowTextAligned(1, text, 446, 337, 0);
                cb.EndText();

                // front page content

                PdfImportedPage page = writer.GetImportedPage(reader, 1);
                cb.AddTemplate(page, 0, 0);

                document.Close();
                writer.Close();
                reader.Close();
                ms.Close();
                byte[] file = ms.ToArray();

                //cp39Form = GetConfig.PdfPathFile("CP39 FORM-2.pdf");

                // open the reader

                size = new Rectangle(792, 612);
                document = new Document(size);

                // open the writer
                ms = new MemoryStream();
                //FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                cb = writer.DirectContent;

                // front page content


                var taxCP39Arr = taxCP39.ToArray();
                var totalPages = Convert.ToInt32(taxCP39Arr.Count() / 24);
                var getModulus = taxCP39Arr.Count() % 24;
                if (getModulus > 0)
                {
                    totalPages += 1;
                }
                var dataIndex = 0;

                for (int i = 0; i < totalPages; i++)
                {
                    document.NewPage();
                    cb = writer.DirectContent;

                    bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
                    cb.SetColorFill(BaseColor.BLACK);
                    cb.SetFontAndSize(bf, 8);

                    PdfPTable mainTable = new PdfPTable(5);
                    Chunk chunk = new Chunk();
                    mainTable.WidthPercentage = 100;
                    float[] widths = new float[] { 0.5f, 0.6f, 1.5f, 1, 0.1f };
                    mainTable.SetWidths(widths);

                    PdfPCell mainCell = new PdfPCell();
                    chunk = new Chunk("No. Rujukan Majikan E", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    mainCell = new PdfPCell(new Phrase(chunk));
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    PdfPTable table = new PdfPTable(12);
                    chunk = new Chunk();
                    //table.WidthPercentage = 5;
                    widths = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
                    table.SetWidths(widths);

                    PdfPCell cell = new PdfPCell();

                    for (int y = 0; y <= arrCountNoEmployerTaxArr; y++)
                    {
                        try
                        {
                            text = noEmployerTaxArr[y].ToString();
                        }
                        catch (Exception ex)
                        {
                            text = "";
                        }
                        chunk = new Chunk(text, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        if (y == 8)
                        {
                            chunk = new Chunk("-", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = 0;
                            table.AddCell(cell);
                        }
                    }
                    mainCell = new PdfPCell(table);
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    mainCell = new PdfPCell(new Phrase(chunk));
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    chunk = new Chunk("Muka Surat", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    mainCell = new PdfPCell(new Phrase(chunk));
                    mainCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    text = (i + 1).ToString();
                    text = GetGenerateFile.TextFileContent(text, 2, "0", true);
                    char[] pageNoArr = text.ToCharArray();

                    table = new PdfPTable(2);
                    chunk = new Chunk();
                    widths = new float[] { 0.1f, 0.1f };
                    table.SetWidths(widths);

                    chunk = new Chunk(pageNoArr[0].ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    chunk = new Chunk(pageNoArr[1].ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    mainCell = new PdfPCell(table);
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Colspan = 5;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    table = new PdfPTable(10);
                    table.SpacingBefore = 10;
                    chunk = new Chunk();
                    table.WidthPercentage = 106;

                    widths = new float[] { 0.5f, 1.5f, 3, 1, 1, 1, 1, 1, 1, 1 };

                    table.SetWidths(widths);

                    cell = new PdfPCell();
                    chunk = new Chunk("BIL.", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. RUJUKAN CUKAI \r\nPENDAPATAN", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NAMA PENUH PEKERJA \r\n(SEPERTI DI KAD PENGENALAN ATAU PASPORT)", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. K/P LAMA", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. K/P BARU", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. \r\nPEKERJA", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("BAGI PEKERJA ASING", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("JUMLAH POTONGAN CUKAI", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. \r\nPASPORT", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("KOD \r\nNEGARA", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("PCB (RM)", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("CP38 (RM)", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    decimal totalPCB = 0;
                    decimal totalCP38 = 0;
                    for (int pageDataIndex = 0; pageDataIndex < 24; pageDataIndex++)
                    {
                        string bil = "";
                        string refTaxNo = "";
                        string workerName = "";
                        string oldIC = "";
                        string newIC = "";
                        string workerNo = "";
                        string passportNo = "";
                        string countryCode = "";
                        string pCB = "";
                        string cP38 = "";
                        var taxCP39Data = new sp_TaxCP39_Result();
                        if (dataIndex < taxCP39Arr.Count())
                        {
                            taxCP39Data = taxCP39Arr[dataIndex];
                            bil = (pageDataIndex + 1).ToString();
                            refTaxNo = taxCP39Data.fld_TaxNo;
                            workerName = taxCP39Data.fld_Nama;
                            newIC = taxCP39Data.fld_Nokp;
                            workerNo = taxCP39Data.fld_Nopkj;
                            passportNo = taxCP39Data.fld_PassportNo;
                            countryCode = taxCP39Data.fld_CountryCode != "MY" ? taxCP39Data.fld_CountryCode : "";
                            pCB = taxCP39Data.fld_CarumanPekerja.ToString("N");
                            cP38 = taxCP39Data.fld_CP38Amount.ToString("N");
                            totalPCB = totalPCB + taxCP39Data.fld_CarumanPekerja;
                            totalCP38 = totalCP38 + taxCP39Data.fld_CP38Amount;
                        }

                        cell = new PdfPCell();
                        chunk = new Chunk(bil, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;

                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(refTaxNo, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(workerName, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(oldIC, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(newIC, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(workerNo, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(passportNo, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(countryCode, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(pCB, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(cP38, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        table.AddCell(cell);
                        dataIndex++;
                    }
                    cell = new PdfPCell();
                    chunk = new Chunk("Borang CP39 boleh diperolehi di laman web : http://www.hasil.gov.my", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Border = 0;
                    cell.Colspan = 6;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("JUMLAH", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Colspan = 2;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk(totalPCB.ToString("N"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk(totalCP38.ToString("N"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Colspan = 6;
                    cell.Border = 0;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("JUMLAH BESAR", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Colspan = 2;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk(totalAmount.ToString("N"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Colspan = 2;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    mainCell = new PdfPCell(table);
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Colspan = 5;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    document.Add(mainTable);
                }

                // front page content

                document.Close();
                writer.Close();
                reader.Close();
                ms.Close();
                byte[] file2 = ms.ToArray();

                //I don't have a web server handy so I'm going to write my final MemoryStream to a byte array and then to disk
                byte[] bytes;


                //Create our final combined MemoryStream
                using (MemoryStream finalStream = new MemoryStream())
                {
                    //Create our copy object
                    PdfCopyFields copy = new PdfCopyFields(finalStream);

                    copy.AddDocument(new PdfReader(file));

                    copy.AddDocument(new PdfReader(file2));
                    copy.Close();

                    //Get the raw bytes to save to disk
                    bytes = finalStream.ToArray();
                }
                output.Write(bytes, 0, bytes.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");

            }
        }

        public FileResult Download(string filePath, string filename)
        {
            string path = HttpContext.Server.MapPath(filePath);

            DownloadFiles.FileDownloads objs = new DownloadFiles.FileDownloads();

            var filesCol = objs.GetFiles(path);
            var CurrentFileName = filesCol.Where(x => x.FileName == filename).FirstOrDefault();

            string contentType = string.Empty;
            contentType = "application/txt";
            return File(CurrentFileName.FilePath, contentType, CurrentFileName.FileName);

        }

        public ActionResult RcmsGenTax()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int[] wlyhid = new int[] { };
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;
            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ActionResult RcmsGenKwsp()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int[] wlyhid = new int[] { };
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;
            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ActionResult RcmsGenSocso()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int[] wlyhid = new int[] { };
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;
            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ActionResult RcmsGenSip()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int[] wlyhid = new int[] { };
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;
            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(dbC.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(dbC.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ViewResult _rcmstax(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_TaxCP39_Result> maybankrcmsList = new List<sp_TaxCP39_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            //ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
            //   .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
            //   .Select(s => s.fld_NamaPndkSyarikat)
            //   .FirstOrDefault();
            //ViewBag.NoSyarikat = dbC.tbl_Syarikat
            //    .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
            //    .Select(s => s.fld_NoSyarikat)
            //    .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ViewBag.ClientId = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_ClientBatchID)
                .FirstOrDefault();

            //if (filter != "")
            //{
            //    ViewBag.clientid = filter;
            //}
            //else
            //{
            //    if (ClientId == null || ClientId == "")
            //    {
            //        if (WilayahIDList == 1)
            //        {
            //            ViewBag.clientid = "PUP" + MonthList + YearList;
            //        }

            //        if (WilayahIDList == 2)
            //        {
            //            ViewBag.clientid = "YM" + MonthList + YearList;
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.clientid = ClientId;
            //    }
            //}

            ViewBag.AccNo = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_NoAcc)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.WilayahID = WilayahIDList;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.Description = "Region " + NamaSyarikat + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null)
            {
                ViewBag.Message = "Please select month, year and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", MonthList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Region", WilayahIDList);
                    parameters.Add("Estate", LadangIDList);
                    con.Open();
                    maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
                ViewBag.RecordNo = maybankrcmsList.Count();

                if (maybankrcmsList.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(maybankrcmsList);
            }
        }

        public ViewResult _rcmskwsp(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsKwsp_Result> maybankrcmsList = new List<sp_MaybankRcmsKwsp_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            //ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
            //   .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
            //   .Select(s => s.fld_NamaPndkSyarikat)
            //   .FirstOrDefault();
            //ViewBag.NoSyarikat = dbC.tbl_Syarikat
            //    .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
            //    .Select(s => s.fld_NoSyarikat)
            //    .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ViewBag.ClientId = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_ClientBatchID)
                .FirstOrDefault();

            //if (filter != "")
            //{
            //    ViewBag.clientid = filter;
            //}
            //else
            //{
            //    if (ClientId == null || ClientId == "")
            //    {
            //        if (WilayahIDList == 1)
            //        {
            //            ViewBag.clientid = "PUP" + MonthList + YearList;
            //        }

            //        if (WilayahIDList == 2)
            //        {
            //            ViewBag.clientid = "YM" + MonthList + YearList;
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.clientid = ClientId;
            //    }
            //}

            ViewBag.AccNo = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_NoAcc)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.WilayahID = WilayahIDList;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.Description = "Region " + NamaSyarikat + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null)
            {
                ViewBag.Message = "Please select month, year and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", MonthList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Region", WilayahIDList);
                    parameters.Add("Estate", LadangIDList);
                    con.Open();
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsKwsp_Result>(con, "sp_MaybankRcmsKwsp", parameters).ToList();
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
                ViewBag.RecordNo = maybankrcmsList.Count();

                if (maybankrcmsList.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(maybankrcmsList);
            }
        }

        public ViewResult _rcmssocso(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsSocso_Result> maybankrcmsList = new List<sp_MaybankRcmsSocso_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            //ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
            //   .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
            //   .Select(s => s.fld_NamaPndkSyarikat)
            //   .FirstOrDefault();
            //ViewBag.NoSyarikat = dbC.tbl_Syarikat
            //    .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
            //    .Select(s => s.fld_NoSyarikat)
            //    .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ViewBag.ClientId = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_ClientBatchID)
                .FirstOrDefault();

            //if (filter != "")
            //{
            //    ViewBag.clientid = filter;
            //}
            //else
            //{
            //    if (ClientId == null || ClientId == "")
            //    {
            //        if (WilayahIDList == 1)
            //        {
            //            ViewBag.clientid = "PUP" + MonthList + YearList;
            //        }

            //        if (WilayahIDList == 2)
            //        {
            //            ViewBag.clientid = "YM" + MonthList + YearList;
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.clientid = ClientId;
            //    }
            //}

            ViewBag.AccNo = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_NoAcc)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.WilayahID = WilayahIDList;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.Description = "Region " + NamaSyarikat + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null)
            {
                ViewBag.Message = "Please select month, year and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", MonthList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Region", WilayahIDList);
                    parameters.Add("Estate", LadangIDList);
                    con.Open();
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSocso_Result>(con, "sp_MaybankRcmsSocso", parameters).ToList();
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
                ViewBag.RecordNo = maybankrcmsList.Count();

                if (maybankrcmsList.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(maybankrcmsList);
            }
        }

        public ViewResult _rcmssip(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsSip_Result> maybankrcmsList = new List<sp_MaybankRcmsSip_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            //ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
            //   .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
            //   .Select(s => s.fld_NamaPndkSyarikat)
            //   .FirstOrDefault();
            //ViewBag.NoSyarikat = dbC.tbl_Syarikat
            //    .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
            //    .Select(s => s.fld_NoSyarikat)
            //    .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ViewBag.ClientId = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_ClientBatchID)
                .FirstOrDefault();

            //if (filter != "")
            //{
            //    ViewBag.clientid = filter;
            //}
            //else
            //{
            //    if (ClientId == null || ClientId == "")
            //    {
            //        if (WilayahIDList == 1)
            //        {
            //            ViewBag.clientid = "PUP" + MonthList + YearList;
            //        }

            //        if (WilayahIDList == 2)
            //        {
            //            ViewBag.clientid = "YM" + MonthList + YearList;
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.clientid = ClientId;
            //    }
            //}

            ViewBag.AccNo = dbC.tbl_Ladang
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList)
                .Select(s => s.fld_NoAcc)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.WilayahID = WilayahIDList;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.Description = "Region " + NamaSyarikat + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null)
            {
                ViewBag.Message = "Please select month, year and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", MonthList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Region", WilayahIDList);
                    parameters.Add("Estate", LadangIDList);
                    con.Open();
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSip_Result>(con, "sp_MaybankRcmsSip", parameters).ToList();
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
                ViewBag.RecordNo = maybankrcmsList.Count();

                if (maybankrcmsList.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(maybankrcmsList);
            }
        }

        public JsonResult CheckGenDataDetailTax(int Month, int Year, int? Region, int? Estate, string[] WorkerId)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            string stringyear = "";
            string stringmonth = "";
            string CorpID = "";
            string ClientID = "";
            string ClientIDText = "";
            string AccNo = "";
            string InitialName = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetNSWL.GetSyarikatRCMSDetail(Region, Estate, out CorpID, out ClientID, out AccNo, out InitialName);

            List<sp_TaxCP39_Result> maybankrcmsList = new List<sp_TaxCP39_Result>();

            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Month", Month);
                parameters.Add("Year", Year);
                parameters.Add("Region", Region);
                parameters.Add("Estate", Estate);
                con.Open();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                }
                else
                {
                    maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            //var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();
            string filename = "M2E TAX (" + InitialName.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            if (ClientID == null || ClientID == "")
            {
                //if (Region == 1)
                //{
                //    ClientIDText = "PUP" + stringmonth + stringyear;
                //}

                //if (Region == 2)
                //{
                //    ClientIDText = "YM" + stringmonth + stringyear;
                //}
                ClientIDText = "";
            }
            else
            {
                ClientIDText = ClientID;
            }

            if (maybankrcmsList.Count() != 0)
            {
                TotalGaji = maybankrcmsList.Sum(s => s.fld_CarumanPekerja);
                CountData = maybankrcmsList.Count();
                msg = GlobalResCorp.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResCorp.msgDataNotFound;
                statusmsg = "warning";
            }

            dbSP.Dispose();
            dbC.Dispose();
            //return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData });

        }

        public JsonResult CheckGenDataDetailKwsp(int Month, int Year, int? Region, int? Estate, string[] WorkerId)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            string stringyear = "";
            string stringmonth = "";
            string CorpID = "";
            string ClientID = "";
            string ClientIDText = "";
            string AccNo = "";
            string InitialName = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetNSWL.GetSyarikatRCMSDetail(Region, Estate, out CorpID, out ClientID, out AccNo, out InitialName);

            List<sp_MaybankRcmsKwsp_Result> maybankrcmsList = new List<sp_MaybankRcmsKwsp_Result>();

            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Month", Month);
                parameters.Add("Year", Year);
                parameters.Add("Region", Region);
                parameters.Add("Estate", Estate);
                con.Open();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsKwsp_Result>(con, "sp_MaybankRcmsKwsp", parameters).ToList();
                }
                else
                {
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsKwsp_Result>(con, "sp_MaybankRcmsKwsp", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            //var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();
            string filename = "M2E EPF (" + InitialName.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            if (ClientID == null || ClientID == "")
            {
                //if (Region == 1)
                //{
                //    ClientIDText = "PUP" + stringmonth + stringyear;
                //}

                //if (Region == 2)
                //{
                //    ClientIDText = "YM" + stringmonth + stringyear;
                //}
                ClientIDText = "";
            }
            else
            {
                ClientIDText = ClientID;
            }

            if (maybankrcmsList.Count() != 0)
            {
                TotalGaji = maybankrcmsList.Sum(s => s.fld_KWSPMjk + s.fld_KWSPPkj);
                CountData = maybankrcmsList.Count();
                msg = GlobalResCorp.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResCorp.msgDataNotFound;
                statusmsg = "warning";
            }

            dbSP.Dispose();
            dbC.Dispose();
            //return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData });

        }

        public JsonResult CheckGenDataDetailSocso(int Month, int Year, int? Region, int? Estate, string[] WorkerId)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            string stringyear = "";
            string stringmonth = "";
            string CorpID = "";
            string ClientID = "";
            string ClientIDText = "";
            string AccNo = "";
            string InitialName = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetNSWL.GetSyarikatRCMSDetail(Region, Estate, out CorpID, out ClientID, out AccNo, out InitialName);

            List<sp_MaybankRcmsSocso_Result> maybankrcmsList = new List<sp_MaybankRcmsSocso_Result>();

            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Month", Month);
                parameters.Add("Year", Year);
                parameters.Add("Region", Region);
                parameters.Add("Estate", Estate);
                con.Open();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSocso_Result>(con, "sp_MaybankRcmsSocso", parameters).ToList();
                }
                else
                {
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSocso_Result>(con, "sp_MaybankRcmsSocso", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            //var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();
            string filename = "M2E SOCSO (" + InitialName.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            if (ClientID == null || ClientID == "")
            {
                //if (Region == 1)
                //{
                //    ClientIDText = "PUP" + stringmonth + stringyear;
                //}

                //if (Region == 2)
                //{
                //    ClientIDText = "YM" + stringmonth + stringyear;
                //}
                ClientIDText = "";
            }
            else
            {
                ClientIDText = ClientID;
            }

            if (maybankrcmsList.Count() != 0)
            {
                TotalGaji = maybankrcmsList.Sum(s => s.fld_SocsoMjk + s.fld_SocsoPkj);
                CountData = maybankrcmsList.Count();
                msg = GlobalResCorp.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResCorp.msgDataNotFound;
                statusmsg = "warning";
            }

            dbSP.Dispose();
            dbC.Dispose();
            //return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData });

        }

        public JsonResult CheckGenDataDetailSip(int Month, int Year, int? Region, int? Estate, string[] WorkerId)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            string stringyear = "";
            string stringmonth = "";
            string CorpID = "";
            string ClientID = "";
            string ClientIDText = "";
            string AccNo = "";
            string InitialName = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetNSWL.GetSyarikatRCMSDetail(Region, Estate, out CorpID, out ClientID, out AccNo, out InitialName);

            List<sp_MaybankRcmsSip_Result> maybankrcmsList = new List<sp_MaybankRcmsSip_Result>();

            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Month", Month);
                parameters.Add("Year", Year);
                parameters.Add("Region", Region);
                parameters.Add("Estate", Estate);
                con.Open();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSip_Result>(con, "sp_MaybankRcmsSip", parameters).ToList();
                }
                else
                {
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSip_Result>(con, "sp_MaybankRcmsSip", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            //var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();
            string filename = "M2E EIS (" + InitialName.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            if (ClientID == null || ClientID == "")
            {
                //if (Region == 1)
                //{
                //    ClientIDText = "PUP" + stringmonth + stringyear;
                //}

                //if (Region == 2)
                //{
                //    ClientIDText = "YM" + stringmonth + stringyear;
                //}
                ClientIDText = "";
            }
            else
            {
                ClientIDText = ClientID;
            }

            if (maybankrcmsList.Count() != 0)
            {
                TotalGaji = maybankrcmsList.Sum(s => s.fld_SocsoMjk + s.fld_SocsoPkj);
                CountData = maybankrcmsList.Count();
                msg = GlobalResCorp.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResCorp.msgDataNotFound;
                statusmsg = "warning";
            }

            dbSP.Dispose();
            dbC.Dispose();
            //return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData });

        }

        [HttpPost]
        public ActionResult DownloadTextTax(int Month, int Year, int? Region, int? Estate, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<ModelsCorporate.tblOptionConfigsWeb> CountryCodeDetail = new List<ModelsCorporate.tblOptionConfigsWeb>();

                List<sp_TaxCP39_Result> maybankrcmsList = new List<sp_TaxCP39_Result>();
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", Month);
                    parameters.Add("Year", Year);
                    parameters.Add("Region", Region);
                    parameters.Add("Estate", Estate);
                    con.Open();
                    maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();

                    if (WorkerId == null)
                        WorkerId = new string[] { "0" };

                    if (WorkerId.Contains("0"))
                    {
                        maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                    }
                    else
                    {
                        maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }


                var SyarikatDetail = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == Region && x.fld_ID == Estate).FirstOrDefault();
                //CountryCodeDetail = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlistlhdn").ToList();

                filePath = GetGenerateFile.GenerateFileMaybankTax(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, Region, Estate, filter, PaymentDate, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResCorp.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        [HttpPost]
        public ActionResult DownloadTextKwsp(int Month, int Year, int? Region, int? Estate, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<ModelsCorporate.tblOptionConfigsWeb> CountryCodeDetail = new List<ModelsCorporate.tblOptionConfigsWeb>();

                List<sp_MaybankRcmsKwsp_Result> maybankrcmsList = new List<sp_MaybankRcmsKwsp_Result>();
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", Month);
                    parameters.Add("Year", Year);
                    parameters.Add("Region", Region);
                    parameters.Add("Estate", Estate);
                    con.Open();
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsKwsp_Result>(con, "sp_MaybankRcmsKwsp", parameters).ToList();

                    if (WorkerId == null)
                        WorkerId = new string[] { "0" };

                    if (WorkerId.Contains("0"))
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsKwsp_Result>(con, "sp_MaybankRcmsKwsp", parameters).ToList();
                    }
                    else
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsKwsp_Result>(con, "sp_MaybankRcmsKwsp", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }


                var SyarikatDetail = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == Region && x.fld_ID == Estate).FirstOrDefault();
                //CountryCodeDetail = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlistlhdn").ToList();

                filePath = GetGenerateFile.GenerateFileMaybankKwsp(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, Region, Estate, filter, PaymentDate, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResCorp.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        [HttpPost]
        public ActionResult DownloadTextSocso(int Month, int Year, int? Region, int? Estate, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<ModelsCorporate.tblOptionConfigsWeb> CountryCodeDetail = new List<ModelsCorporate.tblOptionConfigsWeb>();

                List<sp_MaybankRcmsSocso_Result> maybankrcmsList = new List<sp_MaybankRcmsSocso_Result>();
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", Month);
                    parameters.Add("Year", Year);
                    parameters.Add("Region", Region);
                    parameters.Add("Estate", Estate);
                    con.Open();
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSocso_Result>(con, "sp_MaybankRcmsSocso", parameters).ToList();

                    if (WorkerId == null)
                        WorkerId = new string[] { "0" };

                    if (WorkerId.Contains("0"))
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSocso_Result>(con, "sp_MaybankRcmsSocso", parameters).ToList();
                    }
                    else
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSocso_Result>(con, "sp_MaybankRcmsSocso", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }


                var SyarikatDetail = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == Region && x.fld_ID == Estate).FirstOrDefault();
                //CountryCodeDetail = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlistlhdn").ToList();

                filePath = GetGenerateFile.GenerateFileMaybankSocso(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, Region, Estate, filter, PaymentDate, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResCorp.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        [HttpPost]
        public ActionResult DownloadTextSip(int Month, int Year, int? Region, int? Estate, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<ModelsCorporate.tblOptionConfigsWeb> CountryCodeDetail = new List<ModelsCorporate.tblOptionConfigsWeb>();

                List<sp_MaybankRcmsSip_Result> maybankrcmsList = new List<sp_MaybankRcmsSip_Result>();
                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Month", Month);
                    parameters.Add("Year", Year);
                    parameters.Add("Region", Region);
                    parameters.Add("Estate", Estate);
                    con.Open();
                    maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSip_Result>(con, "sp_MaybankRcmsSip", parameters).ToList();

                    if (WorkerId == null)
                        WorkerId = new string[] { "0" };

                    if (WorkerId.Contains("0"))
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSip_Result>(con, "sp_MaybankRcmsSip", parameters).ToList();
                    }
                    else
                    {
                        maybankrcmsList = SqlMapper.Query<sp_MaybankRcmsSip_Result>(con, "sp_MaybankRcmsSip", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }


                var SyarikatDetail = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == Region && x.fld_ID == Estate).FirstOrDefault();
                //CountryCodeDetail = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlistlhdn").ToList();

                filePath = GetGenerateFile.GenerateFileMaybankSip(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, Region, Estate, filter, PaymentDate, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResCorp.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

    }
}