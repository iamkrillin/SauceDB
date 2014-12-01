using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace PerformanceHarness.Items
{
    public class ADOHarness : IPerformanceHarness
    {
        public void InsertTestObject(TestClass item)
        {
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=Sauce;User Id=AppLogin;Password=AppLogin;"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "INSERT INTO TestClass(Name) values(@name); select scope_identity();";
                cmd.Parameters.Add("name", item.Name);
                cmd.Connection = conn;

                using (SqlDataReader v = cmd.ExecuteReader())
                {
                    v.Read();
                    item.ID = Convert.ToInt32(v[0]);
                }
            }
        }

        public void CleanUp()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=sauce;User Id=AppLogin;Password=AppLogin;"))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = string.Format("drop table TestClass;");
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE [dbo].[TestClass]([ID] [int] IDENTITY(1,1) NOT NULL,[Name] [varchar](200) NULL, CONSTRAINT [PK_dbo_TestClass] PRIMARY KEY CLUSTERED ([ID] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }
        }

        public string Name
        {
            get { return "ADO.net"; }
        }

        public TestClass ReadObject(int p)
        {
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=sauce;User Id=AppLogin;Password=AppLogin;"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select ID, Name from testclass where id=@id";
                cmd.Parameters.Add("id", p);
                cmd.Connection = conn;

                using (SqlDataReader v = cmd.ExecuteReader())
                {
                    v.Read();
                    TestClass tc = new TestClass();

                    tc.ID = Convert.ToInt32(v["ID"]);
                    tc.Name = v["Name"].ToString();
                    return tc;
                }
            }
        }
    }
}
