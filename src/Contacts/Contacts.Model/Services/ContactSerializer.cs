using Newtonsoft.Json;

namespace Contacts.Model.Services
{
    /// <summary>
    /// Выполняет сохранение и загрузку коллекции контактов в JSON-файл.
    /// </summary>
    public class ContactSerializer
    {
        /// <summary>
        /// Получает путь к файлу хранения контактов.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ContactSerializer"/>.
        /// </summary>
        public ContactSerializer()
        {
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string dir = Path.Combine(docs, "Contacts");
            FilePath = Path.Combine(dir, "contacts.json");
        }

        /// <summary>
        /// Сохраняет коллекцию контактов в JSON-файл.
        /// </summary>
        public void Save(List<Contact> contacts)
        {
            string? dir = Path.GetDirectoryName(FilePath);

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        /// <summary>
        /// Загружает коллекцию контактов из JSON-файла.
        /// </summary>
        public List<Contact> Load()
        {
            if (!File.Exists(FilePath))
            {
                return new List<Contact>();
            }

            string json = File.ReadAllText(FilePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Contact>();
            }

            List<Contact>? contacts = JsonConvert.DeserializeObject<List<Contact>>(json);
            return contacts ?? new List<Contact>();
        }
    }
}