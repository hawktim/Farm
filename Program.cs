using System;
using System.Text;
using System.Threading.Tasks;

namespace FarmConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IController defaultController = new DefaultControler();
            defaultController.GetActions(new FEventArgs { TypeAction = TypeAction.List });
        }
    }

    public class Product
    {
        public int IdProduct { get; set; }
        public string NameProduct { get; set; }
    }

    public class Aptek
    {
        public string IdAptek { get; set; }
        public string NameAptek { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public class Warehouse
    {
        public int IdWarehouse { get; set; }
        public int IdAptek { get; set; }
        public string NameWarehouse { get; set; }
    }

    public class Partition
    {
        public int IdPartition { get; set; }
        public int IdProduct { get; set; }
        public int IdWarehouse { get; set; }
        public decimal Quantity { get; set; }
    }

}
