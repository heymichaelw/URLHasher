using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace URLHasher.Models
{
    public class Upvote
    {
        public int Id { get; set; }

        public string VoterId { get; set;}
        [ForeignKey("VoterId")]
        public virtual ApplicationUser Voter { get; set; }

        public int VotedURLId { get; set; }
        [ForeignKey("VotedURLId")]
        public virtual URL VotedURL { get; set; }
    }
}