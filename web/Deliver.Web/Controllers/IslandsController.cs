using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deliver.Data.Api;
using Deliver.Data.Models;
using Deliver.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Deliver.Web.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class IslandsController : BaseController
    {
        public IslandsController(DeliverDbContext context, ILogger<IslandsController> logger) : base(context, logger)
        {
        }

        [HttpGet("/api/islands/{id}")]
        public async Task<ApiResponse> Get(int id)
        {
            var item = await Db.Islands.FirstOrDefaultAsync(x => x.IsActive && x.Id == id);
            return Success(item);
        }

        [HttpGet("/api/islands")]
        public async Task<ApiResponse> GetAll(int[] ids = null, string search = null, int page = 1, int pageSize = 10)
        {
            var query = Db.Islands.Where(x => x.IsActive);


            if (ids?.Any() ?? false)
                query = query.Where(x => ids.Contains(x.Id));

            if (search.HasValue())
            {
                search = search.ToLowerInvariant();
                query = query.Where(x => x.Name.ToLower().Contains(search));
            }

            var items = await query.Select(x => new
            {
                x.Id,
                x.Name,
            })
                .PaginateAsync(this, page, pageSize);
            return Success(items);
        }

        [HttpPost("/api/islands")]
        [HttpPut("/api/islands")]
        public async Task<ApiResponse> CreateOrUpdate([FromBody]Island[] islands)
        {
            if (!islands?.Any() ?? true)
                throw new ArgumentNullException(nameof(islands));

            if (islands.Any(x => x.Id > 0))
                Db.UpdateRange(islands.Where(x => x.Id > 0));
            if (islands.Any(x => x.Id < 1))
                await Db.AddRangeAsync(islands.Where(x => x.Id < 1));

            await Db.SaveChangesAsync();
            return Success(islands);
        }
    }
}