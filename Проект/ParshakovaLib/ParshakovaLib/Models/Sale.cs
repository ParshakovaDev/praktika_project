using System;
using System.Collections.Generic;

namespace ParshakovaLib.Models
{
    public partial class Sale
    {
        public int Id { get; set; }
        public int? Partnerid { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; }

        public virtual Partner Partner { get; set; }
    }
}
