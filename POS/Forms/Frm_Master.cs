using DevExpress.XtraEditors;
using System.Linq;
using System.Windows.Forms;

namespace POS.Forms
{
    public partial class Frm_Master : XtraForm
    {
        public static string errorText
        {
            get { return "هذا الحقل مطلوب"; }

            set { }
        }

        public Frm_Master()
        {
            InitializeComponent();
        }

        public virtual void Save()
        {
            XtraMessageBox.Show("تم الحفظ بنجاح");
            RefreshData();
        }

        public virtual void New()
        {
            GetData();
        }

        public virtual void Delete()
        {

        }

        public virtual void GetData()
        {

        }

        public virtual void SetData()
        {

        }

        public virtual void RefreshData()
        {

        }

        public virtual bool IsDataValid()
        {
            return true;
        }

        private void btn_save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (IsDataValid())
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

        private void Frm_Master_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btn_save.PerformClick();
            }
            if (e.KeyCode == Keys.F2)
            {
                New();
            }
            if (e.KeyCode == Keys.F3)
            {
                Delete();
            }
        }
    }
}
