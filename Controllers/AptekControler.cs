using System;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;

namespace FarmConsoleApp
{
    public class AptekControler : BaseControler
    {
        public AptekControler(DefaultControler defaultControler) : base(defaultControler)
        {
            Actions.Add(new ActionHelper(TypeAction.Othes, "Добавить склад", OthesAddWarehouse, "+"));
            Actions.Add(new ActionHelper(TypeAction.Othes, "Список товара в аптеке (на всех складах)", Othes, "="));
        }

        public override string Code => "2";
        public override string Caption => "Аптеки";

        protected override void List(EventArgs args)
        {
            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT IdAptek, NameAptek, Address, Phone FROM [dbo].[Apteks]";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        Console.WriteLine(reader["IdAptek"] + "\t" + reader["NameAptek"] + "\t" + reader["Address"] + "\t" + reader["Phone"]);
                    }
                reader.Close();
            }
        }

        protected override void Remove(EventArgs args)
        {
            if (!(GetArgRemove<AptekControler>(args) is FEventArgs arg))
                return;

            if (int.TryParse(arg.Select, out var code) && code > 0)
            {
                using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
                {
                    connection.Open();
                    var query = "DELETE FROM [dbo].[Parts] WHERE IdWarehouse IN (SELECT IdWarehouse FROM [dbo].[Warehouses] WHERE IdAptek = @IdAptek)";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.Add("@IdAptek", SqlDbType.Int).Value = code;
                    command.ExecuteScalar();
                   
                    query = "DELETE FROM [dbo].[Warehouses] WHERE IdAptek = @IdAptek";
                    command = new SqlCommand(query, connection);
                    command.Parameters.Add("@IdAptek", SqlDbType.Int).Value = code;
                    command.ExecuteScalar();

                    query = "DELETE FROM [dbo].[Apteks] WHERE IdAptek = @IdAptek";
                    command = new SqlCommand(query, connection);
                    command.Parameters.Add("@IdAptek", SqlDbType.Int).Value = code;
                    command.ExecuteScalar();
                }
            }
        }
        
        protected override void Add(EventArgs args)
        {
            var nameAptek = GetValue("Введите наименование:");
            var address = GetValue("Введите адрес:");
            var phone = GetValue("Введите телефон:");

            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO [dbo].[Apteks] (NameAptek, Address, Phone) VALUES (@NameAptek, @Address, @Phone)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@NameAptek", SqlDbType.NVarChar, 100).Value = nameAptek;
                command.Parameters.Add("@Address", SqlDbType.NVarChar, 100).Value = address;
                command.Parameters.Add("@Phone", SqlDbType.NVarChar, 100).Value = phone;
                command.ExecuteScalar();
            }
        }

        protected override bool CheckRecordCode(string codeValue)
        {
            return CheckRecordCode(codeValue, "IdAptek", "[dbo].[Apteks]");
        }

        protected void Othes(EventArgs args)
        {
            if (!(args is FEventArgs arg))
                return;
            if (!(int.TryParse(arg.Select, out var code) && code > 0))
                return;
            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT t.[NameProduct], Sum([Quantity]) as SumQuantity FROM [dbo].[Warehouses] w " +
                    " INNER JOIN [dbo].[Parts] p on w.[IdWarehouse]=p.[IdWarehouse] " +
                    " INNER JOIN [dbo].[Products] t on p.[IdProduct]=t.[IdProduct] " +
                    " WHERE w.[IdAptek]=@IdAptek " +
                    " GROUP BY t.[IdProduct], t.[NameProduct]";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@IdAptek", SqlDbType.Int).Value = code;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    Console.WriteLine(reader["NameProduct"] + "\t" + reader["SumQuantity"]);
                reader.Close();
            }
        }
        protected void OthesAddWarehouse(EventArgs args)
        {
            if (!(args is FEventArgs arg))
                return;
            if (!(int.TryParse(arg.Select, out var code) && code > 0))
                return;

            var controller = DefaultControler.GetController<WarehouseControler>();
            controller.GetActions(new FEventArgs { Code = "", Select = code.ToString(), TypeAction = TypeAction.Add });
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
                string query = "SELECT * FROM [dbo].[Apteks] WHERE IdAptek = @IdAptek";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@IdAptek", SqlDbType.Int).Value = code;
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine($"Код: {reader["IdAptek"]}\r\nНазвание: {reader["NameAptek"]}\r\nАдрес: {reader["Address"]}\r\nТелефон: {reader["Phone"]}");

                }
                else
                    Console.WriteLine($"Записи с кодом '{code}' не обнаруженно");
            }
        }
    }
}
