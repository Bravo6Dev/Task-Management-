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
    public class MembersData
    {
        static string Connstr =
            ConfigurationManager.ConnectionStrings["Connectionstring"].ConnectionString;
        static public DataTable GetData()
        {
            try
            {
                DataTable DT = new DataTable();
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"SELECT 
                                    Members.ID,
                                    PersonID,
                                    FName + ' ' + LName As FullName,
                                    Age,
                                    Phone,
                                    Department,
                                    Semester
                                    FROM Members
                                    JOIN Person ON Person.ID = Members.PersonID";
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
        static public int AddNew(int PersonID, string Department, byte Semester)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"INSERT INTO [dbo].[Members]
                                    ([PersonID]
                                    ,[Department]
                                    ,[Semester])
                                     VALUES
                                    (@PersonID, @Department, @Semester);
                                    SELECT SCOPE_IDENTITY();";
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@PersonID", PersonID);
                        cmd.Parameters.AddWithValue("@Department", Department);
                        cmd.Parameters.AddWithValue("@Semester", Semester);

                        object result = cmd.ExecuteScalar();
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
        static public bool Update(int ID, string Department, byte Semester)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"UPDATE [dbo].[Members]
                                     SET [Department] = @Department 
                                        ,[Semester] = @Semester
                                     WHERE ID = @ID";
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@Department", Department);
                        cmd.Parameters.AddWithValue("@Semester", Semester);

                        return cmd.ExecuteNonQuery() > 0;
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
            using (SqlConnection Conn = new SqlConnection(Connstr))
            {
                string Query = @"DELETE FROM Members
                                WHERE ID = @ID";
                Conn.Open();

                using (SqlCommand cmd = new SqlCommand(Query,Conn))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        static public bool FindById(int ID, ref int PersonID, ref string Department, ref byte Semester)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = "SELECT * FROM Members WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        using (SqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                PersonID = Convert.ToInt32(Reader["PersonID"]);
                                Department = Reader["Department"].ToString();
                                Semester = Convert.ToByte(Reader["Semester"]);
                                return true;
                            }
                            else
                                return false;
                        }
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
