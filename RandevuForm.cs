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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


namespace KlinikOtomasyon
{
    public partial class RandevuForm : Form
    {
        SqlBaglantisi bgl = new SqlBaglantisi();

        public RandevuForm()
        {
            InitializeComponent();
        }

        private void RandevuForm_Load(object sender, EventArgs e)
        {
            // 1. Branşları Çekme
            SqlCommand komutBrans = new SqlCommand("Select BransAd From tbl_Branslar", bgl.baglanti());
            SqlDataReader dr = komutBrans.ExecuteReader();
            while (dr.Read())
            {
                cmbBrans.Items.Add(dr[0]);
            }
            bgl.baglanti().Close();

            // 2. Mevcut Randevuları Listeleme
            randevuListele();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Sorguyu daha güvenli hale getirelim
                SqlCommand komut = new SqlCommand("insert into Tbl_Randevular (RandevuTarih, RandevuSaat, RandevuBrans, RandevuDoktor, HastaTC) values (@p1,@p2,@p3,@p4,@p5)", bgl.baglanti());

                komut.Parameters.AddWithValue("@p1", mskTarih.Text);
                komut.Parameters.AddWithValue("@p2", mskSaat.Text);
                komut.Parameters.AddWithValue("@p3", cmbBrans.Text);
                komut.Parameters.AddWithValue("@p4", cmbDoktor.Text);
                komut.Parameters.AddWithValue("@p5", mskTc.Text);

                // 2. İşlemi yürüt
                int sonuc = komut.ExecuteNonQuery();
                bgl.baglanti().Close();

                // 3. Kontrol: Gerçekten bir satır eklendi mi?
                if (sonuc > 0)
                {
                    MessageBox.Show("Randevu başarıyla oluşturuldu!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    randevuListele(); // Listeyi anında güncellemek için bunu mutlaka çağır!
                }
                else
                {
                    MessageBox.Show("Kayıt yapılamadı, veritabanı satırı kabul etmedi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    komut.ExecuteNonQuery();
                    bgl.baglanti().Close();
                    MessageBox.Show("Randevu başarıyla oluşturuldu!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // BURASI ÇOK KRİTİK: Tabloyu tazelemek için bu metodu burada tekrar çağırıyoruz
                    randevuListele();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (bgl.baglanti().State != ConnectionState.Closed) bgl.baglanti().Close();
            }
        }
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCommand komut = new SqlCommand("Update Tbl_Randevular set RandevuTarih=@p1, RandevuSaat=@p2, RandevuBrans=@p3, RandevuDoktor=@p4, HastaTC=@p5 where RandevuID=@p6", bgl.baglanti());

                komut.Parameters.AddWithValue("@p1", mskTarih.Text);
                komut.Parameters.AddWithValue("@p2", mskSaat.Text);
                komut.Parameters.AddWithValue("@p3", cmbBrans.Text);
                komut.Parameters.AddWithValue("@p4", cmbDoktor.Text);
                komut.Parameters.AddWithValue("@p5", mskTc.Text);
                komut.Parameters.AddWithValue("@p6", txtıd.Text); // Hangi kaydın güncelleneceğini ID belirler

                komut.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Randevu başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                randevuListele(); // Listeyi anında tazele
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                bgl.baglanti().Close();
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            // Sadece kutuları boşaltıyoruz, mesaj kutusu yok!
            txtıd.Clear();
            mskTarih.Clear();
            mskSaat.Clear();
            mskTc.Clear();

            cmbBrans.Text = "";
            cmbDoktor.Text = "";

            // Odaklanmayı başa al
            mskTarih.Focus();
        }



        private void cmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbDoktor.Items.Clear();

            // Sorguda 'TRIM' kullanarak veritabanındaki boşlukları siliyoruz
            // Parametrede de '.Trim()' kullanarak ComboBox'taki boşlukları siliyoruz
            SqlCommand komut = new SqlCommand("Select DoktorAd, DoktorSoyad From tbl_Doktorlar where LTRIM(RTRIM(DoktorBrans)) = @p1", bgl.baglanti());

            komut.Parameters.AddWithValue("@p1", cmbBrans.Text.Trim());

            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                cmbDoktor.Items.Add(dr[0].ToString() + " " + dr[1].ToString());
            }
            bgl.baglanti().Close();
        }
        void randevuListele()
        {
            DataTable dt = new DataTable();
            // En son eklenen randevuyu en üstte görmek için 'desc' ekledik
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Randevular order by RandevuID desc", bgl.baglanti());
            da.Fill(dt);
            dataGridView2.DataSource = dt;
        }


        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Tıklanan satırdaki verileri kutucuklara aktarıyoruz
            int secilen = dataGridView2.SelectedCells[0].RowIndex;

            txtıd.Text = dataGridView2.Rows[secilen].Cells[0].Value.ToString();
            mskTarih.Text = dataGridView2.Rows[secilen].Cells[1].Value.ToString();
            mskSaat.Text = dataGridView2.Rows[secilen].Cells[2].Value.ToString();
            cmbBrans.Text = dataGridView2.Rows[secilen].Cells[3].Value.ToString();
            cmbDoktor.Text = dataGridView2.Rows[secilen].Cells[4].Value.ToString();
            mskTc.Text = dataGridView2.Rows[secilen].Cells[5].Value.ToString();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Tıklanan satırdaki verileri kutucuklara aktarıyoruz
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            txtıd.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            mskTarih.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            mskSaat.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            cmbBrans.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();
            cmbDoktor.Text = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
            mskTc.Text = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
        }

       
            private void btnSil_Click(object sender, EventArgs e)
        {
            // Önce kullanıcıya gerçekten silmek isteyip istemediğini soralım (Profesyonel dokunuş)
            DialogResult onay = MessageBox.Show("Bu randevuyu silmek istediğinize emin misiniz?", "Randevu Silme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (onay == DialogResult.Yes)
            {
                try
                {
                    SqlCommand komut = new SqlCommand("Delete From Tbl_Randevular where RandevuID=@p1", bgl.baglanti());

                    // txtıd kutusundaki ID'ye göre siliyoruz
                    komut.Parameters.AddWithValue("@p1", txtıd.Text);

                    komut.ExecuteNonQuery();
                    bgl.baglanti().Close();

                    MessageBox.Show("Randevu başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // Listeyi tazele ve kutuları boşalt
                    randevuListele();
                    btnTemizle_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                    bgl.baglanti().Close();
                }
            }
        }

        private void btnRandevuPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "PDF Dosyası|*.pdf";
                save.FileName = "Gunluk_Randevu_Listesi.pdf";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    Document pdfDosya = new Document(PageSize.A4.Rotate()); // Randevu tablosu geniştir, sayfayı YATAY yaptık kanka
                    PdfWriter.GetInstance(pdfDosya, new FileStream(save.FileName, FileMode.Create));

                    pdfDosya.Open();

                    // Başlık
                    Paragraph baslik = new Paragraph("OZCELIK OZEL KLINIK - GUNLUK RANDEVU DOKUMU\n\n");
                    baslik.Alignment = Element.ALIGN_CENTER;
                    pdfDosya.Add(baslik);

                    // Tabloyu oluştur
                    PdfPTable tablo = new PdfPTable(dataGridView2.Columns.Count);
                    tablo.WidthPercentage = 100;

                    // Başlıkları ekle
                    for (int i = 0; i < dataGridView2.Columns.Count; i++)
                    {
                        tablo.AddCell(new Phrase(dataGridView2.Columns[i].HeaderText));
                    }

                    // Randevuları ekle
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView2.Columns.Count; j++)
                        {
                            if (dataGridView2.Rows[i].Cells[j].Value != null)
                            {
                                tablo.AddCell(new Phrase(dataGridView2.Rows[i].Cells[j].Value.ToString()));
                            }
                        }
                    }

                    pdfDosya.Add(tablo);
                    pdfDosya.Close();

                    MessageBox.Show("Randevu raporu hazır kanka!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
    }
    






