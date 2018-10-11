using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using System.Text;
using System.Diagnostics;

namespace CamCar_01
{
    public sealed partial class MainPage : Page
    {
        private static IAsyncAction workItemThread;
        GPIO _gpio1 = new GPIO();
        GPIO _gpio2 = new GPIO();
        Cam _cam = new Cam();
        //xxx Matvey = new xxx();
        public MainPage()
        {
                     _cam.Initialize();
                       _gpio1.Initialize(true, 11, 100, 0);
                     _gpio2.Initialize(true, 12, 100, 0);
            // Matvey.Initialize();
            this.InitializeComponent();
            PreviewControl.Source = _cam._mediaCapture;
            _cam.Initialize2();
            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
              (source) =>
                            {
                               CamInteract();
                            }, Windows.System.Threading.WorkItemPriority.High);
        }
        async void CamInteract()
        {
            while (true)
            {
                byte[,] massiv = await _cam.GetPixels();
        //        double angle = Matvey.LineSearch(massiv); 
          //      AngleConvert(angle);
            }
        }

        void AngleConvert(double k)
        {
            if (k > 0) { _gpio1.UpdatePWM(100); _gpio2.UpdatePWM(100 - k); }
            else if (k < 0) { _gpio2.UpdatePWM(100); _gpio1.UpdatePWM(100 - k); }
            else { _gpio1.UpdatePWM(100); _gpio2.UpdatePWM(100); }
        }

        private async void btn1_Click(object sender, RoutedEventArgs e)
        {
                  await  _cam._mediaCapture.StartPreviewAsync();
        }

        private async void btn2_Click(object sender, RoutedEventArgs e)
        {
            await _cam._mediaCapture.StopPreviewAsync();
        }

        private async void btn3_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile storageFile = await storageFolder.CreateFileAsync("line.txt", CreationCollisionOption.OpenIfExists);


            byte[,] massiv = await _cam.GetPixels();
                string line = "";
            for (int i = 0; i < 240; i++)
                {
                    for (int j = 0; j < 320; j++)
                    {
                        if (massiv[j, i] < 127) line += "■";
                        else
                         line += "□";
                    }
                    line += "\r\n";
                }
                FileIO.WriteTextAsync(storageFile, line); 

          /*  bool[,] massiv = await _cam.GetPixels();
            byte[] line = new byte[76800];
            for (int i = 0; i < 240; i++)
            {
                for (int j = 0; j < 320; j++)
                {
                    if (massiv[j, i]) line[320 * i + j] = (byte)255;
                    else
                        line[320 * i + j] = (byte)1;
                }
            }
                    MemoryStream ms = new MemoryStream(line);
                Image returnImage = Image.FromStream(ms);
                
            */

        }
    }
}
