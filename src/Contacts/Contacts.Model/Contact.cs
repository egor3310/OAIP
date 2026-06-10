using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Contacts.Model
{
    /// <summary>
    /// Представляет контакт с поддержкой уведомления об изменении свойств и валидации данных.
    /// </summary>
    public class Contact : ObservableObject, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        private string _name = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _email = string.Empty;

        /// <summary>
        /// Получает или задаёт имя контакта.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    ValidateName();
                }
            }
        }

        /// <summary>
        /// Получает или задаёт номер телефона контакта.
        /// </summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (SetProperty(ref _phoneNumber, value))
                {
                    ValidatePhoneNumber();
                }
            }
        }

        /// <summary>
        /// Получает или задаёт адрес электронной почты контакта.
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    ValidateEmail();
                }
            }
        }

        /// <summary>
        /// Возвращает значение, указывающее, есть ли ошибки валидации.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        /// Происходит при изменении ошибок валидации.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/>.
        /// </summary>
        public Contact()
        {
            ValidateAll();
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/> с заданными данными.
        /// </summary>
        public Contact(string name, string phoneNumber, string email)
        {
            _name = name;
            _phoneNumber = phoneNumber;
            _email = email;

            ValidateAll();
        }

        /// <summary>
        /// Создаёт копию текущего контакта.
        /// </summary>
        public Contact Clone()
        {
            return new Contact(Name, PhoneNumber, Email);
        }

        /// <summary>
        /// Возвращает ошибки валидации для указанного свойства.
        /// </summary>
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return Enumerable.Empty<string>();
            }

            return _errors.TryGetValue(propertyName, out var errors)
                ? errors
                : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Выполняет валидацию всех свойств контакта.
        /// </summary>
        public void ValidateAll()
        {
            ValidateName();
            ValidatePhoneNumber();
            ValidateEmail();
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(HasErrors));
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(HasErrors));
            }
        }

        private void ValidateName()
        {
            const string propertyName = nameof(Name);
            ClearErrors(propertyName);

            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(propertyName, "Name is required.");
                return;
            }

            if (Name.Length > 100)
            {
                AddError(propertyName, "Name cannot be longer than 100 characters.");
            }
        }

        private void ValidatePhoneNumber()
        {
            const string propertyName = nameof(PhoneNumber);
            const int maxDigits = 11;

            ClearErrors(propertyName);

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                AddError(propertyName, "Phone Number is required.");
                return;
            }

            if (!Regex.IsMatch(PhoneNumber, @"^[0-9+\-()\s]+$"))
            {
                AddError(propertyName, "Phone Number can contain only digits and symbols '+-()'.");
            }

            if (PhoneNumber.Count(char.IsDigit) > maxDigits)
            {
                AddError(propertyName, $"Phone Number can contain no more than {maxDigits} digits.");
            }

            if (PhoneNumber.Length > 100)
            {
                AddError(propertyName, "Phone Number cannot be longer than 100 characters.");
            }
        }

        private void ValidateEmail()
        {
            const string propertyName = nameof(Email);
            ClearErrors(propertyName);

            if (string.IsNullOrWhiteSpace(Email))
            {
                AddError(propertyName, "Email is required.");
                return;
            }

            if (Email.Length > 100)
            {
                AddError(propertyName, "Email cannot be longer than 100 characters.");
            }

            if (!Email.Contains('@'))
            {
                AddError(propertyName, "Email must contain '@'.");
            }

            if (!Regex.IsMatch(Email, @"^[A-Za-z0-9@._\-]+$"))
            {
                AddError(propertyName, "Email can contain only latin letters, digits and symbols @ . _ -");
            }
        }
    }
}