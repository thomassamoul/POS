using System;
using System.Linq;

namespace POS.Forms
{
    public partial class Frm_Drawer : Frm_Master
    {
        DAL.Drawer drawer;

        public Frm_Drawer()
        {
            InitializeComponent();
            New();
        }

        public Frm_Drawer(int id)
        {
            InitializeComponent();
            LoadDrawer(id);
        }
        void LoadDrawer(int id)
        {
            using (var db = new DAL.dbDataContext())
            {
                drawer = db.Drawers.Single(s => s.ID == id);
                GetData();
            }
        }

        public override void New()
        {
            drawer = new DAL.Drawer();
            base.New();
        }

        public override void Save()
        {
            if (txt_Name.Text.Trim() == String.Empty)
            {
                txt_Name.ErrorText = "برجاء ادخال اسم الخزنة";
                return;
            }

            var db = new DAL.dbDataContext();

            DAL.Account account;
            if (drawer.ID == 0)
            {
                account = new DAL.Account();
                db.Drawers.InsertOnSubmit(drawer);
                db.Accounts.InsertOnSubmit(account);
            }
            else
            {
                db.Drawers.Attach(drawer);
                account = db.Accounts.Single(s => s.ID == drawer.AccountID);
            }

            SetData();
            account.Name = drawer.Name;
            db.SubmitChanges();
            drawer.AccountID = account.ID;
            db.SubmitChanges();

            base.Save();
        }

        public override void SetData()
        {
            drawer.Name = txt_Name.Text;
            base.SetData();
        }
        public override void GetData()
        {
            txt_Name.Text = drawer.Name;
            base.GetData();
        }



    }
}
