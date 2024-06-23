using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TreciProjekatSportMonks
{
    internal class Program
    {       
        private static readonly string apiKey = "egXDQimr8GMhAs0rpVL9MSYneahtH7ua2wVx4jNoYLNARZNPuqQB53AgxNlD";
        private static readonly string baseUrl = "https://api.sportmonks.com/v3/football";
        private static readonly HttpClient httpClient = new HttpClient();
        static async Task Main(string[] args)
        {
            

            var fixtureId = "11865351";
            var url = $"{baseUrl}/fixtures/{fixtureId}?api_token={apiKey}&include=lineups";

            var observable = Observable.FromAsync(() => httpClient.GetStringAsync(url))
                                        .Select(response =>
                                        {
                                            Logger.Log("Response received from API");
                                            return JObject.Parse(response);
                                        })
                                        .SelectMany(data => data["data"]?["lineups"] ?? new JArray())
                                        .Select(player => new
                                        {
                                            PlayerId = player["player_id"]?.ToString(),
                                            Name = player["player_name"]?.ToString(),
                                            ShirtNumber = player["jersey_number"]?.ToString()
                                        })
                                        .Where(player => player.PlayerId != null);

            observable.ObserveOn(TaskPoolScheduler.Default)
                      .SelectMany(player => GetPlayerDetailsAsync(player.PlayerId)
                                             .Select(details => new
                                             {
                                                 player.Name,
                                                 player.ShirtNumber,
                                                 details.DateOfBirth,
                                                 details.Country
                                             }))
                      .Subscribe(
                          player =>
                          {
                              Console.WriteLine($"{player.Name} - {player.ShirtNumber} - {player.DateOfBirth} - {player.Country}");
                              Logger.Log($"Processed player: {player.Name}, {player.ShirtNumber}, Date of Birth: {player.DateOfBirth}, Country: {player.Country}");
                          },
                          ex =>
                          {
                              Console.WriteLine($"Error: {ex.Message}");
                              Logger.LogError(ex);
                          },
                          () =>
                          {
                              Console.WriteLine("Completed");
                              Logger.Log("Processing completed");
                          });

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static IObservable<(string DateOfBirth, string Country)> GetPlayerDetailsAsync(string playerId)
        {
            var url = $"{baseUrl}/players/{playerId}?api_token={apiKey}&include=country";
            return Observable.FromAsync(() => httpClient.GetStringAsync(url))
                             .Select(response =>
                             {
                                 var data = JObject.Parse(response)["data"];
                                 var dateOfBirth = data["date_of_birth"]?.ToString();
                                 var country = data["country"]?["name"]?.ToString();
                                 return (DateOfBirth: dateOfBirth, Country: country);
                             });
        }
    }
}
    

