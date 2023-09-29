using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using worker;
using user;
using vacancy;
using lang;
using emp;
using System.Security.Principal;
using System.ComponentModel.Design;

class Program
{
    static void Main(string[] args)
    {
        List<string> menuItems = new List<string>
        {
            "login",
            "register",
            "exit",
        };

        int selectedItemIndex = 0;

        while (true)
        {
            Console.Clear();

            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedItemIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.WriteLine(menuItems[i]);
                Console.ResetColor();
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedItemIndex = Math.Max(0, selectedItemIndex - 1);
                    break;
                case ConsoleKey.DownArrow:
                    selectedItemIndex = Math.Min(menuItems.Count - 1, selectedItemIndex + 1);
                    break;
                case ConsoleKey.Enter:
                    if (selectedItemIndex == menuItems.Count - 1)
                    {
                        return;
                    }
                    else
                    {
                        Console.Clear();
                        if(selectedItemIndex== 0)
                        {
                            Login();
                        }
                        else if(selectedItemIndex == 1)
                        {
                            Register();
                        }
                        else if(selectedItemIndex == 2)
                        {
                            Environment.Exit(0);
                        }


                    }
                    break;
            }
        }
    }
    private static List<Worker> workers = new List<Worker>();
    private static List<Employer> employers = new List<Employer>();
    private static User currentUser;

    public static void Login()
    {
        Console.WriteLine("username: ");
        string username = Console.ReadLine();
        Console.WriteLine("password: ");
        string password = Console.ReadLine();
        Console.WriteLine("role");
        string role = Console.ReadLine();

        currentUser = AuthenticateUser(username, password, role);
        if(role == "emp")
        {
            EmployerMenu();
        }
        else WorkerMenu();


        if (currentUser == null)
        {
            Console.WriteLine("error");
        }
    }

    private static User AuthenticateUser(string username, string password , string role)
    {
        User user = workers.FirstOrDefault(w => w.Username == username && w.Password == password && w.Role == role);

        if (user == null)
        {
            user = employers.FirstOrDefault(e => e.Username == username && e.Password == password && e.Role == role);
        }
        return user;
    }




    public static void Register()
    {
        Console.WriteLine("username ");
        string username = Console.ReadLine();
        Console.WriteLine("Şifre: ");
        string password = Console.ReadLine();
        Console.WriteLine("Rolünüz (Worker/Employer): ");
        string role = Console.ReadLine();

        if (role.ToLower() == "w")
        {
            Worker worker = new Worker();
            worker.Username = username;
            worker.Password = password;
            worker.Role = role;
            workers.Add(worker);
            currentUser = worker;
            WorkerMenu();

        }
        else if (role.ToLower() == "emp")
        {
            Employer employer = new Employer();
            employer.Username = username;
            employer.Password = password;
            employer.Role = role;
            employers.Add(employer);
            currentUser = employer;
            EmployerMenu();
        }
        else
        {
            Console.WriteLine("Geçersiz rol.");
        }
    }

    private static void WorkerMenu()
    {
        List<string> menuItems = new List<string>
        {
            "CV",
            "ApplyToVacancy",
            "ViewWorkerInfo",
            "Çıkış"
        };

        int selectedItemIndex = 0;

        while (true)
        {
            Console.Clear();

            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedItemIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.WriteLine(menuItems[i]);
                Console.ResetColor();
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedItemIndex = Math.Max(0, selectedItemIndex - 1);
                    break;
                case ConsoleKey.DownArrow:
                    selectedItemIndex = Math.Min(menuItems.Count - 1, selectedItemIndex + 1);
                    break;
                case ConsoleKey.Enter:
                    if (selectedItemIndex == menuItems.Count - 1)
                    {
                        return;
                    }
                    else
                    {
                        Console.Clear();
                        if (selectedItemIndex == 0)
                        {
                            CreateCV();
                        }
                        else if (selectedItemIndex == 1)
                        {
                            ApplyToVacancy();
                        }
                        else if (selectedItemIndex == 2)
                        {
                            ViewWorkerInfo();
                        }

                    }
                    break;
            }
        }
    }

    private static void CreateCV()
    {
        Worker currentWorker = (Worker)currentUser;
        CV cv = new CV();

        Console.WriteLine("İxtisas: ");
        cv.Specialty = Console.ReadLine();

        Console.WriteLine("Mekteb: ");
        cv.School = Console.ReadLine();

        Console.WriteLine("uni qebul olma ili: ");
        cv.UniversityEntryYear = int.Parse(Console.ReadLine());

        Console.WriteLine("bacariqlar (Vergulle Ayırın): ");
        string skills = Console.ReadLine();
        cv.Skills = skills.Split(',').ToList();

        Console.WriteLine("sirketler (Vergulle Ayırın): ");
        string companies = Console.ReadLine();
        cv.Companies = companies.Split(',').ToList();

        Console.WriteLine("is baslama tarixi (YYYY-MM-DD): ");
        cv.StartDate = DateTime.Parse(Console.ReadLine());

        Console.WriteLine("İs bitme tarixi (YYYY-MM-DD): ");
        cv.EndDate = DateTime.Parse(Console.ReadLine());

        Console.WriteLine("bildiyiniz diller (Dil, Seviye): ");
        string languages = Console.ReadLine();
        string[] languageData = languages.Split(',');
        foreach (var data in languageData)
        {
            string[] languageInfo = data.Trim().Split(' ');
            if (languageInfo.Length == 2)
            {
                cv.Languages.Add(new Language { Name = languageInfo[0], Level = languageInfo[1] });
            }
        }

        Console.WriteLine("Diplom var mı? (yes/no): ");
        string diplom = Console.ReadLine();
        cv.HasDiploma = diplom.ToLower() == "yes";
        currentWorker.CVs.Add(cv);
    }

    private static void ApplyToVacancy()
    {
        Worker currentWorker = (Worker)currentUser;

        Console.WriteLine("is elanlari:");
        List<Employer> eligibleEmployers = GetEligibleEmployers();
        int i = 1;
        foreach (Employer employer in eligibleEmployers)
        {
            foreach (Vacancy vacancy in employer.Vacancies)
            {
                Console.WriteLine($"{i}. {employer.Username} - {vacancy.Title}");
                i++;
            }
        }

        Console.WriteLine("cv gondermek istediyiiz elani yazin (Çıkış için 0): ");
        int choice = int.Parse(Console.ReadLine());

        if (choice == 0)
        {
            return;
        }

        if (choice >= 1 && choice <= i)
        {
            int selectedEmployerIndex = -1;
            int selectedVacancyIndex = choice - 1;
            for (int j = 0; j < eligibleEmployers.Count; j++)
            {
                int vacanciesCount = eligibleEmployers[j].Vacancies.Count;
                if (selectedVacancyIndex < vacanciesCount)
                {
                    selectedEmployerIndex = j;
                    break;
                }
                else
                {
                    selectedVacancyIndex -= vacanciesCount;
                }
            }

            Employer selectedEmployer = eligibleEmployers[selectedEmployerIndex];
            Vacancy selectedVacancy = selectedEmployer.Vacancies[selectedVacancyIndex];
            static void ApplyToSelectedVacancy(Worker worker, Employer employer, Vacancy vacancy)
            {
                Console.WriteLine("basvurun yazin:");
                string applicationText = Console.ReadLine();

                employer.ReceiveApplication(worker, vacancy, applicationText);

                Console.WriteLine("Basvurnuz gönderildi.");
            }

        }
        else
        {
            Console.WriteLine("ERROR.");
        }
    }

    private static List<Employer> GetEligibleEmployers()
    {
        Worker currentWorker = (Worker)currentUser;
        List<Employer> eligibleEmployers = new List<Employer>();

        foreach (CV cv in currentWorker.CVs)
        {
            foreach (Employer employer in employers)
            {
                foreach (Vacancy vacancy in employer.Vacancies)
                {
                    if (cv.Specialty == vacancy.Title)
                    {
                        eligibleEmployers.Add(employer);
                    }
                }
            }
        }

        return eligibleEmployers;
    }

    public static void ApplyToSelectedVacancy(Worker worker, Employer employer, Vacancy vacancy)
    {
        Console.WriteLine("Basvuru yazin:");
        string applicationText = Console.ReadLine();

        employer.ReceiveApplication(worker, vacancy, applicationText);

    }

    private static void ViewWorkerInfo()
    {
        Worker currentWorker = (Worker)currentUser;

        Console.WriteLine("isci haqqinda :");
        Console.WriteLine($"Ad: {currentWorker.Username}");
        Console.WriteLine($"password: {currentWorker.Password}");

        Console.WriteLine("arxiv:");
        foreach (CV cv in currentWorker.CVs)
        {
            Console.WriteLine($"İxtisas: {cv.Specialty}");
        }
    }

    private static void EmployerMenu()
    {
        List<string> menuItems = new List<string>
        {
            "create vacancy",
            "apply",
            "exit"
        };

        int selectedItemIndex = 0;

        while (true)
        {
            Console.Clear();

            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedItemIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.WriteLine(menuItems[i]);
                Console.ResetColor();
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedItemIndex = Math.Max(0, selectedItemIndex - 1);
                    break;
                case ConsoleKey.DownArrow:
                    selectedItemIndex = Math.Min(menuItems.Count - 1, selectedItemIndex + 1);
                    break;
                case ConsoleKey.Enter:
                    if (selectedItemIndex == menuItems.Count - 1)
                    {
                        return;
                    }
                    else
                    {
                        if(selectedItemIndex == 0)
                        {
                            CreateVacancy();
                        }
                        else if(selectedItemIndex == 1)
                        {
                            ApplyToVacancy();
                        }
                        else if(selectedItemIndex == 2)
                        {
                            currentUser = null;
                        }
                    }
                    break;
            }
        }
    }

    private static void ViewEmployerInfo()
    {
        Employer currentEmployer = (Employer)currentUser;

        Console.WriteLine("İşveren Bilgileri:");
        Console.WriteLine($"Ad: {currentEmployer.Username}");
        Console.WriteLine($"Şifre: {currentEmployer.Password}");
    }


    private static void CreateVacancy()
    {
        Employer currentEmployer = (Employer)currentUser;
        Vacancy vacancy = new Vacancy();

        Console.WriteLine("elan basligi ");
        vacancy.Title = Console.ReadLine();

        Console.WriteLine("aiqlamasi ");
        vacancy.Description = Console.ReadLine();

        Console.WriteLine("Maas: ");
        vacancy.Salary = int.Parse(Console.ReadLine());

        Console.WriteLine("Son Basvurm tarixi (YYYY-MM-DD): ");
        vacancy.Deadline = DateTime.Parse(Console.ReadLine());

        currentEmployer.Vacancies.Add(vacancy);
    }




}


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


