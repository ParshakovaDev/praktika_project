using ParshakovaLib;
using ParshakovaLib.Context;
using ParshakovaLib.Models;

namespace ParshakovaTests
{
    [TestClass]
    public sealed class ParshakovaTests
    {
        private ApplicationContext CreateContext()
        {
            return new ApplicationContext();
        }

        private void CleanupTestData(ApplicationContext context)
        {
            var testPartners = context.Partner
                .Where(p => p.Name.StartsWith("Test_"))
                .ToList();

            if (testPartners.Count == 0)
                return;

            var partnerIds = testPartners.Select(p => p.Id).ToList();

            var sales = context.Sale
                .Where(s => s.Partnerid != null && partnerIds.Contains(s.Partnerid.Value))
                .ToList();
            if (sales.Count > 0)
            {
                context.Sale.RemoveRange(sales);
            }

            var discounts = context.Discount
                .Where(d => d.Partnerid != null && partnerIds.Contains(d.Partnerid.Value))
                .ToList();
            if (discounts.Count > 0)
            {
                context.Discount.RemoveRange(discounts);
            }

            context.Partner.RemoveRange(testPartners);
            context.SaveChanges();
        }

        [TestMethod]
        public void UpdateDiscounts_SetsCorrectDiscount_ForDifferentTotalSales()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var partners = new[]
                {
                new Partner
                {
                    Name = "Test_P1",
                    Type = "T",
                    Director = "D",
                    Email = "e1@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1,
                    Sale = new List<Sale> { new() { Quantity = 1000, ProductName = "Test_Prod", Date = DateTime.Today } }
                },
                new Partner
                {
                    Name = "Test_P2",
                    Type = "T",
                    Director = "D",
                    Email = "e2@test",
                    Phone = "2",
                    Legaladdress = "addr",
                    Rating = 1,
                    Sale = new List<Sale> { new() { Quantity = 20000, ProductName = "Test_Prod", Date = DateTime.Today } }
                },
                new Partner
                {
                    Name = "Test_P3",
                    Type = "T",
                    Director = "D",
                    Email = "e3@test",
                    Phone = "3",
                    Legaladdress = "addr",
                    Rating = 1,
                    Sale = new List<Sale> { new() { Quantity = 100000, ProductName = "Test_Prod", Date = DateTime.Today } }
                },
                new Partner
                {
                    Name = "Test_P4",
                    Type = "T",
                    Director = "D",
                    Email = "e4@test",
                    Phone = "4",
                    Legaladdress = "addr",
                    Rating = 1,
                    Sale = new List<Sale> { new() { Quantity = 400000, ProductName = "Test_Prod", Date = DateTime.Today } }
                },
            };

                context.Partner.AddRange(partners);
                context.SaveChanges();

                var service = new Service();

                service.UpdateDiscounts(context);

                var discounts = context.Discount
                    .Where(d => d.Partner != null && d.Partner.Name.StartsWith("Test_"))
                    .OrderBy(d => d.Partnerid)
                    .Select(d => d.Percentage ?? 0)
                    .ToList();

                CollectionAssert.AreEqual(new List<double> { 0, 5, 10, 15 }, discounts);
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void GetPartnerByProperties_ReturnsPartnerByName()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                context.Partner.Add(new Partner
                {
                    Name = "Test_Partner",
                    Type = "T",
                    Director = "D",
                    Email = "e@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                });
                context.SaveChanges();

                var service = new Service();

                var result = service.GetPartnerByProperties(context, "Test_Partner");

                Assert.IsNotNull(result);
                Assert.AreEqual("Test_Partner", result.Name);
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void AddPartner_AddsNewPartner()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var service = new Service();
                var partner = new Partner
                {
                    Name = "Test_AddPartner",
                    Type = "T",
                    Director = "D",
                    Email = "e@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                };

                service.AddPartner(context, partner);

                Assert.IsTrue(context.Partner.Any(p => p.Name == "Test_AddPartner"));
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void UpdatePartner_UpdatesExistingPartner()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var partner = new Partner
                {
                    Name = "Test_UpdatePartner",
                    Type = "T",
                    Director = "D",
                    Email = "e@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                };
                context.Partner.Add(partner);
                context.SaveChanges();

                var service = new Service();
                partner.Name = "Test_UpdatePartner_Changed";

                service.UpdatePartner(context, partner);

                Assert.IsTrue(context.Partner.Any(p => p.Name == "Test_UpdatePartner_Changed"));
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void DeletePartner_DeletesPartnerAndSales()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var partner = new Partner
                {
                    Name = "Test_DeletePartner",
                    Type = "T",
                    Director = "D",
                    Email = "e@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                };
                context.Partner.Add(partner);
                context.SaveChanges();

                var sale = new Sale
                {
                    Partnerid = partner.Id,
                    ProductName = "Test_DeleteSale",
                    Quantity = 5,
                    Date = DateTime.Today
                };
                context.Sale.Add(sale);
                context.SaveChanges();

                var service = new Service();

                service.DeletePartner(context, partner);

                Assert.IsFalse(context.Partner.Any(p => p.Name == "Test_DeletePartner"));
                Assert.IsFalse(context.Sale.Any(s => s.ProductName == "Test_DeleteSale"));
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void AddSale_AddsNewSale()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var partner = new Partner
                {
                    Name = "Test_AddSalePartner",
                    Type = "T",
                    Director = "D",
                    Email = "e@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                };
                context.Partner.Add(partner);
                context.SaveChanges();

                var service = new Service();

                var sale = new Sale
                {
                    Partnerid = partner.Id,
                    ProductName = "Test_AddSale",
                    Quantity = 10,
                    Date = DateTime.Today
                };

                service.AddSale(context, sale);

                Assert.IsTrue(context.Sale.Any(s => s.ProductName == "Test_AddSale"));
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void UpdateSale_UpdatesExistingSale()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var partner = new Partner
                {
                    Name = "Test_UpdateSalePartner",
                    Type = "T",
                    Director = "D",
                    Email = "e@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                };
                context.Partner.Add(partner);
                context.SaveChanges();

                var sale = new Sale
                {
                    Partnerid = partner.Id,
                    ProductName = "Test_UpdateSale",
                    Quantity = 10,
                    Date = DateTime.Today
                };
                context.Sale.Add(sale);
                context.SaveChanges();

                var service = new Service();
                sale.Quantity = 20;

                service.UpdateSale(context, sale);

                Assert.AreEqual(20, context.Sale.Single(s => s.ProductName == "Test_UpdateSale").Quantity);
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void DeleteSale_DeletesExistingSale()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var partner = new Partner
                {
                    Name = "Test_DeleteSalePartner",
                    Type = "T",
                    Director = "D",
                    Email = "e@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                };
                context.Partner.Add(partner);
                context.SaveChanges();

                var sale = new Sale
                {
                    Partnerid = partner.Id,
                    ProductName = "Test_DeleteSale",
                    Quantity = 10,
                    Date = DateTime.Today
                };
                context.Sale.Add(sale);
                context.SaveChanges();

                var service = new Service();

                service.DeleteSale(context, sale);

                Assert.IsFalse(context.Sale.Any(s => s.ProductName == "Test_DeleteSale"));
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void GetSales_ReturnsSalesForPartner()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var partner1 = new Partner
                {
                    Name = "Test_P1",
                    Type = "T",
                    Director = "D",
                    Email = "e1@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                };
                var partner2 = new Partner
                {
                    Name = "Test_P2",
                    Type = "T",
                    Director = "D",
                    Email = "e2@test",
                    Phone = "2",
                    Legaladdress = "addr",
                    Rating = 1
                };

                context.Partner.AddRange(partner1, partner2);
                context.SaveChanges();

                context.Sale.AddRange(
                    new Sale { Partnerid = partner1.Id, ProductName = "Test_A", Quantity = 1, Date = DateTime.Today },
                    new Sale { Partnerid = partner1.Id, ProductName = "Test_B", Quantity = 2, Date = DateTime.Today },
                    new Sale { Partnerid = partner2.Id, ProductName = "Test_C", Quantity = 3, Date = DateTime.Today }
                );
                context.SaveChanges();

                var service = new Service();

                var salesForP1 = service.GetSales(context, partner1).ToList();

                Assert.AreEqual(2, salesForP1.Count);
                CollectionAssert.AreEquivalent(new[] { "Test_A", "Test_B" }, salesForP1.Select(s => s.ProductName).ToList());
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void GetPartners_ReturnsPartnersWithDiscounts()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var partner = new Partner
                {
                    Name = "Test_GetPartners",
                    Type = "T",
                    Director = "D",
                    Email = "e@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                };
                context.Partner.Add(partner);
                context.SaveChanges();

                context.Discount.Add(new Discount
                {
                    Partnerid = partner.Id,
                    Percentage = 5
                });
                context.SaveChanges();

                var service = new Service();

                var partners = service.GetPartners(context)
                    .Where(p => p.Name == "Test_GetPartners")
                    .ToList();

                Assert.AreEqual(1, partners.Count);
                Assert.IsTrue(partners[0].Discount.Any());
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void LoadPartners_ReturnsPartnersWithDiscounts()
        {
            using var context = CreateContext();
            CleanupTestData(context);
            try
            {
                var partner = new Partner
                {
                    Name = "Test_LoadPartners",
                    Type = "T",
                    Director = "D",
                    Email = "e@test",
                    Phone = "1",
                    Legaladdress = "addr",
                    Rating = 1
                };
                context.Partner.Add(partner);
                context.SaveChanges();

                context.Discount.Add(new Discount
                {
                    Partnerid = partner.Id,
                    Percentage = 10
                });
                context.SaveChanges();

                var service = new Service();

                var partners = service.LoadPartners(context)
                    .Where(p => p.Name == "Test_LoadPartners")
                    .ToList();

                Assert.AreEqual(1, partners.Count);
                Assert.IsTrue(partners[0].Discount.Any());
            }
            finally
            {
                CleanupTestData(context);
            }
        }

        [TestMethod]
        public void SaveChanges_DoesNotThrow_WhenContextIsNull()
        {
            var service = new Service();

            service.SaveChanges(null);
        }
    }
}