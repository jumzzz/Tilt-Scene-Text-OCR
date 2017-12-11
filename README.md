# Tilt-Scene-Text-OCR
Reads Scene Text in Tilted orientation.

# Tested on following IDEs
- **Visual Studio 2013**
- **Visual Studio 2015**

# Dependencies
You can acquire the following using **NuGet Package Manager**
- **EMGU.CV.3.3**
- **Tesseract.3.0.2.0**

# Overview

**This is done by the following steps**

1) Initialize necessary parameters.
2) Start the image acquisition from the camera.
3) Apply Canny Algorithm and Thresholding.

![Fig. 1 - Canny Algorithm + Thresholding](https://github.com/jumzzz/Tilt-Scene-Text-OCR/blob/master/tilt_screenshots/canny.png?raw=true)
 
   *Fig. 1 - Canny Algorithm + Thresholding*

4) Apply Closed Morphology and Erosion

![Fig. 2 - Closed Morphology + Erosion](https://github.com/jumzzz/Tilt-Scene-Text-OCR/blob/master/tilt_screenshots/morphology.png?raw=true)
 
   *Fig. 2 - Closed Morphology + Erosion*

5) Apply Dilation

![Fig. 3 - Dilation](https://github.com/jumzzz/Tilt-Scene-Text-OCR/blob/master/tilt_screenshots/dilation.png?raw=true)
 
   *Fig. 3 - Dilation*

6) Find the Contours of the Dilated image. Then acquire each Region of Interests (ROI) in terms of Rotated Rectangle.

7) Determine the Tilt Orientation and Angles of each acquired Contour Rotated Rectangles.

![Fig. 4 - Tilt Orientation and Angle (in Degrees)](https://github.com/jumzzz/Tilt-Scene-Text-OCR/blob/master/tilt_screenshots/tilt_angle.png?raw=true)

   *Fig. 4 - Tilt Orientation and Angle (in Degrees)*

8) For each Contour Rotated Rectangles -- crop it from the Grayscaled original input image and apply image rotation based on its Tilt Orientation and its Tilt Angle.

9) Apply Tesseract APIs OCR, and this will be the actual result.

- This example is tilted and oriented upside down.


![Fig. 5 - Tilted and oriented upside down](https://github.com/jumzzz/Tilt-Scene-Text-OCR/blob/master/tilt_screenshots/final_out_reverse.png?raw=true)

   *Fig. 5 - Tilted and oriented upside down*

- This example is tilted to the left.

![Fig. 6 - Tilted to the left](https://github.com/jumzzz/Tilt-Scene-Text-OCR/blob/master/tilt_screenshots/final_out_left.png?raw=true)

   *Fig. 6 - Tilted to the left*

- This example is tilted to the right.

![Fig. 7 - Tilted to the right](https://github.com/jumzzz/Tilt-Scene-Text-OCR/blob/master/tilt_screenshots/final_out_right.png?raw=true)

   *Fig. 7 - Tilted to the right*


# Miscellaneous

- Make sure that Tesseract's **tessdata** folder exists inside the directory of the executable.


