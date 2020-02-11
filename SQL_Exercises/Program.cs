using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Data.SqlClient;

namespace SQL_Exercises
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbName = "ExerciseDb";
            string table1Name = "DataTable";
            string table2Name = "Entry_log";

            string masterConString = "Data Source=DESKTOP-KONOVT3\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
            string dbConString = "Data Source=DESKTOP-KONOVT3\\SQLEXPRESS;Initial Catalog=" + dbName + ";Integrated Security=True";
            Console.WriteLine("Press any key to check if 'ExerciseDb' exists.");
            Console.ReadKey();
            Thread.Sleep(1000);

            Console.WriteLine("Checking if Database 'ExerciseDb' exists...");
            Thread.Sleep(2000);
            if (isDbExists(dbName, masterConString))
            {
                Console.Clear();
                Console.WriteLine("Database exists.");
            }
            else
            {
                Console.WriteLine("Database does not exists.");
                Thread.Sleep(1000);
                Console.WriteLine("Please wait while we create the database");
                Thread.Sleep(2000);
                CreateDb(dbName, masterConString);

            }

            Thread.Sleep(1000);
            Console.WriteLine($"Press any key to create tables '{table1Name}' and '{table2Name}'");
            Console.ReadKey();

            if (isTableExists(table1Name, dbName, dbConString))
            {
                Console.Clear();
                Console.WriteLine($"{table1Name} exists in {dbName}");
            }
            else
            {
                Console.WriteLine($"{table1Name} does not exists.");
                Thread.Sleep(1000);
                Console.WriteLine($"Please wait while we create the {table1Name}");
                Thread.Sleep(2000);
                CreateTable(table1Name, dbName, dbConString);
            }

            if (isTableExists(table2Name, dbName, dbConString))
            {

                Console.WriteLine($"{table2Name} exists in {dbName}");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"{table2Name} does not exists.");
                Thread.Sleep(1000);
                Console.WriteLine($"Please wait while we create the {table2Name}");
                Thread.Sleep(2000);
                CreateLogTable(table2Name, table1Name, dbName, dbConString);
            }

            Console.ReadKey();
        }

        public static bool isDbExists(string dbName, string conStr)
        {
            string cmdText = "SELECT * FROM master.dbo.sysdatabases WHERE name='" + dbName + "'";
            bool isExist = false;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(cmdText, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        isExist = reader.HasRows;
                    }
                }
                con.Close();
            }
            return isExist;
        }

        public static bool isTableExists(string tableName, string dbName, string conStr)
        {
            bool isExist = false;
            SqlConnection con = new SqlConnection(conStr);
            string cmdText = "SELECT CASE WHEN EXISTS(SELECT * FROM information_schema.tables WHERE table_name = '" + tableName + "') THEN 1 ELSE 0 END";
            SqlCommand cmd = new SqlCommand(cmdText, con);
            try
            {
                con.Open();
                isExist = (int)cmd.ExecuteScalar() == 1;
            }
            catch (System.Exception e)
            {
                Console.WriteLine("An error occured:");
                Console.WriteLine(e.ToString());
                Console.WriteLine("Press any key to continue..");
                Console.ReadKey();
            }
            finally
            {
                con.Close();
            }
            return isExist;
        }

        public static void CreateDb(string dbName, string conStr)
        {
            SqlConnection con = new SqlConnection(conStr);
            string cmdText = "CREATE DATABASE " + dbName;
            SqlCommand cmd = new SqlCommand(cmdText, con);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                Console.Clear();
                Console.WriteLine("Database created succesfully!");
            }
            catch (System.Exception e)
            {
                Console.WriteLine("An error occured:");
                Console.WriteLine(e.ToString());
                Console.WriteLine("Press any key to continue..");
                Console.ReadKey();
            }
            finally
            {
                con.Close();
            }
        }

        public static void CreateTable(string tableName, string dbName, string conStr)
        {
            SqlConnection con = new SqlConnection(conStr);
            string cmdText = "CREATE TABLE " + tableName +
                             " (ID int IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                             "Name nvarchar(50) NULL, " +
                             "LastName nvarchar(50) NULL, " +
                             "Age int NULL);";
            SqlCommand cmd = new SqlCommand(cmdText, con);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine($"{tableName} created succefully!");
            }
            catch (System.Exception e)
            {
                Console.WriteLine("An error occured:");
                Console.WriteLine(e.ToString());
                Console.WriteLine("Press any key to continue..");
                Console.ReadKey();
            }
            finally
            {
                con.Close();
            }
        }

        public static void CreateLogTable(string logTableName, string referenceTableName, string dbName, string conStr)
        {
            SqlConnection con = new SqlConnection(conStr);
            string columnNameOfPK = "(SELECT cu.COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE cu " +
                                    "WHERE EXISTS ( SELECT tc.* FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc " +
                                    "WHERE tc.CONSTRAINT_CATALOG = '" + dbName + "' AND tc.TABLE_NAME = '" + referenceTableName + "' AND " +
                                    "tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND tc.CONSTRAINT_NAME = cu.CONSTRAINT_NAME ))";
            bool isPKNameFound = false;
            SqlCommand cmd = new SqlCommand(columnNameOfPK, con);
            try
            {
                con.Open();
                var returnedValue = cmd.ExecuteScalar();
                if (returnedValue != null)
                {
                    columnNameOfPK = returnedValue.ToString();
                    isPKNameFound = true;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(e);
            }
            finally
            {
                con.Close();
            }

            string cmdText = "CREATE TABLE " + logTableName +
                             " (LogID int IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                             "DataID int FOREIGN KEY REFERENCES " + referenceTableName + "(" + columnNameOfPK + ")," +
                             " Name nvarchar(50) NULL, " +
                             "LastName nvarchar(50) NULL, " +
                             "Age int NULL," +
                             "InputDate datetime NOT NULL);";

            cmd = new SqlCommand(cmdText, con);

            try
            {
                if (!isPKNameFound)
                {
                    throw new NotSupportedException("The PK name of parent table was not found");
                }
                con.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine($"{logTableName} created succefully!");
            }
            catch (System.Exception e)
            {
                Console.WriteLine("An error occured:");
                Console.WriteLine(e.ToString());
                Console.WriteLine("Press any key to continue..");
                Console.ReadKey();
            }
            finally
            {
                con.Close();
            }

        }

    }
}
