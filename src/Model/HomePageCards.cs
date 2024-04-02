using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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


        public ServerUpdateEntry(string date = "Error", string content = "Error")
        {
            Date = date;
            Content = content;
        }
    }

    internal class SceneryCard : ObservableObject
    {
        private Bitmap _image;
        public Bitmap Image
        {
            get => _image;
            set
            {
                _image = value;
                NotifyPropertyChanged(nameof(Image));
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        private string _creator;
        public string Creator
        {
            get => _creator;
            set => _creator = value;
        }

        public SceneryCard(Uri ImageLocation, string title, string creator, string description)
        {
            _image = new(AssetLoader.Open(new Uri($@"avares://SceneryStream/Assets/Aircraft.png")));
            if (ImageLocation.ToString().Contains("avares"))
            {
                Image = new(AssetLoader.Open(ImageLocation)); 
            }
            else 
            { 
                if (ImageLocation.ToString().ToUpper().Contains("HTTP://") || ImageLocation.ToString().ToUpper().Contains("HTTPS://"))
                {
                    LoadFromWeb(ImageLocation);
                }
                else
                {
                    if (ImageLocation.ToString().Contains("srvload"))
                    {
                        string[] locationSplit = ImageLocation.OriginalString.Split(':');
                        switch (App.ServiceInstance.Platform.ToString())
                        {
                            case "Win32NT":
                                Image = new Bitmap($@"{App.Preferences.DriveLetter}:\{locationSplit[1]}");
                                break;

                            case "Unix":
                                Image = new Bitmap($@"~/mnt/{App.Preferences.DriveLetter}:\{locationSplit[1]}");
                                break;
                        }
                    }
                    else
                    {
                        Image = new Bitmap(ImageLocation.OriginalString);
                    }
                    
                }
            }
            _title = title;
            _description = description;
            _creator = creator;
        }

        public async void LoadFromWeb(Uri url)
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsByteArrayAsync();
                Image = new Bitmap(new MemoryStream(data));
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            }
        }
    }
}
