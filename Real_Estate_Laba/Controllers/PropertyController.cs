using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Real_Estate_Laba.Data;
using Real_Estate_Laba.Helpers;
using Real_Estate_Laba.Models;
using Real_Estate_Laba.ViewModels;

namespace Real_Estate_Laba.Controllers
{
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Api_Service _service;
        private readonly AzureBlobStorageService _blob;
        private readonly IConfiguration _configuration;

        public PropertyController(ApplicationDbContext context, Api_Service service, AzureBlobStorageService blob, IConfiguration configuration)
        {
            _context = context;
            _service = service;
            _blob = blob;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> GetProperty(string city)
        {
            return Ok(await _service.GetPropertyData(city));
        }

        public async Task<IActionResult> Index()
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(await _context.Properties.Where(x => x.AgentId == agentId).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Properties == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties.Include(x => x.Location).Include(x => x.Image)
                .FirstOrDefaultAsync(m => m.Id == id);
            var location = await _context.Locations.FindAsync(property.Location.Id);
            if (@property == null)
            {
                return NotFound();
            }

            var propertyVM = new PropertyVM()
            {
                Property = @property,
                Location = location
            };
            return View(propertyVM);
        }

        public IActionResult Create()
        {
            var viewModel = new PropertyVM();
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyVM viewModel)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var property = new Property
            {
                Status = viewModel.Property.Status,
                Type = viewModel.Property.Type,
                BedroomCount = viewModel.Property.BedroomCount,
                BathroomCount = viewModel.Property.BathroomCount,
                Price = viewModel.Property.Price,
                Area = viewModel.Property.Area,
                AgentId = agentId,
                Location = viewModel.Location
            };

            if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.ImageFile.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var containerName = _configuration["containerName"]; 
                    var blobName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ImageFile.FileName);

                    var imageUrl = await _blob.UploadImageAsync(memoryStream, containerName, blobName);

                    property.Image = new Image { ImageData = imageUrl };
                }
            }

            await _context.Locations.AddAsync(viewModel.Location);
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Properties == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties.Include(x => x.Location).FirstOrDefaultAsync(x => x.Id == id);
            var propertyVm = new PropertyVM()
            {
                Property = property,
                Location = property.Location
            };
            if (@property == null)
            {
                return NotFound();
            }
            return View(propertyVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyVM propert)
        {
            if (id != propert.Property.Id)
            {
                return NotFound();
            }

            try
            {
                propert.Property.AgentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var existingProperty = await _context.Properties
                    .Include(p => p.Location)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingProperty == null)
                {
                    return NotFound();
                }

                existingProperty.Location.Country = propert.Location.Country;
                existingProperty.Location.City = propert.Location.City;
                existingProperty.Location.State = propert.Location.State;
                existingProperty.Location.Street = propert.Location.Street;

                existingProperty.Status = propert.Property.Status;
                existingProperty.Type = propert.Property.Type;
                existingProperty.BedroomCount = propert.Property.BedroomCount;
                existingProperty.BathroomCount = propert.Property.BathroomCount;
                existingProperty.Price = propert.Property.Price;
                existingProperty.Area = propert.Property.Area;

                _context.Properties.Update(existingProperty);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyExists(propert.Property.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Properties == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Properties == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Properties'  is null.");
            }
            var @property = await _context.Properties.Include(x => x.Location).FirstOrDefaultAsync(x => x.Id == id);
            if (@property != null)
            {
                _context.Properties.Remove(@property);
            }
            var location = await _context.Locations.FindAsync(property.Location.Id);
            if (location != null)
            {
                _context.Locations.Remove(location);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
            return (_context.Properties?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }







    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(PropertyVM viewModel)
    //{
    //    var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    //    if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
    //    {
    //        using (var memoryStream = new MemoryStream())
    //        {
    //            await viewModel.ImageFile.CopyToAsync(memoryStream);
    //            var image = new Image { ImageData = memoryStream.ToArray() };

    //            _context.Images.Add(image);
    //            await _context.SaveChangesAsync();

    //            viewModel.Property.Image = image;
    //        }
    //    }

    //    await _context.Locations.AddAsync(viewModel.Location);
    //    await _context.SaveChangesAsync();

    //    viewModel.Property.Location = viewModel.Location;
    //    viewModel.Property.AgentId = agentId;
    //    await _context.Properties.AddAsync(viewModel.Property);
    //    await _context.SaveChangesAsync();

    //    return RedirectToAction("Index");
    //}
}
