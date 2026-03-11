using System;

/// <summary>
/// Summary description for Class1
/// </summary>

    internal class MainVM : INotifyPropertyChanged
    {
        private readonly ContactSerializer _serializer;

        private Contact _contact;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainVM()
        {
            _serializer = new ContactSerializer();
            _contact = new Contact();

            SaveCommand = new SaveCommand(this);
            LoadCommand = new LoadCommand(this);
        }

        public Contact Contact
        {
            get => _contact;
            private set
            {
                _contact = value;
                OnPropertyChanged(nameof(Contact));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Name
        {
            get => Contact.Name;
            set
            {
                if (Contact.Name == value) return;
                Contact.Name = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => Contact.PhoneNumber;
            set
            {
                if (Contact.PhoneNumber == value) return;
                Contact.PhoneNumber = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => Contact.Email;
            set
            {
                if (Contact.Email == value) return;
                Contact.Email = value;
                OnPropertyChanged();
            }
        }

        public SaveCommand SaveCommand { get; }
        public LoadCommand LoadCommand { get; }

        // Методы для команд
        public void Save()
        {
            _serializer.Save(Contact);
        }

        public void Load()
        {
            var loaded = _serializer.Load();
            if (loaded != null)
            {
                Contact = loaded; // важно: поднимем PropertyChanged для всех полей
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

