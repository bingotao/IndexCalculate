using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataInit
{
    public enum MatrType
    {
        Complete = 0, Mixture = 1, Both = 2
    }

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


        private static double[] oVector = null;
        public static double[,] disMatrA = null;
        public static double[,] disMatrB = null;

        public double[,] timeMatr = null;
        public double[,] timedisMatr = null;


        public DTSTICaculator()
        {

        }

        public DTSTICaculator(DataTable dtZDValues, DataTable dtZDLocation, double lambda, int timeH)
        {
            if (disMatrA == null || disMatrB == null)
            {
                disMatrA = GetZDDistanceMatrixA(dtZDLocation, lambda);
                disMatrB = GetZDDistanceMatrixB(dtZDLocation, lambda);
                ZDCount = dtZDLocation.Rows.Count;
                SJCount = dtZDValues.Rows.Count / ZDCount;
                oVector = GetOrignVector(dtZDValues);
            }
            this.timeMatr = this.GetTimeMatr(SJCount, timeH);
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
        /// 获取时空权重矩阵
        /// </summary>
        /// <returns></returns>
        public double[,] GetTimeDisMatr(MatrType matrType)
        {
            int iLength = ZDCount * SJCount;
            double[,] matr = new double[iLength, iLength];
            double[,] disMatr = matrType == MatrType.Complete ? disMatrA : disMatrB;
            for (int i = 0; i < iLength; i++)
            {
                for (int j = 0; j < iLength; j++)
                {
                    int ZD1 = i / SJCount;
                    int ZD2 = j / SJCount;
                    int SJ1 = i % SJCount;
                    int SJ2 = j % SJCount;
                    matr[i, j] = disMatr[ZD1, ZD2] * timeMatr[SJ1, SJ2];
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