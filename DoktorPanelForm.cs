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
    public partial class DoktorPanelForm : Form
    {
        SqlBaglantisi bgl = new SqlBaglantisi();

        public DoktorPanelForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void DoktorPanelForm_Load(object sender, EventArgs e)
        {
            SqlCommand komut2 = new SqlCommand("Select BransAd From Tbl_Branslar", bgl.baglanti());
            SqlDataReader dr2 = komut2.ExecuteReader();
            while (dr2.Read())
            {
                cmbBrans.Items.Add(dr2[0]);
            }
            bgl.baglanti().Close();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Doktorlar", bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;


            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DataSource = dt;


            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[1].Width = 80;
            dataGridView1.Columns[2].Width = 80;
            dataGridView1.Columns[3].Width = 100;
            dataGridView1.Columns[4].Width = 90;
            dataGridView1.Columns[5].Width = 70;


            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8);
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("insert into Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) values (@p1,@p2,@p3,@p4,@p5)", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", txtAd.Text);
            komut.Parameters.AddWithValue("@p2", txtSoyad.Text);
            komut.Parameters.AddWithValue("@p3", cmbBrans.Text);
            komut.Parameters.AddWithValue("@p4", mskTc.Text);
            komut.Parameters.AddWithValue("@p5", txtSifre.Text);


            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Doktor başarıyla eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);


            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Doktorlar", bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            txtAd.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            txtSoyad.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            cmbBrans.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();
            mskTc.Text = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
            txtSifre.Text = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        void doktorListele()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Doktorlar", bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            bgl.baglanti().Close();
        }
        private void btnSil_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("Delete from Tbl_Doktorlar where DoktorTC=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", mskTc.Text);
            int sonuc = komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            if (sonuc > 0)
            {
                MessageBox.Show("Veritabanından da silindi!");
            }
            else
            {
                MessageBox.Show("Uyarı: Kod çalıştı ama veritabanında eşleşen kayıt bulunamadı!");
            }
            bgl.baglanti().Close();

            MessageBox.Show("Doktor başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            doktorListele();


        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("Update Tbl_Doktorlar set DoktorAd=@p1, DoktorSoyad=@p2, DoktorBrans=@p3, DoktorSifre=@p5 where DoktorTC=@p4", bgl.baglanti());

            komut.Parameters.AddWithValue("@p1", txtAd.Text);
            komut.Parameters.AddWithValue("@p2", txtSoyad.Text);
            komut.Parameters.AddWithValue("@p3", cmbBrans.Text);
            komut.Parameters.AddWithValue("@p4", mskTc.Text);
            komut.Parameters.AddWithValue("@p5", txtSifre.Text);

            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Doktor bilgileri güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            doktorListele();
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
        }
            private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
            Color cizgiRengi = Color.DodgerBlue; 
            int kalinlik = 5; 

            
            ControlPaint.DrawBorder(e.Graphics, groupBox1.ClientRectangle,
                cizgiRengi, kalinlik, ButtonBorderStyle.Solid,  
                cizgiRengi, kalinlik, ButtonBorderStyle.Solid,  
                cizgiRengi, kalinlik, ButtonBorderStyle.Solid,  
                cizgiRengi, kalinlik, ButtonBorderStyle.Solid); 
        }

        private void btnDoktorPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "PDF Dosyası|*.pdf";
                save.FileName = "Klinik_Doktor_Listesi.pdf";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    Document pdfDosya = new Document(PageSize.A4);
                    PdfWriter.GetInstance(pdfDosya, new FileStream(save.FileName, FileMode.Create));

                    pdfDosya.Open();

                    
                    Paragraph baslik = new Paragraph("BULUT OZEL KLINIK - DOKTOR KADROMUZ\n\n");
                    baslik.Alignment = Element.ALIGN_CENTER;
                    pdfDosya.Add(baslik);

                    
                    PdfPTable tablo = new PdfPTable(dataGridView1.Columns.Count);
                    tablo.WidthPercentage = 100;

                    
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        tablo.AddCell(new Phrase(dataGridView1.Columns[i].HeaderText));
                    }

                    
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            if (dataGridView1.Rows[i].Cells[j].Value != null)
                            {
                                tablo.AddCell(new Phrase(dataGridView1.Rows[i].Cells[j].Value.ToString()));
                            }
                        }
                    }

                    pdfDosya.Add(tablo);
                    pdfDosya.Close();

                    MessageBox.Show("Doktor listesi PDF olarak hazır!", "Başarılı");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}


