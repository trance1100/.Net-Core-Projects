using System;                     // Tells that any types from the System namespace are in scope
using System.Collections.Generic; // Used to call a Iterator method.
using System.IO;                  // Used to access the File class
using System.Threading.Tasks;     // Used to make Task

namespace TeleprompterConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RunTeleprompter().Wait();
        }

        private static async Task RunTeleprompter()
        {
            var config = new TelePrompterConfig();
            var displayTask = ShowTeleprompter(config);

            var speedTask = GetInput(config);
            await Task.WhenAny(displayTask, speedTask);
        }

        private static async Task ShowTeleprompter(TelePrompterConfig config)
        {
            var words = ReadFrom(@"C:\Users\Larri\Programming\newApp\Quotes.txt");  // Read the Quotes.
            foreach (var word in words) // Loop through the words from the text.
            {
                Console.Write(word);    // Print out word
                if (!string.IsNullOrWhiteSpace(word))   // If not reached EOF
                {
                    await Task.Delay(config.DelayInMilliseconds);   // Delay 
                }
            }
            config.SetDone();   // Done flag is True.
        }

        private static async Task GetInput(TelePrompterConfig config)
        {
            Action work = () =>
            {
                do {    // Read keysrtokes. 
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == '>') // Increase speed
                        config.UpdateDelay(-10);
                    else if (key.KeyChar == '<')    // Decrease speed
                        config.UpdateDelay(10);
                    else if (key.KeyChar == 'X' || key.KeyChar == 'x')  // Finish reading file
                        config.SetDone();
                } while (!config.Done); // Wait until finished
            };
            await Task.Run(work);
        }
        static IEnumerable<string> ReadFrom(string file) // Receives path of file; IEnumerable returns a sequence, each sequence is evaluated lazily. Each item is generated as it is requested.
        {
            string line;
            using (var reader = File.OpenText(file))    // We open the file; we use the var to reference the StreamReader object that is returned.
            {
                while ((line = reader.ReadLine()) != null) //Read lines until reach the end.
                {
                    var words = line.Split(' ');    // Split all the items into words
                    var lineLength = 0;
                    foreach (var word in words)
                    {
                        yield return word + " ";    // returns word followed by a space
                        lineLength += word.Length + 1;  // gets the length of the word and adds it to the variable.
                        if (lineLength > 70)    // Once words is greater than 70
                        {
                            yield return Environment.NewLine;   // Make a new line
                            lineLength = 0;                     // Set the line length back to 0
                        }
                    }
                    yield return Environment.NewLine;  // Make a new line
                }
            }
        }
    }
}