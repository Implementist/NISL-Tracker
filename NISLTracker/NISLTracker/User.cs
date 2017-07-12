using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NISLTracker
{
    public class User
    {
        public string UserName { get; set; }
        public string AuthorizationCode { get; set; }
        public string SecurityStamp { get; set; }
        public string Identity { get; set; }
        public int Laboratory { get; set; }
    }
}
