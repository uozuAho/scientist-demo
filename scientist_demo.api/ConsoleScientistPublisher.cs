using System;
using System.Text.Json;
using System.Threading.Tasks;
using GitHub;
using JsonDiffPatchDotNet;

namespace scientist_demo.api
{
    public class ConsoleScientistPublisher : IResultPublisher
    {
        public Task Publish<T, TClean>(Result<T, TClean> result)
        {
            Log($"Publishing results for experiment '{result.ExperimentName}'");
            Log($"Result: {(result.Matched ? "MATCH" : "MISMATCH")}");
            Log($"Control duration: {result.Control.Duration}");

            foreach (var observation in result.Candidates)
            {
                var jdp = new JsonDiffPatch();
                Log($"Candidate name: {observation.Name}");
                Log($"Candidate duration: {observation.Duration}");
                Log($"Candidate diff:");
                Log(jdp.Diff(Serialize(result.Control.Value), Serialize(observation.Value)));
            }

            return Task.FromResult(0);
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
        }

        private static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}
