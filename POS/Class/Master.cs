using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using static POS.Forms.Frm_Master;

namespace POS.Class
{
    public static class Master
    {
        public class ValueAndID
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        private static List<ValueAndID> productTypesList = new List<ValueAndID>()
            {
                new ValueAndID() { ID = 0, Name = "مخزني" },
                new ValueAndID() { ID = 1, Name = "خدمي" }
            };

        public enum ProductType
        {
            Inventory,
            Service
        }

        private static List<ValueAndID> partTypesList = new List<ValueAndID>()
            {
                new ValueAndID() { ID = 0, Name = "موورد" },
                new ValueAndID() { ID = 1, Name = "عميل" }
        };

        public enum PartType
        {
            Vendor,
            Customer
        }

        private static List<ValueAndID> invoiceTypesList = new List<ValueAndID>()
            {
                new ValueAndID() { ID = (int)InvoiceType.Purchase, Name = "مشتريات" },
                new ValueAndID() { ID = (int)InvoiceType.Sales, Name = "مبيعات" },
                new ValueAndID() { ID = (int)InvoiceType.PurchaseReturn, Name = "مردود مشتريات" },
                new ValueAndID() { ID = (int)InvoiceType.SalesReturn, Name = "مردود مبيعات" },
        };

        public static List<ValueAndID> PartTypesList { get => partTypesList; set => partTypesList = value; }
        public static List<ValueAndID> ProductTypesList { get => productTypesList; set => productTypesList = value; }
        public static List<ValueAndID> InvoiceTypesList { get => invoiceTypesList; set => invoiceTypesList = value; }

        public enum InvoiceType
        {
            Purchase = SourceType.Purchase,
            Sales = SourceType.Sales,
            PurchaseReturn = SourceType.PurchaseReturn,
            SalesReturn = SourceType.SalesReturn
        }

        public enum SourceType
        {
            Purchase,
            Sales,
            PurchaseReturn,
            SalesReturn
        }

        public enum CostDistributionOptions
        {
            ByPrice,
            ByQty,
        }

        public static bool IsTextValid(this TextEdit txt)
        {
            if (txt.Text.Trim() == string.Empty)
            {
                txt.ErrorText = errorText;
                return false;
            }
            return true;
        }

        public static bool IsEditValueValid(this LookUpEditBase lkp)
        {
            if (lkp.IsEditValueOfTypeInt() == false)
            {
                lkp.ErrorText = errorText;
                return false;
            }
            return true;
        }

        public static bool IsEditValueValidAndNotZero(this LookUpEditBase lkp)
        {
            if (lkp.IsEditValueOfTypeInt() == false || Convert.ToInt32(lkp.EditValue) == 0)
            {
                lkp.ErrorText = errorText;
                return false;
            }
            return true;
        }

        public static bool IsDateValid(this DateEdit dt)
        {
            if (dt.DateTime.Year < 1950)
            {
                dt.ErrorText = errorText;
                return false;
            }
            return true;
        }

        public static bool IsEditValueOfTypeInt(this LookUpEditBase edit)
        {
            var val = edit.EditValue;

            return (val is int || val is byte);
        }

        public static void InitializeData(this RepositoryItemLookUpEditBase repo, object dataSource, GridColumn column, GridControl grid)
        {
            InitializeData(repo, dataSource, column, grid, "Name", "ID");
        }
        public static void InitializeData(this RepositoryItemLookUpEditBase repo, object dataSource, GridColumn column, GridControl grid, string displayMember, string valueMember)
        {
            if (repo == null)
                repo = new RepositoryItemLookUpEdit();

            repo.DataSource = dataSource;
            repo.DisplayMember = displayMember;
            repo.ValueMember = valueMember;
            repo.NullText = "";
            column.ColumnEdit = repo;
            if (grid != null)
                grid.RepositoryItems.Add(repo);
        }

        public static void InitializeData(this LookUpEdit lkp, object dataSource)
        {
            lkp.InitializeData(dataSource, "Name", "ID");
        }

        public static void InitializeData(this LookUpEdit lkp, object dataSource, string displayMember, string valueMember)
        {
            lkp.Properties.DataSource = dataSource;
            lkp.Properties.ValueMember = valueMember;
            lkp.Properties.DisplayMember = displayMember;
            //lkp.Properties.PopulateColumns();
            //lkp.Properties.Columns[valueMember].Visible = false;
        }

        public static void InitializeData(this GridLookUpEdit lkp, object dataSource)
        {
            lkp.InitializeData(dataSource, "Name", "ID");
        }

        public static void InitializeData(this GridLookUpEdit lkp, object dataSource, string displayMember, string valueMember)
        {
            lkp.Properties.DataSource = dataSource;
            lkp.Properties.ValueMember = valueMember;
            lkp.Properties.DisplayMember = displayMember;
        }

        public static string GetNextNumberInString(string number)
        {
            if (number == string.Empty || number == null)

                return "1";
            string str1 = "";
            foreach (char c in number)
                str1 = char.IsDigit(c) ? str1 + c.ToString() : "";
            if (str1 == string.Empty)
                return number + "1";

            string str2 = str1.Insert(0, "1");
            str2 = (Convert.ToInt64(str2) + 1).ToString();
            string str3 = str2[0] == '1' ? str2.Remove(0, 1) : str2.Remove(0, 1).Insert(0, "1");

            int index = number.LastIndexOf(str1);
            number = number.Remove(index);
            number = number.Insert(index, str3);
            return number;
        }
    }
}