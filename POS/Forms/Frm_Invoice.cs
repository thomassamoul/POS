using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using POS.Class;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static POS.Class.Master;

namespace POS.Forms
{
    public partial class Frm_Invoice : Frm_Master
    {
        private DAL.InvoiceHeader invoice;
        InvoiceType type;
        DAL.dbDataContext generalDB;
        RepositoryItemLookUpEdit repoItemsAll;
        RepositoryItemGridLookUpEdit repoItems;
        private RepositoryItemLookUpEdit repoUOM;
        private RepositoryItemLookUpEdit repoStores;

        DAL.InvoiceDetail detailsInstance = new DAL.InvoiceDetail();

        public Frm_Invoice(InvoiceType type)
        {
            InitializeComponent();
            this.type = type;
            lkp_PartType.EditValueChanged += Lkp_PartType_EditValueChanged;
            RefreshData();
            New();
        }

        #region GridView    
        private void Frm_Invoice_Load(object sender, EventArgs e)
        {
            switch (type)
            {
                case InvoiceType.Purchase:
                    this.Text = "فاتورة مشتريات";
                    break;
                case InvoiceType.Sales:
                    break;
                case InvoiceType.PurchaseReturn:
                    break;
                case InvoiceType.SalesReturn:
                    break;
                default:
                    break;
            }

            lkp_PartType.InitializeData(PartTypesList);
            glkp_PartID.ButtonClick += Lkp_PartType_ButtonClick;
            gridView1.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;

            gridView1.Columns[nameof(detailsInstance.ID)].Visible = false;
            gridView1.Columns[nameof(detailsInstance.InvoiceID)].Visible = false;

            repoUOM.InitializeData(Session.UnitNames, gridView1.Columns[nameof(detailsInstance.ItemUnitID)], gridControl1);
            repoStores.InitializeData(Session.Stores, gridView1.Columns[nameof(detailsInstance.StoreID)], gridControl1);

            repoItems = new RepositoryItemGridLookUpEdit();
            repoItems.InitializeData(Session.ProductsView.Where(x => x.IsActive == true), gridView1.Columns[nameof(detailsInstance.ItemID)], gridControl1);
            repoItems.ValidateOnEnterKey = true;
            repoItems.AllowNullInput = DefaultBoolean.False;
            repoItems.BestFitMode = BestFitMode.BestFitResizePopup;
            repoItems.ImmediatePopup = true;
            repoItems.Buttons.Add(new EditorButton(ButtonPredefines.Plus));

            var repoView = repoItems.View;
            repoView.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            repoView.OptionsSelection.UseIndicatorForSelection = true;
            repoView.OptionsView.ShowAutoFilterRow = true;
            repoView.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.ShowAlways;
            repoView.PopulateColumns(repoItems.DataSource);
            repoView.Columns["IsActive"].Visible = false;
            repoView.Columns["Type"].Visible = false;

            repoView.Columns["ID"].Caption = "كود";
            repoView.Columns["Name"].Caption = "الاسم";
            repoView.Columns["Descreption"].Caption = "الوصف";
            repoView.Columns["CategoryName"].Caption = "الفئه";
            repoItemsAll.InitializeData(Session.ProductsView, gridView1.Columns[nameof(detailsInstance.ItemID)], gridControl1);

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

            gridView1.Columns.Add(new GridColumn() { Name = "clmCode", FieldName = "Code", Caption = "الكود", UnboundType = DevExpress.Data.UnboundColumnType.String });
            gridView1.Columns.Add(new GridColumn()
            {
                Name = "clmIndex",
                FieldName = "Index",
                Caption = "مسلسل",
                UnboundType = DevExpress.Data.UnboundColumnType.Integer,
                MaxWidth = 30
            });

            gridView1.Columns[nameof(detailsInstance.ItemID)].Caption = "الصنف";
            gridView1.Columns[nameof(detailsInstance.TotalCostValue)].Caption = "سعر التكلفه";
            gridView1.Columns[nameof(detailsInstance.Discount)].Caption = "ن.خصم";
            gridView1.Columns[nameof(detailsInstance.DiscountValue)].Caption = "ق.خصم";
            gridView1.Columns[nameof(detailsInstance.ItemQty)].Caption = "الكميه";
            gridView1.Columns[nameof(detailsInstance.ItemUnitID)].Caption = "الوحده";
            gridView1.Columns[nameof(detailsInstance.Price)].Caption = "السعر";
            gridView1.Columns[nameof(detailsInstance.StoreID)].Caption = "المخزن";
            gridView1.Columns[nameof(detailsInstance.TotalCostValue)].Caption = "اجمالي التكلفه";
            gridView1.Columns[nameof(detailsInstance.TotalPrice)].Caption = "اجمالي السعر";

            gridView1.Columns["Index"].OptionsColumn.AllowFocus = false;
            gridView1.Columns[nameof(detailsInstance.TotalCostValue)].OptionsColumn.AllowFocus = false;
            gridView1.Columns[nameof(detailsInstance.TotalCostValue)].OptionsColumn.AllowFocus = false;

            gridView1.Columns["Index"].VisibleIndex = 0;
            gridView1.Columns["Code"].VisibleIndex = 1;
            gridView1.Columns[nameof(detailsInstance.ItemID)].MinWidth = 70;
            gridView1.Columns[nameof(detailsInstance.ItemID)].VisibleIndex = 2;
            gridView1.Columns[nameof(detailsInstance.ItemUnitID)].VisibleIndex = 3;
            gridView1.Columns[nameof(detailsInstance.ItemQty)].VisibleIndex = 4;
            gridView1.Columns[nameof(detailsInstance.Price)].VisibleIndex = 5;
            gridView1.Columns[nameof(detailsInstance.Discount)].VisibleIndex = 6;
            gridView1.Columns[nameof(detailsInstance.DiscountValue)].VisibleIndex = 7;
            gridView1.Columns[nameof(detailsInstance.TotalPrice)].VisibleIndex = 8;
            gridView1.Columns[nameof(detailsInstance.TotalCostValue)].VisibleIndex = 9;
            gridView1.Columns[nameof(detailsInstance.TotalCostValue)].VisibleIndex = 10;
            gridView1.Columns[nameof(detailsInstance.StoreID)].VisibleIndex = 11;

            gridView1.Appearance.EvenRow.BackColor = Color.FromArgb(255, 249, 196);
            gridView1.OptionsView.EnableAppearanceEvenRow = true;

            gridView1.Appearance.OddRow.BackColor = Color.WhiteSmoke;
            gridView1.OptionsView.EnableAppearanceOddRow = true;

            RepositoryItemButtonEdit buttonEdit = new RepositoryItemButtonEdit();
            gridControl1.RepositoryItems.Add(buttonEdit);
            buttonEdit.Buttons.Clear();
            buttonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
            buttonEdit.ButtonClick += ButtonEdit_ButtonClick;
            GridColumn clmnDelete = new GridColumn()
            {
                Name = "clmnDelete",
                Caption = "",
                FieldName = "Delete",
                ColumnEdit = buttonEdit,
                VisibleIndex = 100,
                Width = 15
            };
            buttonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            gridView1.Columns.Add(clmnDelete);

            #region Events
            spn_DiscountValue.Enter += new EventHandler(this.Spn_DiscountValue_Enter);
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

            lkp_Branch.EditValueChanging += Lkp_Branch_EditValueChanging;
            gridView1.CustomRowCellEditForEditing += GridView1_CustomRowCellEditForEditing;
            gridView1.CellValueChanged += GridView1_CellValueChanged;
            gridView1.RowCountChanged += GridView1_RowCountChanged;
            gridView1.RowUpdated += GridView1_RowUpdated;

            gridView1.CustomUnboundColumnData += GridView1_CustomUnboundColumnData;

            gridControl1.ProcessGridKey += GridControl1_ProcessGridKey;

            gridView1.ValidateRow += GridView1_ValidateRow;
            gridView1.InvalidRowException += GridView1_InvalidRowException;

            this.Activated += Frm_Invoice_Activated;
            #endregion
        }

        private void ButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            GridView view = ((GridControl)((ButtonEdit)sender).Parent).MainView as GridView;
            if (view.FocusedRowHandle >= 0)
            {
                view.DeleteSelectedRows();
            }
        }

        private void Frm_Invoice_Activated(object sender, EventArgs e)
        {
            MoveFocusToGrid();
        }

        private void GridView1_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            if (e.Row == null || (e.Row as DAL.InvoiceDetail).ItemID == 0)
            {
                gridView1.DeleteRow(e.RowHandle);
            }
            e.ExceptionMode = ExceptionMode.Ignore;
        }

        private void GridView1_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            if (!(e.Row is DAL.InvoiceDetail row) || row.ItemID == 0)
            {
                e.Valid = false;
                return;
            }
        }

        private void GridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (!(sender is GridControl control)) return;

            if (!(control.FocusedView is GridView view)) return;

            if (view.FocusedColumn == null) return;

            if (e.KeyCode == Keys.Return)
            {
                string focusedColumn = view.FocusedColumn.FieldName;

                if (view.FocusedColumn.FieldName == "Code" || view.FocusedColumn.FieldName == "ItemID")
                {
                    GridControl1_ProcessGridKey(sender, new KeyEventArgs(Keys.Tab));
                }

                if (view.FocusedRowHandle < 0)
                {
                    view.AddNewRow();
                    view.FocusedColumn = view.Columns[focusedColumn];
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Tab)
            {
                view.FocusedColumn = view.VisibleColumns[view.FocusedColumn.VisibleIndex - 1];
                e.Handled = true;
            }
        }

        string enteredCode;
        private void GridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Code")
            {
                if (e.IsSetData)
                {
                    enteredCode = e.Value.ToString();
                }
                else if (e.IsGetData)
                {
                    e.Value = enteredCode;
                }
            }
            else if (e.Column.FieldName == "Index")
                e.Value = gridView1.GetVisibleRowHandle(e.ListSourceRowIndex) + 1;
        }

        private void GridView1_RowUpdated(object sender, RowObjectEventArgs e)
        {
            if (!(gridView1.DataSource is Collection<DAL.InvoiceDetail> items))
                spn_Total.EditValue = 0;

            else
                spn_Total.EditValue = items.Sum(x => x.TotalPrice);
        }
        int CurrentRowsCount = 0;
        private void GridView1_RowCountChanged(object sender, EventArgs e)
        {
            if (CurrentRowsCount < gridView1.RowCount)
            {
                var rows = (gridView1.DataSource as Collection<DAL.InvoiceDetail>);
                var lastRow = rows.Last();
                var row = rows.FirstOrDefault(x => x.ItemID == lastRow.ItemID && x.ItemUnitID == lastRow.ItemUnitID && x != lastRow);

                if (row != null)
                {
                    row.ItemQty += lastRow.ItemQty;

                    rows.Remove(lastRow);
                }
            }

            CurrentRowsCount = gridView1.RowCount;
            GridView1_RowUpdated(sender, null);
        }

        private void GridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (!(gridView1.GetRow(e.RowHandle) is DAL.InvoiceDetail row))
                return;
            Session.ProductViewClass itemV = null;
            Session.ProductViewClass.ProductUOMView unitV = null;
            if (e.Column.FieldName == "Code")
            {
                string ItemCode = e.Value.ToString();
                if (Session.GlobalSettings.ReadFormScaleBarcode &&
                    ItemCode.Length == Session.GlobalSettings.BarcodeLength &&
                    ItemCode.StartsWith(Session.GlobalSettings.ScaleBarcodePrefix))
                {
                    var itemCodeString = e.Value.ToString()
                        .Substring(Session.GlobalSettings.ScaleBarcodePrefix.Length,
                        Session.GlobalSettings.ProductCodeLength);
                    ItemCode = Convert.ToInt32(itemCodeString).ToString();
                    string Readvalue = e.Value.ToString().Substring(
                        Session.GlobalSettings.ScaleBarcodePrefix.Length +
                        Session.GlobalSettings.ProductCodeLength);
                    if (Session.GlobalSettings.IgnoreCheckDigit)
                        Readvalue = Readvalue.Remove(Readvalue.Length - 1, 1);
                    double value = Convert.ToDouble(Readvalue);
                    value /= (Math.Pow(10, Session.GlobalSettings.DivideValueBy));
                    if (Session.GlobalSettings.ReadMode == Session.GlobalSettings.ReadValueMode.Weight)
                        row.ItemQty = value;

                    else if (Session.GlobalSettings.ReadMode == Session.GlobalSettings.ReadValueMode.Price)
                    {
                        itemV = Session.ProductsView.FirstOrDefault(x => x.Units.Select(u => u.Barcode).Contains(ItemCode));
                        if (itemV != null)
                        {
                            unitV = itemV.Units.First(x => x.Barcode == ItemCode);

                            switch (type)
                            {
                                case InvoiceType.Purchase:
                                case InvoiceType.PurchaseReturn:
                                    row.ItemQty = value / unitV.BuyPrice;
                                    break;
                                case InvoiceType.Sales:
                                case InvoiceType.SalesReturn:
                                    row.ItemQty = value / unitV.SellPrice;

                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (itemV == null)
                    itemV = Session.ProductsView.FirstOrDefault(x => x.Units.Select(u => u.Barcode).Contains(ItemCode));

                if (itemV != null)
                {
                    row.ItemID = itemV.ID;
                    if (unitV == null)
                        unitV = itemV.Units.First(x => x.Barcode == ItemCode);

                    row.ItemUnitID = unitV.UnitID;

                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(detailsInstance.ItemID)], row.ItemID));

                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(detailsInstance.ItemUnitID)], row.ItemUnitID));

                    enteredCode = string.Empty;

                    return;
                }
                enteredCode = string.Empty;
            }

            itemV = Session.ProductsView.Single(x => x.ID == row.ItemID);

            if (row.ItemUnitID == 0)
            {
                row.ItemUnitID = itemV.Units.First().UnitID;
                GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(detailsInstance.ItemUnitID)], row.ItemUnitID));
            }

            unitV = itemV.Units.Single(x => x.UnitID == row.ItemUnitID);

            switch (e.Column.FieldName)
            {
                case nameof(detailsInstance.ItemID):
                    if (row.StoreID == 0 && lkp_Branch.IsEditValueValidAndNotZero())
                        row.StoreID = Convert.ToInt32(lkp_Branch.EditValue);
                    break;
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

        private void GridView1_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == nameof(detailsInstance.ItemUnitID))
            {
                RepositoryItemLookUpEdit repo = new RepositoryItemLookUpEdit
                {
                    NullText = ""
                };
                e.RepositoryItem = repo;
                if (!(gridView1.GetRow(e.RowHandle) is DAL.InvoiceDetail row))
                    return;
                var item = Session.ProductsView.SingleOrDefault(x => x.ID == row.ItemID);
                if (item == null)
                    return;
                repo.DataSource = item.Units;
                repo.ValueMember = "UnitID";
                repo.DisplayMember = "UnitName";
            }
            else if (e.Column.FieldName == nameof(detailsInstance.ItemID))
            {
                e.RepositoryItem = repoItems;
            }
        }

        #endregion

        private void Lkp_Branch_EditValueChanging(object sender, ChangingEventArgs e)
        {
            var items = gridView1.DataSource as Collection<DAL.InvoiceDetail>;
            if (e.OldValue is int && e.NewValue is int)
            {
                foreach (var row in items)
                {
                    if (row.StoreID == Convert.ToInt32(e.OldValue))
                        row.StoreID = Convert.ToInt32(e.NewValue);
                }
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


        #region SpenEditCalculations

        private void Spn_Net_MouseDoubleClick(object sender, EventArgs e)
        {
            spn_Paid.EditValue = spn_Net.EditValue;
        }

        private void Spn_Net_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (Convert.ToDouble(e.OldValue) == Convert.ToDouble(spn_Paid.EditValue))
                spn_Paid.EditValue = e.NewValue;
        }

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
        private void Spn_DiscountValue_Enter(object sender, EventArgs e)
        {
            IsDiscountValueFocused = true;
        }

        private void Spn_DiscountValue_Leave(object sender, EventArgs e)
        {
            IsDiscountValueFocused = false;
        }
        #endregion

        void MoveFocusToGrid()
        {
            gridView1.Focus();
            gridView1.FocusedColumn = gridView1.Columns["Code"];
            gridView1.AddNewRow();
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

            if (gridView1.RowCount == 0)
            {
                numberOfErrors++;
                XtraMessageBox.Show("برجاء ادخال صنف واحد علي الاقل", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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

        private string GetNewInvoiceCode()
        {
            string maxCode;
            using (var db = new DAL.dbDataContext())
            {
                maxCode = db.InvoiceHeaders.Where(x => x.InvoiceType == (int)type).Select(x => x.Code).Max();
            }

            return GetNextNumberInString(maxCode);
        }

        public override void New()
        {
            invoice = new DAL.InvoiceHeader()
            {
                Drawer = Session.Defaults.Drawer,
                Date = DateTime.Now,
                PostedToStore = true,
                PostDate = DateTime.Now,
                Code = GetNewInvoiceCode(),
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
            MoveFocusToGrid();
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

            invoice.InvoiceType = (byte)type;

            base.SetData();
        }

        public override void Save()
        {
            gridView1.UpdateCurrentRow();
            var db = new DAL.dbDataContext();
            if (invoice.ID == 0)
            {
                db.InvoiceHeaders.InsertOnSubmit(invoice);
            }
            else
            {
                db.InvoiceHeaders.Attach(invoice);
            }
            SetData();

            var items = (Collection<DAL.InvoiceDetail>)gridView1.DataSource;


            if (invoice.Expences > 0)
            {
                var totalPrice = items.Sum(x => x.TotalPrice);
                var totalQTY = items.Sum(x => x.ItemQty);
                var ByPriceUnit = invoice.Expences / totalPrice;// السعر لكل جنيه واحد  
                var ByQtyUnit = invoice.Expences / totalQTY;    //السعر لكل قطعه واحده من الوحده المختاره  
                XtraDialogArgs args = new XtraDialogArgs();
                UserControls.CostDistributionOption distributionOption = new UserControls.CostDistributionOption();
                args.Caption = "";
                args.Content = distributionOption;
                ((XtraBaseArgs)args).Buttons = new DialogResult[]
                {
                DialogResult.OK
                };
                args.Showing += Args_Showing;

                XtraDialog.Show(args);


                foreach (var row in items)
                {

                    if (distributionOption.SelectedOption == Master.CostDistributionOptions.ByPrice)
                        row.CostValue = (row.TotalPrice / row.ItemQty) + (ByPriceUnit * row.Price); // توزيع بالسعر
                    else
                        row.CostValue = (row.TotalPrice / row.ItemQty) + (ByQtyUnit); // توزيع بالكميه 

                    row.TotalCostValue = row.ItemQty * row.CostValue;
                }
            }
            else
            {
                foreach (var row in items)
                {
                    row.CostValue = row.TotalPrice / row.ItemQty;
                    row.TotalCostValue = row.TotalPrice;
                }

            }
            var msg = $"   {invoice.ID} لعميل {glkp_PartID.Text} فاتوره مبيعات رقم ";

            #region Journals


            // فاتوره بقيمه بضاعه 1000
            // ضريبه 120
            // مصروف نقل 100
            // خصم 50 

            //  1170  الصافي 


            //         حساب المخزون           - مدين - 1100
            //         حساب ضريبه مضافه       - مدين -  120   
            // حساب المورد            - دائن -         1220


            //          حساب المورد          - مدين - 50 
            //     حساب خصم نقدي مكتسب  - دائن -      50


            //        حساب المورد         -مدين - 1000  
            //   حساب الخذنه         -دائن -      1000 
            db.Journals.DeleteAllOnSubmit(db.Journals.Where(x => x.SourceType == (byte)type && x.SourceID == invoice.ID));
            db.SubmitChanges();
            var partAccountID = db.CustomersAndVendors.Single(x => x.ID == invoice.PartID).AccountID;
            var store = db.Stores.Single(x => x.ID == invoice.Branch);
            var drawer = db.Drawers.Single(x => x.ID == invoice.Drawer);
            db.Journals.InsertOnSubmit(new DAL.Journal() // Part
            {
                AccountID = partAccountID,
                Code = 54545,
                Credit = invoice.Total + invoice.TaxValue + invoice.Expences,
                Debit = 0,
                InsertDate = invoice.Date,
                Notes = msg,
                SourceID = invoice.ID,
                SourceType = (byte)type,
            });
            db.Journals.InsertOnSubmit(new DAL.Journal() // Store Inventory
            {
                AccountID = store.InventoryAccountID,
                Code = 54545,
                Credit = 0,
                Debit = invoice.Total + invoice.Expences,
                InsertDate = invoice.Date,
                Notes = msg,
                SourceID = invoice.ID,
                SourceType = (byte)type,
            });

            if (invoice.Tax > 0)
                db.Journals.InsertOnSubmit(new DAL.Journal() // 
                {
                    AccountID = Session.Defaults.PurchaseTax,
                    Code = 54545,
                    Credit = 0,
                    Debit = invoice.TaxValue,
                    InsertDate = invoice.Date,
                    Notes = msg + " - ضريبه مضافه ",
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                });
            //if (Invoice.Expences  > 0)
            //    db.Journals.InsertOnSubmit(new DAL.Journal() // 
            //    {
            //        AccountID = Session.Defualts.PurchaseExpences,
            //        Code = 54545,
            //        Credit = 0,
            //        Debit = Invoice.Expences,
            //        InsertDate = Invoice.Date,
            //        Notes = msg + " -   مصروفات شراء",
            //        SourceID = Invoice.ID,
            //        SourceType = (byte)Type,
            //    });

            if (invoice.DiscountVale > 0)
            {
                db.Journals.InsertOnSubmit(new DAL.Journal() // Store Tax
                {
                    AccountID = Session.Defaults.DiscountReceivedAccount,
                    Code = 54545,
                    Credit = invoice.DiscountVale,
                    Debit = 0,
                    InsertDate = invoice.Date,
                    Notes = msg + " -   خصم شراء",
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                });
                db.Journals.InsertOnSubmit(new DAL.Journal() // Store Tax
                {
                    AccountID = partAccountID,
                    Code = 54545,
                    Credit = 0,
                    Debit = invoice.DiscountVale,
                    InsertDate = invoice.Date,
                    Notes = msg + " -   خصم شراء",
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                });

            }


            if (invoice.Paid > 0)
            {
                db.Journals.InsertOnSubmit(new DAL.Journal() //
                {
                    AccountID = drawer.AccountID,
                    Code = 54545,
                    Credit = invoice.Paid,
                    Debit = 0,
                    InsertDate = invoice.Date,
                    Notes = msg + " - سداد",
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                });
                db.Journals.InsertOnSubmit(new DAL.Journal() //
                {
                    AccountID = partAccountID,
                    Code = 54545,
                    Credit = 0,
                    Debit = invoice.Paid,
                    InsertDate = invoice.Date,
                    Notes = msg + " - سداد",
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                });
            }
            #endregion


            foreach (var row in items)
                row.InvoiceID = invoice.ID;
            generalDB.SubmitChanges();
            db.StoreLogs.DeleteAllOnSubmit(db.StoreLogs.Where(x => x.SourceType == (byte)type && x.SourceID == invoice.ID));
            db.SubmitChanges();
            if (invoice.PostedToStore)
                foreach (var row in items)
                {
                    var unitView = Session.ProductsView.Single(x => x.ID == row.ItemID).Units.Single(x => x.UnitID == row.ItemUnitID);
                    db.StoreLogs.InsertOnSubmit(new DAL.StoreLog()
                    {
                        ProductID = row.ItemID,
                        InsertTime = invoice.PostDate.Value,
                        SourceID = invoice.ID,
                        SourceType = (byte)type,
                        Notes = msg,
                        IsInTransaction = true,
                        StoreID = row.StoreID,
                        Qty = row.ItemQty * unitView.Factor,
                        CostValue = row.CostValue / unitView.Factor

                    });
                }
            db.SubmitChanges();
            base.Save();
        }

        private void Args_Showing(object sender, XtraMessageShowingArgs e)
        {
            e.Form.ControlBox = false;
            e.Form.Height = 150;
            e.Buttons[DialogResult.OK].Text = "متابعة وحفظ";
        }
    }
}
