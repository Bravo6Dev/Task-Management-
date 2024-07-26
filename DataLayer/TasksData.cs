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
    public class TasksData
    {
        static private string Connstr = ConfigurationManager
            .ConnectionStrings["Connectionstring"]
            .ConnectionString;
        public static async Task<int> GetCount()
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = "SELECT COUNT(*) FROM Tasks";
                    await Conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        
        }
        public static int AddNew(int ProjId,int MemberId, string TaskName, byte Status,
            DateTime CreateDate, DateTime Deadline,int UserId)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"INSERT INTO [dbo].[Tasks]
                                    ([Project_id]
                                    ,[Member_id]
                                    ,[TaskName]
                                    ,[Status]
                                    ,[StartDate]
                                    ,[Deadline]
                                    ,[UserId])
                                    VALUES
                                    (@ProjectId, @MemberId, @TaskName, @Status, @CreateDate,
                                    @Deadline, @UserId);
                                    SELECT SCOPE_IDENTITY();";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ProjectId", ProjId);
                        cmd.Parameters.AddWithValue("@MemberId", MemberId);
                        cmd.Parameters.AddWithValue("@TaskName", TaskName);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@CreateDate", CreateDate);
                        cmd.Parameters.AddWithValue("@Deadline", Deadline);
                        cmd.Parameters.AddWithValue("@UserId", UserId);

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
        public static bool UpdateStatus(int TaskId, byte Status)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"UPDATE [dbo].[Tasks]
                                    SET [Status] = @Status
                                    WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", TaskId);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        return cmd.ExecuteNonQuery() > 0;
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
                    string Query = @"DELETE FROM Tasks
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
        public static bool FindById(int TaskId,ref int ProjectId,ref int MemebrId, ref string TaskName, ref byte Status,
            ref DateTime CreateDate, ref DateTime Deadline, ref int UserId)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(Connstr))
                {
                    string Query = @"SELECT * FROM Tasks
                                    WHERE ID = @ID";
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", TaskId);
                        using (SqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                ProjectId = Convert.ToInt32(Reader["Project_id"]);
                                MemebrId = Convert.ToInt32(Reader["Member_Id"]);
                                TaskName = Convert.ToString(Reader["TaskName"]);
                                Status = Convert.ToByte(Reader["Status"]);
                                CreateDate = Convert.ToDateTime(Reader["StartDate"]);
                                Deadline = Convert.ToDateTime(Reader["Deadline"]);
                                UserId = Convert.ToInt32(Reader["UserId"]);
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
    }
}
