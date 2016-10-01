using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging.Filters;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using AForge.Math.Random;
using AForge;

namespace ImageProccessing_wpf.ImageHelpers
{
    public static class GeneralImageMethods
    {
        public static Bitmap Crop(Bitmap input, Rectangle cropArea)
        {
            Crop crop = new Crop(cropArea);
            return crop.Apply(input);
        }
        public static Bitmap Zoom(Bitmap image, double zoomPercentage)
        {
            int newWidth = (int)Math.Floor(image.Width * zoomPercentage);
            int newHeight = (int)Math.Floor(image.Height * zoomPercentage);
            ResizeBicubic resizer = new ResizeBicubic(newWidth, newHeight);
            return resizer.Apply(image);
        }
        public static Bitmap Resize(Bitmap input, int newWidth, int newHeight)
        {           
            ResizeBicubic resizer = new ResizeBicubic(newWidth, newHeight);
            return resizer.Apply(input);
        }
        public static Bitmap DetectEdges(Bitmap image)
        {
            Edges edgeDetector = new Edges();
            return edgeDetector.Apply(image);
        }
        public static Bitmap ToGrayscale(Bitmap input)
        {
            Grayscale converter = new Grayscale(0.2125, 0.7154, 0.0721);
            return converter.Apply(input);
        }
        public static Bitmap Rotate(Bitmap input, double angle)
        {
            if (angle < 0)
            {
                angle = 360 + angle;
            }
            if (angle == 0)
            {
                return input;
            }
            RotateBicubic rotator = new RotateBicubic(angle);
            return rotator.Apply(input);
        }
        public static Bitmap AddNoiseToImage(Bitmap input)
        {
            // create random generator
            IRandomNumberGenerator generator = new UniformGenerator(new Range(-50, 50));
            // create filter
            AdditiveNoise filter = new AdditiveNoise(generator);
            // apply the filter            
            return filter.Apply(input);
        }
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        public static bool IsGrayscale(Bitmap input)
        {
            return AForge.Imaging.Image.IsGrayscale(input);
        }
        public static Bitmap AdjustHLS(Bitmap input, int hue, int saturation, int luminance)
        {
            Image<Hls, byte> hlsImage = new Image<Hls, byte>(input);
            hlsImage[0] += hue;
            hlsImage[1] += saturation;
            hlsImage[2] += luminance;
            return hlsImage.ToBitmap();
        }
        public static Bitmap AdjustRGB(Bitmap input, int red, int green, int blue)
        {
            Image<Rgb, byte> rgbImage = new Image<Rgb, byte>(input);
            rgbImage[0] += red;
            rgbImage[1] += green;
            rgbImage[2] += blue;
            return rgbImage.ToBitmap();
        }

        public static Bitmap AdjustContrastBrightness(Bitmap input, int brightnessAdjustment, int contrastAdjustment)
        {
            BrightnessCorrection bCorrector = new BrightnessCorrection(brightnessAdjustment);
            ContrastCorrection cCorrector = new ContrastCorrection(contrastAdjustment);
            return bCorrector.Apply(cCorrector.Apply(input));
        }
        public static Bitmap GammaCorrection(Bitmap input, double gammaCoeff)
        {
            if (IsGrayscale(input))
            {
                Image<Gray, Byte> image = new Image<Gray, byte>(input);
                image._GammaCorrect(gammaCoeff);
                return image.ToBitmap();
            }
            else
            {
                Image<Rgb, Byte> image = new Image<Rgb, byte>(input);
                image._GammaCorrect(gammaCoeff);
                return image.ToBitmap();
            }
        }

    }
}
