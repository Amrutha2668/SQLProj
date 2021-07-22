using System;
using System.Data.SqlClient;
using System.Data;
using CsvHelper;
using System.IO;
using System.Globalization;

namespace SQLProj
{
    class Loader
    {
        public static void Load(String Path, String ConStr, String DbName)
        {
            string[] queries = { $"Data Source=DESKTOP-KTRBOEN;Initial Catalog= {DbName} ;Integrated Security=true;",
                                  
                                 "create database " ,

                                 "create table DataTab(" +
                                 "AUTHORIZED_CAP float not null," +
                                 "DATE_OF_REGISTRATION int not null," +
                                 "PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN varchar(300) not null)",

                                 "INSERT INTO dbo.DataTab (" +
                                 "AUTHORIZED_CAP,DATE_OF_REGISTRATION," +
                                 "PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN)" +
                                 "VALUES (@CAPT,@REGISTRATION,@PRINCIPAL_BUSINESS_ACTIVITY)"
                                };

            SqlConnection con;
            using (con = new SqlConnection(ConStr))
            {
                con.Open();
                //Create DB operation.
                CreateDb(con, DbName, queries[1]);

                //connection string for the created database.
                con = new SqlConnection(queries[0]);
                CreateTable(con,queries[2]);

                //same connection string that has sent to creation,Data storing operation.
                ReadDataToDb(Path, con,queries[3]);

                con.Close();
            }
            Console.ReadLine();
        }

        static void CreateDb(SqlConnection Con, string Db, string Query)
        {
            try
            {
                var cmd = new SqlCommand(Query+ Db, Con);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Database creation Succesfull!");
            }
            catch (SqlException e)
            {
                Console.WriteLine($"{Db} Database already exists.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        static void CreateTable(SqlConnection con, string Query)
        {
            //connecting with the created database
            con.Open();
            try
            {
                //Table Creation
                SqlCommand cmd = new SqlCommand(Query, con);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Table created.");
            }
            catch (SqlException e)
            {
                Console.WriteLine("Table already exists!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        static void ReadDataToDb(string Path, SqlConnection con, string Query)
        {
            //csv file operation 
            var strReader = new StreamReader(Path);
            var csvRead = new CsvReader(strReader, CultureInfo.InvariantCulture);

            using (var cmd = new SqlCommand(Query, con))
            {
                con.Open();
                //reading data from csv file
                var recs = csvRead.GetRecords<dynamic>();
                foreach (var rec in recs)
                {
                    string year = rec.DATE_OF_REGISTRATION;
                    int reg = 00;
                    if (!year.Equals("NA")) reg = Int32.Parse("20" + year.Substring(year.Length - 2));
                    float cap = float.Parse(rec.AUTHORIZED_CAP);
                    string pba = rec.PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN;
                    pba = pba.Replace("'","");
                    //defining parameters along with values
                    cmd.Parameters.Add("@CAPT", SqlDbType.Float).Value = cap;
                    cmd.Parameters.Add("@REGISTRATION", SqlDbType.Int).Value = reg;
                    cmd.Parameters.Add("@PRINCIPAL_BUSINESS_ACTIVITY", SqlDbType.VarChar, 300).Value = pba;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            Console.WriteLine("Successfully stored!");
            con.Close();
        }
    }
}
