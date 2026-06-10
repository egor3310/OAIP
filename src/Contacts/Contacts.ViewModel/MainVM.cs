using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contacts.Model;
using Contacts.Model.Services;
using System.Collections.ObjectModel;


namespace Contacts.ViewModel
{


    /// <summary>
    /// Главная модель представления приложения.
    /// </summary>
    public partial class MainVM : ObservableObject
    {
        private readonly ContactSerializer _serializer;

        /// <summary>
        /// Получает коллекцию контактов.
        /// </summary>
        public ObservableCollection<Contact> Contacts { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanEdit))]
        [NotifyPropertyChangedFor(nameof(CanRemove))]
        [NotifyCanExecuteChangedFor(nameof(EditCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
        private Contact? selectedContact;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanApply))]
        [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
        private Contact? editingContact;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsReadOnly))]
        [NotifyPropertyChangedFor(nameof(IsApplyVisible))]
        [NotifyPropertyChangedFor(nameof(CanAdd))]
        [NotifyPropertyChangedFor(nameof(CanEdit))]
        [NotifyPropertyChangedFor(nameof(CanRemove))]
        [NotifyPropertyChangedFor(nameof(CanApply))]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
        [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
        private Status mode;

        /// <summary>
        /// Определяет, доступны ли поля только для чтения.
        /// </summary>
        public bool IsReadOnly => Mode == Status.EditorMode.None;

        /// <summary>
        /// Определяет, должна ли отображаться кнопка Apply.
        /// </summary>
        public bool IsApplyVisible => Mode != Status.EditorMode.None;

        /// <summary>
        /// Определяет, доступна ли команда добавления.
        /// </summary>
        public bool CanAdd => Mode == Status.EditorMode.None;

        /// <summary>
        /// Определяет, доступна ли команда редактирования.
        /// </summary>
        public bool CanEdit => Mode == Status.EditorMode.None && SelectedContact != null;

        /// <summary>
        /// Определяет, доступна ли команда удаления.
        /// </summary>
        public bool CanRemove => Mode == Status.EditorMode.None && SelectedContact != null;

        /// <summary>
        /// Определяет, доступна ли команда применения изменений.
        /// </summary>
        public bool CanApply => Mode != Status.EditorMode.None &&
                                EditingContact != null &&
                                !EditingContact.HasErrors;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MainVM"/>.
        /// </summary>
        public MainVM()
        {
            _serializer = new ContactSerializer();
            Contacts = new ObservableCollection<Contact>(_serializer.Load());

            Mode = Status.EditorMode.None;

            if (Contacts.Count > 0)
            {
                SelectedContact = Contacts[0];
                EditingContact = SelectedContact.Clone();
            }
        }

        partial void OnSelectedContactChanged(Contact? value)
        {
            if (Mode != Status.EditorMode.None)
            {
                Mode = Status.EditorMode.None;
            }

            EditingContact = value?.Clone();
        }

        partial void OnEditingContactChanged(Contact? oldValue, Contact? newValue)
        {
            if (oldValue != null)
            {
                oldValue.ErrorsChanged -= EditingContact_ErrorsChanged;
            }

            if (newValue != null)
            {
                newValue.ErrorsChanged += EditingContact_ErrorsChanged;
            }
        }

        private void EditingContact_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanApply));
            ApplyCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanExecuteAdd))]
        private void Add()
        {
            SelectedContact = null;
            EditingContact = new Contact();
            Mode = Status.EditorMode.Add;
        }

        private bool CanExecuteAdd()
        {
            return CanAdd;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteEdit))]
        private void Edit()
        {
            if (SelectedContact == null) return;

            Mode = Status.EditorMode.Edit;
            EditingContact = SelectedContact.Clone();
        }

        private bool CanExecuteEdit()
        {
            return CanEdit;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteRemove))]
        private void Remove()
        {
            if (SelectedContact == null) return;

            int removedIndex = Contacts.IndexOf(SelectedContact);
            Contacts.Remove(SelectedContact);

            if (Contacts.Count == 0)
            {
                SelectedContact = null;
                EditingContact = null;
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

            SaveContacts();
        }

        private bool CanExecuteRemove()
        {
            return CanRemove;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteApply))]
        private void Apply()
        {
            if (EditingContact == null || EditingContact.HasErrors)
            {
                return;
            }

            if (Mode == Status.EditorMode.Add)
            {
                Contact newContact = EditingContact.Clone();
                Contacts.Add(newContact);

                Mode = Status.EditorMode.None;
                SelectedContact = newContact;
                EditingContact = newContact.Clone();
            }
            else if (Mode == Status.EditorMode.Edit && SelectedContact != null)
            {
                SelectedContact.Name = EditingContact.Name;
                SelectedContact.PhoneNumber = EditingContact.PhoneNumber;
                SelectedContact.Email = EditingContact.Email;

                Mode = Status.EditorMode.None;
                EditingContact = SelectedContact.Clone();
            }

            SaveContacts();
        }

        private bool CanExecuteApply()
        {
            return CanApply;
        }

        private void SaveContacts()
        {
            _serializer.Save(Contacts.ToList());
        }
    }
}