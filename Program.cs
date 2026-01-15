using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        var trips = LoadTrips("202301-citibike-tripdata_1.csv");

        // Czyszczenie danych – usuwamy rekordy bez nazw stacji
        trips = trips
            .Where(t =>
                !string.IsNullOrWhiteSpace(t.StartStation) &&
                !string.IsNullOrWhiteSpace(t.EndStation))
            .ToList();

        Console.WriteLine($"Wczytano rekordów: {trips.Count}");

        // 1. Godziny szczytu w zależności od typu użytkownika
        var peakHours =
            trips.GroupBy(t => new { t.UserType, Hour = t.StartedAt.Hour })
                 .Select(g => new
                 {
                     g.Key.UserType,
                     g.Key.Hour,
                     Count = g.Count()
                 })
                 .OrderByDescending(x => x.Count)
                 .Take(10);

        Console.WriteLine("\n=== Godziny szczytu ===");
        foreach (var p in peakHours)
            Console.WriteLine($"{p.UserType} | {p.Hour}:00 -> {p.Count}");

        // 2. Stacje z najdłuższym średnim czasem przejazdu
        var longestTrips =
            trips.GroupBy(t => t.StartStation)
                 .Where(g => g.Count() > 500)
                 .Select(g => new
                 {
                     Station = g.Key,
                     AvgDuration = g.Average(t =>
                         (t.EndedAt - t.StartedAt).TotalMinutes)
                 })
                 .OrderByDescending(x => x.AvgDuration)
                 .Take(10);

        Console.WriteLine("\n=== Najdłuższe średnie przejazdy ===");
        foreach (var s in longestTrips)
            Console.WriteLine($"{s.Station}: {s.AvgDuration:F1} min");

        // 3. Porównanie dni roboczych i weekendów
        var weekendAnalysis =
            trips.GroupBy(t =>
                t.StartedAt.DayOfWeek == DayOfWeek.Saturday ||
                t.StartedAt.DayOfWeek == DayOfWeek.Sunday)
                 .Select(g => new
                 {
                     IsWeekend = g.Key,
                     Count = g.Count()
                 });

        Console.WriteLine("\n=== Weekend vs dni robocze ===");
        foreach (var w in weekendAnalysis)
            Console.WriteLine($"{(w.IsWeekend ? "Weekend" : "Dni robocze")}: {w.Count}");

        // 4. Nierównowaga stacji (analiza logistyczna)
        var starts =
            trips.GroupBy(t => t.StartStation)
                 .ToDictionary(g => g.Key, g => g.Count());

        var ends =
            trips.GroupBy(t => t.EndStation)
                 .ToDictionary(g => g.Key, g => g.Count());

        var imbalance =
            starts.Keys
                  .Select(station => new
                  {
                      Station = station,
                      Starts = starts[station],
                      Ends = ends.ContainsKey(station) ? ends[station] : 0,
                      Balance = starts[station] -
                                (ends.ContainsKey(station) ? ends[station] : 0)
                  })
                  .OrderByDescending(x => Math.Abs(x.Balance))
                  .Take(10);

        Console.WriteLine("\n=== Nierównowaga stacji ===");
        foreach (var i in imbalance)
            Console.WriteLine($"{i.Station}: starty {i.Starts}, końce {i.Ends}, saldo {i.Balance}");

        // 5. Średni czas przejazdu w zależności od pory dnia
        var durationByTime =
            trips.GroupBy(t =>
                t.StartedAt.Hour < 6 ? "Noc" :
                t.StartedAt.Hour < 12 ? "Rano" :
                t.StartedAt.Hour < 18 ? "Dzień" : "Wieczór")
                 .Select(g => new
                 {
                     TimeOfDay = g.Key,
                     AvgDuration = g.Average(t =>
                         (t.EndedAt - t.StartedAt).TotalMinutes)
                 });

        Console.WriteLine("\n=== Średni czas przejazdu ===");
        foreach (var d in durationByTime)
            Console.WriteLine($"{d.TimeOfDay}: {d.AvgDuration:F1} min");
    }

    static List<Trip> LoadTrips(string path)
    {
        var trips = new List<Trip>();

        using var reader = new StreamReader(path);
        reader.ReadLine(); // nagłówek

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var cols = line.Split(',');

            if (cols.Length < 13)
                continue;

            if (!DateTime.TryParse(cols[2], CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var start))
                continue;

            if (!DateTime.TryParse(cols[3], CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var end))
                continue;

            trips.Add(new Trip
            {
                StartedAt = start,
                EndedAt = end,
                StartStation = cols[4],
                EndStation = cols[6],
                UserType = cols[12]
            });
        }

        return trips;
    }
}
