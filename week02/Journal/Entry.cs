public class Entry
{
    public string Date { get; }
    public string Prompt { get; }
    public string Response { get; }
/*********  COSTRUCTOR  ******************/
    public Entry(string date, string prompt, string response)
    {
        Date = date;
        Prompt = prompt;
        Response = response;
    }
/*********  ENTRADA AL DIARIO  ******************/
    public void Display()
    {
        Console.WriteLine($"Date: {Date}");
        Console.WriteLine($"Prompt: {Prompt}");
        Console.WriteLine($"Response: {Response}\n");
    }

    public string FormatForFile()
    {
        return $"{Date}|{Prompt}|{Response}";
    }

    public static Entry ParseFromFile(string line)
    {
        string[] parts = line.Split('|');
        return new Entry(parts[0], parts[1], parts[2]);
    }
}
