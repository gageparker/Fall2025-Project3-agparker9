using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_agparker9.Data;
using Fall2025_Project3_agparker9.Models;
using Fall2025_Project3_agparker9.ViewModels;
using Fall2025_Project3_agparker9.Services;

using VaderSharp2;

namespace Fall2025_Project3_agparker9.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly AiService _aiService;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IConfiguration configuration, AiService aiService)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
        _aiService = aiService;
    }

    //index cshtml view
    public async Task<IActionResult> Index()
    {
        // Get all movies to display in the carousel
        var movies = await _context.Movie.ToListAsync();
        return View(movies);
    }
    
    
    //movie create cshtml view
    public IActionResult Create()
    {
        return View();
    }

    //movie create controller for poster
    [HttpPost]
    public async Task<IActionResult> Create(MovieModel movie, IFormFile posterFile)
    {
        if (posterFile != null && posterFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await posterFile.CopyToAsync(ms);
            movie.Poster = ms.ToArray();
        }

        _context.Movie.Add(movie);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Movies));
    }
    
    //edit cshtml controller
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _context.Movie.FindAsync(id);
        if (movie == null)
            return NotFound();

        return View(movie);
    }
    
    //edit poster controller
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MovieModel movie, IFormFile? posterFile)
    {
        var existingMovie = await _context.Movie.FindAsync(id);
        if (existingMovie == null)
            return NotFound();

        existingMovie.Title = movie.Title;
        existingMovie.Genre = movie.Genre;
        existingMovie.Year = movie.Year;
        
        if (posterFile != null && posterFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await posterFile.CopyToAsync(ms);
            existingMovie.Poster = ms.ToArray();
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Movies));
    }
    
    //delete controller
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _context.Movie.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        _context.Movie.Remove(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Movies));
    }
    
    //movies cshtml controller
    public async Task<IActionResult> Movies()
    {
        var movies = await _context.Movie.ToListAsync();
        return View(movies); 
    }
    
    //actors controller
    public async Task<IActionResult> Actors()
    {
        var actors = await _context.Actor.ToListAsync();
        return View(actors);
    }

    //actor create cshtml view
    public IActionResult CreateActor()
    {
        return View();
    }
    
    //actor create controller for photo
    [HttpPost]
    public async Task<IActionResult> CreateActor(ActorModel actor, IFormFile photoFile)
    {
        if (photoFile != null && photoFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await photoFile.CopyToAsync(ms);
            actor.Photo = ms.ToArray();
        }

        _context.Actor.Add(actor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Actors));
    }
    
    //edit cshtml controller
    public async Task<IActionResult> ActorEdit(int id)
    {
        var actor = await _context.Actor.FindAsync(id);
        if (actor == null)
            return NotFound();

        return View(actor);
    }
    
    //edit poster controller
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActorEdit(int id, ActorModel actor, IFormFile? photoFile)
    {
        var existingActor = await _context.Actor.FindAsync(id);
        if (existingActor == null)
            return NotFound();

        existingActor.Name = actor.Name;
        existingActor.Gender = actor.Gender;
        existingActor.Age = actor.Age;
        
        if (photoFile != null && photoFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await photoFile.CopyToAsync(ms);
            existingActor.Photo = ms.ToArray();
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Actors));
    }
    
    //movie details controller
    public async Task<IActionResult> MovieDetails(int? id)
    {
        if (id == null) return NotFound();

        var movie = await _context.Movie.FirstOrDefaultAsync(m => m.MovieId == id);
        if (movie == null) return NotFound();

        // Generate AI reviews
        var reviews = await _aiService.GenerateMovieReviewsAsync(movie.Title, movie.Genre, movie.Year);

        // Perform sentiment analysis
        var analyzer = new SentimentIntensityAnalyzer();
        var reviewsWithSentiment = reviews.Select(review =>
        {
            var sentiment = analyzer.PolarityScores(review);
            return new ReviewWithSentiment
            {
                Review = review,
                SentimentScore = sentiment.Compound,
                SentimentLabel = GetSentimentLabel(sentiment.Compound)
            };
        }).ToList();

        // Calculate average sentiment
        var avgSentiment = reviewsWithSentiment.Average(r => r.SentimentScore);
        
        var actors = await _aiService.GetActorsInMovieAsync(movie.Title);
        
        var viewModel = new MovieDetailsViewModel
        {
            Movie = movie,
            Reviews = reviewsWithSentiment,
            AverageSentiment = avgSentiment,
            SentimentLabel = GetSentimentLabel(avgSentiment),
            Actors = actors
        };

        return View(viewModel);
    }

    //actors details controller
    public async Task<IActionResult> ActorDetails(int? id)
    {
        if (id == null) return NotFound();

        var actor = await _context.Actor.FirstOrDefaultAsync(a => a.ActorId == id);
        if (actor == null) return NotFound();

        // Generate AI tweets
        var tweets = await _aiService.GenerateActorTweetsAsync(actor.Name, actor.Age, actor.Gender);

        // Perform sentiment analysis
        var analyzer = new SentimentIntensityAnalyzer();
        var tweetsWithSentiment = tweets.Select(tweet =>
        {
            var sentiment = analyzer.PolarityScores(tweet);
            return new TweetWithSentiment
            {
                Tweet = tweet,
                SentimentScore = sentiment.Compound,
                SentimentLabel = GetSentimentLabel(sentiment.Compound)
            };
        }).ToList();

        // Calculate average sentiment
        var avgSentiment = tweetsWithSentiment.Average(t => t.SentimentScore);
        
        var movies = await _aiService.GetMoviesForActorAsync(actor.Name);
        
        var viewModel = new ActorDetailsViewModel
        {
            Actor = actor,
            Tweets = tweetsWithSentiment,
            AverageSentiment = avgSentiment,
            SentimentLabel = GetSentimentLabel(avgSentiment),
            Movies = movies
        };

        return View(viewModel);
    }

    
    private string GetSentimentLabel(double score)
    {
        if (score >= 0.05) return "Positive";
        if (score <= -0.05) return "Negative";
        return "Neutral";
    }
    
   
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}