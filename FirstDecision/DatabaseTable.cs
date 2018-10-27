using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FirstDecision {
    internal class DatabaseTable<DATA_TYPE> where DATA_TYPE : new() {
        private List<DATA_TYPE> data;
        private string filePath;

        public DatabaseTable(string filePath, bool addDummies) {
            this.filePath = filePath;

            if (File.Exists(filePath)) {
                LoadData();
            }
            else {
                data = new List<DATA_TYPE>();
                if (addDummies) {
                    data.Add(new DATA_TYPE());
                    data.Add(new DATA_TYPE());
                    data.Add(new DATA_TYPE());
                }
                SaveData();
            }
        }

        ~DatabaseTable() {
            SaveData();
        }

        private void SaveData() {
            
            FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(stream);
            string dataString = JsonConvert.SerializeObject(data);
            writer.WriteLine(dataString);
            writer.Flush();
            writer.Close();
            stream.Close();
        }

        private void LoadData() {
            FileStream stream = new FileStream(filePath, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            data = JsonConvert.DeserializeObject<List<DATA_TYPE>>(reader.ReadToEnd());
            reader.Close();
            stream.Close();
        }

        public virtual void InsertElement(DATA_TYPE element) {
            data.Add(element);
            SaveData();
        }

        public virtual void DeleteElement(DATA_TYPE element) {
            data.Remove(element);
            SaveData();
        }

        public virtual List<DATA_TYPE> SelectAll() {
            return data;
        }

    }
}
