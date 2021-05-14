using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using POS.Class;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using static POS.Class.Master;

namespace POS.Forms
{
    public partial class Frm_Invoice : Frm_Master
    {
        private DAL.InvoiceHeader invoice;
        InvoiceType type;
        DAL.dbDataContext generalDB;
        RepositoryItemGridLookUpEdit repoItems;
        RepositoryItemLookUpEdit repoUOM;
        RepositoryItemLookUpEdit repoStores;

        DAL.InvoiceDetail detailsInstance = new DAL.InvoiceDetail();

        public Frm_Invoice(InvoiceType type)
        {
            InitializeComponent();
            this.type = type;
            lkp_PartType.EditValueChanged += Lkp_PartType_EditValueChanged;
            RefreshData();
            New();
        }

        private void Frm_Invoice_Load(object sender, EventArgs e)
        {
            spn_Total.EditValue = 800;
            lkp_PartType.InitializeData(PartTypesList);
            glkp_PartID.ButtonClick += Lkp_PartType_ButtonClick;
            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;

            gridView1.Columns[nameof(detailsInstance.ID)].Visible = false;
            gridView1.Columns[nameof(detailsInstance.InvoiceID)].Visible = false;

            repoUOM.InitializeData(Session.UnitNames, gridView1.Columns[nameof(detailsInstance.ItemUnitID)], gridControl1);
            repoStores.InitializeData(Session.Stores, gridView1.Columns[nameof(detailsInstance.StoreID)], gridControl1);

            repoItems = new RepositoryItemGridLookUpEdit();
            repoItems.InitializeData(Session.ProductsView.Where(x => x.IsActive == true), gridView1.Columns[nameof(detailsInstance.ItemID)], gridControl1);
            
            RepositoryItemSpinEdit spinEdit = new RepositoryItemSpinEdit();
            gridView1.Columns[nameof(detailsInstance.TotalPrice)].ColumnEdit = spinEdit;
            gridView1.Columns[nameof(detailsInstance.Price)].ColumnEdit = spinEdit;
            gridView1.Columns[nameof(detailsInstance.ItemQty)].ColumnEdit = spinEdit;
            gridView1.Columns[nameof(detailsInstance.DiscountValue)].ColumnEdit = spinEdit;

            RepositoryItemSpinEdit spinRatioEdit = new RepositoryItemSpinEdit();
            gridView1.Columns[nameof(detailsInstance.Discount)].ColumnEdit = spinRatioEdit;

            spinRatioEdit.Increment = 0.01m;
            spinRatioEdit.Mask.EditMask = "P";
            spinRatioEdit.Mask.UseMaskAsDisplayFormat = true;
            spinRatioEdit.MaxValue = 1;


            gridControl1.RepositoryItems.Add(spinRatioEdit);
            gridControl1.RepositoryItems.Add(spinEdit);

            gridView1.Columns[nameof(detailsInstance.TotalPrice)].OptionsColumn.AllowFocus = false;

            #region Events
            spn_DiscountValue.Enter += new EventHandler(this.spn_DiscountValue_Enter);
            spn_DiscountValue.Leave += Spn_DiscountValue_Leave;
            spn_DiscountValue.EditValueChanged += Spn_DiscountRation_EditValueChanged;
            spn_DiscountRation.EditValueChanged += Spn_DiscountRation_EditValueChanged;

            spn_TaxValue.Enter += Spn_TaxValue_Enter;
            spn_TaxValue.Leave += Spn_TaxValue_Leave;
            spn_TaxValue.EditValueChanged += Spn_TaxValue_EditValueChanged;
            spn_Tax.EditValueChanged += Spn_TaxValue_EditValueChanged;

            spn_TaxValue.EditValueChanged += Spn_EditValueChanged;
            spn_DiscountValue.EditValueChanged += Spn_EditValueChanged;
            spn_Expences.EditValueChanged += Spn_EditValueChanged;
            spn_Total.EditValueChanged += Spn_EditValueChanged;

            spn_Paid.EditValueChanged += Spn_Paid_EditValueChanged;
            spn_Net.EditValueChanged += Spn_Paid_EditValueChanged;
            spn_Net.EditValueChanging += Spn_Net_EditValueChanging;

            spn_Net.DoubleClick += Spn_Net_MouseDoubleClick; 

            gridView1.CustomRowCellEditForEditing += GridView1_CustomRowCellEditForEditing;
            gridView1.CellValueChanged += GridView1_CellValueChanged;
            gridView1.RowCountChanged += GridView1_RowCountChanged;
            gridView1.RowUpdated += GridView1_RowUpdated;
            #endregion
        }

        private void Spn_Net_MouseDoubleClick(object sender, EventArgs e)
        {
            spn_Paid.EditValue = spn_Net.EditValue;
        }

        private void Spn_Net_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (Convert.ToDouble(e.OldValue) == Convert.ToDouble(spn_Paid.EditValue))
                spn_Paid.EditValue = e.NewValue;
        }

        private void GridView1_RowUpdated(object sender, RowObjectEventArgs e)
        {
            var items = gridView1.DataSource as Collection<DAL.InvoiceDetail>;
            if (items == null)
                spn_Total.EditValue = 0;

            else
                spn_Total.EditValue = items.Sum(x => x.TotalPrice);
        }

        private void GridView1_RowCountChanged(object sender, EventArgs e)
        {
            GridView1_RowUpdated(sender, null);
        }

        private void GridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var row = gridView1.GetRow(e.RowHandle) as DAL.InvoiceDetail;
            if (row == null)
                return;
            var itemV = Session.ProductsView.Single(x => x.ID == row.ItemID);


            if (row.ItemUnitID == 0)
            {
                row.ItemUnitID = itemV.Units.First().UnitID;
                GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(detailsInstance.ItemUnitID)], row.ItemUnitID));
            }

            var unitV = itemV.Units.Single(x => x.UnitID == row.ItemUnitID);

            switch (e.Column.FieldName)
            {
                case nameof(detailsInstance.ItemUnitID):
                    if (type == Master.InvoiceType.Purchase || type == Master.InvoiceType.PurchaseReturn)
                        row.Price = unitV.BuyPrice;
                    if (row.ItemQty == 0)
                        row.ItemQty = 1;
                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(detailsInstance.Price)], row.Price));
                    break;

                case nameof(detailsInstance.Price):
                case nameof(detailsInstance.Discount):
                case nameof(detailsInstance.ItemQty):

                    row.DiscountValue = row.Discount * (row.ItemQty * row.Price);

                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(detailsInstance.DiscountValue)], row.DiscountValue));

                    break;

                case nameof(detailsInstance.DiscountValue):
                    if (gridView1.FocusedColumn.FieldName == nameof(detailsInstance.DiscountValue))
                        row.Discount = row.DiscountValue / (row.ItemQty * row.Price);
                    row.TotalPrice = (row.ItemQty * row.Price) - row.DiscountValue;

                    break;
                default:
                    break;
            }
        }

        private void GridView1_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "ItemUnitID")
            {
                RepositoryItemLookUpEdit repo = new RepositoryItemLookUpEdit();
                repo.NullText = "";
                e.RepositoryItem = repo;
                var row = gridView1.GetRow(e.RowHandle) as DAL.InvoiceDetail;
                if (row == null)
                    return;
                var item = Session.ProductsView.SingleOrDefault(x => x.ID == row.ItemID);
                if (item == null)
                    return;
                repo.DataSource = item.Units;
                repo.ValueMember = "UnitID";
                repo.DisplayMember = "UnitName";
            }
            else if (e.Column.FieldName == "ItemID")
            {
                e.RepositoryItem = repoItems;
            }
        }

        private void Lkp_PartType_EditValueChanged(object sender, EventArgs e)
        {
            if (lkp_PartType.IsEditValueOfTypeInt())
            {
                int partType = Convert.ToInt32(lkp_PartType.EditValue);
                if (partType == (int)Master.PartType.Customer)
                    glkp_PartID.InitializeData(Session.Customers);
                else if (partType == (int)Master.PartType.Vendor)
                    glkp_PartID.InitializeData(Session.Vendors);
            }
        }

        public override void RefreshData()
        {
            lkp_Branch.InitializeData(Session.Stores);
            lkp_Drawer.InitializeData(Session.Drawers);
            base.RefreshData();
        }

        public override bool IsDataValid()
        {
            int numberOfErrors = 0;

            numberOfErrors += txt_Code.IsTextValid() ? 0 : 1;
            numberOfErrors += lkp_PartType.IsEditValueValid() ? 0 : 1;
            numberOfErrors += lkp_Drawer.IsEditValueValid() ? 0 : 1;
            numberOfErrors += lkp_Branch.IsEditValueValid() ? 0 : 1;

            numberOfErrors += glkp_PartID.IsEditValueValidAndNotZero() ? 0 : 1;
            numberOfErrors += dt_Date.IsDateValid() ? 0 : 1;

            if (chk_PostToStore.Checked)
                numberOfErrors += dt_PostDate.IsDateValid() ? 0 : 1;

            return (numberOfErrors == 0);
        }
        #region SpenEditCalculations
        private void Spn_Paid_EditValueChanged(object sender, EventArgs e)
        {
            var net = Convert.ToDouble(spn_Net.EditValue);
            var paind = Convert.ToDouble(spn_Paid.EditValue);

            spn_Remaing.EditValue = net - paind;
        }

        private void Spn_EditValueChanged(object sender, EventArgs e)
        {
            var total = Convert.ToDouble(spn_Total.EditValue);
            var tax = Convert.ToDouble(spn_TaxValue.EditValue);
            var discound = Convert.ToDouble(spn_DiscountValue.EditValue);
            var expences = Convert.ToDouble(spn_Expences.EditValue);

            spn_Net.EditValue = (total + tax - discound + expences);


        }

        private void Spn_TaxValue_EditValueChanged(object sender, EventArgs e)
        {
            var total = Convert.ToDouble(spn_Total.EditValue);
            var val = Convert.ToDouble(spn_TaxValue.EditValue);
            var ratio = Convert.ToDouble(spn_Tax.EditValue);

            if (IsTaxValueFocused)
                spn_Tax.EditValue = (val / total);
            else
                spn_TaxValue.EditValue = total * ratio;

        }

        private bool IsTaxValueFocused;

        private void Spn_TaxValue_Leave(object sender, EventArgs e)
        {
            IsTaxValueFocused = false;
        }

        private void Spn_TaxValue_Enter(object sender, EventArgs e)
        {
            IsTaxValueFocused = true;
        }

        private void Spn_DiscountRation_EditValueChanged(object sender, EventArgs e)
        {
            var total = Convert.ToDouble(spn_Total.EditValue);
            var discountVal = Convert.ToDouble(spn_DiscountValue.EditValue);
            var discountRation = Convert.ToDouble(spn_DiscountRation.EditValue);

            if (IsDiscountValueFocused)
            {
                spn_DiscountRation.EditValue = (discountVal / total);
            }
            else
            {
                spn_DiscountValue.EditValue = total * discountRation;
            }
        }

        private void Lkp_PartType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
            {
                using (var frm = new Frm_CustomerVendor(Convert.ToInt32(lkp_PartType.EditValue) == (int)PartType.Customer))
                {
                    frm.ShowDialog();
                    RefreshData();
                }
            }
        }

        private bool IsDiscountValueFocused;
        private void spn_DiscountValue_Enter(object sender, EventArgs e)
        {
            IsDiscountValueFocused = true;
        }

        private void Spn_DiscountValue_Leave(object sender, EventArgs e)
        {
            IsDiscountValueFocused = false;
        }
        #endregion

        public override void New()
        {
            invoice = new DAL.InvoiceHeader()
            {
                Drawer = Session.Defaults.Drawer,
                Date = DateTime.Now,
                PostedToStore = true,
                PostDate = DateTime.Now,
            };

            switch (type)
            {
                case InvoiceType.PurchaseReturn:
                case InvoiceType.Purchase:
                    invoice.PartType = (int)PartType.Vendor;
                    invoice.PartID = (int)Session.Defaults.Vendor;
                    invoice.Branch = Session.Defaults.RawStore;
                    break;

                case InvoiceType.SalesReturn:
                case InvoiceType.Sales:
                    invoice.PartType = (int)PartType.Customer;
                    invoice.PartID = (int)Session.Defaults.Customer;
                    invoice.Branch = Session.Defaults.Store;
                    break;

                default:
                    throw new NotImplementedException();
            }

            base.New();
        }

        public override void GetData()
        {
            lkp_Branch.EditValue = invoice.Branch;
            lkp_Drawer.EditValue = invoice.Drawer;
            lkp_PartType.EditValue = invoice.PartType;
            glkp_PartID.EditValue = invoice.PartID;
            txt_Code.Text = invoice.Code;
            dt_Date.DateTime = invoice.Date;
            dt_DelivaryDate.EditValue = invoice.DeliveryDate;
            dt_PostDate.EditValue = invoice.PostDate;
            memoEme_ShoppingAddress.Text = invoice.ShippingAddress;
            me_Notes.Text = invoice.Notes;
            chk_PostToStore.Checked = invoice.PostedToStore;
            spn_DiscountRation.EditValue = invoice.DiscountRation;
            spn_DiscountValue.EditValue = invoice.DiscountVale;
            spn_Expences.EditValue = invoice.Expences;
            spn_Net.EditValue = invoice.Net;
            spn_Paid.EditValue = invoice.Paid;
            spn_Remaing.EditValue = invoice.Remaining;
            spn_Tax.EditValue = invoice.Tax;
            spn_TaxValue.EditValue = invoice.TaxValue;
            spn_Total.EditValue = invoice.Total;
            generalDB = new DAL.dbDataContext();
            gridControl1.DataSource = generalDB.InvoiceDetails.Where(x => x.InvoiceID == invoice.ID);
            base.GetData();
        }

        public override void SetData()
        {
            invoice.Branch = Convert.ToInt32(lkp_Branch.EditValue);
            invoice.Drawer = Convert.ToInt32(lkp_Drawer.EditValue);
            invoice.PartType = Convert.ToByte(lkp_PartType.EditValue);
            invoice.PartID = Convert.ToInt32(glkp_PartID.EditValue);
            invoice.Code = txt_Code.Text;
            invoice.Date = dt_Date.DateTime;
            invoice.DeliveryDate = dt_DelivaryDate.EditValue as DateTime?;
            invoice.PostDate = dt_PostDate.EditValue as DateTime?;
            invoice.ShippingAddress = memoEme_ShoppingAddress.Text;
            invoice.Notes = me_Notes.Text;
            invoice.PostedToStore = chk_PostToStore.Checked;
            invoice.DiscountRation = Convert.ToDouble(spn_DiscountRation.EditValue);
            invoice.DiscountVale = Convert.ToDouble(spn_DiscountValue.EditValue);
            invoice.Expences = Convert.ToDouble(spn_Expences.EditValue);
            invoice.Net = Convert.ToDouble(spn_Net.EditValue);
            invoice.Paid = Convert.ToDouble(spn_Paid.EditValue);
            invoice.Remaining = Convert.ToDouble(spn_Remaing.EditValue);
            invoice.Tax = Convert.ToDouble(spn_Tax.EditValue);
            invoice.TaxValue = Convert.ToDouble(spn_TaxValue.EditValue);
            invoice.Total = Convert.ToDouble(spn_Total.EditValue);

            base.SetData();
        }

    }
}
