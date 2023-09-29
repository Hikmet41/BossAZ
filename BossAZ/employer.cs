using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using user;
using vacancy;
using worker;

namespace emp
{
    public class Employer : User
    {
        public List<Vacancy> Vacancies { get; set; } = new List<Vacancy>();

        public void ReceiveApplication(Worker worker, Vacancy vacancy, string applicationText)
        {

        }
    }
}
