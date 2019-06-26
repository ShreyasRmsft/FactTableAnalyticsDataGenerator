using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace db_connect
{
    class Program
    {
        static void Main(string[] args)
        {
            // GenerateFileDim();
            FactTable.GenerateFacttable();
        }

        private static void GenerateFileDim()
        {
            const int ProjectSK = 12345;
            const int BranchSK = 1234;

            var sqlFormattedDate = 20190501;

            var files = Directory.EnumerateFiles(@"E:\VSO\src", "*.*", SearchOption.AllDirectories)
                .Where(file => new string[] { ".cs", ".tsx", ".ts" }
                .Contains(Path.GetExtension(file)))
                .ToList();

            Dictionary<string, long> durableSK = new Dictionary<string, long>();

            long fileSK = 0;

            foreach (var file in files)
            {
                durableSK.Add(file, fileSK++);
            }

            using (var fileStream = new StreamWriter(@"F:\test\AzDevopsFilesMultipleBranchesMonthAccurate.csv"))
            {
                fileStream.WriteLine("FileSK, DurableFileKey, BranchSK, ProjectSK, FullPath, RowEffectiveDate, RowExpiryDate, CurrentRow");
                fileStream.Flush();

                fileSK = 0;

                // First Insert All files for each active branch
                for (int i = 0; i < 10; ++i)
                {
                    foreach (var file in files)
                    {
                        var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                            fileSK++,
                            durableSK[file],
                            BranchSK + i,
                            ProjectSK,
                            file,
                            sqlFormattedDate,
                            sqlFormattedDate + 1,
                            "Expired");

                        fileStream.WriteLine(line);
                        fileStream.Flush();
                    }
                }


                for (int day = 0; day <= 30; ++day)
                {
                    for (int build = 0; build <= 30; ++build)
                    {
                        for (int branch = 0; branch < 10; ++branch)
                        {
                            var randomSkip = new Random().Next(900, 90000);
                            var randomTake = 20;    // new Random().Next(900, 1000);

                            var resultSet = files.Skip(randomSkip).Take(randomTake);

                            foreach (var file in resultSet)
                            {
                                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                                    fileSK++,
                                    durableSK[file],
                                    branch % 2 == 0 ? BranchSK : BranchSK + branch,
                                    ProjectSK,
                                    file + day + build + branch,
                                    sqlFormattedDate + day,
                                    sqlFormattedDate + day + 1,
                                    "Expired");

                                fileStream.WriteLine(line);
                                fileStream.Flush();
                            }
                        }
                    }
                }

                for (int i = 0; i < 10; ++i)
                {
                    foreach (var file in files)
                    {
                        var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                            fileSK++,
                            durableSK[file],
                            BranchSK + i,
                            ProjectSK,
                            file,
                            sqlFormattedDate + 31,
                            "99991231",
                            "Current");

                        fileStream.WriteLine(line);
                        fileStream.Flush();
                    }
                }
            }
        }
    }
}
