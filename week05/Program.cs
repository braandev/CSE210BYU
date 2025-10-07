using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

abstract class Activity
{
    private string _name;
    private string _description;
    private int _durationSeconds;

    protected Activity(string name, string description)
    {
        _name = name;
        _description = description;
    }

    public string Name => _name;
    public string Description => _description;
    public int DurationSeconds => _durationSeconds;

    public void Run()
    {
        Console.Clear();
        ShowStartMessage();
        PreparePause(3);
        var sw = Stopwatch.StartNew();
        try
        {
            DoActivity();
        }
        finally
        {
            sw.Stop();
            ShowEndMessage(Math.Min(_durationSeconds, (int)sw.Elapsed.TotalSeconds));
        }
    }

    protected abstract void DoActivity();

    private void ShowStartMessage()
    {
        Console.WriteLine($"--- {Name} ---");
        Console.WriteLine(Description);
        Console.WriteLine();
        _durationSeconds = AskDurationSeconds();
        Console.WriteLine("Get ready to begin...");
    }

    protected void ShowEndMessage(int effectiveSeconds)
    {
        Console.WriteLine();
        PauseWithSpinner(2, "Well done");
        Console.WriteLine();
        Console.WriteLine($"You completed: {Name} for {effectiveSeconds} seconds.");
        PauseWithSpinner(2, "Take a deep breath before continuing");
    }

    protected void PreparePause(int seconds)
    {
        PauseWithSpinner(seconds, "Starting in");
    }

    protected int AskDurationSeconds()
    {
        while (true)
        {
            Console.Write("Duration in seconds? ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out int s) && s > 0)
                return s;
            Console.WriteLine("Please enter a positive integer.");
        }
    }

    protected void PauseWithSpinner(int seconds, string label = "")
    {
        char[] spin = new[] { '|', '/', '-', '\\' };
        var end = DateTime.UtcNow.AddSeconds(seconds);
        int i = 0;
        while (DateTime.UtcNow < end)
        {
            Console.Write($"\r{label} {spin[i % spin.Length]} ");
            Thread.Sleep(120);
            i++;
        }
        Console.Write("\r" + new string(' ', Math.Max(label.Length + 4, 20)) + "\r");
    }

    protected void Countdown(int seconds, string label = "")
    {
        for (int i = seconds; i >= 1; i--)
        {
            Console.Write($"\r{label}{i}s ");
            Thread.Sleep(1000);
        }
        Console.Write("\r" + new string(' ', (label?.Length ?? 0) + 6) + "\r");
    }

    protected void ProgressBarSeconds(int seconds, string label)
    {
        int width = 20;
        for (int elapsed = 1; elapsed <= seconds; elapsed++)
        {
            double pct = elapsed / (double)seconds;
            int filled = (int)Math.Round(pct * width);
            Console.Write($"\r{label} [{new string('█', filled)}{new string('░', width - filled)}] {seconds - elapsed + 1}s");
            Thread.Sleep(1000);
        }
        Console.Write("\r" + new string(' ', label.Length + width + 15) + "\r");
    }

    protected bool TimeRemaining(DateTime endUtc) => DateTime.UtcNow < endUtc;
}

class BreathingActivity : Activity
{
    public BreathingActivity() : base(
        "Breathing Activity",
        "This activity will help you relax by slowly inhaling and exhaling. Clear your mind and focus on your breathing."
    ) { }

    protected override void DoActivity()
    {
        int inhale = 4;
        int exhale = 6;
        var end = DateTime.UtcNow.AddSeconds(DurationSeconds);
        while (TimeRemaining(end))
        {
            int remaining = (int)Math.Ceiling((end - DateTime.UtcNow).TotalSeconds);
            if (remaining <= 0) break;
            int thisIn = Math.Min(inhale, remaining);
            Console.WriteLine("Inhale...");
            ProgressBarSeconds(thisIn, "Inhale ");
            remaining -= thisIn;
            if (remaining <= 0) break;
            int thisEx = Math.Min(exhale, remaining);
            Console.WriteLine("Exhale...");
            ProgressBarSeconds(thisEx, "Exhale ");
            Console.WriteLine();
        }
    }
}

class ReflectionActivity : Activity
{
    private readonly Queue<string> _promptsQueue;
    private readonly Queue<string> _questionsQueue;
    private static readonly Random _rand = new();

    public ReflectionActivity() : base(
        "Reflection Activity",
        "Reflect on moments when you showed strength and resilience. Recognize your power and how to apply it in other areas of your life."
    )
    {
        _promptsQueue = ShuffleToQueue(new[]
        {
            "Think of a time when you stood up for someone else.",
            "Think of a time when you did something really difficult.",
            "Think of a time when you helped someone in need.",
            "Think of a time when you did something truly selfless."
        });
        _questionsQueue = ShuffleToQueue(new[]
        {
            "Why was this experience meaningful to you?",
            "Have you ever done anything like this before?",
            "How did you start?",
            "How did you feel when you finished?",
            "What made this time different from others?",
            "What do you like most about this experience?",
            "What could you learn from this experience that applies to other situations?",
            "What did you learn about yourself through this experience?",
            "How can you keep this experience in mind in the future?"
        });
    }

    protected override void DoActivity()
    {
        var end = DateTime.UtcNow.AddSeconds(DurationSeconds);
        if (_promptsQueue.Count == 0) RefillPrompts();
        var prompt = _promptsQueue.Dequeue();
        Console.WriteLine();
        Console.WriteLine("Consider this prompt:");
        Console.WriteLine($"» {prompt}");
        Console.WriteLine();
        PauseWithSpinner(5, "Take a moment to recall");
        while (TimeRemaining(end))
        {
            if (_questionsQueue.Count == 0) RefillQuestions();
            var q = _questionsQueue.Dequeue();
            Console.WriteLine($"• {q}");
            PauseWithSpinner(6, "Reflecting");
            Console.WriteLine();
        }
    }

    private static Queue<T> ShuffleToQueue<T>(IEnumerable<T> seq)
    {
        var list = seq.ToList();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _rand.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return new Queue<T>(list);
    }

    private void RefillPrompts()
    {
        var fresh = new[]
        {
            "Think of a time when you stood up for someone else.",
            "Think of a time when you did something really difficult.",
            "Think of a time when you helped someone in need.",
            "Think of a time when you did something truly selfless."
        };
        foreach (var p in ShuffleToQueue(fresh)) _promptsQueue.Enqueue(p);
    }

    private void RefillQuestions()
    {
        var fresh = new[]
        {
            "Why was this experience meaningful to you?",
            "Have you ever done anything like this before?",
            "How did you start?",
            "How did you feel when you finished?",
            "What made this time different from others?",
            "What do you like most about this experience?",
            "What could you learn from this experience that applies to other situations?",
            "What did you learn about yourself through this experience?",
            "How can you keep this experience in mind in the future?"
        };
        foreach (var q in ShuffleToQueue(fresh)) _questionsQueue.Enqueue(q);
    }
}

class ListingActivity : Activity
{
    private readonly Queue<string> _promptsQueue;
    public int ItemsCount { get; private set; }

    public ListingActivity() : base(
        "Listing Activity",
        "List as many positive things as you can in a given area. Broaden your positive outlook."
    )
    {
        _promptsQueue = new Queue<string>(Shuffle(new[]
        {
            "Who are people you appreciate?",
            "What are your personal strengths?",
            "Who have you helped this week?",
            "When have you felt inspired this month?",
            "Who are some of your personal heroes?"
        }));
    }

    protected override void DoActivity()
    {
        if (_promptsQueue.Count == 0) Refill();
        var prompt = _promptsQueue.Dequeue();
        Console.WriteLine();
        Console.WriteLine("Prompt:");
        Console.WriteLine($"» {prompt}");
        Console.WriteLine("Get ready to start listing...");
        Countdown(5, "Starting in ");
        var end = DateTime.UtcNow.AddSeconds(DurationSeconds);
        ItemsCount = 0;
        Console.WriteLine("Start typing (press Enter after each item):");
        while (DateTime.UtcNow < end)
        {
            if (Console.KeyAvailable == false)
            {
                Thread.Sleep(50);
                continue;
            }
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            ItemsCount++;
        }
        Console.WriteLine();
        Console.WriteLine($"Total items entered: {ItemsCount}");
    }

    private static IEnumerable<T> Shuffle<T>(IEnumerable<T> seq)
    {
        var rng = new Random();
        var list = seq.ToList();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    private void Refill()
    {
        foreach (var p in Shuffle(new[]
        {
            "Who are people you appreciate?",
            "What are your personal strengths?",
            "Who have you helped this week?",
            "When have you felt inspired this month?",
            "Who are some of your personal heroes?"
        })) _promptsQueue.Enqueue(p);
    }
}

class GratitudeActivity : Activity
{
    public GratitudeActivity() : base(
        "Gratitude Activity",
        "Write down three things you feel grateful for right now. Focus on simple, concrete things."
    ) { }

    protected override void DoActivity()
    {
        Console.WriteLine();
        Console.WriteLine("Write three things you are grateful for today:");
        int n = 0;
        var end = DateTime.UtcNow.AddSeconds(DurationSeconds);
        while (n < 3 && DateTime.UtcNow < end)
        {
            Console.Write($"#{n + 1}: ");
            var line = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(line)) n++;
        }
        if (n < 3)
        {
            Console.WriteLine("Time is up, you can finish later if you wish.");
        }
        else
        {
            Console.WriteLine("Thank you for sharing your gratitude!");
        }
        PauseWithSpinner(2, "Processing");
    }
}

class SessionLog
{
    public DateTime Timestamp { get; set; }
    public string Activity { get; set; } = "";
    public int DurationSeconds { get; set; }
    public int? ItemsCount { get; set; }
}

class AppStats
{
    public int BreathingCount { get; set; }
    public int ReflectionCount { get; set; }
    public int ListingCount { get; set; }
    public int GratitudeCount { get; set; }
    public int TotalSeconds { get; set; }
    public List<SessionLog> Sessions { get; set; } = new();

    public void Add(string activity, int seconds, int? items = null)
    {
        TotalSeconds += seconds;
        switch (activity)
        {
            case "Breathing Activity": BreathingCount++; break;
            case "Reflection Activity": ReflectionCount++; break;
            case "Listing Activity": ListingCount++; break;
            case "Gratitude Activity": GratitudeCount++; break;
        }
        Sessions.Add(new SessionLog
        {
            Activity = activity,
            DurationSeconds = seconds,
            ItemsCount = items,
            Timestamp = DateTime.Now
        });
    }
}

static class Storage
{
    private const string FileName = "activity_log.json";

    public static AppStats Load()
    {
        try
        {
            if (!File.Exists(FileName)) return new AppStats();
            var json = File.ReadAllText(FileName);
            var stats = JsonSerializer.Deserialize<AppStats>(json);
            return stats ?? new AppStats();
        }
        catch
        {
            return new AppStats();
        }
    }

    public static void Save(AppStats stats)
    {
        try
        {
            var json = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
        catch { }
    }
}

class Program
{
    static void Main()
    {
        var stats = Storage.Load();
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("Mindfulness Console App");
            Console.WriteLine("-----------------------");
            Console.WriteLine("1) Breathing Activity");
            Console.WriteLine("2) Reflection Activity");
            Console.WriteLine("3) Listing Activity");
            Console.WriteLine("4) Gratitude Activity");
            Console.WriteLine("5) View Statistics");
            Console.WriteLine("0) Exit");
            Console.Write("Choose an option: ");
            var op = Console.ReadLine();

            switch (op)
            {
                case "1":
                    var b = new BreathingActivity();
                    b.Run();
                    stats.Add(b.Name, b.DurationSeconds);
                    WaitEnter();
                    break;
                case "2":
                    var r = new ReflectionActivity();
                    r.Run();
                    stats.Add(r.Name, r.DurationSeconds);
                    WaitEnter();
                    break;
                case "3":
                    var l = new ListingActivity();
                    l.Run();
                    stats.Add(l.Name, l.DurationSeconds, l.ItemsCount);
                    WaitEnter();
                    break;
                case "4":
                    var g = new GratitudeActivity();
                    g.Run();
                    stats.Add(g.Name, g.DurationSeconds);
                    WaitEnter();
                    break;
                case "5":
                    ShowStats(stats);
                    WaitEnter();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    Thread.Sleep(800);
                    break;
            }
            Storage.Save(stats);
        }
    }

    static void ShowStats(AppStats s)
    {
        Console.Clear();
        Console.WriteLine("Statistics");
        Console.WriteLine("-----------");
        Console.WriteLine($"Breathing: {s.BreathingCount} sessions");
        Console.WriteLine($"Reflection: {s.ReflectionCount} sessions");
        Console.WriteLine($"Listing: {s.ListingCount} sessions");
        Console.WriteLine($"Gratitude: {s.GratitudeCount} sessions");
        Console.WriteLine($"Total time: {s.TotalSeconds} s");
        Console.WriteLine();
        if (s.Sessions.Count > 0)
        {
            Console.WriteLine("Recent sessions:");
            foreach (var sess in s.Sessions.TakeLast(5))
            {
                var extra = sess.ItemsCount.HasValue ? $" | items: {sess.ItemsCount}" : "";
                Console.WriteLine($"- {sess.Timestamp:g} · {sess.Activity} · {sess.DurationSeconds}s{extra}");
            }
        }
        else
        {
            Console.WriteLine("No sessions recorded yet.");
        }
    }

    static void WaitEnter()
    {
        Console.WriteLine();
        Console.Write("Press Enter to return to the menu...");
        Console.ReadLine();
    }
}
