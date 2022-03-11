using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Azure_Speach
{

    public class Program
    {
        static async Task Main()
        {
            await SynthesisToMp3FileAsync();
        }
                
        public static async Task SynthesisToMp3FileAsync()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            // The default language is "sr-RS".
            var config = SpeechConfig.FromSubscription("ed4ee12e000b49aabde7309b64f3167a", "westeurope");
            
            Console.WriteLine("Please choose language.");
            Console.WriteLine("1 - Serbian (sr-RS) Sophie");
            Console.WriteLine("2 - Croatian (hr-HR) Srecko");
            Console.Write("> ");
            string language = Console.ReadLine();
            if (language == "2")
            {
                config.SpeechSynthesisLanguage = "hr-HR";
                config.SpeechSynthesisVoiceName = "hr-HR-SreckoNeural";
            }
            else 
            {
                config.SpeechSynthesisLanguage = "sr-RS";
                config.SpeechSynthesisVoiceName = "sr-RS-SophieNeural";
            }
            
            // Sets the synthesis output format.
            // The full list of supported format can be found here:
            // https://docs.microsoft.com/azure/cognitive-services/speech-service/rest-text-to-speech#audio-outputs
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3);

            // Creates a speech synthesizer using file as audio output.
            // Replace with your own audio file name.
            var fileName = "outputaudio.mp3";
            using (var fileOutput = AudioConfig.FromWavFileOutput(fileName))
            using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
            {
                while (true)
                {
                    // Receives a text from console input and synthesize it to mp3 file.
                    // For Cyrillic Encoding needs to be in Unicode.
                    Console.OutputEncoding = Console.InputEncoding = Encoding.Unicode;

                    Console.WriteLine("");
                    Console.WriteLine("Enter some text that you want to synthesize, or enter empty text to exit.");
                    Console.WriteLine("For Serbian please enter text in Cyrillic.");
                    Console.Write("> ");
                    string text = Console.ReadLine();
                    if (string.IsNullOrEmpty(text))
                    {
                        break;
                    }

                    using (var result = await synthesizer.SpeakTextAsync(text))
                    {
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            Console.WriteLine($"Speech synthesized for text [{text}], and the audio was saved to [{fileName}]");
                        }
                        else if (result.Reason == ResultReason.Canceled)
                        {
                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                            Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                            if (cancellation.Reason == CancellationReason.Error)
                            {
                                Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                                Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                                Console.WriteLine($"CANCELED: Did you update the subscription info?");
                            }
                        }
                    }
                }
            }
        }
    }

}
