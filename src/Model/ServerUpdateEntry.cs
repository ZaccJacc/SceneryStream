using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneryStream.src.Model
{
    internal class ServerUpdateEntry
    {
        private string _date;
        public string Date
        {
            get => _date;
            set => _date = value;
        }

        private string _content;
        public string Content
        {
            get => _content;
            set => _content = value;
        }


        public ServerUpdateEntry(string date, string content)
        {
            Date = date;
            Content = content;
        }
    }
}
