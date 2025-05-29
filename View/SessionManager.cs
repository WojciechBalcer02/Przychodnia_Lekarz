using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolMedUMG.View
{
    public static class SessionManager
    {
        public static string CurrentUsername { get; set; }
        public static string connStrSQL { get; set; }
        public static string accType { get; set; }
    }
}
