using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class Emails
    {
        public IList<string> FirstStepWorkers() => ReadFile("FirstStep");

        private IList<string> ReadFile(string filename)
        {
            var path = $"./{filename}.txt";
            if (!File.Exists(path))
            {
                var stream = File.CreateText(path);
                stream.Dispose();
                return new List<string>();
            }

            return File.ReadAllLines(path).ToList();
        }
    }
}
