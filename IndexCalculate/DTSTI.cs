using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MathNet.Numerics;

namespace IndexCalculate
{

    /// <summary>
    /// 空间相互作用矩阵类型
    /// </summary>
    public enum SpatialMatrixType
    {
        Complete = 0,//b
        Mixture = 1 //a
    }
    /// <summary>
    /// 提供 DTSTI 计算的静态方法
    /// 计算 DTSTI 需要的输入：
    /// 1、站点的观测值数据
    /// 2、站点的位置信息
    /// 3、计算空间相互作用的 类型（complete 还是 mixture）以及 λ
    /// 4、计算时间相互作用的  time lag
    /// 5、计算消趋势的 time scale
    /// </summary>
    public class DTSTI
    {

        /// <summary>
        /// 计算空间相互作用矩阵
        /// </summary>
        /// <param name="dtCoordinates"></param>
        /// <param name="type"></param>
        /// <param name="dLambda"></param>
        /// <param name="sSiteIDField"></param>
        /// <param name="sXField"></param>
        /// <param name="sYField"></param>
        /// <returns></returns>
        public static double[,] GetSpatialWeightMatrix(DataTable dtCoordinates, SpatialMatrixType type, double dLambda,
            string sSiteIDField = "ZDDM", string sXField = "X", string sYField = "Y")
        {
            DataView dv = dtCoordinates.DefaultView;
            /* 按站点代码排序 */
            dv.Sort = sSiteIDField + " asc";
            DataTable dt = dv.ToTable();
            int iSiteCount = dt.Rows.Count;
            double[,] matrix = new double[iSiteCount, iSiteCount];
            for (int i = 0; i < iSiteCount; i++)
            {
                DataRow drI = dt.Rows[i];
                /* 计算单位为千米 */
                double xi = (double)drI[sXField] / 1000;
                double yi = (double)drI[sYField] / 1000;
                for (int j = 0; j < iSiteCount; j++)
                {
                    /* 同一个站点，空间距离为0，根据类型不同取值不同 */
                    if (i == j)
                    {
                        matrix[i, i] = type == SpatialMatrixType.Complete ? 0 : 1;
                    }
                    else
                    {
                        DataRow drJ = dt.Rows[j];
                        double xj = (double)drJ[sXField] / 1000;
                        double yj = (double)drJ[sYField] / 1000;
                        //距离值
                        double dis = Math.Sqrt((xi - xj) * (xi - xj) + (yi - yj) * (yi - yj));
                        //空间相互作用值
                        matrix[i, j] = Math.Exp(-dis / dLambda);
                    }
                }
            }
            return matrix;
        }

        /// <summary>
        /// 计算时间相互作用矩阵（使用Martrix，效率较高）
        /// </summary>
        /// <param name="iMatrixLength"></param>
        /// <param name="iTimeLag"></param>
        /// <returns></returns>
        public static double[,] GetTemporalWeightMatrix(int iMatrixLength, int iTimeLag)
        {
            double[,] timeMatrix = new double[iMatrixLength, iMatrixLength];
            for (int i = 0; i < iMatrixLength; i++)
            {
                for (int j = 0; j < iMatrixLength; j++)
                {
                    timeMatrix[i, j] = Math.Abs(i - j) <= iTimeLag ? 1 : 0;
                }
            }
            return timeMatrix;
        }

        /// <summary>
        /// 获取原始时空序列
        /// </summary>
        /// <param name="dtTimeSpatial"></param>
        /// <param name="sSiteField"></param>
        /// <param name="sTimeField"></param>
        /// <param name="sValueField"></param>
        /// <returns></returns>
        public static double[] GetOriginalSpatioTemporalSeries(DataTable dtTimeSpatial, string sSiteField = "ZDDM", string sTimeField = "X", string sValueField = "Y")
        {
            DataView dv = dtTimeSpatial.DefaultView;
            /* 根据站点ID以及X值排序，以确保序列的正确性
            先按站点排，再按站点时间排
            Rx={x(1, 1), x(1, 2), x(1, 3), …, x(1, T), x(2, 1), x(2, 2), x(2, 3), …, x(2, T), ..., x(P, i), ..., x(N, T)}
            P为站点，T为时间
            这里的实际降雨数据中N=9个站点，T=1095长度的序列 */
            dv.Sort = string.Format(@"{0} asc,{1} asc", sSiteField, sTimeField);
            DataTable dt = dv.ToTable();
            /* 创建序列 */
            double[] oSeries = new double[dtTimeSpatial.Rows.Count];
            for (int i = 0; i < dtTimeSpatial.Rows.Count; i++)
            {
                oSeries[i] = (double)dtTimeSpatial.Rows[i][sValueField];
            }
            return oSeries;
        }

        /// <summary>
        /// 获取滞后时空序列（使用Martrix，效率较高，使用TimeLag效率较低）
        /// </summary>
        /// <param name="OriginalSpatioTemporalSeries"></param>
        /// <param name="SpatialWeightMatrix"></param>
        /// <param name="iTimeLag"></param>
        /// <returns></returns>
        public static double[] GetLaggedSpatioTemporalSeries(double[] OriginalSpatioTemporalSeries, double[,] SpatialWeightMatrix, int iTimeLag)
        {
            int iSeriesLength = OriginalSpatioTemporalSeries.Length;
            int iSpatialLength = SpatialWeightMatrix.GetLength(0);
            int iTemporalLength = iSeriesLength / iSpatialLength;
            double[] laggedSpatioTemporalSeries = new double[iSeriesLength];
            for (int i = 0; i < iSeriesLength; i++)
            {
                double[] tmpSeries = new double[iSeriesLength];
                for (int j = 0; j < iSeriesLength; j++)
                {
                    int ZD1 = i / iTemporalLength;
                    int ZD2 = j / iTemporalLength;
                    int SJ1 = i % iTemporalLength;
                    int SJ2 = j % iTemporalLength;
                    double tWeight = GetTemporalWeight(SJ1, SJ2, iTimeLag);
                    double v = SpatialWeightMatrix[ZD1, ZD2] * tWeight;
                    tmpSeries[j] = v;
                }
                tmpSeries = GetIntegratedSeries(tmpSeries);
                laggedSpatioTemporalSeries[i] = SeriesMultiplySeries(tmpSeries, OriginalSpatioTemporalSeries);
            }
            return laggedSpatioTemporalSeries;
        }

        /// <summary>
        /// 获取滞后时空序列（使用Martrix，效率较高，使用TimeLag效率较低）
        /// </summary>
        /// <param name="OriginalSpatioTemporalSeries"></param>
        /// <param name="SpatialWeightMatrix"></param>
        /// <param name="TemporalWeightMatrix"></param>
        /// <returns></returns>
        public static double[] GetLaggedSpatioTemporalSeries(double[] OriginalSpatioTemporalSeries, double[,] SpatialWeightMatrix, double[,] TemporalWeightMatrix)
        {
            int iSeriesLength = OriginalSpatioTemporalSeries.Length;
            int iSpatialLength = SpatialWeightMatrix.GetLength(0);
            int iTemporalLength = iSeriesLength / iSpatialLength;
            double[] laggedSpatioTemporalSeries = new double[iSeriesLength];
            for (int i = 0; i < iSeriesLength; i++)
            {
                List<double> m = new List<double>();
                double[] tmpSeries = new double[iSeriesLength];
                for (int j = 0; j < iSeriesLength; j++)
                {
                    int ZD1 = i / iTemporalLength;
                    int ZD2 = j / iTemporalLength;
                    int SJ1 = i % iTemporalLength;
                    int SJ2 = j % iTemporalLength;
                    double tWeight = TemporalWeightMatrix[SJ1, SJ2];
                    double sWeight = SpatialWeightMatrix[ZD1, ZD2];
                    double v = sWeight * tWeight;
                    tmpSeries[j] = v;
                }
                tmpSeries = GetStandardSeries(tmpSeries);
                laggedSpatioTemporalSeries[i] = SeriesMultiplySeries(tmpSeries, OriginalSpatioTemporalSeries);
            }
            return laggedSpatioTemporalSeries;
        }

        /// <summary>
        /// 时空序列求和标准化
        /// </summary>
        /// <param name="Series"></param>
        /// <returns></returns>
        private static double[] GetStandardSeries(double[] Series)
        {
            double dSum = 0;
            int iLength = Series.Length;
            double[] StandardSeries = new double[iLength];
            for (int i = 0; i < iLength; i++)
            {
                dSum += Series[i];
            }
            for (int i = 0; i < iLength; i++)
            {
                StandardSeries[i] = dSum == 0 ? dSum : Series[i] / dSum;
            }
            return StandardSeries;
        }

        /// <summary>
        /// 获取两个时间的相互作用值
        /// </summary>
        /// <param name="iTimeA"></param>
        /// <param name="iTimeB"></param>
        /// <param name="iTimeLag"></param>
        /// <returns></returns>
        private static double GetTemporalWeight(int iTimeA, int iTimeB, int iTimeLag)
        {
            return Math.Abs(iTimeA - iTimeB) <= iTimeLag ? 1 : 0;
        }

        /// <summary>
        /// 求 TDGSTI
        /// </summary>
        /// <param name="OriginalIntegratedSpatioTemporalSeries"></param>
        /// <param name="LaggedIntegratedSpatioTemporalSeries"></param>
        /// <param name="dTimeScale"></param>
        /// <returns></returns>
        public static double GetDetrend(double[] OriginalIntegratedSpatioTemporalSeries, double[] LaggedIntegratedSpatioTemporalSeries, int dTimeScale)
        {
            //窗口跨度
            int iTimeScaleLength = dTimeScale + 1;
            int iSeriesLength = OriginalIntegratedSpatioTemporalSeries.Length;
            //窗口个数
            int iTimeScaleCount = iSeriesLength - dTimeScale;
            double dFxySumLO = 0;
            double dFxySumOO = 0;
            for (int i = 0; i < iTimeScaleCount; i++)
            {
                //单窗口拟合
                double[] xSeries = new double[iTimeScaleLength];//单窗口X值
                double[] yLaggedWindowSeries = new double[iTimeScaleLength];//单窗口滞后序列Y值
                double[] yOriginalWindowSeries = new double[iTimeScaleLength];//单窗口原始序列Y值
                //单窗口赋值
                for (int j = 0; j < iTimeScaleLength; j++)
                {
                    xSeries[j] = i + j;
                    yLaggedWindowSeries[j] = LaggedIntegratedSpatioTemporalSeries[i + j];
                    yOriginalWindowSeries[j] = OriginalIntegratedSpatioTemporalSeries[i + j];
                }

                double[] Labcd = Fit.Polynomial(xSeries, yLaggedWindowSeries, 3);//3次多项式拟合求系数
                double[] Oabcd = Fit.Polynomial(xSeries, yOriginalWindowSeries, 3);//3次多项式拟合求系数

                double dSumLO = 0;
                double dSumOO = 0;
                for (int n = 0; n < iTimeScaleLength; n++)
                {
                    double x = xSeries[n];
                    double dL = yLaggedWindowSeries[n] - (Labcd[0] + Labcd[1] * x + Labcd[2] * x * x + Labcd[3] * x * x * x);
                    double dO = yOriginalWindowSeries[n] - (Oabcd[0] + Oabcd[1] * x + Oabcd[2] * x * x + Oabcd[3] * x * x * x);
                    dSumLO += dL * dO;
                    dSumOO += dO * dO;
                }
                dFxySumLO += dSumLO / iTimeScaleLength;
                dFxySumOO += dSumOO / iTimeScaleLength;
            }
            double dDetrendedConvariance = dFxySumLO / iTimeScaleCount;
            double dDetrendedVariance = dFxySumOO / iTimeScaleCount;
            double TDGSTI = dDetrendedConvariance / dDetrendedVariance;
            return TDGSTI;
        }

        /// <summary>
        /// 求完整化序列
        /// </summary>
        /// <param name="Series"></param>
        /// <returns></returns>
        public static double[] GetIntegratedSeries(double[] Series)
        {
            double average = AverageSeries(Series);
            return GetIntegratedSeries(Series, average);
        }

        /// <summary>
        /// 序列求乘积
        /// </summary>
        /// <param name="SeriesA"></param>
        /// <param name="SeriesB"></param>
        /// <returns></returns>
        private static double SeriesMultiplySeries(double[] SeriesA, double[] SeriesB)
        {
            double v = 0;
            double length = SeriesA.Length;
            for (int i = 0; i < length; i++)
            {
                v += SeriesA[i] * SeriesB[i];
            }
            return v;
        }

        /// <summary>
        /// 求完整化序列
        /// </summary>
        /// <param name="Series"></param>
        /// <param name="dAverage"></param>
        /// <returns></returns>
        private static double[] GetIntegratedSeries(double[] Series, double dAverage)
        {
            double[] nSeries = new double[Series.Length];
            double v = 0;
            for (int i = 0; i < Series.Length; i++)
            {
                v += Series[i] - dAverage;
                nSeries[i] = v;
            }
            return nSeries;
        }

        /// <summary>
        /// 获取序列平均值
        /// </summary>
        /// <param name="Series"></param>
        /// <returns></returns>
        private static double AverageSeries(double[] Series)
        {
            double d = 0;
            int iCount = Series.Length;
            for (int i = 0; i < iCount; i++)
            {
                d += Series[i];
            }
            return d / iCount;
        }
    }
}
