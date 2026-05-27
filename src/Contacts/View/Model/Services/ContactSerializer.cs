using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using View.Model;

namespace View.Model.Services
{
    /// <summary>
    /// Выполняет сохранение и загрузку коллекции контактов в JSON-файл.
    /// </summary>
    public class ContactSerializer
    {
        /// <summary>
        /// Получает путь к файлу хранения контактов.
        /// </summary>
        /// 
        public string FilePath { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ContactSerializer"/>
        /// и задаёт путь к файлу хранения контактов.
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
        /// <param name="contacts">Коллекция контактов для сохранения.</param>
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
        /// <returns>Список контактов, загруженных из файла.</returns>
        public List<Contact> Load()
        {
            if (!File.Exists(FilePath))
            {
                return new List<Contact>();
            }

            string json = File.ReadAllText(FilePath);
            List<Contact>? contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

            return contacts ?? new List<Contact>();
        }
    }
}