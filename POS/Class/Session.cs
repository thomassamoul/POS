using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static POS.Class.DatabaseWatcher;

namespace POS.Class
{
    public static class Session
    {

        public static class Defaults
        {
            public static int Drawer { get => 1; }
            public static int Customer { get => 1; }
            public static int Vendor { get => 2; }
            public static int Store { get => 1006; }
            public static int RawStore { get => 1006; }
            public static int DiscountAllowedAccount { get => 7; }
            public static int DiscountReceivedAccount { get => 6; }
             public static int SalesTax { get => 40; }
            public static int PurchaseTax { get => 41; }
            public static int PurchaseExpences { get => 42; }
        }
        public static class GlobalSettings
        {
            public static Boolean ReadFormScaleBarcode { get => true; }
            public static string ScaleBarcodePrefix { get => "21"; }
            public static byte ProductCodeLength { get => 5; }
            public static byte BarcodeLength { get => 13; }
            public static byte ValueCodeLength { get => 5; }
            public static ReadValueMode ReadMode { get => ReadValueMode.Price; }
            public static Boolean IgnoreCheckDigit { get => true; }
            public static byte DivideValueBy { get => 2; }

            public enum ReadValueMode
            {
                Weight,
                Price,
            }
        }

        private static BindingList<DAL.UnitName> unitNames;
        public static BindingList<DAL.UnitName> UnitNames
        {
            get
            {
                if (unitNames == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        unitNames = new BindingList<DAL.UnitName>(db.UnitNames.ToList());
                    }
                }
                return unitNames;
            }
        }

        private static BindingList<DAL.Product> _products;

        public static BindingList<DAL.Product> Products
        {
            get
            {
                if (_products == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _products = new BindingList<DAL.Product>(db.Products.ToList());
                    }
                    DatabaseWatcher.Products = new TableDependency.SqlClient.SqlTableDependency<Product>(Properties.Settings.Default.POSConnectionString);
                    DatabaseWatcher.Products.OnChanged += DatabaseWatcher.ProductsChanged;
                    DatabaseWatcher.Products.Start();
                }
                return _products;
            }
        }

        private static BindingList<ProductViewClass> productViewClasses;
        public static BindingList<ProductViewClass> ProductsView
        {
            get
            {
                if (productViewClasses == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        var data = from pr in Session.Products
                                   join cg in db.ProductCategories on pr.CategoryID equals cg.ID
                                   select new ProductViewClass
                                   {
                                       ID = pr.ID,
                                       Code = pr.Code,
                                       Name = pr.Name,
                                       CategoryName = cg.Name,
                                       Descreption = pr.Descreption,
                                       IsActive = pr.IsActive,
                                       Type = pr.Type,
                                       Units = (from u in db.ProductUnits
                                                where u.ProductID == pr.ID
                                                join un in db.UnitNames on u.UnitID equals un.ID
                                                select new ProductViewClass.ProductUOMView
                                                {
                                                    UnitID = u.UnitID,
                                                    UnitName = un.Name,
                                                    Factor = u.factor,
                                                    SellPrice = u.SellPrice,
                                                    BuyPrice = u.BuyPrice,
                                                    Barcode = u.Barcode,
                                                }).ToList()
                                   };
                        productViewClasses = new BindingList<ProductViewClass>(data.ToList());
                    }
                }
                return productViewClasses;
            }
        }

        public class ProductViewClass
        {
            public static ProductViewClass GetProduct(int id)
            {
                ProductViewClass obj;
                using (var db = new DAL.dbDataContext())
                {
                    var data = from pr in Session.Products
                               where pr.ID == id
                               join cg in db.ProductCategories on pr.CategoryID equals cg.ID
                               select new ProductViewClass
                               {
                                   ID = pr.ID,
                                   Code = pr.Code,
                                   Name = pr.Name,
                                   CategoryName = cg.Name,
                                   Descreption = pr.Descreption,
                                   IsActive = pr.IsActive,
                                   Type = pr.Type,
                                   Units = (from u in db.ProductUnits
                                            where u.ProductID == pr.ID
                                            join un in db.UnitNames on u.UnitID equals un.ID
                                            select new ProductViewClass.ProductUOMView
                                            {
                                                UnitID = u.UnitID,
                                                UnitName = un.Name,
                                                Factor = u.factor,
                                                SellPrice = u.SellPrice,
                                                BuyPrice = u.BuyPrice,
                                                Barcode = u.Barcode,
                                            }).ToList()
                               };
                    obj = data.First();
                };
                return obj;
            }
            public int ID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string Descreption { get; set; }
            public Boolean IsActive { get; set; }
            public byte Type { get; set; }
            public List<ProductUOMView> Units { get; set; }
            public class ProductUOMView
            {
                public int UnitID { get; set; }
                public string UnitName { get; set; }
                public Double Factor { get; set; }
                public Double SellPrice { get; set; }
                public Double BuyPrice { get; set; }
                public string Barcode { get; set; }

            }
        }

        private static BindingList<DAL.CustomersAndVendor> _vendors;

        public static BindingList<DAL.CustomersAndVendor> Vendors
        {
            get
            {
                if (_vendors == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _vendors = new BindingList<DAL.CustomersAndVendor>(db.CustomersAndVendors.Where(x => x.IsCustomer == false).ToList());
                    }
                    DatabaseWatcher.Vendors = new TableDependency.SqlClient.SqlTableDependency<CustomersAndVendors>(Properties.Settings.Default.POSConnectionString,
                        filter: new DatabaseWatcher.VendorsOnly());
                    DatabaseWatcher.Vendors.OnChanged += VendorsChanged;
                    DatabaseWatcher.Vendors.Start();

                }
                return _vendors;
            }
        }

        private static BindingList<DAL.CustomersAndVendor> _customers;

        public static BindingList<DAL.CustomersAndVendor> Customers
        {
            get
            {
                if (_customers == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _customers = new BindingList<DAL.CustomersAndVendor>(db.CustomersAndVendors.Where(x => x.IsCustomer == true).ToList());
                    }
                    DatabaseWatcher.Customers = new TableDependency.SqlClient.SqlTableDependency<CustomersAndVendors>(Properties.Settings.Default.POSConnectionString,
                        filter: new DatabaseWatcher.CustomerOnly());
                    DatabaseWatcher.Customers.OnChanged += DatabaseWatcher.CustomerChanged;
                    DatabaseWatcher.Customers.Start();

                }
                return _customers;
            }
        }

        private static BindingList<DAL.Drawer> _drawer;

        public static BindingList<DAL.Drawer> Drawers
        {
            get
            {
                if (_drawer == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _drawer = new BindingList<DAL.Drawer>(db.Drawers.ToList());
                    }
                    // DatabaseWatcher.Product = new TableDependency.SqlClient.SqlTableDependency<Products>(Properties.Settings.Default.POSConnectionString);
                    // DatabaseWatcher.Product.OnChanged += DatabaseWatcher.ProductsChanged;
                    // DatabaseWatcher.Product.Start();

                }
                return _drawer;
            }
        }

        private static BindingList<DAL.Store> _store;

        public static BindingList<DAL.Store> Stores
        {
            get
            {
                if (_store == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _store = new BindingList<DAL.Store>(db.Stores.ToList());
                    }
                    // DatabaseWatcher.Product = new TableDependency.SqlClient.SqlTableDependency<Products>(Properties.Settings.Default.POSConnectionString);
                    // DatabaseWatcher.Product.OnChanged += DatabaseWatcher.ProductsChanged;
                    // DatabaseWatcher.Product.Start();

                }
                return _store;
            }
        }

    }
}