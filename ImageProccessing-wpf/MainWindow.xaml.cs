using AForge.Imaging;
using ImageProccessing_wpf.HelperClasses;
using ImageProccessing_wpf.ImageHelpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace ImageProccessing_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Private members
        private System.Drawing.Bitmap currentImage;
        private System.Drawing.Bitmap originalImage;
        private System.Drawing.Bitmap temp;
        private List<ImageHistoryModel> imageHistory;
        private List<ImageHistoryModel> imagesUndone;
        private bool isGrayScale;
        private string lastLoadedOrSavedFileName;
        private double zoomRatio;
        #endregion

        #region GUI Controls
        //private System.Windows.Controls.Image imageContainer;
        private Slider redThresholdSlider;
        private Slider blueThresholdSlider;
        private Slider greenThresholdSlider;

        private TextBox redThreshold;
        private TextBox blueThreshold;
        private TextBox greenThreshold;


        private Slider rotateAngleSlider;
        private TextBox rotateAngleTextBox;
        private Button equalizeHistogramBtn;
        private RadioButton weightedAvgRBtn;
        private RadioButton standardAvgRBtn;
        //
        //Sharpen
        //
        private RadioButton standardSharpenRadioBtn;
        private RadioButton gaussianSharpenRadioBtn;
        private TextBlock sharpenErrorText;
        private Label lblSharpenKernel;
        private TextBox txtSharpenKernelSize;
        private Label lblSharpenSigma;
        private TextBox txtSharpenSigma;
        private Button sharpenApplyBtn;
        #endregion

        #region Selection variables
        private bool isSelecting = false;
        private bool isSelected = false;
        //private System.Drawing.Rectangle selection;
        private int x0, y0, x1, y1;
        #endregion


        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Histogram Properties
        private PointCollection luminanceHistogramPoints = null;
        private PointCollection redColorHistogramPoints = null;
        private PointCollection greenColorHistogramPoints = null;
        private PointCollection blueColorHistogramPoints = null;
        private PointCollection grayColorHistogramPoints = null;
        public PointCollection LuminanceHistogramPoints
        {
            get
            {
                return this.luminanceHistogramPoints;
            }
            set
            {
                if (this.luminanceHistogramPoints != value)
                {
                    this.luminanceHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("LuminanceHistogramPoints"));
                    }
                }
            }
        }
        public PointCollection RedColorHistogramPoints
        {
            get
            {
                return this.redColorHistogramPoints;
            }
            set
            {
                if (this.redColorHistogramPoints != value)
                {
                    this.redColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("RedColorHistogramPoints"));
                    }
                }
            }
        }
        public PointCollection GreenColorHistogramPoints
        {
            get
            {
                return this.greenColorHistogramPoints;
            }
            set
            {
                if (this.greenColorHistogramPoints != value)
                {
                    this.greenColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("GreenColorHistogramPoints"));
                    }
                }
            }
        }
        public PointCollection BlueColorHistogramPoints
        {
            get
            {
                return this.blueColorHistogramPoints;
            }
            set
            {
                if (this.blueColorHistogramPoints != value)
                {
                    this.blueColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("BlueColorHistogramPoints"));
                    }
                }
            }
        }
        public PointCollection GrayColorHistogramPoints
        {
            get
            {
                return this.grayColorHistogramPoints;
            }
            set
            {
                if (this.grayColorHistogramPoints != value)
                {
                    this.grayColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("GrayColorHistogramPoints"));
                    }
                }
            }
        }
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            redValue.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(redValue_TextInput),
                   true);
            blueValue.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(blueValue_TextInput),
                   true);
            greenValue.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(greenValue_TextInput),
                   true);
            hueValue.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(hueValue_TextInput),
                   true);
            saturationValue.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(satValue_TextInput),
                   true);
            luminanceValue.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(lumValue_TextInput),
                   true);
            brightnessTxt.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(brightnessTxt_TextInput),
                   true);
            contrastTxt.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(contrastTxt_TextInput),
                   true);
            widthTxt.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(widthTxt_TextInput),
                   true);
            heightTxt.AddHandler(TextBox.TextInputEvent,
                   new TextCompositionEventHandler(heightTxt_TextInput),
                   true);
        }

        private void openMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.OpenFile();
        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.SaveFile(A.Save);
        }
        private void saveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.SaveFile(A.SaveAs);
        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #region Adjust menu methods and event handlers
        private void edgesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.currentImage = GeneralImageMethods.DetectEdges(currentImage);
            imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.ShowEdges });
            this.AddBitmapImageToImage();
        }

        private void sharpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            this.imageOptionsGrid.Children.RemoveRange(3, this.imageOptionsGrid.Children.Count - 3);
            brightnessContrastControls.Visibility = Visibility.Hidden;
            this.colorAdjustement.Visibility = Visibility.Hidden;
            this.standardSharpenRadioBtn = new RadioButton();
            this.standardSharpenRadioBtn.IsChecked = true;
            this.standardSharpenRadioBtn.VerticalAlignment = VerticalAlignment.Top;
            this.standardSharpenRadioBtn.HorizontalAlignment = HorizontalAlignment.Left;
            this.standardSharpenRadioBtn.Height = 20;
            this.standardSharpenRadioBtn.Width = 150;
            this.standardSharpenRadioBtn.Content = "Standard sharpening";
            this.standardSharpenRadioBtn.Margin = new Thickness(10, this.labelImgTools.Margin.Top + this.labelImgTools.Height + 10, 0, 0);
            this.standardSharpenRadioBtn.Checked += standardSharpenRadioBtn_Checked;
            this.imageOptionsGrid.Children.Add(this.standardSharpenRadioBtn);

            this.gaussianSharpenRadioBtn = new RadioButton();
            this.gaussianSharpenRadioBtn.VerticalAlignment = VerticalAlignment.Top;
            this.gaussianSharpenRadioBtn.HorizontalAlignment = HorizontalAlignment.Left;
            this.gaussianSharpenRadioBtn.Height = 20;
            this.gaussianSharpenRadioBtn.Width = 150;
            this.gaussianSharpenRadioBtn.Content = "Gaussian sharpening";
            this.gaussianSharpenRadioBtn.Margin = new Thickness(this.standardSharpenRadioBtn.Margin.Left + this.standardSharpenRadioBtn.Width + 10, this.labelImgTools.Margin.Top + this.labelImgTools.Height + 10, 0, 0);
            this.gaussianSharpenRadioBtn.Checked += gaussianSharpenRadioBtn_Checked;
            this.imageOptionsGrid.Children.Add(this.gaussianSharpenRadioBtn);

            this.lblSharpenKernel = new Label();
            lblSharpenKernel.Height = 30;
            lblSharpenKernel.Width = 80;
            lblSharpenKernel.Content = "Kernel size: ";
            lblSharpenKernel.HorizontalAlignment = HorizontalAlignment.Left;
            lblSharpenKernel.VerticalAlignment = VerticalAlignment.Top;
            lblSharpenKernel.Margin = new Thickness(10, this.standardSharpenRadioBtn.Margin.Top + this.standardSharpenRadioBtn.Height + 10, 0, 0);
            //this.imageOptionsGrid.Children.Add(this.lblSharpenKernel);

            this.txtSharpenKernelSize = new TextBox();
            this.txtSharpenKernelSize.Width = 40;
            txtSharpenKernelSize.HorizontalAlignment = HorizontalAlignment.Left;
            txtSharpenKernelSize.VerticalAlignment = VerticalAlignment.Top;
            this.txtSharpenKernelSize.Margin = new Thickness(this.lblSharpenKernel.Margin.Left + this.lblSharpenKernel.Width + 5, this.lblSharpenKernel.Margin.Top, 0, 0);
            this.txtSharpenKernelSize.TextChanged += txtGaussianKernelInfo_TextChanged;
            //this.imageOptionsGrid.Children.Add(this.txtSharpenKernelSize);

            this.lblSharpenSigma = new Label();
            lblSharpenSigma.Height = 30;
            lblSharpenSigma.Width = 80;
            lblSharpenSigma.HorizontalAlignment = HorizontalAlignment.Left;
            lblSharpenSigma.VerticalAlignment = VerticalAlignment.Top;
            lblSharpenSigma.Content = "Sigma: ";
            lblSharpenSigma.Margin = new Thickness(this.txtSharpenKernelSize.Margin.Left + this.txtSharpenKernelSize.Width + 10, this.lblSharpenKernel.Margin.Top, 0, 0);
            //this.imageOptionsGrid.Children.Add(this.lblSharpenSigma);

            this.txtSharpenSigma = new TextBox();
            this.txtSharpenSigma.Width = 40;
            txtSharpenSigma.HorizontalAlignment = HorizontalAlignment.Left;
            txtSharpenSigma.VerticalAlignment = VerticalAlignment.Top;
            this.txtSharpenSigma.Margin = new Thickness(this.lblSharpenSigma.Margin.Left + this.lblSharpenSigma.Width, this.lblSharpenKernel.Margin.Top, 0, 0);
            this.txtSharpenSigma.TextChanged += txtGaussianKernelInfo_TextChanged;
            //this.imageOptionsGrid.Children.Add(this.txtSharpenSigma);

            this.sharpenApplyBtn = new Button();
            sharpenApplyBtn.Content = "Apply";
            sharpenApplyBtn.Height = 20;
            this.sharpenApplyBtn.Width = 60;
            sharpenApplyBtn.Margin = new Thickness(10, this.lblSharpenSigma.Margin.Top + this.lblSharpenSigma.Height + 10, 0, 0);
            sharpenApplyBtn.HorizontalAlignment = HorizontalAlignment.Left;
            sharpenApplyBtn.VerticalAlignment = VerticalAlignment.Top;
            sharpenApplyBtn.Click += sharpenBtn_Click;
            this.imageOptionsGrid.Children.Add(sharpenApplyBtn);
                        
            this.sharpenErrorText = new TextBlock();
            sharpenErrorText.Text = "";
            sharpenErrorText.Foreground = Brushes.Red;
            this.sharpenErrorText.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.sharpenErrorText.TextWrapping = TextWrapping.Wrap;
            sharpenErrorText.Margin = new Thickness(this.sharpenApplyBtn.Margin.Left + this.sharpenApplyBtn.Width + 5, this.sharpenApplyBtn.Margin.Top, 0, 0);
            sharpenErrorText.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        private void txtGaussianKernelInfo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.imageOptionsGrid.Children.Remove(sharpenErrorText);
            }
            catch
            {

            }
        }
        private void sharpenBtn_Click(object sender, EventArgs e)
        {
            if (gaussianSharpenRadioBtn.IsChecked.Value && (String.IsNullOrEmpty(txtSharpenSigma.Text) || String.IsNullOrEmpty(txtSharpenKernelSize.Text)))
            {
                sharpenErrorText.Text = "Please enter the kernel size and sigma value";
                this.imageOptionsGrid.Children.Add(sharpenErrorText);
                return;
            }
            if (gaussianSharpenRadioBtn.IsChecked.Value)
            {
                double sigma = Convert.ToDouble(txtSharpenSigma.Text);
                int kSize = Convert.ToInt32(txtSharpenKernelSize.Text);
                currentImage = ImageFilters.ApplyGaussianSharpenFilter(currentImage, kSize, sigma);
            }
            else {
                currentImage = ImageFilters.ApplySharpenFilter(currentImage);
            }
            this.imagesUndone = new List<ImageHistoryModel>();
            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Sharpen });
            AddBitmapImageToImage();

        }
        private void standardSharpenRadioBtn_Checked(object sender, EventArgs e)
        {
            try
            {
                this.imageOptionsGrid.Children.Remove(lblSharpenKernel);
                this.imageOptionsGrid.Children.Remove(lblSharpenSigma);
                this.imageOptionsGrid.Children.Remove(txtSharpenKernelSize);
                this.imageOptionsGrid.Children.Remove(txtSharpenSigma);
                this.imageOptionsGrid.Children.Remove(sharpenErrorText);
            }
            catch {
                return;
            }
        }
        private void gaussianSharpenRadioBtn_Checked(object sender, EventArgs e)
        {
            try
            {
                this.imageOptionsGrid.Children.Add(lblSharpenKernel);
                this.imageOptionsGrid.Children.Add(lblSharpenSigma);
                this.imageOptionsGrid.Children.Add(txtSharpenKernelSize);
                this.imageOptionsGrid.Children.Add(txtSharpenSigma);
                this.imageOptionsGrid.Children.Remove(sharpenErrorText);
            }
            catch
            {
                return;
            }
        }

        #endregion
        private void grayscaleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();
            this.redoToolbarBtn.IsEnabled = false;
            this.undoToolbarBtn.IsEnabled = true;

            currentImage = GeneralImageMethods.ToGrayscale(currentImage);
            isGrayScale = true;
            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage });
            this.AddBitmapImageToImage();
        }
        private void undoToolbarBtn_Click(object sender, RoutedEventArgs e)
        {
            if (imageHistory != null && imageHistory.Count > 0)
            {
                this.imagesUndone.Add(imageHistory.Last());
                this.redoToolbarBtn.IsEnabled = true;

                if (imageHistory.Count > 1)
                {
                    currentImage = imageHistory[imageHistory.Count - 2].Image;
                    imageHistory.Remove(imageHistory.Last());
                }
                else {
                    currentImage = imageHistory.Last().Image;
                }
                isGrayScale = GeneralImageMethods.IsGrayscale(currentImage);
                this.AddBitmapImageToImage();
                if (imageHistory.Count == 0)
                {
                    this.undoToolbarBtn.IsEnabled = false;
                }
            }

        }
        private void redoToolbarBtn_Click(object sender, RoutedEventArgs e)
        {
            if (imagesUndone != null && imagesUndone.Count > 1)
            {
                this.imageHistory.Add(imagesUndone.Last());
                this.undoToolbarBtn.IsEnabled = true;
                currentImage = imagesUndone.Last().Image;
                isGrayScale = GeneralImageMethods.IsGrayscale(currentImage);
                imagesUndone.Remove(imagesUndone.Last());
                this.AddBitmapImageToImage();
                if (imagesUndone.Count == 0)
                {
                    this.redoToolbarBtn.IsEnabled = false;
                }
            }
        }
        public void zoomInMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();
            
            zoomRatio += 0.1;
            currentImage = GeneralImageMethods.Zoom(imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Zoom).Image, zoomRatio);
            imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Zoom });
            this.AddBitmapImageToImage();
            this.zoomRatioTxt.Text = zoomRatio.ToString();
            this.redoToolbarBtn.IsEnabled = false;
        }
        private void zoomOutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();

            zoomRatio -= 0.1;
            currentImage = GeneralImageMethods.Zoom(imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Zoom).Image, zoomRatio);
            imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Zoom });
            this.AddBitmapImageToImage();
            this.redoToolbarBtn.IsEnabled = false;
        }
        private void emboosMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();
            this.redoToolbarBtn.IsEnabled = false;
            this.undoToolbarBtn.IsEnabled = true;
            
            currentImage = ImageEffects.Emboss(currentImage);
            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage });
            this.AddBitmapImageToImage();
        }

        #region Sketch effect methods and event handlers
        private void sketchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imageOptionsGrid.Children.RemoveRange(3, this.imageOptionsGrid.Children.Count - 3);
            brightnessContrastControls.Visibility = Visibility.Hidden;
            this.colorAdjustement.Visibility = Visibility.Hidden;
            //
            //red threshold
            //
            redThresholdSlider = new Slider();
            TextBlock redThresholdSliderLabel = new TextBlock();
            redThresholdSliderLabel.Text = "Red threshold value";
            redThresholdSliderLabel.FontSize = 10;
            redThresholdSliderLabel.TextWrapping = TextWrapping.Wrap;
            redThresholdSliderLabel.VerticalAlignment = VerticalAlignment.Top;
            redThresholdSliderLabel.HorizontalAlignment = HorizontalAlignment.Left;
            redThresholdSliderLabel.Width = 70;
            redThresholdSliderLabel.Height = 29;
            redThresholdSliderLabel.Margin = new Thickness(15, this.labelImgTools.Margin.Top + this.labelImgTools.Height + 15, 0, 0);
            this.imageOptionsGrid.Children.Add(redThresholdSliderLabel);
            redThresholdSlider.Maximum = 255;
            redThresholdSlider.HorizontalAlignment = HorizontalAlignment.Left;
            redThresholdSlider.VerticalAlignment = VerticalAlignment.Top;
            redThresholdSlider.Height = 29;
            redThresholdSlider.Width = 150;
            redThresholdSlider.Margin = new Thickness(redThresholdSliderLabel.Margin.Left + redThresholdSliderLabel.Width + 10,
                                                      this.labelImgTools.Margin.Top + this.labelImgTools.Height + 15,
                                                      0,
                                                      0);
            redThresholdSlider.Value = 80;
            redThresholdSlider.ValueChanged += trshldSlider_ValueChanged;
            redThresholdSlider.Name = "redTrshldSlider";
            this.imageOptionsGrid.Children.Add(redThresholdSlider);
            redThreshold = new TextBox();
            redThreshold.Text = "80";
            redThreshold.Name = "redTrshld";
            redThreshold.VerticalAlignment = VerticalAlignment.Top;
            redThreshold.HorizontalAlignment = HorizontalAlignment.Left;
            redThreshold.TextChanged += trshldTextBox_ValueChanged;
            redThreshold.Margin = new Thickness(redThresholdSlider.Margin.Left + redThresholdSlider.Width + 10,
                                                      this.labelImgTools.Margin.Top + this.labelImgTools.Height + 15,
                                                      0,
                                                      0);
            this.imageOptionsGrid.Children.Add(redThreshold);

            //
            //blue threshold
            //
            TextBlock blueThresholdSliderLabel = new TextBlock();
            blueThresholdSliderLabel.Text = "Blue threshold value";
            blueThresholdSliderLabel.FontSize = 10;
            blueThresholdSliderLabel.TextWrapping = TextWrapping.Wrap;
            blueThresholdSliderLabel.VerticalAlignment = VerticalAlignment.Top;
            blueThresholdSliderLabel.HorizontalAlignment = HorizontalAlignment.Left;
            blueThresholdSliderLabel.Width = 70;
            blueThresholdSliderLabel.Height = 29;
            blueThresholdSliderLabel.Margin = new Thickness(15, redThresholdSliderLabel.Margin.Top + redThresholdSliderLabel.Height + 15, 0, 0);
            this.imageOptionsGrid.Children.Add(blueThresholdSliderLabel);

            blueThresholdSlider = new Slider();
            blueThresholdSlider.Maximum = 255;
            blueThresholdSlider.HorizontalAlignment = HorizontalAlignment.Left;
            blueThresholdSlider.VerticalAlignment = VerticalAlignment.Top;
            blueThresholdSlider.Height = 29;
            blueThresholdSlider.Width = 150;
            blueThresholdSlider.Margin = new Thickness(blueThresholdSliderLabel.Margin.Left + blueThresholdSliderLabel.Width + 10,
                                                       redThresholdSlider.Margin.Top + redThresholdSlider.Height + 15,
                                                       36,
                                                       0);
            blueThresholdSlider.Value = 80;
            blueThresholdSlider.ValueChanged += trshldSlider_ValueChanged;
            blueThresholdSlider.Name = "blueTrshldSlider";
            this.imageOptionsGrid.Children.Add(blueThresholdSlider);
            blueThreshold = new TextBox();
            blueThreshold.Text = "80";
            blueThreshold.Name = "blueTrshld";
            blueThreshold.VerticalAlignment = VerticalAlignment.Top;
            blueThreshold.HorizontalAlignment = HorizontalAlignment.Left;
            blueThreshold.TextChanged += trshldTextBox_ValueChanged;
            blueThreshold.Margin = new Thickness(blueThresholdSlider.Margin.Left + blueThresholdSlider.Width + 10,
                                                      redThresholdSlider.Margin.Top + redThresholdSlider.Height + 15,
                                                      0,
                                                      0);
            this.imageOptionsGrid.Children.Add(blueThreshold);

            //
            //green threshold
            //
            TextBlock greenThresholdSliderLabel = new TextBlock();
            greenThresholdSliderLabel.Text = "Green threshold value";
            greenThresholdSliderLabel.FontSize = 10;
            greenThresholdSliderLabel.TextWrapping = TextWrapping.Wrap;
            greenThresholdSliderLabel.VerticalAlignment = VerticalAlignment.Top;
            greenThresholdSliderLabel.HorizontalAlignment = HorizontalAlignment.Left;
            greenThresholdSliderLabel.Width = 70;
            greenThresholdSliderLabel.Height = 29;
            greenThresholdSliderLabel.Margin = new Thickness(15, blueThresholdSliderLabel.Margin.Top + blueThresholdSliderLabel.Height + 15, 0, 0);
            this.imageOptionsGrid.Children.Add(greenThresholdSliderLabel);

            greenThresholdSlider = new Slider();
            greenThresholdSlider.Maximum = 255;
            greenThresholdSlider.HorizontalAlignment = HorizontalAlignment.Left;
            greenThresholdSlider.VerticalAlignment = VerticalAlignment.Top;
            greenThresholdSlider.Height = 29;
            greenThresholdSlider.Width = 150;
            greenThresholdSlider.Margin = new Thickness(greenThresholdSliderLabel.Margin.Left + greenThresholdSliderLabel.Width + 10,
                                                       blueThresholdSlider.Margin.Top + blueThresholdSlider.Height + 15,
                                                       36,
                                                       0);
            greenThresholdSlider.Value = 80;
            greenThresholdSlider.ValueChanged += trshldSlider_ValueChanged;
            greenThresholdSlider.Name = "greenTrshldSlider";
            this.imageOptionsGrid.Children.Add(greenThresholdSlider);
            greenThreshold = new TextBox();
            greenThreshold.Text = "80";
            greenThreshold.Name = "greenTrshld";
            greenThreshold.VerticalAlignment = VerticalAlignment.Top;
            greenThreshold.HorizontalAlignment = HorizontalAlignment.Left;
            greenThreshold.TextChanged += trshldTextBox_ValueChanged;
            greenThreshold.Margin = new Thickness(greenThresholdSlider.Margin.Left + greenThresholdSlider.Width + 10,
                                                      blueThresholdSlider.Margin.Top + blueThresholdSlider.Height + 15,
                                                      0,
                                                      0);
            this.imageOptionsGrid.Children.Add(greenThreshold);


            Button applyThresholdButton = new Button();
            applyThresholdButton.Content = "Apply";
            applyThresholdButton.FontSize = 10;

            applyThresholdButton.VerticalAlignment = VerticalAlignment.Top;
            applyThresholdButton.HorizontalAlignment = HorizontalAlignment.Left;
            applyThresholdButton.Width = 70;
            applyThresholdButton.Margin = new Thickness(15,
                                                        greenThresholdSliderLabel.Margin.Top + greenThresholdSliderLabel.Height + 15,
                                                        0,
                                                        0);
            applyThresholdButton.Click += applyTrshldButton_Click;
            this.imageOptionsGrid.Children.Add(applyThresholdButton);
        }
        private void trshldSlider_ValueChanged(object sender, EventArgs e)
        {
            Slider sliderChanged = sender as Slider;
            switch (sliderChanged.Name)
            {
                case "greenTrshldSlider":
                    this.greenThreshold.Text = ((int)greenThresholdSlider.Value).ToString();
                    break;
                case "redTrshldSlider":
                    this.redThreshold.Text = ((int)redThresholdSlider.Value).ToString();
                    break;
                case "blueTrshldSlider":
                    this.blueThreshold.Text = ((int)blueThresholdSlider.Value).ToString();
                    break;
                default:
                    break;
            };
        }
        private void trshldTextBox_ValueChanged(object sender, EventArgs e)
        {
            TextBox txtChanged = sender as TextBox;
            switch (txtChanged.Name)
            {
                case "greenTrshld":
                    this.greenThresholdSlider.Value = Convert.ToInt32(this.greenThreshold.Text);
                    break;
                case "redTrshld":
                    this.redThresholdSlider.Value = Convert.ToInt32(this.redThreshold.Text);
                    break;
                case "blueTrshld":
                    this.blueThresholdSlider.Value = Convert.ToInt32(this.blueThreshold.Text);
                    break;
                default:
                    break;
            };
        }
        private void applyTrshldButton_Click(object sender, EventArgs e)
        {

            this.imagesUndone = new List<ImageHistoryModel>();
            this.redoToolbarBtn.IsEnabled = false;
            this.undoToolbarBtn.IsEnabled = true;

            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Sketch });
            currentImage = imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Sketch).Image.Clone(new System.Drawing.Rectangle(0, 0, imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Sketch).Image.Width, imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Sketch).Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            currentImage = ImageEffects.Sketch(currentImage,
                (int)Math.Floor(this.redThresholdSlider.Value),
                (int)Math.Floor(this.blueThresholdSlider.Value),
                (int)Math.Floor(this.greenThresholdSlider.Value));
            this.AddBitmapImageToImage();
        }
        #endregion

        //TODO: fix for textboxchanged event
        #region Rotate methods and event handlers
        private void rotateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imageOptionsGrid.Children.RemoveRange(3, this.imageOptionsGrid.Children.Count - 3);
            brightnessContrastControls.Visibility = Visibility.Hidden;
            this.colorAdjustement.Visibility = Visibility.Hidden;

            TextBlock rotateAngleLabel = new TextBlock();
            rotateAngleLabel.Text = "Rotate angle [-360; 360]";
            rotateAngleLabel.FontSize = 12;
            rotateAngleLabel.TextWrapping = TextWrapping.Wrap;
            rotateAngleLabel.VerticalAlignment = VerticalAlignment.Top;
            rotateAngleLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
            rotateAngleLabel.Height = 29;
            rotateAngleLabel.Margin = new Thickness(15, this.labelImgTools.Margin.Top + this.labelImgTools.Height + 15, 0, 0);
            this.imageOptionsGrid.Children.Add(rotateAngleLabel);
            rotateAngleSlider = new Slider();
            rotateAngleSlider.Maximum = 360;
            rotateAngleSlider.Minimum = -360;
            rotateAngleSlider.HorizontalAlignment = HorizontalAlignment.Left;
            rotateAngleSlider.VerticalAlignment = VerticalAlignment.Top;
            rotateAngleSlider.Height = 29;
            rotateAngleSlider.Width = 170;
            rotateAngleSlider.Margin = new Thickness(15,
                                                      rotateAngleLabel.Margin.Top + rotateAngleLabel.Height + 5,
                                                      15,
                                                      0);
            rotateAngleSlider.Value = 0;
            rotateAngleSlider.PreviewMouseLeftButtonUp += rotateSlider_LeftMouseButtonUp;
            rotateAngleSlider.ValueChanged += rotateAngleSlider_ValueChanged;
            this.imageOptionsGrid.Children.Add(rotateAngleSlider);
            rotateAngleTextBox = new TextBox();
            rotateAngleTextBox.Text = "0";
            rotateAngleTextBox.VerticalAlignment = VerticalAlignment.Top;
            rotateAngleTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            rotateAngleTextBox.PreviewMouseDown += rotateAngleTextBox_MouseDown;
            rotateAngleTextBox.LostFocus += rotateAngleTextBox_LostFocus;
            rotateAngleTextBox.Margin = new Thickness(rotateAngleSlider.Margin.Left + rotateAngleSlider.Width + 10,
                                                      rotateAngleLabel.Margin.Top + this.labelImgTools.Height + 5,
                                                      0,
                                                      0);
            this.imageOptionsGrid.Children.Add(rotateAngleTextBox);
        }
        private void rotateAngleTextBox_LostFocus(object sender, EventArgs e)
        {
            this.rotateAngleTextBox.TextChanged -= rotateAngleTextBox_ValueChanged;
        }
        private void rotateAngleTextBox_MouseDown(object sender, EventArgs e)
        {
            this.rotateAngleTextBox.TextChanged += rotateAngleTextBox_ValueChanged;
        }
        private void rotateAngleSlider_ValueChanged(object sender, EventArgs e)
        {
            Slider slider = sender as Slider;
            this.rotateAngleTextBox.Text = Math.Round(slider.Value, 2).ToString();
        }
        private void rotateSlider_LeftMouseButtonUp(object sender, EventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();
            Slider slider = sender as Slider;

            if (this.imageHistory == null || this.imageHistory.Count == 0 || this.imageHistory.Last().Manipulation == null || this.imageHistory.Last().Manipulation != HelperClasses.Manipulation.Rotate)
            {
                this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Rotate });
            }
            else
            {
                currentImage = imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Rotate).Image
                    .Clone(new System.Drawing.Rectangle(0,
                                                        0,
                                                        imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Rotate).Image.Width,
                                                        imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Rotate).Image.Height
                                                        ),
                           System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }
            currentImage = GeneralImageMethods.Rotate(currentImage, slider.Value);
            this.AddBitmapImageToImage();
        }
        private void rotateAngleTextBox_ValueChanged(object sender, EventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();

            TextBox txtBox = sender as TextBox;
            double value = 0;
            if (!String.IsNullOrEmpty(txtBox.Text))
            {
                value = Convert.ToDouble(txtBox.Text);
            }
            this.rotateAngleSlider.Value = value;
            if (this.imageHistory.Last().Manipulation == null || this.imageHistory.Last().Manipulation != HelperClasses.Manipulation.Rotate)
            {
                this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Rotate });
            }
            else
            {
                currentImage = imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Rotate).Image
                    .Clone(new System.Drawing.Rectangle(0,
                                                        0,
                                                        imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Rotate).Image.Width,
                                                        imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Rotate).Image.Height
                                                        ),
                           System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }
            currentImage = GeneralImageMethods.Rotate(currentImage, this.rotateAngleSlider.Value);
            this.AddBitmapImageToImage();

        }
        #endregion

        #region Image Noise methods and menu items
        private void clearNoiseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imageOptionsGrid.Children.RemoveRange(3, this.imageOptionsGrid.Children.Count - 3);
            brightnessContrastControls.Visibility = Visibility.Hidden;
            this.colorAdjustement.Visibility = Visibility.Hidden;
            TextBlock avgNoiseReductLabel = new TextBlock();
            avgNoiseReductLabel.HorizontalAlignment = HorizontalAlignment.Left;
            avgNoiseReductLabel.VerticalAlignment = VerticalAlignment.Top;
            avgNoiseReductLabel.Margin = new Thickness(10, this.labelImgTools.Margin.Top + this.labelImgTools.Height + 10, 0, 0);
            avgNoiseReductLabel.Width = 130;
            //avgNoiseReductLabel.Height = 25;
            avgNoiseReductLabel.Text = "Averaging noise reduction";
            avgNoiseReductLabel.ToolTip = "Performs noise reduction by giving each pixel the average intensity of a 3x3 neighbourhood";
            avgNoiseReductLabel.TextWrapping = TextWrapping.Wrap;
            
            this.imageOptionsGrid.Children.Add(avgNoiseReductLabel);
            this.weightedAvgRBtn = new RadioButton();
            weightedAvgRBtn.IsChecked = true;
            weightedAvgRBtn.Content = "Weighted average";
            weightedAvgRBtn.ToolTip = "Uses a higher weights for pixels closest to the center";
            weightedAvgRBtn.Margin = new Thickness(avgNoiseReductLabel.Margin.Left + avgNoiseReductLabel.Width + 5, this.labelImgTools.Margin.Top + this.labelImgTools.Height + 10, 0, 0);
            weightedAvgRBtn.HorizontalAlignment = HorizontalAlignment.Left;
            weightedAvgRBtn.VerticalAlignment = VerticalAlignment.Top;
            weightedAvgRBtn.Height = 15;
            weightedAvgRBtn.Width = 120;
            this.imageOptionsGrid.Children.Add(weightedAvgRBtn);
            this.standardAvgRBtn = new RadioButton();
            standardAvgRBtn.Content = "Standard average";
            
            standardAvgRBtn.Margin = new Thickness(avgNoiseReductLabel.Margin.Left + avgNoiseReductLabel.Width + 5, this.weightedAvgRBtn.Margin.Top + this.weightedAvgRBtn.Height + 3, 0, 0);
            standardAvgRBtn.HorizontalAlignment = HorizontalAlignment.Left;
            standardAvgRBtn.VerticalAlignment = VerticalAlignment.Top;
            standardAvgRBtn.Height = 15;
            standardAvgRBtn.Width = 120;
            this.imageOptionsGrid.Children.Add(standardAvgRBtn);

            TextBlock medNoiseReductLabel = new TextBlock();
            medNoiseReductLabel.HorizontalAlignment = HorizontalAlignment.Left;
            medNoiseReductLabel.VerticalAlignment = VerticalAlignment.Top;
            medNoiseReductLabel.Margin = new Thickness(10, this.standardAvgRBtn.Margin.Top + this.standardAvgRBtn.Height + 5, 0, 0);
            medNoiseReductLabel.Width = 130;
            
            medNoiseReductLabel.Text = "Noise reduction with median";
            medNoiseReductLabel.ToolTip = "Performs noise reduction by giving each pixel the median intensity of a 3x3 neighbourhood";
            medNoiseReductLabel.TextWrapping = TextWrapping.Wrap;
            this.imageOptionsGrid.Children.Add(medNoiseReductLabel);

            Button avgDeNoising = new Button();
            avgDeNoising.Content = "Apply";
            avgDeNoising.HorizontalAlignment = HorizontalAlignment.Stretch;
            avgDeNoising.VerticalAlignment = VerticalAlignment.Top;
            avgDeNoising.Margin = new Thickness(this.weightedAvgRBtn.Margin.Left + this.weightedAvgRBtn.Width + 5, avgNoiseReductLabel.Margin.Top, 5, 0);

            avgDeNoising.Click += avgDenoisingBtn_Click;
            this.imageOptionsGrid.Children.Add(avgDeNoising);
            Button medDeNoising = new Button();
            medDeNoising.Content = "Apply";
            medDeNoising.HorizontalAlignment = HorizontalAlignment.Stretch;
            medDeNoising.VerticalAlignment = VerticalAlignment.Top;
            medDeNoising.Margin = new Thickness(this.weightedAvgRBtn.Margin.Left + this.weightedAvgRBtn.Width + 5, medNoiseReductLabel.Margin.Top, 5, 0);
            medDeNoising.Click += medDenoisingBtn_Click;
            this.imageOptionsGrid.Children.Add(medDeNoising);
        }
        private void addNoiseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            imagesUndone = new List<ImageHistoryModel>();
            currentImage = GeneralImageMethods.AddNoiseToImage(currentImage);
            imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.AddedNoise });
            this.AddBitmapImageToImage();
        }
        private void avgDenoisingBtn_Click(object sender, EventArgs e)
        {
            imagesUndone = new List<ImageHistoryModel>();
            currentImage = ImageFilters.ApplyAvgDenoisingFilter(currentImage, this.weightedAvgRBtn.IsChecked.Value);
            imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.ClearedNoise });
            this.AddBitmapImageToImage();
        }
        private void medDenoisingBtn_Click(object sender, EventArgs e)
        {
            imagesUndone = new List<ImageHistoryModel>();
            currentImage = ImageFilters.ApplyMedDenoisingFilter(currentImage);
            imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.ClearedNoise });
            this.AddBitmapImageToImage();
        }
        #endregion

        #region Window Events
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.saveMenuItem.IsEnabled = false;
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                this.OpenFile();
            }
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                this.SaveFile(A.Save);
            }
        }
        #endregion

        #region MouseEvents
        private void imageContainer_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Controls.Image imgContainer = (System.Windows.Controls.Image)sender;
            imgContainer.Focus();
            Border border = (Border)imgContainer.Parent;
            border.BorderBrush = Brushes.Blue;
        }
        #endregion

        #region ShortcutMethods
        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName.ToLower();
                if (fileName.EndsWith(".png") ||
                    fileName.EndsWith(".jpg") ||
                    fileName.EndsWith(".gif"))
                {
                    lastLoadedOrSavedFileName = fileName;
                    currentImage = new System.Drawing.Bitmap(openFileDialog.FileName);
                    originalImage = (System.Drawing.Bitmap)currentImage.Clone();
                    isGrayScale = GeneralImageMethods.IsGrayscale(currentImage);
                    zoomRatio = 1;
                    imageHistory = new List<ImageHistoryModel>();
                    imagesUndone = new List<ImageHistoryModel>();
                    imageHistory.Add(new ImageHistoryModel { Image = originalImage, Manipulation = HelperClasses.Manipulation.NoManipulation });
                    //imageContainer = new System.Windows.Controls.Image();
                    imageContainer.Visibility = Visibility.Visible;
                    imageContainer.Name = "currentImage";
                    imageContainer.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    imageContainer.Width = currentImage.Width;
                    imageContainer.Height = currentImage.Height;
                    int marginRightLeft = Convert.ToInt32((this.LoadedImageContainer.Width - imageContainer.Width) > 0 ? (this.LoadedImageContainer.Width - imageContainer.Width) : 0 / 2);
                    int marginTopBottom = Convert.ToInt32((this.LoadedImageContainer.Height - imageContainer.Height) > 0 ? (this.LoadedImageContainer.Height - imageContainer.Height) : 0 / 2);

                    //Border border = new Border();
                    border.Width = imageContainer.Width + 4;
                    border.Height = imageContainer.Height + 4;
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = Brushes.White;
                    border.Child = imageContainer;
                    border.Margin = new Thickness(marginRightLeft, marginTopBottom, marginRightLeft, marginTopBottom);
                    this.LoadedImageContainer.Content = border;
                    this.saveMenuItem.IsEnabled = false;
                    imageContainer.MouseDown += imageContainer_MouseDown;
                    this.zoomRatioTxt.Text = "1";
                    this.EnableMenuItems();
                    this.DisplayHistogram(currentImage);
                    this.fileTextBox.Text = this.lastLoadedOrSavedFileName;
                    this.imgSize.Text = currentImage.Width + "x" + currentImage.Height;
                }
            }
        }

        private void SaveFile(A saveOption)
        {
            //System.Drawing.Bitmap imgToSave = new System.Drawing.Bitmap(currentImage);
            if (saveOption == A.SaveAs)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "Image files (*.bmp, *.jpg, *.png, *.gif)|*.bmp;*.jpg;*.png;*.gif|All files (*.*)|*.*";
                string extension = System.IO.Path.GetExtension(lastLoadedOrSavedFileName);

                fileDialog.FileName = "Unknown" + extension;

                if (fileDialog.ShowDialog() == true)
                {
                    if (System.IO.File.Exists(fileDialog.FileName))
                    {
                        System.IO.File.Delete(fileDialog.FileName);
                    }
                    currentImage.Save(fileDialog.FileName);
                }

            }
            //else
            //{
            //    imageHistory = new List<ImageHistoryModel>();
            //    currentImage.Dispose();
            //    originalImage.Dispose();
            //    this.imageContainer = new System.Windows.Controls.Image();
            //    if (System.IO.File.Exists(lastLoadedOrSavedFileName))
            //    {
            //        System.IO.File.Delete(lastLoadedOrSavedFileName);
            //    }
            //    imgToSave
            //        .Save(lastLoadedOrSavedFileName);

            //}
            //currentImage = imgToSave;
            //imgToSave.Dispose();
            //this.AddBitmapImageToImage(null,true);

        }
        #endregion


        #region Histogram methods & event handlers
        private void DisplayHistogram(System.Drawing.Bitmap image)
        {
            ImageStatisticsHSL hslStatistics = null;
            ImageStatistics rgbStatistics = new ImageStatistics(image);
            if (!isGrayScale)
            {
                // Luminance
                hslStatistics = new ImageStatisticsHSL(image);
                this.LuminanceHistogramPoints = ConvertToPointCollection(hslStatistics.Luminance.Values);
                // RGB


                this.RedColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Red.Values);
                this.GreenColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Green.Values);
                this.BlueColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Blue.Values);
                this.histogramGraph.Points = this.LuminanceHistogramPoints;
                this.histogramBorder.Background = Brushes.White;
                this.luminosityBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                this.luminosityBtn.IsEnabled = true;
                this.luminosityBtn.Content = "Luminosity";
                this.redBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
                this.redBtn.IsEnabled = true;
                this.blueBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
                this.blueBtn.IsEnabled = true;
                this.greenBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
                this.greenBtn.IsEnabled = true;
            }
            else {
                this.luminosityBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                this.luminosityBtn.IsEnabled = true;
                this.luminosityBtn.Content = "Gray level";
                this.GrayColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Gray.Values);
                this.greenBtn.IsEnabled = false;
                this.blueBtn.IsEnabled = false;
                this.redBtn.IsEnabled = false;

            }
            this.UpdateStatistics(rgbStatistics, hslStatistics);

        }
        private void UpdateStatistics(ImageStatistics rgbStats, ImageStatisticsHSL hslStats = null)
        {
            if (!isGrayScale)
            {
                this.pixelCount.Text = "Pixel Count: " + rgbStats.PixelsCount + "px";
                this.luminosity.Text = "Luminosity";
                this.lumRange.Text = "Range: [" + Math.Round(hslStats.Luminance.Min, 3) + "; " + Math.Round(hslStats.Luminance.Max, 3) + "]";
                this.lumMed.Text = "Med: " + Math.Round(hslStats.Luminance.Median, 3);
                this.lumAvg.Text = "Avg: " + Math.Round(hslStats.Luminance.Mean, 3);
                this.lumStdDev.Text = "S. dev: " + Math.Round(hslStats.Luminance.StdDev, 3);

                this.redRange.Text = "Range: [" + rgbStats.Red.Min + "; " + rgbStats.Red.Max + "]";
                this.redMed.Text = "Med: " + rgbStats.Red.Median;
                this.redAvg.Text = "Avg: " + Math.Round(rgbStats.Red.Mean, 0);
                this.redStdDev.Text = "S. dev: " + Math.Round(rgbStats.Red.StdDev, 2);

                this.blueRange.Text = "Range: [" + rgbStats.Blue.Min + "; " + rgbStats.Blue.Max + "]";
                this.blueMed.Text = "Med: " + rgbStats.Blue.Median;
                this.blueAvg.Text = "Avg: " + Math.Round(rgbStats.Blue.Mean, 0);
                this.blueStdDev.Text = "S. dev: " + Math.Round(rgbStats.Blue.StdDev, 2);

                this.greenRange.Text = "Range: [" + rgbStats.Green.Min + "; " + rgbStats.Green.Max + "]";
                this.greenMed.Text = "Med: " + rgbStats.Green.Median;
                this.greenAvg.Text = "Avg: " + Math.Round(rgbStats.Green.Mean, 0);
                this.greenStdDev.Text = "S. dev: " + Math.Round(rgbStats.Green.StdDev, 2);
            }
            else {
                this.pixelCount.Text = "Pixel Count: " + rgbStats.PixelsCount + "px";
                this.luminosity.Text = "Gray";
                this.lumRange.Text = "Range: [" + rgbStats.Gray.Min + "; " + rgbStats.Gray.Max + "]";
                this.lumMed.Text = "Med: " + rgbStats.Gray.Median;
                this.lumAvg.Text = "Avg: " + Math.Round(rgbStats.Gray.Mean, 0);
                this.lumStdDev.Text = "S. dev: " + Math.Round(rgbStats.Gray.StdDev, 3);

                this.redRange.Text = "Range:";
                this.redMed.Text = "Med: ";
                this.redAvg.Text = "Avg: ";
                this.redStdDev.Text = "S. dev: ";

                this.blueRange.Text = "Range: ";
                this.blueMed.Text = "Med: ";
                this.blueAvg.Text = "Avg: ";
                this.blueStdDev.Text = "S. dev: ";

                this.greenRange.Text = "Range: ";
                this.greenMed.Text = "Med: ";
                this.greenAvg.Text = "Avg: ";
                this.greenStdDev.Text = "S. dev: ";
            }
        }
        private void btnLuminosity_Click(object sender, RoutedEventArgs e)
        {
            if (isGrayScale)
            {
                this.histogramGraph.Points = this.GrayColorHistogramPoints;
            }
            else
            {
                this.histogramGraph.Points = this.LuminanceHistogramPoints;
            }
            this.histogramGraph.Fill = Brushes.Black;
            this.luminosityBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            this.redBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.blueBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.greenBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
        }
        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            this.histogramGraph.Points = this.RedColorHistogramPoints;
            this.histogramGraph.Fill = Brushes.Red;
            this.luminosityBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.redBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            this.blueBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.greenBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
        }
        private void blueBtn_Click(object sender, RoutedEventArgs e)
        {
            this.histogramGraph.Points = this.BlueColorHistogramPoints;
            this.histogramGraph.Fill = Brushes.Blue;
            this.luminosityBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.redBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.blueBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            this.greenBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
        }
        private void greenBtn_Click(object sender, RoutedEventArgs e)
        {
            this.histogramGraph.Points = this.GreenColorHistogramPoints;
            this.histogramGraph.Fill = Brushes.Green;
            this.luminosityBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.redBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.blueBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.greenBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
        }

        private PointCollection ConvertToPointCollection(int[] values)
        {
            int max = values.Max();

            PointCollection points = new PointCollection();
            // first point (lower-left corner)
            points.Add(new Point(0, max));
            // middle points
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new Point(i, max - values[i]));
            }
            // last point (lower-right corner)
            points.Add(new Point(values.Length - 1, max));

            return points;
        }


        private void histogramMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imageOptionsGrid.Children.RemoveRange(3, this.imageOptionsGrid.Children.Count - 3);
            this.brightnessContrastControls.Visibility = Visibility.Hidden;
            this.colorAdjustement.Visibility = Visibility.Hidden;
            this.equalizeHistogramBtn = new Button();
            this.equalizeHistogramBtn.Content = "Equalize Histogram";
            this.equalizeHistogramBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF9, 0xF5, 0xF5));
            this.equalizeHistogramBtn.VerticalAlignment = VerticalAlignment.Top;
            this.equalizeHistogramBtn.HorizontalAlignment = HorizontalAlignment.Left;
            this.equalizeHistogramBtn.Margin = new Thickness(15, this.labelImgTools.Margin.Top + this.labelImgTools.Height + 15, 0, 0);
            this.equalizeHistogramBtn.Click += equalizeHistogramBtn_Click;
            this.imageOptionsGrid.Children.Add(this.equalizeHistogramBtn);
        }
        private void equalizeHistogramBtn_Click(object sender, EventArgs e)
        {
            if (this.imageHistory.Last().Manipulation == HelperClasses.Manipulation.HistogramEqualization)
            {
                return;
            }
            currentImage = ImageFilters.EqualizeHistogram(currentImage);
            imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.HistogramEqualization });
            AddBitmapImageToImage();
        }

        #endregion

        #region Shapes menu methods and handlers
        public void detectFacesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            imagesUndone = new List<ImageHistoryModel>();
            currentImage = ShapeDetector.DetectFacesAndDrawBorders(currentImage);
            imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.FaceBordersDrawn });
            this.AddBitmapImageToImage();
        }
        //public void findFacesMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    imagesUndone = new List<ImageHistoryModel>();
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        string fileName = openFileDialog.FileName.ToLower();
        //        if (fileName.EndsWith(".png") ||
        //            fileName.EndsWith(".jpg") ||
        //            fileName.EndsWith(".gif"))
        //        {
        //            System.Drawing.Bitmap face = new System.Drawing.Bitmap(openFileDialog.FileName);
        //            ShapeDetector.FindFace(currentImage, face);                   
        //        }
        //    }

        //}
        #endregion

        public void AddBitmapImageToImage(System.Drawing.Bitmap image = null, bool isFromSave = false)
        {
            if (image == null)
            {
                image = currentImage;
            }
            imageContainer.Source = GeneralImageMethods.BitmapToImageSource(image);
            imageContainer.Width = image.Width;
            imageContainer.Height = image.Height;
            int marginRightLeft = Convert.ToInt32((this.LoadedImageContainer.Width - imageContainer.Width) > 0 ? (this.LoadedImageContainer.Width - imageContainer.Width) : 0 / 2);
            int marginTopBottom = Convert.ToInt32((this.LoadedImageContainer.Height - imageContainer.Height) > 0 ? (this.LoadedImageContainer.Height - imageContainer.Height) : 0 / 2);

            
            border.Width = imageContainer.Width + 4;
            border.Height = imageContainer.Height + 4;
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = Brushes.Blue;
            //border.Child = imageContainer;
            border.Margin = new Thickness(marginRightLeft, marginTopBottom, marginRightLeft, marginTopBottom);
            this.imgSize.Text = image.Width + "x" + image.Height;
            this.LoadedImageContainer.Content = border;
            this.saveMenuItem.IsEnabled = !isFromSave;
            this.saveAsMenuItem.IsEnabled = !isFromSave;
            this.saveToolbarBtn.IsEnabled = !isFromSave;
            this.undoToolbarBtn.IsEnabled = !isFromSave;
            this.DisplayHistogram(image);
        }

        private void EnableMenuItems()
        {
            //
            //Edit Menu
            //
            this.zoomInMenuItem.IsEnabled = true;
            this.zoomOutMenuItem.IsEnabled = true;
            this.cropMenuItem.IsEnabled = true;
            this.resizeMenuItem.IsEnabled = true;
            this.rotateMenuItem.IsEnabled = true;
            this.grayscaleMenuItem.IsEnabled = true;
            //
            //Effects Menu
            //
            this.sepiaMenuItem.IsEnabled = true;
            this.negativeMenuItem.IsEnabled = true;
            this.sketchMenuItem.IsEnabled = true;
            this.embossMenuItem.IsEnabled = true;
            //this.hiSatchMenuItem.IsEnabled = true;
            //this.blackWhiteMenuItem.IsEnabled = true;
            //this.comicMenuItem.IsEnabled = true;
            //this.gothamMenuItem.IsEnabled = true;
            //this.lomographMenuItem.IsEnabled = true;
            //this.loSatchMenuItem.IsEnabled = true;
            //this.polaroidMenuItem.IsEnabled = true;
            //
            //Adjust Menu
            //
            this.colorMenuItem.IsEnabled = true;
            this.brightnessContrastMenuItem.IsEnabled = true;
            this.sharpenMenuItem.IsEnabled = true;
            this.smoothMenuItem.IsEnabled = true;
            this.imageEdgesMenuItem.IsEnabled = true;
            //
            //Enhance Menu
            //
            this.histogramMenuItem.IsEnabled = true;
            this.clearNoiseMenuItem.IsEnabled = true;
            //
            //Degrade Menu
            //
            this.addNoiseMenuItem.IsEnabled = true;
            //
            //Shapes menu
            //
            this.detectFacesMenuItem.IsEnabled = true;
            //this.findFacesMenuItem.IsEnabled = true;
        }


        private void colorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            if (!isGrayScale)
            {

                temp = (System.Drawing.Bitmap)currentImage.Clone();
                brightnessContrastControls.Visibility = Visibility.Hidden;
                this.imageOptionsGrid.Children.RemoveRange(3, this.imageOptionsGrid.Children.Count - 3);
                this.colorAdjustement.Visibility = Visibility.Visible;
            }
        }

        private void rgb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            temp = GeneralImageMethods.AdjustRGB(currentImage, (int)redSlider.Value, (int)greenSlider.Value, (int)blueSlider.Value);
            
            this.redValue.Text = ((int)redSlider.Value).ToString();
            this.blueValue.Text = ((int)blueSlider.Value).ToString();
            this.greenValue.Text = ((int)greenSlider.Value).ToString();
            this.AddBitmapImageToImage(temp);
        }

        private void rgbBtn_Click(object sender, RoutedEventArgs e)
        {
            currentImage = GeneralImageMethods.AdjustRGB(currentImage, (int)redSlider.Value, (int)greenSlider.Value, (int)blueSlider.Value);
            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.RGBColorChange});
            this.redValue.Text = ((int)redSlider.Value).ToString();
            this.blueValue.Text = ((int)blueSlider.Value).ToString();
            this.greenValue.Text = ((int)greenSlider.Value).ToString();
        }

        
        private void redValue_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox senderTxt = sender as TextBox;
            if (String.IsNullOrEmpty(senderTxt.Text))
            {
                return;
            }
            int number = 0;
            redSlider.Value = Int32.TryParse(this.redValue.Text, out number) ? number : 0;

        }
        private void blueValue_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox senderTxt = sender as TextBox;
            if (String.IsNullOrEmpty(senderTxt.Text))
            {
                return;
            }
            int number = 0;
            blueSlider.Value = Int32.TryParse(this.blueValue.Text, out number) ? number : 0;

        }
        private void greenValue_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox senderTxt = sender as TextBox;
            if (String.IsNullOrEmpty(senderTxt.Text))
            {
                return;
            }
            int number = 0;
            greenSlider.Value = Int32.TryParse(this.greenValue.Text, out number) ? number : 0;

        }

        private void hueValue_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox senderTxt = sender as TextBox;
            if (String.IsNullOrEmpty(senderTxt.Text))
            {
                return;
            }
            int number = 0;
            hueSlider.Value = Int32.TryParse(this.hueValue.Text, out number) ? number : 0;
            
        }

        private void satValue_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox senderTxt = sender as TextBox;
            if (String.IsNullOrEmpty(senderTxt.Text))
            {
                return;
            }
            int number = 0;
            
            saturationSlider.Value = Int32.TryParse(this.saturationValue.Text, out number) ? number : 0;
            
        }

        private void lumValue_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox senderTxt = sender as TextBox;
            if (String.IsNullOrEmpty(senderTxt.Text))
            {
                return;
            }
            int number = 0;            
            luminanceSlider.Value = Int32.TryParse(this.luminanceValue.Text, out number) ? number : 0;
        }

        private void hslColorApply_Click(object sender, RoutedEventArgs e)
        {
            currentImage = GeneralImageMethods.AdjustHLS(currentImage, (int)hueSlider.Value, (int)saturationSlider.Value, (int)luminanceSlider.Value);

            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.HSLColorChange });
            this.hueSlider.Value = 0;
            this.saturationSlider.Value = 0;
            this.luminanceSlider.Value = 0;
        }

        private void hslSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            temp = GeneralImageMethods.AdjustHLS(currentImage, (int)hueSlider.Value, (int)saturationSlider.Value, (int)luminanceSlider.Value);

            this.hueValue.Text = ((int)hueSlider.Value).ToString();
            this.saturationValue.Text = ((int)saturationSlider.Value).ToString();
            this.luminanceValue.Text = ((int)luminanceSlider.Value).ToString();
            this.AddBitmapImageToImage(temp);
        }

        private void autoBrightnessContrast_Click(object sender, RoutedEventArgs e)
        {
            temp = GeneralImageMethods.GammaCorrection(currentImage, 1.8);
            AddBitmapImageToImage(temp);
        }

        private void brightnessContrastMenuItem_Click(object sender, RoutedEventArgs e)
        {
            temp = (System.Drawing.Bitmap)currentImage.Clone();
            brightnessContrastControls.Visibility = Visibility.Visible;
            this.imageOptionsGrid.Children.RemoveRange(3, this.imageOptionsGrid.Children.Count - 3);
            this.colorAdjustement.Visibility = Visibility.Hidden;
        }

        private void brightnessContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            temp = GeneralImageMethods.AdjustContrastBrightness(currentImage, (int)brightnessSlider.Value, (int)contrastSlider.Value);

            this.brightnessTxt.Text = ((int)brightnessSlider.Value).ToString();
            this.contrastTxt.Text = ((int)contrastSlider.Value).ToString();
            this.AddBitmapImageToImage(temp);
        }


        private void applyContrastBrightnessBtn_Click(object sender, RoutedEventArgs e)
        {
            currentImage = temp;
            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.BrightnessContrastAdjusted });
        }

        private void brightnessTxt_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox senderTxt = sender as TextBox;
            if (String.IsNullOrEmpty(senderTxt.Text))
            {
                return;
            }
            int number = 0;
            Int32.TryParse(this.brightnessTxt.Text, out number);
            if (number < -255)
                number = -255;
            if (number > 255)
                number = 255;
            brightnessSlider.Value = number; 
        }
        private void contrastTxt_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox senderTxt = sender as TextBox;
            if (String.IsNullOrEmpty(senderTxt.Text))
            {
                return;
            }
            int number = 0;
            Int32.TryParse(this.contrastTxt.Text, out number);
            if (number < -255)
                number = -255;
            if (number > 255)
                number = 255;
            contrastSlider.Value = number;
        }

        private void backToOriginalBtn_Click(object sender, RoutedEventArgs e)
        {
            this.currentImage = originalImage;
            this.AddBitmapImageToImage();
        }

        private void smoothMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.smoothTools.Visibility = Visibility.Visible;
            this.colorAdjustement.Visibility = Visibility.Hidden;
            this.brightnessContrastControls.Visibility = Visibility.Hidden;
            this.resizeTools.Visibility = Visibility.Hidden;    
        }

        private void sepiaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();
            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Sepia });
            currentImage = ImageFilters.ApplySepiaFilter(currentImage);
            
            AddBitmapImageToImage();
        }

        private void negativeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();
            currentImage = ImageEffects.Invert(currentImage);
            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Inverted });
            AddBitmapImageToImage();
        }

        private void resizeApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();
            int newWidth = 0;
            if (Int32.TryParse(this.widthTxt.Text, out newWidth))
            {

            }
            else {
                newWidth = currentImage.Width;
            }
            int newHeight = 0;
            if (Int32.TryParse(this.heightTxt.Text, out newHeight))
            {

            }
            else
            {
                newHeight = currentImage.Height;
            }
            currentImage = GeneralImageMethods.Resize(currentImage, newWidth, newHeight);
            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage});
            AddBitmapImageToImage();
        }
        private void widthTxt_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(this.aspectRatioCheckBox.IsChecked == true))
            {
                return;
            }
            int newWidth = 0;
            if (Int32.TryParse(this.widthTxt.Text, out newWidth))
            {
                this.heightTxt.Text = Math.Floor((double)newWidth / ((double)currentImage.Width / currentImage.Height)).ToString();
            }
        }
        private void heightTxt_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(this.aspectRatioCheckBox.IsChecked == true))
            {
                return;
            }
            int newHeight = 0;
            if (Int32.TryParse(this.widthTxt.Text, out newHeight))
            {
                this.widthTxt.Text = Math.Floor((double)newHeight * ((double)currentImage.Width / currentImage.Height)).ToString();
            }
        }

        private void resizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.resizeTools.Visibility = Visibility.Visible;
            this.colorAdjustement.Visibility = Visibility.Hidden;
            this.brightnessContrastControls.Visibility = Visibility.Hidden;
            this.widthTxt.Text = currentImage.Width.ToString();
            this.heightTxt.Text = currentImage.Height .ToString();

        }

        private void cropMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!isSelected)
            {
                return;
            }
           
            this.imagesUndone = new List<ImageHistoryModel>();
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(Math.Min(x0, x1), Math.Min(y0, y1),
                Math.Abs(x0 - x1), Math.Abs(y0 - y1));
            currentImage = GeneralImageMethods.Crop(currentImage, rect);
            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage });
            AddBitmapImageToImage();
        }


        private void zoomToolbarBtn_Click(object sender, RoutedEventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();

            zoomRatio = Convert.ToDouble(this.zoomRatioTxt.Text);
            currentImage = GeneralImageMethods.Zoom(imageHistory.Last(i => i.Manipulation != HelperClasses.Manipulation.Zoom).Image, zoomRatio);
            imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Zoom });
            this.AddBitmapImageToImage();
            this.redoToolbarBtn.IsEnabled = false;
        }


        private void applyGaussianSmoothBtn_Click(object sender, RoutedEventArgs e)
        {
            this.imagesUndone = new List<ImageHistoryModel>();
            int kernel = 0;
            double sigma = 0;
            if (Int32.TryParse(this.smoothKernelValueTxt.Text, out kernel))
            {

            }
            else {
                return;
            }
            if (Double.TryParse(this.sigmaValueTxt.Text, out sigma))
            {

            }
            else
            {
                return;
            }
            currentImage = ImageFilters.ApplyGaussianBlurFilter(currentImage, kernel, sigma);

            this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Smooth });
            AddBitmapImageToImage();
        }

        //private void blackWhiteMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    this.imagesUndone = new List<ImageHistoryModel>();
        //    currentImage = ImageEffects.BlackWhite(currentImage);
        //    this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.BlackWhite });
        //    AddBitmapImageToImage();
        //}

        //private void comicMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    this.imagesUndone = new List<ImageHistoryModel>();
        //    currentImage = ImageEffects.Comic(currentImage);
        //    this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Comic });
        //    AddBitmapImageToImage();
        //}

        //private void gothamMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    this.imagesUndone = new List<ImageHistoryModel>();
        //    currentImage = ImageEffects.Gotham(currentImage);
        //    this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Gotham });
        //    AddBitmapImageToImage();
        //}

        //private void hiSatchMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    this.imagesUndone = new List<ImageHistoryModel>();
        //    currentImage = ImageEffects.HiSatch(currentImage);
        //    this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.HiSatch });
        //    AddBitmapImageToImage();
        //}

        //private void lomographMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    this.imagesUndone = new List<ImageHistoryModel>();
        //    currentImage = ImageEffects.Lomograph(currentImage);
        //    this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Lomograph });
        //    AddBitmapImageToImage();
        //}

        //private void loSatchMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    this.imagesUndone = new List<ImageHistoryModel>();
        //    currentImage = ImageEffects.LoSatch(currentImage);
        //    this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.LoSatch });
        //    AddBitmapImageToImage();
        //}

        //private void polaroidMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    this.imagesUndone = new List<ImageHistoryModel>();
        //    currentImage = ImageEffects.Polaroid(currentImage);
        //    this.imageHistory.Add(new ImageHistoryModel { Image = currentImage, Manipulation = HelperClasses.Manipulation.Polaroid });
        //    AddBitmapImageToImage();
        //}

        private void imageContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isSelecting = true;
            isSelected = false;
            //selection = new System.Drawing.Rectangle((int)e.MouseDevice.GetPosition((System.Windows.Controls.Image)sender).X, (int)e.MouseDevice.GetPosition((System.Windows.Controls.Image)sender).Y, 0, 0);
            x0 = (int)e.MouseDevice.GetPosition((System.Windows.Controls.Image)sender).X;
            y0 = (int)e.MouseDevice.GetPosition((System.Windows.Controls.Image)sender).Y;
        }

        private void imageContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isSelecting)
            {
                return;
            }
            x1 = (int)e.MouseDevice.GetPosition((System.Windows.Controls.Image)sender).X;
            y1 = (int)e.MouseDevice.GetPosition((System.Windows.Controls.Image)sender).Y;
            //selection.Width = Math.Abs(currentX - x0);
            //selection.Height = Math.Abs(currentY - y0);
            //if (currentX < x0)
            //{
            //    selection.X = currentX;
            //}
            //if (currentY < y0)
            //{
            //    selection.Y = currentY;
            //}
            System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)currentImage.Clone();
            using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp))
            {
                gr.DrawRectangle(System.Drawing.Pens.DarkSlateGray, Math.Min(x0, x1), Math.Min(y0, y1),
                Math.Abs(x0 - x1), Math.Abs(y0 - y1));
            }
            this.imageContainer.Source = GeneralImageMethods.BitmapToImageSource(bmp);

        }

        private void imageContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isSelected)
            {
                this.imageContainer.Source = GeneralImageMethods.BitmapToImageSource(currentImage);
                isSelecting = false;
                isSelected = false;
                return;
            }
            else if (isSelecting)
            {
                
                isSelecting = false;
                isSelected = true;
            }
        }
    }
    public enum A { Save = 0, SaveAs = 1 }
}
