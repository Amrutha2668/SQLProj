using System;
using System.IO;

namespace SQLProj
{
    class Program
    {
        static void Main(string[] args)
        {
            //csv file path
            string path = @"C:\Users\Ammu\Downloads\Maharashtra.csv";
            string[] flName = path.Split(Path.DirectorySeparatorChar);
            string dbName = (flName[flName.Length - 1]).Replace(".csv", "");
            
            //SQL server connection string
            string conStr = "Data Source=DESKTOP-KTRBOEN;Database=master;Integrated Security=true;";
            
            //Accepting user input. 
            Console.WriteLine("Enter name of the Operation you want to perform: ");
            string op = Console.ReadLine().ToLower();

            //Operations.
            if (op.Equals("load")) Loader.Load(path, conStr, dbName);
            else if (op.Equals("analyze")) Analyzer.Analyze(dbName);
            else Console.WriteLine("No such operation performed");
            Console.ReadLine();
        }
    }
}
