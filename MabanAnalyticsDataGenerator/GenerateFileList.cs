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
            var files = Directory.EnumerateFiles(repoPath, "*.*", SearchOption.AllDirectories)
                .Where(file => new string[] { ".cs", ".tsx", ".ts" }
                .Contains(Path.GetExtension(file)))
                //.Take(10)
                .ToList();

            File.WriteAllLines(outFile, files);
        }
    }
}
