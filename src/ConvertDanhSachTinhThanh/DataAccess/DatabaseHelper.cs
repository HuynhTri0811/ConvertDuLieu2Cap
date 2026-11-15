using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ConvertDanhSachTinhThanh.DataAccess
{
    /// <summary>
    /// Các hàm SQL Server dùng chung. Mỗi hàm tự mở và đóng kết nối.
    /// Form1 hiện chưa dùng lớp này mà vẫn tự mở SqlConnection trong từng handler.
    /// </summary>
    public class DatabaseHelper
    {
        /// <summary>
        /// Chạy câu SELECT và ánh xạ từng dòng bằng mapFunction.
        /// Lỗi kết nối hoặc truy vấn được bọc lại thành Exception với thông báo tiếng Việt/emoji.
        /// </summary>
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

        /// <summary>
        /// Chạy lần lượt các lệnh trên cùng một kết nối, bỏ qua lệnh rỗng.
        /// Dùng cho các batch của script SQL đã tách theo GO. Không chạy trong transaction.
        /// </summary>
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

        /// <summary>
        /// Gọi stored procedure với các tham số; giá trị null được gửi thành DBNull.
        /// </summary>
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
