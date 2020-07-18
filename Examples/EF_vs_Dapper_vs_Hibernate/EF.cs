using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NHibernate.Linq;

namespace EF_vs_Dapper_vs_Hibernate
{
    class EFContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("data source=(localdb)\\MSSQLLocalDB;Database=EFProducts;Integrated Security=True;");
        }

        public DbSet<Product> Product { get; set; }
    }

    class EF
    {
        private EFContext _context;

        public EF()
        {
            _context = new EFContext();
        }

        public void GenerateData()
        {
            Random r = new Random();
            for (int i = 0; i < Program.N; i++)
            {
                Product product = new Product()
                {
                    ID = i+1,
                    Name = "Product" + i,
                    Price = r.Next(0, 10_000)
                };

                _context.Product.Add(product);
            }

            _context.SaveChanges();
        }

        public void GetAll()
        {
            var list = _context.Product;

            foreach (Product product in list)
            {
                var tmp = product.Name;
            }
        }

        public void GetWithFilter()
        {
            var list = _context.Product
                .Where(p => p.Price == 5_000);

            foreach (Product product in list)
            {
                var tmp = product.Name;
            }
        }

        public void UpdateAll()
        {
            var list = _context.Product;

            foreach (Product product in list)
            {
                product.Price *= 2;
            }
            _context.UpdateRange(list);
            _context.SaveChanges();
        }

        public void DeleteHalf()
        {
            var list = _context.Product
                .Where(p => p.Price > 5_000);

            //foreach (Product product in list)
            //{
            //    _context.Entry(product).State = EntityState.Deleted;
            //}

            _context.Product.RemoveRange(list);

            _context.SaveChanges();

        }

        public void Test()
        {
            _context.Product.RemoveRange(_context.Product);
            _context.SaveChanges();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            GenerateData();

            sw.Stop();
            Console.WriteLine("EF Insert {1} = {0}", sw.ElapsedMilliseconds, Program.N);
            sw = new Stopwatch(); sw.Start();

            GetAll();

            sw.Stop();
            Console.WriteLine("EF GetAll = {0}", sw.ElapsedMilliseconds);
            sw = new Stopwatch(); sw.Start();

            GetWithFilter();

            sw.Stop();
            Console.WriteLine("EF GetWithFilter = {0}", sw.ElapsedMilliseconds);
            sw = new Stopwatch(); sw.Start();

            UpdateAll();

            sw.Stop();
            Console.WriteLine("EF UpdateAll = {0}", sw.ElapsedMilliseconds);
            sw = new Stopwatch(); sw.Start();

            DeleteHalf();

            sw.Stop();
            Console.WriteLine("EF DeleteHalf = {0}", sw.ElapsedMilliseconds);
            //sw = new Stopwatch();sw.Start();
        }
    }
}
