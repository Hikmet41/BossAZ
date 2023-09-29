using lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVVV
{
    public class CV
    {
        public string Specialty { get; set; }
        public string School { get; set; }
        public int UniversityEntryYear { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public List<string> Companies { get; set; } = new List<string>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Language> Languages { get; set; } = new List<Language>();
        public bool HasDiploma { get; set; }
        public string GitLink { get; set; }
        public string LinkedIn { get; set; }
    }


}
