public static class ScriptureLibrary
{
    private static List<Scripture> scriptures = new List<Scripture>
    {
        new Scripture(new Reference("John", 3, 16), "For God so loved the world, that he gave his only begotten Son."),
        new Scripture(new Reference("Proverbs", 3, 5, 6), "Trust in the Lord with all your heart and lean not on your own understanding.")
    };

    public static Scripture GetRandomScripture()
    {
        Random rand = new Random();
        int index = rand.Next(scriptures.Count);
        return scriptures[index];
    }
}
