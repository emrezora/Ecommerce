using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public interface IRevenueService
    {
        IEnumerable<IGrouping<string, Order>> GetBaseDataGroupedBySalesChannel(DateTime from, DateTime to);
        IEnumerable<IGrouping<string, Order>> GetBaseDataGroupedBySalesChannel();
    }

    public class RevenueService : IRevenueService
    {
        private readonly ISession _session;

        public RevenueService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<IGrouping<string, Order>> GetBaseDataGroupedBySalesChannel(DateTime from, DateTime to)
        {
            return _session.QueryOver<Order>()
                        .Where(item => item.OrderDate >= from.Date && item.OrderDate <= to.Date)
                        .Cacheable()
                        .List()
                        .GroupBy(x => x.SalesChannel);
        }

        public IEnumerable<IGrouping<string, Order>> GetBaseDataGroupedBySalesChannel()
        {
            return _session.QueryOver<Order>()
                        .Where(item => item.OrderDate >= CurrentRequestData.Now.Date && item.OrderDate <= CurrentRequestData.Now.Date.AddHours(24))
                        .Cacheable()
                        .List()
                        .GroupBy(x => x.SalesChannel);
        }
    }
}