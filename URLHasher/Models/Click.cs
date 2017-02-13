using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace URLHasher.Models
{
    public class Click
    {
        public int Id { get; set; }
        public string Short { get; set; }
        public DateTime Accessed { get; set; }
    }
}