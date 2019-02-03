using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Net;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        MySqlConnection con;
        string ConnectionString="server=127.0.0.1;user=root;database=girls;password=++++++;charset=utf8; Allow Zero Datetime=true;";

        public string gethtml(string url)
        {
            HttpWebRequest proxy_request = (HttpWebRequest)WebRequest.Create(url);
            proxy_request.Method = "GET";
            proxy_request.ContentType = "application/x-www-form-urlencoded";
            proxy_request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/532.5 (KHTML, like Gecko) Chrome/4.0.249.89 Safari/532.5";
            proxy_request.KeepAlive = true;
            HttpWebResponse resp = proxy_request.GetResponse() as HttpWebResponse;
            string html = "";
            using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                html = sr.ReadToEnd();
            html = html.Trim();
            return html;
        }


        public void SqlQuery(string sql, string message)
        {
            using (MySqlConnection con = new MySqlConnection(ConnectionString))
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    //MessageBox.Show(message);
                }

                catch (Exception ex)
                {
                   // MessageBox.Show(ex.Message);
                }
            }
        }
        
        public DataTable Get_DataTable(string queryString)
        {
            DataTable dt = new DataTable();
            MySqlCommand com = new MySqlCommand(queryString, con);

            try
            {
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dt.Load(dr);
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dt;
        }

        public DataTable GetGirls(string id)
        {
            string q = @"SELECT pid FROM tab_girls";
            return Get_DataTable(q);
        }

        public DataTable GetPhotos(string id)
        {
            string q = @"SELECT big_photo, small_photo FROM photos";
            return Get_DataTable(q);
        }

        public Form1()
        {
            InitializeComponent();
        }

        public string clr(string s)
        {
            s = s.Substring(s.IndexOf("pink\">")+6, s.Length - s.IndexOf("pink\">")-6);
            s = s.Substring(0, s.IndexOf("<"));
            return s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename);
            textBox1.Text = fileText;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            con = new MySqlConnection();
            con.ConnectionString = ConnectionString;
            MySqlConnectionStringBuilder mysqlCSB = new MySqlConnectionStringBuilder();
            mysqlCSB.ConnectionString = ConnectionString;
            con.Open();
           // SqlQuery("INSERT into tab_girls VALUES(NULL,'1','2','3','','','','','','','','');","OK");
            dataGridView1.DataSource = GetGirls("");

            dataGridView2.DataSource = GetPhotos("");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;

            for (; ; )
            {
                s = s.Substring(s.IndexOf("\"girl_info\"") + 1, s.Length - s.IndexOf("\"girl_info\"") - 1);

                int ipos = s.IndexOf("\"girl_info\"");
                if (ipos < 0)
                    break;

                string s1 = s.Substring(0, ipos); //

                textBox2.Text = s1;

                string pid = s1.Substring(s1.IndexOf("profile")+8, 7);
                pid = pid.Substring(0, pid.IndexOf("\""));
                //MessageBox.Show(id);
                
                string phone = s1.Substring(s1.IndexOf("girl_phone") + 16, 15);
                string description = "";

                if (s1.IndexOf("girl_discription") > 0)
                {
                    description = s1.Substring(s1.IndexOf("girl_discription") + 36, s1.Length - s1.IndexOf("girl_discription") - 36);

                    if (description.IndexOf("<br>") > 0)
                        description = description.Substring(0, description.IndexOf("<br>"));
                }

                // MessageBox.Show(description);
                string photo = s1.Substring(s1.IndexOf("_files/prost") + 7, 46);
                photo = photo.Substring(0, photo.IndexOf("\""));
                //MessageBox.Show(photo);

                string nameraion = s1.Substring(s1.IndexOf("girl_name fll") + 16, 150);
                string name = nameraion.Substring(nameraion.IndexOf(">") + 1, nameraion.Length - nameraion.IndexOf(">") - 1);
                name = name.Substring(0, name.IndexOf("<"));
                string raion = nameraion.Substring(nameraion.IndexOf("<br>") + 4, nameraion.Length - nameraion.IndexOf("<br>") - 4);
                raion = raion.Substring(0, raion.IndexOf("</div>"));

                string vozrast = s1.Substring(s1.IndexOf("Возраст"), 40); vozrast = clr(vozrast);
                string rost = s1.Substring(s1.IndexOf("Рост"), 40); rost = clr(rost);
                string ves = s1.Substring(s1.IndexOf("Вес"), 50); ves = clr(ves);
                string grud = s1.Substring(s1.IndexOf("Грудь"), 50); grud = clr(grud);
                string chas = s1.Substring(s1.IndexOf("1 час"), 50); chas = clr(chas);
                string dvachasa = s1.Substring(s1.IndexOf("2 часа"), 50); dvachasa = clr(dvachasa);
                string noch = s1.Substring(s1.IndexOf("Ночь"), 50); noch = clr(noch);
                string viezd = s1.Substring(s1.IndexOf("Выезд"), 50); viezd = clr(viezd).Trim();

                textBox3.Text += "\r\n" + name + " " + raion + " " + phone + " " + photo + "\r\n" + vozrast + " " + rost + " " + ves + " " + grud + " " + chas + " " + dvachasa + " " + noch + " " + viezd;
                textBox3.Text += description;
                SqlQuery("INSERT into tab_girls VALUES(NULL,'" + pid + "'," + "NOW()" + ",'" + name + "','" + raion + "','" + phone + "','" + vozrast + "','" + rost + "','" + ves + "','" + grud + "','" + chas + "','" + dvachasa + "','" + noch + "','" + photo + "','" + description + "','" + viezd + "')", "OK");
            }
        }


        public void get_photos(string pid)
        {
            string s = gethtml("https://sexonsk.online/prostitutka/" + pid + "/");

            for (;;)
            {
                s = s.Substring(s.IndexOf("<a href='/photos/prostitutka") + 17, s.Length - s.IndexOf("<a href='/photos/prostitutka") - 17);

                int ipos = s.IndexOf("<a href='/photos/prostitutka");
                if (ipos < 0)
                    break;

                string s1 = s.Substring(0, 260);

                string bigphoto=s1.Substring(0,s1.IndexOf("'"));
                string smallphoto = s1.Substring(s1.IndexOf("'/")+1, s1.Length - s1.IndexOf("'/")-1);
                smallphoto = smallphoto.Substring(0, smallphoto.IndexOf("'"));
                
                SqlQuery("INSERT into photos VALUES(NULL, NOW(), '" + pid + "','" + bigphoto + "','" + smallphoto + "')", "OK");
            }   
        }

        public void get_skills(string pid)
        {
            string s = gethtml("https://sexonsk.online/prostitutka/" + pid + "/");
            Dictionary<string, int> map = new Dictionary<string, int>();

            map.Add("Классический секс", 1);
            map.Add("Оральный секс", 1);
            map.Add("Анальный секс", 1); //1
            map.Add("Принимаю кунилингус", 1);//4
            map.Add("Минет без презерватива", 1);//3
            map.Add("Групповой секс", 1);
            map.Add("Эротический массаж", 1);
            map.Add("Стриптиз", 1);//8
            map.Add("Секс с семейными парами", 1); //2
            map.Add("Лесбийский секс", 1);//7
            map.Add("Делаю кунилингус", 1);
            map.Add("Ролевые игры", 1);//5
            map.Add("Страпон", 1);//5
            map.Add("Игрушки", 1);
            map.Add("Золотой дождь", 1);
            map.Add("Эскорт", 1);

            for (;;)
            {
                s = s.Substring(s.IndexOf("strike") + 8, s.Length - s.IndexOf("strike") - 8);

                int ipos = s.IndexOf("strike");
                if (ipos < 0)
                    break;

                string s1 = s.Substring(0, 100);
                s1 = s1.Substring(0, s1.IndexOf("<"));
                map[s1] = 0;
            }

            string outs = "";
            string sql_req="";
            foreach (var pair in map)
            {
                outs += pair.Key + ":" + pair.Value + "\r\n";
                sql_req += ", '" + pair.Value + "'";
            }
            
            string query="INSERT into skills VALUES(NULL, '" + pid + "','" + outs + "'" + sql_req + ")";
            //MessageBox.Show(query);
            SqlQuery(query, "OK");
        }

        public void get_comments(string pid)
        {
            string s = gethtml("https://sexonsk.online/prostitutka/" + pid + "/");


            for (; ; )
            {
                s = s.Substring(s.IndexOf("word-wrap:break-word;") + 23, s.Length - s.IndexOf("word-wrap:break-word;") - 23);

                int ipos = s.IndexOf("word-wrap:break-word;");
                if (ipos < 0)
                    break;

                string s1 = s.Substring(0, 1300);

                string comment = s1.Substring(0, s1.IndexOf("<"));
                string name = s1.Substring(s1.IndexOf("Написал(") + 14, 100);
                string date = name.Substring(name.IndexOf("</b>") + 6, 30);
                name = name.Substring(0, name.IndexOf("</b>"));
                name = name.Trim();
                date = date.Substring(0, date.IndexOf(".<"));

                //MessageBox.Show(comment);
                //MessageBox.Show(name);
                //MessageBox.Show(date);

                string query = "INSERT into comments VALUES(NULL, '" + pid + "','" + comment + "','" + name + "','" + date + "')";
                // MessageBox.Show(query);
                SqlQuery(query, "OK");
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            string pid = dataGridView1.CurrentCell.Value.ToString();
            get_photos(pid);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string s = dataGridView1.CurrentCell.Value.ToString();
            MessageBox.Show(s);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = dataGridView1.Rows.Count;
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                string pid=dataGridView1.Rows[i].Cells[0].Value.ToString();
                get_photos(pid);
                progressBar1.Value++;
                Application.DoEvents();
                this.Text = i.ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string pid = "160194";
            get_skills(pid);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = dataGridView1.Rows.Count;
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                string pid = dataGridView1.Rows[i].Cells[0].Value.ToString();
                get_skills(pid);
                progressBar1.Value++;
                Application.DoEvents();
                this.Text = i.ToString();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string pid = "311034";
            get_comments(pid);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = dataGridView1.Rows.Count;
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                string pid = dataGridView1.Rows[i].Cells[0].Value.ToString();
                get_comments(pid);
                progressBar1.Value++;
                Application.DoEvents();
                this.Text = i.ToString();
            }
        }

        private void DownloadPhoto(string fn)
        {
            fn = fn.Substring(0, fn.IndexOf("?"));
            WebClient wc = new WebClient();
            string url = "https://sexonsk.online/photos/140_210/" + fn;
            string save_path = Path.GetDirectoryName(Application.ExecutablePath) + "\\";

            //MessageBox.Show(save_path);
            string name = fn;
            wc.DownloadFile(url, save_path + name);
        }


        private void button9_Click(object sender, EventArgs e)
        {
            DownloadPhoto("prostitutka_318451_5c4ac95bbe340_r.jpg");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = dataGridView2.Rows.Count;
            for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
            {
                string pid = dataGridView2.Rows[i].Cells[0].Value.ToString();

                DownloadPhoto(pid);

                progressBar1.Value++;
                Application.DoEvents();
                this.Text = i.ToString();
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
