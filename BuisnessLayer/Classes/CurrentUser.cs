using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Classes
{
    public class CurrentUser
    {
        static public Users User { get; set; }
        static public DateTime GetDateTime => DateTime.Now;

        static public bool CheckPermission(Users.enPermssion Permission)
            => (User.Permission & (int)Permission) == (int)Permission;
    }
}
