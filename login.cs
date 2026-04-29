using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace KlinikOtomasyon
{
    public partial class login : Form
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=ZEHRA;Initial Catalog=KullaniciOtomasyonDB;Integrated Security=True");
        public login()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                string sql = "SELECT * FROM Kullanicilar WHERE KullaniciAdi COLLATE Turkish_CS_AS = @user AND Sifre COLLATE Turkish_CS_AS = @pass";
                SqlCommand komut = new SqlCommand(sql, baglanti);
                komut.Parameters.AddWithValue("@user", txtKullanicii.Text.Trim());
                komut.Parameters.AddWithValue("@pass", txtSifree.Text.Trim());

                SqlDataReader oku = komut.ExecuteReader();

                if (oku.Read())
                {
                    MessageBox.Show("Giriş Başarılı! HOŞ GELDİN " + oku["ROL"].ToString());
                    anasayfa ana = new anasayfa();
                    ana.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Hatalı! Girilen: " + txtKullanici.Text.Trim() + " / " + txtSifre.Text.Trim());
                }
                oku.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("HATA OLUŞTU!: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }
    }
}