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
using System.Collections;
namespace Izlozba
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection konekcija;
        SqlCommand komanda;
        SqlCommand komanda1;
        DataTable dt1, dt2, dt3, dt4;


        SqlDataAdapter da;
        void Konekcija()
        {
            konekcija = new SqlConnection();
            konekcija.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Izlozba;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            komanda = new SqlCommand();
            komanda1 = new SqlCommand();
            komanda.Connection = konekcija;
            komanda1.Connection = konekcija;
            dt1 = new DataTable();
            dt2 = new DataTable();
            dt3 = new DataTable();
            dt4 = new DataTable();
            da = new SqlDataAdapter();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Konekcija();
            ArrayList lista = new ArrayList();
            komanda.CommandText = "SELECT Pas.Pas_ID,Pas.Ime FROM Pas ";
            da.SelectCommand = komanda;
            da.Fill(dt1);
            int i;
            //comboBox1
            for (i = 0; i < dt1.Rows.Count; i++)
            {
                lista.Add(Convert.ToInt32(dt1.Rows[i][0]));
                lista.Add(Convert.ToString(dt1.Rows[i][1]));
                string kombo1 = dt1.Rows[i][0].ToString() + " - " + dt1.Rows[i][1].ToString();
                comboBox1.Items.Add(kombo1);
            }
            komanda.CommandText = "SELECT Izlozba.id_izlozbe,Izlozba.Mesto,Izlozba.Datum FROM Izlozba ";
            da.SelectCommand = komanda;
            da.Fill(dt2);
          //comboBox2
            for (i = 0; i < dt2.Rows.Count; i++)
            {
                lista.Add(Convert.ToString(dt2.Rows[i][0]));
                lista.Add(Convert.ToString(dt2.Rows[i][1]));
                string kombo2 = dt2.Rows[i][0].ToString() + " - " + dt2.Rows[i][1].ToString()+ " - "+Convert.ToDateTime(dt2.Rows[i][2]).ToShortDateString();
                if(!comboBox2.Items.Contains(kombo2))
                comboBox2.Items.Add(kombo2);
                if (!comboBox4.Items.Contains(kombo2))
                    comboBox4.Items.Add(kombo2);
            }
            dt2.Clear();
            //comboBox3
            komanda.CommandText = "SELECT Kategorija.id_kategorije,Kategorija.Naziv FROM Kategorija";
            da.SelectCommand = komanda;
            da.Fill(dt3);
            for (i = 0; i < dt3.Rows.Count; i++)
            {
                lista.Add(Convert.ToInt32(dt3.Rows[i][0]));
                lista.Add(Convert.ToString(dt3.Rows[i][1]));
                string kombo3 = dt3.Rows[i][0].ToString() + " - " + dt3.Rows[i][1].ToString();
                if (!comboBox3.Items.Contains(kombo3))
                    comboBox3.Items.Add(kombo3);
                
            }
            dt3.Clear();
            komanda.CommandText = "";
            komanda.CommandText = "SELECT id_izlozbe,id_kategorije,pas_id FROM Rezultat";
            da.SelectCommand = komanda;
            da.Fill(dt4);

        }
        string [] kombo1;
        string [] kombo2;

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Konekcija();
            string select = "SELECT KATEGORIJA.id_kategorije,KATEGORIJA.naziv,COUNT (REZULTAT.rezultat)";
            string from = "FROM KATEGORIJA INNER JOIN REZULTAT ON KATEGORIJA.id_kategorije=REZULTAT.id_kategorije";
            string where = "WHERE REZULTAT.id_izlozbe=@id_izlozbe";
            string group = "GROUP BY KATEGORIJA.id_kategorije,KATEGORIJA.naziv";
            komanda.Parameters.AddWithValue("@id_izlozbe", comboBox4.Text.Substring(0, 9));
            komanda.CommandText = select + " " + from + " " + where + " " + group;
            da.SelectCommand = komanda;
            da.Fill(dt1);
            dataGridView1.DataSource = dt1;
            dataGridView1.Columns[0].HeaderText = "ID kategorije";
            dataGridView1.Columns[1].HeaderText = "Naziv kategorije";
            dataGridView1.Columns[2].HeaderText = "Broj pasa po kategoriji ";
            chart1.Series["Pas"].IsValueShownAsLabel = true;
            ArrayList kategorija = new ArrayList();
            ArrayList broj_pasa = new ArrayList();
            chart1.Series["Pas"].Points.Clear();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                kategorija.Add(Convert.ToString(dataGridView1.Rows[i].Cells[1].Value));
               broj_pasa.Add(Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value));
                chart1.Series["Pas"].Points.AddXY(kategorija[i], broj_pasa[i]);
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            Konekcija();
            int a, b;
            string[] izlozba = comboBox4.Text.Split('-');
            komanda.CommandText = "SELECT COUNT (*) FROM Rezultat WHERE id_izlozbe=@id_izlozbe";
            komanda.Parameters.AddWithValue("@id_izlozbe", izlozba[0]);
            komanda1.CommandText = "SELECT COUNT (rezultat) FROM Rezultat WHERE id_izlozbe=@id_izlozbe";
            komanda1.Parameters.AddWithValue("@id_izlozbe", izlozba[0]);
            try
            {
                konekcija.Open();
                a = Convert.ToInt32(komanda.ExecuteScalar());
                b = Convert.ToInt32(komanda1.ExecuteScalar());
                label7.Text = a.ToString();
                label8.Text = b.ToString();
            }
            catch
            {
                MessageBox.Show("GRESKA");
            }
            finally 
            {
                konekcija.Close();
            }
        }

        string[] kombo3;
        void kombo_vrednosti()
        {
            kombo1 = comboBox1.Text.Split('-');//Id-imepsa
            kombo2 = comboBox2.Text.Split('-');//BEO042021-BEOGRAD-25.04.2021
            kombo3 = comboBox3.Text.Split('-');//Id-kategorija

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Konekcija();
            kombo_vrednosti();
            komanda.CommandText = "INSERT INTO Rezultat(id_izlozbe,id_kategorije,pas_id)VALUES (@izlozbaID,@kategorijaID,@pasID)";
            komanda.Parameters.AddWithValue("@izlozbaID", kombo2[0]);
            komanda.Parameters.AddWithValue("@kategorijaID", kombo3[0]);
            komanda.Parameters.AddWithValue("@pasID", kombo1[0]);
            bool postoji = false;
            for (int i = 0; i < dt4.Rows.Count; i++)
            {
                if (kombo2[0] == dt4.Rows[i][0].ToString() && kombo3[0] == dt4.Rows[i][1].ToString() && kombo1[0] == dt4.Rows[i][2].ToString())
                    postoji = true;
                break;
            }
            try
            {
                if (postoji == false)
                {
                    konekcija.Open();
                    komanda.ExecuteNonQuery();
                    MessageBox.Show("Pas je upisan na izabranu izlozbu");
                }
                else
                    MessageBox.Show("Pas je vec upisan na izabranu izlozbu ");
            }
            catch
            {
                MessageBox.Show("GRESKA!");
            }
            finally
            {
                konekcija.Close();
            }
            Form1_Load(sender, e);
        }
    }
}
