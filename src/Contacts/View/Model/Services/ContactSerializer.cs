using System;
using System.IO;
using Newtonsoft.Json;

namespace View.Model.Services
{
    public class ContactSerializer
    {
        public string FilePath { get; set; }

        public ContactSerializer()
        {
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dir = Path.Combine(docs, "Contacts");
            FilePath = Path.Combine(dir, "contacts.json");
        }

        public void Save(Contact contact)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(contact, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public Contact? Load()
        {
            if (!File.Exists(FilePath))
                return null;

            var json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Contact>(json);
        }
    }
}