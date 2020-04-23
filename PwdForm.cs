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

namespace SFCScanBarcode
{
    public partial class PwdForm : Form
    {
        private SqlConnection conn;
        string sPass = "";
        public PwdForm()
        {
            InitializeComponent();

            string sql = "select password from tblPassword";
            DataTable dt = new DataTable();
            dt = sqlDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                sPass = dt.Rows[0][0].ToString();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == sPass)
                {
                    Form1.pwd = true;
                    textBox1.Text = "";
                    this.Close();                    
                    return;
                }
                MessageBox.Show(this, "MẬT KHẨU KHÔNG ĐÚNG/密码错误");
                textBox1.Text = "";
            }
        }

        private void PwdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Form1.pwd)
            {
                e.Cancel = true;
            }
            else
                e.Cancel = false;
        }
        public DataTable sqlDataTable(string sql)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            DataTable dt = new DataTable();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            cmd.Connection = GetConnection();
            sda.SelectCommand = cmd;
            try
            {
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Dispose();
                sda.Dispose();
                SqlClose();
            }
            return dt;
        }
        public void Open()
        {
            string strConn = "Data Source = 10.8.145.155; Initial Catalog = CentralDatabase; User ID = o-sfc; Password = o-sfc.aac";
            try
            {
                conn = new SqlConnection(strConn);
                conn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public SqlConnection GetConnection()
        {
            Open();
            return conn;
        }
        public void SqlClose()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
            {
                conn.Dispose();
                conn.Close();
            }
        }
    }
}
