using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_CRUD_OPERATION.Models;
using MVC_CRUD_OPERATION.ViewModel;
using NToastNotify;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_CRUD_OPERATION.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private new List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        
        private long _maxAllowedPosterSize = 1048576;

        public MoviesController(ApplicationDbContext context,IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.OrderByDescending(o=>o.Rate).ToListAsync();
            return View(movies);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new MovieFromViewModel
            {
                Genres = await _context.Genres.OrderBy(o=>o.Name).ToListAsync()
            };   
            return View("MovieForm", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MovieFromViewModel model)
        {
           if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(o => o.Name).ToListAsync();
                return View("MovieForm", model);
            }

            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(o => o.Name).ToListAsync();
                ModelState.AddModelError("Poster", "please select movie poster");
                return View("MovieForm", model);
            }

            var poster = files.FirstOrDefault();
          
            if (!_allowedExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(o => o.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .png or jpg are allowed");
                return View("MovieForm", model);
            }
            if(poster.Length> _maxAllowedPosterSize)
            {
                model.Genres = await _context.Genres.OrderBy(o => o.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB");
                return View("MovieForm", model);
            }

            using var dataStream=new MemoryStream();

            await poster.CopyToAsync(dataStream);

            var movies = new Movie
            {
                Title=model.Title,
                GenreId=model.GenreId,
                Year=model.Year,
                Rate=model.Rate,
                Storyline=model.Storyline,
                Poster=dataStream.ToArray()
            };
            _context.Movies.Add(movies);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Movie Created Successfylly");

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            var viewModel = new MovieFromViewModel
            {
                Id=movie.Id,
                Title=movie.Title,
                GenreId=movie.GenreId,
                Rate=movie.Rate,
                Year=movie.Year,
                Storyline = movie.Storyline,
                Poster=movie.Poster,
                Genres = await _context.Genres.OrderBy(o => o.Name).ToListAsync()
            };
            return View("MovieForm", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(MovieFromViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(o => o.Name).ToListAsync();
                return View("MovieForm", model);
            }


            var movie = await _context.Movies.FindAsync(model.Id);

            if (movie == null)
                return NotFound();

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();
                if (!_allowedExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(o => o.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .png or jpg are allowed");
                    return View("MovieForm", model);
                }
                if (poster.Length > _maxAllowedPosterSize)
                {
                    model.Genres = await _context.Genres.OrderBy(o => o.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB");
                    return View("MovieForm", model);
                }

                movie.Poster=model.Poster; 
            }

            movie.Title = model.Title;
            movie.GenreId = model.GenreId;
            movie.Year = model.Year;
            movie.Rate = model.Rate;
            movie.Storyline = model.Storyline;

            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Movie Update Successfully");
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.Include(g=>g.Genre).SingleOrDefaultAsync(i=>i.Id==id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return  Ok();
        }

     
    }
}
