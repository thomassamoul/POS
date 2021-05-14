using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace POS.Forms
{
    public partial class Frm_Main : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        public static Frm_Main _instance;

        public static Frm_Main Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_Main();

                return _instance;
            }
        }

        public Frm_Main()
        {
            InitializeComponent();
            accordionControl1.ElementClick += AccordionControl1_ElementClick;
        }

        private void AccordionControl1_ElementClick(object sender, DevExpress.XtraBars.Navigation.ElementClickEventArgs e)
        {
            var tag = e.Element.Tag as string;
            if (tag != string.Empty)
            {
                OpenFormByName(tag);
            }
        }

        public static void OpenFormByName(string name)
        {
            Form frm = null;

            switch (name)
            {
                case "Frm_Vendor":
                    frm = new Frm_CustomerVendor(false);
                    break;
                case "Frm_Customer":
                    frm = new Frm_CustomerVendor(true);
                    break;
                case "Frm_VendorList":
                    frm = new Frm_CustomerVendorList(false);
                    break;
                case "Frm_CustomerList":
                    frm = new Frm_CustomerVendorList(true);
                    break;
                case "Frm_PurchaseInvoice":
                    frm = new Frm_Invoice(Class.Master.InvoiceType.Purchase);
                    break;
                
                default:
                    var ins = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.Name == name);
                    if (ins != null)
                    {
                        frm = Activator.CreateInstance(ins) as Form;
                        if (Application.OpenForms[frm.Name] != null)
                        {
                            frm = Application.OpenForms[frm.Name];
                        }
                        else
                        {
                               frm.Show();
                        }
                        frm.BringToFront();
                    }
                    break;
            }

            if (frm != null)
            {
                if (Application.OpenForms[frm.Name] != null)
                {
                    frm = Application.OpenForms[frm.Name];
                }
                else
                {
                    frm.Show();
                }
                frm.BringToFront();
            }
        }
    }
}
