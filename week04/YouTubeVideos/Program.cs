using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        List<Video> videos = new List<Video>();

        Video video1 = new Video("How to Train Your Dog", "Brandon Cazorla", 420);
        video1.AddComment(new Comment("Anna", "Excellent explanation, very clear!"));
        video1.AddComment(new Comment("Luis", "This helped me with my puppy."));
        video1.AddComment(new Comment("Camila", "Great video, thanks for sharing!"));
        videos.Add(video1);

        Video video2 = new Video("Top 5 Programming Tips", "TechZone", 300);
        video2.AddComment(new Comment("Mark", "Tip number 3 saved my project."));
        video2.AddComment(new Comment("Lucy", "Very useful for beginners."));
        video2.AddComment(new Comment("James", "Thanks for the great content!"));
        videos.Add(video2);

        Video video3 = new Video("Travel Guide: Denmark", "Global Explorer", 550);
        video3.AddComment(new Comment("Carla", "Beautiful places!"));
        video3.AddComment(new Comment("Thomas", "Now I really want to visit Copenhagen."));
        video3.AddComment(new Comment("Michael", "Nice overview, I loved it."));
        videos.Add(video3);

        Video video4 = new Video("C# for Beginners", "CodeMaster", 480);
        video4.AddComment(new Comment("Nicolas", "I learned more here than in class!"));
        video4.AddComment(new Comment("Laura", "Excellent teacher."));
        video4.AddComment(new Comment("Peter", "Thanks, very well explained."));
        videos.Add(video4);

        foreach (Video video in videos)
        {
            video.DisplayVideoInfo();
            Console.WriteLine();
        }
    }
}
