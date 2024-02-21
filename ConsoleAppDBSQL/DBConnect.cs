using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace ConsoleAppDBSQL;

public class DBConnect
{
        private MySqlConnection _connection;
        private string _server;
        private string _database;
        private string _uid;
        private string _password;

        public DBConnect()
        {
                Initialize();
        }

        private void Initialize()
        {
                _server = "localhost";
                _database = "connectcsharptomysql";
                _uid = "root";
                _password = "";
                string connectionString = $"SERVER={_server};" +
                                          $"DATABASE={_database};" +
                                          $"UID={_uid};" +
                                          $"PASSWORD={_password};";
                _connection = new MySqlConnection(connectionString);
        }
        
        //open connection to database
        //open connection to database
        private bool OpenConnection()
        {
                try
                {
                        _connection.Open();
                        return true;
                }
                catch (MySqlException e)
                {
                        //When handling errors, you can your application's response based 
                        //on the error number.
                        //The two most common error numbers when connecting are as follows:
                        //0: Cannot connect to server.
                        //1045: Invalid user name and/or password.
                        switch (e.Number)
                        {
                                case 0:
                                        Console.WriteLine("Cannot connect to server.");
                                        break;
                                case 1045:
                                        Console.WriteLine("Invalid username/password");
                                        break;
                        };
                        return false;
                }
        }

        //Close connection
        private bool CloseConnection()
        {
                try
                {
                        _connection.Close();
                        return true;
                }
                catch (MySqlException e)
                {
                        Console.WriteLine(e.Message);
                        return false;
                }
        }

        //Insert statement
        public void Insert()
        {
                string query = "INSERT INTO tableinfo (name, age) VALUES ('John Smith', '33')";
                //open connection
                if (OpenConnection() == true)
                {
                        //create command and assign the query and connection from the constructor
                        MySqlCommand command = new MySqlCommand(query, _connection);
                        
                        //execute command
                        command.ExecuteNonQuery();
                        
                        //close connection
                        CloseConnection();
                }
        }

        //Update statement
        public void Update()
        {
                string query = 
                       $"UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";
                
                //Open connection
                if (OpenConnection())
                {
                        //create mysql command
                        MySqlCommand command = new MySqlCommand();
                        //assign the query using CommandText
                        command.CommandText = query;
                        //Assign the connection using Connection
                        command.Connection = _connection;
                        //execute query
                        command.ExecuteNonQuery();
                        //close connection
                        CloseConnection();
                }
        }
        
        //Delete statement
        public void Delete()
        {
                string query = 
                        $"DELETE FROM tableinfo WHERE name='John Smith'";
                if (OpenConnection())
                {
                        MySqlCommand command = new MySqlCommand(query, _connection);
                        command.ExecuteNonQuery();
                        CloseConnection();
                }
        }

        //Select statement
        public List <string> [] Select()
        {
                string query = $"SELECT * FROM tableinfo";
                
                //create a list to store the result
                List<string>[] list = new List<string>[3];
                list[0] = new List<string>();
                list[1] = new List<string>();
                list[2] = new List<string>();
                
                //open connection
                if (OpenConnection())
                {
                        //create command
                        MySqlCommand command = new MySqlCommand(query, _connection);
                        //create a data reader and execute the command
                        MySqlDataReader dataReader = command.ExecuteReader();
                        //Read the data and store then in the list
                        while (dataReader.Read())
                        {
                                list[0].Add(dataReader["id"] + "");
                                list[1].Add(dataReader["name"] + "");
                                list[2].Add(dataReader["age"] + "");
                                
                        }
                        
                        //close data reader
                        dataReader.Close();
                        //close connection
                        CloseConnection();
                        //return list to be displayed
                        return list;
                }
                return list;
        }

        //Count statement
        public int Count()
        {
                string query = "SELECT Count(*) FROM tableinfo";
                int count = -1;
                
                //open connection
                if (OpenConnection())
                {
                        //Create Mysql command
                        MySqlCommand command = new MySqlCommand(query, _connection);
                        
                        //ExecuteScalar will return one value
                        count = int.Parse(command.ExecuteScalar() + "");
                        
                        //close connection
                        CloseConnection();
                        return count;
                }
                
                return count;
        }

        //Backup
        public void Backup()
        {
                try
                {
                        DateTime time = DateTime.Now;
                        int year = time.Year;
                        int month = time.Month;
                        int day = time.Day;
                        int hour = time.Hour;
                        int minute = time.Minute;
                        int second = time.Second;
                        int millisecond = time.Millisecond;

                        //Save file to Desktop /Users/necatihan/RiderProjects/ConsoleAppDBSQL
                        string path = $"/Users/necatihan/RiderProjects/ConsoleAppDBSQL/BackupDB/" +
                                      $"MySqlBackup" +
                                      $"{year}_" +
                                      $"{month}_" +
                                      $"{day}_" +
                                      $"{hour}_" +
                                      $"{minute}_" +
                                      $"{second}_" +
                                      $"{millisecond}.sql";
                        StreamWriter file = new StreamWriter(path);
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "mysqldump";
                        psi.RedirectStandardInput = false;
                        psi.RedirectStandardOutput = true;
                        psi.Arguments =
                                string.Format(
                                        @"-u{0} -p{1} -h{2} {3}", _uid, _password, _server, _database);
                        psi.UseShellExecute = false;
                        
                        Process process = Process.Start(psi);

                        string output = process.StandardOutput.ReadToEnd();
                        file.WriteLine(output);
                        process.WaitForExit();
                        file.Close();
                        process.Close();
                }
                catch (IOException e)
                {
                        Console.WriteLine("Error, unable to backup");
                        throw;
                }
        }

        //Restore
        public void Restore()
        {
                try
                {
                        string path = $"/Users/necatihan/RiderProjects/ConsoleAppDBSQL/BackupDB/" +
                                      $"MySqlBackup.sql";
                        StreamReader file = new StreamReader(path);
                        string input = file.ReadToEnd();
                        file.Close();
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "mysql";
                        psi.RedirectStandardInput = true;
                        psi.RedirectStandardOutput = false;
                        psi.Arguments = 
                                string.Format(@"-u{0} -p{1} -h{2} {3}", _uid, _password, _server, _database);
                        psi.UseShellExecute = false;
                        
                        Process process = Process.Start(psi);
                        process.StandardInput.WriteLine(input);
                        process.StandardInput.Close();
                        process.WaitForExit();
                        process.Close();
                }
                catch (IOException e)
                {
                        Console.WriteLine("Error, unable to restore");
                        throw;
                }
        }


}
