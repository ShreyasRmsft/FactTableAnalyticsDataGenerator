using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace db_connect
{
    public struct FileProps
    {
        public long FileSK { get; set; }
        public long FileDurableSK { get; set; }
        public int TotalLines { get; set; }
        public int ExecutableLines { get; set; }
        public int CoveredLines { get; set; }
    }

    public class FactTable
    {
        public static void GenerateFacttable()
        {
            const int ProjectSK = 12345;
            const int BranchSK = 1234;
            const int BuildPipeLineSK = 123;

            string repoName = "VSO";
            string repo = @"E:\VSO\src";

            var files = Directory.EnumerateFiles(repo, "*.*", SearchOption.AllDirectories)
                .Where(file => new string[] { ".cs", ".tsx", ".ts" }
                .Contains(Path.GetExtension(file)))
                //.Take(10)
                .ToList();

            Dictionary<string, FileProps> durableSK = new Dictionary<string, FileProps>();

            long gFileSK = 0;
            var watch = Stopwatch.StartNew();
            foreach (var file in files)
            {
                durableSK.Add(file, new FileProps
                {
                    FileSK = gFileSK++,
                    FileDurableSK = gFileSK-1,
                    TotalLines = new Random().Next(240, 300),
                    ExecutableLines = new Random().Next(200, 220),
                    CoveredLines = new Random().Next(150, 180)
                });
            }
            Console.WriteLine(watch.ElapsedMilliseconds);

            string filepath = string.Format(@"F:\test\{1}\FileCoverageFact-default-{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmssfff"), repoName);

            using (var fileStream = new StreamWriter(filepath))
            {
                fileStream.WriteLine("DateSK, FileSK, DurableFileSK, BuildPipeLineSK, BranchSK, ProjectSK, BuildID, CodeChurn, TotalLines, ExecutableLines, CoveredLines, FullPath");
                fileStream.Flush();

                for (int branch = 0; branch < 6; ++branch)
                {
                    var sqlTimeStamp = DateTime.Now.ToString("yyyyMMdd");

                    var buildId = new Random().Next(0, 100000);
                    var watch2 = Stopwatch.StartNew();
                    foreach (var file in files)
                    {
                        var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
                            sqlTimeStamp,
                            durableSK[file].FileSK,
                            durableSK[file].FileDurableSK,
                            BuildPipeLineSK + ((branch % 2 == 0) ? 0 : branch),
                            BranchSK + ((branch % 2 == 0) ? 0 : branch),
                            ProjectSK,
                            buildId,
                            0,
                            durableSK[file].TotalLines,
                            durableSK[file].ExecutableLines,
                            durableSK[file].CoveredLines,
                            file);

                        fileStream.WriteLine(line);
                    }
                    fileStream.Flush();
                    Console.WriteLine(watch2.ElapsedMilliseconds);
                }
            }

            for (int day = 0; day < 7; ++day)
            {
                var sqlTimeStamp = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")) + day + 1;
                filepath = string.Format(@"F:\test\{2}\FileCoverageFact-{0}-{1}.csv", day, DateTime.Now.ToString("yyyyMMddHHmmssfff"), repoName);
                using (var fileStream = new StreamWriter(filepath))
                {
                    fileStream.WriteLine("DateSK, FileSK, DurableFileSK, BuildPipeLineSK, BranchSK, ProjectSK, BuildID, CodeChurn, TotalLines, ExecutableLines, CoveredLines, FullPath");
                    fileStream.Flush();

                    for (int build = 0; build <= 33; ++build)
                    {
                        for (int branch = 0; branch < 6; ++branch)
                        {
                            var randomSkip = new Random().Next(900, 90000);
                            var randomTake = 100;

                            var tempFiles = files.ToList();
                            var resultSet = tempFiles.Skip(randomSkip).Take(randomTake).ToList();

                            foreach (var result in resultSet)
                            {
                                tempFiles.Remove(result);
                                var fileProp = durableSK[result];

                                fileProp.FileSK = gFileSK++;
                                var newFile = result + day + build + branch;

                                durableSK.Add(newFile, fileProp);
                                tempFiles.Add(newFile);
                            }

                            var watch3 = Stopwatch.StartNew();

                            var buildId = new Random().Next(0, 100000);
                            var codeChurn = new Random().Next(1, 10);

                            foreach (var file in tempFiles)
                            {
                                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
                                    sqlTimeStamp,
                                    durableSK[file].FileSK,
                                    durableSK[file].FileDurableSK,
                                    BuildPipeLineSK + ((branch % 2 == 0) ? 0 : branch),
                                    BranchSK + ((branch % 2 == 0) ? 0 : branch),
                                    ProjectSK,
                                    buildId,
                                    codeChurn,
                                    //files.Contains(file) ? 0 : codeChurn,
                                    durableSK[file].TotalLines,
                                    durableSK[file].ExecutableLines,
                                    durableSK[file].CoveredLines,
                                    file);

                                fileStream.WriteLine(line);
                            }
                            fileStream.Flush();
                            Console.WriteLine(watch3.ElapsedMilliseconds);
                            Console.WriteLine("Done buildID {0}, day {1}, build {2}, branch {3}", buildId, day, build, branch);
                            files.Union(tempFiles);
                        }
                    }
                }
            }
        }
    }
}
