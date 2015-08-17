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

        private static int ZDCount = 0;
        private static int SJCount = 0;
        private static int TimeH = 0;


        private static double[] oVector = null;
        private static double oVectorAverage = 0;
        private static double[] newoVector = null;

        public double lagVectorAverage = 0;
        public double[] lagVector = null;
        public double[] newlagVector = null;

        public static double[,] disMatrA = null;
        public static double[,] disMatrB = null;

        public double[,] timeMatr = null;
        public double[,] timedisMatr = null;


        public DTSTICaculator()
        {

        }

        public DTSTICaculator(DataTable dtZDValues, MatrType type, DataTable dtZDLocation, double lambda, int timeH)
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
        private double GetDetrend(int timeScale)
        {
            int iTimeScaleLength = timeScale + 1;
            int iTimeScaleCount = ZDCount * SJCount - timeScale;
            double[] dFxyLO = new double[iTimeScaleCount];
            double[] dFxyOO = new double[iTimeScaleCount];
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

                double[] Labcd = Fit.Polynomial(xSeries, yLSeries, 3);
                double[] Oabcd = Fit.Polynomial(xSeries, yOSeries, 3);

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
        /// 获取新时空序列
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
        public double[] GetLagVector(MatrType type)
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
            dv.Sort = string.Format(@"{0} asc,{1} asc", s_ZDValueID, s_ZDValueX);
            DataTable dt = dv.ToTable();
            oVector = new double[dtZDValues.Rows.Count];

            for (int i = 0; i < dtZDValues.Rows.Count; i++)
            {
                oVector[i] = (double)dtZDValues.Rows[i][s_ZDValueY];
            }
            return oVector;
        }

        /// <summary>
        /// 获取原始序列平均值
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
        public List<List<double>> GetTimeDisMatr(MatrType matrType)
        {
            int iLength = ZDCount * SJCount;
            List<List<double>> matr = new List<List<double>>();

            double[,] disMatr = matrType == MatrType.Complete ? disMatrA : disMatrB;
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
            for (int i = 0; i < iLength; i++)
            {
                double dSum = 0;
                for (int j = 0; j < iLength; j++)
                {
                    dSum += i < j ? matr[j][i] : matr[i][j];
                }

                for (int n = 0; n <= i; n++)
                {
                    matr[i][n] = matr[i][n] / dSum;
                }
            }
            return matr;
        }

        /// <summary>
        /// 获取时间权重矩阵
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
            return GetZDDistanceMatrix(dtZDLocation, MatrType.Complete, lambda);
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
            return GetZDDistanceMatrix(dtZDLocation, MatrType.Mixture, lambda);
        }


        /// <summary>
        /// 空间权重矩阵计算
        /// </summary>
        /// <param name="dtZDLocation"></param>
        /// <param name="type"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        private static double[,] GetZDDistanceMatrix(DataTable dtZDLocation, MatrType type, double lambda)
        {
            DataView dv = dtZDLocation.DefaultView;
            dv.Sort = s_DICMATR_ZDDM + " asc";
            DataTable dt = dv.ToTable();
            int iCount = dt.Rows.Count;
            double[,] matr = new double[iCount, iCount];
            for (int i = 0; i < iCount; i++)
            {
                DataRow drI = dt.Rows[i];
                double xi = (double)drI[s_DICMATR_X] / 1000;
                double yi = (double)drI[s_DICMATR_Y] / 1000;
                for (int j = 0; j < iCount; j++)
                {
                    DataRow drJ = dt.Rows[j];
                    double xj = (double)drJ[s_DICMATR_X] / 1000;
                    double yj = (double)drJ[s_DICMATR_Y] / 1000;
                    double dis = Math.Sqrt((xi - xj) * (xi - xj) + (yi - yj) * (yi - yj));
                    matr[i, j] = Math.Exp(-dis / lambda);
                }
            }
            if (type == MatrType.Complete)
            {
                for (int i = 0; i < iCount; i++)
                {
                    matr[i, i] = 0;
                }
            }
            return matr;
        }
    }
}