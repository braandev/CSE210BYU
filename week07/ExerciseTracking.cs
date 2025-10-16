using System;
using System.Collections.Generic;

public abstract class Activity
{
    private string _date;
    private int _length;

    public Activity(string date, int length)
    {
        _date = date;
        _length = length;
    }

    public string Date { get { return _date; } }
    public int Length { get { return _length; } }

    public abstract double GetDistance();
    public abstract double GetSpeed();
    public abstract double GetPace();

    public virtual string GetSummary()
    {
        return $"{Date} {GetType().Name} ({Length} min): " +
               $"Distance {GetDistance():0.0} km, " +
               $"Speed {GetSpeed():0.0} km/h, " +
               $"Pace {GetPace():0.0} min per km";
    }
}

public class Running : Activity
{
    private double _distance;

    public Running(string date, int length, double distance)
        : base(date, length)
    {
        _distance = distance;
    }

    public override double GetDistance() => _distance;

    public override double GetSpeed() => (GetDistance() / Length) * 60;

    public override double GetPace() => Length / GetDistance();
}

public class Cycling : Activity
{
    private double _speed;

    public Cycling(string date, int length, double speed)
        : base(date, length)
    {
        _speed = speed;
    }

    public override double GetDistance() => (_speed * Length) / 60;

    public override double GetSpeed() => _speed;

    public override double GetPace() => 60 / _speed;
}

public class Swimming : Activity
{
    private int _laps;

    public Swimming(string date, int length, int laps)
        : base(date, length)
    {
        _laps = laps;
    }

    public override double GetDistance() => _laps * 50 / 1000.0;

    public override double GetSpeed() => (GetDistance() / Length) * 60;

    public override double GetPace() => Length / GetDistance();
}

class Program
{
    static void Main()
    {
        List<Activity> activities = new List<Activity>()
        {
            new Running("Nov 3, 2022", 30, 4.8),
            new Cycling("Nov 3, 2022", 30, 9.7),
            new Swimming("Nov 3, 2022", 30, 20)
        };

        foreach (Activity activity in activities)
        {
            Console.WriteLine(activity.GetSummary());
        }
    }
}
