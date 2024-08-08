using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCustom
{
    public class WebhookDTO
    {
        [StringLength(50)]
        public string Company { get; set; }

        [StringLength(10)]
        public string Event { get; set; }

        [StringLength(20)]
        public string Action { get; set; }

        public object Data { get; set; }
    }
}