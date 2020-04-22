using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deliver.Data.Api;
using Deliver.Data.Common;
using Deliver.Data.Models;
using Deliver.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Deliver.Web.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Deliver.Web.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class ShopsController : BaseController
    {
        static PaymentMethod[] allPaymentMethods = (PaymentMethod[])Enum.GetValues(typeof(PaymentMethod));
        private readonly IHttpContextAccessor contextAccessor;
        private readonly UserManager<User> _userManager;

        public ShopsController(DeliverDbContext context,
            ILogger<ShopsController> logger,
            IHttpContextAccessor contextAccessor,
            UserManager<User> userManager
            ) : base(context, logger)
        {
            this.contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        [HttpGet("/api/shops/{id}")]
        public async Task<ApiResponse> Get(string id)
        {
            var item = await BaseQuery().Where(x => x.IsActive && x.Id == id).Select(x => new
            {
                x.Id,
                x.Name,
                Islands = x.Islands.Select(i => i.Island).Select(i => new { i.Id, i.Name }).ToArray(),
                x.PhoneNumber,
                x.BmlAccount,
                x.MibAccount,
                PaymentMethods = x.PaymentMethods
                        .Where(m => m.IsActive)
                        .Select(m => m.PaymentMethod)
                        .ToArray()
            }).FirstOrDefaultAsync();

            return Success(item);
        }

        [HttpGet("/api/shops")]
        public async Task<ApiResponse> GetAll(string[] ids = null, string search = null, int island = -1, int page = 1, int pageSize = 10)
        {
            var query = BaseQuery().Where(x => x.IsActive);

            if (ids?.Any() ?? false)
                query = query.Where(x => ids.Contains(x.Id));

            if (search.HasValue())
            {
                search = search.ToLowerInvariant();
                query = query.Where(x =>
                x.Name.ToLower().Contains(search) ||
                (x.Islands != null && x.Islands.Any(i => i.Island.Name.ToLower().Contains(search))
                ));
            }

            if(island > 0)
            {
                query = query.Where(x => x.Islands != null
                && x.Islands.Any(i => i.IslandId == island));
            }

            var items = await query
                .OrderBy(x => x.Name)
                .Select(x => new
            {
                x.Id,
                x.Name,
                Islands = x.Islands.Select(i => i.Island).Select(i => new { i.Id, i.Name }).ToArray(),
                x.PhoneNumber,
                    PaymentMethods = x.PaymentMethods
                        .Where(m => m.IsActive)
                        .Select(m => m.PaymentMethod)
                        .ToArray()
                })
                .PaginateAsync(this, page, pageSize);

            return Success(items);
        }

        [Authorize]
        [HttpGet("/api/shops/orders")]
        public async Task<ApiResponse> GetOrders(
            string[] ids = null,
            string search = null,
            PaymentMethod? paymentMethod = null,
            DateTime? date = null,
            OrderStatus? status = null,
            int island = -1,
            int page = 1,
            int pageSize = 10)
        {
            var userId = contextAccessor.HttpContext.User.GetUserId();
            var query = Db.Orders.Where(x => x.UserId == userId && x.IsActive);

            if (ids?.Any() ?? false)
                query = query.Where(x => ids.Contains(x.Id));

            if (paymentMethod.HasValue)
            {
                query = query.Where(x => x.PaymentMethods.Any(m => m.PaymentMethod == paymentMethod.Value));
            }

            if (date.HasValue)
            {
                var end = date.Value.Date.AddDays(1);
                query = query.Where(x => x.CreationTime >= date.Value && x.CreationTime < end);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (search.HasValue())
            {
                search = search.ToLowerInvariant();
                query = query.Where(x =>
                x.Id.ToLower().Contains(search) ||
                (x.Island != null && x.Island.Name.ToLower().Contains(search)) ||
                x.Address.ToLower().Contains(search) ||
                (x.Notes != null && x.Notes.ToLower().Contains(search))
                );
            }

            if (island > 0)
            {
                query = query.Where(x => x.IslandId == island);
            }

            var items = await query
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new
                {
                    x.Id,
                    x.Address,
                    x.Price,
                    x.Notes,
                    Island = new { x.Island.Id, x.Island.Name },
                    OrderItems = x.OrderItems.Where(i => i.IsActive).Select(i => new { i.OrderDetails, i.Notes }),
                    x.PhoneNumber,
                    x.Status,
                    PaymentMethods = x.PaymentMethods
                        .Where(m => m.IsActive)
                        .Select(m => m.PaymentMethod)
                        .ToArray(),
                })
                .PaginateAsync(this, page, pageSize);

            return Success(items);
        }

        [Authorize]
        [HttpGet("/api/shops/orders/{id}")]
        public async Task<ApiResponse> GetOrder(string id)
        {
            var userId = contextAccessor.HttpContext.User.GetUserId();
            var order = await Db.Orders.Where(x => x.UserId == userId && x.IsActive && x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Address,
                    x.Price,
                    x.Notes,
                    Island = new { x.Island.Id, x.Island.Name },
                    OrderItems = x.OrderItems.Where(i => i.IsActive).Select(i => new { i.OrderDetails, i.Notes }),
                    x.PhoneNumber,
                    x.Status,
                    PaymentMethods = x.PaymentMethods
                        .Where(m => m.IsActive)
                        .Select(m => m.PaymentMethod)
                        .ToArray()
                }).FirstOrDefaultAsync();

            return Success(order);
        }

        [Authorize]
        [HttpPost("/api/shops/orders/{id}")]
        public async Task<ApiResponse> UpdateOrder([FromBody]UpdateOrderVm vm, string id)
        {
            var userName = contextAccessor.HttpContext.User.Identity.Name;
            var order = await Db.Orders.Where(x => x.User.UserName == userName && x.IsActive && x.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new ArgumentException(nameof(id));

            if(vm.Status.HasValue)
                order.Status = vm.Status.Value;
            if (vm.Price.HasValue)
                order.Price = vm.Price.Value;
            order.Notes = vm.Notes;

            Db.Orders.Update(order);
            await Db.SaveChangesAsync();
            return Success(true);
        }

        //[HttpPost("/api/shops")]
        //[HttpPut("/api/shops")]
        //public async Task<ApiResponse> CreateOrUpdate([FromBody]User[] users)
        //{
        //    if (!users?.Any() ?? true)
        //        throw new ArgumentNullException(nameof(users));

        //    if (users.Any(x => x.Id.IsEmpty()))
        //        Db.UpdateRange(users.Where(x => x.Id.IsEmpty()));
        //    if (users.Any(x => !x.Id.IsEmpty()))
        //        await Db.AddRangeAsync(users.Where(x => !x.Id.IsEmpty()));

        //    await Db.SaveChangesAsync();
        //    return Success(users);
        //}

        private IQueryable<User> BaseQuery() => Db.Users.Where(x => x.UserType == UserType.Shop);
    }
}