using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using Tesseract;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace tilt_reader
{
    public partial class FormTiltReaderMain : Form
    {
        private bool _videoRunning = false;
        private Bitmap _bmInitImg;
        private Bitmap _bmCanny;
        private Bitmap _bmEroded;
        private Bitmap _bmGrassfire;
        private Bitmap _bmDilated;
        private Bitmap _bmLaplace;

        private TesseractEngine _tessEngine;
        //private Emgu.CV.OCR.Tesseract _ocr;

        private const string LABEL_START = "Start";
        private const string LABEL_STOP = "Stop";
        
        private readonly string FIRST_DIR_RIGHT = "Right";
        private readonly string FIRST_DIR_LEFT = "Left";
        private readonly string FIRST_DIR_NONE = "None";

        private readonly string DIR_IMG_ORIENT_LOG = "image_orientation";
        private const string DIR_IMG_LOG = "image_log";
        private const string DIR_IMG_180_LOG = "image180_log";
        
        private readonly string DIR_ANGLE_LOG = "angle_logger";
        private readonly string FILE_ANGLE_LOG = "angle_logger.csv";
        private string _filePathAngle;
        
        private VideoCapture _capture;

        private MCvScalar _mcvBlue = new MCvScalar(255, 0, 0);
        private MCvScalar _mcvBlack = new MCvScalar(0, 0, 0);
        private MCvScalar _mcvRed = new MCvScalar(0, 0, 255);
        private MCvScalar _mcvGreen = new MCvScalar(0, 255, 0);
        private MCvScalar _mcvYellow = new MCvScalar(51.0, 255.0, 255.0);
        private MCvScalar _mcvWhite = new MCvScalar(255.0, 255.0, 255.0);
        private MCvScalar _mcvTextColor = new MCvScalar(169, 192, 229);


        private int DEBUG_CHANNELS_ARG0 = 0;
        private int DEBUG_CHANNELS_ARG1 = 0;
        private int DEBUG_CHANNELS_ARG2 = 0;

        private int DEBUG_NON_ZEROES_ARG0 = 0;
        private int DEBUG_NON_ZEROES_ARG1 = 0;
        private int DEBUG_NON_ZEROES_ARG2 = 0;


        private DepthType DEBUG_DEPTH_TYPE_ARG0;
        private DepthType DEBUG_DEPTH_TYPE_ARG1;
        private DepthType DEBUG_DEPTH_TYPE_ARG2;

        private Size DEBUG_SIZE_ARG0;
        private Size DEBUG_SIZE_ARG1;
        private Size DEBUG_SIZE_ARG2;

        private const int DEFAULT_OCR_HEIGHT = 40;

        List<Tuple<UMat, UMat>> _normalRotatedImgs = new List<Tuple<UMat, UMat>>();
        List<UMat> _rotated180Imgs = new List<UMat>();
        
        //private Thread _ocrThread;

        public FormTiltReaderMain()
        {
            InitializeComponent();

            //rbAngle.Checked = true;
            rbText.Checked = true;
            cbActivate180.Checked = true;
            CvInvoke.UseOpenCL = true;
            //CvInvoke.UseOptimized = true;

            if (!Directory.Exists(DIR_IMG_ORIENT_LOG))
                Directory.CreateDirectory(DIR_IMG_ORIENT_LOG);

            if (!Directory.Exists(DIR_ANGLE_LOG))
                Directory.CreateDirectory(DIR_ANGLE_LOG);

            if (!Directory.Exists(DIR_IMG_180_LOG))
                Directory.CreateDirectory(DIR_IMG_180_LOG);

            _filePathAngle = Path.Combine(DIR_ANGLE_LOG, FILE_ANGLE_LOG);

            if (!File.Exists(_filePathAngle))
                File.Create(_filePathAngle).Close();


            _tessEngine = new TesseractEngine(@"./tessdata","eng", EngineMode.TesseractAndCube);
            _tessEngine.DefaultPageSegMode = Tesseract.PageSegMode.SingleLine;
            _tessEngine.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
            //_tessEngine.DefaultPageSegMode = Tesseract.PageSegMode.SingleLine;

            cb_bbNormal.Checked = true;
            cb_TilBB.Checked = true;

        }

        private void testerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void detectionTesterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void VideoRun() 
        {
            using (Mat initImgMat = _capture.QueryFrame())
            {       
                
                Image<Bgr, Byte> initImg = initImgMat.ToImage<Bgr, Byte>();
                
                
                Image<Hsv, Byte> initImgHSV = initImgMat.ToImage<Hsv, Byte>();

                UMat imgUMatHSV = initImgHSV.ToUMat();
                UMat[] hsvSplit = imgUMatHSV.Split();
                UMat hsvValue = hsvSplit[2];

                UMat umatBlur = new UMat(hsvValue.Size, hsvValue.Depth, hsvValue.NumberOfChannels);
                CvInvoke.MedianBlur(hsvValue, umatBlur, 5);

                UMat umatCanny = new UMat(hsvValue.Size, hsvValue.Depth, hsvValue.NumberOfChannels);
                UMat umatThreshold = new UMat(hsvValue.Size, hsvValue.Depth, hsvValue.NumberOfChannels);
                UMat umatClosed = new UMat(hsvValue.Size, hsvValue.Depth, hsvValue.NumberOfChannels);
                UMat umatBinary = new UMat(hsvValue.Size, hsvValue.Depth, hsvValue.NumberOfChannels);

                UMat umatSobel = new UMat(hsvValue.Size, hsvValue.Depth, hsvValue.NumberOfChannels);
                UMat umatLaplace = new UMat(hsvValue.Size, hsvValue.Depth, hsvValue.NumberOfChannels);

                CvInvoke.MedianBlur(hsvSplit[2], umatBlur, 5);

                CvInvoke.Canny(umatBlur, umatCanny, 50.0, 255.0);
                UMat umatDenoised = new UMat();
                CvInvoke.BoxFilter(umatCanny, umatDenoised, DepthType.Default, new Size(3, 3), new Point(-1, -1), false, BorderType.Replicate);

                CvInvoke.Threshold(umatDenoised, umatBinary, 150.0, 255.0, ThresholdType.ToZero);
                UMat umatBinCopy = umatBinary.Clone();

                UMat element0 = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(25, 25), new Point(-1, -1)).GetUMat(AccessType.Fast);


                UMat element1 = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(3, 3), new Point(-1, -1)).GetUMat(AccessType.Fast);
                UMat element2 = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(3, 3), new Point(-1, -1)).GetUMat(AccessType.Fast);
                Point anchor = new Point(-1, -1);

                CvInvoke.MorphologyEx(umatBinary, umatClosed, MorphOp.Close, element0, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0));

                UMat umatEroded = new UMat(hsvValue.Size, hsvValue.Depth, hsvValue.NumberOfChannels);
                UMat umatDilated = new UMat(hsvValue.Size, hsvValue.Depth, hsvValue.NumberOfChannels);

                CvInvoke.Erode(umatClosed, umatEroded, element1, anchor, 5, BorderType.Default, new MCvScalar(255));
                CvInvoke.Dilate(umatEroded, umatDilated, element2, anchor, 9, BorderType.Default, new MCvScalar(255));

                UMat grassfireInput = umatDilated.Clone();

                VectorOfVectorOfPoint vp = new VectorOfVectorOfPoint();
                UMat umatHierchy = new UMat();
               
                EmguCVFunc.GrassFire(grassfireInput, vp, umatHierchy);
                List<VectorOfPoint> hulls = new List<VectorOfPoint>();
                VectorOfVectorOfPoint contours_poly = new VectorOfVectorOfPoint(vp.Size);


                try
                {
                    List<Tuple<string, double, Rectangle, RotatedRect>> tiltProperties = EmguCVFunc.TiltOrientations(umatBinary, vp);
                    _normalRotatedImgs.Clear();

                    List<Tuple<Point, RotatedRect,Bitmap>> listLocRotBM = new List<Tuple<Point, RotatedRect, Bitmap>>();
                    List<Tuple<Point, RotatedRect, Bitmap>> listLocRotBM180 = new List<Tuple<Point, RotatedRect, Bitmap>>();

                    List<Tuple<Point, RotatedRect, UMat>> listLocRotU = new List<Tuple<Point, RotatedRect, UMat>>();
                    List<Tuple<Point, RotatedRect, UMat>> listLocRotU180 = new List<Tuple<Point, RotatedRect, UMat>>();


                    foreach (Tuple<string, double, Rectangle, RotatedRect> tp in tiltProperties)
                    {
                        double areaRect = EmguCVFunc.RotatedRectArea(tp.Item4);
                        double totalArea = (double)(umatBinary.Size.Height) * (double)(umatBinary.Size.Width);
                        double areaRatio = areaRect / totalArea;

                        Tuple<PointF, PointF> longestPoints = EmguCVFunc.LongestPointPair(tp.Item4);

                        double longestDistHeight = tp.Item3.Height;
                        double distRatioHeight = longestDistHeight / (double)umatBinary.Size.Height;

                        double longestDistWidth = tp.Item3.Width;
                        double distRatioWidth = longestDistWidth / (double)umatBinary.Size.Width;

                        try
                        {

                            if (areaRatio > 0.009 && areaRatio < 0.5)
                            {
                                UMat childTmp = EmguCVFunc.CropByRotRect(hsvValue, tp.Item4, tp.Item2, tp.Item1);
                                UMat child = new UMat(childTmp.Size, DepthType.Cv64F, childTmp.NumberOfChannels);
                                childTmp.ConvertTo(child, DepthType.Cv64F);

                                PointF pt = new PointF((float)child.Size.Width / 2.0f, (float)child.Size.Height / 2.0f);

                                UMat childToBeRotatedTmp = child.Clone();
                                UMat childToBeRotated = new UMat(childTmp.Size, DepthType.Cv32F, childTmp.NumberOfChannels);
                                childToBeRotatedTmp.ConvertTo(childToBeRotated, DepthType.Cv32F);

                                UMat map = new UMat(new Size(2, 3), DepthType.Cv32F, 1);
                                UMat dest = child.Clone();


                                _normalRotatedImgs.Add(new Tuple<UMat, UMat>(child, dest));
                                List<List<Point>> lvp = EmguCVFunc.MinSideCrossings(tp.Item4, 4);

                                if (rbText.Checked)
                                {

                                    UMat resizedImg = new UMat();

                                    double resizeScale = (double)dest.Size.Width / (double)dest.Size.Height;
                                    int resizeWidth = (int)((double)DEFAULT_OCR_HEIGHT * resizeScale);

                                    if (DEFAULT_OCR_HEIGHT < childTmp.Size.Width)
                                    {
                                        CvInvoke.Resize(childTmp, resizedImg, new Size(resizeWidth, DEFAULT_OCR_HEIGHT), 0, 0, Inter.Area);
                                    }
                                    else resizedImg = childTmp.Clone();

                                    Tuple<Point, RotatedRect, Bitmap> locBM = new Tuple<Point, RotatedRect, Bitmap>(tp.Item3.Location, tp.Item4, resizedImg.Bitmap);
                                    listLocRotBM.Add(locBM);

                                    Tuple<Point, RotatedRect, UMat> locRecU = new Tuple<Point, RotatedRect, UMat>(tp.Item3.Location, tp.Item4, resizedImg);
                                    listLocRotU.Add(locRecU);
                                }
                                else
                                {
                                    CvInvoke.PutText(initImg, tp.Item1 + ": " + Math.Round(tp.Item2, 2).ToString(), tp.Item3.Location, FontFace.HersheyComplex, 0.7, _mcvYellow);

                                    if (cb_TilBB.Checked) 
                                    {
                                        DrawSpecial.DrawRotRect(initImg, tp.Item4, _mcvBlue, 2);
                                    }

                                    if (cb_bbNormal.Checked) 
                                    {
                                        CvInvoke.Rectangle(initImg, tp.Item4.MinAreaRect(), _mcvBlue, 2);
                                    }
                                    
                                } 
                            }

                        }
                        catch (Exception exe)
                        {
                            string nonzero0 = "Arg0 Non-Zeroes: " + DEBUG_NON_ZEROES_ARG0.ToString() + "\n";
                            string nonzero1 = "Arg1 Non-Zeroes: " + DEBUG_NON_ZEROES_ARG1.ToString() + "\n";
                            string nonzero2 = "Arg2 Non-Zeroes: " + DEBUG_NON_ZEROES_ARG2.ToString() + "\n";

                            string depthType0 = "Arg0 Depth Type: " + DEBUG_DEPTH_TYPE_ARG0.ToString() + "\n";
                            string depthType1 = "Arg1 Depth Type: " + DEBUG_DEPTH_TYPE_ARG1.ToString() + "\n";
                            string depthType2 = "Arg2 Depth Type: " + DEBUG_DEPTH_TYPE_ARG2.ToString() + "\n";

                            string size0 = "Arg0 Size: " + DEBUG_SIZE_ARG0.ToString() + "\n";
                            string size1 = "Arg1 Size: " + DEBUG_SIZE_ARG1.ToString() + "\n";
                            string size2 = "Arg2 Size: " + DEBUG_SIZE_ARG2.ToString() + "\n";

                            string message = exe.Message + "\n";

                            MessageBox.Show(nonzero0 + nonzero1 + nonzero2 +
                                            depthType0 + depthType1 + depthType2 +
                                            size0 + size1 + size2 +
                                            message);
                        }
                    }
                    // NOPE (FAGGITH)
                    List<Tuple<Point, RotatedRect, string>> ocrList = new List<Tuple<Point, RotatedRect, string>>();
                    List<Tuple<Point, RotatedRect, string>> ocrList180 = new List<Tuple<Point, RotatedRect, string>>();

                    foreach (Tuple<Point, RotatedRect, UMat> locRotU in listLocRotU)
                    {
                        using (var imgTxt = locRotU.Item3.Bitmap)
                        {
                            
                            using (var page = _tessEngine.Process(imgTxt))
                            {

                                string textRead = page.GetText();
                                float confidence = page.GetMeanConfidence();

                                if (confidence > 0.81) 
                                {
                                    if (textRead.Length > 2)
                                    {
                                        string finalRead = textRead.Substring(0, textRead.Length - 2);
                                        ocrList.Add(new Tuple<Point, RotatedRect, string>(locRotU.Item1, locRotU.Item2, finalRead));

                                    }
                                
                                }
                                else
                                   listLocRotU180.Add(locRotU);
                                
                            }
                        }
                    };


                    if (cbActivate180.Checked) 
                    {
                        _rotated180Imgs.Clear();

                        foreach (var locRotU in listLocRotU180)
                        {
                            UMat flipped = new UMat();
                            CvInvoke.Flip(locRotU.Item3, flipped, FlipType.Vertical);
                            CvInvoke.Flip(flipped, flipped, FlipType.Horizontal);

                            _rotated180Imgs.Add(flipped);

                            using (var imgTxt = flipped.Bitmap)
                            {

                                using (var page = _tessEngine.Process(imgTxt))
                                {

                                    
                                    string textRead = page.GetText();
                                    float confidence = page.GetMeanConfidence();

                                    if (confidence > 0.8)
                                    {
                                        if (textRead.Length > 2)
                                        {
                                            string finalRead = textRead.Substring(0, textRead.Length - 2);
                                            ocrList.Add(new Tuple<Point, RotatedRect, string>(locRotU.Item1, locRotU.Item2, finalRead));

                                        }
                                    }
                                }

                            }
                        }
                    
                    }

                    ocrList.ForEach(prs => DrawSpecial.DrawRotRect(initImg, prs.Item2, _mcvBlue, 2));
                    ocrList.ForEach(prs => CvInvoke.PutText(initImg, prs.Item3, prs.Item1, FontFace.HersheyDuplex, 0.7, _mcvGreen,1));

                }
                catch (Exception e)
                {
                    string message = e.Message;
                    string stackTrace = e.StackTrace;

                    MessageBox.Show(message + "\n" + stackTrace);
                }

                _bmInitImg = initImg.ToBitmap();
                _bmCanny = umatBinary.Bitmap;
                _bmEroded = umatEroded.Bitmap;
                _bmGrassfire = grassfireInput.Bitmap;
                _bmDilated = umatDilated.Bitmap;
                
                if (rbNormal.Checked && _videoRunning)
                {
                    pictureBoxVideo.Image = _bmInitImg;
                }
                else if (rbCanny.Checked && _videoRunning)
                {
                    pictureBoxVideo.Image = _bmCanny;
                }
                else if (rbEroded.Checked && _videoRunning)
                {
                    pictureBoxVideo.Image = _bmEroded;
                }
                else if (rbGrassfire.Checked && _videoRunning)
                {
                    pictureBoxVideo.Image = _bmGrassfire;
                }
                else if (rbDilated.Checked && _videoRunning)
                {
                    pictureBoxVideo.Image = _bmDilated;
                }
                
            }
        
        }

        private void ProcessFrame_Idle(object sender, EventArgs arg) 
        {
            var watch = Stopwatch.StartNew();
            VideoRun();
            watch.Stop();
            long elapsedTime = watch.ElapsedMilliseconds;
            long fps = 1000L / elapsedTime;
            labelFPS.Text = fps.ToString();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text.Equals(LABEL_START))
            {
                // init camera
                _capture = new VideoCapture(0);

                if (_capture.QueryFrame() != null) 
                {
                    if (!Directory.Exists(DIR_IMG_LOG))
                        Directory.CreateDirectory(DIR_IMG_LOG);

                    int height = pictureBoxVideo.Size.Height;
                    int width = pictureBoxVideo.Size.Width;

                    _capture.SetCaptureProperty(CapProp.Fps, 30);
                    //_capture.SetCaptureProperty(CapProp.FrameHeight, 480);
                    //_capture.SetCaptureProperty(CapProp.FrameWidth, 1280);
                    Application.Idle += ProcessFrame_Idle;
                    btnStartStop.Text = LABEL_STOP;
                    _videoRunning = true;

                    
                }

            }
            else if (btnStartStop.Text.Equals(LABEL_STOP)) 
            {
               // stop camera
                if (_capture != null) 
                {
                    _capture.Dispose();
                    Application.Idle -= ProcessFrame_Idle;
                    btnStartStop.Text = LABEL_START;
                    _videoRunning = false;
                }
            }
        }

        private void trackBarSymmetry_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void pictureBoxVideo_Click(object sender, EventArgs e)
        {
           
        }

        private void detectionTrainToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void timerImageLog_Tick(object sender, EventArgs e)
        {
            int counter = 1;
            int counter180 = 1;

            if(cbOrientation.Checked)
            {
                Debug.WriteLine("Image Orientation Logging..");

                foreach (Tuple<UMat, UMat> nri in _normalRotatedImgs)
                {
                    string imgLogPath = Path.Combine(DIR_IMG_LOG, Misc.GenDateTimeStr() + (++counter).ToString());

                    nri.Item1.Save(imgLogPath + "-nor.jpg");
                    nri.Item2.Save(imgLogPath + "-rot.jpg");
                }

                foreach (var r180 in _rotated180Imgs) 
                {
                    string imgLogPath = Path.Combine(DIR_IMG_180_LOG, Misc.GenDateTimeStr() + (++counter180).ToString());
                    r180.Save(imgLogPath + ".jpg");
                }
            
            }
            

        }

        private void cbOrientation_CheckedChanged(object sender, EventArgs e)
        {

            if (cbOrientation.Checked) timerImageLog.Start();
            else timerImageLog.Stop();
        }

        private void rbNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNormal.Checked && !_videoRunning)
                pictureBoxVideo.Image = _bmInitImg;
        }

        private void rbCanny_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCanny.Checked && !_videoRunning)
                pictureBoxVideo.Image = _bmCanny;
        }

        private void rbEroded_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEroded.Checked && !_videoRunning)
                pictureBoxVideo.Image = _bmEroded;
        }

        private void rbDilated_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDilated.Checked && !_videoRunning)
                pictureBoxVideo.Image = _bmDilated;


            
        }

        private void rbValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGrassfire.Checked && !_videoRunning)
                pictureBoxVideo.Image = _bmGrassfire;
        }

        private bool AllBlanks(string input)
        {
            char blank = ' ';

            foreach (char c in input)
                if (c.Equals(blank)) return false;

            return true;

        }

        private string ListToCSV(List<string> values) 
        {
            StringBuilder sb = new StringBuilder();

            foreach (var str in values) 
            {
                sb.Append(str);
                sb.Append(",");
            }

            return sb.ToString();
        
        }
    }
}
