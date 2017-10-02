using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterInventory
{
    public class Cartridge
    {
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Memo { get; set; }
        public string RemoveMemo { get; set; }
        public string Requisitioner { get; set; }
        public string InternalNote { get; set; }
        public string HashID { get; set; }
        public double Price  { get; set; }
        public int Quantity { get; set; }
        public DateTime ReceiveDate { get; set; }
        public DateTime RemoveDate { get; set; }
        public int CartQuantity { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Model, Brand);
        }


        public Cartridge HardCopy()
        {
            return System.MemberwiseClone.Copy(this);
        }
    }

    
}
