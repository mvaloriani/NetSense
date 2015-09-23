using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RealSenseWrapper.Core.Model
{
    public class RealSenseDataService : IRealSenseDataService
    {

        public void StoreUserImage(Action<int, Exception> callback, BitmapSource faceImageSource, string imageFileName)
        {
            int result = IOManager.SaveUserImage(faceImageSource, imageFileName);
            callback(result, null);
        }
    }
}