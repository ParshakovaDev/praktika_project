using System;
using System.Collections.Generic;

namespace ParshakovaLib.Models
{
    public partial class Discount
    {
        public int Id { get; set; }
        public int? Partnerid { get; set; }
        public double? Percentage { get; set; }
        public virtual Partner Partner { get; set; }
    }
}
