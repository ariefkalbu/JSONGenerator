using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GenerateJSON
{
    public class ConfigurationFile
    {
        private String inputFileName;
        private String valueDefault;
        private String outputFileName;

        public string InputFileName { get => inputFileName; set => inputFileName = value; }
        public string ValueDefault { get => valueDefault; set => valueDefault = value; }
        public string OutputFileName { get => outputFileName; set => outputFileName = value; }
    }

    public class GetConfigurationFile
    {
        public bool StartProcess()
        {
            FileInfo[] files = this.GetBaseConfiguration();
            if (files == null) return false;
            List<ConfigurationFile> configurationFiles = this.configurationFiles(files);
            if (configurationFiles == null || configurationFiles.Count <= 0) return false;
            ProcessDeleteFile(configurationFiles);
            if (!ProcessConfiguration(configurationFiles)) return false;
            return true;
        }

        private FileInfo[] GetBaseConfiguration()
        {
            DirectoryInfo directory = new DirectoryInfo(@"./");
            FileInfo[] files = directory.GetFiles("base.config.xml");
            if (files == null || files.Count() <= 0)
            {
                Console.WriteLine("Files base.config.xml Not Found");
                return null;
            }
            return files;
        }

        private List<ConfigurationFile> configurationFiles(FileInfo[] files)
        {
            List<ConfigurationFile> result = new List<ConfigurationFile>();
            foreach (FileInfo file in files)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(file.FullName);

                XmlNodeList nodes = xmlDocument.GetElementsByTagName("data");
                foreach (XmlNode node in nodes)
                {
                    ConfigurationFile configurationFile = new ConfigurationFile();
                    configurationFile.InputFileName = node.Attributes["input_file_name"].Value;
                    configurationFile.ValueDefault = node.Attributes["value"].Value;
                    configurationFile.OutputFileName = node.Attributes["output_file_name"].Value;
                    result.Add(configurationFile);
                }
            }
            return result;
        }

        private bool ProcessConfiguration(List<ConfigurationFile> configurationFiles)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.

                foreach (ConfigurationFile configurationFile in configurationFiles)
                {
                    Dictionary<String, object> valueWord = new Dictionary<string, object>();
                    Dictionary<String, object> duplicateWord = new Dictionary<string, object>();
                    Console.WriteLine("##---------------- START ----------------##");
                    using (StreamReader sr = new StreamReader("./" + configurationFile.InputFileName))
                    {
                        string line;

                        // Read and display lines from the file until 
                        // the end of the file is reached. 
                        int i = 0;
                        while ((line = sr.ReadLine()) != null)
                        {
                            valueWord.Add(line, configurationFile.ValueDefault);
                            Console.WriteLine(i + ". " + line);
                            i++;
                        }
                        string json = JsonConvert.SerializeObject(valueWord, Newtonsoft.Json.Formatting.Indented);
                        string output_file_name = configurationFile.OutputFileName;

                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter("./" + output_file_name + ".json", true))
                        {
                            writer.WriteLine(json);
                        }

                        Console.WriteLine("##---------------- SUCCESS ----------------##");
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();

            return true;
        }

        private void ProcessDeleteFile(List<ConfigurationFile> configurationFiles)
        {
            foreach (ConfigurationFile configurationFile in configurationFiles)
            {
                string output_file_name = configurationFile.OutputFileName;
                if (File.Exists(@"./" + output_file_name + ".json"))
                {
                    File.Delete(@"./" + output_file_name + ".json");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GetConfigurationFile getConfigurationFile = new GetConfigurationFile();
            getConfigurationFile.StartProcess();
        }
    }
}

