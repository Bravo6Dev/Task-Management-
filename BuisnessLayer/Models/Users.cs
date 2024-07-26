using DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer
{
    public class Users
    {
        enum enMode { AddNew = 1, Update = 2 }
        public enum enPermssion { All = -1, ManageMember = 1, ManageUsers = 2, 
            ManageTasks = 4, ManageProjects = 8}

        enMode Mode;

        public int UserID { get; set; }
        public int PersonID { get; set; }
        public People Person { get; set; }
        public string Password { get; set; }
        public int Permission { get; set; }
        public string PermissionState
        {
            get
            {
                if (Permission == -1)
                    return "ادمــــن";
                else
                    return "مستخـــدم";
            }
        }
        public string UserName => Person.FullName;

        public Users()
        {
            UserID = -1;
            PersonID = -1;
            Person = new People();
            Password = string.Empty;
            Permission = 0;
            Mode = enMode.AddNew;
        }
        Users(int ID, int PersonID, string Password, int Permission)
        {
            this.UserID = ID;
            this.PersonID = PersonID;
            Person = People.GetById(PersonID);
            this.Password = Password;
            this.Permission = Permission;
            Mode = enMode.Update;
        }

        private bool AddNew()
        {
            try
            {
                UserID = UsersData.AddNew(PersonID, Password, Permission);
                return UserID != -1;
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
                return UsersData.Update(UserID, Password, Permission);
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
                    else return false;
                case enMode.Update:
                    return Update();
            }
            return false;
        }

        static public bool Delete(int UserID)
        {
            try
            {
                return UsersData.Delete(UserID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public IEnumerable<Users> UsersList()
        {
            try
            {
                DataTable dt = UsersData.GetData();
                List <Users> Users = new List <Users>();

                foreach (DataRow row in dt.Rows)
                {
                    Users.Add(new Users(
                        Convert.ToInt32(row["ID"]),
                        Convert.ToInt32(row["person_id"]),
                        row["password"].ToString(),
                        Convert.ToInt32(row["Permission"])));
                }
                return Users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public People GetPersonByUser(int UserID)
        {
            Users User = GetById(UserID);
            if (User == null)
                throw new KeyNotFoundException($"User With {UserID} ID Not Found");
            return User.Person;
        }
        public static Users GetById(int userID)
        {
            string Password = string.Empty;
            int PersonID = -1, Permission = 0;
            try
            {
                return UsersData.FindById(userID, ref PersonID, ref Password, ref Permission)?
                    new Users(userID, PersonID, Password, Permission) : null;
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
                return UsersData.IsExist(PersonID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
