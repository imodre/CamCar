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
using Windows.UI.Core;

namespace CamCar_01
{
    public sealed partial class MainPage : Page
    {
        private static IAsyncAction workItemThread;
        GPIO _gpio1 = new GPIO();
       // GPIO _gpio2 = new GPIO();
        Cam _cam = new Cam();
        Stopwatch delayControl = new Stopwatch();
     
        public MainPage()
        {
                     _cam.Initialize();
                       _gpio1.Initialize(true, 11, 100, 0);
                        delayControl.Start();
         //            _gpio2.Initialize(true, 12, 100, 0);
            this.InitializeComponent();
            PreviewControl.Source = _cam._mediaCapture;
            _cam.Initialize2();
            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
              (source) =>
                            {
                               CamInteract();
                            }, Windows.System.Threading.WorkItemPriority.High);
        }
        void CamInteract()
        {
            while (true)
            {
                byte[,] massiv = _cam.GetPixels().Result;
                short angle = AllSideAlgorithm.GetAngle(massiv); 
                string data = AngleConvert(angle);
                DebugScreenUpdate(angle, data);
            }
        }

        string AngleConvert(short k)
        {
            if (k > 0) { string data = "255;" + Convert.ToString(255 - k) + ";."; delayControl.Stop(); if (delayControl.ElapsedMilliseconds >= 40) _gpio1.SerialSend(data); else { Thread.Sleep(Convert.ToInt32(40 - delayControl.ElapsedMilliseconds)); _gpio1.SerialSend(data); } delayControl.Reset(); delayControl.Start(); return data; }
            else if (k < 0) { string data = Convert.ToString(255 - k) + ";255;."; delayControl.Stop(); if (delayControl.ElapsedMilliseconds >= 40) _gpio1.SerialSend(data); else { Thread.Sleep(Convert.ToInt32(40 - delayControl.ElapsedMilliseconds)); _gpio1.SerialSend(data); } delayControl.Reset(); delayControl.Start(); return data; }
            else { string data = "255;255;."; delayControl.Stop(); if (delayControl.ElapsedMilliseconds >= 40) _gpio1.SerialSend(data); else { Thread.Sleep(Convert.ToInt32(40 - delayControl.ElapsedMilliseconds)); _gpio1.SerialSend(data); } delayControl.Reset(); delayControl.Start(); return data; }
        }

        void DebugScreenUpdate(short _first, string _second)
        {
            try
            {
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { first.Text = Convert.ToString(_first); });
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { second.Text = _second; });

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void Btn1_Click(object sender, RoutedEventArgs e)
        {
                  await  _cam._mediaCapture.StartPreviewAsync();
        }

        private async void Btn2_Click(object sender, RoutedEventArgs e)
        {
            await _cam._mediaCapture.StopPreviewAsync();
        }

        private async void Btn3_Click(object sender, RoutedEventArgs e)
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
