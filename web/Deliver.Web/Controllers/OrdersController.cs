using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deliver.Data.Api;
using Deliver.Data.Common;
using Deliver.Data.Models;
using Deliver.Web.Data;
using Deliver.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Deliver.Web.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class OrdersController : BaseController
    {
        static PaymentMethod[] allPaymentMethods = (PaymentMethod[])Enum.GetValues(typeof(PaymentMethod));
        public OrdersController(DeliverDbContext context, ILogger<OrdersController> logger) : base(context, logger)
        {
        }

        [HttpGet("/api/Orders/{id}")]
        public async Task<ApiResponse> Get(string id)
        {
            var item = await Db.Orders.Include(x => x.OrderItems).
                Where(x => x.IsActive && x.Id == id).Select(x => new
                {
                    x.Id,
                    x.Address,
                    x.Price,
                    x.Notes,
                    Island = new { x.Island.Id, x.Island.Name },
                    OrderItems = x.OrderItems.Where(i => i.IsActive).Select(i => new { i.OrderDetails, i.Notes }),
                    x.PhoneNumber,
                    PaymentMethods = x.PaymentMethods.Select(m => m.PaymentMethod),
                    x.Status,
                    Shop = x.UserId
                }).FirstOrDefaultAsync();

            return Success(item);
        }

        [HttpGet("/api/Orders")]
        public async Task<ApiResponse> GetAll(string[] ids = null, string shopId = null, int page = 1, int pageSize = 10)
        {
            var query = Db.Orders.Include(x => x.OrderItems).Where(x => x.IsActive);

            if (ids?.Any() ?? false)
                query = query.Where(x => ids.Contains(x.Id));

            if (shopId.HasValue())
            {
                query = query
                    .Where(x => 
                    x.UserId != null && 
                    x.User != null && 
                    x.User.UserType == UserType.Shop && 
                    x.User.Id == shopId);
            }

            var items = await query
                .PaginateAsync(this, page, pageSize);

            return Success(items);
        }

        [HttpPost("/api/Orders")]
        [HttpPut("/api/Orders")]
        public async Task<ApiResponse> CreateOrUpdate([FromBody]OrderVm[] vms)
        {
            if (!vms?.Any() ?? true)
                throw new ArgumentNullException(nameof(vms));

            var storeIds = vms.Select(x => x.Store).ToArray();
            var validities = await Task.WhenAll(storeIds.Select(
                    id => Db.Users.Where(x => x.IsActive).Select(x => x.Id).ContainsAsync(id)
                    ));

            if (validities.Any(x => !x))
                throw new Exception("Failed to find specified store.");

            var orders = vms.Select(x => {
                var order = new Order
                {
                    UserId = x.Store,
                    Address = x.Address,
                    IslandId = x.Island,
                    PhoneNumber = x.Phone,
                    CreationTime = DateTime.Today,
                    OrderItems = x.OrderItems.Select(i => new OrderItem
                    {
                        OrderDetails = i
                    }).ToList(),
                    PaymentMethods = x.PaymentMethods
                        .Select(m => new OrderPaymentMethod { PaymentMethod = m })
                        .ToArray()
                };
                return order;
            }).ToArray();

            if (orders.Any(x => x.Id.HasValue()))
                Db.UpdateRange(orders.Where(x => x.Id.HasValue()));
            if (orders.Any(x => x.Id.IsEmpty()))
                await Db.AddRangeAsync(orders.Where(x => x.Id.IsEmpty()));

            await Db.SaveChangesAsync();
            return Success(orders.Select(x => x.Id).ToArray());
        }

        [HttpDelete("/api/Orders/{id}")]
        public async Task<ApiResponse> Remove(string id)
        {
            var order = await Db.Orders.Where(x => x.IsActive && x.Id == id).FirstOrDefaultAsync();
            if (order == null)
                return Success(true);

            order.IsActive = false;
            Db.Update(order);
            await Db.SaveChangesAsync();
            return order.IsActive ? Error(new Exception("Failed to remove order")) : Success(true);
        }
    }
}