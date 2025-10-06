using System;
using System.Collections.Generic;
using System.IO;

public class Journal
{
    private List<Entry> entries = new List<Entry>();

    public void AddEntry(Entry entry)
    {
        entries.Add(entry);
    }

    public void Display()
    {
        if (entries.Count == 0)
        {
            Console.WriteLine("No journal entries yet.");
        }
        else
        {
            foreach (Entry entry in entries)
            {
                entry.Display();
            }
        }
    }

    public void SaveToFile(string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            foreach (Entry entry in entries)
            {
                writer.WriteLine(entry.FormatForFile());
            }
        }
        Console.WriteLine("Journal saved.");
    }

    public void LoadFromFile(string filename)
    {
        if (File.Exists(filename))
        {
            entries.Clear();
            string[] lines = File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                entries.Add(Entry.ParseFromFile(line));
            }
            Console.WriteLine("Journal loaded.");
        }
        else
        {
            Console.WriteLine("File not found.");
        }
    }
}
