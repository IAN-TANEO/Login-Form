using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace LoginSystem
{
    public class DatabaseHelper
    {
        private readonly string databasePath;
        private readonly string connectionString;

        public DatabaseHelper()
        {
            databasePath = Path.Combine(Application.StartupPath, "users.db");
            connectionString = $"Data Source={databasePath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                bool needsSetup = !File.Exists(databasePath);

                if (needsSetup)
                {
                    SQLiteConnection.CreateFile(databasePath);
                }

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    
                    string checkTableSql = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Users'";
                    var tableExists = Convert.ToInt32(new SQLiteCommand(checkTableSql, connection).ExecuteScalar()) > 0;

                    if (!tableExists)
                    {
                        
                        string createTable = @"
                            CREATE TABLE Users (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Username TEXT NOT NULL UNIQUE,
                                Password TEXT NOT NULL,
                                DisplayName TEXT,
                                Email TEXT,
                                IsGoogleAuth INTEGER DEFAULT 0,
                                LastLogin TEXT
                            )";
                        new SQLiteCommand(createTable, connection).ExecuteNonQuery();

                        
                        string insertAdmin = @"
                            INSERT INTO Users (Username, Password, DisplayName, Email)
                            VALUES ('admin', 'admin123', 'Administrator', 'admin@example.com')";
                        new SQLiteCommand(insertAdmin, connection).ExecuteNonQuery();
                    }
                    else
                    {
                        
                        AddColumnIfNotExists(connection, "DisplayName", "TEXT");
                        AddColumnIfNotExists(connection, "Email", "TEXT");
                        AddColumnIfNotExists(connection, "IsGoogleAuth", "INTEGER DEFAULT 0");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed: {ex.Message}");
            }
        }

        private void AddColumnIfNotExists(SQLiteConnection connection, string columnName, string columnType)
        {
            string checkColumnSql = $"PRAGMA table_info(Users);";
            var command = new SQLiteCommand(checkColumnSql, connection);
            var reader = command.ExecuteReader();
            bool columnExists = false;

            while (reader.Read())
            {
                if (reader["name"].ToString() == columnName)
                {
                    columnExists = true;
                    break;
                }
            }

            if (!columnExists)
            {
                string addColumnSql = $"ALTER TABLE Users ADD COLUMN {columnName} {columnType}";
                new SQLiteCommand(addColumnSql, connection).ExecuteNonQuery();
            }
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT 1 FROM Users WHERE (Username = @Username OR Email = @Username) AND Password = @Password";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        return command.ExecuteScalar() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}");
                return false;
            }
        }

        public bool UserExists(string email)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool CreateUser(string username, string password, string displayName, string email, bool isGoogleAuth = false)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Users (Username, Password, DisplayName, Email, IsGoogleAuth) 
                        VALUES (@Username, @Password, @DisplayName, @Email, @IsGoogleAuth)";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@DisplayName", displayName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@IsGoogleAuth", isGoogleAuth ? 1 : 0);
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating user: {ex.Message}");
                return false;
            }
        }
    }
}