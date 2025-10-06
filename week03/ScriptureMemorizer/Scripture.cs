using System;
using System.Collections.Generic;

public class Scripture
{
    private Reference _reference;
    private List<Word> _words;

    public Scripture(Reference reference, string text)
    {
        _reference = reference;
        _words = new List<Word>();

        foreach (string word in text.Split(' '))
        {
            _words.Add(new Word(word));
        }
    }

    public void Display()
    {
        Console.WriteLine(_reference);
        foreach (Word word in _words)
        {
            Console.Write(word.GetDisplayText() + " ");
        }
    }

    public void HideRandomWords(int count)
    {
        Random rand = new Random();
        List<Word> visibleWords = _words.FindAll(w => w.IsVisible());

        for (int i = 0; i < count && visibleWords.Count > 0; i++)
        {
            int index = rand.Next(visibleWords.Count);
            visibleWords[index].Hide();
            visibleWords.RemoveAt(index);
        }
    }

    public bool AllWordsHidden()
    {
        return _words.TrueForAll(w => !w.IsVisible());
    }
}
