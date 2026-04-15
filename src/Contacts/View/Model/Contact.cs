using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace View.Model
{
    /// <summary>
    /// Представляет модель контакта с основными данными: именем, телефоном и электронной почтой.
    /// </summary>
    public class Contact : INotifyPropertyChanged
    {
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
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Получает или задаёт имя контакта.
        /// </summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber == value) return;
                _phoneNumber = value;
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
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Получает или задаёт адрес электронной почты контакта.
        /// </summary>
        public Contact()
        {
            _name = string.Empty;
            _phoneNumber = string.Empty;
            _email = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/> с заданными данными.
        /// </summary>
        /// <param name="name">Имя контакта.</param>
        /// <param name="phoneNumber">Номер телефона контакта.</param>
        /// <param name="email">Адрес электронной почты контакта.</param>
        public Contact(string name, string phoneNumber, string email)
        {
            _name = name;
            _phoneNumber = phoneNumber;
            _email = email;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/> с заданными данными.
        /// </summary>
        /// <param name="name">Имя контакта.</param>
        /// <param name="phoneNumber">Номер телефона контакта.</param>
        /// <param name="email">Адрес электронной почты контакта.</param>
        public Contact Clone()
        {
            return new Contact(Name, PhoneNumber, Email);
        }

        /// <summary>
        /// Создаёт копию текущего контакта.
        /// </summary>
        /// <returns>Новый объект <see cref="Contact"/> с теми же значениями свойств.</returns>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие <see cref="PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Имя изменённого свойства.</param>
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}