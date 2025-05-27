public static class ScriptureLibrary
{
    private static List<Scripture> scriptures = new List<Scripture>
    {
        new Scripture(new Reference("Juan", 3, 16), "Porque de tal manera amó Dios al mundo, que ha dado a su Hijo unigénito."),
        new Scripture(new Reference("Proverbios", 3, 5, 6), "Confía en Jehová con todo tu corazón, y no te apoyes en tu propia prudencia.")
    };

    public static Scripture GetRandomScripture()
    {
        Random rand = new Random();
        int index = rand.Next(scriptures.Count);
        return scriptures[index];
    }
}
