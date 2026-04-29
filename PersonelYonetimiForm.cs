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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace KlinikOtomasyon
{
    public partial class PersonelYonetimiForm : Form
    {
        SqlBaglantisi bgl = new SqlBaglantisi();
        public PersonelYonetimiForm()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PersonelYonetimiForm_Load(object sender, EventArgs e)
        {

        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            // Sütun isimlerini senin SQL tablonla birebir eşitledim kanka:
            SqlCommand komut = new SqlCommand("insert into tbl_Personeller (PersonelAd, PersonelSoyad, PersonelUnvan, PersonelBrans, PersonelTC, PersonelTelefon, PersonelSifre) values (@p1, @p2, @p3, @p4, @p5, @p6, @p7)", bgl.baglanti());

            komut.Parameters.AddWithValue("@p1", txtAd.Text);
            komut.Parameters.AddWithValue("@p2", txtSoyad.Text);
            komut.Parameters.AddWithValue("@p3", cmbUnvan.Text);
            komut.Parameters.AddWithValue("@p4", cmbBrans.Text);
            komut.Parameters.AddWithValue("@p5", mskTC.Text);
            komut.Parameters.AddWithValue("@p6", mskTelefon.Text);
            komut.Parameters.AddWithValue("@p7", txtSifre.Text);

            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Personel başarıyla kaydedildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Eğer listele metodunu yazdıysan burayı aç:
            listele();
        }
        void listele()
        {
            DataTable dt = new DataTable();
            // Burayı da tbl_Personeller yapıyoruz
            SqlDataAdapter da = new SqlDataAdapter("Select * From tbl_Personeller", bgl.baglanti());
            da.Fill(dt);
            dgvPersonel.DataSource = dt;
        }






        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            // SQL'de PersonelAd içinde yazdığın harfler geçenleri anlık getirir
            SqlDataAdapter da = new SqlDataAdapter("Select * From tbl_Personeller where PersonelAd Like '%" + txtAra.Text + "%'", bgl.baglanti());
            da.Fill(dt);
            dgvPersonel.DataSource = dt;
        }




        private void dgvPersonel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Tıklanan satırın numarasını alıyoruz
            int secilen = dgvPersonel.SelectedCells[0].RowIndex;

            // Hücrelerdeki verileri ilgili kutulara dağıtıyoruz
            // Sıralama SQL'deki sütun sırasına göre gider (0, 1, 2...)
            txtId.Text = dgvPersonel.Rows[secilen].Cells[0].Value.ToString();
            txtAd.Text = dgvPersonel.Rows[secilen].Cells[1].Value.ToString();
            txtSoyad.Text = dgvPersonel.Rows[secilen].Cells[2].Value.ToString();
            cmbUnvan.Text = dgvPersonel.Rows[secilen].Cells[3].Value.ToString();
            cmbBrans.Text = dgvPersonel.Rows[secilen].Cells[4].Value.ToString();
            mskTC.Text = dgvPersonel.Rows[secilen].Cells[5].Value.ToString();
            mskTelefon.Text = dgvPersonel.Rows[secilen].Cells[6].Value.ToString();
            txtSifre.Text = dgvPersonel.Rows[secilen].Cells[7].Value.ToString();
        }



        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            SqlCommand komutGuncelle = new SqlCommand("Update tbl_Personeller set PersonelAd=@p1, PersonelSoyad=@p2, PersonelUnvan=@p3, PersonelBrans=@p4, PersonelTC=@p5, PersonelTelefon=@p6, PersonelSifre=@p7 where Personelid=@p8", bgl.baglanti());

            komutGuncelle.Parameters.AddWithValue("@p1", txtAd.Text);
            komutGuncelle.Parameters.AddWithValue("@p2", txtSoyad.Text);
            komutGuncelle.Parameters.AddWithValue("@p3", cmbUnvan.Text);
            komutGuncelle.Parameters.AddWithValue("@p4", cmbBrans.Text);
            komutGuncelle.Parameters.AddWithValue("@p5", mskTC.Text);
            komutGuncelle.Parameters.AddWithValue("@p6", mskTelefon.Text);
            komutGuncelle.Parameters.AddWithValue("@p7", txtSifre.Text);
            komutGuncelle.Parameters.AddWithValue("@p8", txtId.Text); // Hangi ID'ye sahip personel değişecekse o

            komutGuncelle.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Personel bilgileri güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            listele(); // Tabloyu anlık yenile
        }



        private void btnSil_Click(object sender, EventArgs e)
        {
            SqlCommand komutSil = new SqlCommand("Delete From tbl_Personeller where Personelid=@p1", bgl.baglanti());
            komutSil.Parameters.AddWithValue("@p1", txtId.Text);
            komutSil.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Personel kaydı silindi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            listele(); // Tabloyu tazele
        }



        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtId.Text = "";
            txtAd.Text = "";
            txtSoyad.Text = "";
            cmbUnvan.SelectedIndex = -1; // ComboBox'ı ilk haline (boş) döndürür
            cmbBrans.SelectedIndex = -1;
            mskTC.Text = "";
            mskTelefon.Text = "";
            txtSifre.Text = "";

            // İmleci direkt Ad kutusuna odaklar, yeni kayıt için hazır bekler
            txtAd.Focus();
        }

        private void txtAra_TextChanged_1(object sender, EventArgs e)
        {
           
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From tbl_Personeller where PersonelAd Like '%" + txtAra.Text + "%'", bgl.baglanti());
            da.Fill(dt);
            dgvPersonel.DataSource = dt;
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "PDF Dosyası|*.pdf";
                save.FileName = "Personel_Listesi_Raporu.pdf";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    // 1. Sayfa Yapısını Kur (A4 ve Yatay yapmak istersen PageSize.A4.Rotate() diyebilirsin)
                    Document pdfDosya = new Document(PageSize.A4);
                    PdfWriter.GetInstance(pdfDosya, new FileStream(save.FileName, FileMode.Create));

                    pdfDosya.Open();

                    // 2. Başlık (Senin Figma tasarımın gibi şık olsun)
                    Paragraph baslik = new Paragraph("OZCELIK OZEL KLINIK - PERSONEL DOKUMU\n\n");
                    baslik.Alignment = Element.ALIGN_CENTER;
                    pdfDosya.Add(baslik);

                    // 3. Tabloyu Oluştur (DataGridView'daki sütun sayısına göre)
                    PdfPTable tablo = new PdfPTable(dgvPersonel.Columns.Count);
                    tablo.WidthPercentage = 100; // Sayfaya tam yayılsın

                    // Tablo Başlıklarını Ekle
                    for (int i = 0; i < dgvPersonel.Columns.Count; i++)
                    {
                        tablo.AddCell(new Phrase(dgvPersonel.Columns[i].HeaderText));
                    }

                    // Verileri Satır Satır Ekle
                    for (int i = 0; i < dgvPersonel.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgvPersonel.Columns.Count; j++)
                        {
                            if (dgvPersonel.Rows[i].Cells[j].Value != null)
                            {
                                tablo.AddCell(new Phrase(dgvPersonel.Rows[i].Cells[j].Value.ToString()));
                            }
                        }
                    }

                    pdfDosya.Add(tablo);
                    pdfDosya.Close();

                    MessageBox.Show("Rapor başarıyla oluşturuldu kanka!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }
    }
    }




      






