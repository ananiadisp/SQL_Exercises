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

    }
}
