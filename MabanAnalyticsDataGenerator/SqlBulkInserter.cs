using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace MabanAnalyticsDataGenerator
{
    public class SqlBulkInserter
    {
        public static void InsertIntoDatabase(string file)
        {
            // Process tool = new Process();
            // tool.StartInfo.FileName = "handle64.exe";
            // tool.StartInfo.Arguments = file + " /accepteula";
            // tool.StartInfo.UseShellExecute = false;
            // tool.StartInfo.RedirectStandardOutput = true;
            // tool.Start();
            // tool.WaitForExit();
            // string outputTool = tool.StandardOutput.ReadToEnd();

            // string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
            // foreach (Match match in Regex.Matches(outputTool, matchPattern))
            // {
            //     //Process.GetProcessById(int.Parse(match.Value)).Kill();
            //     Console.WriteLine(Process.GetProcessById(int.Parse(match.Value)).ProcessName);
            //     Console.WriteLine(Process.GetProcessById(int.Parse(match.Value)).Id);
            // }

            try
            {
                string queryString = @"
BULK INSERT [dbo].[FileCoverageFact]
FROM '" + file + @"'
WITH
(
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',  --CSV field delimiter
    ROWTERMINATOR = '\n',   --Use to shift the control to next row
    TABLOCK
)";
                string connectionString = "Server=localhost;Database=AnalyticsPlayground2;Trusted_Connection=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}