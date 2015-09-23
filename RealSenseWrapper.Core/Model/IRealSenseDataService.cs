using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RealSenseWrapper.Core.Model
{
    public interface IRealSenseDataService
    {
        void StoreUserImage(Action<int, Exception> callback, BitmapSource faceImageSource, string imageFileName);
    }
}
