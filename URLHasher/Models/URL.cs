using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace URLHasher.Models
{
    public class URL
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Long { get; set; }
        public string Short { get; set; }
        public DateTime Created { get; set; }

        public string OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; }
    }
}