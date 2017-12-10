using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Shape;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using Emgu.CV.Util;
using Emgu.CV.Cuda;

namespace tilt_reader
{
    
    public class TiltDetectionCollection
    {     

    }

    /// <summary>
    /// Class for Tilt Detection per image
    /// </summary>
    public class TiltDetection 
    {
        public UMat InputImg { get; set; }
        public Tuple<string, string> TiltStatus { get; set; }

        private readonly string FIRST_YES = "Yes";
        private readonly string FIRST_NO = "Yes";
        private readonly string FIRST_CONFLICT = "Yes";
        private readonly string FIRST_DIR_RIGHT = "Right";
        private readonly string FIRST_DIR_LEFT = "Left";
        private readonly string FIRST_DIR_A = "a";
        private readonly string FIRST_DIR_B = "b";
        private readonly string FIRST_DIR_C = "c";
        private readonly string FIRST_DIR_D = "d";
        private readonly string FIRST_DIR_E = "e";
        private readonly string FIRST_DIR_BLANK = "-";

        private double _densityTopLeft = 0.0;
        private double _densityTopRight = 0.0;
        private double _densityBotLeft = 0.0;
        private double _densityBotRight = 0.0;


        
        public TiltDetection() 
        { }

        public TiltDetection(UMat inputImg) 
        {
            InputImg = inputImg;
            
        }

        public void ComputeTilt() 
        {
            TiltStatus = FirstStage();
            TiltStatus = SecondStage(TiltStatus);
        }

        private double ImgDensity(Size croppedSize, double nonZeroes) 
        {
            double actualArea = croppedSize.Height * croppedSize.Width;
            return nonZeroes / actualArea;
        }

        private bool PresenceOfChar(Size croppedSize, double nonZeroes) 
        {
            double total = croppedSize.Width * croppedSize.Height;
            double ratio = nonZeroes / total;

            if (ratio > 0.1) return true;
            else return false;
        }

        private Tuple<string, string> SecondStage(Tuple<string, string> tiltStatus) 
        {
            Tuple<string, string> result = tiltStatus;
            
            if (tiltStatus.Item1.Equals(FIRST_CONFLICT))
            {
                if (tiltStatus.Item2.Equals(FIRST_DIR_A)) 
                {
                    result = new Tuple<string, string>(FIRST_YES, FIRST_DIR_LEFT);
                }
                else if (tiltStatus.Item2.Equals(FIRST_DIR_B)) 
                {
                    result = new Tuple<string, string>(FIRST_YES, FIRST_DIR_RIGHT);
                }
                else if (tiltStatus.Item2.Equals(FIRST_DIR_C)) 
                {
                    result = new Tuple<string, string>(FIRST_YES, FIRST_DIR_RIGHT);
                }
                else if (tiltStatus.Item2.Equals(FIRST_DIR_D)) 
                {
                    result = new Tuple<string, string>(FIRST_YES, FIRST_DIR_LEFT);
                }
                
            }

            return tiltStatus;
        }

       
        private Tuple<string,string> FirstStage() 
        {
            int actual_width = InputImg.Size.Width;
            int actual_height = InputImg.Size.Height;

            int cropped_width = actual_width / 5;
            int cropped_height = InputImg.Size.Height / 5;

            Size cropped_size = new Size(cropped_width, cropped_height);

            Point locTopLeft = new Point(0, 0);
            Point locTopRight = new Point(actual_width - cropped_width - 1, 0);
            Point locBotLeft = new Point(0, actual_height - cropped_height - 1);
            Point locBotRight = new Point(actual_width - cropped_width - 1, actual_height - cropped_height - 1);

            Rectangle roiTopLeft = new Rectangle(locTopLeft, cropped_size);
            Rectangle roiTopRight = new Rectangle(locTopRight, cropped_size);
            Rectangle roiBotLeft = new Rectangle(locBotLeft, cropped_size);
            Rectangle roiBotRight = new Rectangle(locBotRight, cropped_size);

            UMat imgTopLeft = new UMat(InputImg, roiTopLeft);
            UMat imgTopRight = new UMat(InputImg, roiTopRight);
            UMat imgBotLeft = new UMat(InputImg, roiBotLeft);
            UMat imgBotRight = new UMat(InputImg, roiBotRight);

            double nonZeroesTopLeft = CvInvoke.CountNonZero(imgTopLeft);
            double nonZeroesTopRight = CvInvoke.CountNonZero(imgTopRight);
            double nonZeroesBotLeft = CvInvoke.CountNonZero(imgBotLeft);
            double nonZeroesBotRight = CvInvoke.CountNonZero(imgBotRight);

            _densityTopLeft = ImgDensity(cropped_size, nonZeroesTopLeft);
            _densityTopRight = ImgDensity(cropped_size, nonZeroesTopRight);
            _densityBotLeft = ImgDensity(cropped_size, nonZeroesBotLeft);
            _densityBotRight = ImgDensity(cropped_size, nonZeroesBotRight);

            bool pTL = _densityTopLeft > 0.1 ? true : false;
            bool pTR = _densityTopRight > 0.1 ? true : false;
            bool pBL = _densityBotLeft > 0.1 ? true : false;
            bool pBR = _densityBotRight > 0.1 ? true : false;

            if      (!pTL && pTR && pBL && !pBR) return new Tuple<string, string>(FIRST_YES, FIRST_DIR_RIGHT);
            else if (pTL && !pTR && !pBL && pBR) return new Tuple<string, string>(FIRST_YES, FIRST_DIR_LEFT);
            else if (pTL && pTR && !pBL && pBR) return new Tuple<string, string>(FIRST_CONFLICT, FIRST_DIR_A);
            else if (pTL && pTR && pBL && !pBR) return new Tuple<string, string>(FIRST_CONFLICT, FIRST_DIR_B);
            else if (pTL && !pTR && pBL && pBR) return new Tuple<string, string>(FIRST_YES,  FIRST_DIR_LEFT);
            else if (pTL && !pTR && !pBL && !pBR) return new Tuple<string, string>(FIRST_YES, FIRST_DIR_LEFT);
            else if (!pTL && !pTR && !pBL && pBR) return new Tuple<string, string>(FIRST_YES, FIRST_DIR_LEFT);
            else if (!pTL && pTR && !pBL && !pBR) return new Tuple<string, string>(FIRST_YES, FIRST_DIR_RIGHT);
            else if (!pTL && !pTR && pBL && !pBR) return new Tuple<string, string>(FIRST_YES, FIRST_DIR_RIGHT);
            else if (!pTL && pTR && pBL && pBR) return new Tuple<string, string>(FIRST_CONFLICT, FIRST_DIR_C);
            else if (pTL && pTR &&  !pBL && !pBR) return new Tuple<string, string>(FIRST_CONFLICT, FIRST_DIR_D);
            else if (!pTL && !pTR && !pBL && !pBR) return new Tuple<string, string>(FIRST_CONFLICT, FIRST_DIR_E);
            else return new Tuple<string, string>(FIRST_NO, FIRST_DIR_BLANK);
           
        }
    }

 
}
