using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ConfigModel
    {
        public int TaskId { get; set; }
        public int TaskCount { get; set; }
        public string ServerEvetName { get; set; }
        public bool OnlyHumanoid { get; set; }
    }
}
