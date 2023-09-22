using System.DirectoryServices.AccountManagement;
using System.Net.WebSockets;

namespace TestTask.Models
{
    public partial class Users
    {
        private static TestTaskDBContext? _context;
        public static TestTaskDBContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new TestTaskDBContext();
                }
                return _context;
            }
        }

        public static Users? CreateUser(string login, out string message)
        {
            var properties = UserADProperties.GetProperties(login);
            if (properties == null)
            {
                message = "Пользователь не найден в AD!";
                return null;
            }
            else
            {
                var user = new Users
                {
                    Login = login,
                    NameLast = properties?.NameLast,
                    NameFirst = properties?.NameFirst,
                    NameMiddle = properties?.NameMiddle,
                    Email = properties?.Email
                };
                Context.Users.Add(user);
                Context.SaveChanges();
                message = "Пользователь добавлен в БД!";
                return user;
            }
        }

        public static bool IsAuthenticated(string login, string password, out string message)
        {
            var adContext = new PrincipalContext(ContextType.Domain);
            if (!adContext.ValidateCredentials(login, password, ContextOptions.Negotiate))
            {
                var user = GetUserByLogin(login);

                if (user == null)
                {
                    message = "Пользователь отсутствует в БД!";
                    return false;
                }
                message = "Аутентификация прошла успешно!";
                return true;
            }
            else
            {
                message = "Неверный пользователь или пароль!";
                return false;
            }
        }

        internal static string RemoveUser(string login)
        {
            Users? user = GetUserByLogin(login);
            Context.Users.Remove(user);
            Context.SaveChanges();
            return "Пользователь удален из БД!";
        }

        internal static Users? GetUserByLogin(string login)
        {
            return Context.Users.Where(x => x.Login == login).ToList()[0];
        }

        internal string ChangeField(string nameField, string? newValue)
        {
            //Написать метод, который вносит изменения в свойство по наименованию и возвращает строку статуса изменения или ошибки
            return "";
        }

        private UserADProperties? _properties;

        private UserADProperties? UserADProperties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = UserADProperties.GetProperties(Login);
                }
                return _properties;
            }
        }

        public string? ADEmail => UserADProperties?.Email;
        public string? ADStaffNumber => UserADProperties?.StaffNumber;
        public string? ADDepartment => UserADProperties?.Department;
        public string? ADPosition => UserADProperties?.Position;
    }
}
