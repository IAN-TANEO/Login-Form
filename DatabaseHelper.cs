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
                if (!File.Exists(databasePath))
                {
                    SQLiteConnection.CreateFile(databasePath);

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();


                        string createTable = @"
                            CREATE TABLE Users (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Username TEXT NOT NULL UNIQUE,
                                Password TEXT NOT NULL
                            )";

                        new SQLiteCommand(createTable, connection).ExecuteNonQuery();

                        string insertAdmin = @"
                            INSERT INTO Users (Username, Password)
                            VALUES ('admin', 'admin123')";

                        new SQLiteCommand(insertAdmin, connection).ExecuteNonQuery();

                        MessageBox.Show("Database created with admin account:\nUsername: admin\nPassword: admin123");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed: {ex.Message}");
            }
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 1 FROM Users 
                        WHERE Username = @Username 
                        AND Password = @Password";

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
    }
}