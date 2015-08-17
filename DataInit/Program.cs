using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInit
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            string sMDBPath = @"C:\Users\Administrator\Desktop\全国AQI.mdb";
            string sConStr = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};User Id=admin;Password=;", sMDBPath);
            OleDbDataAdapter dbAdapterO = new OleDbDataAdapter("select * from 江苏分站降水数据2009 order by rq asc", sConStr);
            DataTable dto = new DataTable();
            dbAdapterO.Fill(dto);
            string[] columnList = new string[]{
"JS58027_徐州",
"JS58026_邳州",
"JS58040_赣榆",
"JS58130_睢宁",
"JS58038_沭阳",
"JS58047_灌云",
"JS58135_泗洪",
"JS58141_淮安",
"JS58143_阜宁"
            };
            OleDbDataAdapter dbAdapter = new OleDbDataAdapter("select * from IndexValues", sConStr);
            OleDbCommandBuilder dbCommandBuilder = new OleDbCommandBuilder(dbAdapter);
            DataTable dt = new DataTable();
            dbAdapter.Fill(dt);
            foreach (string zdid in columnList)
            {
                for (int i = 0; i < dto.Rows.Count; i++)
                {
                    DataRow dro = dto.Rows[i];
                    DataRow dr = dt.NewRow();
                    dr["RQ"] = dro[0];
                    dr["ZDDM"] = zdid;
                    dr["X"] = i + 1;
                    dr["Y"] = dro[zdid];
                    dt.Rows.Add(dr);
                }
            }
            dbAdapter.Update(dt);
             */
            /*
            string sMDBPath = @"C:\Users\Administrator\Desktop\全国AQI.mdb";
            string sConStr = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};User Id=admin;Password=;", sMDBPath);
            string[] columnList = new string[]{
"JS58027_徐州",
"JS58026_邳州",
"JS58040_赣榆",
"JS58130_睢宁",
"JS58038_沭阳",
"JS58047_灌云",
"JS58135_泗洪",
"JS58141_淮安",
"JS58143_阜宁"
            };
            string sInCondition = string.Empty;
            foreach (string sZDDM in columnList)
            {
                sInCondition += string.Format("'{0}',", sZDDM);
            }
            sInCondition = sInCondition.Trim(new char[] { ',' });
            OleDbDataAdapter dbAdapterO = new OleDbDataAdapter(string.Format("select * from 江苏气象站点位置信息 where scode in ({0})", sInCondition), sConStr);
            DataTable dto = new DataTable();
            dbAdapterO.Fill(dto);

            OleDbDataAdapter dbAdapter = new OleDbDataAdapter("select * from ZDLocation", sConStr);
            OleDbCommandBuilder dbCommandBuilder = new OleDbCommandBuilder(dbAdapter);
            DataTable dt = new DataTable();
            dbAdapter.Fill(dt);

            for (int i = 0; i < dto.Rows.Count; i++)
            {
                DataRow dro = dto.Rows[i];
                DataRow dr = dt.NewRow();
                dr["ZDDM"] = dro["sCode"];
                dr["X"] = dro["x"];
                dr["Y"] = dro["y"];
                dt.Rows.Add(dr);
            }
            dbAdapter.Update(dt);*/

            string sMDBPath = @"C:\Users\Administrator\Desktop\全国AQI.mdb";
            string sConStr = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};User Id=admin;Password=;", sMDBPath);
            string sSQL = string.Format("select * from ZDLocation");

            OleDbDataAdapter dbAdapter = new OleDbDataAdapter(sSQL, sConStr);
            DataTable dt = new DataTable();
            dbAdapter.Fill(dt);
            Dictionary<int, Dictionary<int, double>> distance = new Dictionary<int, Dictionary<int, double>>();

            foreach (DataRow drI in dt.Rows)
            {
                int sZDDMI = (int)drI["ZDDM"];
                double xi = (double)drI["X"];
                double yi = (double)drI["Y"];

                var dic = new Dictionary<int, double>();
                distance.Add(sZDDMI, dic);
                foreach (DataRow drJ in dt.Rows)
                {
                    int sZDDMJ = (int)drJ["ZDDM"];
                    double xj = (double)drJ["X"];
                    double yj = (double)drJ["Y"];

                    double dis = Math.Sqrt((xi - xj) * (xi - xj) + (yi - yj) * (yi - yj));
                    dic.Add(sZDDMJ, dis);
                }
            }




        }
    }
}
