using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using Microsoft.VisualBasic.FileIO;

namespace bambino
{
    public partial class Form1 : Form
    {
        public MySqlConnection conn;
        public MySqlCommand cmd;
        public int rows_inserted = 0;
        public decimal status_percentage = 0;
        public decimal total_files = 0;
        //public MySqlDataReader reader;

        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitDatabase();
        }
        public void ChooseFolder()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                button2.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChooseFolder();

        }

        private void textbox1_Change(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false)
            {
                Console.WriteLine("Started");
                button2.Enabled = false;
                button3.Enabled = true;
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Something went wrong. Please Restart Bambino");
            }
        }

        private void ParseCSV(string path)
        {
            List<string> columns = new List<string>();
            int count = 1;
            TextFieldParser parser = new TextFieldParser(path);
            parser.HasFieldsEnclosedInQuotes = true;
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                //Process row
                string[] fields = parser.ReadFields();

                if (count == 1)
                {
                    //Process CSV Headers
                    foreach (string field in fields)
                    {
                        string has_space = field.Replace(" ", "_");
                        string no_dash = has_space.Replace("-", "_");
                        string no_slash = no_dash.Replace("/", "_");
                        columns.Add(no_slash);
                    }
                }
                else
                {
                    if (columns.ToArray().Length == 422)
                    {
                        InsertData(columns.ToArray(), fields);
                    }
                }

                count += 1;
            }
            parser.Close();
        }

        public bool InitDatabase()
        {

            bool connected = false;
            String connectionString = "SERVER=<yourserver-ip-or-name>;PORT=3306;DATABASE=<your-db-name>;UID=<your-db-user>;PASSWORD=<your-db-password>;convert zero datetime=True;";
          
            try
            {
                conn = new MySqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                connected = true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
            }
            return connected;
        }

        public void InsertData(string[] columns, string[] values)
        {
            string sql_statement = "INSERT INTO your_data(" + string.Join(",", columns) + @") VALUES(@" + string.Join(",@", columns) + @")";
            cmd = new MySqlCommand(sql_statement, conn);
            string field_value = "";
            for (int i = 0; i < columns.Length; i++)
            {
                //Console.WriteLine("{0} {1}", i,values.Length);
                field_value = i < values.Length ? values[i] : "";
                cmd.Parameters.AddWithValue("@" + columns[i], field_value);
            }
            string result = "";
            int affected_rows = cmd.ExecuteNonQuery();
            if (affected_rows > 0)
            {
                rows_inserted += affected_rows;
            }
            else
            {
                result = "Something went wrong when inserting " + values[0];
            }
        }

        private void ShowProcess(string path)
        {
            string current_process = "Processing " + path;
            if (label4.InvokeRequired)
            {
                label4.Invoke(new MethodInvoker(() => label4.Text = current_process));
            }
            else
            {
                label4.Text = current_process;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] filePaths = Directory.GetFiles(textBox1.Text, "*.csv");
            decimal completed = 0;
            total_files = filePaths.Length;
            foreach (string path in filePaths)
            {
                ShowProcess(path);
                ParseCSV(path);
                completed += 1;
                status_percentage = (completed / total_files) * 100;
                //Delete file after processed
                File.Delete(path);
                //send progress report to background worker
                backgroundWorker1.ReportProgress(Decimal.ToInt32(status_percentage));
                // Simulate long task
                System.Threading.Thread.Sleep(100);
            }
            string result = "Total Affected Rows: " + rows_inserted.ToString();

            if (label2.InvokeRequired)
            {
                label2.Invoke(new MethodInvoker(() => label2.Text = result));
            }
            else
            {
                label2.Text = result;
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage);
            this.progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                label3.Text = "Process Canceled!";
            }
            else if (e.Error != null)
            {
                label3.Text = "Error: " + e.Error.Message;
            }
            else
            {
                label3.Text = "Process Completed";
                label4.Text = "";
                button3.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                Console.WriteLine("Cancelled");
                // Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
            }
        }
    }
}
