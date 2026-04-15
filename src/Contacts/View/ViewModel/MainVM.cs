using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Linq;
using View.Model.Services;
using View.Model;
using View.ViewModel;
using View.Model;
using View.Model.Services;

namespace View.ViewModel
{

    /// <summary>
    /// Определяет режим работы редактора контактов.
    /// </summary>
    public enum EditorMode
    {
        None,
        Add,
        Edit
    }

    /// <summary>
    /// Главная модель представления приложения для работы с коллекцией контактов.
    /// </summary>
    public class MainVM : INotifyPropertyChanged
    {
        private readonly ContactSerializer _serializer;

        private Contact? _selectedContact;
        private EditorMode _mode;

        private string _editorName;
        private string _editorPhoneNumber;
        private string _editorEmail;

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

                _selectedContact = value;
                OnPropertyChanged();

                // Если в данный момент не редактируем и не создаем,
                // просто отображаем выбранный контакт справа
                if (_mode == EditorMode.None)
                {
                    LoadSelectedToEditor();
                }
                else
                {
                    // Если во время Add/Edit выбрали другой контакт,
                    // то несохраненные изменения отменяются
                    CancelEditingAndShowSelected();
                }

                RefreshUIState();
            }
        }

        /// <summary>
        /// Получает или задаёт имя, отображаемое в редакторе контакта.
        /// </summary>
        public string EditorName
        {
            get => _editorName;
            set
            {
                if (_editorName == value) return;
                _editorName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Получает или задаёт номер телефона, отображаемый в редакторе контакта.
        /// </summary>
        public string EditorPhoneNumber
        {
            get => _editorPhoneNumber;
            set
            {
                if (_editorPhoneNumber == value) return;
                _editorPhoneNumber = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Получает или задаёт адрес электронной почты, отображаемый в редакторе контакта.
        /// </summary>
        public string EditorEmail
        {
            get => _editorEmail;
            set
            {
                if (_editorEmail == value) return;
                _editorEmail = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Определяет, доступны ли поля редактора только для чтения.
        /// </summary>
        public bool IsReadOnly => _mode == EditorMode.None;

        /// <summary>
        /// Определяет, доступны ли поля редактора только для чтения.
        /// </summary>
        public bool IsApplyVisible => _mode != EditorMode.None;

        /// <summary>
        /// Определяет, должна ли быть видима кнопка применения изменений.
        /// </summary>
        public bool CanAdd => _mode == EditorMode.None;

        /// <summary>
        /// Определяет, доступна ли команда добавления контакта.
        /// </summary>
        public bool CanEdit => _mode == EditorMode.None && SelectedContact != null;

        /// <summary>
        /// Определяет, доступна ли команда удаления контакта.
        /// </summary>
        public bool CanRemove => _mode == EditorMode.None && SelectedContact != null;

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

            _editorName = string.Empty;
            _editorPhoneNumber = string.Empty;
            _editorEmail = string.Empty;

            _mode = EditorMode.None;

            AddCommand = new AddCommand(this);
            EditCommand = new EditCommand(this);
            RemoveCommand = new RemoveCommand(this);
            ApplyCommand = new ApplyCommand(this);

            if (Contacts.Count > 0)
            {
                SelectedContact = Contacts[0];
            }
            else
            {
                ClearEditorFields();
            }

            RefreshUIState();
        }

        /// <summary>
        /// Переводит приложение в режим добавления нового контакта.
        /// </summary>
        public void Add()
        {
            _mode = EditorMode.Add;
            _selectedContact = null;
            OnPropertyChanged(nameof(SelectedContact));

            ClearEditorFields();
            RefreshUIState();
        }

        /// <summary>
        /// Переводит приложение в режим редактирования выбранного контакта.
        /// </summary>
        public void Edit()
        {
            if (SelectedContact == null) return;

            _mode = EditorMode.Edit;
            LoadSelectedToEditor();
            RefreshUIState();
        }

        /// <summary>
        /// Применяет изменения после добавления или редактирования контакта.
        /// </summary>
        public void Apply()
        {
            if (_mode == EditorMode.Add)
            {
                Contact newContact = new Contact(EditorName, EditorPhoneNumber, EditorEmail);
                Contacts.Add(newContact);
                SelectedContact = newContact;
            }
            else if (_mode == EditorMode.Edit && SelectedContact != null)
            {
                SelectedContact.Name = EditorName;
                SelectedContact.PhoneNumber = EditorPhoneNumber;
                SelectedContact.Email = EditorEmail;
            }

            _mode = EditorMode.None;
            SaveContacts();

            if (SelectedContact != null)
            {
                LoadSelectedToEditor();
            }
            else
            {
                ClearEditorFields();
            }

            RefreshUIState();
        }

        /// <summary>
        /// Удаляет выбранный контакт из коллекции.
        /// </summary>
        public void Remove()
        {
            if (SelectedContact == null) return;

            int removedIndex = Contacts.IndexOf(SelectedContact);
            Contacts.Remove(SelectedContact);

            if (Contacts.Count == 0)
            {
                SelectedContact = null;
                ClearEditorFields();
            }
            else
            {
                int newIndex = removedIndex;
                if (newIndex >= Contacts.Count)
                {
                    newIndex = Contacts.Count - 1;
                }

                SelectedContact = Contacts[newIndex];
            }

            _mode = EditorMode.None;
            SaveContacts();
            RefreshUIState();
        }

        /// <summary>
        /// Отменяет редактирование и отображает данные выбранного контакта.
        /// </summary>
        private void CancelEditingAndShowSelected()
        {
            _mode = EditorMode.None;

            if (SelectedContact != null)
            {
                LoadSelectedToEditor();
            }
            else
            {
                ClearEditorFields();
            }
        }

        /// <summary>
        /// Загружает данные выбранного контакта в поля редактора.
        /// </summary>
        private void LoadSelectedToEditor()
        {
            if (SelectedContact == null)
            {
                ClearEditorFields();
                return;
            }

            EditorName = SelectedContact.Name;
            EditorPhoneNumber = SelectedContact.PhoneNumber;
            EditorEmail = SelectedContact.Email;
        }

        /// <summary>
        /// Очищает поля редактора контакта.
        /// </summary>
        private void ClearEditorFields()
        {
            EditorName = string.Empty;
            EditorPhoneNumber = string.Empty;
            EditorEmail = string.Empty;
        }

        /// <summary>
        /// Очищает поля редактора контакта.
        /// </summary>
        private void SaveContacts()
        {
            _serializer.Save(Contacts.ToList());
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

            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
            ApplyCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Происходит при изменении значения свойства
        /// </summary>
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