using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Dapper;

namespace EF_vs_Dapper_vs_Hibernate
{
    class Dapper
    {
        private string connectionString =
            "data source=(localdb)\\MSSQLLocalDB;Database=DapperDB;Integrated Security=True;";

        public void GenerateData()
        {
            Random r = new Random();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var list = new List<Product>();
                for (int i = 0; i < Program.N; i++)
                {
                    Product product = new Product()
                    {
                        ID = i + 1,
                        Name = "Product" + i,
                        Price = r.Next(0, 10_000)
                    };
                    list.Add(product);
                }
                var sqlQuery = "INSERT INTO Product (Name, Price) VALUES(@Name, @Price)";
                db.Execute(sqlQuery, list);
            }
        }


        public void GetAll()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (var product in db.Query<Product>("SELECT * FROM Product").ToList())
                {
                    var t = product.Name;
                }
            }
        }

        public void GetWithFilter()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (var product in db.Query<Product>("SELECT * FROM Product where Price>5000").ToList())
                {
                    var t = product.Name;
                }
            }
        }

        public void UpdateAll()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "UPDATE Product SET Price = Price * 2 ";
                db.Execute(sqlQuery);
            }
        }

        public void DeleteHalf()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "DELETE FROM Product WHERE Price>5000";
                db.Execute(sqlQuery);
            }
        }

        public void Test()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "DELETE FROM Product";
                db.Execute(sqlQuery);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            GenerateData();

            sw.Stop();
            Console.WriteLine("Dapper Insert {1} = {0}", sw.ElapsedMilliseconds, Program.N);
            sw = new Stopwatch();
            sw.Start();

            GetAll();

            sw.Stop();
            Console.WriteLine("Dapper GetAll = {0}", sw.ElapsedMilliseconds);
            sw = new Stopwatch();
            sw.Start();

            GetWithFilter();

            sw.Stop();
            Console.WriteLine("Dapper GetWithFilter = {0}", sw.ElapsedMilliseconds);
            sw = new Stopwatch();
            sw.Start();

            UpdateAll();

            sw.Stop();
            Console.WriteLine("Dapper UpdateAll = {0}", sw.ElapsedMilliseconds);
            sw = new Stopwatch();
            sw.Start();

            DeleteHalf();

            sw.Stop();
            Console.WriteLine("Dapper DeleteHalf = {0}", sw.ElapsedMilliseconds);
            //sw = new Stopwatch();sw.Start();
        }
    }
}
