using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AccountingOfArrivalApp.Classes
{
    public class User : INotifyPropertyChanged
    {
        private int id;
        private byte[] photo;
        private string login;
        private string password;
        private string surname;
        private string name;
        private string patronymic;
        private DateTime? birthday;
        private int? type;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
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
        public DateTime? DateOfBirthday
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
        public int? IdTypeUser
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("IdTypeUser");
            }
        }
        public string TypeUser { get; set; }
        public int? Age
        {
            get
            {
                int age = DateTime.Today.Year - ((DateTime)DateOfBirthday).Year;
                if (((DateTime)DateOfBirthday).AddYears(age) < DateTime.Today) age--;
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

        public static User CurrentUser { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public User(int idUser, byte[] photo, string login, string password, string surname, string name, string patronymic, DateTime dateOfBirthday, int typeUser)
        {
            Id = idUser;
            Photo = photo;
            Login = login;
            Password = password;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            DateOfBirthday = dateOfBirthday;
            IdTypeUser = typeUser;
            TypeUser = ClassHelper.db.TypesOfUsers.FirstOrDefault(x => x.idUserTypes == typeUser).NameType;
            CurrentUser = this;
        }
        public User(Users user)
        {
            Id = user.idUser;
            Photo = user.Photo;
            Login = user.Login;
            Password = user.Password;
            Surname = user.Surname;
            Name = user.Name;
            Patronymic = user.Patronymic;
            DateOfBirthday = user.DateOfBirthday;
            IdTypeUser = (int)user.idUserTypes;
            TypeUser = user.TypesOfUsers.NameType;
            CurrentUser = this;
        }
        public static void Update(Users u)
        {
            CurrentUser.Photo = u.Photo;
            CurrentUser.Login = u.Login;
            CurrentUser.Password = u.Password;
            CurrentUser.Surname = u.Surname;
            CurrentUser.Name = u.Name;
            CurrentUser.Patronymic = u.Patronymic;
            CurrentUser.DateOfBirthday = u.DateOfBirthday;
            CurrentUser.IdTypeUser = (int)u.idUserTypes;
            CurrentUser.TypeUser = u.TypesOfUsers.NameType;
        }
    }
}
