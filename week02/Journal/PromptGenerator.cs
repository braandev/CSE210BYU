using System;
using System.Collections.Generic;

public class PromptGenerator
{
    private List<string> prompts = new List<string>
    {
        "¿Quién fue la persona más interesante con la que interactué hoy?",
        "¿Cuál fue la mejor parte de mi día?",
        "¿Cómo vi la mano del Señor en mi vida hoy?",
        "¿Cuál fue la emoción más fuerte que sentí hoy?",
        "Si tuviera que hacer una cosa hoy, ¿qué sería?"
    };

    private Random random = new Random();

    public string GetRandomPrompt()
    {
        int index = random.Next(prompts.Count);
        return prompts[index];
    }
}
