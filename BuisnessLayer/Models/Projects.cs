using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Models
{
    public class Projects
    {
        public enum enStatus { NotStart = 1, OnProgress = 2, Finish = 3}
        enum enMode { AddNew, Update }
        enMode Mode;

        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public enStatus Status { get; set; }
        public List<Members> MembersInProject { get; set; }
        /// <summary>
        /// Get Data Depends On Status enum
        /// </summary>
        public string GetStatus 
        {
            get
            {
                switch (Status)
                {
                    case enStatus.NotStart:
                        return "لم يتم البدء";
                    case enStatus.OnProgress:
                        return "قيد التطوير";
                    case enStatus.Finish:
                        return "تم الانتهاء";
                }
                return string.Empty;
            }
        }

        public Projects()
        {
            ProjectID = -1;
            ProjectName = string.Empty;
            CreatedDate = new DateTime();
            UserId = -1;
            Description = string.Empty;
            MembersInProject = new List<Members>();
            Mode = enMode.AddNew;
        }
        Projects(int ProjectID, string ProjectName, DateTime CreatedDate,
            int UserId, string Description, byte Status)
        {
            this.ProjectID = ProjectID;
            this.ProjectName = ProjectName;
            this.CreatedDate = CreatedDate;
            this.UserId = UserId;
            this.Description = Description;
            this.Status = (enStatus)Status;
            // We need to implement the members from the member in project table
            Mode = enMode.Update;
        }

        private bool AddNew()
        {
            try
            {
                ProjectID = ProjectsData.AddNew(ProjectName, CreatedDate,
                    UserId, Description, (byte)Status);
                return ProjectID != -1;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
        private bool Update()
        {
            try
            {
                return ProjectsData.Update(ProjectID, (byte)Status);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Save()
        {
            try
            {
                switch (Mode)
                {
                    case enMode.AddNew:
                        if (AddNew())
                        {
                            Mode = enMode.Update;
                            return true;
                        }
                        else return false;
                    case enMode.Update:
                        return Update();
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get The Members In This Project
        /// </summary>
        public async Task<DataTable> GetMembersAsync()
        {
            try
            {
                return await ProjectsData.GetMembersAsync(ProjectID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Add New Member To Project
        /// </summary>
        /// <param name="ID">MemberID To Added To Project</param>
        public bool AddMember(int ID)
        {
            try
            {
                return ProjectsData.AddNewMember(ProjectID, ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Delete Member From Project
        /// </summary>
        /// <param name="ID">Memeber ID</param>
        public bool DeleteMember(int ID)
        {
            try
            {
                return ProjectsData.DeleteMember(ProjectID, ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Check If This Memeber Exist in This Project Or Not
        /// </summary>
        public bool IsMemberExist(int MemberID)
        {
            try
            {
                return ProjectsData.IsMemberExist(ProjectID, MemberID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// When we Add New Project There is Possible We Have More Than
        /// Members So We Saved Depends On AddMemeber Method
        /// </summary>
        /// <returns></returns>
        public bool SaveMembers()
        {
            if (MembersInProject.Count == 0) return false;
            foreach (Members member in MembersInProject)
            {
                if (!AddMember(member.MemberID))
                    return false;
            }
            return true;
        }

        public async Task<List<Tasks>> GetTasksAsync()
        {
            try
            {
                DataTable DT = await ProjectsData.GetTasksAsync(ProjectID);
                List<Tasks> tasks = new List<Tasks>();
                foreach (DataRow dr in DT.Rows)
                    tasks.Add(
                        new Tasks(Convert.ToInt32(dr["ID"]),
                        Convert.ToInt32(dr["Project_id"]),
                        Convert.ToInt32(dr["Member_id"]),
                        dr["TaskName"].ToString(),
                        Convert.ToByte(dr["Status"]),
                        Convert.ToDateTime(dr["StartDate"]),
                        Convert.ToDateTime(dr["Deadline"]),
                        Convert.ToInt32(dr["UserId"])));
                return tasks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public Projects GetById(int ProjectID)
        {
            if (ProjectID == -1)
                return null;
            try
            {
                string Name = string.Empty, Description = string.Empty;
                byte Status = 0; int CreatedBy = -1;
                DateTime CreatedDate = new DateTime();
                return ProjectsData.GetbyId(ProjectID, ref Name, ref CreatedDate
                    , ref CreatedBy, ref Description, ref Status) ?
                    new Projects(ProjectID, Name, CreatedDate, CreatedBy,
                    Description, Status) : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public bool Delete(int ProjectID)
        {
            try
            {
                return ProjectsData.Delete(ProjectID);
            }
            catch (Exception ex)
            { throw ex; }
        }
        static public async Task<List<Projects>> GetProjects()
        {
            try
            {
                DataTable DT = await ProjectsData.GetDataAsync();
                List<Projects> Projects = new List<Projects>();
                foreach (DataRow dr in DT.Rows)
                {
                    Projects.Add(new Projects(
                        Convert.ToInt32(dr["ID"]),
                        dr["ProjectName"].ToString(),
                        Convert.ToDateTime(dr["CreatedAt"]),
                        Convert.ToInt32(dr["CreatedBy"]),
                        dr["Description"].ToString(),
                        Convert.ToByte(dr["Status"])));
                }
                return Projects;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public async Task<List<Projects>> GetNProjects(int num)
        {
            if (num <= 0)
                throw new ArgumentException("numbers of rows must be more than 0");
            try
            {
                DataTable DT = await ProjectsData.GetTopNAsync(num);
                List<Projects> Projects = new List<Projects>();
                foreach (DataRow dr in DT.Rows)
                {
                    Projects.Add(new Projects(
                        Convert.ToInt32(dr["ID"]),
                        dr["ProjectName"].ToString(),
                        Convert.ToDateTime(dr["CreatedAt"]),
                        Convert.ToInt32(dr["CreatedBy"]),
                        dr["Description"].ToString(),
                        Convert.ToByte(dr["Status"])));
                }
                return Projects;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
