using System;
using System.Data.SqlClient;
using System.Data;

namespace FarmConsoleApp
{
    public class WarehouseControler : BaseControler
    {
        public WarehouseControler(DefaultControler defaultControler) : base(defaultControler)
        {
            Actions.Add(new ActionHelper(TypeAction.Othes, "Добавить партию", OthesAddParty, "+"));
        }

        public override string Code => "3";
        public override string Caption => "Склад";
        protected override void List(EventArgs args)
        {
            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT [IdWarehouse], [NameWarehouse], NameAptek FROM [dbo].[Warehouses] w INNER JOIN [dbo].[Apteks] a ON w.IdAptek=a.IdAptek ORDER BY NameAptek";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                        Console.WriteLine(reader["IdWarehouse"] + "\t" + reader["NameWarehouse"] + "\t" + reader["NameAptek"]);
                reader.Close();
            }
        }

        protected override void Remove(EventArgs args)
        {
            if (!(GetArgRemove<WarehouseControler>(args) is FEventArgs arg))
                return;

            if (int.TryParse(arg.Select, out var code) && code > 0)
            {
                using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
                {
                    connection.Open();
                    var query = "DELETE FROM [dbo].[Parts] WHERE IdWarehouse IN (SELECT IdWarehouse FROM [dbo].[Warehouses] WHERE IdWarehouse = @IdWarehouse)";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.Add("@IdWarehouse", SqlDbType.Int).Value = code;
                    command.ExecuteScalar();

                    query = "DELETE FROM [dbo].[Warehouses] WHERE IdWarehouse = @IdWarehouse";
                    command = new SqlCommand(query, connection);
                    command.Parameters.Add("@IdWarehouse", SqlDbType.Int).Value = code;
                    command.ExecuteScalar();
                }
            }
        }

        protected override void Add(EventArgs args)
        {
            var nameWarehouse = GetValue("Введите наименование:");
            string idAptek = "0";
            if (args is FEventArgs arg && !string.IsNullOrWhiteSpace(arg.Select))
                idAptek = arg.Select;
            else
                idAptek = GetSelectValue<AptekControler>("Введите код аптеки:");
            

            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO [dbo].[Warehouses] ([NameWarehouse], IdAptek) VALUES (@NameWarehouse, @IdAptek)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@NameWarehouse", SqlDbType.NVarChar, 100).Value = nameWarehouse;
                command.Parameters.Add("@IdAptek", SqlDbType.Int).Value = idAptek;
                command.ExecuteScalar();
            }
        }

        protected override bool CheckRecordCode(string codeValue)
        {
            return CheckRecordCode(codeValue, "IdWarehouse", "[dbo].[Warehouses]");
        }

        protected override void Detail(EventArgs args)
        {
            if (!(args is FEventArgs arg))
                return;
            if (!(int.TryParse(arg.Code, out var code) && code > 0))
                return;

            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT [IdWarehouse], [NameWarehouse], NameAptek FROM [dbo].[Warehouses] w INNER JOIN [dbo].[Apteks] a ON w.IdAptek=a.IdAptek WHERE IdWarehouse=@IdWarehouse ORDER BY NameAptek";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@IdWarehouse", SqlDbType.Int).Value = code;
                var reader = command.ExecuteReader();
                if (reader.Read())
                    Console.WriteLine($"Код: {reader["IdWarehouse"]}\r\nНазвание: {reader["NameWarehouse"]}\r\nАптека: {reader["NameAptek"]}");
                else
                    Console.WriteLine($"Записи с кодом '{code}' не обнаруженно");
            }
        }
        protected void OthesAddParty(EventArgs args)
        {
            if (!(args is FEventArgs arg))
                return;
            if (!(int.TryParse(arg.Select, out var code) && code > 0))
                return;

            var controller = DefaultControler.GetController<PartControler>();
            controller.GetActions(new FEventArgs { Code = "", Select = code.ToString(), TypeAction = TypeAction.Add });

        }
    }
}
