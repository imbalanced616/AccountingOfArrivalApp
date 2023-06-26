//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AccountingOfArrivalApp.Classes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Controls;

    public partial class Users : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            this.InvoicesOnArrival = new HashSet<InvoicesOnArrival>();
        }

        private int id;
        private byte[] photo;
        private string login;
        private string password;
        private string surname;
        private string name;
        private string patronymic;
        private DateTime? birthday;
        private int? type;

        public int idUser
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("idUser");
            }
        }
        public byte[] Photo
        {
            get { return photo; }
            set
            {
                photo = value;
                OnPropertyChanged("Photo");
            }
        }
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged("Login");
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }
        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                OnPropertyChanged("Surname");
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Patronymic
        {
            get { return patronymic; }
            set
            {
                patronymic = value;
                OnPropertyChanged("Patronymic");
            }
        }
        public string FIO { get { return $"{Surname} {Name} {Patronymic}"; } }
        public Nullable<DateTime> DateOfBirthday
        {
            get { return birthday; }
            set
            {
                birthday = value;
                OnPropertyChanged("DateOfBirthday");
                OnPropertyChanged("Age");
                OnPropertyChanged("AgeType");
            }
        }
        public Nullable<int> idUserTypes
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("idUserTypes");
            }
        }
        public int? Age
        {
            get
            {
                if (DateOfBirthday == null) return 0;
                int age = DateTime.Today.Year - DateOfBirthday.Value.Year;
                if (((DateTime)DateOfBirthday).AddYears(age) > DateTime.Today) age--;
                return age;
            }
        }
        public string AgeType
        {
            get
            {
                int age = Convert.ToInt32(Age) % 10;
                if (age == 1) return "год";
                else if (age == 2 || age == 3 || age == 4) return "года";
                else return "лет";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoicesOnArrival> InvoicesOnArrival { get; set; }
        public virtual TypesOfUsers TypesOfUsers { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public static Users New(Users u)
        {
            return new Users() { idUser = u.idUser, Photo = u.Photo, Login = u.Login, Password = u.Password, Surname = u.Surname, Name = u.Name, Patronymic = u.Patronymic, DateOfBirthday = u.DateOfBirthday, idUserTypes = u.idUserTypes };
        }
        public static void UndoСhanges(Users current, Users old)
        {
            if (current == null || old == null) return;
            current.Photo = old.Photo;
            current.Login = old.Login;
            current.Password = old.Password;
            current.Surname = old.Surname;
            current.Name = old.Name;
            current.Patronymic = old.Patronymic;
            current.DateOfBirthday = old.DateOfBirthday;
            current.idUserTypes = old.idUserTypes;
        }
        public static List<Users> ToList()
        {
            return ClassHelper.db.Users.ToList();
        }
        public static List<Users> ToListBySort(MenuItem menuItemParent, MenuItem menuItem)
        {
            switch (menuItemParent.Header.ToString())
            {
                case "По логину":
                    if (menuItem.Header.ToString() == "От А до Я") return ToList().OrderBy(x => x.Login).ToList();
                    else return ToList().OrderByDescending(x => x.Login).ToList();
                case "По ФИО":
                    if (menuItem.Header.ToString() == "От А до Я") return ToList().OrderBy(x => x.FIO).ToList();
                    else return ToList().OrderByDescending(x => x.FIO).ToList();
                default: return ToList();
            }
        }
        public static List<Users> ToListByFilter(MenuItem menuItem, int filterBy)
        {
            if (filterBy == 0) return ToList().Where(x => x.TypesOfUsers.NameType == menuItem.Header.ToString()).ToList();
            else if (filterBy == 1)
            {
                switch (menuItem.Header.ToString())
                {
                    case "От 25 и меньше": return ToList().Where(x => x.Age <= 25).ToList();
                    case "От 26 и 35": return ToList().Where(x => x.Age > 25 && x.Age <= 35).ToList();
                    case "От 36 и 45": return ToList().Where(x => x.Age > 35 && x.Age <= 45).ToList();
                    case "От 46 и 55": return ToList().Where(x => x.Age > 45 && x.Age <= 55).ToList();
                    case "От 56 и больше": return ToList().Where(x => x.Age > 55).ToList();
                    default: return ToList();
                }
            }
            else return ToList();
        }
        public static List<Users> ToListBySearch(string search)
        {
            return ToList().Where(x => x.idUser.ToString().Contains(search) || x.FIO.ToLower().Contains(search) || x.Login.ToLower().Contains(search) || x.Password.ToLower().Contains(search) || x.TypesOfUsers.NameType.ToLower().Contains(search)).ToList();
        }
    }
}