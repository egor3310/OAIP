using System;
using System.IO;
using Newtonsoft.Json;

namespace View.Model.Services
{
    /// <summary>
    /// Класс ContactSerializer отвечает за создание json файла. Для того чтобы пользователь сохранял файл и вытаскивал из него данные 
    /// </summary>
    public class ContactSerializer
    {
        /// <summary>
        /// Свойство FilePath для хранения файла 
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// констрктор, который создает папку и файл
        /// </summary>
        public ContactSerializer()
        {
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dir = Path.Combine(docs, "Contacts");
            FilePath = Path.Combine(dir, "contacts.json");
        }

        /// <summary>
        /// метод создает файл на компьютере 
        /// </summary>
        /// <param name="contact"></param>
        public void Save(Contact contact)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(contact, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }
        /// <summary>
        /// Метод Load отвечает за конвертирование json и переноса данных на интерфейс 
        /// </summary>
        /// <returns></returns>
        public Contact? Load()
        {
            if (!File.Exists(FilePath))
                return null;

            var json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Contact>(json);
        }
    }
}