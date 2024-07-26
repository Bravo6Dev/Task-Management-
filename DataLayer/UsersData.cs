using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UsersData
    {
        static string Connstr = ConfigurationManager.ConnectionStrings["Connectionstring"].ConnectionString;
        private static DataTable ExecuteQueryAsync(string query)
        {
            try
            {
                using (var conn = new SqlConnection(Connstr))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var dt = new DataTable();
                            dt.Load(reader);
                            return dt;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
        public static DataTable GetData()
        {
            var query = @"SELECT 
                          Users.ID,
                          person_id,
                          password,
                          permission
                          FROM Users
                          JOIN Person ON Users.person_id = Person.ID";
            return  ExecuteQueryAsync(query);
        }
        public static int AddNew(int PersonID, string Password, int Permission)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"INSERT INTO [dbo].[Users]
                                    ([person_id]
                                    ,[password]
                                    ,[permission])
                                     VALUES
                                     (@person_id, @Password, @permission);
                                     SELECT SCOPE_IDENTITY();";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@person_id", PersonID);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        cmd.Parameters.AddWithValue("@permission", Permission);

                        object result = cmd.ExecuteNonQuery();
                        if (result != null && int.TryParse(result.ToString(), out int ID))
                            return ID;
                        else
                            return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool CheckPassword(int ID, string Password)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"SELECT password FROM Users
                                    WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            return result.ToString() == Password;
                        else
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool Delete(int ID)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"DELETE FROM Users
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
        public static bool FindById(int ID, ref int PersonID, ref string Password, ref int Permission)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"SELECT * FROM Users
                                    WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        using (SqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                PersonID = Convert.ToInt32(Reader["person_id"]);
                                Password = Convert.ToString(Reader["Password"]);
                                Permission = Convert.ToInt32(Reader["Permission"]);
                            }
                            else
                                return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool Update(int ID, string Password, int Permission)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"UPDATE [dbo].[Users]
                                     SET [password] = @Password
                                    ,[permission] = @Permission
                                     WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        cmd.Parameters.AddWithValue("@Permission", Permission);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool IsExist(int PersonID)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"SELECT FOUND = 1 FROM Users
                                    WHERE person_id = @ID";
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", PersonID);
                        return cmd.ExecuteScalar() != null;
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
