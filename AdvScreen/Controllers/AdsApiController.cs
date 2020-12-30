using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dal.Data;
using Dal.Models;

namespace AdvScreen.Controllers
{
    //[Route("api/[controller]")]
    //[Route("[controller]")]
    //[ApiController]
    public class AdsApiController : ControllerBase
    {

        
        private readonly ApplicationDbContext _context;

        public AdsApiController(ApplicationDbContext context)
        {            
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;
        }

        //GET: api/AdsApi
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Advertisement>>> GetAdvertisements()
        {
            return await _context.Advertisements.ToListAsync();
        }

        // GET: api/AdsApi/5
        //[HttpGet("{id}")]
        [HttpGet]
        //  [Route("[action]/{id}")]
        //[HttpGet("getadvertisement/{id}")]

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("[controller]/[action]/{pointId}")]
        [Route("[controller]/[action]/{pointId}/{curAdId}")]       
        public async Task<ActionResult<Advertisement>> GetAdvertisement(int pointId, int? curAdId)
        {
            var advs = _context.Advertisements.Where(a => a.PointId == pointId && a.AdvertisementStatus.Name == "Active")
                //.Include(a => a.ApplicationUser)
                .Include(a => a.Point)
                .Include(a => a.AdvertisementStatus)
                .OrderBy(a => a.StartDate)
                ;
            Advertisement advertisement;

            if (curAdId.HasValue)
            {
                var CurrentAdvertisement = _context.Advertisements.Find(curAdId);
                var existAds = advs.Where(elem => elem.StartDate > CurrentAdvertisement.StartDate).Any();
                if (existAds)
                {
                    advertisement = await advs.Where(elem => elem.StartDate > CurrentAdvertisement.StartDate).FirstOrDefaultAsync();
                }
                else
                {
                    advertisement = await advs.FirstOrDefaultAsync();
                }
            }
            else
            {
                advertisement = await advs.FirstOrDefaultAsync();
            }
            if (advertisement == null)
            {
                return NotFound();
            }
            return Ok(advertisement);
        }




        //[HttpGet("{id}")]

        //[HttpGet("GetPoint/{id}")]
        [HttpGet]
        //[Route("[action]/{id}")]
        public async Task<ActionResult<Point>> GetPoint(int id)
        {
            var point = await _context.Points.FindAsync(id);

            if (point == null)
            {
                return NotFound();
            }

            return point;
        }
    }
}

