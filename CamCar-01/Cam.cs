using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices;

namespace CamCar_01
{
    [ComImport]
    [Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
  {
        void GetBuffer(out byte* buffer, out uint capacity);
   }
    class Cam
    {
       public Windows.Media.Capture.MediaCapture _mediaCapture = new Windows.Media.Capture.MediaCapture();
        
        async public void Initialize()
        {
            (App.Current as App).MediaCapture = _mediaCapture;

            await _mediaCapture.InitializeAsync();
        } 
         public async void Initialize2()
        {
            var VideoModesList = _mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview).ToArray();
             _mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, VideoModesList[24]);
           await _mediaCapture.StartPreviewAsync();
            await _mediaCapture.StopPreviewAsync();
            await _mediaCapture.StartPreviewAsync();
            await   _mediaCapture.StopPreviewAsync();
          await  _mediaCapture.StartPreviewAsync();
        }
        public async Task<byte[,]> GetPixels()
        {
            try
            {

   
               var previewProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
                //s

                var videoFrame = new VideoFrame(BitmapPixelFormat.Gray8, (int)previewProperties.Width, (int)previewProperties.Height);
                using (var currentFrame = await _mediaCapture.GetPreviewFrameAsync(videoFrame))
                {
                    SoftwareBitmap previewFrame = currentFrame.SoftwareBitmap;
                        return EditPixels(previewFrame);
                    //return null;
                }
            }
            catch (Exception ex)
            { 
                System.Diagnostics.Debug.WriteLine("exception1");
                return null;
            }
            }

        public unsafe byte[,] EditPixels(SoftwareBitmap bitmap)
        {
            try
            {
                //byte[] array = new byte[????];
                byte[,] array = new byte[320, 240];
                if (bitmap.BitmapPixelFormat == BitmapPixelFormat.Gray8)
                {
                    const int BYTES_PER_PIXEL = 1;

                    using (var buffer = bitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
                    using (var reference = buffer.CreateReference())
                    {
                        byte* data;
                        uint capacity;
                        ((IMemoryBufferByteAccess)reference).GetBuffer(out data, out capacity);

                        var desc = buffer.GetPlaneDescription(0);

                        for (uint row = 0; row < desc.Height; row++)
                        {
                            for (uint col = 0; col < desc.Width; col++)
                            {
                                var currPixel = desc.StartIndex + desc.Stride * row + BYTES_PER_PIXEL * col;
                                //         array[currPixel / 8] <<= 1;
                                //       array[currPixel / 8] += (data[currPixel] > 127) ? (byte)0 : (byte)1;                       byte/8
                                //     array[col, row] = !(data[currPixel] > 127);                                                       bool
                                array[col, row] = data[currPixel];
                            }
                        }
                    }
                }
           //     System.Diagnostics.Debug.WriteLine("Success");
                return array;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("exception2");
                return null;
            }
            }
    }
}
