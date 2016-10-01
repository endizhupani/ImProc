using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProccessing_wpf.ImageHelpers
{
    public static class ShapeDetector
    {
        
        public static Bitmap DetectFacesAndDrawBorders(Bitmap input)
        {
            Rectangle[] faces = DetectFaces(input);
            Image<Bgr, byte> cvImage = new Image<Bgr, byte>(input);
            foreach (var face in faces)
            {
                cvImage.Draw(face, new Bgr(Color.Blue), 1);

            }
            return cvImage.Bitmap;
        }
        public static Rectangle[] DetectFaces(Bitmap input)
        {
            Image<Bgr, byte> cvImage = new Image<Bgr, byte>(input);
            CascadeClassifier cascadeClassifier = new CascadeClassifier(Application.StartupPath + "/Resources/HaarFiles/haarcascade_frontalface_alt.xml");
            using(Image<Gray, Byte> Gray = cvImage.Convert<Gray, Byte>()){
                Rectangle[] faces = cascadeClassifier.DetectMultiScale(Gray, 1.1, 10);
                return faces;
            }
        }
        //public static Bitmap FindFace(Bitmap input, Bitmap faceToFind)
        //{
        //    Rectangle[] faces = DetectFaces(input);
        //    Rectangle rectFaceToFind = DetectFaces(faceToFind)[0];
        //    Image<Bgr, Byte> inputCvImage = new Image<Bgr, byte>(input);
        //    Image<Bgr, byte> faceToFindCvImage = new Image<Bgr, byte>(faceToFind);
        //    Image<Gray, Byte> faceCvImage = new Image<Gray, byte>(faceToFindCvImage.GetSubRect(rectFaceToFind).ToBitmap());
        //    FaceRecognizer recognizer = new EigenFaceRecognizer();
        //    var faceImages = new Image<Gray, byte>[1];
        //    var labels = new int[1];
        //    faceImages[0] = faceCvImage.Resize(100,100,Emgu.CV.CvEnum.Inter.Cubic);
        //    labels[0] = 1;
        //    recognizer.Train(faceImages, labels);
        //    foreach (Rectangle rect in faces)
        //    {
        //        Image<Gray, Byte> imgToTest = new Image<Gray, byte>(inputCvImage.GetSubRect(rect).ToBitmap());
        //        var result = recognizer.Predict(imgToTest.Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic));
        //        if (result.Label == 1)
        //        {
        //            inputCvImage.Draw(rect, new Bgr(Color.BurlyWood), 3);
        //        }
        //    }
        //    return inputCvImage.Bitmap;

        //}
    }
}
