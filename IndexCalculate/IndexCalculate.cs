using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Cells;
using MathNet.Numerics;

namespace IndexCalculate
{
    public partial class indexCalculate : Form
    {
        private string s_databasePath = string.Empty;
        private string s_outputPath = string.Empty;
        private DataTable dt_timeScale = null;
        private DataTable dt_timeLag = null;
        private SpatialMatrixType Type = SpatialMatrixType.Complete;
        private string sSQLGetZD = "select * from ZDLocation";
        private string sSQLGetZDValue = "select * from IndexValues";
        private string sConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};User Id=admin;Password=;";
        BackgroundWorker bw = null;

        public indexCalculate()
        {
            InitializeComponent();
            BindTable();
            SetDefaultPath();
            InitBackgroundWorker();
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
                dc.Width = 50;
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
                dc.Width = 50;
            }
        }

        private void SetDefaultPath()
        {
            string sCurrentPath = Environment.CurrentDirectory;
            string[] files = Directory.GetFiles(sCurrentPath, "*.mdb");
            if (files.Length > 0)
            {
                string sFile = files[0];
                string sFileName = Path.Combine(sCurrentPath, sFile);
                this.s_databasePath = sFileName;
                this.lb_databasePath.Text = sFileName;
            }
            this.s_outputPath = sCurrentPath;
            this.lb_outputPath.Text = sCurrentPath;
        }

        private void InitBackgroundWorker()
        {
            this.bw = new BackgroundWorker();
            this.bw.WorkerSupportsCancellation = true;
            this.bw.WorkerReportsProgress = true;
            this.bw.DoWork += new DoWorkEventHandler(Execute);
            this.bw.ProgressChanged += new ProgressChangedEventHandler(Progress);
            this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(End);
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

        private void Execute(object sender, DoWorkEventArgs e)
        {
            try
            {
                Condtions conditions = this.GetConditions();
                string sErrorMsg = string.Empty;
                if (conditions.MDBPath == string.Empty)
                {
                    sErrorMsg = "尚未设置数据源！";
                }
                else if (conditions.TimelagRange.Rows.Count == 0)
                {
                    sErrorMsg = "尚未设置TimeLag！";
                }
                else if (conditions.TimescaleRange.Rows.Count == 0)
                {
                    sErrorMsg = "尚未设置TimeScale！";
                }
                if (sErrorMsg != string.Empty)
                {
                    this.bw.ReportProgress(0, sErrorMsg);
                }
                else
                {
                    DateTime begin = DateTime.Now;
                    Caculate(conditions);
                    DateTime end = DateTime.Now;
                    TimeSpan timeSpan = end - begin;
                    this.bw.ReportProgress(0, string.Format("共耗时：{0}", timeSpan));
                }
            }
            catch (System.Exception ex)
            {
                this.bw.ReportProgress(0, ex.Message);
            }
        }

        private void Progress(object sender, ProgressChangedEventArgs e)
        {
            this.lbMessage.Text = e.UserState.ToString();
        }

        private void End(object sender, AsyncCompletedEventArgs e)
        {
            if (e.UserState != null)
            {
                this.lbMessage.Text = e.UserState.ToString();
            }
            this.btn_create.Enabled = true;
            this.btnGetMoran.Enabled = true;
        }

        private void btn_create_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            Cells cells = sheet.Cells;
            double[] dArray = ARFIMASeriesGenerator.CreateNormalSeries(139.63, 103.96, 1000);
            double[] dARDIMA = ARFIMASeriesGenerator.CreateARFIMASeries(dArray, 0.1);

            for (int i = 0; i < dARDIMA.Length; i++)
            {
                cells[i, 0].PutValue(dARDIMA[i]);
            }

            string sOutPath = Path.Combine(Environment.CurrentDirectory, string.Format("test.xls"));
            workbook.Save(sOutPath);
        }

        private void Caculate(Condtions condtions)
        {
            this.bw.ReportProgress(0, string.Format("正在准备数据..."));

            DataTable dtTimeLagRange = condtions.TimelagRange;
            DataTable dtTimeScaleRange = condtions.TimescaleRange;
            List<int> listTimeLag = GetRangeList(dtTimeLagRange);
            List<int> listTimeScale = GetRangeList(dtTimeScaleRange);
            string sConStr = string.Format(this.sConnectionString, condtions.MDBPath);
            OleDbDataAdapter dbAdapter1 = new OleDbDataAdapter(this.sSQLGetZD, sConStr);
            DataTable dtSiteLocation = new DataTable();
            dbAdapter1.Fill(dtSiteLocation);
            OleDbDataAdapter dbAdapter2 = new OleDbDataAdapter(this.sSQLGetZDValue, sConStr);
            DataTable dtSiteSeries = new DataTable();
            dbAdapter2.Fill(dtSiteSeries);


            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            Cells cells = sheet.Cells;

            this.bw.ReportProgress(0, string.Format("正在计算..."));
            double[,] SpatialWeightMatrix = DTSTI.GetSpatialWeightMatrix(dtSiteLocation, condtions.Type, condtions.Lambda);
            double[] OriginalSpatioTemporalSeries = DTSTI.GetOriginalSpatioTemporalSeries(dtSiteSeries);
            double[] IntegratedOriginalSpatioTemporalSeries = DTSTI.GetIntegratedSeries(OriginalSpatioTemporalSeries);

            int iRow = 1;
            for (int i = 0; i < listTimeScale.Count; i++)
            {
                int iTimeScale = listTimeScale[i];

                cells[iRow, 0].PutValue(iTimeScale);
                int iColumn = 1;

                for (int j = 0; j < listTimeLag.Count; j++)
                {
                    int iTimeLag = listTimeLag[j];
                    cells[0, iColumn].PutValue(iTimeLag);

                    double[,] TemporalWeightMatrix = DTSTI.GetTemporalWeightMatrix(dtSiteSeries.Rows.Count / dtSiteLocation.Rows.Count, iTimeLag);
                    double[] LaggedSpatioTemporalSeries = DTSTI.GetLaggedSpatioTemporalSeries(OriginalSpatioTemporalSeries, SpatialWeightMatrix, TemporalWeightMatrix);
                    double[] IntegratedLaggedSpatioTemporalSeries = DTSTI.GetIntegratedSeries(LaggedSpatioTemporalSeries);
                    double DTSTIValue = DTSTI.GetDetrend(IntegratedOriginalSpatioTemporalSeries, IntegratedLaggedSpatioTemporalSeries, iTimeScale);
                    this.bw.ReportProgress(0, string.Format("TimeScale:{1},TimeLag:{0},DTSTI:{2}", iTimeLag, iTimeScale, DTSTIValue));
                    cells[iRow, iColumn].PutValue(DTSTIValue);
                    iColumn++;
                }
                iRow++;
            }
            System.IO.MemoryStream ms = workbook.SaveToStream();
            byte[] bt = ms.ToArray();
            string sOutPath = Path.Combine(condtions.OutputFolder, string.Format("{0}(Lambda={1}).xls", (condtions.Type == SpatialMatrixType.Complete ? "Complete" : "Mixtrue"), condtions.Lambda));
            workbook.Save(sOutPath);
        }

        private List<int> GetRangeList(DataTable dtRange)
        {
            List<int> list = new List<int>();
            int iRowCount = dtRange.Rows.Count;
            foreach (DataRow dr in dtRange.Rows)
            {
                int iMin = (int)dr[0];
                int iMax = (int)dr[1];
                int iInteral = (int)dr[2];
                for (; iMin < iMax; iMin += iInteral)
                {
                    list.Add(iMin);
                }
                list.Add(iMax);
            }
            list = list.Distinct().ToList();
            list.Sort();
            return list;
        }

        private Condtions GetConditions()
        {
            Condtions conditions = new Condtions();
            conditions.OutputFolder = this.s_outputPath;
            conditions.MDBPath = this.s_databasePath;
            conditions.TimescaleRange = this.dt_timeScale;
            conditions.TimelagRange = this.dt_timeLag;
            conditions.Type = this.Type;
            conditions.Lambda = (double)this.nudLambda.Value;
            return conditions;
        }

        private void cbxMoran_CheckedChanged(object sender, EventArgs e)
        {
            bool bChecked = this.cbxMoran.Checked;
            if (bChecked)
            {
                this.numMoranTimeLag.Enabled = true;
                this.numMoranTimeScale.Enabled = true;
                this.btnGetMoran.Enabled = true;
            }
            else
            {
                this.numMoranTimeLag.Enabled = false;
                this.numMoranTimeScale.Enabled = false;
                this.btnGetMoran.Enabled = false;
            }
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            Type = sender.Equals(this.rb_complete) ? SpatialMatrixType.Complete : SpatialMatrixType.Mixture;
        }
    }


    public class Condtions
    {
        public string OutputFolder = string.Empty;
        public string MDBPath = string.Empty;
        public DataTable TimescaleRange = null;
        public DataTable TimelagRange = null;
        public SpatialMatrixType Type = SpatialMatrixType.Complete;
        public double Lambda = 0;
    }
}
