using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Ecommerce.Data
{
    public class DbContext
    {
        private readonly string _connectionString;

        public DbContext()
        {
            // Try multiple connection string names for flexibility
            // Priority: EcommerceDB2 > EcommerceDB > First available connection string
            ConnectionStringSettings connectionStringSettings = null;
            
            // Try EcommerceDB2 first
            connectionStringSettings = ConfigurationManager.ConnectionStrings["EcommerceDB2"];
            
            // Fallback to EcommerceDB
            if (connectionStringSettings == null)
            {
                connectionStringSettings = ConfigurationManager.ConnectionStrings["EcommerceDB"];
            }
            
            // Last resort: use the first available connection string (excluding system ones)
            if (connectionStringSettings == null)
            {
                foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
                {
                    // Skip system connection strings (they start with LocalSqlServer, etc.)
                    if (cs.Name != "LocalSqlServer" && !string.IsNullOrEmpty(cs.ConnectionString))
                    {
                        connectionStringSettings = cs;
                        break;
                    }
                }
            }
            
            if (connectionStringSettings == null)
            {
                // Debug: List all available connection strings
                var availableConnections = new System.Text.StringBuilder();
                availableConnections.AppendLine("Available connection strings:");
                foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
                {
                    availableConnections.AppendLine($"  - {cs.Name}");
                }
                
                throw new ConfigurationErrorsException(
                    $"No valid connection string found in Web.config.\n\n" +
                    $"Tried: 'EcommerceDB2', 'EcommerceDB'\n\n" +
                    $"{availableConnections.ToString()}\n\n" +
                    "Please ensure your Web.config contains:\n" +
                    "<connectionStrings>\n" +
                    "  <add name=\"EcommerceDB2\" connectionString=\"...\" providerName=\"System.Data.SqlClient\" />\n" +
                    "</connectionStrings>");
            }
            
            _connectionString = connectionStringSettings.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ConfigurationErrorsException($"Connection string '{connectionStringSettings.Name}' is empty in Web.config.");
            }
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
