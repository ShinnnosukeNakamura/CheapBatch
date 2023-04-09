using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLinkTest.Model
{
    public class TaskSettings
    {
        public string TaskDirectory { get; set; }
        public string InProgressDirectory { get; set; }
        public string CompletedDirectory { get; set; }
        public string FailedDirectory { get; set; }

    }
}
