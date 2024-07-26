using BuisnessLayer.Classes;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer
{
    public class People
    {
        enum enMode { AddNew = 1, Update = 2 }
        enMode Mode;

        public int PersonID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string FullName
        {
            get
            {
                return $"{FName} {LName}";
            }
        }
        public byte Age { get; set; }
        public string Phone { get; set; }

        public People()
        {
            PersonID = -1;
            FName = string.Empty;
            LName = string.Empty;
            Age = 0;
            Phone = string.Empty;
            Mode = enMode.AddNew;
        }
        People(int ID, string FName, string LName, byte Age, string Phone)
        {
            this.PersonID = ID;
            this.FName = FName;
            this.LName = LName;
            this.Age = Age;
            this.Phone = Phone;
            Mode = enMode.Update;
        }

        private bool AddNew()
        {
            try
            {
                if (Validate())
                {
                    PersonID = PersonData.AddNew(FName, LName, Age, Phone);
                    return PersonID != -1;
                }
                else return false;
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
                    return PersonData.Update(PersonID, FName, LName, Age, Phone);
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        bool Validate()
        {
            if (string.IsNullOrEmpty(FName) || string.IsNullOrEmpty(LName))
                throw new Exception("Name must be added");
            if (Age <= 16 || Age >= 50)
                throw new Exception("Age must be in range 16 To 50");
            if (!Validation.PhoneNumber(Phone))
                throw new Exception("Phone Number must be in pattern (e.g 091-6232949)");
            return true;
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

        static public DataTable GetPeople()
        {
            try
            {
                return PersonData.GetAllData();
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
                return PersonData.Delete(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public People GetById(int ID)
        {
            try
            {
                string FName = string.Empty, LName = string.Empty, Phone = string.Empty;
                byte Age = 0;

                return PersonData.Find(ID, ref FName, ref LName, ref Age, ref Phone) ?
                    new People(ID, FName, LName, Age, Phone) : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public List<People> PeopleList()
        {
            try
            {
                DataTable dt = PersonData.GetAllData();
                List<People> people = new List<People>();

                foreach (DataRow row in dt.Rows)
                {
                    people.Add(new People(
                        Convert.ToInt32(row["ID"]),
                        row["FName"].ToString(),
                        row["LName"].ToString(),
                        Convert.ToByte(row["Age"]),
                        row["Phone"].ToString()
                    ));
                }

                return people;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching people data", ex);
            }
        }
    }
}
