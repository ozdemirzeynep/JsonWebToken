﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi2
{
    public interface IUserManagementService
    {
        bool IsValidUser(string userName, string password);
    }
}
