using System;
using System.Collections.Generic;

namespace ParshakovaLib.Models
{
    public partial class Partner
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Director { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Legaladdress { get; set; }
        public int? Rating { get; set; }

        public virtual ICollection<Discount> Discount { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }
    }
}
