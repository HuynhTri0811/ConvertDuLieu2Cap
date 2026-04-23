using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ConvertDanhSachTinhThanh.DataAccess
{
    public class DatabaseHelper
    {
        public static List<T> ExecuteReader<T>(string connectionString, string query, Func<SqlDataReader, T> mapFunction)
        {
            List<T> result = new List<T>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException ex)
                {
                    throw new Exception("❌ Connection failed: " + ex.Message);
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result.Add(mapFunction(reader));
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("❌ Error: " + ex.Message);
                    }
                }
            }

            return result;
        }

        public static void ExecuteNonQuery(string connectionString, List<string> commands)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    foreach (var command in commands)
                    {
                        if (!string.IsNullOrWhiteSpace(command))
                        {
                            using (var cmd = new SqlCommand(command, connection))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error executing commands: " + ex.Message);
                }
            }
        }

        public static void ExecuteStoredProcedure(string connectionString, string procedureName, Dictionary<string, object> parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new SqlCommand(procedureName, connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error executing stored procedure: " + ex.Message);
                }
            }
        }
    }
}
