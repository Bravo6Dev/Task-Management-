using DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer
{
    public class Members
    {
        enum enMode { AddNew, Update}
        enMode Mode;

        public int MemberID { get; set; }
        public int PersonID { get; set; }
        public People Person { get; set; }
        public string Department { get; set; }
        public byte Semester { get; set; }

        public string Name 
        { 
            get
            {
                return Person.FullName;
            }
        }

        public Members()
        {
            MemberID = -1;
            PersonID = -1;
            Person = new People();
            Department = string.Empty;
            Semester = 0;
            Mode = enMode.AddNew;
        }
        Members(int MemberID, int PersonID, string Department, byte Semester)
        {
            this.MemberID = MemberID;
            this.PersonID = PersonID;
            Person = People.GetById(PersonID);
            this.Department = Department;
            this.Semester = Semester;
            Mode = enMode.Update;
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(Department))
                throw new Exception("Department Must be added");
            if (Semester < 1 || Semester > 8)
                throw new Exception("Semester must be in range from 1 to 8");
            return true;
        }
        private bool AddNew()
        {
            try
            {
                if (Validate())
                {
                    MemberID = MembersData.AddNew(PersonID, Department, Semester);
                    return MemberID != -1;
                }
                else
                    return false;
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
                if (Validate())
                    return MembersData.Update(MemberID, Department, Semester);
                else return false;
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
                    else
                        return false;
                case enMode.Update:
                    return Update();
            }
            return false;
        }

        static public IEnumerable<Members> GetMembersList()
        {
            try
            {
                List <Members> members = new List <Members>(); 
                DataTable DT = MembersData.GetData();
                foreach (DataRow dr in DT.Rows)
                {
                    members.Add(new Members(
                        Convert.ToInt32(dr["ID"]),
                        Convert.ToInt32(dr["PersonID"]),
                        dr["Department"].ToString(),
                        Convert.ToByte(dr["Semester"])));
                }
                return members;
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
                return MembersData.Delete(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public Members GetById(int ID)
        {
            string Department = string.Empty;
            int PersonID = -1;
            byte Semester = 0;
            try
            {
                return MembersData.FindById(ID, ref PersonID, ref Department, ref Semester) ?
                    new Members(ID, PersonID, Department, Semester) : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
