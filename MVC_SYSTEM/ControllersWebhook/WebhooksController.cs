using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MVC_SYSTEM.ControllersWebhook
{
    [Route("api/webhooks")]
    public class WebhooksController : ApiController
    {
        [HttpPost]
        public IHttpActionResult ReceiveWebhook(WebhookDTO webhookDataDTO)
        {
            using (var db = new MVC_SYSTEM_ModelsCorporate())
            {
                var webhookData = new WebhookData
                {
                    Company = webhookDataDTO.Company,
                    Event = webhookDataDTO.Event,
                    Action = webhookDataDTO.Action,
                    Data = webhookDataDTO.Data.ToString()
                };
                db.WebhookDatas.Add(webhookData);
                db.SaveChanges();
            }
            return Ok("Webhook received successfully");
        }
    }
}
