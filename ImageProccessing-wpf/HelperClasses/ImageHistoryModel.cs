using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProccessing_wpf.HelperClasses
{
    public class ImageHistoryModel
    {
        public Bitmap Image { get; set; }
        public Manipulation? Manipulation { get; set; }
    }
    public enum Manipulation {
        NoManipulation = 0,
        Rotate = 1,
        Sketch = 2,
        ShowEdges = 3,
        HistogramEqualization = 4,
        AddedNoise = 5,
        ClearedNoise = 6,
        Sharpen = 7,
        FourierTransform = 8,
        Grayscale = 9,
        FaceBordersDrawn = 10,
        RGBColorChange = 11,
        HSLColorChange = 12,
        BrightnessContrastAdjusted = 13,
        Smooth = 14,
        Sepia = 15,
        Inverted = 16,
        Zoom = 17,
        BlackWhite = 18,
        Comic = 19,
        Gotham = 20,
        HiSatch = 21,
        Lomograph = 22,
        LoSatch = 23,
        Polaroid = 24,

    }
}
