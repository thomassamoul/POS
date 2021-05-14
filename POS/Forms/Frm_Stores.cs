using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace POS.Forms
{
    public partial class Frm_Stores : XtraForm
    {
        DAL.Store store;
        public Frm_Stores()
        {
            InitializeComponent();
            New();
        }

        public Frm_Stores(int id)
        {
            InitializeComponent();
            var db = new DAL.dbDataContext();
            store = db.Stores.Where(s => s.ID == id).First();
            GetData();
        }

        void Save()
        {
            if (textEdit1.Text.Trim() == String.Empty)
            {
                textEdit1.ErrorText = "برجاء ادخال اسم الفرع";
                return;
            }
            var db = new DAL.dbDataContext();

            if (store.ID == 0)
                db.Stores.InsertOnSubmit(store);
            else
                db.Stores.Attach(store);

            SetData();

            db.SubmitChanges();
            XtraMessageBox.Show("تم الحفظ بنجاح");

        }

        void GetData()
        {
            textEdit1.Text = store.Name;
        }

        void SetData()
        {
            store.Name = textEdit1.Text;
        }

        void New()
        {
            store = new DAL.Store();
            GetData();
        }

        private void btn_save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            New();
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Delete();
        }

        private void Delete()
        {
            var db = new DAL.dbDataContext();
            if (XtraMessageBox.Show(text: "هل تريد الحذف", caption: "تاكيد الحذف", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                db.Stores.Attach(store);
                db.Stores.DeleteOnSubmit(store);
                db.SubmitChanges();
                XtraMessageBox.Show("تم الحذف بنجاح");
                New();
            }
        }
    }
}
