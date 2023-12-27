using System;
using System.Data.SqlClient;
using System.Data;

namespace FarmConsoleApp
{
    public class PartControler : BaseControler
    {
        public PartControler(DefaultControler defaultControler) : base(defaultControler)
        {
        }

        public override string Code => "4";
        public override string Caption => "Партия";

        protected override bool CheckRecordCode(string codeValue)
        {
            return CheckRecordCode(codeValue, "IdPart", "[dbo].[Parts]");
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
                string query = "SELECT IdPart, p.[NameProduct], w.[NameWarehouse], a.[NameAptek], [Quantity]  FROM [dbo].[Parts] m " +
                    " INNER JOIN [dbo].[Products] p on p.[IdProduct]=m.[IdProduct]" +
                    " INNER JOIN [dbo].[Warehouses] w on w.[IdWarehouse]=m.[IdWarehouse]" +
                    " INNER JOIN [dbo].[Apteks] a on a.[IdAptek]=w.[IdAptek]" +
                    " WHERE IdPart = @IdPart";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@IdPart", SqlDbType.Int).Value = code;
                var reader = command.ExecuteReader();
                if (reader.Read())
                    Console.WriteLine($"Код: {reader["IdPart"]}\r\nТовар: {reader["NameProduct"]}\r\nСклад: {reader["NameWarehouse"]}\r\nАптека: {reader["NameAptek"]}\r\nКол-во: {reader["Quantity"]}");
                else
                    Console.WriteLine($"Записи с кодом '{code}' не обнаруженно");
            }
        }

        protected override void List(EventArgs args)
        {
            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT IdPart, p.[NameProduct], w.[NameWarehouse], a.[NameAptek], [Quantity]  FROM [dbo].[Parts] m " +
                    " INNER JOIN [dbo].[Products] p on p.[IdProduct]=m.[IdProduct]" +
                    " INNER JOIN [dbo].[Warehouses] w on w.[IdWarehouse]=m.[IdWarehouse]" +
                    " INNER JOIN [dbo].[Apteks] a on a.[IdAptek]=w.[IdAptek]";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["IdPart"]}\t{reader["NameProduct"]}\t{reader["NameWarehouse"]}\t{reader["NameAptek"]}\t{reader["Quantity"]}");
                    }
                reader.Close();
            }
        }

        protected override void Remove(EventArgs args)
        {
            if (!(GetArgRemove<PartControler>(args) is FEventArgs arg))
                return;

            if (int.TryParse(arg.Select, out var code) && code > 0)
            {
                using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
                {
                    connection.Open();
                    string query = "DELETE FROM [dbo].[Parts] WHERE IdPart = @IdPart";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@IdPart", SqlDbType.Int).Value = code;
                    command.ExecuteScalar();
                }
            }
        }

        protected override void Add(EventArgs args)
        {
            var idProduct = GetSelectValue<ProductControler>("Необходимо выбрать товар:");

            string idWarehouse = "0";
            if (args is FEventArgs arg && !string.IsNullOrWhiteSpace(arg.Select))
                idWarehouse = arg.Select;
            else
                idWarehouse = GetSelectValue<WarehouseControler>("Необходимо выбрать склад:");

            var sQuantity = GetValue("Введите количество:");
            if (!decimal.TryParse(sQuantity.Replace('.',','), out var quantity)){ quantity = 0; }

            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO [dbo].[Parts] ([IdProduct],[IdWarehouse],[Quantity]) VALUES (@IdProduct,@IdWarehouse,@Quantity)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@IdProduct", SqlDbType.Int).Value = idProduct;
                command.Parameters.Add("@IdWarehouse", SqlDbType.Int).Value = idWarehouse;
                command.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = quantity;
                command.ExecuteScalar();
            }
        }
    }
}
