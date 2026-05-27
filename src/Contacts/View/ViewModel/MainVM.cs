using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using View.Model;
using View.Model.Services;

namespace View.ViewModel
{
    /// <summary>
    /// Определяет режим работы редактора контактов.
    /// </summary>
    public enum EditorMode
    {
        /// <summary>
        /// Режим просмотра без редактирования.
        /// </summary>
        None,

        /// <summary>
        /// Режим создания нового контакта.
        /// </summary>
        Add,

        /// <summary>
        /// Режим редактирования существующего контакта.
        /// </summary>
        Edit
    }

    /// <summary>
    /// Главная модель представления приложения.
    /// </summary>
    public class MainVM : INotifyPropertyChanged
    {
        private readonly ContactSerializer _serializer;

        private Contact? _selectedContact;
        private Contact? _backupContact;
        private EditorMode _mode;

        /// <summary>
        /// Получает коллекцию контактов.
        /// </summary>
        public ObservableCollection<Contact> Contacts { get; }

        /// <summary>
        /// Получает или задаёт выбранный контакт.
        /// </summary>
        public Contact? SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (_selectedContact == value) return;

                if (_mode == EditorMode.Add)
                {
                    if (_selectedContact != null)
                    {
                        Contacts.Remove(_selectedContact);
                    }
                }
                else if (_mode == EditorMode.Edit && _selectedContact != null && _backupContact != null)
                {
                    _selectedContact.Name = _backupContact.Name;
                    _selectedContact.PhoneNumber = _backupContact.PhoneNumber;
                    _selectedContact.Email = _backupContact.Email;
                }

                _mode = EditorMode.None;
                _backupContact = null;

                _selectedContact = value;
                OnPropertyChanged();

                RefreshUIState();
            }
        }

        /// <summary>
        /// Определяет, доступны ли поля только для чтения.
        /// </summary>
        public bool IsReadOnly => _mode == EditorMode.None;

        /// <summary>
        /// Определяет, должна ли отображаться кнопка Apply.
        /// </summary>
        public bool IsApplyVisible => _mode != EditorMode.None;

        /// <summary>
        /// Определяет, доступна ли команда добавления.
        /// </summary>
        public bool CanAdd => _mode == EditorMode.None;

        /// <summary>
        /// Определяет, доступна ли команда редактирования.
        /// </summary>
        public bool CanEdit => _mode == EditorMode.None && SelectedContact != null;

        /// <summary>
        /// Определяет, доступна ли команда удаления.
        /// </summary>
        public bool CanRemove => _mode == EditorMode.None && SelectedContact != null;

        /// <summary>
        /// Определяет, доступна ли команда применения изменений.
        /// </summary>
        public bool CanApply => _mode != EditorMode.None && SelectedContact != null && !SelectedContact.HasErrors;

        /// <summary>
        /// Получает команду добавления контакта.
        /// </summary>
        public AddCommand AddCommand { get; }

        /// <summary>
        /// Получает команду редактирования контакта.
        /// </summary>
        public EditCommand EditCommand { get; }

        /// <summary>
        /// Получает команду удаления контакта.
        /// </summary>
        public RemoveCommand RemoveCommand { get; }

        /// <summary>
        /// Получает команду применения изменений.
        /// </summary>
        public ApplyCommand ApplyCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MainVM"/>.
        /// </summary>
        public MainVM()
        {
            _serializer = new ContactSerializer();
            Contacts = new ObservableCollection<Contact>(_serializer.Load());

            _mode = EditorMode.None;

            AddCommand = new AddCommand(this);
            EditCommand = new EditCommand(this);
            RemoveCommand = new RemoveCommand(this);
            ApplyCommand = new ApplyCommand(this);

            if (Contacts.Count > 0)
            {
                _selectedContact = Contacts[0];
            }

            SubscribeToContacts();
            RefreshUIState();
        }

        /// <summary>
        /// Переводит приложение в режим добавления нового контакта.
        /// </summary>
        public void Add()
        {
            Contact newContact = new Contact();
            Contacts.Add(newContact);
            SelectedContact = newContact;
            _mode = EditorMode.Add;

            SubscribeToSelectedContact();
            RefreshUIState();
        }

        /// <summary>
        /// Переводит приложение в режим редактирования выбранного контакта.
        /// </summary>
        public void Edit()
        {
            if (SelectedContact == null) return;

            _backupContact = SelectedContact.Clone();
            _mode = EditorMode.Edit;

            SubscribeToSelectedContact();
            RefreshUIState();
        }

        /// <summary>
        /// Применяет изменения после добавления или редактирования контакта.
        /// </summary>
        public void Apply()
        {
            if (SelectedContact == null || SelectedContact.HasErrors)
            {
                return;
            }

            _mode = EditorMode.None;
            _backupContact = null;

            SaveContacts();
            RefreshUIState();
        }

        /// <summary>
        /// Удаляет выбранный контакт из коллекции.
        /// </summary>
        public void Remove()
        {
            if (SelectedContact == null) return;

            int removedIndex = Contacts.IndexOf(SelectedContact);
            Contact removedContact = SelectedContact;

            Contacts.Remove(removedContact);

            if (Contacts.Count == 0)
            {
                _selectedContact = null;
                OnPropertyChanged(nameof(SelectedContact));
            }
            else
            {
                int newIndex = removedIndex;
                if (newIndex >= Contacts.Count)
                {
                    newIndex = Contacts.Count - 1;
                }

                _selectedContact = Contacts[newIndex];
                OnPropertyChanged(nameof(SelectedContact));
            }

            SaveContacts();
            RefreshUIState();
        }

        /// <summary>
        /// Сохраняет коллекцию контактов в файл.
        /// </summary>
        private void SaveContacts()
        {
            _serializer.Save(Contacts.ToList());
        }

        /// <summary>
        /// Подписывает существующие контакты на отслеживание изменений ошибок.
        /// </summary>
        private void SubscribeToContacts()
        {
            foreach (var contact in Contacts)
            {
                contact.ErrorsChanged -= Contact_ErrorsChanged;
                contact.ErrorsChanged += Contact_ErrorsChanged;
            }
        }

        /// <summary>
        /// Подписывает выбранный контакт на отслеживание изменений ошибок.
        /// </summary>
        private void SubscribeToSelectedContact()
        {
            if (SelectedContact == null) return;

            SelectedContact.ErrorsChanged -= Contact_ErrorsChanged;
            SelectedContact.ErrorsChanged += Contact_ErrorsChanged;
        }

        /// <summary>
        /// Обрабатывает изменение ошибок валидации у контакта.
        /// </summary>
        private void Contact_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanApply));
            ApplyCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Обновляет состояние пользовательского интерфейса и команд.
        /// </summary>
        private void RefreshUIState()
        {
            OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(IsApplyVisible));
            OnPropertyChanged(nameof(CanAdd));
            OnPropertyChanged(nameof(CanEdit));
            OnPropertyChanged(nameof(CanRemove));
            OnPropertyChanged(nameof(CanApply));

            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
            ApplyCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Происходит при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие изменения свойства.
        /// </summary>
        /// <param name="propertyName">Имя изменённого свойства.</param>
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}