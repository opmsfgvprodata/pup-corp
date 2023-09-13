using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MVC_SYSTEM.ModelsSP;
namespace MVC_SYSTEM.ControllersFLAAPI
{
    public class WorkerInfoController : ApiController
    {
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public List<sp_RptMasterDataPkj_Result> Get(int NegaraID, int SyarikatID, int WilayahList, int LadangList, string KerakyatanList, string StatusList, string KategoriPekerjaList, int getuserid)
        {
            List<sp_RptMasterDataPkj_Result> rptMasterDataPkjResults = new List<sp_RptMasterDataPkj_Result>();

            return dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, WilayahList, LadangList,
                    KerakyatanList, StatusList, KategoriPekerjaList, getuserid)
                .ToList();
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}