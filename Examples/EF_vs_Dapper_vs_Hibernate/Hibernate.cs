using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using System.Data.SqlClient;
using System.Diagnostics;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace EF_vs_Dapper_vs_Hibernate
{
    class Hibernate
    {
        private Configuration myConfiguration;
        private ISessionFactory mySessionFactory;
        private ISession mySession;

        public Hibernate()
        {
            myConfiguration = new Configuration();
            myConfiguration.Configure("..//..//..//hibernate.cfg.xml");
            mySessionFactory = myConfiguration.BuildSessionFactory();
            mySession = mySessionFactory.OpenSession();
            
            //GenerateData();


        }

        public void GenerateData()
        {
            Random r = new Random();
            using (mySession.BeginTransaction())
            {
                for (int i = 0; i < Program.N; i++)
                {

                    Product product = new Product()
                    {
                        Name = "Product"+i,
                        Price = r.Next(0,10_000)
                    };
                    
                    mySession.Save(product);
                }

                mySession.Transaction.Commit();
            }
        }

        public void GetAll()
        {
            ICriteria criteria = mySession.CreateCriteria<Product>();
            IList<Product> list = criteria.List<Product>();

            foreach (Product product in list)
            {
                var tmp = product.Name;
            }
        }

        public void GetWithFilter()
        {
            ICriteria criteria = mySession.CreateCriteria<Product>();

            var list = mySession.QueryOver<Product>()
                .Where(p=> p.Price == 5_000);

            foreach (Product product in list.List())
            {
                var tmp = product.Name;
            }
        }

        public void UpdateAll()
        {
            ICriteria criteria = mySession.CreateCriteria<Product>();
            IList<Product> list = criteria.List<Product>();

            using (mySession.BeginTransaction())
            {
                foreach (Product product in list)
                {
                    product.Price *= 2;
                    mySession.Save(product);
                }
                mySession.Transaction.Commit();
            }
        }

        public void DeleteHalf()
        {

            using (ISession session = mySessionFactory.OpenSession())
            {
                SqlConnection connection = session.Connection as SqlConnection;
                SqlCommand command = new SqlCommand("Delete from Product where Price>5000", connection);
                command.ExecuteNonQuery();
            }
        }

        public void Test()
        {
            using (ISession session = mySessionFactory.OpenSession())
            {
                SqlConnection connection = session.Connection as SqlConnection;
                SqlCommand command = new SqlCommand("Delete from Product", connection);
                command.ExecuteNonQuery();
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            GenerateData();

            sw.Stop();
            Console.WriteLine("Hibernate Insert {1} = {0}", sw.ElapsedMilliseconds,Program.N);
            sw = new Stopwatch(); sw.Start();

            GetAll();

            sw.Stop();
            Console.WriteLine("Hibernate GetAll = {0}", sw.ElapsedMilliseconds);
            sw = new Stopwatch(); sw.Start();

            GetWithFilter();

            sw.Stop();
            Console.WriteLine("Hibernate GetWithFilter = {0}", sw.ElapsedMilliseconds);
            sw = new Stopwatch(); sw.Start();

            UpdateAll();

            sw.Stop();
            Console.WriteLine("Hibernate UpdateAll = {0}", sw.ElapsedMilliseconds);
            sw = new Stopwatch(); sw.Start();

            DeleteHalf();

            sw.Stop();
            Console.WriteLine("Hibernate DeleteHalf = {0}", sw.ElapsedMilliseconds);
            //sw = new Stopwatch();sw.Start();
        }
    }
}
