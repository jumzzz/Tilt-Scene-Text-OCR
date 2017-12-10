using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;


namespace tilt_reader
{
    public class Vector2D
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2D(int xVal, int yVal)
        {
            X = xVal;
            Y = yVal;
        }
    }

    public class Misc 
    {
        
        public static string FloatsToCSV(List<float> vals)
        {
            StringBuilder csvSB = new StringBuilder();

            foreach (float f in vals)
            {
                csvSB.Append(f.ToString());
                csvSB.Append(",");
            }

            string result = csvSB.ToString();
            return result;
        }

        public static string DoublesToCSV(List<double> vals) 
        {
            StringBuilder csvSB = new StringBuilder();

            foreach (double d in vals) 
            {
                csvSB.Append(d.ToString());
                csvSB.Append(",");
            }

            string result = csvSB.ToString().Substring(0, csvSB.Length - 1);
            return result;
        }

        public static void AppendToCSVFile(string fileLoc, List<string> param)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string s in param) 
                sb.AppendLine(s);
            
            using (StreamWriter sw = File.AppendText(fileLoc))
            {
                sw.WriteLine(sb.ToString());
                sw.Close();
            }
        }

        public static string GenDateTimeStr() 
        {
            DateTime dt = DateTime.Now;
            //string s0 = dt.Year.ToString();
            string s1 = dt.Month.ToString();
            string s2 = dt.Day.ToString();
            string s3 = dt.Hour.ToString();
            string s4 = dt.Minute.ToString();
            //string s5 = dt.Second.ToString();
            string s6 = dt.Millisecond.ToString();

            return s1 + s2 + s3 + s4 + s6;


        }
    }
    public class DrawSpecial 
    {
        public static void DrawRotRect(IInputOutputArray img, RotatedRect rect, MCvScalar color, int thickness)
        {
            PointF[] rectVerticesF = rect.GetVertices();
            List<PointF> listVerticesF = rectVerticesF.ToList();
            List<Point> listVertices = new List<Point>();

            listVerticesF.ForEach(ptF => listVertices.Add(new Point((int)ptF.X, (int)ptF.Y)));


            for (int j = 0; j < 4; j++)
                CvInvoke.Line(img, listVertices[j], listVertices[(j + 1) % 4], color, thickness);
        }
    }

    public class EmguCVFunc
    {


        public static List<Tuple<string, double ,Rectangle, RotatedRect>> TiltOrientations(UMat parent, VectorOfVectorOfPoint contours) 
        {
            // acquire cropped images first
            // 3 parameters are needed to be acquired by each
            // 1. cropped image
            // 2. rectangular ROI
            // 3. rotated rect

            List<Tuple<UMat, Rectangle,RotatedRect>> initalList = 
                new List<Tuple<UMat, Rectangle,RotatedRect>>();
            int len = contours.Size;

            for (int i = 0; i < len; i++) 
            {
                RotatedRect rotRectROI = CvInvoke.MinAreaRect(contours[i]);
            
                //Rectangle rectROI = rotRectROI.MinAreaRect();
                Rectangle rectROI = CvInvoke.BoundingRectangle(contours[i]);
                rectROI = CorrectRectangle(parent.Size, rectROI);
                UMat cropped = new UMat(parent, rectROI);
                
                
                Tuple<UMat, Rectangle,RotatedRect> pairCroppedROI = new Tuple<UMat, Rectangle, RotatedRect>(cropped, rectROI,rotRectROI);
                initalList.Add(pairCroppedROI);
            }

            List<Tuple<string, double, Rectangle, RotatedRect>> resultList = new List<Tuple<string, double, Rectangle, RotatedRect>>();

            foreach (Tuple<UMat, Rectangle, RotatedRect> tp in initalList) 
            {
                //TiltDetection td = new TiltDetection(tp.Item1);
                //td.ComputeTilt();

                string orientation = Inclination(tp.Item3);
                double angle = TiltAngle(tp.Item3);
                Tuple<string, double, Rectangle, RotatedRect> result = 
                    new Tuple<string, double, Rectangle, RotatedRect>(orientation, 
                        angle, 
                        tp.Item2, 
                        tp.Item3);

                resultList.Add(result);
            }

            return resultList;
        }

        public static Tuple<PointF, PointF> LongestPointPair(RotatedRect rectRot) 
        {
            PointF[] rect = rectRot.GetVertices();
            double maxDist = EmguCVFunc.Distance(rect[0], rect[1 % 4]);
            Tuple<PointF, PointF> longestPair = new Tuple<PointF,PointF>(rect[0], rect[1 % 4]);

            for (int i = 1; i < 4; i++)
                if (EmguCVFunc.Distance(rect[i], rect[(i + 1) % 4]) > maxDist)
                {
                    maxDist = EmguCVFunc.Distance(rect[i], rect[(i + 1) % 4]);
                    longestPair = new Tuple<PointF, PointF>(rect[i], rect[(i + 1) % 4]);
                }

           
             return longestPair;

        }

        public static string Inclination(RotatedRect rectOrig) 
        {
            string FIRST_DIR_RIGHT = "Right";
            string FIRST_DIR_LEFT = "Left";
            string FIRST_DIR_NONE = "None";

            Tuple<PointF, PointF> longestPair = LongestPointPair(rectOrig);

            // find left most side

            PointF ptLeftMost = longestPair.Item1.X < longestPair.Item2.X ?
                longestPair.Item1 : longestPair.Item2;

            PointF ptRightMost = longestPair.Item1.X > longestPair.Item2.X ?
                longestPair.Item1 : longestPair.Item2;

            if (ptLeftMost.Y < ptRightMost.Y) return FIRST_DIR_LEFT;
            else if (ptLeftMost.Y > ptRightMost.Y) return FIRST_DIR_RIGHT;
            else return FIRST_DIR_NONE;
            
        }

        public static double TiltAngle(RotatedRect rectOrig)
        {

            PointF[] rect = rectOrig.GetVertices();

            Tuple<PointF, PointF> longestPair = new Tuple<PointF, PointF>(rect[0], rect[1 % 4]);
            double maxDist = EmguCVFunc.Distance(rect[0], rect[1 % 4]);

            PointF lowestPt = EmguCVFunc.MaxPoint(longestPair);



            for (int i = 1; i < 4; i++)
                if (EmguCVFunc.Distance(rect[i], rect[(i + 1) % 4]) > maxDist)
                {
                    maxDist = EmguCVFunc.Distance(rect[i], rect[(i + 1) % 4]);
                    longestPair = new Tuple<PointF, PointF>(rect[i], rect[(i + 1) % 4]);
                }


            float xAxis = longestPair.Item1.X > longestPair.Item2.X ?
                longestPair.Item1.X : longestPair.Item2.X;

            float yAxis = longestPair.Item1.Y > longestPair.Item2.Y ?
                longestPair.Item1.Y : longestPair.Item2.Y;


            PointF rightVertex = new PointF(xAxis, yAxis);

            // compute for x axis distance

            float xDel0 = Math.Abs(longestPair.Item1.X - rightVertex.X);
            float xDel1 = Math.Abs(longestPair.Item2.X - rightVertex.X);
            float xDel = xDel0 > xDel1 ? xDel0 : xDel1;

            float yDel0 = Math.Abs(longestPair.Item1.Y - rightVertex.Y);
            float yDel1 = Math.Abs(longestPair.Item2.Y - rightVertex.Y);
            float yDel = yDel0 > yDel1 ? yDel0 : yDel1;

            double angle = Math.Atan2((double)yDel, (double)xDel);

            return angle*180.0/Math.PI;

        }


        public static PointF MaxPoint(Tuple<PointF, PointF> pointTuple) 
        {
            if (pointTuple.Item1.Y > pointTuple.Item2.Y) return pointTuple.Item1;
            else return pointTuple.Item2;
        }

        public static double Distance(PointF pt1, PointF pt2) 
        {
            double delX = pt1.X - pt2.X;
            double delY = pt1.Y - pt2.Y;

            double delX2 = delX * delX;
            double delY2 = delY * delY;

            return Math.Sqrt(delX2 + delY2);

        }

        public static VectorOfPoint LargestContour(VectorOfVectorOfPoint vvp)
        {
            int largestIndex = 0;
            double currentLargest = CvInvoke.ContourArea(vvp[0]);

            for (int i = 1; i < vvp.Size; i++)
            {
                double area = CvInvoke.ContourArea(vvp[i]);

                if (area > currentLargest)
                {
                    currentLargest = area;
                    largestIndex = i;
                }
            }

            return vvp[largestIndex];
        }


        public static double DistPoint(PointF pt1, PointF pt2)
        {
            double xSquared = (pt1.X - pt2.X) * (pt1.X - pt2.X);
            double ySquared = (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y);

            return Math.Sqrt(xSquared + ySquared);
        }

        public static double DistPoint(Point pt1, Point pt2)
        {
            double xSquared = (pt1.X - pt2.X) * (pt1.X - pt2.X);
            double ySquared = (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y);

            return Math.Sqrt(xSquared + ySquared);
        }

        public static double TotalDist(VectorOfPoint vp)
        {
            double totalDist = 0.0;

            for (int i = 0; i < vp.Size - 1; i++)
                totalDist += DistPoint(vp[i], vp[i + 1]);

            return totalDist;
        }

        public static double MeanOfValues(List<double> vals)
        {
            double totalSum = 0.0;
            vals.ForEach(d => totalSum += d);
            return totalSum / ((double)vals.Count);

        }

        public static double RotatedRectArea(RotatedRect rotRect)
        {
            PointF[] vertices = rotRect.GetVertices();
            List<double> myDists = new List<double>();

            for (int j = 0; j < 4; j++)
                myDists.Add(DistPoint(vertices[j], vertices[(j + 1) % 4]));

            return myDists.Min() * myDists.Max();

        }

        // feature 1
        public static double Solidity(VectorOfPoint contour, VectorOfPoint convexHull)
        {
            double contourArea = CvInvoke.ContourArea(contour);

            VectorOfPoint vpHull = new VectorOfPoint();

            CvInvoke.ApproxPolyDP(convexHull, vpHull, 0.001, true);

            double hullArea = Math.Abs(CvInvoke.ContourArea(vpHull));

            return contourArea / hullArea;
        }

        // feature 2
        public static double Rectangularity(VectorOfPoint vp)
        {
            double shapeArea = CvInvoke.ContourArea(vp);
            double rectArea = RotatedRectArea(CvInvoke.MinAreaRect(vp));

            if (rectArea > 0.0)
                return shapeArea / rectArea;
            else return 0.0;
        }

        // feature 3
        public static double CircularityRatio(VectorOfPoint vp)
        {

            
            // acquire centroid of points
            MCvMoments moments = CvInvoke.Moments(vp);
            int xCenter = (int)(moments.M10 / moments.M00);
            int yCenter = (int)(moments.M01 / moments.M00);
            Point centroid = new Point(xCenter, yCenter);

            // acquire mean of radial dist
            List<double> diffToCentroid = new List<double>();
            int len = vp.Size;
            double n = len;

            for (int i = 0; i < len; i++)
                diffToCentroid.Add(DistPoint(vp[i], centroid));

            double mean = (diffToCentroid.Sum()) / n;

            // acquire std dev of radial dist
            List<double> diffToMean = new List<double>();
            diffToCentroid.ForEach(val => diffToMean.Add((val - mean) * (val - mean)));
            double sigma = (diffToMean.Sum()) / (n - 1);

            if (sigma > 0.0)
                return mean / sigma;
            else return 0.0;
        }

        public static Rectangle CorrectRectangle(Size maxSize, Rectangle rect) 
        {
            // const cv::Mat &toBeCropped, const cv::Rect &inputROI, cv::Rect &correctedROI
            int width0 = rect.Width;
            int height0 = rect.Height;

            int widthTotal = maxSize.Width;
            int heightTotal = maxSize.Height;


            int posX = rect.X;
            int posY = rect.Y;

            int width1 = width0;
            int height1 = height0;

            if (posX <= 0)
            {
                posX = 1;
            }

            if (posY <= 0)
            {
                posY = 1;
            }

            if (posX >= widthTotal)
            {
                posX = widthTotal - 1;

            }

            if (posY >= heightTotal)
            {
                posY = heightTotal;
            }

            if (posX + width0 > widthTotal)
            {
                width1 = widthTotal - posX;
            }

            if (posY + height0 > heightTotal)
            {
                height1 = heightTotal - posY;
            }


            return new Rectangle(posX, posY, width1, height1);
            //correctedROI = cv::Rect(posX,posY,width1,height1);


        }

        public static bool UseParentInstead(UMat parent, Rectangle rect)
        {
            bool val0 = rect.Location.X + rect.Width >= parent.Size.Width;
            bool val1 = rect.Location.Y + rect.Height >= parent.Size.Height;
            bool val2 = rect.Location.X <= 0;
            bool val3 = rect.Location.Y <= 0;

            bool useParent = val0 || val1 || val2 || val3;

            if (useParent) return true;
            else return false;
        }

        public static int BoolCount(bool arg)
        {
            if (arg) return 1;
            else return 0;
        }


        public static double GetSymmetryRatio(VectorOfPoint vp)
        {
            double totalDist = TotalDist(vp);

            List<double> listOfDists = new List<double>();

            for (int i = 0; i < vp.Size - 1; i++)
                listOfDists.Add(DistPoint(vp[i], vp[i + 1]) / totalDist);

            double meanOfVals = MeanOfValues(listOfDists);
            List<double> diffToMeanSq = new List<double>();
            listOfDists.ForEach(d => diffToMeanSq.Add((d - meanOfVals) * (d - meanOfVals)));

            double denum = (double)listOfDists.Count - 1.0;
            return diffToMeanSq.Sum() / denum;

        }

        public static UMat SkeletonNew(UMat input)
        {
            UMat inputClone = input.Clone();

            Image<Gray, Byte> inputImg = inputClone.ToImage<Gray, Byte>();

            UMat skel = new UMat(input.Size, input.Depth, input.NumberOfChannels);
            UMat temp = new UMat(input.Size, input.Depth, input.NumberOfChannels);
            UMat eroded = new UMat(input.Size, input.Depth, input.NumberOfChannels);

            Image<Gray, Byte> skelImg = skel.ToImage<Gray, Byte>();
            Image<Gray, Byte> tempImg = temp.ToImage<Gray, Byte>();
            Image<Gray, Byte> erodedImg = eroded.ToImage<Gray, Byte>();

            bool done = false;

            do
            {
                erodedImg = inputImg.Dilate(1);
                tempImg = inputImg.Erode(1);

                CvInvoke.Subtract(inputImg, tempImg, tempImg);
                CvInvoke.BitwiseOr(skelImg, tempImg, skelImg);

                erodedImg.CopyTo(inputImg);

                int size = inputImg.Size.Height * inputImg.Size.Width;
                int zeros = inputImg.Size.Height * inputImg.Size.Width - CvInvoke.CountNonZero(inputImg);

                if (zeros == size) done = true;

            } while (!done);

            return inputImg.ToUMat();

        }

        public static UMat Skeleton(UMat input, int iter)
        {
            Point anchor = new Point(-1, -1);
            UMat element = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(3, 3), anchor).GetUMat(AccessType.Fast);


            MCvScalar borderValue = new MCvScalar(255);
            UMat img = input.Clone();


            UMat imageRaw = new UMat(input.Size, input.Depth, input.NumberOfChannels);


            CvInvoke.Dilate(img, img, element, anchor, iter, BorderType.Reflect, borderValue);
            CvInvoke.Erode(img, img, element, anchor, iter + 1, BorderType.Reflect, borderValue);

            return img;

        }

        public static UMat Thinning(UMat im)
        {
            UMat imClone = im.Clone();
            UMat res0 = new UMat();
            CvInvoke.ConvertScaleAbs(imClone, res0, 1.0 / 255.0, 0.0);

            UMat prev = new UMat(imClone.Size, imClone.Depth, imClone.NumberOfChannels);
            UMat diff = new UMat();

            int rowLimit = res0.Rows - 1;
            int colLimit = res0.Cols - 1;
            do
            {
                res0 = ThinningIteration(res0, 0, rowLimit, colLimit);
                res0 = ThinningIteration(res0, 1, rowLimit, colLimit);
                CvInvoke.AbsDiff(res0, prev, diff);
                res0.CopyTo(prev);

            } while (CvInvoke.CountNonZero(diff) > 0);

            UMat resultFinal = new UMat();

            CvInvoke.ConvertScaleAbs(res0, resultFinal, 255.0, 0);

            return resultFinal;


        }

        public static double AverageBendingEnergy(VectorOfPoint vp)
        {

            List<Vector2D> firstDerivativeList = GetDerivativeList(vp);
            List<Vector2D> secondDerivativeList = GetDerivativeList(firstDerivativeList);
            List<double> sumList = new List<double>();
            int limit = firstDerivativeList.Count > secondDerivativeList.Count
                ? secondDerivativeList.Count : firstDerivativeList.Count;

            for (int i = 0; i < limit; i++)
            {
                Vector2D firstDerivativePt = firstDerivativeList[i];
                Vector2D secondDerivativePt = secondDerivativeList[i];

                double curvature = Curvature(firstDerivativePt, secondDerivativePt);
                sumList.Add(curvature * curvature);
            }
            
            return Mean(sumList);
        }


        public static double Mean(List<double> vals)
        {
            double sumTot = 0.0;

            vals.ForEach(v => sumTot += v);

            double n = (double)vals.Count;

            return sumTot;

        }

        public static double Mean(LinkedList<double> vals)
        {
            double sumTot = 0.0;

            foreach (double v in vals)
                sumTot += v;

            double n = (double)vals.Count;

            return sumTot;

        }

        public static double SigmaBiased(List<double> vals)
        {
            double mean = Mean(vals);
            double n = (double)vals.Count;
            double x = 0.0;

            vals.ForEach(d => x += (d - mean) * (d - mean));

            return Math.Sqrt(x / (n));
        }

        public static double Sigma(List<double> vals, double mean)
        {
            double n = (double)vals.Count;
            double x = 0.0;

            vals.ForEach(d => x += (d - mean) * (d - mean));

            return Math.Sqrt(x / (n - 1));
        }

        public static double Curvature(Vector2D firstDerivative, Vector2D secondDerivative)
        {
            double numerator = Math.Abs(firstDerivative.X * secondDerivative.Y -
                firstDerivative.Y * secondDerivative.X);

            double x2 = firstDerivative.X * firstDerivative.X;
            double y2 = firstDerivative.Y * firstDerivative.Y;

            double POWER = 1.5;
            double denominator = Math.Pow(x2 + y2, POWER);

            return numerator / denominator;

        }

        public static List<Vector2D> GetDerivativeList(List<Vector2D> vl)
        {
            List<Vector2D> derivativeList = new List<Vector2D>();

            for (int i = 0; i < vl.Count - 1; i++)
            {
                int x0 = vl[i].X;
                int x1 = vl[i + 1].X;

                int y0 = vl[i].Y;
                int y1 = vl[i + 1].Y;

                int delX = x1 - x0;
                int delY = y1 - y0;

                derivativeList.Add(new Vector2D(delX, delY));

            }

            return derivativeList;
        }


        public static List<Vector2D> GetDerivativeList(VectorOfPoint vp)
        {
            List<Vector2D> derivativeList = new List<Vector2D>();
            for (int i = 0; i < vp.Size - 1; i++)
            {
                int x0 = vp[i].X;
                int x1 = vp[i + 1].X;

                int y0 = vp[i].Y;
                int y1 = vp[i + 1].Y;

                int delX = x1 - x0;
                int delY = y1 - y0;

                derivativeList.Add(new Vector2D(delX, delY));
            }

            return derivativeList;
        }

        public static int NumOfInflectionPoints(VectorOfInt hull) 
        {
            return hull.Size;
        }

        public static int NumOfInflectionPoints(VectorOfPoint hull)
        {
            return hull.Size;
        }

        public static double AspectRatio(VectorOfPoint contour, RotatedRect rotRect) 
        {
            PointF[] vertices = rotRect.GetVertices();
            List<double> myDists = new List<double>();

            for (int j = 0; j < 4; j++)
                myDists.Add(DistPoint(vertices[j], vertices[(j + 1) % 4]));

            double rectPerim =  myDists.Min() + myDists.Max();

            double contourPerim = CvInvoke.ArcLength(contour, true);

            return contourPerim / rectPerim;
        }

        public static bool PointInRect(Rectangle rect, Point pt)
        {
            // A is upper left point
            // B is upper right point
            // C is lower right point
            // D is lower left point

            int Ax = rect.Location.X;
            int Ay = rect.Location.Y;

            int Bx = rect.Location.X + rect.Width;
            int By = rect.Location.Y;

            int Dx = rect.Location.X;
            int Dy = rect.Location.Y + rect.Height;

            int Mx = pt.X;
            int My = pt.Y;

            Vector2D AB = new Vector2D(Bx - Ax, By - Ay);
            Vector2D AD = new Vector2D(Dx - Ax, Dy - Ay);
            Vector2D AM = new Vector2D(Mx - Ax, My - Ay);

            int dotAMAB = AM.X * AB.X + AM.Y * AB.Y;
            int dotABAB = AB.X * AB.X + AB.Y * AB.Y;
            int dotAMAD = AM.X * AD.X + AM.Y * AD.Y;
            int dotADAD = AD.X * AD.X + AD.Y * AD.Y;

            bool v1 = 0 < dotAMAB && dotAMAB <= dotABAB;
            bool v2 = 0 < dotAMAD && dotAMAD <= dotADAD;

            return v1 && v2;
        }

        public static double Compactness(VectorOfPoint contour) 
        {
            double area = CvInvoke.ContourArea(contour);
            double perim = CvInvoke.ArcLength(contour, true);

            return Math.Sqrt(area) / perim;

        }

        public uint[] HorizontalCrossings(UMat input) 
        {
            int MIN_THRESH_DIFF = 120;
            Image<Gray, Byte> img = input.ToImage<Gray, Byte>();

            int height = img.Height;
            int width = img.Width;

            int h0 = height / 6;
            int h1 = 3 * height / 6;
            int h2 = 5 * height / 6;

            uint count0 = 0;
            uint count1 = 0;
            uint count2 = 0;

            for (int col = 1; col < width - 1; col++) 
            {
                int diff0 = Math.Abs(img.Data[h0, col, 0] - img.Data[h0, col - 1, 0]);
                int diff1 = Math.Abs(img.Data[h1, col, 0] - img.Data[h1, col - 1, 0]);
                int diff2 = Math.Abs(img.Data[h2, col, 0] - img.Data[h2, col - 1, 0]);

                if (diff0 >= MIN_THRESH_DIFF) count0++;
                if (diff1 >= MIN_THRESH_DIFF) count1++;
                if (diff2 >= MIN_THRESH_DIFF) count2++;
            }

            return new[]{count0, count1, count2};

        }

        public static double Eccentricity(VectorOfPoint contour, MCvMoments moments) 
        {
            int xCenter = (int)(moments.M10 / moments.M00);
            int yCenter = (int)(moments.M01 / moments.M00);
         
            Point centroid = new Point(xCenter, yCenter);
            List<double> diffXToCenterX = new List<double>();
            List<double> diffYToCenterY = new List<double>();

            int N = contour.Size;
            int Nd = N;

            for (int i = 0; i < N; i++) 
            {
                diffXToCenterX.Add(contour[i].X - xCenter);
                diffYToCenterY.Add(contour[i].Y - yCenter);
            }

            List<double> cxxList = new List<double>();
            List<double> cxyList = new List<double>();
            List<double> cyxList = new List<double>(); 
            List<double> cyyList = new List<double>();

            diffXToCenterX.ForEach(val => cxxList.Add(val * val));
            diffYToCenterY.ForEach(val => cyyList.Add(val * val));

            for (int i = 0; i < N; i++) 
            {
                cxyList.Add(diffXToCenterX[i] * diffYToCenterY[i]);
                cyxList.Add(diffYToCenterY[i] * diffXToCenterX[i]);
            }

            double cxx = cxxList.Sum()/Nd;
            double cxy = cxyList.Sum()/Nd;
            double cyx = cyxList.Sum()/Nd;
            double cyy = cyyList.Sum()/Nd;

            double a = (cxx + cyy) * (cxx + cyy);
            double b = 4.0d * (cxx * cyy - cxy * cxy);

            double A = cxx + cxy;
            double B = a - b > 0.0 ? Math.Sqrt(a - b) : 0.0;

            double lambda1 = 0.5d * (A + B);
            double lambda2 = 0.5d * (A - B);

            return lambda2 / lambda1;

        }



        public static UMat ThinningIteration(UMat im, int iter, int rowLimit, int colLimit)
        {
            Image<Gray, Byte> imGray = im.Clone().ToImage<Gray, Byte>();


            UMat marker = new UMat(im.Size, im.Depth, im.NumberOfChannels);
            Image<Gray, Byte> markerImg = marker.ToImage<Gray, Byte>();

            byte[, ,] imgGrayData = imGray.Data;

            for (int i = 1; i < rowLimit; i++)
            {
                for (int j = 1; j < colLimit; j++)
                {

                    byte p2 = imgGrayData[i - 1, j, 0];
                    byte p3 = imgGrayData[i - 1, j + 1, 0];
                    byte p4 = imgGrayData[i, j + 1, 0];
                    byte p5 = imgGrayData[i + 1, j + 1, 0];
                    byte p6 = imgGrayData[i + 1, j, 0];
                    byte p7 = imgGrayData[i + 1, j - 1, 0];
                    byte p8 = imgGrayData[i, j - 1, 0];
                    byte p9 = imgGrayData[i - 1, j - 1, 0];



                    int A = BoolCount(p2 == 0 && p3 == 1) + BoolCount(p3 == 0 && p4 == 1) +
                            BoolCount(p4 == 0 && p5 == 1) + BoolCount(p5 == 0 && p6 == 1) +
                            BoolCount(p6 == 0 && p7 == 1) + BoolCount(p7 == 0 && p8 == 1) +
                            BoolCount(p8 == 0 && p9 == 1) + BoolCount(p9 == 0 && p2 == 1);

                    int B = p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9;

                    int m1 = iter == 0 ? (p2 * p4 * p6) : (p2 * p4 * p8);
                    int m2 = iter == 0 ? (p4 * p6 * p8) : (p2 * p6 * p8);

                    if (A == 1 && (B >= 2 && B <= 6) && m1 == 0 && m2 == 0)
                        markerImg.Data[i, j, 0] = 1;
                }


            }

            //im &= ~marker;
            UMat result0 = new UMat();
            CvInvoke.BitwiseNot(markerImg.ToUMat(), result0);
            UMat imgClone = im.Clone();
            UMat result1 = new UMat();

            CvInvoke.BitwiseAnd(imgClone, result0, result1);

            return result1;

        }

        public static List<Point> GeneratePointInBet(Point pt0, Point pt1, int N)
        {
            List<Point> resultList = new List<Point>();

            int x0 = pt0.X;
            int y0 = pt0.Y;

            int x1 = pt1.X;
            int y1 = pt1.Y;

            for (int k = 1; k <= N; k++)
            {

                double kd = k;
                double xDeld = x1 - x0;
                double yDeld = y1 - y0;
                double Nd = N;


                double xd = (kd * xDeld / Nd) + x0;
                double yd = (kd * yDeld / Nd) + y0;

                double xRounded = Math.Round(xd, 0);
                double yRounded = Math.Round(yd, 0);

                Point pt = new Point((int)xRounded, (int)yRounded);
                resultList.Add(pt);
            }

            return resultList;
        }

        public static Tuple<Point, Point> AssignLeftRightPt(Point pt0, Point pt1)
        {
            if (pt0.X < pt1.X) return new Tuple<Point, Point>(pt0, pt1);
            else return new Tuple<Point, Point>(pt1, pt0);
        }

        public static Tuple<Point, Point> AssignUpDownPt(Point pt0, Point pt1)
        {
            if (pt0.Y < pt1.Y) return new Tuple<Point, Point>(pt0, pt1);
            else return new Tuple<Point, Point>(pt1, pt0);
        }


        public static List<List<Point>> MinSideCrossings(RotatedRect rect, int minSlice )
        {
            PointF[] vertices = rect.GetVertices();

            List<Tuple<PointF, PointF>> listOfLines = new List<Tuple<PointF, PointF>>();

            for (int j = 0; j < 4; j++)
            {

                Tuple<PointF, PointF> line = new Tuple<PointF, PointF>(vertices[j], vertices[(j + 1) % 4]);
                listOfLines.Add(line);
            }

            List<Tuple<Point, Point>> lor = new List<Tuple<Point, Point>>();

            foreach (Tuple<PointF, PointF> tp in listOfLines)
            {
                int x0 = (int)tp.Item1.X;
                int y0 = (int)tp.Item1.Y;
                Point pt0 = new Point(x0, y0);

                int x1 = (int)tp.Item2.X;
                int y1 = (int)tp.Item2.Y;

                Point pt1 = new Point(x1, y1);


                //Tuple<Point, Point> orderedPt = AssignUpDownPt(pt0, pt1);
                //orderedPt = AssignLeftRightPt(pt0, pt1);
                Tuple<Point, Point> orderedPt = AssignLeftRightPt(pt0, pt1);
                orderedPt = AssignUpDownPt(pt0, pt1);
                lor.Add(orderedPt);

            }

            lor.Sort((x, y) => Distance(x.Item1, x.Item2).CompareTo(Distance(y.Item1, y.Item2)));

            List<Point> lhs = GeneratePointInBet(lor[0].Item1, lor[0].Item2, minSlice);
            List<Point> rhs = GeneratePointInBet(lor[1].Item1, lor[1].Item2, minSlice);

            List<List<Point>> resultPoints = new List<List<Point>>();

            for (int i = 0; i < lhs.Count; i++)
            {

                int maxSample = (int)Math.Floor(Distance(lhs[i], rhs[i]));
                List<Point> lineE2E = GeneratePointInBet(lhs[i], rhs[i], maxSample);

                resultPoints.Add(lineE2E);

            }

            return resultPoints;
        }

        public static void GrassFire(IInputOutputArray input, IOutputArray points, IOutputArray hierarchy) 
        {
            CvInvoke.FindContours(input, points, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                
        }


        public static UMat CropByRotRect(UMat parent, RotatedRect rotRect, double alterAngle, string tilt) 
        {

            string FIRST_DIR_RIGHT = "Right";
            string FIRST_DIR_LEFT = "Left";
            UMat m = new UMat();
            UMat rotated = new UMat();
            UMat cropped = new UMat();

            float angle = 0;
            SizeF rotRectSize = rotRect.Size;


            string m0 = "Angle 0: " + Math.Round(angle, 2).ToString();
            string m1 = "Width: " + Math.Round(rotRectSize.Width, 2);
            string m2 = "Height: " + Math.Round(rotRectSize.Height, 2);
            //string m1 = "Angle 1: " + Math.Round(alterAngle, 2).ToString();
            string message = m0 + "\t" + m1 + "\t" + m2;

            Debug.WriteLine(message);
            
            
            int width = rotRectSize.Width > rotRectSize.Height ? (int)rotRectSize.Width : (int)rotRectSize.Height;
            int height = rotRectSize.Width < rotRectSize.Height ? (int)rotRectSize.Width : (int)rotRectSize.Height;
            //int width = 

            //Size finalSize = new Size((int)rotRectSize.Width, (int)rotRectSize.Height);

             
            //Size finalSize = new Size((int)rotRectSize.Width, (int)(rotRectSize.Height + rotRectSize.Height/20));

            Size finalSize = new Size(width, height + height/20);
            
            //Size finalSize = new Size(side, side);


            if (tilt.Equals(FIRST_DIR_RIGHT))
                angle = (float)(-1.0 * alterAngle);
            else if (tilt.Equals(FIRST_DIR_LEFT))
                angle = (float)alterAngle;
            else angle = 0;

            

            CvInvoke.GetRotationMatrix2D(rotRect.Center, angle, 1.0, m);
            CvInvoke.WarpAffine(parent, rotated, m, parent.Size, Inter.Cubic);

            CvInvoke.GetRectSubPix(rotated, finalSize, rotRect.Center, cropped);


            return cropped;
        
        }

    }

}
