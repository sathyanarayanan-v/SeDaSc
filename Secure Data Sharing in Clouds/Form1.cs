using FireSharp.Config; //Configuration Files for Firesharp to connect C# to Firebase
using FireSharp.Interfaces; // Methods to Send and recieve Data from Firebase
using FireSharp.Response;   
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using System.Data;

namespace Secure_Data_Sharing_in_Clouds
{
    public partial class Form1 : Form
    {
        IFirebaseConfig Config = new FirebaseConfig
        {
            AuthSecret = "lJRZ4SEeMf7XzmPHbUPjRbnIyjKuajy7NVTX5B3x",
            BasePath= "https://sedascincloud.firebaseio.com/",

        };
        IFirebaseClient client;
        public static int CheckForInternetConnection()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                using (client.OpenRead("https://www.google.com"))
                {
                    return 5;
                }
            }
            catch
            {
                return 6;
            }
        }
        public double CheckInternetSpeed()
        {
            // Create Object Of WebClient
            System.Net.WebClient wc = new System.Net.WebClient();

            //DateTime Variable To Store Download Start Time.
            DateTime dt1 = DateTime.Now;

            //Number Of Bytes Downloaded Are Stored In ‘data’
            byte[] data = wc.DownloadData("https://www.google.com");

            //DateTime Variable To Store Download End Time.
            DateTime dt2 = DateTime.Now;

            //To Calculate Speed in Kb Divide Value Of data by 1024 And Then by End Time Subtract Start Time To Know Download Per Second.
            return Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2);
        }
        static string encrypt;
        static string decrypt;
        bool flag = true;
        public Form1()
        {
            InitializeComponent();
            
            //button1.Enabled = false;
            sha_len.Text = SHA_256.TextLength.ToString();
            acce_len.Text = KEY_TB.TextLength.ToString();
            Password_tb.PasswordChar =textBox5.PasswordChar=textBox4.PasswordChar= '\u25CF';
            client = new FireSharp.FirebaseClient(Config);
            int x = CheckForInternetConnection();
            if ((client!=null))
            {
                pictureBox1.BackColor = pictureBox2.BackColor = Color.Green;
            }
           // label48.Text = CheckInternetSpeed().ToString() + " " + "Kb/S.";
            groupBox1.Enabled = groupBox3.Enabled = groupBox4.Enabled = groupBox5.Enabled = false;
            this.WindowState = FormWindowState.Maximized;           
        }
        /*...................Random String Generation Begins.................................*/
        string grpid;

        public static string GetRandomAlphanumericString(int length)
        {
            const string alphanumericCharacters ="ABCDEFGHIJKLMNOPQRSTUVWXYZ" +"abcdefghijklmnopqrstuvwxyz";
            return GetRandomString(length, alphanumericCharacters);
        }

        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }

        /*...................Random String Generation Ends.................................*/


        /*...................Encryption of SHA-256 Begins.................................*/

            static void EncryptAesManaged(string raw, byte[] ba) 
            {
                try 
                {
                    StringBuilder str = new StringBuilder();
                    using(AesManaged aes = new AesManaged())
                    {  
                        byte[] encrypted = Encrypt(raw, ba, aes.IV);
                        foreach (Byte b in encrypted)
                        str.Append(b.ToString("x2"));
                        encrypt = str.ToString();
                        decrypt= Decrypt(encrypted,ba, aes.IV);
                    }  
                }
                catch (Exception exp) 
                { 
                    MessageBox.Show(exp.Message);  
                }  
            }
  /*.......................Encryption Call....................................*/
    static byte[] Encrypt(string plainText, byte[] Key, byte[] IV) {  
        byte[] encrypted;  
        // Create a new AesManaged.    
        using(AesManaged aes = new AesManaged()) {  
            // Create encryptor    
            ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);  
            // Create MemoryStream    
            using(MemoryStream ms = new MemoryStream()) {  
                // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                // to encrypt    
                using(CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {  
                    // Create StreamWriter and write data to a stream    
                    using(StreamWriter sw = new StreamWriter(cs))  
                    sw.Write(plainText);  
                    encrypted = ms.ToArray();  
                }  
            }  
        }  
        // Return encrypted data    
        return encrypted;  
    }

    /*.......................Decryption Call....................................*/
        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV) {  
        string plaintext = null;  
        // Create AesManaged    
        using(AesManaged aes = new AesManaged()) {  
            // Create a decryptor    
            ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);  
            // Create the streams used for decryption.    
            using(MemoryStream ms = new MemoryStream(cipherText)) {  
                // Create crypto stream    
                using(CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) {  
                    // Read crypto stream    
                    using(StreamReader reader = new StreamReader(cs))  
                    plaintext = reader.ReadToEnd();  
                }  
            }  
        }  
        return plaintext;  
    }
        private async void Button1_Click(object sender, EventArgs e)
        {
            Byte[] result;
           string rand= Password_tb.Text = GetRandomAlphanumericString(32);
            StringBuilder Sb = new StringBuilder();
            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                result = hash.ComputeHash(enc.GetBytes(rand));
                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            username_tb.Text = Sb.ToString();
            sha_len.Text = username_tb.TextLength.ToString();
            acce_len.Text = SHA_256.TextLength.ToString();
            EncryptAesManaged(text_tb.Text,result);
            SHA_256.Text = encrypt;
            acce_len.Text = SHA_256.TextLength.ToString();
            KEY_TB.Text = decrypt;
            string csr = textBox2.Text=GetRandomAlphanumericString(32);
           
            StringBuilder sb = new StringBuilder();
            StringBuilder xv = new StringBuilder();
            StringBuilder mm = new StringBuilder();
            foreach (char c in csr.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            foreach (char c in encrypt.ToCharArray())
            {
                xv.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            string binkey = sb.ToString();
            string binkey2 = xv.ToString();
            string userkey = "";
            for (int i = 0; i < binkey.Length; i++)
            {
                char x;
                char y = binkey[i];
                char z = binkey2[i];
                 x=(char)(binkey[i] ^ binkey2[i]);
                 if (x == '\0')
                 {
                     x = '0';
                 }
                 else
                     x = '1';
                userkey+=x;
            }

            string temp = userkey;
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < userkey.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(userkey.Substring(i, 8), 2));
            }
            textBox4.Text=Encoding.ASCII.GetString(byteList.ToArray()); 
            label16.Text = textBox4.TextLength.ToString();
            var data = new Data
................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................


            {
                file = text_tb.Text,
                cipher = SHA_256.Text
            };
            try
            {
                SetResponse response = await client.SetTaskAsync("Users/" + grpid, data);
                Data results = response.ResultAs<Data>();
                progressBar1.Maximum = 100;
                progressBar1.Value = 100;
                progressBar1.BackColor =System.Drawing.Color.Lime;
                if (results.file != null)
                {
                    MessageBox.Show("Successfully Uploaded the file  " + results.file,"Secure Data Sharing in Cloud",MessageBoxButtons.OKCancel,MessageBoxIcon.Information,MessageBoxDefaultButton.Button1);
                }
                string insre = "update  access_rights_read set userkey='"+textBox2.Text+"' where group_id='"+grpid+"'";
                string inswr = "update  access_rights_write set userkey= '" + textBox2.Text + "' where group_id='"+grpid+"'";
                SqlConnection conn = new SqlConnection("Data Source=DESKTOP-VDKS12B\\VS;Initial Catalog=SeDaSc;Integrated Security=True");
                conn.Open();
                SqlCommand insrea = new SqlCommand(insre, conn);
                SqlDataReader obj = insrea.ExecuteReader();
                conn.Close();
                SqlCommand inswri = new SqlCommand(inswr, conn);
                conn.Open();
                SqlDataReader obj2 = inswri.ExecuteReader();
                conn.Close();
                Clipboard.SetText(textBox4.Text);
                MessageBox.Show("User Key Copied to Clipboard"+"\n"+"Please use it to Download Data.","Secure Data Sharing in Cloud", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.ToString());
            }
            //progressBar1.BackColor = Color.Green;
           
        }

        private void Show_hid_btn_Click(object sender, EventArgs e)
        {
            textBox4.PasswordChar = '\0';
        }

        private void hide_pw_btn_Click(object sender, EventArgs e)
        {
           textBox4.PasswordChar = '\u25CF';
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            SHA_256.Text = "";
            Password_tb.Text = "";
            username_tb.Text = "";
            KEY_TB.Text = "";
            sha_len.Text = username_tb.TextLength.ToString();
            acce_len.Text = Password_tb.TextLength.ToString();
        }
        private void Refresh_btn_sign_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 y = new Form1();
            y.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'seDaScDataSet.Users_list' table. You can move, or remove it, as needed.
            try
            {
                this.users_listTableAdapter.Fill(this.seDaScDataSet.Users_list);
                grant_per_btn.Enabled = false;
                grant_per_btn.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            groupBox1.Enabled = false;
           
        }
        public object data { get; set; }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        //To avoid repetition of adding columns

        bool setcolumn = false;
        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
        DataGridViewCheckBoxColumn writeColumn = new DataGridViewCheckBoxColumn();
        string owner,id;


        private void Set_owner_btn_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count==0)
            {
                MessageBox.Show("Please select owner of the File");
            }
            try
            {
                if (!setcolumn)
                {
                    checkColumn.Name = "Read";
                    checkColumn.HeaderText = "Read";
                    checkColumn.Width = 50;
                    checkColumn.ReadOnly = false;
                    checkColumn.FillWeight = 10; //if the datagridview is resized (on form resize) the checkbox won't take up too much; value is relative to the other columns' fill values
                    dataGridView1.Columns.Add(checkColumn);

                    writeColumn.Name = "Write";
                    writeColumn.HeaderText = "Write";
                    writeColumn.Width = 50;
                    writeColumn.ReadOnly = false;
                    writeColumn.FillWeight = 10; //if the datagridview is resized (on form resize) the checkbox won't take up too much; value is relative to the other columns' fill values
                    dataGridView1.Columns.Add(writeColumn);
                    setcolumn = true;
                    owner = dataGridView1.SelectedRows[0].Cells["nameDataGridViewTextBoxColumn"].Value.ToString();
                    id= dataGridView1.SelectedRows[0].Cells["idDataGridViewTextBoxColumn"].Value.ToString();
                    MessageBox.Show("The Owner of the Data is " + owner + "\n\n" + "Please Select Access Rights for the Data and for Creating group.");
                    foreach (DataGridViewRow item in this.dataGridView1.SelectedRows)
                    {
                        dataGridView1.Rows.RemoveAt(item.Index);
                    }
                    Set_owner_btn.Enabled = false;
                    grant_per_btn.Text = "Access Permission for " + owner + "'s data";
                    grant_per_btn.Enabled = true;
                    textBox1.Text = owner;

                }
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }


        }
        ArrayList readlist = new ArrayList();
        ArrayList writelist = new ArrayList();

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            try
            {
                if (e.ColumnIndex == checkColumn.Index && e.RowIndex !=-1)
                {
                    DataGridViewCheckBoxCell ch1 = new DataGridViewCheckBoxCell();
                    ch1 = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    

                    if (ch1.Value == null)
                        ch1.Value = false;
                    switch (ch1.Value.ToString())
                    {
                        case "True":
                            ch1.Value = false;
                            break;
                        case "False":
                            ch1.Value = true;
                            break;
                    }
                    if((Convert.ToBoolean(ch1.Value)))
                    {
                        readlist.Add(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    }
                    //MessageBox.Show(ch1i.Value.ToString());
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            try
            {
                if (e.ColumnIndex == writeColumn.Index && e.RowIndex != -1)
                {
                    DataGridViewCheckBoxCell ch1 = new DataGridViewCheckBoxCell();
                    ch1 = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    if (ch1.Value == null)
                        ch1.Value = false;
                    switch (ch1.Value.ToString())
                    {
                        case "True":
                            ch1.Value = false;
                            break;
                        case "False":
                            ch1.Value = true;
                            break;
                    }
                    if ((Convert.ToBoolean(ch1.Value)))
                    {
                       writelist.Add(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void Upl_btn_Click(object sender, EventArgs e)
        {
            groupBox1.BringToFront();
            groupBox3.BringToFront();
            groupBox1.Enabled = groupBox3.Enabled = true;
            groupBox5.SendToBack();
            groupBox4.SendToBack();
            comboBox1.DataSource = null;
        }

        private void Del_btn_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = groupBox3.Enabled = false;
            groupBox5.BringToFront();
            groupBox5.Enabled = true;
            groupBox4.Enabled = false;
          //  this.comboBox1.DataSource = this.userslistBindingSource1;
        }

        private void Down_btn_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = groupBox3.Enabled = false;
            groupBox5.Enabled = false;
            groupBox4.BringToFront();
            groupBox4.Enabled = true;
            string conStr = "Data Source=DESKTOP-VDKS12B\\VS;Initial Catalog=SeDaSc;Integrated Security=True";

            SqlDataAdapter adapter = new SqlDataAdapter("Select name from Users_list", new SqlConnection(conStr));
            DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);

            comboBox2.DisplayMember = "name";
            comboBox2.ValueMember = "name";
            comboBox2.DataSource = dt;
            textBox5.Text = Clipboard.GetText();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Abstract_paper abstract_ = new Abstract_paper();
            abstract_.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Base_Paper base_ = new Base_Paper();
            base_.Show();
        }

       
       

        private void Del_rec_btn_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are You Sure you want to delete " + comboBox1.Text + "'s data and Remove ownership", "Delete User From ACL", MessageBoxButtons.YesNoCancel);

            if (dr == DialogResult.Yes)
            {
                bool sys = true;
                bool sys1 = true;
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-VDKS12B\\VS;Initial Catalog=SeDaSc;Integrated Security=True");
                string delowner = " DELETE FROM [SeDaSc].[dbo].[access_rights_read] WHERE owner_name = '" + comboBox1.Text + "'";
                string delwri = "DELETE FROM [SeDaSc].[dbo].[access_rights_write] WHERE owner_name = '" + comboBox1.Text + "'";
                SqlCommand re1 = new SqlCommand(delowner, con);
                con.Open();
                SqlDataReader obj1 = re1.ExecuteReader();
                if (obj1.RecordsAffected > 0) sys = true;
                con.Close();
                re1 = new SqlCommand(delwri, con);
                con.Open();
                obj1 = re1.ExecuteReader();
                if (obj1.RecordsAffected > 0) sys1 = true;
                con.Close();
                if((sys && sys1))
                {
                    MessageBox.Show("You are not owner of any files." + "\n" + "Operation Denied by Cryptographic Server", "Secure Data Sharing in Cloud", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    dataGridView3.Rows.Clear();
                }
            }
        }

        private void Get_user_list_Click(object sender, EventArgs e)
        {
             string conStr = "Data Source=DESKTOP-VDKS12B\\VS;Initial Catalog=SeDaSc;Integrated Security=True";

            SqlDataAdapter adapter = new SqlDataAdapter("Select name from Users_list", new SqlConnection(conStr));
            DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);

            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "name";
            comboBox1.DataSource = dt;
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (flag)
            {
                dataGridView3.Rows.Clear();
                string user = comboBox1.Text;
                int count1, count2, count3, max;
                count1 = count2 = count3 = max = 0;
                string fetownerque = "select group_id from [SeDaSc].[dbo].[access_rights_read] where owner_name='" + user + "' ";
                string fetreaque = "select group_id from [SeDaSc].[dbo].[access_rights_read] where read_name='" + user + "' ";
                string fetwrique = "select group_id from [SeDaSc].[dbo].[access_rights_write] where write_name='" + user + "' ";
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-VDKS12B\\VS;Initial Catalog=SeDaSc;Integrated Security=True");
                SqlCommand re1 = new SqlCommand(fetownerque, con);
                con.Open();
                SqlDataReader obj1 = re1.ExecuteReader();
                while (obj1.Read())
                {
                    count1++;
                }
                max = count1;
                con.Close();
                SqlCommand re2 = new SqlCommand(fetreaque, con);
                con.Open();
                SqlDataReader obj2 = re2.ExecuteReader();
                while (obj2.Read())
                {
                    count2++;
                }
                max = (count2 > max) ? count2 : max;
                con.Close();
                SqlCommand re3 = new SqlCommand(fetwrique, con);
                con.Open();
                SqlDataReader obj3 = re3.ExecuteReader();
                while (obj3.Read())
                {
                    count3++;
                }
                max = (count3 > max) ? count3 : max;
                con.Close();
                for (int i = 0; i < max; i++)
                {
                    dataGridView3.Rows.Add();
                    dataGridView3.Rows[i].Cells[0].Value = user;
                }
                if ((max > 0))
                {
                    con.Open();
                    obj2 = re2.ExecuteReader();
                    int p = 0;
                    while (obj2.Read())
                    {
                        dataGridView3.Rows[p].Cells[2].Value = obj2["group_id"];
                        p++;
                    }
                    con.Close();
                    con.Open();
                    obj1 = re1.ExecuteReader();
                    p = 0;
                    while (obj1.Read())
                    {
                        dataGridView3.Rows[p].Cells[1].Value = obj1["group_id"];
                        p++;
                    }
                    con.Close();
                    con.Open();
                    obj3 = re3.ExecuteReader();
                    p = 0;
                    while (obj3.Read())
                    {
                        dataGridView3.Rows[p].Cells[3].Value = obj3["group_id"];
                        p++;
                    }
                    con.Close();
                }
                else
                {
                    MessageBox.Show("No Records Found", "Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                //flag = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-VDKS12B\\VS;Initial Catalog=SeDaSc;Integrated Security=True");
                string delowner = " SELECT distinct group_id FROM [SeDaSc].[dbo].[access_rights_read] WHERE read_name = '" + comboBox2.Text + "'";
                string delwri = "SELECT distinct group_id FROM [SeDaSc].[dbo].[access_rights_write] WHERE write_name = '" + comboBox2.Text + "'";
                string delwri1 = "SELECT distinct group_id FROM [SeDaSc].[dbo].[access_rights_write] WHERE owner_name = '" + comboBox2.Text + "'";
              //  string delwri2 = "SELECT distinct group_id FROM [SeDaSc].[dbo].[access_rights_read] WHERE owner_name = '" + comboBox2.Text + "'";
                SqlCommand re1 = new SqlCommand(delowner, con);
                con.Open();
                SqlDataReader obj1 = re1.ExecuteReader();
                while (obj1.Read())
                {
                    comboBox3.Items.Add(obj1["group_id"]);
                }
                con.Close();
                re1 = new SqlCommand(delwri, con);
                con.Open();
                obj1 = re1.ExecuteReader();
                while (obj1.Read())
                {
                    comboBox3.Items.Add(obj1["group_id"]);
                }
                con.Close();
                re1 = new SqlCommand(delwri1, con);
                con.Open();
                obj1 = re1.ExecuteReader();
                while (obj1.Read())
                {
                    comboBox3.Items.Add(obj1["group_id"]);
                }
                con.Close();
                List<object> list = new List<object>();
                foreach (object o in comboBox3.Items)
                {
                    if (!list.Contains(o))
                    {
                        list.Add(o);
                    }
                }
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(list.ToArray());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void Show_pass_btn_Click(object sender, EventArgs e)
        {
            textBox5.PasswordChar = '\0';
        }

        private void Hide_pass_btn_Click(object sender, EventArgs e)
        {
            textBox5.PasswordChar = '\u25CF';
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            user_key_lbl.Text = textBox5.TextLength.ToString();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                textBox7.Text = GetRandomAlphanumericString(32);
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-VDKS12B\\VS;Initial Catalog=SeDaSc;Integrated Security=True");
                string getki = " SELECT userkey FROM [SeDaSc].[dbo].[access_rights_read] WHERE read_name = '" + comboBox2.Text + "' and group_id='" + comboBox3.Text + "'";
                SqlCommand re1 = new SqlCommand(getki, con);
                con.Open();
                SqlDataReader obj1 = re1.ExecuteReader();
                if (!(obj1.HasRows))
                {
                    MessageBox.Show("There is no Read Access for the Current User ", "Secure Data Sharing in Cloud", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    while(obj1.Read())
                    textBox8.Text = obj1["userkey"].ToString();
                }
                con.Close();
                string getkiwri = " SELECT userkey FROM [SeDaSc].[dbo].[access_rights_write] WHERE write_name = '" + comboBox2.Text + "' and group_id='" + comboBox3.Text + "'";
                re1 = new SqlCommand(getkiwri, con);
                con.Open();
                obj1 = re1.ExecuteReader();
                if (!(obj1.HasRows))
                {
                    MessageBox.Show("There is no Write Access for the Current User ", "Secure Data Sharing in Cloud", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    textBox9.Enabled = false;
                }
                else
                {
                    while(obj1.Read())
                    textBox8.Text = obj1["userkey"].ToString();
                    textBox9.Enabled = true;
                }
                con.Close();
                FirebaseResponse response = await client.GetTaskAsync("Users/" + comboBox3.Text);
                Data results = response.ResultAs<Data>();
                if (results.cipher != null)
                {
                    textBox6.Text = results.cipher;
                }
                else
                {
                    MessageBox.Show("There is no Data for the Current User ", "Secure Data Sharing in Cloud", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
                StringBuilder sb = new StringBuilder();
                StringBuilder xv = new StringBuilder();
                foreach (char c in textBox5.Text.ToCharArray())
                {
                    sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
                }
                foreach (char c in textBox8.Text.ToCharArray())
                {
                    xv.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
                }
                string binkey = sb.ToString();
                string binkey2 = xv.ToString();
                string userkey = "";
                for (int i = 0; i < binkey.Length; i++)
                {
                    char x;
                    char y = binkey[i];
                    char z = binkey2[i];
                    x = (char)(binkey[i] ^ binkey2[i]);
                    if (x == '\0')
                    {
                        x = '0';
                    }
                    else
                        x = '1';
                    userkey += x;
                }

                string temp = userkey;
                List<Byte> byteList = new List<Byte>();

                for (int i = 0; i < userkey.Length; i += 8)
                {
                    byteList.Add(Convert.ToByte(userkey.Substring(i, 8),2));
                }
                AesManaged aes = new AesManaged();
                textBox7.Text = Encoding.ASCII.GetString(byteList.ToArray());
                string ciphers = textBox6.Text;
                    byte[] bytes = Encoding.UTF8.GetBytes(ciphers);
                textBox9.Text = results.file.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox5.Text = "";
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
           /* */
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }



        // Variable names edited for readability

        private void grant_per_btn_Click(object sender, EventArgs e)
        {
            
            try
            {
                grpid = GetRandomAlphanumericString(5);
                List<DataGridViewRow> toDelete = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!((Convert.ToBoolean(row.Cells["Read"].Value)) || (Convert.ToBoolean(row.Cells["Write"].Value))))
                    {
                        toDelete.Add(row);

                    }
                    else
                    {
                        row.Cells["Groupid"].Value = grpid;
                    }
                }
                foreach (DataGridViewRow row in toDelete)
                {
                    dataGridView1.Rows.Remove(row);
                }
                string ReadQuery,reaquery,wriquery,query;
                SqlConnection conn = new SqlConnection("Data Source=DESKTOP-VDKS12B\\VS;Initial Catalog=SeDaSc;Integrated Security=True");
                SqlCommand comm = new SqlCommand
                {
                    Connection = conn
                };
                conn.Open();
                int count = dataGridView1.Rows.Count;
                grpid=(dataGridView1.Rows[0].Cells["groupid"].Value.ToString());
                for (int i = 0; i < readlist.Count; i++)
                {
                    ReadQuery = "insert into access_rights_read (id,owner_name,read_name,group_id) values('"+id+"','"+owner+"','"+readlist[i]+"','"+grpid+"')";
                    comm.CommandText = ReadQuery;
                    comm.ExecuteNonQuery();
                }
                for (int i = 0; i < writelist.Count; i++)
                {
                   query = "insert into access_rights_write (id,owner_name,write_name,group_id) values('" + id + "','" + owner + "','" + writelist[i] + "','" + grpid + "')";
                   comm.CommandText = query;
                   comm.ExecuteNonQuery();
                }
                conn.Close();
                reaquery = "SELECT distinct [read_name],id,owner_name,group_id FROM[SeDaSc].[dbo].[access_rights_read] where [SeDaSc].[dbo].[access_rights_read].group_id='"+grpid+"'";
                wriquery = "SELECT distinct [write_name],id,owner_name,group_id FROM[SeDaSc].[dbo].[access_rights_write] where [SeDaSc].[dbo].[access_rights_write].group_id='" + grpid + "'";
                for(int i=0;i<(System.Math.Max(writelist.Count,readlist.Count));i++)
                {
                   dataGridView2.Rows.Add();
                }
                SqlCommand re1 = new SqlCommand(wriquery,conn);
                conn.Open();
                SqlDataReader obj1 = re1.ExecuteReader();
                int p = 0;
                while (obj1.Read())
                {
                    dataGridView2.Rows[p].Cells["Write_acc"].Value = obj1["write_name"];
                    dataGridView2.Rows[p].Cells[0].Value = obj1["id"];
                    dataGridView2.Rows[p].Cells[4].Value = obj1["group_id"];
                    dataGridView2.Rows[p].Cells[1].Value = obj1["owner_name"];
                    p++;
                }
                conn.Close();
                SqlCommand re2 = new SqlCommand(reaquery, conn);
                conn.Open();
                obj1 = re2.ExecuteReader();
                int c = 0;
                while (obj1.Read())
                {
                    dataGridView2.Rows[c].Cells[4].Value = obj1["group_id"];
                    dataGridView2.Rows[c].Cells[0].Value = obj1["id"];
                    dataGridView2.Rows[c].Cells[1].Value = obj1["owner_name"];
                    dataGridView2.Rows[c].Cells["Read_Access"].Value = obj1["read_name"];
                    c++;
                }
                conn.Close();
                /*..........................................ACL ENDS....................................................................*/

                groupBox1.Enabled = true;
            }   
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
