using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParshakovaLib.Context;
using ParshakovaLib.Models;

namespace ParshakovaLib
{
    // Сервис работы с партнёрами и продажами
    public class Service : IService
    {
        // Пересчёт скидок партнёров на основе общего объёма продаж
        public void UpdateDiscounts(ApplicationContext _context)
        {
            var partners = _context.Partner
                .Include(p => p.Sale)
                .ToList();
            if (partners != null)
            {
                foreach (var partner in partners)
                {
                    var totalSales = partner.Sale?.Sum(s => s.Quantity) ?? 0;

                    double discountPercentageage;

                    if (totalSales <= 10000)
                    {
                        discountPercentageage = 0;
                    }
                    else if (totalSales <= 50000)
                    {
                        discountPercentageage = 5;
                    }
                    else if (totalSales <= 300000)
                    {
                        discountPercentageage = 10;
                    }
                    else
                    {
                        discountPercentageage = 15;
                    }

                    var discount = _context.Discount.FirstOrDefault(d => d.Partnerid == partner.Id);
                    if (discount == null)
                    {
                        _context.Discount.Add(new Discount
                        {
                            Partnerid = partner.Id,
                            Percentage = discountPercentageage
                        });
                    }
                    else
                    {
                        discount.Percentage = discountPercentageage;
                    }
                }
                _context.SaveChanges();
            }

        }

        // Поиск партнёра по имени (с подгрузкой продаж)
        public Partner GetPartnerByProperties(ApplicationContext _context, string name)
        {
            return _context.Partner
                .Include(p => p.Sale)
                .FirstOrDefault(p =>
                    (p.Name == name)

                );
        }

        // Безопасное сохранение изменений контекста
        public void SaveChanges(ApplicationContext _context)
        {
            if (_context != null)
            {
                _context.SaveChanges();
            }
        }

        // Загрузка партнёров вместе со скидками
        public List<Partner> LoadPartners(ApplicationContext _context)
        {
            return _context.Partner
                .Include(p => p.Discount)
                .ToList();
        }

        // Добавление нового партнёра
        public void AddPartner(ApplicationContext _context, Partner partner)
        {
            if (partner != null)
            {
                _context.Partner.Add(partner);
                _context.SaveChanges();
            }
        }

        // Обновление данных существующего партнёра
        public void UpdatePartner(ApplicationContext _context, Partner partner)
        {
            if (partner != null)
            {
                _context.Partner.Update(partner);
                _context.SaveChanges();
            }
        }

        // Получение продаж конкретного партнёра
        public IEnumerable<Sale> GetSales(ApplicationContext _context, Partner partner)
        {
            if (partner == null)
                return Enumerable.Empty<Sale>();

            return _context.Sale
                            .Where(s => s.Partnerid == partner.Id)
                            .ToList();
        }

        // Удаление партнёра и всех его продаж
        public void DeletePartner(ApplicationContext _context, Partner partner)
        {
            if (partner != null)
            {
                var sales = _context.Sale
                                    .Where(s => s.Partnerid == partner.Id)
                                    .ToList();
                if (sales != null)
                {
                    _context.Sale.RemoveRange(sales);
                }
                _context.Partner.Remove(partner);
                _context.SaveChanges();
            }
        }

        // Получение всех партнёров со скидками
        public IEnumerable<Partner> GetPartners(ApplicationContext _context)
        {
            return _context.Partner.Include(p => p.Discount).ToList();
        }

        // Добавление новой продажи
        public void AddSale(ApplicationContext _context, Sale sale)
        {
            if (sale != null)
            {
                _context.Sale.Add(sale);
                _context.SaveChanges();
            }
        }

        // Обновление данных продажи
        public void UpdateSale(ApplicationContext _context, Sale sale)
        {
            if (sale != null)
            {
                _context.Sale.Update(sale);
                _context.SaveChanges();
            }
        }

        // Удаление продажи
        public void DeleteSale(ApplicationContext _context, Sale sale)
        {
            if (sale != null)
            {
                _context.Sale.Remove(sale);
                _context.SaveChanges();
            }
        }
    }
}
