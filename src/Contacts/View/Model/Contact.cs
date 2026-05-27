using System.Collections;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace View.Model
{
    /// <summary>
    /// Представляет контакт с поддержкой уведомления об изменении свойств и валидации данных.
    /// </summary>
    public class Contact : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        private string _name;
        private string _phoneNumber;
        private string _email;

        /// <summary>
        /// Получает или задаёт имя контакта.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                ValidateName();
                OnPropertyChanged();
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
                if (_phoneNumber == value) return;
                _phoneNumber = value;
                ValidatePhoneNumber();
                OnPropertyChanged();
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
                if (_email == value) return;
                _email = value;
                ValidateEmail();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Возвращает значение, указывающее, есть ли ошибки валидации.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        /// Происходит при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Происходит при изменении ошибок валидации.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/>.
        /// </summary>
        public Contact()
        {
            _name = string.Empty;
            _phoneNumber = string.Empty;
            _email = string.Empty;

            ValidateAll();
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/> с заданными данными.
        /// </summary>
        /// <param name="name">Имя контакта.</param>
        /// <param name="phoneNumber">Номер телефона.</param>
        /// <param name="email">Электронная почта.</param>
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
        /// <returns>Копия контакта.</returns>
        public Contact Clone()
        {
            return new Contact(Name, PhoneNumber, Email);
        }

        /// <summary>
        /// Возвращает ошибки валидации для указанного свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        /// <returns>Коллекция ошибок.</returns>
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
        /// Выполняет валидацию всех полей контакта.
        /// </summary>
        public void ValidateAll()
        {
            ValidateName();
            ValidatePhoneNumber();
            ValidateEmail();
        }

        /// <summary>
        /// Проверяет, заполнены ли все поля корректно.
        /// </summary>
        /// <returns><see langword="true"/>, если ошибок нет; иначе <see langword="false"/>.</returns>
        public bool IsValid()
        {
            ValidateAll();
            return !HasErrors;
        }

        /// <summary>
        /// Вызывает событие изменения свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Добавляет ошибку для свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="error">Текст ошибки.</param>
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

        /// <summary>
        /// Удаляет все ошибки указанного свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        private void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(HasErrors));
            }
        }

        /// <summary>
        /// Выполняет валидацию имени контакта.
        /// </summary>
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

        /// <summary>
        /// Выполняет валидацию номера телефона.
        /// </summary>
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

            int digitCount = PhoneNumber.Count(char.IsDigit);
            if (digitCount > maxDigits)
            {
                AddError(propertyName, $"Phone Number can contain no more than {maxDigits} digits.");
            }

            if (PhoneNumber.Length > 100)
            {
                AddError(propertyName, "Phone Number cannot be longer than 100 characters.");
            }
        }

        /// <summary>
        /// Выполняет валидацию электронной почты.
        /// </summary>
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