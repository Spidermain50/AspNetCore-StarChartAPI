using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
	[Route("")]
	[ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;			
        }

		[HttpGet("{id:int}", Name="GetById")]
		public IActionResult GetById (int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

			if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where( e => e.Id == id ).ToList();

            return Ok(celestialObject);
        }

		[HttpGet("{name}")]
		public IActionResult GetByName (string name)
        {
            var celestialObjects = _context.CelestialObjects.Where( co => co.Name == name ).ToList();

			if (!celestialObjects.Any())
            {
                return NotFound();
            }

			foreach(var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where( e => e.OrbitedObjectId == celestialObject.Id ).ToList();
            }

            return Ok( celestialObjects );
        }

		[HttpGet]
		public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach( var celestialObject in celestialObjects )
            {
                celestialObject.Satellites = _context.CelestialObjects.Where( e => e.OrbitedObjectId == celestialObject.Id ).ToList();
            }
            return Ok( celestialObjects );
        }

		[HttpPost]
		public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.Add( celestialObject );
            _context.SaveChanges();

            return CreatedAtRoute( "GetById", new CelestialObject() { Id = celestialObject.Id } );
        }

		[HttpPut("id")]
		public IActionResult Update(int id, CelestialObject sourceCelestialObject)
        {
            var targetCelestialObject = _context.CelestialObjects.Find( id );

            if ( targetCelestialObject == null )
            {
                return NotFound();
            }

            targetCelestialObject.Name = sourceCelestialObject.Name;
            targetCelestialObject.OrbitalPeriod = sourceCelestialObject.OrbitalPeriod;
            targetCelestialObject.OrbitedObjectId = sourceCelestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(targetCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

		[HttpPatch("{id}/{name}")]
		public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find( id );

			if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = name;

            _context.CelestialObjects.Update( celestialObject );
            _context.SaveChanges();

            return NoContent();
        }

		[HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where( co => co.Id == id || co.OrbitedObjectId == id ).ToList();

			if (celestialObjects.Count == 0)
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange( celestialObjects );
            _context.SaveChanges();

            return NoContent();
        }

    }
}
