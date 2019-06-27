using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MabanAnalyticsDataGenerator
{
    public class SqlBulkInserter
    {
        public static void InsertIntoDatabase(string file)
        {
            string queryString = @"
BULK INSERT [dbo].[FileCoverageFact]
FROM '" + file + @"'
WITH
(
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',  --CSV field delimiter
    ROWTERMINATOR = '\n',   --Use to shift the control to next row
    ERRORFILE = '" + file.Replace(".csv", ".txt") + @"',
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
    }
}