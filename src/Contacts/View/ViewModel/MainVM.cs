using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using View.Model;
using View.Model.Services;

namespace View.ViewModel
{
    public enum EditorMode
    {
        None,
        Add,
        Edit
    }

    public class MainVM : INotifyPropertyChanged
    {
        private readonly ContactSerializer _serializer;

        private Contact? _selectedContact;
        private Contact? _editingContact;
        private EditorMode _mode;

        public ObservableCollection<Contact> Contacts { get; }

        public Contact? SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (_selectedContact == value) return;

                _selectedContact = value;

                _mode = EditorMode.None;
                EditingContact = _selectedContact?.Clone();

                OnPropertyChanged();
                RefreshUIState();
            }
        }

        public Contact? EditingContact
        {
            get => _editingContact;
            set
            {
                if (_editingContact != null)
                {
                    _editingContact.ErrorsChanged -= Contact_ErrorsChanged;
                }

                _editingContact = value;

                if (_editingContact != null)
                {
                    _editingContact.ErrorsChanged += Contact_ErrorsChanged;
                }

                OnPropertyChanged();
                RefreshUIState();
            }
        }

        public bool IsReadOnly => _mode == EditorMode.None;

        public bool IsApplyVisible => _mode != EditorMode.None;

        public bool CanAdd => _mode == EditorMode.None;

        public bool CanEdit => _mode == EditorMode.None && SelectedContact != null;

        public bool CanRemove => _mode == EditorMode.None && SelectedContact != null;

        public bool CanApply => _mode != EditorMode.None &&
                                EditingContact != null &&
                                !EditingContact.HasErrors;

        public AddCommand AddCommand { get; }

        public EditCommand EditCommand { get; }

        public RemoveCommand RemoveCommand { get; }

        public ApplyCommand ApplyCommand { get; }

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
                EditingContact = _selectedContact.Clone();
            }

            RefreshUIState();
        }

        public void Add()
        {
            _mode = EditorMode.Add;

            _selectedContact = null;
            OnPropertyChanged(nameof(SelectedContact));

            EditingContact = new Contact();

            RefreshUIState();
        }

        public void Edit()
        {
            if (SelectedContact == null) return;

            _mode = EditorMode.Edit;
            EditingContact = SelectedContact.Clone();

            RefreshUIState();
        }

        public void Apply()
        {
            if (EditingContact == null || EditingContact.HasErrors)
            {
                return;
            }

            if (_mode == EditorMode.Add)
            {
                Contact newContact = EditingContact.Clone();

                Contacts.Add(newContact);

                _mode = EditorMode.None;
                _selectedContact = newContact;
                OnPropertyChanged(nameof(SelectedContact));

                EditingContact = newContact.Clone();
            }
            else if (_mode == EditorMode.Edit && SelectedContact != null)
            {
                SelectedContact.Name = EditingContact.Name;
                SelectedContact.PhoneNumber = EditingContact.PhoneNumber;
                SelectedContact.Email = EditingContact.Email;

                _mode = EditorMode.None;
                EditingContact = SelectedContact.Clone();
            }

            SaveContacts();
            RefreshUIState();
        }

        public void Remove()
        {
            if (SelectedContact == null) return;

            int removedIndex = Contacts.IndexOf(SelectedContact);
            Contacts.Remove(SelectedContact);

            if (Contacts.Count == 0)
            {
                _selectedContact = null;
                EditingContact = null;
            }
            else
            {
                int newIndex = removedIndex;

                if (newIndex >= Contacts.Count)
                {
                    newIndex = Contacts.Count - 1;
                }

                _selectedContact = Contacts[newIndex];
                EditingContact = _selectedContact.Clone();
            }

            _mode = EditorMode.None;

            OnPropertyChanged(nameof(SelectedContact));
            SaveContacts();
            RefreshUIState();
        }

        private void SaveContacts()
        {
            _serializer.Save(Contacts.ToList());
        }

        private void Contact_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanApply));
            ApplyCommand.RaiseCanExecuteChanged();
        }

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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}