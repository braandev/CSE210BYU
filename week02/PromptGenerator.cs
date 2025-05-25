using System;
using System.Collections.Generic;

public class PromptGenerator
{
    private List<string> prompts = new List<string>
    {   
        "What new thing did I learn today?",
        "What challenge did I overcome today?",
        "What am I most grateful for today?",
        "How did I show kindness to someone today?",
        "What is one thing I want to remember about today?",
        "Who was the most interesting person I interacted with today?",
        "What was the best part of my day?",
        "How did I see the hand of the Lord in my life today?",
        "What was the strongest emotion I felt today?",
        "If I could do one thing over today, what would it be?"
    };

    private Random random = new Random();

    public string GetRandomPrompt()
    {
        int index = random.Next(prompts.Count);
        return prompts[index];
    }
}
