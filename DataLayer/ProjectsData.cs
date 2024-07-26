using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataLayer
{
    public class ProjectsData
    {
        static private string Connstr
            = ConfigurationManager.ConnectionStrings["Connectionstring"].ConnectionString;

        static async public Task<DataTable> GetDataAsync()
        {
            try
            {
                DataTable DT = new DataTable();
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = "SELECT * FROM Projects";
                    await Conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        using (SqlDataReader Reader = await cmd.ExecuteReaderAsync())
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
        static public int AddNew(string ProjectName, DateTime CreatedAt, int CreatedBy
            , string Description, byte Status)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"INSERT INTO [dbo].[Projects]
                                    ([ProjectName]
                                    ,[CreatedAt]
                                    ,[CreatedBy]
                                    ,[Description]
                                    ,[Status])
                                     VALUES
                                    (@Name, @CreatedAt, @Createdby, @Description, @Status);
                                    SELECT SCOPE_IDENTITY();";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", ProjectName);
                        cmd.Parameters.AddWithValue("@CreatedAt", CreatedAt);
                        cmd.Parameters.AddWithValue("@Createdby", CreatedBy);
                        if (string.IsNullOrEmpty(Description))
                            cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@Status", Status);
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
        static public bool Update(int ID, byte Status)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"UPDATE [dbo].[Projects]
                                   SET [Status] = @Status
                                   WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", Status);
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
        static public bool Delete(int ID)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"DELETE FROM Projects
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
        static public async Task<DataTable> GetTopNAsync(int num)
        {
            DataTable DT = new DataTable();
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = $@"SELECT TOP {num} * FROM Projects
                                    ORDER BY CreatedAt DESC";
                    await Conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        using (SqlDataReader Reader = await cmd.ExecuteReaderAsync())
                            DT.Load(Reader);
                    }
                }
                return DT;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public bool GetbyId(int ID, ref string Name, ref DateTime CreateDate,
            ref int CreatedBy, ref string Description, ref byte Status)
        {
            try
            {
                DataTable DT = new DataTable();
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = "SELECT * FROM Projects WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        using (SqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                Name = Reader["ProjectName"].ToString();
                                CreateDate = Convert.ToDateTime(Reader["CreatedAt"]);
                                CreatedBy = Convert.ToInt32(Reader["CreatedBy"]);
                                Description = Reader["Description"].ToString();
                                Status = Convert.ToByte(Reader["Status"]);
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
        static public async Task<DataTable> GetMembersAsync(int ProjectID)
        {
            try
            {
                DataTable DT = new DataTable();
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"SELECT 
                                    Person.ID,
                                    FName,
                                    LName, 
                                    Phone,
                                    Members.Department
                                    FROM Person
                                    JOIN Members ON Person.ID = Members.ID
                                    JOIN MembersInProjects ON MembersInProjects.PersonID = Person.ID
                                    WHERE ProjectID = @ID";

                    await Conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ProjectID);
                        using (SqlDataReader Reader = await cmd.ExecuteReaderAsync())
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
        static public bool AddNewMember(int ProjectID, int PersonID)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"INSERT INTO [dbo].[MembersInProjects]
                                    ([PersonID]
                                    ,[ProjectID])
                                    VALUES
                                    (@PersonID, @ProjectID);
                                    SELECT SCOPE_IDENTITY();";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@PersonID", PersonID);
                        cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                        return cmd.ExecuteScalar() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public bool DeleteMember(int ProjectID, int PersonID)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"DELETE FROM MembersInProjects
                                   WHERE ProjectID = @ProjectID AND PersonID = @PersonID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                        cmd.Parameters.AddWithValue("@PersonID", PersonID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public bool IsMemberExist(int ProjectID, int PersonID)
        {
            try
            {
                DataTable DT = new DataTable();
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"SELECT Found = 1 FROM MembersInProjects 
                                    WHERE ProjectID = @ProjectID AND PersonID = @PersonID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                        cmd.Parameters.AddWithValue("@PersonID", PersonID);
                        return cmd.ExecuteScalar() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public async Task<DataTable> GetTasksAsync(int ProjectID)
        {
            try
            {
                DataTable DT = new DataTable();
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"SELECT * FROM Tasks
                                    WHERE Project_id = @ID";

                    await Conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ProjectID);
                        using (SqlDataReader Reader = await cmd.ExecuteReaderAsync())
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
    }
}
