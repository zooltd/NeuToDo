using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeuToDo.Scratch
{
    public class DatabaseHandler : IDisposable
    {
        private SQLiteConnection conn;

        //public static string sqlpath;
        private bool disposed = false;

        private static readonly Lazy<DatabaseHandler> database = new Lazy<DatabaseHandler>(() => new DatabaseHandler());

        private DatabaseHandler()
        {
        }

        public static DatabaseHandler Database
        {
            get { return database.Value; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Dispose();
            }

            disposed = true;
        }

        public bool InitDatabase()
        {
            var ifExist = true;
            try
            {
                this.CreateDatabase();

                ifExist = TableExists(nameof(NewEventModel), conn);
                if (!ifExist)
                    this.CreateTable<NewEventModel>();


                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool TableExists(String tableName, SQLiteConnection connection)
        {
            var cmd = connection.CreateCommand("SELECT name FROM sqlite_master WHERE type = 'table' AND name = @name",
                new object[] { tableName });
            //cmd.CommandText = "SELECT * FROM sqlite_master WHERE type = 'table' AND name = @name";
            //cmd.Parameters.Add("@name", DbType.String).Value = tableName;

            string tabledata = cmd.ExecuteScalar<string>();
            return (cmd.ExecuteScalar<string>() != null);
        }

        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "xamdblocal.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);
            Console.WriteLine(path);
            if (!File.Exists(path)) File.Create(path);
            //var plat = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var conn = new SQLiteConnection(path);
            // Return the database connection 
            return conn;
        }

        private bool CreateDatabase()
        {
            conn = GetConnection();
            string str = conn.DatabasePath;
            return true;
        }

        public bool CreateTable<T>()
            where T : new()
        {
            conn.DropTable<T>();
            conn.CreateTable<T>();
            return true;
        }

        public bool InsertIntoTable<T>(T LoginData)
            where T : new()
        {
            conn.Insert(LoginData);
            return true;
        }

        public bool InsertBulkIntoTable<T>(IList<T> LoginData)
            where T : class //new()
        {
            conn.InsertAll(LoginData);
            return true;
        }

        public List<T> SelectDataFromTable<T>()
            where T : new()
        {
            try
            {
                return conn.Table<T>().ToList();
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        public List<T> SelectTableDatafromQuery<T>(string query)
            where T : new()
        {
            return conn.Query<T>(query, new object[] { })
                .ToList();
        }

        public bool UpdateTableData<T>(string query)
            where T : new()
        {
            conn.Query<T>(query);
            return true;
        }

        public void UpdateTableData<T>(IEnumerable<T> query)
            where T : new()
        {
            conn.UpdateAll(query);
        }

        public void UpdateTableData<T>(T query)
            where T : new()
        {
            conn.Update(query);
        }

        public bool DeleteTableData<T>(T LoginData)
        {
            conn.Delete(LoginData);
            return true;
        }

        public bool DeleteTableDataFromPrimaryKey<T>(object primaryKey)
        {
            conn.Delete(primaryKey);
            return true;
        }

        public bool DeleteTableDataFromQuery<T>(string query)
            where T : new()
        {
            conn.Query<T>(query);
            return true;
        }
    }
}