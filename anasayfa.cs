using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlinikOtomasyon;

namespace KlinikOtomasyon
{
    public partial class anasayfa : Form
    {
        public anasayfa()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DoktorPanelForm frm = new DoktorPanelForm();
            frm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HastaKayitForm frm = new HastaKayitForm();
            frm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RandevuForm fr = new RandevuForm();
            fr.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PersonelYonetimiForm fr = new PersonelYonetimiForm();
            fr.Show();
        }
      
        }
    }
    
    

