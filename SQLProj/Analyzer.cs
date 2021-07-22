using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ConsoleTables;

namespace SQLProj
{
    class Analyzer
    {
        public static void Analyze(String DbName)
        {
            string conStr = $"Data Source=DESKTOP-KTRBOEN;Initial Catalog= {DbName} ;Integrated Security=true;";
            SqlConnection con = new SqlConnection(conStr); ;
            
            try
            {
                con.Open();
                Solution(con);
            }
            //To handle the Sql exception
            catch (SqlException se)
            {
                Console.WriteLine(se.Message);
            }
            //To handle the general operation
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //To perform closing activity
            finally
            {
                con.Close();
            }

        }

        static void Solution(SqlConnection con)
        {
            //capital range
            var authCap = new Dictionary<string, int>();

            //no. of regs per year
            var regs = new Dictionary<int, int>();

            //no. times a particular comp reg in 2015
            var prncAct = new Dictionary<string, int>();

            //particular comp's regs in each year from 2000 to 2019
            var compGrpd = new SortedDictionary<int,Dictionary<string,int>>();

            //Queries are stored in array to avoid the repeating statments of sqlConnect and execute operations.
            string[] queries = {"Select count(AUTHORIZED_CAP)," +
                              "case " +
                              "when AUTHORIZED_CAP between  0 and  100000 then ' <=1L '" +
                              "when AUTHORIZED_CAP between 100000 and 1000000 then '1L to 10L' " +
                              "when AUTHORIZED_CAP between 1000000 and 10000000 then '10L to 1Cr' " +
                              "when AUTHORIZED_CAP between 10000000 and 100000000 then '1Cr to 10Cr' " +
                              "when AUTHORIZED_CAP >100000000 then '>10cr' " +
                              "end as [Numbers] " +
                              "from dbo.DataTab " +
                              "Group by AUTHORIZED_CAP " +
                              "order by AUTHORIZED_CAP" ,

                              "select DATE_OF_REGISTRATION," +
                              "count(*) as registration " +
                              "from dbo.DataTab " +
                              "where DATE_OF_REGISTRATION Between 2000 and 2019 " +
                              "group by DATE_OF_REGISTRATION " +
                              "order by DATE_OF_REGISTRATION",

                              "select PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN, " +
                              "count(PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN) " +
                              "from dbo.DataTab " +
                              "where DATE_OF_REGISTRATION = 2015 " +
                              "group by PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN",

                              "select DATE_OF_REGISTRATION," +
                              "PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN," +
                              "count(PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN) " +
                              "from dbo.DataTab " +
                              "where DATE_OF_REGISTRATION between 2000 and 2019 " +
                              "group by PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN,DATE_OF_REGISTRATION " +
                              "order by DATE_OF_REGISTRATION"
                };

            try
            {
                for (int i = 0; i < queries.Length; i++)
                {
                    var com = new SqlCommand(@queries[i], con);
                    SqlDataReader rec = com.ExecuteReader();
                    if (i == 0)
                    {
                        while(rec.Read())
                        {
                            string key = Convert.ToString(rec[1]);
                            int val = Convert.ToInt32(rec[0]);

                            if (!authCap.ContainsKey(key)) authCap.Add(key, val);
                            else authCap[key] = authCap[key]+val;
                        }
                    }
                    else if (i == 1)
                    {
                        while (rec.Read()) regs.Add(Convert.ToInt32(rec[0]), Convert.ToInt32(rec[1]));   
                    }
                    else if (i == 2)
                    {
                        while (rec.Read()) prncAct.Add(Convert.ToString(rec[0]), Convert.ToInt32(rec[1]));
                    }
                    else
                    {
                        while (rec.Read())
                        {
                            int year = Convert.ToInt32(rec[0]);
                            if (!compGrpd.ContainsKey(year)) compGrpd.Add(Convert.ToInt32(rec[0]), new Dictionary<string, int>());
                            else compGrpd[year].Add(Convert.ToString(rec[1]), Convert.ToInt32(rec[2])); 
                        }
                    }
                    rec.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Console Table Section.
            //tab 1
            Console.WriteLine("Solution for 1st problem:");
            var tab1 = new ConsoleTable("Bin", "Counts");
            foreach (KeyValuePair<string, int> kv in authCap)
            {
                tab1.AddRow(kv.Key, kv.Value);
            }
            tab1.Write();

            //tab 2
            Console.WriteLine("Solution for 2nd probelm:");
            var tab2 = new ConsoleTable("Year","Comp_Regs_Per_Year");
            foreach(KeyValuePair<int,int> kv in regs)
            {
                tab2.AddRow(kv.Key,kv.Value);
            }
            tab2.Write();

            //tab 3
            Console.WriteLine("Solution for 3rd problem:");
            var tab3 = new ConsoleTable("PRINCIPAL_BUSINESS_ACTIVITY-2015", "count");
            foreach (KeyValuePair<string,int> kv in prncAct)
            {
                tab3.AddRow(kv.Key,kv.Value);
            }
            tab3.Write();

            //tab 4
            Console.WriteLine("Solution for 4th problem:");
            var tab4 = new ConsoleTable("PRINCIPAL_BUSINESS_ACT", "count");
            foreach (var key in compGrpd.Keys)
            {
                tab4.AddRow(key,"");
                var values = compGrpd[key];
                foreach (var val in values)
                {
                    tab4.AddRow(val.Key,val.Value);
                }
            }
            tab4.Write();
            Console.WriteLine();
        }
    }
}
