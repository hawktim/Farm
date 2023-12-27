using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmConsoleApp
{
    public class DefaultControler : IController
    {
        private readonly List<IController> _controllers = new List<IController>();
        public string Code => "0";
        public string Caption => "Список действий";

        public DefaultControler()
        {
            _controllers.Add(new ProductControler(this));
            _controllers.Add(new AptekControler(this));
            _controllers.Add(new WarehouseControler(this));
            _controllers.Add(new PartControler(this));
        }
        public string GetConnectionString()
        {
            return @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=farmvmt;Integrated Security=True;Encrypt=False;";
        }
        public T GetController<T>() where T: BaseControler
        {
            return (T)_controllers.FirstOrDefault(x => x is T);
        }

        public FEventArgs GetActions(FEventArgs arg)
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Основное меню");
                foreach (IControlConsole controller in _controllers)
                    Console.WriteLine($"{controller.Code}\t{controller.Caption}");

                Console.WriteLine("Введите код действия или '-' для выхода:");
                string code = Console.ReadLine();
                
                if (code == "-") return new FEventArgs { Code = code }; 

                var first = _controllers.FirstOrDefault(x => x.Code == code);
                first?.GetActions(new FEventArgs { TypeAction = arg.TypeAction });
            }
            while (true);
        }
    }

}
