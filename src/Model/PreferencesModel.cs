using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneryStream.src.Model
{
    internal class PreferencesModel
    {
        public PreferencesModel() { }

        public async Task loadPreferences(String fileName)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            } catch(FileNotFoundException)
            {
                Console.WriteLine("Could not locate file!");
            }
            
        }
    }
}
