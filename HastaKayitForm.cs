using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KlinikOtomasyon
{
    public partial class  HastaKayitForm: Form
    {
        SqlConnection baglanti = new SqlConnection("Data Source=ZEHRA;Initial Catalog=KullaniciOtomasyonDB;Integrated Security=True");
        void listele()
        {
            try
            {
                if (baglanti.State == ConnectionState.Open) baglanti.Close();
                baglanti.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Hastalar", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // Sütunları içeriğe göre otomatik boyutlandır
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                // Satır seçmeyi kolaylaştırmak için
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                baglanti.Close();
            }
            catch (Exception ex) { MessageBox.Show("Listeleme hatası: " + ex.Message); }
        }

        public HastaKayitForm()
        {
            InitializeComponent(); 
            listele(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listele();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                
                string sorgu = "INSERT INTO Hastalar (TCNo, Ad, Soyad, Telefon, Sikayet, DogumTarihi, Cinsiyet, KanGrubu) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";

                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@p1", txtTCNo.Text);
                komut.Parameters.AddWithValue("@p2", txtAd.Text);
                komut.Parameters.AddWithValue("@p3", txtSoyad.Text);
                komut.Parameters.AddWithValue("@p4", txtTelefon.Text);
                komut.Parameters.AddWithValue("@p5", txtSikayet.Text); 
                komut.Parameters.AddWithValue("@p6", dateTimePicker1.Value);
                komut.Parameters.AddWithValue("@p7", comboBox2.Text);
                komut.Parameters.AddWithValue("@p8", comboBox1.Text); 

                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Hasta başarıyla kaydedildi!", "Bilgi");
                listele(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                if (baglanti.State == ConnectionState.Open) baglanti.Close();
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtAd_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtTCNo.Text = dataGridView1.CurrentRow.Cells["TCNo"].Value.ToString();
            txtAd.Text = dataGridView1.CurrentRow.Cells["Ad"].Value.ToString();
            txtSoyad.Text = dataGridView1.CurrentRow.Cells["Soyad"].Value.ToString();
            
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                DialogResult onay = MessageBox.Show("Bu kaydı silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (onay == DialogResult.Yes)
                {
                    try
                    {
                        baglanti.Open();
                        string seciliID = dataGridView1.CurrentRow.Cells[0].Value.ToString();

                        
                        SqlCommand komut = new SqlCommand("DELETE FROM Hastalar WHERE HastaID=@p1", baglanti);
                        komut.Parameters.AddWithValue("@p1", seciliID);

                        komut.ExecuteNonQuery();
                        baglanti.Close();

                        MessageBox.Show("Kayıt başarıyla silindi.", "Bilgi");
                        listele(); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Silme hatası: " + ex.Message);
                        if (baglanti.State == ConnectionState.Open) baglanti.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz satırı tablodan seçin!");
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                // SORGUDÜZENİ: Ad(@p1), Soyad(@p2), Tel(@p3), Sikayet(@p4), Dogum(@p5), Cinsiyet(@p6), Kan(@p7) --- WHERE TCNo(@p8)
                string sorgu = "UPDATE Hastalar SET Ad=@p1, Soyad=@p2, Telefon=@p3, Sikayet=@p4, DogumTarihi=@p5, Cinsiyet=@p6, KanGrubu=@p7 WHERE TCNo=@p8";

                SqlCommand komut = new SqlCommand(sorgu, baglanti);

                // PARAMETRELER (Sorgudaki sırayla aynı olmalı!)
                komut.Parameters.AddWithValue("@p1", txtAd.Text);
                komut.Parameters.AddWithValue("@p2", txtSoyad.Text);
                komut.Parameters.AddWithValue("@p3", txtTelefon.Text);
                komut.Parameters.AddWithValue("@p4", txtSikayet.Text);
                komut.Parameters.AddWithValue("@p5", dateTimePicker1.Value);
                komut.Parameters.AddWithValue("@p6", comboBox2.Text); // Senin kodda cmbCinsiyet yazıyordu, ama yukarıda comboBox2 kullanmışsın!
                komut.Parameters.AddWithValue("@p7", comboBox1.Text); // Senin kodda cmbKanGrubu yazıyordu, ama yukarıda comboBox1 kullanmışsın!
                komut.Parameters.AddWithValue("@p8", txtTCNo.Text);

                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Hasta bilgileri başarıyla güncellendi!", "Bilgi");
                listele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message);
                if (baglanti.State == ConnectionState.Open) baglanti.Close();
            }
        
    }
        } 

    } 







