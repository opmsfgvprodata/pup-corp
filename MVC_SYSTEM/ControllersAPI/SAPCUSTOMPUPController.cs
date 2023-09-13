using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsSAPPUP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MVC_SYSTEM.ControllersAPI
{
    public class SAPCUSTOMPUPController : ApiController
    {
        MVC_SYSTEM_Models_SAPPUP db = new MVC_SYSTEM_Models_SAPPUP();

        [HttpPost]
        public JsonResult<UploadResult> Post(CUSTOMITEM objData)
        {
            errorlog geterror = new errorlog();
            UploadResult UploadResult = new UploadResult();
            SAPPUPMessage returnMessage = new SAPPUPMessage();
            ChangeTimeZone timezone = new ChangeTimeZone();
            SAPPUPConfig sapPupConfig = new SAPPUPConfig();

            var result = "";
            var LogReturn = "";
            var type = "";
            var id = "";
            var number = "";
            var message = "";
            var logNo = "";
            var logMsgNo = "";
            var msg1 = "";
            var msg2 = "PS CUSTOM";
            var msg3 = "";
            var msg4 = "";
            var parameter = "";
            var row = "";
            var field = "";
            var system = returnMessage.SystemName();
            var newRecordCount = 0;
            var updateRecordCount = 0;
            var estateInfo = new ModelsSAPPUP.tbl_Ladang();

            try
            {
                estateInfo =
                    db.tbl_Ladang.SingleOrDefault(x => x.fld_LdgCode == objData.ZBUKRS.ToString() && x.fld_Deleted == false);

                if (estateInfo == null)
                {
                    msg3 = "Unable to find matching company code";
                }

                var customData = db.tbl_SAPCUSTOMPUP.SingleOrDefault(x =>
                    x.fld_NegaraID == estateInfo.fld_NegaraID && x.fld_SyarikatID == estateInfo.fld_SyarikatID &&
                    x.fld_WilayahID == estateInfo.fld_WlyhID && x.fld_LadangID == estateInfo.fld_ID &&
                    x.fld_WilayahCode == objData.ZREGIO && x.fld_PktUtama == objData.ZDIVID &&
                    x.fld_Blok == objData.ZBLKID && x.fld_WBSCode == objData.ZWBSCO);

                sapPupConfig.SaveLog("SAPCUSTOMPUP", JsonConvert.SerializeObject(objData), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WlyhID, estateInfo.fld_ID, "SAP", "Inbound");

                if (customData == null)
                {
                    newRecordCount++;

                    tbl_SAPCUSTOMPUP newSAPCustomPUP = new tbl_SAPCUSTOMPUP();

                    if (!String.IsNullOrEmpty(objData.STATUS))
                    {
                        newSAPCustomPUP.fld_Deleted = true;
                    }

                    else
                    {
                        newSAPCustomPUP.fld_Deleted = false;
                    }

                    newSAPCustomPUP.fld_CompanyCode = objData.ZBUKRS.ToString().Trim();
                    newSAPCustomPUP.fld_WilayahCode = objData.ZREGIO.Trim();
                    newSAPCustomPUP.fld_PktUtama = objData.ZDIVID.Trim();
                    newSAPCustomPUP.fld_Blok = objData.ZBLKID.Trim();
                    newSAPCustomPUP.fld_JnsTnmn = objData.ZCROPT.Trim();
                    newSAPCustomPUP.fld_LsPktUtama = objData.ZHECTA;
                    newSAPCustomPUP.fld_DirianPokok = objData.ZSPHEC;
                    newSAPCustomPUP.fld_WBSCode = objData.ZWBSCO.Trim();
                    newSAPCustomPUP.fld_TahunTnm = objData.ZYEARP;
                    newSAPCustomPUP.fld_StatusTnmn = objData.ZCROPC.Trim();
                    newSAPCustomPUP.fld_LuasBerhasil = objData.ZLSKBH;
                    newSAPCustomPUP.fld_NegaraID = estateInfo.fld_NegaraID;
                    newSAPCustomPUP.fld_SyarikatID = estateInfo.fld_SyarikatID;
                    newSAPCustomPUP.fld_WilayahID = estateInfo.fld_WlyhID;
                    newSAPCustomPUP.fld_LadangID = estateInfo.fld_ID;
                    newSAPCustomPUP.fld_IsSelected = false;
                    newSAPCustomPUP.fld_CreatedBy = "SAP";
                    newSAPCustomPUP.fld_CreatedDT = timezone.gettimezone();
                    newSAPCustomPUP.fld_ModifiedBy = "SAP";
                    newSAPCustomPUP.fld_ModifiedDT = timezone.gettimezone();

                    try
                    {
                        db.tbl_SAPCUSTOMPUP.Add(newSAPCustomPUP);
                        db.SaveChanges();

                        type = returnMessage.SuccessCode();
                        msg1 = returnMessage.CreateDataSuccessMessage(newRecordCount);
                        message = returnMessage.SuccessMessage();
                    }

                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                message = ve.PropertyName + " " + ve.ErrorMessage;
                            }
                        }

                        message = message + message;
                    }
                }

                else
                {
                    updateRecordCount++;

                    customData.fld_LsPktUtama = objData.ZHECTA;
                    customData.fld_DirianPokok = objData.ZSPHEC;
                    customData.fld_StatusTnmn = objData.ZCROPC.Trim();
                    customData.fld_LuasBerhasil = objData.ZLSKBH;
                    customData.fld_ModifiedBy = "SAP";
                    customData.fld_ModifiedDT = timezone.gettimezone();

                    if (!String.IsNullOrEmpty(objData.STATUS))
                    {
                        customData.fld_Deleted = true;
                    }

                    else
                    {
                        customData.fld_Deleted = false;
                    }

                    try
                    {
                        db.SaveChanges();

                        type = returnMessage.SuccessCode();
                        msg1 = returnMessage.UpdateDataSuccessMessage(updateRecordCount);
                        message = returnMessage.SuccessMessage();
                    }

                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                message = ve.PropertyName + " " + ve.ErrorMessage;
                            }
                        }

                        message = message + message;
                    }
                }
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                type = returnMessage.ErrorCode();
                message = ex.Message;
            }

            UploadResult.TYPE = type;
            UploadResult.ID = id;
            UploadResult.NUMBER = number;
            UploadResult.MESSAGE = message;
            UploadResult.LOG_NO = logNo;
            UploadResult.LOG_MSG_NO = logMsgNo;
            UploadResult.MESSAGE_V1 = msg1;
            UploadResult.MESSAGE_V2 = msg2;
            UploadResult.MESSAGE_V3 = msg3;
            UploadResult.MESSAGE_V4 = msg4;
            UploadResult.PARAMETER = parameter;
            UploadResult.ROW = row;
            UploadResult.FIELD = field;
            UploadResult.SYSTEM = system;

            sapPupConfig.SaveLog("SAPCUSTOMPUP", JsonConvert.SerializeObject(UploadResult), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WlyhID, estateInfo.fld_ID, "SAP", "Outbound");

            return Json(UploadResult);
        }
    }
}
