using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using POS.Class;
using System;
using System.Data;
using System.Linq;

namespace POS.Forms
{
    public partial class Frm_CustomerVendorList : Frm_Master
    {
        bool isCustomer;
        private DAL.CustomersAndVendor cusVendor;

        public Frm_CustomerVendorList(bool isCustomer)
        {
            InitializeComponent();
            this.isCustomer = isCustomer;
            RefreshData();
            Text = this.isCustomer ? "عميل" : "مورد";
        }

        private void Frm_CustomerVendorList_Load(object sender, EventArgs e)
        {
            var ins = new DAL.CustomersAndVendor();
            gridView1.Columns[nameof(ins.ID)].Visible = false;
            gridView1.Columns[nameof(ins.AccountID)].Visible = false;
            gridView1.Columns[nameof(ins.IsCustomuer)].Visible = false;
            gridView1.Columns[nameof(ins.Name)].Caption = "الاسم";
            gridView1.Columns[nameof(ins.Phone)].Caption = "الهاتف";
            gridView1.Columns[nameof(ins.Mobile)].Caption = "الموبيل";
            gridView1.Columns[nameof(ins.Address)].Caption = "العنوان";

            gridView1.OptionsBehavior.Editable = false;
            gridView1.DoubleClick += GridView1_DoubleClick;
            if (isCustomer)
                Session.Customers.ListChanged += Vendors_ListChanged;
            else
                Session.Vendors.ListChanged += Vendors_ListChanged;

        }

        private void Vendors_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            RefreshData();
        }

        public override void New()
        {
            var frm = new Frm_CustomerVendor(isCustomer);

            base.New();
        }

        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                var frm = new Frm_CustomerVendor(Convert.ToInt32(view.GetFocusedRowCellValue("ID")));
                frm.Show();
            }
        }

        public override void RefreshData()
        {
            using (var db = new DAL.dbDataContext())
            {
                gridControl1.DataSource = (isCustomer) ? Session.Customers.ToList() : Session.Vendors.ToList();
            }
            base.RefreshData();
        }
    }
}
