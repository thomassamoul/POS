using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace POS.Forms
{
    public partial class Frm_CompanyInfo : XtraForm
    {
        public Frm_CompanyInfo()
        {
            InitializeComponent();
            this.Load += Frm_CompanyInfo_Load;
        }

        private void Frm_CompanyInfo_Load(object sender, EventArgs e)
        {
            GetData();
        }

        void GetData()
        {
            DAL.dbDataContext db = new DAL.dbDataContext();
            DAL.CompanyInfo info = db.CompanyInfos.FirstOrDefault();
            if (info == null)
                return;
            txt_name.Text = info.CompanyName;
            txt_address.Text = info.Address;
            txt_phone.Text = info.Phone;
            txt_mobile.Text = info.Mobile;
        }

        private void btn_save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        private void Frm_CompanyInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                Save();
            }
        }

        void Save()
        {
            if (txt_name.Text.Trim() == String.Empty)
            {
                txt_name.ErrorText = "برجاء ادخال اسم الشركة";
                return;
            }


            DAL.dbDataContext db = new DAL.dbDataContext();
            DAL.CompanyInfo info = db.CompanyInfos.FirstOrDefault();
            if (info == null)
                info = new DAL.CompanyInfo();
            info.CompanyName = txt_name.Text;
            info.Mobile = txt_mobile.Text;
            info.Phone = txt_phone.Text;
            info.Address = txt_address.Text;
            db.SubmitChanges();
            XtraMessageBox.Show("تم الحفظ بنجاح");
        }
    }

}