using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Forms
{
    public partial class Frm_DrawerList : Frm_Master
    {
        public Frm_DrawerList()
        {
            InitializeComponent();
            RefreshData();
        }

        private void Frm_DrawerList_Load(object sender, EventArgs e)
        {
            btn_Delete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btn_save.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["Name"].Caption = "الاسم";
            gridView1.OptionsBehavior.Editable = false;
            gridView1.DoubleClick += GridView1_DoubleClick;
        }

        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                var frm = new Frm_Drawer(Convert.ToInt32(view.GetFocusedRowCellValue("ID")));
                frm.ShowDialog();
                RefreshData();
            }
        }

        public override void New()
        {
            var frm = new Frm_Drawer();
            frm.ShowDialog();
            RefreshData();
        }

        public override void RefreshData()
        {
            using (var db = new DAL.dbDataContext())
            {
                gridControl1.DataSource = db.Drawers.ToList();
            }
            base.RefreshData();
        }
    }
}
