using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EternalQuest
{
    abstract class Goal
    {
        private string _name;
        private string _description;
        private int _points;

        protected Goal(string name, string description, int points)
        {
            _name = name;
            _description = description;
            _points = points;
        }

        public string Name => _name;
        public string Description => _description;
        public int Points => _points;

        public virtual bool IsComplete => false;
        public abstract string GetStatus();
        public abstract (int pointsAwarded, string message) RecordEvent();
        public abstract string Serialize();

        public static Goal Deserialize(string line)
        {
            var parts = line.Split('|');
            if (parts.Length < 4)
                throw new InvalidDataException("Invalid line: " + line);

            string type = parts[0];
            string name = parts[1];
            string desc = parts[2];
            int points = int.Parse(parts[3], CultureInfo.InvariantCulture);

            switch (type)
            {
                case "SimpleGoal":
                {
                    bool complete = parts.Length >= 5 && bool.Parse(parts[4]);
                    return new SimpleGoal(name, desc, points, complete);
                }
                case "EternalGoal":
                {
                    return new EternalGoal(name, desc, points);
                }
                case "ChecklistGoal":
                {
                    int target = int.Parse(parts[4], CultureInfo.InvariantCulture);
                    int current = int.Parse(parts[5], CultureInfo.InvariantCulture);
                    int bonus = int.Parse(parts[6], CultureInfo.InvariantCulture);
                    bool complete = bool.Parse(parts[7]);
                    return new ChecklistGoal(name, desc, points, target, bonus, current, complete);
                }
                default:
                    throw new InvalidDataException("Unknown type: " + type);
            }
        }
    }

    class SimpleGoal : Goal
    {
        private bool _isComplete;

        public SimpleGoal(string name, string description, int points, bool isComplete = false)
            : base(name, description, points)
        {
            _isComplete = isComplete;
        }

        public override bool IsComplete => _isComplete;

        public override string GetStatus()
        {
            string box = _isComplete ? "[X]" : "[ ]";
            return $"{box} {Name} — {Description} ({Points} pts)";
        }

        public override (int pointsAwarded, string message) RecordEvent()
        {
            if (_isComplete)
                return (0, $"\"{Name}\" was already completed. No points awarded.");

            _isComplete = true;
            return (Points, $"You completed \"{Name}\" and earned {Points} points!");
        }

        public override string Serialize()
        {
            return $"SimpleGoal|{Name}|{Description}|{Points}|{_isComplete}";
        }
    }

    class EternalGoal : Goal
    {
        public EternalGoal(string name, string description, int points)
            : base(name, description, points) { }

        public override string GetStatus()
        {
            return $"[∞] {Name} — {Description} (+{Points} each time)";
        }

        public override (int pointsAwarded, string message) RecordEvent()
        {
            return (Points, $"You recorded \"{Name}\" and earned {Points} points.");
        }

        public override string Serialize()
        {
            return $"EternalGoal|{Name}|{Description}|{Points}";
        }
    }

    class ChecklistGoal : Goal
    {
        private int _targetCount;
        private int _currentCount;
        private int _bonusPoints;
        private bool _isComplete;

        public ChecklistGoal(string name, string description, int points, int targetCount, int bonusPoints, int currentCount = 0, bool isComplete = false)
            : base(name, description, points)
        {
            _targetCount = targetCount;
            _bonusPoints = bonusPoints;
            _currentCount = currentCount;
            _isComplete = isComplete;
        }

        public override bool IsComplete => _isComplete;

        public override string GetStatus()
        {
            string box = _isComplete ? "[X]" : "[ ]";
            return $"{box} {Name} — {Description} ({Points} each; Bonus { _bonusPoints } after {_targetCount}) — Progress {_currentCount}/{_targetCount}";
        }

        public override (int pointsAwarded, string message) RecordEvent()
        {
            if (_isComplete)
                return (0, $"\"{Name}\" is already complete ({_currentCount}/{_targetCount}).");

            _currentCount++;
            int gained = Points;
            string msg = $"You recorded \"{Name}\" ({_currentCount}/{_targetCount}) and earned {Points} points.";

            if (_currentCount >= _targetCount && !_isComplete)
            {
                _isComplete = true;
                gained += _bonusPoints;
                msg += $" 🎉 Goal completed! Bonus: {_bonusPoints} points.";
            }

            return (gained, msg);
        }

        public override string Serialize()
        {
            return $"ChecklistGoal|{Name}|{Description}|{Points}|{_targetCount}|{_currentCount}|{_bonusPoints}|{_isComplete}";
        }
    }

    class GoalManager
    {
        private readonly List<Goal> _goals = new List<Goal>();
        private int _score;
        private readonly int[] _badgeMilestones = new[] { 1000, 5000, 10000, 25000, 50000 };
        private readonly HashSet<int> _achievedMilestones = new HashSet<int>();

        public int Score => _score;
        public int Level => 1 + (_score / 1000);
        public void AddGoal(Goal goal) => _goals.Add(goal);
        public IReadOnlyList<Goal> Goals => _goals;

        public string AwardBadgesIfAny(int previousScore, int newScore)
        {
            var unlocked = new List<int>();
            foreach (var m in _badgeMilestones)
            {
                bool crossed = previousScore < m && newScore >= m;
                if (crossed && !_achievedMilestones.Contains(m))
                {
                    _achievedMilestones.Add(m);
                    unlocked.Add(m);
                }
            }

            if (unlocked.Count == 0) return string.Empty;
            return "🏅 Badge unlocked! Milestones: " + string.Join(", ", unlocked.Select(x => x.ToString("N0", CultureInfo.InvariantCulture))) + " points.";
        }

        public string RecordEventAtIndex(int index)
        {
            if (index < 0 || index >= _goals.Count)
                return "Invalid index.";

            var goal = _goals[index];
            int before = _score;
            var (points, message) = goal.RecordEvent();
            _score += points;

            string badgeMsg = AwardBadgesIfAny(before, _score);
            var levelBefore = 1 + (before / 1000);
            var levelAfter = Level;
            string levelMsg = levelAfter > levelBefore ? $"⬆️ You reached Level {levelAfter}!" : string.Empty;

            var extras = new[] { message, string.IsNullOrEmpty(levelMsg) ? null : levelMsg, string.IsNullOrEmpty(badgeMsg) ? null : badgeMsg }
                         .Where(s => !string.IsNullOrWhiteSpace(s));

            return string.Join(" ", extras);
        }

        public void ListGoals()
        {
            if (_goals.Count == 0)
            {
                Console.WriteLine("No goals created yet.");
                return;
            }
            for (int i = 0; i < _goals.Count; i++)
                Console.WriteLine($"{i + 1}. {_goals[i].GetStatus()}");
        }

        public void ShowScore()
        {
            Console.WriteLine($"Total score: {Score} points");
            Console.WriteLine($"Current level: {Level}");
        }

        public void SaveToFile(string path)
        {
            using var sw = new StreamWriter(path);
            sw.WriteLine($"SCORE|{_score}");
            if (_achievedMilestones.Count > 0)
                sw.WriteLine("MILESTONES|" + string.Join(",", _achievedMilestones.OrderBy(x => x)));

            foreach (var g in _goals)
                sw.WriteLine(g.Serialize());
        }

        public void LoadFromFile(string path)
        {
            _goals.Clear();
            _achievedMilestones.Clear();
            _score = 0;

            var lines = File.ReadAllLines(path);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("SCORE|", StringComparison.OrdinalIgnoreCase))
                {
                    _score = int.Parse(line.Split('|')[1], CultureInfo.InvariantCulture);
                }
                else if (line.StartsWith("MILESTONES|", StringComparison.OrdinalIgnoreCase))
                {
                    var arr = line.Split('|');
                    if (arr.Length > 1 && !string.IsNullOrWhiteSpace(arr[1]))
                    {
                        foreach (var token in arr[1].Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int val))
                                _achievedMilestones.Add(val);
                        }
                    }
                }
                else
                {
                    _goals.Add(Goal.Deserialize(line));
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            var manager = new GoalManager();

            while (true)
            {
                Console.WriteLine("\n===== ETERNAL QUEST — MAIN MENU =====");
                Console.WriteLine("1) Create new goal");
                Console.WriteLine("2) List goals");
                Console.WriteLine("3) Record event");
                Console.WriteLine("4) Show score and level");
                Console.WriteLine("5) Save goals to file");
                Console.WriteLine("6) Load goals from file");
                Console.WriteLine("7) Exit");
                Console.Write("Choose an option: ");

                string opt = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                try
                {
                    switch (opt)
                    {
                        case "1":
                            CreateGoalFlow(manager);
                            break;
                        case "2":
                            manager.ListGoals();
                            break;
                        case "3":
                            RecordEventFlow(manager);
                            break;
                        case "4":
                            manager.ShowScore();
                            break;
                        case "5":
                            SaveFlow(manager);
                            break;
                        case "6":
                            LoadFlow(manager);
                            break;
                        case "7":
                            Console.WriteLine("Goodbye! Keep progressing in your Eternal Quest.");
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        static void CreateGoalFlow(GoalManager manager)
        {
            Console.WriteLine("Goal type:");
            Console.WriteLine("  1) Simple (one-time)");
            Console.WriteLine("  2) Eternal (repeated, never complete)");
            Console.WriteLine("  3) Checklist (multiple times with bonus)");
            Console.Write("Choose (1-3): ");
            string type = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Name: ");
            string name = NonEmptyReadLine();
            Console.Write("Description: ");
            string desc = NonEmptyReadLine();
            int points = ReadInt("Points per completion: ", min: 0);

            switch (type)
            {
                case "1":
                    manager.AddGoal(new SimpleGoal(name, desc, points));
                    Console.WriteLine("Simple goal created.");
                    break;
                case "2":
                    manager.AddGoal(new EternalGoal(name, desc, points));
                    Console.WriteLine("Eternal goal created.");
                    break;
                case "3":
                    int target = ReadInt("Required times: ", min: 1);
                    int bonus = ReadInt("Bonus points: ", min: 0);
                    manager.AddGoal(new ChecklistGoal(name, desc, points, target, bonus));
                    Console.WriteLine("Checklist goal created.");
                    break;
                default:
                    Console.WriteLine("Invalid type.");
                    break;
            }
        }

        static void RecordEventFlow(GoalManager manager)
        {
            if (manager.Goals.Count == 0)
            {
                Console.WriteLine("No goals available.");
                return;
            }

            Console.WriteLine("Select a goal:");
            manager.ListGoals();

            int index = ReadInt("Goal number: ", min: 1, max: manager.Goals.Count) - 1;
            string result = manager.RecordEventAtIndex(index);
            Console.WriteLine(result);
        }

        static void SaveFlow(GoalManager manager)
        {
            Console.Write("File name to save (e.g., goals.txt): ");
            string path = NonEmptyReadLine();
            manager.SaveToFile(path);
            Console.WriteLine($"Saved to \"{path}\".");
        }

        static void LoadFlow(GoalManager manager)
        {
            Console.Write("File name to load (e.g., goals.txt): ");
            string path = NonEmptyReadLine();
            if (!File.Exists(path))
            {
                Console.WriteLine("File not found.");
                return;
            }
            manager.LoadFromFile(path);
            Console.WriteLine($"Loaded from \"{path}\".");
        }

        static int ReadInt(string prompt, int? min = null, int? max = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine();
                if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int val))
                {
                    if (min.HasValue && val < min.Value)
                    {
                        Console.WriteLine($"Must be ≥ {min.Value}.");
                        continue;
                    }
                    if (max.HasValue && val > max.Value)
                    {
                        Console.WriteLine($"Must be ≤ {max.Value}.");
                        continue;
                    }
                    return val;
                }
                Console.WriteLine("Invalid value.");
            }
        }

        static string NonEmptyReadLine()
        {
            while (true)
            {
                string s = Console.ReadLine()?.Trim() ?? "";
                if (!string.IsNullOrWhiteSpace(s)) return s;
                Console.Write("Cannot be empty. Enter again: ");
            }
        }
    }
}
