﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public static void GenerateFactTableDataInParallel()
        {
            List<string> repos = new List<string>
            {
                "devdiv",
                "roslyn",
                "VSO",
                "VSOWithSrc",
                "VsTest"
            };

            int projectSk = 1000;
            int branchSK = 10000;
            int buildPipeLineSK = 10000;

            var taskList = new List<Task>();

            foreach (var repo in repos)
            {
                var task = new Task(() => GenerateFacttable(repo, projectSk, branchSK, buildPipeLineSK));
                taskList.Add(task);
                projectSk += 1;
                branchSK += 10;
                buildPipeLineSK += 10;
            }

            Task.WaitAll(taskList.ToArray());
        }

        public static void GenerateFacttable(string repoName, int projectSk, int startingBranchSK, int startingBuildPipeLineSK)
        {
            Random randomNumberGenerator = new Random();

            Dictionary<string, FileProps> durableSK = new Dictionary<string, FileProps>();
            long gFileSK = 0;

            // read from file
            var files = File.ReadAllLines($"{repoName}.txt"); 

            var watch = Stopwatch.StartNew();
            foreach (var file in files)
            {
                durableSK.Add(file, new FileProps
                {
                    FileSK = gFileSK++,
                    FileDurableSK = gFileSK - 1,
                    TotalLines = randomNumberGenerator.Next(240, 300),
                    ExecutableLines = randomNumberGenerator.Next(200, 220),
                    CoveredLines = randomNumberGenerator.Next(150, 180)
                });
            }

            Console.WriteLine(watch.ElapsedMilliseconds);

            Directory.CreateDirectory(string.Format(@"C:\AnalyticsPlayground\Data\{0}", repoName));
            string filepath = string.Format(@"C:\AnalyticsPlayground\Data\{1}\FileCoverageFact-default-{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmssfff"), repoName);

            using (var fileStream = new StreamWriter(filepath))
            {
                fileStream.WriteLine("DateSK,FileSK,DurableFileSK,BuildPipeLineSK,BranchSK,ProjectSK,BuildID,CodeChurn,TotalLines,ExecutableLines,CoveredLines,FullPath");
                fileStream.Flush();

                for (int branch = 0; branch < 10; ++branch)
                {
                    var sqlTimeStamp = DateTime.Now.ToString("yyyyMMdd");

                    var buildId = randomNumberGenerator.Next(0, 100000);
                    var watch2 = Stopwatch.StartNew();
                    foreach (var file in files)
                    {
                        var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                            sqlTimeStamp,
                            durableSK[file].FileSK,
                            durableSK[file].FileDurableSK,
                            startingBuildPipeLineSK + ((branch % 2 == 0) ? 0 : branch),
                            startingBranchSK + ((branch % 2 == 0) ? 0 : branch),
                            projectSk,
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

            for (int day = 0; day < 30; ++day)
            {
                var sqlTimeStamp = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")) + day + 1;
                filepath = string.Format(@"C:\AnalyticsPlayground\Data\{2}\FileCoverageFact-{0}-{1}.csv", day, DateTime.Now.ToString("yyyyMMddHHmmssfff"), repoName);
                using (var fileStream = new StreamWriter(filepath))
                {
                    fileStream.WriteLine("DateSK,FileSK,DurableFileSK,BuildPipeLineSK,BranchSK,ProjectSK,BuildID,CodeChurn,TotalLines,ExecutableLines,CoveredLines,FullPath");
                    fileStream.Flush();

                    for (int build = 0; build <= 25; ++build)
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

                                if (!durableSK.ContainsKey(newFile))
                                {
                                    durableSK.Add(newFile, fileProp);
                                }

                                tempFiles.Add(newFile);
                            }

                            var watch3 = Stopwatch.StartNew();

                            var buildId = new Random().Next(0, 100000);
                            var codeChurn = new Random().Next(1, 10);

                            foreach (var file in tempFiles)
                            {
                                var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                                    sqlTimeStamp,
                                    durableSK[file].FileSK,
                                    durableSK[file].FileDurableSK,
                                    startingBuildPipeLineSK + ((branch % 2 == 0) ? 0 : branch),
                                    startingBranchSK + ((branch % 2 == 0) ? 0 : branch),
                                    projectSk,
                                    buildId,
                                    codeChurn,
                                    //files.Contains(file) ? 0 : codeChurn,
                                    randomNumberGenerator.Next(240, 300),
                                    randomNumberGenerator.Next(200, 220),
                                    randomNumberGenerator.Next(150, 180),
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
