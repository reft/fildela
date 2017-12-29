using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fildela.Web.Models.UserModels
{
    public class AuthenticationProviderModel
    {
        public int AuthenticationProviderID { get; set; }

        public string Name { get; set; }

        public string IconClass { get; set; }

        public string IconColor { get; set; }

        public bool IsLinked { get; set; }
    }
}