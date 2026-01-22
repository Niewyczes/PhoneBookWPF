using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhoneBookWPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Contact> _contacts;
        private Contact _selectedContact;
        private Contact _editingContact;
        private string _searchText;
        private ICollectionView _contactsView;
        private bool _isEditing;
        private bool _isAdding;
        private bool _showWelcome;
        private Theme _currentTheme = Theme.Default;

        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set
            {
                _contacts = value;
                OnPropertyChanged();
                // Uaktualnij ContactsView po zmianie Contacts
                ContactsView = CollectionViewSource.GetDefaultView(_contacts);
                ContactsView.Filter = ContactFilter;
            }
        }

        public ICollectionView ContactsView
        {
            get => _contactsView;
            set
            {
                _contactsView = value;
                OnPropertyChanged();
            }
        }

        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsContactSelected));

                if (_selectedContact != null)
                {
                    ShowWelcome = false;
                    if (!IsEditing && !IsAdding)
                    {
                        EditingContact = (Contact)_selectedContact.Clone();
                    }
                }
            }
        }

        public Contact EditingContact
        {
            get => _editingContact;
            set
            {
                _editingContact = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ContactsView?.Refresh();
            }
        }

        public bool ShowWelcome
        {
            get => _showWelcome;
            set
            {
                _showWelcome = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowContactDetails));
            }
        }

        public bool ShowContactDetails => !ShowWelcome && (SelectedContact != null || IsEditing || IsAdding);

        public bool IsContactSelected => SelectedContact != null;

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotEditing));
            }
        }

        public bool IsAdding
        {
            get => _isAdding;
            set
            {
                _isAdding = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotEditing));
            }
        }

        public bool IsNotEditing => !IsEditing && !IsAdding;

        public Theme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                OnPropertyChanged();
                ThemeManager.CurrentTheme = value;
                SaveSettings();
            }
        }

        public Array Themes => Enum.GetValues(typeof(Theme));

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ChangeThemeCommand { get; }

        private const string DataFile = "phonebook.json";
        private const string SettingsFile = "settings.json";

        public MainViewModel()
        {
            // Inicjalizacja kolekcji
            Contacts = new ObservableCollection<Contact>();

            // Załaduj ustawienia (w tym motyw)
            LoadSettings();

            // Załaduj kontakty z pliku
            LoadContacts();

            // Pokaż panel powitalny na początku
            ShowWelcome = true;

            // Inicjalizacja komend
            AddCommand = new RelayCommand(AddContact);
            EditCommand = new RelayCommand(EditContact, CanEditContact);
            DeleteCommand = new RelayCommand(DeleteContact, CanDeleteContact);
            SaveCommand = new RelayCommand(SaveContact);
            CancelCommand = new RelayCommand(CancelEdit);
            ChangeThemeCommand = new RelayCommand(ChangeTheme);
        }

        private bool ContactFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var contact = item as Contact;
            if (contact == null) return false;

            var searchLower = SearchText.ToLower();
            return contact.FirstName.ToLower().Contains(searchLower) ||
                   contact.LastName.ToLower().Contains(searchLower) ||
                   contact.PhoneNumber.ToLower().Contains(searchLower) ||
                   (contact.Email != null && contact.Email.ToLower().Contains(searchLower)) ||
                   (contact.Address != null && contact.Address.ToLower().Contains(searchLower));
        }

        private void AddContact(object obj)
        {
            EditingContact = new Contact();
            IsAdding = true;
            IsEditing = false;
            ShowWelcome = false;
        }

        private void EditContact(object obj)
        {
            if (SelectedContact != null)
            {
                EditingContact = (Contact)SelectedContact.Clone();
                IsEditing = true;
                IsAdding = false;
                ShowWelcome = false;
            }
        }

        private bool CanEditContact(object obj)
        {
            return SelectedContact != null && !IsEditing && !IsAdding;
        }

        private bool CanDeleteContact(object obj)
        {
            return SelectedContact != null && !IsEditing && !IsAdding;
        }

        private void DeleteContact(object obj)
        {
            if (SelectedContact != null)
            {
                var result = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć kontakt {SelectedContact.FirstName} {SelectedContact.LastName}?",
                    "Potwierdzenie usunięcia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Contacts.Remove(SelectedContact);
                    SelectedContact = null;
                    SaveContacts();

                    // Jeśli nie ma już kontaktów, pokaż panel powitalny
                    if (Contacts.Count == 0)
                    {
                        ShowWelcome = true;
                    }
                }
            }
        }

        private void SaveContact(object obj)
        {
            // Walidacja danych
            if (EditingContact == null) return;

            var validationResults = ValidateContact(EditingContact);
            if (validationResults.Any())
            {
                var errors = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                MessageBox.Show($"Błędy walidacji:\n{errors}", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsAdding)
            {
                EditingContact.CreatedDate = DateTime.Now;
                EditingContact.ModifiedDate = DateTime.Now;
                Contacts.Add(EditingContact);
                SelectedContact = EditingContact;
            }
            else if (IsEditing)
            {
                var existingContact = Contacts.FirstOrDefault(c => c.Id == EditingContact.Id);
                if (existingContact != null)
                {
                    EditingContact.ModifiedDate = DateTime.Now;
                    var index = Contacts.IndexOf(existingContact);
                    Contacts[index] = EditingContact;
                    SelectedContact = EditingContact;
                }
            }

            SaveContacts();
            CancelEdit(null);
        }

        private List<ValidationResult> ValidateContact(Contact contact)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(contact);
            Validator.TryValidateObject(contact, context, results, true);
            return results;
        }

        private void CancelEdit(object obj)
        {
            IsAdding = false;
            IsEditing = false;

            // Przywróć oryginalny kontakt do edycji
            if (SelectedContact != null)
            {
                EditingContact = (Contact)SelectedContact.Clone();
                ShowWelcome = false;
            }
            else
            {
                ShowWelcome = true;
            }
        }

        private void ChangeTheme(object obj)
        {
            if (obj is Theme theme)
            {
                CurrentTheme = theme;
            }
        }

        private void LoadContacts()
        {
            try
            {
                if (File.Exists(DataFile))
                {
                    var json = File.ReadAllText(DataFile);

                    // Dla System.Text.Json:
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var contacts = JsonSerializer.Deserialize<ObservableCollection<Contact>>(json, options);

                    // Dla Newtonsoft.Json:
                    // var contacts = JsonConvert.DeserializeObject<ObservableCollection<Contact>>(json);

                    if (contacts != null)
                    {
                        Contacts = contacts;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania danych: {ex.Message}\nPlik: {Path.GetFullPath(DataFile)}",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveContacts()
        {
            try
            {
                // Dla System.Text.Json:
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(Contacts, options);

                // Dla Newtonsoft.Json:
                // var json = JsonConvert.SerializeObject(Contacts, Formatting.Indented);

                File.WriteAllText(DataFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania danych: {ex.Message}\nŚcieżka: {Path.GetFullPath(DataFile)}",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);

                    // Dla System.Text.Json:
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var settings = JsonSerializer.Deserialize<AppSettings>(json, options);

                    // Dla Newtonsoft.Json:
                    // var settings = JsonConvert.DeserializeObject<AppSettings>(json);

                    if (settings != null)
                    {
                        CurrentTheme = settings.Theme;
                    }
                }
            }
            catch
            {
                // Ignoruj błędy - użyj domyślnego motywu
                CurrentTheme = Theme.Default;
            }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new AppSettings { Theme = CurrentTheme };

                // Dla System.Text.Json:
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(settings, options);

                // Dla Newtonsoft.Json:
                // var json = JsonConvert.SerializeObject(settings, Formatting.Indented);

                File.WriteAllText(SettingsFile, json);
            }
            catch
            {
                // Ignoruj błędy zapisu ustawień
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class ValidationExtensions
    {
        public static List<ValidationResult> Validate(this object obj)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(obj);
            Validator.TryValidateObject(obj, context, results, true);
            return results;
        }
    }

    // Klasa dla ustawień aplikacji
    public class AppSettings
    {
        public Theme Theme { get; set; } = Theme.Default;
    }
}