using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;

namespace IndexCalculate
{

    public class ARFIMASeriesGenerator
    {
        /// <summary>
        /// 生成正态分布序列
        /// </summary>
        /// <param name="dMean">期望</param>
        /// <param name="dDeviation">方差</param>
        /// <param name="iCount">个数</param>
        /// <returns></returns>
        public static double[] CreateNormalSeries(double dMean, double dDeviation, int iCount)
        {
            IEnumerable<double> dSequence = Generate.NormalSequence(dMean, dDeviation);
            double[] dNewSequence = new double[iCount];
            int i = 0;
            foreach (double v in dSequence)
            {
                if (i >= iCount)
                {
                    break;
                }
                else
                {
                    dNewSequence[i] = v;
                    i++;
                }
            }
            return dNewSequence;
        }

        /// <summary>
        /// 生成ARFIMA序列
        /// </summary>
        /// <param name="normalSeries"></param>
        /// <param name="dRho"></param>
        /// <param name="dZ0"></param>
        /// <returns></returns>
        public static double[] CreateARFIMASeries(double[] normalSeries, double dRho, double dZ0 = 0)
        {
            int iSeriesLength = normalSeries.Length;
            double[] ARFIMASeries = new double[iSeriesLength];
            ARFIMASeries[0] = dZ0;
            for (int i = 1; i < iSeriesLength; i++)
            {
                //计算单个Zi
                double Zi = 0;
                for (int j = 1; j <= i; j++)
                {
                    //计算单项
                    double div = 1;
                    for (int n = 0; n < j; n++)
                    {
                        double dSingleItem = (n - dRho) / (n + 1);
                        div *= dSingleItem;
                    }
                    //单项累计
                    Zi += ARFIMASeries[i - j] * div;
                }
                //加余项
                Zi += normalSeries[i];
                ARFIMASeries[i] = Zi;
            }
            return ARFIMASeries;
        }

        /// <summary>
        /// 生成ARFIMA序列
        /// </summary>
        /// <param name="dMean"></param>
        /// <param name="dDeviation"></param>
        /// <param name="iCount"></param>
        /// <param name="dRho"></param>
        /// <param name="dZ0"></param>
        /// <returns></returns>
        public static double[] CreateARFIMASeries(double dMean, double dDeviation, int iCount, double dRho, double dZ0 = 0)
        {
            double[] normalSeries = CreateNormalSeries(dMean, dDeviation, iCount);
            return CreateARFIMASeries(normalSeries, dRho, dZ0);
        }
    }
}
