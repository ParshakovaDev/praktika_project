using ParshakovaLib.Context;
using ParshakovaLib.Models;
using System.Collections.Generic;

namespace ParshakovaLib
{
    public interface IService
    {
        void UpdateDiscounts(ApplicationContext context);

        Partner GetPartnerByProperties(ApplicationContext context, string name);

        void SaveChanges(ApplicationContext context);

        List<Partner> LoadPartners(ApplicationContext context);

        void AddPartner(ApplicationContext context, Partner partner);

        void UpdatePartner(ApplicationContext context, Partner partner);

        IEnumerable<Sale> GetSales(ApplicationContext context, Partner partner);

        void DeletePartner(ApplicationContext context, Partner partner);

        IEnumerable<Partner> GetPartners(ApplicationContext context);

        void AddSale(ApplicationContext context, Sale sale);

        void UpdateSale(ApplicationContext context, Sale sale);

        void DeleteSale(ApplicationContext context, Sale sale);
    }
}

