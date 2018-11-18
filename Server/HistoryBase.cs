using System.Collections.Generic;
using System.IO;
using Models;
using Newtonsoft.Json;

namespace Server
{
    public class HistoryBase
    {
        public HistoryBase()
        {
            var file = File.ReadAllText(Filename);
            Records = JsonConvert.DeserializeObject<List<OrderData>>(file);
        }

        public IList<OrderData> Records { get; set; }

        public void Save(OrderData newRecord)
        {
            Records.Add(newRecord);
            File.WriteAllText(Filename, JsonConvert.SerializeObject(Records));
        }

        private string Filename => "./History.json";
    }
}
