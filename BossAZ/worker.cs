using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using user;

namespace worker
{
    public class Worker : User
    {
        public string City { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }
        public List<CV> CVs { get; set; } = new List<CV>();
    }

}
