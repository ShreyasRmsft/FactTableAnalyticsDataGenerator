using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MabanAnalyticsDataGenerator;

namespace db_connect
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(" ,", args));

            //GenerateFileDim();
            //FileListGenerator.GenerateListOfFiles(args[0], $"{args[1]}.txt");

            FactTable.GenerateFactTableDataInParallel();
        }
    }
}
