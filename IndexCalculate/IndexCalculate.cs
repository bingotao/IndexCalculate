using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndexCalculate
{
    public partial class indexCalculate : Form
    {
        private string s_databasePath = string.Empty;
        private string s_outputPath = string.Empty;
        private DataTable dt_timeScale = null;
        private DataTable dt_timeLag = null;

        private string sSQLGetZD = "select * from ZDLocation";
        private string sSQLGetZDValue = "select * from IndexValues";
        private string sConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};User Id=admin;Password=;";

        public indexCalculate()
        {
            InitializeComponent();
            BindTable();
        }

        private void BindTable()
        {
            this.dt_timeScale = new DataTable();
            DataColumn dcMin = new DataColumn("min", typeof(int));
            DataColumn dcMax = new DataColumn("max", typeof(int));
            DataColumn dcInteral = new DataColumn("interal", typeof(int));
            this.dt_timeScale.Columns.Add(dcMin);
            this.dt_timeScale.Columns.Add(dcMax);
            this.dt_timeScale.Columns.Add(dcInteral);
            this.dg_timeScaleRanage.DataSource = dt_timeScale;

            foreach (DataGridViewColumn dc in this.dg_timeScaleRanage.Columns)
            {
                dc.Width = 60;
            }

            this.dt_timeLag = new DataTable();
            DataColumn dcMin2 = new DataColumn("min", typeof(int));
            DataColumn dcMax2 = new DataColumn("max", typeof(int));
            DataColumn dcInteral2 = new DataColumn("interal", typeof(int));
            this.dt_timeLag.Columns.Add(dcMin2);
            this.dt_timeLag.Columns.Add(dcMax2);
            this.dt_timeLag.Columns.Add(dcInteral2);
            this.dg_timeLagRanage.DataSource = dt_timeLag;

            foreach (DataGridViewColumn dc in this.dg_timeLagRanage.Columns)
            {
                dc.Width = 60;
            }
        }


        private void btn_openDatabase_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sFileName = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(sFileName);
                if (fileInfo.Extension != ".mdb")
                {
                    MessageBox.Show("文件类型不正确，请选择mdb文件！");
                }
                else
                {
                    this.s_databasePath = sFileName;
                    this.lb_databasePath.Text = sFileName;
                }
            }
        }

        private void btn_chooseOutputPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowerDialog = new FolderBrowserDialog();
            folderBrowerDialog.ShowNewFolderButton = true;
            if (folderBrowerDialog.ShowDialog() == DialogResult.OK)
            {
                string sOutputPath = folderBrowerDialog.SelectedPath;
                this.s_outputPath = sOutputPath;
                this.lb_outputPath.Text = sOutputPath;
            }
        }

        private void btn_create_Click(object sender, EventArgs e)
        {
            Condtions conditions = this.GetConditions();
            bool b = true;
            string sErrorMsg = string.Empty;
            //if (string.IsNullOrEmpty(condition.OutputFolder))
            //{
            //    sErrorMsg = "输出路径尚未设置！";
            //}
            //else 
            if (string.IsNullOrEmpty(conditions.MDBPath))
            {
                sErrorMsg = "数据库文件尚未选择！";
            }
            else if (conditions.Type == MatrType.None)
            {
                sErrorMsg = "尚未选择任何类型！";
            }


            if (sErrorMsg.Length != 0)
            {
                MessageBox.Show(sErrorMsg);
                return;
            }

            Test(conditions);
        }

        private void Test(Condtions condtions)
        {
            string sConStr = string.Format(this.sConnectionString, condtions.MDBPath);
            OleDbDataAdapter dbAdapter1 = new OleDbDataAdapter(this.sSQLGetZD, sConStr);
            DataTable ZDLocations = new DataTable();
            dbAdapter1.Fill(ZDLocations);
            OleDbDataAdapter dbAdapter2 = new OleDbDataAdapter(this.sSQLGetZDValue, sConStr);
            DataTable ZDValues = new DataTable();
            dbAdapter2.Fill(ZDValues);
            DataTable dtTimelagRange = condtions.TimelagRange;
            foreach (DataRow drTimelagRagRange in dtTimelagRange.Rows)
            {




            }
        }


        private Condtions GetConditions()
        {
            Condtions conditions = new Condtions();
            conditions.OutputFolder = this.s_outputPath;
            conditions.MDBPath = this.s_databasePath;
            conditions.TimescaleRange = this.dt_timeScale;
            conditions.TimelagRange = this.dt_timeLag;
            conditions.Type = GetMatrType();
            conditions.Lambda = (double)this.nudLambda.Value;
            return conditions;
        }

        private MatrType GetMatrType()
        {
            MatrType Type = MatrType.None;
            bool cCkd = this.cb_type_complete.Checked;
            bool mCkd = this.cb_type_mixture.Checked;
            if (cCkd && mCkd)
            {
                Type = MatrType.Both;
            }
            else if (cCkd)
            {
                Type = MatrType.Complete;
            }
            else if (mCkd)
            {
                Type = MatrType.Mixture;
            }
            return Type;
        }
    }

    public enum MatrType
    {
        None = -1, Complete = 0, Mixture = 1, Both = 2
    }

    public class Condtions
    {
        public string OutputFolder = string.Empty;
        public string MDBPath = string.Empty;
        public DataTable TimescaleRange = null;
        public DataTable TimelagRange = null;
        public MatrType Type = MatrType.Complete;
        public double Lambda = 0;
    }
}
