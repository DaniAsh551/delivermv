using CsvHelper;
using CsvHelper.Configuration;
using Deliver.Data.Common;
using Deliver.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Deliver.Web.Data
{
    public class DeliverDbContextInitializer
    {
        readonly DeliverDbContext _db;
        private readonly UserManager<User> _userManager;
        readonly Random _rnd = new Random();
        public DeliverDbContextInitializer(DeliverDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public void Seed()
        {
            _db.Database.EnsureCreated();
            //Check if db has been seeded before.
            if (_db.Islands.Any())
            {
                return;
            }


            SampleAtoll[] atolls = null;
            DoWithResource("atolls.csv", reader => 
            {
                atolls = reader.GetRecords<SampleAtoll>().ToArray();
            });

            SampleIsland[] seedIslands = null;
            DoWithResource("islands.csv", reader =>
            {
                seedIslands = reader.GetRecords<SampleIsland>().ToArray();
            });

            var islands = seedIslands.Select(x => new Island
            {
                Name = $"{atolls.Where(a => a.PseudoId == x.AtollId).FirstOrDefault().Name}. {x.Name}"
            }).OrderBy(x => x.Name).ToArray();

            _db.AddRange(islands);
            _db.SaveChanges();

            var noOfShops = _rnd.Next(50, 150);
            var shops = Enumerable.Range(0, noOfShops).Select(_ => {

                var name = InitializerHelpers.GetLoremIpsum(20);
                var paymentMethods = InitializerHelpers.GetRandomPaymentMethods();
                var _shop = new User 
                { 
                     UserName = name.Replace(' ', '_'),
                     PhoneNumber = _rnd.Next(7000000, 9999999).ToString(),
                     Islands = Enumerable.Range(1,_rnd.Next(1,3))
                        .Select(x => islands.GetRandom()).Distinct()
                        .Select(x => new IslandShop { Island = x }).ToArray(),
                     Name = name,
                     UserType = UserType.Shop,
                     PaymentMethods = paymentMethods
                        .Select(x => new UserPaymentMethod { PaymentMethod = x }).ToArray(),
                     BmlAccount = paymentMethods.Contains(PaymentMethod.BankTransferBml) 
                        ? ("773" + _rnd.Next(0000000000, 999999999).ToString().PadLeft(10, _rnd.Next(0, 9).ToString()[0])) : "",
                     MibAccount = paymentMethods.Contains(PaymentMethod.BankTransferMib) 
                        ? ("773" + _rnd.Next(0000000000, 999999999).ToString().PadLeft(10, _rnd.Next(0, 9).ToString()[0])) : ""
                };

                var createTask = _userManager.CreateAsync(_shop, "A1*" + name.Replace(' ', '_'));
                createTask.Wait();
                var result = createTask.Result;

                if(!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                return _shop;
            }).OrderBy(x => x.Name).ToArray();

            //create 20 orders for first shop
            var shop = shops[0];
            var shopPaymentMethods = shop.PaymentMethods.Select(x => x.PaymentMethod).ToArray();

            var orders = Enumerable.Range(0, 20).Select(i => {
                
                var order = new Order
                {
                    Address = InitializerHelpers.GetLoremIpsum(10),
                    Island = shop.Islands.GetRandom().Island,
                    OrderItems = Enumerable.Range(0, _rnd.Next(0, 2))
                                .Select(x => new OrderItem { OrderDetails = InitializerHelpers.GetLoremIpsum() })
                                .ToArray(),
                    PhoneNumber = _rnd.Next(7000000, 9999999).ToString(),
                    Notes = _rnd.Next(0, 1) > 0 ? InitializerHelpers.GetLoremIpsum() : null,
                    UserId = shop.Id,
                    PaymentMethods = InitializerHelpers.GetRandomPaymentMethods().Intersect(shopPaymentMethods)
                        .Select(x => new OrderPaymentMethod { PaymentMethod = x }).ToArray()
                };

                if (!order.PaymentMethods.Any())
                    order.PaymentMethods = new OrderPaymentMethod[] { new OrderPaymentMethod { PaymentMethod = PaymentMethod.CashOnDelivery } };

                return order;
            }).ToArray();

            _db.AddRange(orders);
            _db.SaveChanges();
        }

        private void DoWithResource(string resourceName, Action<CsvReader> action)
        {
            var dataCore = Assembly.GetExecutingAssembly();
            if (!dataCore.FullName.Contains("Deliver.Web"))
            {
                dataCore = Assembly.Load(dataCore.GetReferencedAssemblies().FirstOrDefault(x => x.FullName.Contains("Deliver.Web")));
            }
            using (var stream = dataCore.GetManifestResourceStream($"Deliver.Web.SeedData.{resourceName}"))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.MissingFieldFound = null;
                action(csv);
            }
        }
    }

    class SampleAtoll
    {
        public int PseudoId { get; set; }
        public string Name { get; set; }
    }

    class SampleIsland
    {
        public int AtollId { get; set; }
        public string Name { get; set; }
    }

    static class InitializerHelpers
    {
        private static Random Random = new Random();
        private static readonly string[] _loremIpsumWords = "lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur excepteur sint occaecat cupidatat non proident sunt in culpa qui officia deserunt mollit anim id est laborum sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium totam rem aperiam eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt neque porro quisquam est qui dolorem ipsum quia dolor sit amet consectetur adipisci velit sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem ut enim ad minima veniam quis nostrum exercitationem ullam corporis suscipit laboriosam nisi ut aliquid ex ea commodi consequatur quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur vel illum qui dolorem eum fugiat quo voluptas nulla pariatur"
               .Split(' ');

        public static string GetLoremIpsum(int charCount = 50)
        {
            var loremIpsum = string.Empty;
            while (loremIpsum.Length < charCount)
            {
                loremIpsum += _loremIpsumWords.GetRandom() + ' ';
            }

            loremIpsum = loremIpsum.Remove(loremIpsum.Length - 1, 1);
            loremIpsum = loremIpsum.LimitTo(charCount);
            loremIpsum = char.ToUpper(loremIpsum[0]) + loremIpsum.Remove(0, 1);
            return loremIpsum;
        }

        public static T GetRandom<T>(this IEnumerable<T> items)
        {
            var skip = Random.Next(items.Count());
            return items.Skip(skip).FirstOrDefault();
        }

        public static PaymentMethod[] GetRandomPaymentMethods() 
        {
            var method = PaymentMethod.CashOnDelivery;

            var allValues = (PaymentMethod[])Enum.GetValues(typeof(PaymentMethod));
            allValues = allValues.Where(x => x != method).ToArray();

            var noOfMethods = Random.Next(1, allValues.Length - 1);
            var methods = new List<PaymentMethod>();

            for(var i = 0; i < noOfMethods; ++i)
            {
                var _method = allValues.GetRandom();
                if (methods.Contains(_method))
                    continue;

                methods.Add(_method);
            }

            return methods.ToArray();
        }
    }
}
