using System;
using System.IO;
using System.Net;
using CommandLine;
using CommandLine.Text;

namespace Ship
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            
            if (!Parser.Default.ParseArguments(args, options)) return;

            try
            {
                PerformTask(options);
            }
            catch (WebException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private static void PerformTask(Options options)
        {
            using (var client = new WebClient())
            {
                if (options.Snapshot)
                {
                    // TODO KO
//                    Stream stream = client.OpenRead(options.Address);
//                    String request = reader.ReadToEnd();
                }
                else
                {
                    client.UploadFile(options.Address, options.InputFile);
                }
            }
        }
    }

    class Options
    {
        [Option('s', "snapshot", Required = false, 
          HelpText = "Trigger Sitecore to serialise it's content tree.")]
        public bool Snapshot { get; set; }

        [Option('p', "package", Required = false,
          HelpText = "Sitecore update package file to be installed.")]
        public string InputFile { get; set; }

        [Option('u', "url", Required = true,
          HelpText = "URL to send the request to.")]
        public string Address { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
