using System;
using System.IO;
using System.Text.Json;

namespace Simple_Http_Server
{
    class Config
    {
        public String[] Prefixes { get; set; }

        public String HtmlDirectory { get; set; }

        public Config()
        {
        }

        public static void InitFile(String fileName)
        {
            Config config = new Config()
            {
                Prefixes = new String[]
                {
                    "http://localhost:7777"
                },
                HtmlDirectory = "www"
            };

            String json = config.ToJson();
            File.WriteAllText(fileName, json);
        }

        public static Config ParseFromJson(String json)
        {
            return JsonSerializer.Deserialize<Config>(json);
        }

        public static Config ParseFromFile(String fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("The file was not found.");

            String json = File.ReadAllText(fileName);
            return Config.ParseFromJson(json);
        }

        public string ToJson()
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, options);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}
