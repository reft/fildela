using Fildela.Data.Database.DataLayer;
using System;

namespace Fildela.InitDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionstring = InitDB();

            Console.WriteLine("Database updated. Press any key to exit. \n\n");

            Console.WriteLine("Connectionstring: " + connectionstring);
            Console.ReadKey();
        }

        private static string InitDB()
        {
            DataLayer DB = new DataLayer();

            DB.Database.Initialize(true);

            return DB.Database.Connection.ConnectionString;
        }
    }
}