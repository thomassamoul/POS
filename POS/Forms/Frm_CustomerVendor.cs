using System;
using System.Data;
using System.Linq;

namespace POS.Forms
{
    public partial class Frm_CustomerVendor : Frm_Master
    {
        bool isCustomer;
        DAL.CustomersAndVendor cusVendor;

        public Frm_CustomerVendor(bool isCustomer)
        {
            this.isCustomer = isCustomer;
            InitializeComponent();
            New();
        }

        public Frm_CustomerVendor(int id)
        {
            InitializeComponent();
            LoadObject(id);
        }

        private void LoadObject(int id)
        {
            using (var db = new DAL.dbDataContext())
            {
                cusVendor = db.CustomersAndVendors.Single(s => s.ID == id);
                isCustomer = cusVendor.IsCustomuer;
                GetData();
            }
        }

        private void Frm_CustomerVendor_Load(object sender, EventArgs e)
        {
            this.Text = (isCustomer) ? "عميل" : "مورد";
        }

        public override void New()
        {
            cusVendor = new DAL.CustomersAndVendor();
            base.New();
        }

        public override void GetData()
        {
            txt_Name.Text = cusVendor.Name;
            txt_Mobile.Text = cusVendor.Mobile;
            txt_Phone.Text = cusVendor.Phone;
            txt_Address.Text = cusVendor.Address;
            txt_AccountID.Text = cusVendor.AccountID.ToString();
            base.GetData();
        }

        public override void SetData()
        {
            cusVendor.Name = txt_Name.Text;
            cusVendor.Mobile = txt_Mobile.Text;
            cusVendor.Phone = txt_Phone.Text;
            cusVendor.Address = txt_Address.Text;
            cusVendor.IsCustomuer = isCustomer;
            base.SetData();
        }

        bool IsDataValidated()
        {
            if (txt_Name.Text.Trim() == string.Empty)
            {
                txt_Name.ErrorText = "هذا الحقل مطلوب";
                return false;
            }
            var db = new DAL.dbDataContext();
            if (db.CustomersAndVendors.Where(x => x.Name.Trim() == txt_Name.Text.Trim() && x.IsCustomuer == isCustomer &&
            x.ID != cusVendor.ID).Count() > 0)
            {
                txt_Name.ErrorText = "هذا الاسم مسجل مسبقا";
                return false;
            }
            return true;
        }

        public override void Save()
        {
            if (IsDataValidated() == false)

                return;

            var db = new DAL.dbDataContext();
            DAL.Account account;

            if (cusVendor.ID == 0)
            {
                db.CustomersAndVendors.InsertOnSubmit(cusVendor);
                account = new DAL.Account();
                db.Accounts.InsertOnSubmit(account);

            }
            else
            {
                db.CustomersAndVendors.Attach(cusVendor);
                account = db.Accounts.Single(s => s.ID == cusVendor.AccountID);

            }

            SetData();
            account.Name = cusVendor.Name;
            db.SubmitChanges();
            cusVendor.AccountID = account.ID;
            db.SubmitChanges();

            base.Save();
        }
    }
}