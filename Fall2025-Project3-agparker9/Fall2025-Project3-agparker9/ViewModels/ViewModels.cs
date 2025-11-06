using Fall2025_Project3_agparker9.Models;

namespace Fall2025_Project3_agparker9.ViewModels;

public class MovieDetailsViewModel
{
    public MovieModel Movie { get; set; } = null!;
    public List<ReviewWithSentiment> Reviews { get; set; } = new();
    public double AverageSentiment { get; set; }
    public string SentimentLabel { get; set; } = string.Empty;
    public List<string> Actors { get; set; } = new();
}

public class ReviewWithSentiment
{
    public string Review { get; set; } = string.Empty;
    public double SentimentScore { get; set; }
    public string SentimentLabel { get; set; } = string.Empty;
}

public class ActorDetailsViewModel
{
    public ActorModel Actor { get; set; } = null!;
    public List<TweetWithSentiment> Tweets { get; set; } = new();
    public double AverageSentiment { get; set; }
    public string SentimentLabel { get; set; } = string.Empty;
    public List<string> Movies { get; set; } = new();

}

public class TweetWithSentiment
{
    public string Tweet { get; set; } = string.Empty;
    public double SentimentScore { get; set; }
    public string SentimentLabel { get; set; } = string.Empty;
}