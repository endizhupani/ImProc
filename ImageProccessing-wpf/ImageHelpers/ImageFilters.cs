using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProccessing_wpf.ImageHelpers
{
    public static class ImageFilters
    {
        public static Bitmap EqualizeHistogram(Bitmap input)
        {
            HistogramEqualization equalizer = new HistogramEqualization();
            return equalizer.Apply(input);
        }
        public static Bitmap ApplyAvgDenoisingFilter(Bitmap input, bool isWeighted)
        {
            Mean meanDenoisingFilter = new Mean();
            if (isWeighted)
            {
                int[,] kernel = new int[,] {
                    {1, 2, 1 },
                    {2, 4, 2 },
                    {1, 2, 1 }
                };
                meanDenoisingFilter.Kernel = kernel;
                meanDenoisingFilter.Divisor = 16;
                input = meanDenoisingFilter.Apply(input);
            }
            else {
                input = meanDenoisingFilter.Apply(input);
            }
            return input;
        }
        public static Bitmap ApplyMedDenoisingFilter(Bitmap input)
        {
            Median medianDeNoisingFilter = new Median();
            return medianDeNoisingFilter.Apply(input);
        }
        public static Bitmap ApplySharpenFilter(Bitmap input)
        {
            Sharpen sharpenFilter = new Sharpen();
            return sharpenFilter.Apply(input);
        }
        public static Bitmap ApplyGaussianSharpenFilter(Bitmap input, int kernelSize, double sigma)
        {
            GaussianSharpen gaussianSharpenFilter = new GaussianSharpen(sigma, kernelSize);
            return gaussianSharpenFilter.Apply(input);
        }
        public static Bitmap ApplyGaussianBlurFilter(Bitmap input, int kernelSize, double sigma)
        {
            
            GaussianBlur gaussianBlurFilter = new GaussianBlur(sigma, kernelSize);
           
                return gaussianBlurFilter.Apply(input);

            //}
            //else {
            //    if (blurOutside)
            //    {
            //        int distanceToBlur = Math.Min(Math.Min(selectionX, selectionY), kernelSize);
            //        int pixelStep = Math.Max(3, distanceToBlur / kernelSize);
            //        int currentStep = pixelStep;
            //        int kernelReduction = distanceToBlur / pixelStep;
            //        Bitmap temp = (Bitmap)input.Clone();
            //        input = gaussianBlurFilter.Apply(input);
            //        while (kernelSize > 0)
            //        {
                        
            //            if (kernelSize - kernelReduction < 1)
            //            {
            //                gaussianBlurFilter.Size = 2;
            //                input = gaussianBlurFilter.Apply(input);
            //                Rectangle rect1 = new Rectangle(selectionX, selectionY, selectionWidth, selectionHeight);
            //                Bitmap selectedPortion1 = new Bitmap(rect1.Width, rect1.Height);
            //                using (Graphics g = Graphics.FromImage(selectedPortion1))
            //                {
            //                    g.DrawImage(temp, new Rectangle(0, 0, rect1.Width, rect1.Height), rect1, GraphicsUnit.Pixel);
            //                }
                            
            //                //selectedPortion = gaussianBlurFilter.Apply(selectedPortion);
            //                using (Graphics g = Graphics.FromImage(input))
            //                {
            //                    g.DrawImage(selectedPortion1, rect1);
            //                }
            //                break;
            //            }
            //            Rectangle rect = new Rectangle(selectionX-currentStep,selectionY- currentStep, input.Width+currentStep, input.Height + currentStep);
            //            Bitmap selectedPortion = new Bitmap(rect.Width, rect.Height);
            //            using (Graphics g = Graphics.FromImage(selectedPortion))
            //            {
            //                g.DrawImage(temp, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            //            }
            //            gaussianBlurFilter.Size = kernelSize;
            //            selectedPortion = gaussianBlurFilter.Apply(selectedPortion);
            //            using (Graphics g = Graphics.FromImage(input))
            //            {
            //                g.DrawImage(selectedPortion, rect);
            //            }
            //            currentStep += pixelStep;
            //            kernelSize -= kernelReduction;
            //        }
            //        //Rectangle rect = new Rectangle(selectionX, selectionY, selectionWidth, selectionHeight);
            //            //Bitmap selectedPortion = new Bitmap(rect.Width, rect.Height);
            //        //    using (Graphics g = Graphics.FromImage(selectedPortion))
            //        //    {
            //        //        g.DrawImage(input, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            //        //    }
            //        ////AddBitmapImageToImage(selectedPortion);
            //        ////Bitmap temp = (Bitmap)input.Clone();
                        
            //        //while (kernelSize > 0 && distanceToBlur > pixelStep)
            //        //{
            //        //    Rectangle rect1 = new Rectangle(selectionX - distanceToBlur, selectionY - distanceToBlur, selectionWidth + distanceToBlur, selectionHeight + distanceToBlur);
            //        //    Bitmap selectedPortion1 = new Bitmap(rect.Width, rect.Height);
            //        //    using (Graphics g = Graphics.FromImage(selectedPortion1))
            //        //    {
            //        //        g.DrawImage(temp, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            //        //    }
            //        //    using (Graphics g = Graphics.FromImage(input))
            //        //    {
            //        //        g.DrawImage(selectedPortion, rect);
            //        //    }
            //        //}
            //        //}
            //        //input = gaussianBlurFilter.Apply(input);
            //        return input;
            //    }
            //    else {
            //        Rectangle rect = new Rectangle(selectionX, selectionY, selectionWidth, selectionHeight);
            //        Bitmap selectedPortion = new Bitmap(rect.Width, rect.Height);
            //        using (Graphics g = Graphics.FromImage(selectedPortion))
            //        {
            //            g.DrawImage(input, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            //        }
            //        //AddBitmapImageToImage(selectedPortion);

            //        selectedPortion = gaussianBlurFilter.Apply(selectedPortion);
            //        using (Graphics g = Graphics.FromImage(input))
            //        {
            //            g.DrawImage(selectedPortion, rect);
            //        }
            //        return input;
            //    }
                
            //}
            
        }
        public static Bitmap ApplySepiaFilter(Bitmap input)
        {
            Sepia sepiaFilter = new Sepia();
            return sepiaFilter.Apply(input);
        }
        
    }
}
