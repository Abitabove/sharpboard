//    Copyright 2010 SharpBoard Library authors
//
//    This file is part of SharpBoard Library.
//
//    SharpBoard Library is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    SharpBoard Library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with SharpBoard Library.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace SharpBoardLibrary
{
    public class Warper
    {
        float[] srcX = new float[4];
        float[] srcY = new float[4];
        float[] dstX = new float[4];
        float[] dstY = new float[4];
        float[] srcMat = new float[16];
        float[] dstMat = new float[16];
        float[] warpMat = new float[16];

        public bool Dirty = true;
        public float[] SrcX
        {
            get { return srcX; }
        }
        public float[] SrcY
        {
            get { return srcY; }
        }

        // Delfo
        //area of quadrliatera
        public float ActualArea
        {
            get
            {
                // float area = 0.5f * Math.Abs((srcX[1] - srcX[2]) * (srcY[0] - srcY[3]) - (srcX[0] - srcX[3]) * (srcY[1] - srcY[2]));
                float area = 0.5f * Math.Abs((srcX[1] - srcX[3]) * (srcY[0] - srcY[2]) - (srcX[0] - srcX[2]) * (srcY[1] - srcY[3]));
                return area;
            }
        }

        // Delfo
        // Util area
        public float UtilArea
        {
            get
            {
                float calibrationMargin = .1f;
                //area of ideal calibration coordinates (to match the screen)
                float idealArea = (1 - 2 * calibrationMargin) * 1024 * (1 - 2 * calibrationMargin) * 768;
                float util = (ActualArea / idealArea) * 100;

                return util;
            }
        }

        public Warper()
        {
            SetIdentity();
        }

        public override string ToString()
        {
            string s = string.Format("{0}|{1}|{2}|{3}",
                FloatArrayToString(srcX),
                FloatArrayToString(srcY),
                FloatArrayToString(dstX),
                FloatArrayToString(dstY));

            return s;
        }

        string FloatArrayToString(float[] f)
        {
            int i = 0;
            string s = "";
            for (i = 0; i < f.Length-1; i++)
            {
                s += f[i] + ";";
            }
            s += f[i];
            return s;
        }

        static float[] ToFloatArray(string s)
        {
            string[] parts = s.Split(';');
            float[] f = new float[4];
            for (int i = 0; i < parts.Length; i++)
            {
                f[i] = int.Parse(parts[i]);
            }
            return f;
        }

        public static bool TryParse(string s, out Warper w)
        {
            bool ok = false;
            w = null;
            try
            {
                string[] parts = s.Split('|');
                float[] srcX = Warper.ToFloatArray(parts[0]);
                float[] srcY = Warper.ToFloatArray(parts[1]);
                float[] dstX = Warper.ToFloatArray(parts[2]);
                float[] dstY = Warper.ToFloatArray(parts[3]);
                w = new Warper();
                w.srcX = srcX;
                w.srcY = srcY;
                w.dstX = dstX;
                w.dstY = dstY;
                ok = true;
            }
            catch (Exception)
            {
                ok = false;
                throw;
            }

            return ok;
        }

        public static Warper Parse(string s)
        {
            Warper w = null;
            bool ok = Warper.TryParse(s, out w);
            if (!ok)
            {
                throw new FormatException("Invalid Warper coords");
            }
            return w;
        }

        public void SetSource(CalibrationPoints src)
        {
            SetSource(src.X[0], src.Y[0], src.X[1], src.Y[1], src.X[2], src.Y[2], src.X[3], src.Y[3]);
        }

        public void SetSource(float x0,
                        float y0,
                        float x1,
                        float y1,
                        float x2,
                        float y2,
                        float x3,
                        float y3)
        {
            srcX[0] = x0;
            srcY[0] = y0;
            srcX[1] = x1;
            srcY[1] = y1;
            srcX[2] = x2;
            srcY[2] = y2;
            srcX[3] = x3;
            srcY[3] = y3;
            Dirty = true;
        }

        public void SetDestination(CalibrationPoints dst)
        {
            SetDestination(dst.X[0], dst.Y[0], dst.X[1], dst.Y[1], dst.X[2], dst.Y[2], dst.X[3], dst.Y[3]);
        }

        public void SetDestination(float x0,
                                    float y0,
                                    float x1,
                                    float y1,
                                    float x2,
                                    float y2,
                                    float x3,
                                    float y3)
        {
            dstX[0] = x0;
            dstY[0] = y0;
            dstX[1] = x1;
            dstY[1] = y1;
            dstX[2] = x2;
            dstY[2] = y2;
            dstX[3] = x3;
            dstY[3] = y3;
            Dirty = true;
        }


        public void ComputeWarp()
        {
            ComputeQuadToSquare(srcX[0], srcY[0],
                                    srcX[1], srcY[1],
                                    srcX[2], srcY[2],
                                    srcX[3], srcY[3],
                                    srcMat);
            ComputeSquareToQuad(dstX[0], dstY[0],
                                    dstX[1], dstY[1],
                                    dstX[2], dstY[2],
                                    dstX[3], dstY[3],
                                    dstMat);
            MultMats(srcMat, dstMat, warpMat);
            Dirty = false;
        }

        void SetIdentity()
        {
            SetSource(0.0f, 0.0f,
                           1.0f, 0.0f,
                           0.0f, 1.0f,
                           1.0f, 1.0f);
            SetDestination(0.0f, 0.0f,
                           1.0f, 0.0f,
                           0.0f, 1.0f,
                           1.0f, 1.0f);
            ComputeWarp();
        }

        void MultMats(float[] srcMat, float[] dstMat, float[] resMat)
        {
            // DSTDO/CBB: could be faster, but not called often enough to matter
            for (int r = 0; r < 4; r++)
            {
                int ri = r * 4;
                for (int c = 0; c < 4; c++)
                {
                    resMat[ri + c] = (srcMat[ri] * dstMat[c] +
                              srcMat[ri + 1] * dstMat[c + 4] +
                              srcMat[ri + 2] * dstMat[c + 8] +
                              srcMat[ri + 3] * dstMat[c + 12]);
                }
            }
        }

        void ComputeSquareToQuad(float x0,
                                            float y0,
                                            float x1,
                                            float y1,
                                            float x2,
                                            float y2,
                                            float x3,
                                            float y3,
                                            float[] mat)
        {

            float dx1 = x1 - x2, dy1 = y1 - y2;
            float dx2 = x3 - x2, dy2 = y3 - y2;
            float sx = x0 - x1 + x2 - x3;
            float sy = y0 - y1 + y2 - y3;
            float g = (sx * dy2 - dx2 * sy) / (dx1 * dy2 - dx2 * dy1);
            float h = (dx1 * sy - sx * dy1) / (dx1 * dy2 - dx2 * dy1);
            float a = x1 - x0 + g * x1;
            float b = x3 - x0 + h * x3;
            float c = x0;
            float d = y1 - y0 + g * y1;
            float e = y3 - y0 + h * y3;
            float f = y0;

            mat[0] = a; mat[1] = d; mat[2] = 0; mat[3] = g;
            mat[4] = b; mat[5] = e; mat[6] = 0; mat[7] = h;
            mat[8] = 0; mat[9] = 0; mat[10] = 1; mat[11] = 0;
            mat[12] = c; mat[13] = f; mat[14] = 0; mat[15] = 1;
        }

        void ComputeQuadToSquare(float x0,
                                            float y0,
                                            float x1,
                                            float y1,
                                            float x2,
                                            float y2,
                                            float x3,
                                            float y3,
                                            float[] mat)
        {
            ComputeSquareToQuad(x0, y0, x1, y1, x2, y2, x3, y3, mat);

            // invert through adjoint

            float a = mat[0], d = mat[1],	/* ignore */		g = mat[3];
            float b = mat[4], e = mat[5],	/* 3rd col*/		h = mat[7];
            /* ignore 3rd row */
            float c = mat[12], f = mat[13];

            float A = e - f * h;
            float B = c * h - b;
            float C = b * f - c * e;
            float D = f * g - d;
            float E = a - c * g;
            float F = c * d - a * f;
            float G = d * h - e * g;
            float H = b * g - a * h;
            float I = a * e - b * d;

            // Probably unnecessary since 'I' is also scaled by the determinant,
            //   and 'I' scales the homogeneous coordinate, which, in turn,
            //   scales the X,Y coordinates.
            // Determinant  =   a * (e - f * h) + b * (f * g - d) + c * (d * h - e * g);
            float idet = 1.0f / (a * A + b * D + c * G);

            mat[0] = A * idet; mat[1] = D * idet; mat[2] = 0; mat[3] = G * idet;
            mat[4] = B * idet; mat[5] = E * idet; mat[6] = 0; mat[7] = H * idet;
            mat[8] = 0; mat[9] = 0; mat[10] = 1; mat[11] = 0;
            mat[12] = C * idet; mat[13] = F * idet; mat[14] = 0; mat[15] = I * idet;
        }

        public float[] GetWarpMatrix()
        {
            return warpMat;
        }

        public void Warp(float srcX, float srcY, ref float dstX, ref float dstY)
        {
            if (Dirty)
                ComputeWarp();
            Warper.Warp(warpMat, srcX, srcY, ref dstX, ref dstY);
        }

        public void WarpPresenter(float screenWidth, float screenHeight, ref float dstX, ref float dstY)
        {
            float xFactor = screenWidth / 1010;
            float yFactor = screenHeight / 760;
            dstX = screenWidth - (dstX * xFactor);
            dstY = dstY * yFactor;
        }

        public static void Warp(float[] mat, float srcX, float srcY, ref float dstX, ref float dstY)
        {
            float[] result = new float[4];
            float z = 0;
            result[0] = (float)(srcX * mat[0] + srcY * mat[4] + z * mat[8] + 1 * mat[12]);
            result[1] = (float)(srcX * mat[1] + srcY * mat[5] + z * mat[9] + 1 * mat[13]);
            result[2] = (float)(srcX * mat[2] + srcY * mat[6] + z * mat[10] + 1 * mat[14]);
            result[3] = (float)(srcX * mat[3] + srcY * mat[7] + z * mat[11] + 1 * mat[15]);
            dstX = result[0] / result[3];
            dstY = result[1] / result[3];
        }
    }
}
