using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contacts.Model;
using Contacts.Model.Services;
using System.Collections.ObjectModel;

namespace Contacts.ViewModel
{
    /// <summary>
    /// Определяет режим работы редактора контактов.
    /// </summary>
    public enum EditorMode
    {
        /// <summary>
        /// Режим просмотра.
        /// </summary>
        None,

        /// <summary>
        /// Режим добавления.
        /// </summary>
        Add,

        /// <summary>
        /// Режим редактирования.
        /// </summary>
        Edit
    }

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
        private EditorMode mode;

        /// <summary>
        /// Определяет, доступны ли поля только для чтения.
        /// </summary>
        public bool IsReadOnly => Mode == EditorMode.None;

        /// <summary>
        /// Определяет, должна ли отображаться кнопка Apply.
        /// </summary>
        public bool IsApplyVisible => Mode != EditorMode.None;

        /// <summary>
        /// Определяет, доступна ли команда добавления.
        /// </summary>
        public bool CanAdd => Mode == EditorMode.None;

        /// <summary>
        /// Определяет, доступна ли команда редактирования.
        /// </summary>
        public bool CanEdit => Mode == EditorMode.None && SelectedContact != null;

        /// <summary>
        /// Определяет, доступна ли команда удаления.
        /// </summary>
        public bool CanRemove => Mode == EditorMode.None && SelectedContact != null;

        /// <summary>
        /// Определяет, доступна ли команда применения изменений.
        /// </summary>
        public bool CanApply => Mode != EditorMode.None &&
                                EditingContact != null &&
                                !EditingContact.HasErrors;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MainVM"/>.
        /// </summary>
        public MainVM()
        {
            _serializer = new ContactSerializer();
            Contacts = new ObservableCollection<Contact>(_serializer.Load());

            Mode = EditorMode.None;

            if (Contacts.Count > 0)
            {
                SelectedContact = Contacts[0];
                EditingContact = SelectedContact.Clone();
            }
        }

        partial void OnSelectedContactChanged(Contact? value)
        {
            if (Mode != EditorMode.None)
            {
                Mode = EditorMode.None;
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
            Mode = EditorMode.Add;
        }

        private bool CanExecuteAdd()
        {
            return CanAdd;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteEdit))]
        private void Edit()
        {
            if (SelectedContact == null) return;

            Mode = EditorMode.Edit;
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

            if (Mode == EditorMode.Add)
            {
                Contact newContact = EditingContact.Clone();
                Contacts.Add(newContact);

                Mode = EditorMode.None;
                SelectedContact = newContact;
                EditingContact = newContact.Clone();
            }
            else if (Mode == EditorMode.Edit && SelectedContact != null)
            {
                SelectedContact.Name = EditingContact.Name;
                SelectedContact.PhoneNumber = EditingContact.PhoneNumber;
                SelectedContact.Email = EditingContact.Email;

                Mode = EditorMode.None;
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