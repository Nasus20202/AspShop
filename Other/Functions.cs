using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class Functions
    {
        public static int PermissionLevel(string role)
        {
            if (role == null)
                return 0;
            role = role.ToLower();
            if (role == "admin")
                return 5;
            if (role == "manager")
                return 4;
            if (role == "editor")
                return 3;
            if (role == "employee")
                return 2;
            return 1;
        }
    }
}
