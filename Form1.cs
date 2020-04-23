using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading;

namespace SFCScanBarcode
{
    public partial class Form1 : Form
    {        
        public static bool pwd = true;
        int iResult = 0;
        string SNFormat="";
        string newFormat = "";
        public Form1()
        {
            InitializeComponent();
        }        

        private void Form1_Load(object sender, EventArgs e)
        {
            //Check program is running
            var exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
            if (exists.Length > 1)
            {
                MessageBox.Show("Program Already Running", "MSG");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }
            //Finish
            try
            {
                ProjectLoad();
            }
            catch
            {
                MessageBox.Show("Can not connect to database");
                return;
            }
        }

        public void ProjectLoad()
        {
            SqlConn.GetDbName = "CentralDatabase";
            //get Station
            string sql2 = "select ID, [StationName] from [tblStation]";
            DataTable dt2 = new DataTable();
            try
            {
                dt2 = SqlConn.sqlDataTable(sql2);
                if (dt2.Rows.Count > 0)
                {
                    cmbStation.DataSource = dt2;
                    cmbStation.DisplayMember = "StationName";
                    cmbStation.ValueMember = "ID";
                    cmbStation.SelectedIndex = -1;
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(this, "Load Station Fail: " + ex2.Message);
                return;
            }
            //Load project
            string sql = "select [Description] from [T_DataBaseCfg]";
            DataTable dt = new DataTable();
            try
            {
                dt = SqlConn.sqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    cmbProject.DataSource = dt;
                    cmbProject.DisplayMember = "Description";
                    cmbProject.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Load Project Fail: " + ex.Message);
                return;
            }            
        }        
        private void cmbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtScan.Enabled = false;
            if (cmbProject.Text.Trim() != "" && cmbProject.Text.Trim() != "System.Data.DataRowView")
            {
                SqlConn.GetDbName = "SFC" + cmbProject.Text;
                string sql = "select ID, ProductName from T_SnFormat";
                DataTable dt = new DataTable();
                try
                {
                    dt = SqlConn.sqlDataTable(sql);
                    //if (dt.Rows.Count > 0)
                    {                        
                        cmbModel.DataSource = dt;                        
                        cmbModel.DisplayMember = "ProductName";
                        cmbModel.ValueMember = "ID";
                        cmbModel.SelectedIndex = -1;                        
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, "Load Model Fail: " + ex.Message);
                }
                //if (cmbProject.Text.Trim() == "SAMSUNG" || cmbProject.Text.Trim() == "GOOGLE")
                //{
                //    cbLQ.Visible = true;
                //    cbLQ.Checked = true;
                //}
                //else
                //    cbLQ.Visible = false;
            }
        }

        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbProject.Text == "")
            {
                MessageBox.Show(this, "CHƯA CHỌN TÊN HÀNG/项目为空");
                return;
            }
            if(cmbModel.Text.Trim() != "" && cmbModel.Text.Trim() != "System.Data.DataRowView")
            {
                string sql = string.Format("select ValidateFormat from T_SnFormat where ProductName='{0}'", cmbModel.Text.Trim());
                try
                {
                    DataTable dt = new DataTable();
                    dt = SqlConn.sqlDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        SNFormat = dt.Rows[0][0].ToString();
                        //MessageBox.Show(ValidateFormat);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ValidateFormat get Fail: " + ex.Message);
                    return;
                }
                //-----------------------------------------------------------------------
                newFormat = "";
                if (SNFormat.Contains(".{"))
                {
                    newFormat = SNFormat.Replace(".{", @"\w{");
                }
                else
                {
                    
                    string[] kt = new string[SNFormat.Length];
                    int count = 0;
                    for (int i = 0; i < SNFormat.Length; i++)
                    {
                        kt[i] = SNFormat.Substring(i, 1);
                        if (kt[i] == "*")
                        {
                            count = count + 1;
                        }
                        else
                        {
                            if (count > 0)
                            {
                                newFormat = newFormat + @"\w{" + count + "}" + kt[i];
                                count = 0;
                            }
                            else
                            {
                                newFormat = newFormat + kt[i];
                            }
                        }
                    }
                    if (count > 0)
                    {
                        newFormat = newFormat + @"\w{" + count + "}";
                        count = 0;
                    }
                }
                //MessageBox.Show(newFormat);
                //--------------------------------------------------------------------------

                if (cmbStation.Text != "" && cbType.Text.Trim() != "")
                {                    
                    txtScan.Enabled = true;
                    txtScan.Focus();
                }
            }            
        }

        private void cmbStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtScan.Enabled == false && cmbProject.Text.Trim() != "" && cmbModel.Text.Trim() != "" && cbType.Text.Trim() != "")
            {
                txtScan.Enabled = true;
                txtScan.Focus();                
            }
        }

        public void ResultStatus(string status)
        {
            txtScan.Enabled = true;
            txtScan.Focus();
            if (status.Contains("PASS"))
            {
                iResult = 1;
                label4.BackColor = Color.Lime;
                label4.Text = txtScan.Text.Trim() + ":OK";
                label4.ForeColor = Color.Blue;
                cmbModel.Enabled = true;
                cmbProject.Enabled = true;
                cmbStation.Enabled = true;
                lvDetail.Items.Add(txtScan.Text.Trim());
                lvDetail.EnsureVisible(lvDetail.Items.Count - 1);
                txtScan.Clear();
                //iCurrentRun();
            }
            else if (status.Contains("FAIL"))
            {
                SoundPlayer player = new SoundPlayer(System.IO.Directory.GetCurrentDirectory() + @"\Motorola Startac 9.wav");
                player.Play();
                iResult = 0;
                label4.BackColor = Color.Red;
                //label4.Text = "FAIL";
                label4.ForeColor = Color.White;
                cmbModel.Enabled = true;
                cmbProject.Enabled = true;
                cmbStation.Enabled = true;
                if (txtScan.Text.Trim() != "aac@2018")
                {
                    lvDetail.Items.Add(txtScan.Text.Trim());
                    lvDetail.Items[lvDetail.Items.Count - 1].ForeColor = Color.Red;
                    lvDetail.EnsureVisible(lvDetail.Items.Count - 1);
                }
                txtScan.Clear();
            }
            else if (status.Contains("RUN"))
            {
                label4.BackColor = Color.Yellow;
                label4.Text = "RUN";
                label4.ForeColor = Color.Black;
                cmbModel.Enabled = false;
                cmbProject.Enabled = false;
                cmbStation.Enabled = false;
            }
            else
            {
                label4.BackColor = Color.Aqua;
                label4.Text = "Standby";
                label4.ForeColor = Color.Navy;
                cmbModel.Enabled = true;
                cmbProject.Enabled = true;
                cmbStation.Enabled = true;
                txtScan.Clear();
            }
        }

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtScan.Text.Trim() != "")
            {
                SnCheck();
            }
        }
        public void SnCheck()
        {
            if (rTX.Checked)
            {
                SqlConn.GetDbName = "SFC" + cmbProject.Text;//version 1.0.5 added
                /*//Check SN Format                          
                //Thread.Sleep(300);
                if (txtScan.Text.Trim().Length != SNFormat.Length || !Regex.IsMatch(txtScan.Text.Trim().Replace(" ", "A"), newFormat))
                {
                    MessageBox.Show("NG");
                    txtScan.Clear();
                }
                else
                {
                    MessageBox.Show("OK");
                    txtScan.Clear();
                    return;
                }

                return;*/

                if (cmbProject.Text == "" || cmbModel.Text == "" || cmbStation.Text == "")
                {
                    MessageBox.Show(this, "DỮ LIỆU CÀI ĐẶT TRỐNG/设置数据为空");
                    ResultStatus("STANDBY");
                    return;
                }
                if (toolStripStatusLabel2.Text.Contains("未设定"))
                {
                    MessageBox.Show("CHƯA CÀI ĐẶT SỐ LÔ/批次号未设置");
                    ResultStatus("STANDBY");
                    return;
                }
                string sn = txtScan.Text.Trim();
                int cQty = Convert.ToInt32(toolStripStatusLabel6.Text.Trim());
                int pQty = Convert.ToInt32(toolStripStatusLabel8.Text.Trim());
                if (cQty >= Convert.ToInt32(toolStripStatusLabel4.Text.Trim()))
                {
                    MessageBox.Show("LÔ NÀY ĐÃ ĐẦY/此批次已满");
                    ResultStatus("STANDBY");
                    txtScan.Enabled = false;
                    return;
                }
                ResultStatus("RUN");
                //Check SN Format
                //if (sn.Length != SNFormat.Length || !Regex.IsMatch(sn.Replace(" ", "A"), newFormat))

                //Haicomment
                //if (!Regex.IsMatch(sn.Replace(" ", "A"), newFormat))
               // {                  
                   // ResultStatus("FAIL");
                   // label4.Text = sn + ":ĐỊNH DẠNG SN SAI/SN规则错误";
                  //  PwdConfirm();
                   // return;
                //}
                //haiendcomment

                //Check tblData Result
                string sCheckResult = string.Format("select Result from tblData WITH(NOLOCK) where SN='{0}' and StationID={1} order by CreateTime desc", sn, cmbStation.SelectedValue);
                DataTable dt1 = new DataTable();
                try
                {
                    dt1 = SqlConn.sqlDataTable(sCheckResult);
                }
                catch (Exception ex1)
                {
                    MessageBox.Show("Can not check Data: " + ex1.Message);
                    return;
                }
                if (dt1.Rows.Count > 0)
                {
                    bool tblDataResult = bool.Parse(dt1.Rows[0][0].ToString());
                    if (tblDataResult)
                    {
                        label4.Text = sn + ": TRÙNG MÃ/重码，请隔离";
                        ResultStatus("FAIL");
                        PwdConfirm();
                        return;
                    }
                }

                // if ((cmbProject.Text.Trim() == "SAMSUNG" || cmbProject.Text.Trim() == "GOOGLE") && cbLQ.Checked)//version 1.0.5 added
                if (cbLQ.Checked)
                {
                    //check TQ test result
                    SqlConn.GetDbName = "sfcRelay";
                    string TQCheck = string.Format("Select top 1 Result From T_SnCollection Where SN = '{0}' and Station='漏气测试'  order by ID desc", sn);
                    DataTable dt2 = new DataTable();
                    try
                    {
                        dt2 = SqlConn.sqlDataTable(TQCheck);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "CheckSN TQ connect Fail: " + ex.Message);
                        return;
                    }
                    if (dt2.Rows.Count == 0)
                    {
                        label4.Text = sn + ":CHƯA TEST RÒ KHÍ/漏气未测试";
                        ResultStatus("FAIL");
                        PwdConfirm();
                        return;
                    }
                    else if (dt2.Rows.Count > 0)
                    {
                        string sResult = dt2.Rows[0]["Result"].ToString();
                        if (sResult != "OK")
                        {
                            label4.Text = sn + ":RÒ KHÍ TEST NG/漏气测试NG";
                            ResultStatus("FAIL");
                            PwdConfirm();
                            return;
                        }
                    }
                    SqlConn.GetDbName = "SFC" + cmbProject.Text;
                }
                //Test Ang-ten
                if (cbAT.Checked)
                {
                    //check TQ test result
                    //SqlConn.GetDbName = "sfcRelay";
                    string TQCheck = string.Format("select IsPass, PassLevel from T_SN_Test WITH(NOLOCK) where SN='{0}' and IsRepair = '4' order by [Time] desc", sn);
                    DataTable dt2 = new DataTable();
                    try
                    {
                        dt2 = SqlConn.sqlDataTable(TQCheck);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "CheckSN TQ connect Fail: " + ex.Message);
                        return;
                    }
                    if (dt2.Rows.Count == 0)
                    {
                        label4.Text = sn + ":CHƯA TEST ĂNG-TEN/天线未测试";
                        ResultStatus("FAIL");
                        PwdConfirm();
                        return;
                    }
                    else if (dt2.Rows.Count > 0)
                    {
                        string sResult = dt2.Rows[0]["PassLevel"].ToString();
                        if (sResult != "OK")
                        {
                            label4.Text = sn + ":ĂNG-TEN TEST NG/天线测试NG";
                            ResultStatus("FAIL");
                            PwdConfirm();
                            return;
                        }
                    }
                    SqlConn.GetDbName = "SFC" + cmbProject.Text;
                }
                //check NI test result                
                string sqlCheck = string.Format("select IsPass, PassLevel from T_SN_Test WITH(NOLOCK) where SN='{0}' and IsRepair = '3' order by [Time] desc", sn);
                //YangJunHai add for check xing neng times 20200408
                string sqlCheck2 = string.Format("select TestTime from T_TestTimeBySN WITH(NOLOCK) where SN='{0}'", sn);
                DataTable dt = new DataTable();
                DataTable dttesttime = new DataTable();
                try
                {
                    dt = SqlConn.sqlDataTable(sqlCheck);
                    dttesttime = SqlConn.sqlDataTable(sqlCheck2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "CheckSN connect Fail: " + ex.Message);
                    return;
                }
                if (dt.Rows.Count == 0)
                {
                    label4.Text = sn + ":CHƯA TEST TÍNH NĂNG/性能未测试";
                    ResultStatus("FAIL");
                    PwdConfirm();
                    return;
                }
                else if (dt.Rows.Count > 0)
                {
                    int iResult = int.Parse(dt.Rows[0]["IsPass"].ToString());
                    int testTime = int.Parse(dttesttime.Rows[0]["TestTime"].ToString());
                    if (cbType.Text.Trim() == "A")
                    {
                        if (testTime == 1)
                        {
                            if (iResult == 1)
                            {
                                if (cbType.Text.Trim() != "") // 检查产品的档位
                                {
                                    string pType = dt.Rows[0]["PassLevel"].ToString();
                                    //int i = pType.LastIndexOf("_") + 1;
                                    //pType = pType.Substring(i, pType.LastIndexOf(".txt") - i);
                                    //MessageBox.Show(pType);   
                                    if (pType.Trim() != cbType.Text.Trim())
                                    {
                                        label4.Text = sn + ":LOẠI HÀNG KHÔNG ĐÚNG/产品的档位错误 -- " + pType;
                                        ResultStatus("FAIL");
                                        PwdConfirm();
                                        return;
                                    }
                                }
                                pQty++;
                                toolStripStatusLabel8.Text = pQty.ToString();
                                ResultStatus("PASS");
                            }
                            else
                            {
                                label4.Text = sn + ":TÍNH NĂNG TEST NG/性能测试NG";
                                ResultStatus("FAIL");
                                PwdConfirm();
                                return;
                            }
                        }
                        if (testTime > 1)
                        {
                            if (iResult == 1)
                            {
                                if (cbType.Text.Trim() != "") // 检查产品的档位
                                {
                                    string pType = dt.Rows[0]["PassLevel"].ToString();
                                    //int i = pType.LastIndexOf("_") + 1;
                                    //pType = pType.Substring(i, pType.LastIndexOf(".txt") - i);
                                    //MessageBox.Show(pType);   
                                    if (pType.Trim() != cbType.Text.Trim())
                                    {
                                        label4.Text = sn + ":LOẠI HÀNG KHÔNG ĐÚNG/产品的档位错误 -- " + pType;
                                        ResultStatus("FAIL");
                                        PwdConfirm();
                                        return;
                                    }
                                }
                                pQty++;
                                toolStripStatusLabel8.Text = pQty.ToString();
                                label4.Text = sn + ":TÍNH NĂNG TEST NG/性能测试NG";
                                ResultStatus("FAIL");
                                PwdConfirm();
                                return;
                            }
                            else
                            {
                                label4.Text = sn + ":TÍNH NĂNG TEST NG/性能测试NG";
                                ResultStatus("FAIL");
                                PwdConfirm();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (iResult == 1)
                        {
                            if (cbType.Text.Trim() != "") // 检查产品的档位
                            {
                                string pType = dt.Rows[0]["PassLevel"].ToString();
                                //int i = pType.LastIndexOf("_") + 1;
                                //pType = pType.Substring(i, pType.LastIndexOf(".txt") - i);
                                //MessageBox.Show(pType);   
                                if (pType.Trim() != cbType.Text.Trim())
                                {
                                    label4.Text = sn + ":LOẠI HÀNG KHÔNG ĐÚNG/产品的档位错误 -- " + pType;
                                    ResultStatus("FAIL");
                                    PwdConfirm();
                                    return;
                                }
                            }
                            pQty++;
                            toolStripStatusLabel8.Text = pQty.ToString();
                            ResultStatus("PASS");
                        }
                        else
                        {
                            label4.Text = sn + ":TÍNH NĂNG TEST NG/性能测试NG";
                            ResultStatus("FAIL");
                            PwdConfirm();
                            return;
                        }
                    }
                }
                cQty++;
                toolStripStatusLabel6.Text = cQty.ToString();
                //update pici  
                string sqlUpdate = "";
                if (cQty == Convert.ToInt32(toolStripStatusLabel4.Text.Trim()))
                {
                    sqlUpdate = string.Format("update tblLotNo set FinishQty={0}, PassQty={1}, EndTime='{2}' where LotName='{3}'", cQty, pQty, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), toolStripStatusLabel2.Text);
                }
                else
                {
                    sqlUpdate = string.Format("update tblLotNo set FinishQty={0}, PassQty={1} where LotName='{2}'", cQty, pQty, toolStripStatusLabel2.Text);
                }
                try
                {
                    SqlConn.ExecuteNonSql(sqlUpdate);
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(this, "Update LotNo Fail: " + ex2.Message);
                    return;
                }
                //insert Data
                string detail = label4.Text;
                if (detail.Contains("OK"))
                    detail = "OK";
                string sqlInsert = string.Format("insert into tblData (SN, Result, StationID, ModelID, CreateTime, Detail) values ('{0}', {1}, {2}, {3}, '{4}', N'{5}')", sn, iResult, cmbStation.SelectedValue, cmbModel.SelectedValue, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), detail);
                try
                {
                    SqlConn.ExecuteNonSql(sqlInsert);
                }
                catch (Exception ex3)
                {
                    MessageBox.Show(this, "Insert SN Fail: " + ex3.Message);
                    return;
                }
            }
            else if (rdAT.Checked)
            {
                SqlConn.GetDbName = "SFC" + cmbProject.Text;//version 1.0.5 added
                /*//Check SN Format                          
                //Thread.Sleep(300);
                if (txtScan.Text.Trim().Length != SNFormat.Length || !Regex.IsMatch(txtScan.Text.Trim().Replace(" ", "A"), newFormat))
                {
                    MessageBox.Show("NG");
                    txtScan.Clear();
                }
                else
                {
                    MessageBox.Show("OK");
                    txtScan.Clear();
                    return;
                }

                return;*/

                if (cmbProject.Text == "" || cmbModel.Text == "" || cmbStation.Text == "")
                {
                    MessageBox.Show(this, "DỮ LIỆU CÀI ĐẶT TRỐNG/设置数据为空");
                    ResultStatus("STANDBY");
                    return;
                }
                if (toolStripStatusLabel2.Text.Contains("未设定"))
                {
                    MessageBox.Show("CHƯA CÀI ĐẶT SỐ LÔ/批次号未设置");
                    ResultStatus("STANDBY");
                    return;
                }
                string sn = txtScan.Text.Trim();
                int cQty = Convert.ToInt32(toolStripStatusLabel6.Text.Trim());
                int pQty = Convert.ToInt32(toolStripStatusLabel8.Text.Trim());
                if (cQty >= Convert.ToInt32(toolStripStatusLabel4.Text.Trim()))
                {
                    MessageBox.Show("LÔ NÀY ĐÃ ĐẦY/此批次已满");
                    ResultStatus("STANDBY");
                    txtScan.Enabled = false;
                    return;
                }
                ResultStatus("RUN");
                //Check SN Format
                //if (sn.Length != SNFormat.Length || !Regex.IsMatch(sn.Replace(" ", "A"), newFormat))
                if (!Regex.IsMatch(sn.Replace(" ", "A"), newFormat))
                {
                    //MessageBox.Show("ĐỊNH DẠNG SN SAI/SN规则错误");                    
                    ResultStatus("FAIL");
                    label4.Text = sn + ":ĐỊNH DẠNG SN SAI/SN规则错误";
                    PwdConfirm();
                    return;
                }
                //Check tblData Result
                string sCheckResult = string.Format("select Result from tblData WITH(NOLOCK) where SN='{0}' and StationID={1} order by CreateTime desc", sn, cmbStation.SelectedValue);
                DataTable dt1 = new DataTable();
                try
                {
                    dt1 = SqlConn.sqlDataTable(sCheckResult);
                }
                catch (Exception ex1)
                {
                    MessageBox.Show("Can not check Data: " + ex1.Message);
                    return;
                }
                if (dt1.Rows.Count > 0)
                {
                    bool tblDataResult = bool.Parse(dt1.Rows[0][0].ToString());
                    if (tblDataResult)
                    {
                        label4.Text = sn + ": TRÙNG MÃ/重码，请隔离";
                        ResultStatus("FAIL");
                        PwdConfirm();
                        return;
                    }
                }
                //Check LQ test result
                if (cbLQ.Checked)
                {
                    //check TQ test result
                    SqlConn.GetDbName = "sfcRelay";
                    string TQCheck = string.Format("Select top 1 Result From T_SnCollection Where SN = '{0}' and Station='漏气测试'  order by ID desc", sn);
                    DataTable dt2 = new DataTable();
                    try
                    {
                        dt2 = SqlConn.sqlDataTable(TQCheck);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "CheckSN TQ connect Fail: " + ex.Message);
                        return;
                    }
                    if (dt2.Rows.Count == 0)
                    {
                        label4.Text = sn + ":CHƯA TEST RÒ KHÍ/漏气未测试";
                        ResultStatus("FAIL");
                        PwdConfirm();
                        return;
                    }
                    else if (dt2.Rows.Count > 0)
                    {
                        string sResult = dt2.Rows[0]["Result"].ToString();
                        if (sResult != "OK")
                        {
                            label4.Text = sn + ":RÒ KHÍ TEST NG/漏气测试NG";
                            ResultStatus("FAIL");
                            PwdConfirm();
                            return;
                        }
                    }
                    SqlConn.GetDbName = "SFC" + cmbProject.Text;
                }
                //check AT test result                
                string sqlCheck = string.Format("select IsPass, PassLevel from T_SN_Test WITH(NOLOCK) where SN='{0}' and IsRepair = '4' order by [Time] desc", sn);
                DataTable dt = new DataTable();
                try
                {
                    dt = SqlConn.sqlDataTable(sqlCheck);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "CheckSN connect Fail: " + ex.Message);
                    return;
                }
                if (dt.Rows.Count == 0)
                {
                    label4.Text = sn + ":CHƯA TEST ĂNG-TEN/天线未测试";
                    ResultStatus("FAIL");
                    PwdConfirm();
                    return;
                }
                else if (dt.Rows.Count > 0)
                {
                    int iResult = int.Parse(dt.Rows[0]["IsPass"].ToString());
                    if (iResult == 1)
                    {
                        if (cbType.Text.Trim() != "") // 检查产品的档位
                        {
                            string pType = dt.Rows[0]["PassLevel"].ToString();
                            //int i = pType.LastIndexOf("_") + 1;
                            //pType = pType.Substring(i, pType.LastIndexOf(".txt") - i);
                            //MessageBox.Show(pType);   
                            if (pType.Trim() != "OK")
                            {
                                label4.Text = sn + ":ĂNG-TEN TEST NG/天线测试NG -- ";
                                ResultStatus("FAIL");
                                PwdConfirm();
                                return;
                            }
                        }
                        pQty++;
                        toolStripStatusLabel8.Text = pQty.ToString();
                        ResultStatus("PASS");
                    }
                    else
                    {
                        label4.Text = sn + ":ĂNG-TEN TEST NG/天线测试NG";
                        ResultStatus("FAIL");
                        PwdConfirm();
                        return;
                    }
                }
                cQty++;
                toolStripStatusLabel6.Text = cQty.ToString();
                //update pici  
                string sqlUpdate = "";
                if (cQty == Convert.ToInt32(toolStripStatusLabel4.Text.Trim()))
                {
                    sqlUpdate = string.Format("update tblLotNo set FinishQty={0}, PassQty={1}, EndTime='{2}' where LotName='{3}'", cQty, pQty, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), toolStripStatusLabel2.Text);
                }
                else
                {
                    sqlUpdate = string.Format("update tblLotNo set FinishQty={0}, PassQty={1} where LotName='{2}'", cQty, pQty, toolStripStatusLabel2.Text);
                }
                try
                {
                    SqlConn.ExecuteNonSql(sqlUpdate);
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(this, "Update LotNo Fail: " + ex2.Message);
                    return;
                }
                //insert Data
                string detail = label4.Text;
                if (detail.Contains("OK"))
                    detail = "OK";
                string sqlInsert = string.Format("insert into tblData (SN, Result, StationID, ModelID, CreateTime, Detail) values ('{0}', {1}, {2}, {3}, '{4}', N'{5}')", sn, iResult, cmbStation.SelectedValue, cmbModel.SelectedValue, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), detail);
                try
                {
                    SqlConn.ExecuteNonSql(sqlInsert);
                }
                catch (Exception ex3)
                {
                    MessageBox.Show(this, "Insert SN Fail: " + ex3.Message);
                    return;
                }
            }
            else
            {
                if (toolStripStatusLabel2.Text.Contains("未设定"))
                {
                    MessageBox.Show("CHƯA CÀI ĐẶT SỐ LÔ/批次号未设置");
                    ResultStatus("STANDBY");
                    return;
                }
                string sn = txtScan.Text.Trim();
                int cQty = Convert.ToInt32(toolStripStatusLabel6.Text.Trim());
                int pQty = Convert.ToInt32(toolStripStatusLabel8.Text.Trim());
                if (cQty >= Convert.ToInt32(toolStripStatusLabel4.Text.Trim()))
                {
                    MessageBox.Show("LÔ NÀY ĐÃ ĐẦY/此批次已满");
                    ResultStatus("STANDBY");
                    txtScan.Enabled = false;
                    return;
                }
                ResultStatus("RUN");
                //Check tblData Result
                string sCheckResult = string.Format("select Result from tblData WITH(NOLOCK) where SN='{0}' order by CreateTime desc", sn);                
                DataTable dt1 = new DataTable();
                try
                {
                    dt1 = SqlConn.sqlDataTable(sCheckResult);
                }
                catch (Exception ex1)
                {
                    MessageBox.Show("Can not check Data: " + ex1.Message);
                    return;
                }
                if (dt1.Rows.Count > 0)
                {
                    bool tblDataResult = bool.Parse(dt1.Rows[0][0].ToString());
                    if (tblDataResult)
                    {
                        label4.Text = sn + ": TRÙNG MÃ/重码，请隔离";
                        ResultStatus("FAIL");
                        PwdConfirm();
                        return;
                    }
                }
                //check TQ test result
                string sqlCheck = string.Format("Select top 1 Result From T_SnCollection Where SN = '{0}' and Station='漏气测试'  order by ID desc", sn);
                DataTable dt = new DataTable();
                try
                {
                    dt = SqlConn.sqlDataTable(sqlCheck);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "CheckSN connect Fail: " + ex.Message);
                    return;
                }
                if (dt.Rows.Count == 0)
                {
                    label4.Text = sn + ":CHƯA TEST RÒ KHÍ/漏气未测试";
                    ResultStatus("FAIL");
                    PwdConfirm();
                    return;
                }
                else if (dt.Rows.Count > 0)
                {
                    string sResult = dt.Rows[0]["Result"].ToString();
                    if (sResult == "OK")
                    {
                        pQty++;
                        toolStripStatusLabel8.Text = pQty.ToString();
                        ResultStatus("PASS");
                    }
                    else
                    {
                        label4.Text = sn + ":RÒ KHÍ TEST NG/漏气测试NG";
                        ResultStatus("FAIL");
                        PwdConfirm();
                        return;
                    }
                }
                cQty++;
                toolStripStatusLabel6.Text = cQty.ToString();
                //update pici  
                string sqlUpdate = "";
                if (cQty == Convert.ToInt32(toolStripStatusLabel4.Text.Trim()))
                {
                    sqlUpdate = string.Format("update tblLotNo set FinishQty={0}, PassQty={1}, EndTime='{2}' where LotName='{3}'", cQty, pQty, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), toolStripStatusLabel2.Text);
                }
                else
                {
                    sqlUpdate = string.Format("update tblLotNo set FinishQty={0}, PassQty={1} where LotName='{2}'", cQty, pQty, toolStripStatusLabel2.Text);
                }
                try
                {
                    SqlConn.ExecuteNonSql(sqlUpdate);
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(this, "Update LotNo Fail: " + ex2.Message);
                    return;
                }
                //insert Data
                string detail = label4.Text;
                if (detail.Contains("OK"))
                    detail = "OK";
                string sqlInsert = string.Format("insert into tblData (SN, Result, CreateTime, Detail) values ('{0}', {1}, '{2}', N'{3}')", sn, iResult, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), detail);
                try
                {
                    SqlConn.ExecuteNonSql(sqlInsert);
                }
                catch (Exception ex3)
                {
                    MessageBox.Show(this, "Insert SN Fail: " + ex3.Message);
                    return;
                }
            }
        }

        private void btnLotNo_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(cmbModel.SelectedValue.ToString());
            LotNoForm lotForm = new LotNoForm(this, txtScan, toolStripStatusLabel2, toolStripStatusLabel4, toolStripStatusLabel6, toolStripStatusLabel8, lvDetail);
            lotForm.StartPosition = FormStartPosition.CenterScreen;
            lotForm.Show();
            lotForm.TopMost = true;            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!pwd)
            {
                e.Cancel = true;
            }
            else
                e.Cancel = false;
        }
        public void PwdConfirm()
        {
            pwd = false;
            PwdForm pwdform = new PwdForm();
            pwdform.StartPosition = FormStartPosition.CenterScreen;
            pwdform.ShowDialog();
            pwdform.TopMost = true;
        }

        private void rTX_CheckedChanged(object sender, EventArgs e)
        {
            if (rTX.Checked || rdAT.Checked)
            {
                cmbProject.Enabled = true;
                cmbModel.Enabled = true;
                cmbStation.Enabled = true;
                cbLQ.Show();
                cbAT.Show();

            }
            else
            {                
                cmbProject.Enabled = false;
                cmbModel.Enabled = false;
                cmbStation.Enabled = false;
                cmbStation.SelectedIndex = -1;
                cmbModel.SelectedIndex = -1;
                cmbProject.SelectedIndex = -1;
                txtScan.Enabled = true;
                cbLQ.Hide();
                cbAT.Hide();
                SqlConn.GetDbName = "sfcRelay";
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtScan.Enabled == false && cmbProject.Text.Trim() != "" && cmbModel.Text.Trim() != "" && cbType.Text.Trim() != "")
            {
                txtScan.Enabled = true;
                txtScan.Focus();
            }
        }

        private void rdAT_CheckedChanged(object sender, EventArgs e)
        {
            if (rTX.Checked || rdAT.Checked)
            {
                cmbProject.Enabled = true;
                cmbModel.Enabled = true;
                cmbStation.Enabled = true;
                cbLQ.Show();
                cbAT.Show();

            }
            else
            {
                cmbProject.Enabled = false;
                cmbModel.Enabled = false;
                cmbStation.Enabled = false;
                cmbStation.SelectedIndex = -1;
                cmbModel.SelectedIndex = -1;
                cmbProject.SelectedIndex = -1;
                txtScan.Enabled = true;
                cbLQ.Hide();
                cbAT.Hide();
                SqlConn.GetDbName = "sfcRelay";
            }
        }
    }
}
