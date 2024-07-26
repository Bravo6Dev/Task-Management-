using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Models
{
    public class Tasks
    {
        public enum enStatus { NotStart = 1, OnProgress = 2, Finish = 3}

        enum enMode { AddNew, Update}
        enMode Mode;

        public int TaskId { get; set; }
        public int Project_id { get; set; }
        public int Memebr_id { get; set; }
        public Projects Project { get; set; }
        public string TaskName { get; set; }
        public enStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Deadline { get; set; }
        public int UserId { get; set; }

        public string GetStatus 
        {
            get
            {
                switch (Status)
                {
                    case Tasks.enStatus.NotStart:
                        return "لم يتم البدء";
                    case Tasks.enStatus.OnProgress:
                        return "قــيـد الــتطـويـر";
                    case Tasks.enStatus.Finish:
                        return "تم الانـتــهـاء";
                }
                return string.Empty;
            }
        }

        public Tasks()
        {
            TaskId = -1;
            Project_id = -1;
            Memebr_id = -1;
            Project = new Projects();
            TaskName = string.Empty;
            CreateDate = DateTime.MinValue;
            Deadline = DateTime.MaxValue;
            UserId = -1;
            Mode = enMode.AddNew;   
        }
        public Tasks(int TaskId, int ProjectId,int Memebr_id, string TaskName, 
            byte Status, DateTime CreateDate, DateTime Deadline, int UserId)
        {
            this.TaskId = TaskId;
            this.Project_id = ProjectId;
            this.Memebr_id = Memebr_id;
            Project = Projects.GetById(ProjectId);
            this.TaskName = TaskName;
            this.Status = (enStatus)Status;
            this.CreateDate = CreateDate;
            this.Deadline = Deadline;
            this.UserId = UserId;
            Mode = enMode.Update;
        }

        private bool AddNew()
        {
            try
            {
                TaskId = TasksData.AddNew(Project_id,Memebr_id, TaskName, (byte)Status, CreateDate,
                    Deadline, UserId);
                return TaskId != -1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool UpdateStatus()
        {
            try
            {
                return TasksData.UpdateStatus(TaskId, (byte)Status);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (AddNew())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else  return false;
                case enMode.Update:
                    return UpdateStatus();
            }
            return false;
        }

        static public Tasks GetById(int id)
        {
            try
            {
                int ProjId = -1, UserId = -1, MemberId = -1;
                byte Status = 0;
                string TaskName = string.Empty;
                DateTime CraeteDate = DateTime.MinValue, Deadline = DateTime.MinValue;
                return TasksData.FindById(id, ref ProjId, ref MemberId,ref TaskName, ref Status, ref CraeteDate, ref Deadline, ref UserId) ?
                    new Tasks(id, ProjId, MemberId, TaskName, Status, CraeteDate, Deadline, UserId) : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public bool Delete(int TaskId)
        {
            try
            {
                return TasksData.Delete(TaskId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public async Task<int> GetCount()
        {
            try
            {
                return await TasksData.GetCount();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
