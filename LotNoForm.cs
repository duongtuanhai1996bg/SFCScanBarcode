﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SFCScanBarcode
{
    public partial class LotNoForm : Form
    {
        Form frm;
        TextBox tb;
        ToolStripStatusLabel lblLotNo;
        ToolStripStatusLabel lblLotQty;
        ToolStripStatusLabel lblCurrentQty;
        ToolStripStatusLabel lblOK;
        ListView lv;
        string moDel;
        public LotNoForm(Form Frm, TextBox Tb, ToolStripStatusLabel LblLotNo, ToolStripStatusLabel LblLotQty, ToolStripStatusLabel LblCurrentQty, ToolStripStatusLabel LblOK, ListView LV, string model)
        {
            InitializeComponent();
            frm = Frm;
            tb = Tb;
            lblLotNo = LblLotNo;
            lblLotQty = LblLotQty;
            lblCurrentQty = LblCurrentQty;
            lblOK = LblOK;
            lv = LV;
            moDel = model;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            LotNoSetup();
        }
        public void LotNoSetup()
        {
            ulong number = 0;
            if (txtLotName.Text == "" && !ulong.TryParse(txtLotQty.Text.Trim(), out number))
            {
                MessageBox.Show(this, "DỮ LIỆU NHẬP VÀO KHÔNG ĐÚNG/输入数据错误");
                return;
            }
            //Check LotNo
            
            string sql = string.Format("select ID, Qty, FinishQty, IsFinish, PassQty from tblLotNo where LotName='{0}'", txtLotName.Text.Trim());
            DataTable dt = new DataTable();
            try
            {
                dt = SqlConn.sqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    int iQty = Convert.ToInt32(dt.Rows[0]["Qty"]);
                    int iFinishQty = Convert.ToInt32(dt.Rows[0]["FinishQty"]);
                    int iPassQty = Convert.ToInt32(dt.Rows[0]["PassQty"]);
                    int iIsFinish = Convert.ToInt32(dt.Rows[0]["IsFinish"]);
                    if (iIsFinish == 1)
                    {
                        MessageBox.Show(this, "LÔ NÀY ĐÃ ĐẦY/此批次已满");
                        return;
                    }
                    lblLotQty.Text = iQty.ToString();
                    lblCurrentQty.Text = iFinishQty.ToString();
                    lblOK.Text = iPassQty.ToString();                    
                }
                else if (dt.Rows.Count <= 0)
                {
                    lblLotQty.Text = txtLotQty.Text.Trim();
                    lblCurrentQty.Text = "0";
                    lblOK.Text = "0";
                    string sInsert = string.Format("insert into tblLotNo ([LotName],[Qty],[FinishQty],[PassQty],[IsFinish],[BeginTime]) values ('{0}', {1}, {2}, {3}, {4}, '{5}')", txtLotName.Text.Trim(), Convert.ToInt32(txtLotQty.Text.Trim()), 0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    try
                    {
                        SqlConn.ExecuteNonSql(sInsert);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(this, "Insert LotNo Fail: " + ex.Message);
                        return;
                    }
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(this, "Check LotNo Fail: " + ex2.Message);
            }
            //YangJunHai add check lot format 20200604
            //MessageBox.Show(moDel);
            string sqllot = string.Format("select ProductCode from T_SnFormat WITH(NOLOCK) where ProductName='{0}'", moDel);
            DataTable dtlot = new DataTable();
            try
            {
                dtlot = SqlConn.sqlDataTable(sqllot);
                string lot = dtlot.Rows[0]["ProductCode"].ToString();
                //string newlotname = lot.Replace("*", "");
                // string[] listlotname = lot.Split('*');
                // string newlotname = listlotname[0];
                if (txtLotName.Text.Contains(lot))
                {
                    //MessageBox.Show("OKKKKKKKKKKKLA");
                    lblLotNo.Text = txtLotName.Text.Trim();
                    tb.Focus();
                    lv.Items.Clear();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("SAI SỐ LÔ / 批次号错误！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "CheckSN connect Fail: " + ex.Message);
                return;
            }
            //Hai add end
        }

        private void txtLotQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LotNoSetup();
            }
        }
    }
}
