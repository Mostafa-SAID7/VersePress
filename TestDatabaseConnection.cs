using Microsoft.Data.SqlClient;
using System;

namespace VersePress.DatabaseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=db43358.public.databaseasp.net;Database=db43358;User Id=db43358;Password=Pi5?#2SebC+4;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("✅ Connection successful!");
                    Console.WriteLine();
                    
                    // Check if __EFMigrationsHistory table exists
                    string checkMigrationsQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = '__EFMigrationsHistory'";
                    
                    using (SqlCommand command = new SqlCommand(checkMigrationsQuery, connection))
                    {
                        int count = (int)command.ExecuteScalar();
                        Console.WriteLine($"Migration history table exists: {(count > 0 ? "✅ Yes" : "❌ No")}");
                    }
                    
                    // List all tables
                    string listTablesQuery = @"
                        SELECT TABLE_NAME 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_TYPE = 'BASE TABLE'
                        ORDER BY TABLE_NAME";
                    
                    Console.WriteLine("\n📋 Tables in database:");
                    using (SqlCommand command = new SqlCommand(listTablesQuery, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int tableCount = 0;
                        while (reader.Read())
                        {
                            tableCount++;
                            Console.WriteLine($"  {tableCount}. {reader.GetString(0)}");
                        }
                        Console.WriteLine($"\nTotal tables: {tableCount}");
                    }
                    
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
