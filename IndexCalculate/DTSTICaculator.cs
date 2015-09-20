using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MathNet.Numerics;

namespace IndexCalculate
{
    public class DTSTICaculator
    {
        private static string s_DICMATR_ZDDM = "ZDDM";
        private static string s_DICMATR_X = "X";
        private static string s_DICMATR_Y = "Y";

        private static string s_ZDValueID = "ZDDM";
        private static string s_ZDValueX = "X";
        private static string s_ZDValueY = "Y";

        private static int ZDCount = 0;//站点个数
        private static int SJCount = 0;//时间长度
        private static int TimeH = 0;//时间跨度


        public static double[] oVector = null;//原时空序列
        public static double oVectorAverage = 0;//原始序列平均值
        public static double[] newoVector = null;//新时空序列

        public double lagVectorAverage = 0;//滞后时空序列平均值
        public double[] lagVector = null;//滞后时空序列
        public double[] newlagVector = null;//新滞后时空序列

        public static double[,] disMatrA = null;//A类空间相互作用矩阵
        public static double[,] disMatrB = null;//B类空间相互作用矩阵

        public double[,] timeMatr = null;//时间相互作用矩阵
        public double[,] timedisMatr = null;//时空相互作用矩阵

        public DTSTICaculator()
        {

        }

        public DTSTICaculator(DataTable dtZDValues, SpatialMatrixType type, DataTable dtZDLocation, double lambda, int timeH)
        {
            if (disMatrA == null || disMatrB == null)
            {
                disMatrA = GetZDDistanceMatrixA(dtZDLocation, lambda);
                disMatrB = GetZDDistanceMatrixB(dtZDLocation, lambda);
                ZDCount = dtZDLocation.Rows.Count;
                SJCount = dtZDValues.Rows.Count / ZDCount;
                oVector = GetOrignVector(dtZDValues);
                oVectorAverage = GetVectorAverage(oVector);
                newoVector = GetNewVector(oVector, oVectorAverage);
                TimeH = timeH;
            }
            this.timeMatr = this.GetTimeMatr(SJCount, timeH);
            this.lagVector = this.GetLagVector(type);
            this.lagVectorAverage = this.GetLagVectorAverage(lagVector);
            this.newlagVector = GetNewVector(lagVector, lagVectorAverage);
        }


        public DataTable GetDetrend(DataTable dtTimescale)
        {
            DataTable dtResults = new DataTable();
            DataColumn dcTimescale = new DataColumn("TIMESCALE", typeof(int));
            DataColumn dcTDGSTI = new DataColumn("TDGSTI", typeof(double));
            dtResults.Columns.Add(dcTimescale);
            dtResults.Columns.Add(dcTDGSTI);

            return dtResults;
        }


        /// <summary>
        /// 获取消趋势协方差
        /// </summary>
        /// <returns></returns>
        public double GetDetrend(int timeScale)
        {
            //窗口跨度
            int iTimeScaleLength = timeScale + 1;
            //窗口个数
            int iTimeScaleCount = ZDCount * SJCount - timeScale;
            double[] dFxyLO = new double[iTimeScaleCount];//每个窗口的计算值
            double[] dFxyOO = new double[iTimeScaleCount];//每个窗口的计算值
            double dFxySumLO = 0;
            double dFxySumOO = 0;
            for (int i = 0; i < iTimeScaleCount; i++)
            {
                double[] xSeries = new double[iTimeScaleLength];
                double[] yLSeries = new double[iTimeScaleLength];
                double[] yOSeries = new double[iTimeScaleLength];
                for (int j = 0; j < iTimeScaleLength; j++)
                {
                    xSeries[j] = i + j;
                    yLSeries[j] = this.newlagVector[i + j];
                    yOSeries[j] = newoVector[i + j];
                }

                double[] Labcd = Fit.Polynomial(xSeries, yLSeries, 3);//3次多项式拟合求系数
                double[] Oabcd = Fit.Polynomial(xSeries, yOSeries, 3);//3次多项式拟合求系数

                double dSumLO = 0;
                double dSumOO = 0;
                for (int n = 0; n < iTimeScaleLength; n++)
                {
                    double xSN = xSeries[n];
                    double dL = yLSeries[n] - (Labcd[0] + Labcd[1] * xSN + Labcd[2] * xSN * xSN + Labcd[3] * xSN * xSN * xSN);
                    double dO = yOSeries[n] - (Oabcd[0] + Oabcd[1] * xSN + Oabcd[2] * xSN * xSN + Oabcd[3] * xSN * xSN * xSN);
                    dSumLO += dL * dO;
                    dSumOO += dO * dO;
                }
                dFxyLO[i] = dSumLO / iTimeScaleLength;
                dFxyOO[i] = dSumOO / iTimeScaleLength;
                dFxySumLO += dSumLO / iTimeScaleLength;
                dFxySumOO += dSumOO / iTimeScaleLength;
            }
            double TDGSTI_LO = dFxySumLO / iTimeScaleCount;
            double TDGSTI_OO = dFxySumOO / iTimeScaleCount;
            double TDGSTI = TDGSTI_LO / TDGSTI_OO;
            return TDGSTI;
        }

        /// <summary>
        /// 获取新时空序列，老的时空序列-老的时空序列的平均值
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="average"></param>
        /// <returns></returns>
        public static double[] GetNewVector(double[] vector, double average)
        {
            double[] nVector = new double[vector.Length];
            double v = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                v += vector[i] - average;
                nVector[i] = v;
            }
            return nVector;
        }

        /// <summary>
        /// 获取滞后时空序列平均值
        /// </summary>
        /// <param name="lagVector"></param>
        /// <returns></returns>
        public double GetLagVectorAverage(double[] lagVector)
        {
            this.lagVectorAverage = GetVectorAverage(lagVector);
            return this.lagVectorAverage;
        }

        /// <summary>
        /// 计算滞后时空序列
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public double[] GetLagVector(SpatialMatrixType type)
        {
            int iLength = ZDCount * SJCount;
            double[] lagVector = new double[iLength];
            List<List<double>> timeDisMatr = this.GetTimeDisMatr(type);
            for (int i = 0; i < iLength; i++)
            {
                double v = 0;
                for (int j = 0; j < iLength; j++)
                {
                    double d = j <= i ? timeDisMatr[i][j] : timeDisMatr[j][i];
                    v += d * oVector[j];
                }
                lagVector[i] = v;
            }
            return lagVector;
        }

        /// <summary>
        /// 获取初始时空序列
        /// </summary>
        /// <param name="dtZDValues"></param>
        /// <returns></returns>
        private static double[] GetOrignVector(DataTable dtZDValues)
        {
            DataView dv = dtZDValues.DefaultView;
            /* 根据站点ID以及X值排序，以确保序列的正确性
            先按站点排，再按站点时间排
            Rx={x(1, 1), x(1, 2), x(1, 3), …, x(1, T), x(2, 1), x(2, 2), x(2, 3), …, x(2, T), ..., x(P, i), ..., x(N, T)}
            P为站点，T为时间
            这里的实际降雨数据中N=9个站点，T=1095长度的序列 */
            dv.Sort = string.Format(@"{0} asc,{1} asc", s_ZDValueID, s_ZDValueX);
            DataTable dt = dv.ToTable();
            /* 创建序列 */
            oVector = new double[dtZDValues.Rows.Count];
            for (int i = 0; i < dtZDValues.Rows.Count; i++)
            {
                oVector[i] = (double)dtZDValues.Rows[i][s_ZDValueY];
            }
            return oVector;
        }

        /// <summary>
        /// 获取序列平均值
        /// </summary>
        /// <param name="oVector"></param>
        /// <returns></returns>
        private static double GetVectorAverage(double[] oVector)
        {
            double d = 0;
            int iCount = oVector.Length;
            for (int i = 0; i < iCount; i++)
            {
                d += oVector[i];
            }
            return d / iCount;
        }

        /// <summary>
        /// 获取时空权重矩阵
        /// </summary>
        /// <returns></returns>
        public List<List<double>> GetTimeDisMatr(SpatialMatrixType SpatialMatrixType)
        {
            int iLength = ZDCount * SJCount;
            List<List<double>> matr = new List<List<double>>();

            double[,] disMatr = SpatialMatrixType == SpatialMatrixType.Complete ? disMatrA : disMatrB;
            //下三角矩阵
            for (int i = 0; i < iLength; i++)
            {
                List<double> m = new List<double>();
                for (int j = 0; j <= i; j++)
                {
                    int ZD1 = i / SJCount;
                    int ZD2 = j / SJCount;
                    int SJ1 = i % SJCount;
                    int SJ2 = j % SJCount;
                    double v = disMatr[ZD1, ZD2] * timeMatr[SJ1, SJ2];
                    m.Add(v);
                }
                matr.Add(m);
            }

            List<double> rSum = new List<double>();
            //标准化
            for (int i = 0; i < iLength; i++)
            {
                double dSum = 0;
                for (int j = 0; j < iLength; j++)
                {
                    dSum += i < j ? matr[j][i] : matr[i][j];
                }
                rSum.Add(dSum);
            }

            for (int i = 0; i < iLength; i++)
            {
                for (int j = 0; j < matr[i].Count; j++)
                {
                    matr[i][j] = matr[i][j] / rSum[i];
                }
            }
            return matr;
        }

        /// <summary>
        /// 获取时间相互作用矩阵
        /// 超过一定的时间跨度，相互作用为0，否则为1
        /// </summary>
        /// <param name="iMatrLength"></param>
        /// <param name="timeH"></param>
        /// <returns></returns>
        private double[,] GetTimeMatr(int iMatrLength, int timeH)
        {
            double[,] timeMatr = new double[iMatrLength, iMatrLength];
            for (int i = 0; i < iMatrLength; i++)
            {
                for (int j = 0; j < iMatrLength; j++)
                {
                    timeMatr[i, j] = Math.Abs(i - j) <= timeH ? 1 : 0;
                }
            }
            return timeMatr;
        }

        /// <summary>
        /// 空间权重矩阵A计算
        /// </summary>
        /// <param name="dtZDLocation"></param>
        /// <param name="type"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        private static double[,] GetZDDistanceMatrixA(DataTable dtZDLocation, double lambda)
        {
            return GetZDDistanceMatrix(dtZDLocation, SpatialMatrixType.Complete, lambda);
        }


        /// <summary>
        /// 空间权重矩阵B计算
        /// </summary>
        /// <param name="dtZDLocation"></param>
        /// <param name="type"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        private static double[,] GetZDDistanceMatrixB(DataTable dtZDLocation, double lambda)
        {
            return GetZDDistanceMatrix(dtZDLocation, SpatialMatrixType.Mixture, lambda);
        }


        /// <summary>
        /// 空间相互作用矩阵
        /// </summary>
        /// <param name="dtZDLocation"></param>
        /// <param name="type"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        private static double[,] GetZDDistanceMatrix(DataTable dtZDLocation, SpatialMatrixType type, double lambda)
        {
            DataView dv = dtZDLocation.DefaultView;
            /* 按站点代码排序 */
            dv.Sort = s_DICMATR_ZDDM + " asc";
            DataTable dt = dv.ToTable();
            int iCount = dt.Rows.Count;
            double[,] matr = new double[iCount, iCount];
            for (int i = 0; i < iCount; i++)
            {
                DataRow drI = dt.Rows[i];
                /* 计算单位为千米 */
                double xi = (double)drI[s_DICMATR_X] / 1000;
                double yi = (double)drI[s_DICMATR_Y] / 1000;
                for (int j = 0; j < iCount; j++)
                {
                    /* 同一个站点，空间距离为0，根据类型不同取值不同 */
                    if (i == j)
                    {
                        matr[i, i] = type == SpatialMatrixType.Complete ? 0 : 1;
                    }
                    else
                    {
                        DataRow drJ = dt.Rows[j];
                        double xj = (double)drJ[s_DICMATR_X] / 1000;
                        double yj = (double)drJ[s_DICMATR_Y] / 1000;
                        //距离值
                        double dis = Math.Sqrt((xi - xj) * (xi - xj) + (yi - yj) * (yi - yj));
                        //空间相互作用值
                        matr[i, j] = Math.Exp(-dis / lambda);
                    }
                }
            }
            return matr;
        }
    }
}