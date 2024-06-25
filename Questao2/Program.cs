using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        await CalculateGoals("Paris Saint-Germain", 2013);
        await CalculateGoals("Chelsea", 2014);
    }

    static async Task CalculateGoals(string team, int year)
    {
        int totalGoals = 0;
        int page = 1;

        while (true)
        {
            var response = await client.GetStringAsync($"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}&page={page}");
            var data = JObject.Parse(response);
            var matches = data["data"];

            if (matches == null || !matches.HasValues)
                break;

            foreach (var match in matches)
            {
                if (match["team1"] != null && match["team1goals"] != null)
                {
                    if (match["team1"]!.ToString() == team)
                    {
                        totalGoals += int.Parse(match["team1goals"]!.ToString());
                    }
                }
            }

            page++;
            if (data["total_pages"] is null || page > int.Parse(data["total_pages"]!.ToString()))
                break;
        }

        page = 1;
        while (true)
        {
            var response = await client.GetStringAsync($"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team2={team}&page={page}");
            var data = JObject.Parse(response);
            var matches = data["data"];

            if (matches == null || !matches.HasValues)
                break;

            foreach (var match in matches)
            {
                if (match["team2"] != null && match["team2goals"] != null)
                {
                    if (match["team2"]!.ToString() == team)
                    {
                        totalGoals += int.Parse(match["team2goals"]!.ToString());
                    }
                }
            }

            page++;
            if (data["total_pages"] is null || page > int.Parse(data["total_pages"]!.ToString()))
                break;
        }
         // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
        Console.WriteLine($"Team {team} scored {totalGoals} goals in {year}");
    }
}
