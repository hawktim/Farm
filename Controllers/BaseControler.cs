using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.CodeDom;

namespace FarmConsoleApp
{
    public abstract class BaseControler: IController
    {
        protected DefaultControler DefaultControler { get; }
        protected List<ActionHelper> Actions { get; }
        protected BaseControler(DefaultControler defaultControler)
        {
            DefaultControler = defaultControler;
            Actions = new List<ActionHelper>
            {
                new ActionHelper(TypeAction.List, "Список", List),
                new ActionHelper(TypeAction.Add,"Добавить", Add),
                new ActionHelper(TypeAction.Remove,"Удалить", Remove),
                new ActionHelper(TypeAction.Select,"Выбрать", Select),
                new ActionHelper(TypeAction.Detail,"Просмотр", Detail)
            };
        }
        public abstract string Code { get; }
        public abstract string Caption { get; }
        public virtual FEventArgs GetActions(FEventArgs arg)
        {
            var lastAction = arg.TypeAction;
            
            ActionHelper currentAction = Actions.First(x => x.TypeAction == arg.TypeAction);
            string code = "";
            do
            {
                Console.Clear();

                switch (currentAction.TypeAction)
                {
                    case TypeAction.List:
                        if (CheckRecordCode(code))
                        {
                            arg = GetActions(new FEventArgs { Code = code, TypeAction = TypeAction.Detail });
                            code = arg.Code;
                            continue;
                        }

                        Console.WriteLine($"{this.Caption} ({currentAction.Caption})");
                        currentAction.Execute(new FEventArgs { Code = code });
                        
                        foreach (ActionHelper itemAction in Actions.Where(x => x.TypeAction == TypeAction.Add || x.TypeAction == TypeAction.Remove))
                            Console.Write($"{itemAction.Caption}({itemAction.Code})\t");

                        Console.WriteLine("\nВведите код действия или '' для выхода в предыдущее меню:");
                        code = Console.ReadLine();

                        if (code == "") return new FEventArgs { Code = code };

                        var first = Actions.FirstOrDefault(x => x.Code == code.ToLower());
                        if (currentAction.TypeAction == TypeAction.List && (first?.TypeAction == TypeAction.Add || first?.TypeAction == TypeAction.Remove))
                            currentAction = Actions.First(x => x.TypeAction == first.TypeAction);
                        break;

                    case TypeAction.Add:
                    case TypeAction.Remove:
                        Console.WriteLine($"{this.Caption} ({currentAction.Caption})");
                        currentAction.Execute(new FEventArgs { Select = arg.Select, Code = code });
                        currentAction = Actions.First(x => x.TypeAction == TypeAction.List);
                        code = "";
                        continue;

                    case TypeAction.Detail:
                        Console.WriteLine($"{this.Caption} ({currentAction.Caption})");
                        currentAction.Execute(new FEventArgs { Code = arg.Code });
                        foreach (ActionHelper itemAction in Actions.Where(x => x.TypeAction == TypeAction.Remove || x.TypeAction == TypeAction.Othes))
                            Console.Write($"{itemAction.Caption}({itemAction.Code})\t");

                        Console.WriteLine("\nВведите код действия или '' для выхода в предыдущее меню:");
                        code = Console.ReadLine();
                        var firstDetail = Actions.FirstOrDefault(x => x.Code == code.ToLower() && (x.TypeAction == TypeAction.Remove || x.TypeAction == TypeAction.Othes));
                        if (firstDetail!=null)
                        {
                            currentAction = firstDetail;
                            arg = new FEventArgs { Select = arg.Code, TypeAction = TypeAction.Othes };
                            continue;
                        }
                        return new FEventArgs { Code = "", TypeAction = TypeAction.Detail };

                    case TypeAction.Select:
                        Console.WriteLine($"{this.Caption} ({currentAction.Caption})");
                        currentAction.Execute(new FEventArgs { Code = code });
                        Console.WriteLine("\nВведите код строчки или '' для выхода в предыдущее меню:");
                        code = Console.ReadLine();
                        if (CheckRecordCode(code))
                            return new FEventArgs { Select = code };

                        return new FEventArgs { Code = code };

                    default:
                        Console.WriteLine($"{this.Caption} ({currentAction.Caption})");
                        currentAction.Execute(arg);
                        Console.WriteLine("\nВведите код строчки или '' для выхода в предыдущее меню:");
                        code = Console.ReadLine();
                        if (CheckRecordCode(code))
                            return new FEventArgs { Select=code};

                        return new FEventArgs { Code = code };
                }
            }
            while (true);
        }
        protected abstract void Detail(EventArgs args);
        protected abstract void Add(EventArgs args);
        protected abstract void Remove(EventArgs args);
        protected abstract void List(EventArgs args);
        protected virtual void Select(EventArgs args) 
        {
            List(args);
        }
        protected string GetValue(string caption, int lenght = 100)
        {
            string result;
            var i = 0;
            do
            {
                Console.WriteLine(caption);
                result = Console.ReadLine();
                i++;
                if (i > 3) throw new ConsoleException($"Было произведено более 5 проверок {caption}");
            } while (string.IsNullOrEmpty(result) || result.Length > lenght);

            return result;
        }

        protected string GetSelectValue<T>(string caption) where T : BaseControler
        {
            string result = "";
            var i = 0;
            do
            {
                var controller = DefaultControler.GetController<T>();
                var arg = controller.GetActions( new FEventArgs { TypeAction = TypeAction.Select });
                
                result = arg.Select;

                i++;
                if (i > 3) throw new ConsoleException($"Было произведено более 5 проверок {caption}");
            } while (string.IsNullOrEmpty(result));

            return result;
        }
        protected abstract bool CheckRecordCode(string codeValue);
        protected bool CheckRecordCode(string codeValue, string keyField, string tableName)
        {
            if (int.TryParse(codeValue, out var code) && code > -1)
            {
                using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
                {
                    connection.Open();
                    string query = $"SELECT {keyField} FROM {tableName} WHERE {keyField} = @IdValue";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@IdValue", SqlDbType.Int).Value = code;
                    var reader = command.ExecuteReader();
                    reader.Read();
                    return reader.HasRows;
                }
            }
            return false;
        }
        protected FEventArgs GetArgRemove<T>(EventArgs args) where T : BaseControler
        {
            if (args is FEventArgs arg && CheckRecordCode(arg.Select))
                return arg;

            var controler = DefaultControler.GetController<T>();
            return controler.GetActions(new FEventArgs { TypeAction = TypeAction.Select });
        }
    }
}
