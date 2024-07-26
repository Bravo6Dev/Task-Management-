using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class PersonData
    {
        static string Connstr =
           ConfigurationManager.ConnectionStrings["Connectionstring"].ConnectionString;
        static public DataTable GetAllData()
        {
            DataTable DT = new DataTable();
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = "SELECT * FROM Person";

                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        using (SqlDataReader Reader = cmd.ExecuteReader())
                        {
                            DT.Load(Reader);
                        }
                    }
                }
                return DT;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public bool Find(int ID, ref string FName, ref string LName
            , ref byte Age, ref string Phone)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"SELECT * FROM Person
                                    WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        using (SqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                FName = Convert.ToString(Reader["FName"]);
                                LName = Convert.ToString(Reader["LName"]);
                                Age = Convert.ToByte(Reader["Age"]);
                                Phone = Convert.ToString(Reader["Phone"]);
                            }
                            return true;
                        }
                    }
                }    
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public int AddNew(string FName, string LName, byte Age, string Phone)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"INSERT INTO [dbo].[Person]
                                    ([FName]
                                    ,[LName]
                                    ,[Age]
                                    ,[Phone])
                                     VALUES
                                     (@FName, @LName, @Age, @Phone);
                                     SELECT SCOPE_IDENTITY();";

                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@FName", FName);
                        cmd.Parameters.AddWithValue("@LName", LName);
                        cmd.Parameters.AddWithValue("@Age", Age);
                        cmd.Parameters.AddWithValue("@Phone", Phone);

                        object result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int ID)) { return ID; }
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public bool Delete(int ID)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"DELETE FROM Person
                                    WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public bool Update(int ID, string FName, string LName, byte Age, string Phone)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"UPDATE [dbo].[Person]
                                   SET [FName] = @FName
                                      ,[LName] = @LName
                                      ,[Age] = @Age
                                      ,[Phone] = @Phone
                                    WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@FName", FName);
                        cmd.Parameters.AddWithValue("@LName", LName);
                        cmd.Parameters.AddWithValue("@Age", Age);
                        cmd.Parameters.AddWithValue("@Phone", Phone);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
