using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connString = "Host=192.168.0.76;Port=26257;Database=bank;Username=root";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS accounts(id INT PRIMARY KEY, balance INT)", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO accounts (id, balance) VALUES (@p, @p1)";
                    cmd.Parameters.AddWithValue("p", 4);
                    cmd.Parameters.AddWithValue("p1", (decimal)1000.0);
                    int result = cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand("SELECT id, balance FROM accounts", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            Console.WriteLine(reader.GetString(0));
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
