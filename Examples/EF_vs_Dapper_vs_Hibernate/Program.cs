using System;

namespace EF_vs_Dapper_vs_Hibernate
{
    class Program
    {
        public static int N = 3000;

        static void Main(string[] args)
        {
            Console.WriteLine("Hibernate:");
            Hibernate hibernate = new Hibernate();
            hibernate.Test();
            Console.WriteLine();

            Console.WriteLine("EF:");
            EF ef = new EF();
            ef.Test();
            Console.WriteLine();

            Console.WriteLine("Dapper:");
            Dapper dapper = new Dapper();
            dapper.Test();
            Console.WriteLine();

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
