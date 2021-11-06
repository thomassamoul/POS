using DevExpress.XtraEditors;
using System;
using System.Linq;

namespace POS.Forms
{
    public partial class Frm_StoresList : XtraForm
    {
        public Frm_StoresList()
        {
            InitializeComponent();
        }

        private void Frm_StoresList_Load(object sender, EventArgs e)
        {
            RefreshData();

            gridView1.OptionsBehavior.Editable = false;
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["Name"].Caption = "الاسم";
            gridView1.DoubleClick += GridView1_DoubleClick;

        }

        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            int SoteID = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
            Frm_Stores frm = new Frm_Stores(SoteID);
            frm.ShowDialog();
            RefreshData();
        }
        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshData();
        }

        void RefreshData()
        {
            var db = new DAL.dbDataContext();
            gridControl1.DataSource = db.Stores.Select(x => new { x.ID, x.Name });

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Frm_Stores frm = new Frm_Stores();
            frm.ShowDialog();
            RefreshData();
        }

    }
}
