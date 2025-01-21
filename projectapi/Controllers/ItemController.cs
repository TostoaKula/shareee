using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectapi.Context;
using projectapi.Models;

namespace projectapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ItemController(ApplicationDbContext context)
        { _context = context; }

        [HttpGet("allitems")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            return await _context.Items.ToListAsync();

        }
        [HttpPost("postitem")]
        public async Task<ActionResult<Item>> PostItem(Item items)
        {
            var item = new Item()
            {
                Id = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.Now,
                Name = items.Name,
                Description = items.Description,
                Price = items.Price,
                IsAvailable = items.IsAvailable,
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItems", new { id = items.Id }, items);

        }
    }
}

