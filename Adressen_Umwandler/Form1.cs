using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Web;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Adressen_Umwandler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = this.Text + " | Version: " + Application.ProductVersion.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = ""; 
            for (int f = 0; f < (dataGridView1.Rows.Count -1); f++)
            {
                string t = Convert.ToString(dataGridView1.Rows[f].Cells[0].Value);
                string t1 = t; 
                string[] t2 = t1.Split(';');
                string[] t3 = t2[0].Split(' ');
                string t4 = "";
                if (Convert.ToString(dataGridView1.Rows[f].Cells[0].Value) != "" && t3.Length > 1)
                {
                    if (Convert.ToBoolean(dataGridView1.Rows[f].Cells[1].Value) == true)
                    {
                        for (int i = 0; i < t3.Length; i++)
                        {
                            if (i == 0)
                            {
                                t4 += t3[i + 1];
                            }
                            else if (i == 1)
                            {
                                t4 += t3[i - 1];
                            }
                            else
                            {
                                t4 += t3[i];
                            }
                            if (i == t3.Length - 1)
                            {
                                t4 += ";";
                            }
                            else
                            {
                                t4 += " ";
                            }
                        }
                        richTextBox1.Text += t4 + t2[1] + ";" + t2[2] + ";" + t2[3] + ";" + t2[4];
                    }
                    else
                    {
                        if (f == 0)
                        {
                            richTextBox1.Text += t1;
                        }
                        else
                        {
                            richTextBox1.Text += t1;
                        }
                    }
                }             
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox_ort.Text != "" && textBox_unternehmen.Text != "")
            {
                dataGridView1.Rows.Clear();
                // Internetseite
                WebClient wc = new WebClient();
                String was = textBox_unternehmen.Text;
                String was2 = was.Replace("ä", "ae");
                String was3 = was2.Replace("ü", "ue");
                string html_code = wc.DownloadString(@"http://www.gelbeseiten.de/" + was3 + "/" +textBox_ort.Text);
                String code = HttpUtility.HtmlDecode(html_code);
                string suche = "<h1 id=\"yourSearchInfo\">(.*?)</h1>";
                Regex reg_suche = new Regex(suche, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                MatchCollection matches_suche = reg_suche.Matches(code);
                dataGridView1.Columns[0].HeaderText = "Import (in der Umgebung von "+textBox_ort.Text+")";
                foreach (Match match_suche in matches_suche)
                {
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                       dataGridView1.Columns[0].HeaderText = "Import (" + match_suche.Groups[1].Value.Replace("&nbsp;", " ") + ")";
                    }
                }
                string box = "<div class=\"table\"(.*?)<div class=\"outer_footer\">";
                Regex reg_box = new Regex(box, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                MatchCollection matches_box = reg_box.Matches(code);
                // Boxen selektieren
                foreach (Match match_box in matches_box)
                {
                    // Quelcode der aktuellen Box ermitteln
                    string box_string = match_box.Groups[1].Value;

                    // Name auslesen
                    string name_out = "";
                    string name = "<span itemprop=\"name\">(.*?)</span>";
                    Regex reg_name = new Regex(name, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    MatchCollection matches_name = reg_name.Matches(box_string);
                    foreach (Match match_name in matches_name)
                    {
                        name_out = match_name.Groups[1].Value;

                    }

                    // Straße auslesen
                    string strasse_out = "";
                    string strasse = "<span itemprop=\"streetAddress\">(.*?)</span>";
                    Regex reg_strasse = new Regex(strasse, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    MatchCollection matches_strasse = reg_strasse.Matches(box_string);
                    foreach (Match match_strasse in matches_strasse)
                    {
                        strasse_out = match_strasse.Groups[1].Value;
                    }

                    // PLZ auslesen
                    string plz_out = "";
                    string plz = "<span itemprop=\"postalCode\">(.*?)</span>";
                    Regex reg_plz = new Regex(plz, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    MatchCollection matches_plz = reg_plz.Matches(box_string);
                    foreach (Match match_plz in matches_plz)
                    {
                        plz_out = match_plz.Groups[1].Value;

                    }

                    // Ort auslesen
                    string ort_out = "";
                    string ort = "<span itemprop=\"addressLocality\">(.*?)</span>";
                    Regex reg_ort = new Regex(ort, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    MatchCollection matches_ort = reg_ort.Matches(box_string);
                    foreach (Match match_ort in matches_ort)
                    {
                        ort_out = match_ort.Groups[1].Value;

                    }

                    // Telefon auslesen
                    string telefon_out = "";
                    string telefon = "<span class=\"nummer\">(.*?)</span";
                    Regex reg_telefon = new Regex(telefon, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    MatchCollection matches_telefon = reg_telefon.Matches(box_string);
                    foreach (Match match_telefon in matches_telefon)
                    {
                        telefon_out = match_telefon.Groups[1].Value;

                    }
                    if (telefon_out == "") { telefon_out = "0"; }             

                    // Ausgeben
                    string ausgabe = name_out + ";" + strasse_out + ";" + plz_out + ";" + ort_out + ";" + telefon_out;
                    string ausgabe2 = ausgabe.Replace("\t", "");
                    string ausgabe3 = ausgabe2.Replace("\n", "");
                    dataGridView1.Rows.Add(ausgabe3 + "\n");
                }

                if (dataGridView1.RowCount > 7)
                {
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    column.Width = 487;
                }
                else
                {
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    column.Width = 504;
                }
            }
            else
            {
                MessageBox.Show("Bitte beide Eingabefelder ausfüllen!");
            }


            //
            richTextBox1.Text = "";
            for (int f = 0; f < (dataGridView1.Rows.Count - 1); f++)
            {
                string t = Convert.ToString(dataGridView1.Rows[f].Cells[0].Value);
                string t1 = t;
                string[] t2 = t1.Split(';');
                string[] t3 = t2[0].Split(' ');
                string t4 = "";
                if (Convert.ToString(dataGridView1.Rows[f].Cells[0].Value) != "" && t3.Length > 1)
                {
                    if (Convert.ToBoolean(dataGridView1.Rows[f].Cells[1].Value) == true)
                    {
                        for (int i = 0; i < t3.Length; i++)
                        {
                            if (i == 0)
                            {
                                t4 += t3[i + 1];
                            }
                            else if (i == 1)
                            {
                                t4 += t3[i - 1];
                            }
                            else
                            {
                                t4 += t3[i];
                            }
                            if (i == t3.Length - 1)
                            {
                                t4 += ";";
                            }
                            else
                            {
                                t4 += " ";
                            }
                        }
                        richTextBox1.Text += t4 + t2[1] + ";" + t2[2] + ";" + t2[3] + ";" + t2[4];
                    }
                    else
                    {
                        if (f == 0)
                        {
                            richTextBox1.Text += t1;
                        }
                        else
                        {
                            richTextBox1.Text += t1;
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.RowCount > 7)
            {
                DataGridViewColumn column = dataGridView1.Columns[0];
                column.Width = 487;
            }
            else
            {
                DataGridViewColumn column = dataGridView1.Columns[0];
                column.Width = 504;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoBox ibox = new InfoBox();
            ibox.Show();
            
        }

        private void button_zwischenablage_Click(object sender, EventArgs e)
        {
            // Zwischenablage
            if (richTextBox1.Text != null && richTextBox1.Text != "")
            {
                Clipboard.SetText(richTextBox1.Text);
            }
        }

        private void uRLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gelbeseiten.de");
        }

        private void themenkatalogÖffnenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gelbeseiten.de/-");
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            for (int f = 0; f < (dataGridView1.Rows.Count - 1); f++)
            {
                string t = Convert.ToString(dataGridView1.Rows[f].Cells[0].Value);
                string t1 = t;
                string[] t2 = t1.Split(';');
                string[] t3 = t2[0].Split(' ');
                string t4 = "";
                if (Convert.ToString(dataGridView1.Rows[f].Cells[0].Value) != "" && t3.Length > 1)
                {
                    if (Convert.ToBoolean(dataGridView1.Rows[f].Cells[1].Value) == true)
                    {
                        for (int i = 0; i < t3.Length; i++)
                        {
                            if (i == 0)
                            {
                                t4 += t3[i + 1];
                            }
                            else if (i == 1)
                            {
                                t4 += t3[i - 1];
                            }
                            else
                            {
                                t4 += t3[i];
                            }
                            if (i == t3.Length - 1)
                            {
                                t4 += ";";
                            }
                            else
                            {
                                t4 += " ";
                            }
                        }
                        richTextBox1.Text += t4 + t2[1] + ";" + t2[2] + ";" + t2[3] + ";" + t2[4];
                    }
                    else
                    {
                        if (f == 0)
                        {
                            richTextBox1.Text += t1;
                        }
                        else
                        {
                            richTextBox1.Text += t1;
                        }
                    }
                }
            }
        }
    }
}
