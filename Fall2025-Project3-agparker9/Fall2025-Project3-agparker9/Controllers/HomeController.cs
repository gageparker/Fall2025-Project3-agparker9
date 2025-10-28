using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_agparker9.Data;
using Fall2025_Project3_agparker9.Models;

namespace Fall2025_Project3_agparker9.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    //index cshtml view
    public IActionResult Index()
    {
        return View();
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

    public IActionResult CreateActor()
    {
        return View();
    }
    
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
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}