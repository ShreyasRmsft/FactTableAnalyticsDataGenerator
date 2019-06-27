using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MabanAnalyticsDataGenerator
{
    public class FileListGenerator
    {
        public static void GenerateListOfFiles(string repoPath, string outFile)
        {
            Console.WriteLine(repoPath);
            Console.WriteLine(outFile);

            var files = Directory.EnumerateFiles(repoPath, "*.*", SearchOption.AllDirectories)
                .Where(file => new string[] { ".cs", ".tsx", ".ts" }
                .Contains(Path.GetExtension(file)))
                .Select(file => file.Remove(0, repoPath.Length + 1).Replace('\\', '/'))
                .ToList();


            //var files = Directory.EnumerateFiles(repoPath, "*.*", SearchOption.AllDirectories)
            //    .Where(file => new string[] { ".cs", ".tsx", ".ts" }
            //    .Contains(Path.GetExtension(file)))
            //    .Select(file => file.Remove(0, repoPath.Length -3).Replace('\\', '/'))
            //    .ToList();

            File.WriteAllLines(outFile, files);
        }
    }
}
