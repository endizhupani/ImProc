using AForge.Imaging;
using AForge.Imaging.Filters;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProccessing_wpf.ImageHelpers
{
    public static class ImageEffects
    {
        public static Bitmap Emboss(Bitmap input)
        {
            //emboss efect kernel
            int[,] kernel = {
            { -2, -1,  0 },
            { -1,  1,  1 },
            {  0,  1,  2 } };
            Convolution convultionFilters = new Convolution(kernel);
            return convultionFilters.Apply(input);
        }
        public static Bitmap Sketch(Bitmap input, int redThreshold, int blueThreshold, int greenThreshold)
        {
            int[,] kernel = {
                { 1, 2, 1, },  
               { 2, 4, 2, },   
               { 1, 2, 1, },
            };
            Convolution convultionFilters = new Convolution(kernel);
            Threshold thresholdFilter = new Threshold(redThreshold);
            input = convultionFilters.Apply(input);
            // extract red channel
            ExtractChannel extractFilter = new ExtractChannel(RGB.R);
            Bitmap channel = extractFilter.Apply(input);
            // threshold channel
            thresholdFilter.ApplyInPlace(channel);
            // put the channel back
            ReplaceChannel replaceFilter = new ReplaceChannel(RGB.R, channel);
            replaceFilter.ApplyInPlace(input);

            // extract blue channel
            extractFilter = new ExtractChannel(RGB.B);
            channel = extractFilter.Apply(input);
            // threshold channel
            thresholdFilter.ThresholdValue = blueThreshold;
            thresholdFilter.ApplyInPlace(channel);
            // put the channel back
            replaceFilter = new ReplaceChannel(RGB.B, channel);
            replaceFilter.ApplyInPlace(input);
            
            // extract green channel
            extractFilter = new ExtractChannel(RGB.G);
            channel = extractFilter.Apply(input);
            // threshold channel
            thresholdFilter.ThresholdValue = greenThreshold;
            thresholdFilter.ApplyInPlace(channel);
            // put the channel back
            replaceFilter = new ReplaceChannel(RGB.G, channel);
            replaceFilter.ApplyInPlace(input);

            return input;
        }
        public static Bitmap Invert(Bitmap input)
        {
            Invert inverter = new Invert();
            return inverter.Apply(input);
        }
        //public static Bitmap BlackWhite(Bitmap input)
        //{
        //    byte[] photobytes = MemoryStream.
        //    using (MemoryStream inStream = new MemoryStream())
        //    {
        //        using (MemoryStream outStream = new MemoryStream())
        //        {
        //            // Initialize the ImageFactory using the overload to preserve EXIF metadata.
        //            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
        //            {
        //                // Load, resize, set the format and quality and save an image.
        //                imageFactory.Load(inStream)
        //                            .Resize(size)
        //                            .Format(format)
        //                            .Save(outStream);
        //            }
        //            // Do something with the stream.
        //        }
        //    }
        //    ImageFactory imgFactory = new ImageFactory(preserveExifData: true);
        //    imgFactory.Load()
        //    Invert inverter = new Invert();
        //    return inverter.Apply(input);
        //}
    }
}
