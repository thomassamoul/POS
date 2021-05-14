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

        public static List<ValueAndID> ProductTypesList = new List<ValueAndID>()
            {
                new ValueAndID() { ID = 0, Name = "مخزني" },
                new ValueAndID() { ID = 1, Name = "خدمي" }
            };

        public enum ProductType
        {
            Inventory,
            Service
        }

        public static List<ValueAndID> PartTypesList = new List<ValueAndID>()
            {
                new ValueAndID() { ID = 0, Name = "موورد" },
                new ValueAndID() { ID = 1, Name = "عميل" }
        };

        public enum PartType
        {
            Vendor,
            Customer
        }

        public static List<ValueAndID> InvoiceTypesList = new List<ValueAndID>()
            {
                new ValueAndID() { ID = (int)InvoiceType.Purchase, Name = "مشتريات" },
                new ValueAndID() { ID = (int)InvoiceType.Sales, Name = "مبيعات" },
                new ValueAndID() { ID = (int)InvoiceType.PurchaseReturn, Name = "مردود مشتريات" },
                new ValueAndID() { ID = (int)InvoiceType.SalesReturn, Name = "مردود مبيعات" },
        };

        public enum InvoiceType
        {
            Purchase,
            Sales,
            PurchaseReturn,
            SalesReturn
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
    }
}