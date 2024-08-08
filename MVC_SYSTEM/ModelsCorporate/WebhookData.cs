namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WebhookData")]
    public partial class WebhookData
    {
        public long ID { get; set; }

        [StringLength(50)]
        public string Company { get; set; }

        [StringLength(10)]
        public string Event { get; set; }

        [StringLength(20)]
        public string Action { get; set; }

        [Column(TypeName = "text")]
        public string Data { get; set; }
    }
}
