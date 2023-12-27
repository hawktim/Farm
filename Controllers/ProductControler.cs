using System;
using System.Data;
using System.Data.SqlClient;

namespace FarmConsoleApp
{
    public class ProductControler : BaseControler
    {
        public ProductControler(DefaultControler defaultControler): base(defaultControler)
        {
            
        }
        public override string Code => "1";
        public override string Caption => "Товар";
        protected override void List(EventArgs args)
        {
            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT IdProduct, NameProduct FROM [dbo].[Products]";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader(); 
                if(reader.HasRows)
                while (reader.Read()) 
                { 
                    Console.WriteLine(reader["IdProduct"] + "\t" + reader["NameProduct"]); 
                } 
                reader.Close(); 
            }
        }

        protected override void Remove(EventArgs args)
        {
            if (!(GetArgRemove<ProductControler>(args) is FEventArgs arg))
                return;

            if (int.TryParse(arg.Select, out var code) && code > 0)
            {
                using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
                {
                    connection.Open();
                    var query = "DELETE FROM [dbo].[Products] WHERE IdProduct = @IdProduct";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.Add("@IdProduct", SqlDbType.Int).Value = code;
                    command.ExecuteScalar();

                    query = "DELETE FROM [dbo].[Parts] WHERE IdProduct = @IdProduct";
                    command = new SqlCommand(query, connection);
                    command.Parameters.Add("@IdProduct", SqlDbType.Int).Value = code;
                    command.ExecuteScalar();
                }
            }
        }

        protected override void Add(EventArgs args)
        {
            var nameProductText = GetValue("Введите наименование:");
            using (SqlConnection connection = new SqlConnection(DefaultControler.GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO [dbo].[Products] (NameProduct) VALUES (@NameProduct)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@NameProduct", SqlDbType.NVarChar, 100).Value = $"{nameProductText}";
                command.ExecuteScalar();
            }
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
                string query = "SELECT * FROM [dbo].[Products] WHERE IdProduct = @IdProduct";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@IdProduct", SqlDbType.Int).Value = code;
                var reader = command.ExecuteReader();
                if (reader.Read())
                    Console.WriteLine($"Код: {reader["IdProduct"]}\r\nНазвание: {reader["NameProduct"]}");
                else
                    Console.WriteLine($"Записи с кодом '{code}' не обнаруженно");
            }
        }

        protected override bool CheckRecordCode(string codeValue)
        {
            return CheckRecordCode(codeValue, "IdProduct", "[dbo].[Products]");
        }
    }
}
