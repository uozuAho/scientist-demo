using System;
using System.Threading.Tasks;
using GitHub;

namespace scientist_demo.api
{
    public class ConsoleScientistPublisher : IResultPublisher
    {
        public Task Publish<T, TClean>(Result<T, TClean> result)
        {
            Log($"Publishing results for experiment '{result.ExperimentName}'");
            Log($"Result: {(result.Matched ? "MATCH" : "MISMATCH")}");
            Log($"Control value: {result.Control.Value}");
            Log($"Control duration: {result.Control.Duration}");
            foreach (var observation in result.Candidates)
            {
                Log($"Candidate name: {observation.Name}");
                Log($"Candidate value: {observation.Value}");
                Log($"Candidate duration: {observation.Duration}");
            }

            return Task.FromResult(0);
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
