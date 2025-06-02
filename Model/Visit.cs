using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolMedUMG.Model
{
    public class Visit
    {
        public string causeOfVisit { get; set; }
        public string additionalInfo { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfVisit { get; set; }
        public string serviceName { get; set; }
    }
}
