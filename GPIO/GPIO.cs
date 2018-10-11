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
using System.Net.Http;
using Windows.Devices.Gpio;
using Magellanic.ServoController;
using Windows.Devices.Pwm;
using Microsoft.IoT.DeviceCore.Pwm;
using Microsoft.IoT.Devices.Pwm;
using Windows.Devices.SerialCommunication;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;

namespace IoTUtilities
{
    public class GPIO
    {
        private static IAsyncAction workItemThread;
        GpioController gpio = GpioController.GetDefault();
        private PwmPin _pwmPin;
        private PwmController _pwmController;
        private double frequency;
        bool PWMEndlessUpdateMode = false;
        public string LastSerialResult;
        SerialDevice serialPort;

        public string Temp1;
        public string Temp2;
        public string CO1;
        public string CO2;

        DataWriter dataWriteObject = null;
        DataReader dataReaderObject = null;
        private CancellationTokenSource ReadCancellationTokenSource;

        public void GPIOout(int port, int mode, int millis)
        {
            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                (source) =>
                {
                    using (GpioPin pin = gpio.OpenPin(port))
                    {
                        if (mode == 1)
                            pin.Write(GpioPinValue.High);
                        else
                            pin.Write(GpioPinValue.Low);
                        pin.SetDriveMode(GpioPinDriveMode.Output);
                        ManualResetEvent mre = new ManualResetEvent(false);
                        mre.WaitOne(millis);
                    }

                }, Windows.System.Threading.WorkItemPriority.High);

        }
        public double GPIOin(int port, string pullMode)
        {
            using (GpioPin pin = gpio.OpenPin(port))
            {
                if (pullMode == "PULLDOWN")
                    pin.SetDriveMode(GpioPinDriveMode.InputPullDown);
                else if (pullMode == "PULLUP")
                    pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
                else pin.SetDriveMode(GpioPinDriveMode.Input);
                return Convert.ToDouble(pin.Read());
            }
        }
        private async void SetupPWM(int pin, double freq, double defaultActiveDutyCycle)
        {
            var pwmManager = new PwmProviderManager();
            pwmManager.Providers.Add(new SoftPwm());

            var pwmControllers = await pwmManager.GetControllersAsync();
            _pwmController = pwmControllers[0];
            _pwmController.SetDesiredFrequency(freq); //frequency in Hz

            _pwmPin = _pwmController.OpenPin(pin);
            if (PWMEndlessUpdateMode)
            {
                _pwmPin.Start();

                _pwmPin.SetActiveDutyCyclePercentage(defaultActiveDutyCycle); //set Active Duty in millis

            }

            frequency = freq;
        }
        public void UpdatePWM(double defaultActiveDutyCycle)
        {
            _pwmPin.SetActiveDutyCyclePercentage(defaultActiveDutyCycle); //set Active Duty in millis
        }
            public void UpdatePWM(double defaultActiveDutyCycle, int ActiveTime)
        {
                         //   workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                // (source) =>
                //{
                _pwmPin.Start();
                _pwmPin.SetActiveDutyCyclePercentage(defaultActiveDutyCycle / (1000 / frequency)); //set Active Duty in millis
                ManualResetEvent mre = new ManualResetEvent(false);
                mre.WaitOne(ActiveTime);
                _pwmPin.Stop();
                //   }, Windows.System.Threading.WorkItemPriority.High);
            
        }
        public void ServoRotate(double degree)
        {
            UpdatePWM((degree / 180 + 1) / (1000 / frequency), 1000);
        }


        public async void SetupSerial()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector("UART0");                   /* Find the selector string for the serial device   */
                var dis = await DeviceInformation.FindAllAsync(aqs);                    /* Find the serial device with our selector string  */
                serialPort = await SerialDevice.FromIdAsync(dis[0].Id);    /* Create an serial device with our selected device */
                if (serialPort == null) return;
                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.BaudRate = 9600;
                serialPort.Parity = SerialParity.None;
                serialPort.StopBits = SerialStopBitCount.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = SerialHandshake.None;
                ReadCancellationTokenSource = new CancellationTokenSource();
            }
            catch
            {
                Dispose();
            }
        }

        public async void SerialSend(string data)
        {
            try
            {
                if (serialPort != null)
                {
                    // Create the DataWriter object and attach to OutputStream
                    dataWriteObject = new DataWriter(serialPort.OutputStream);

                    //Launch the WriteAsync task to perform the write
                    await WriteAsync(data);
                }
            }
            catch
            {
            }
            finally
            {
                // Cleanup once complete
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
        }
        private async Task WriteAsync(string data)
        {
            Task<UInt32> storeAsyncTask;

            if (data.Length != 0)
            {
                // Load the text from the sendText input text box to the dataWriter object
                dataWriteObject.WriteString(data);

                // Launch an async task to complete the write operation
                storeAsyncTask = dataWriteObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await storeAsyncTask;
            }
        }

      
        public void Dispose()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;

        }
        public void Initialize(bool EndlessUpdateMode, int pin, double freq, double ActiveDuty) //servo - false,18,50,1.5
        {
            PWMEndlessUpdateMode = EndlessUpdateMode;
            SetupPWM(pin, freq, ActiveDuty); //servo - 18,50,1.5
            SetupSerial();
        }

    }
}
