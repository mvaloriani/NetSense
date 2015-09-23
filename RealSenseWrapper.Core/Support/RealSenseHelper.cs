using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using RealSenseWrapper.Core;
using System.Runtime.InteropServices;
namespace RealSenseWrapper.Core
{
    public static class RealSenseHelper
    {
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
        {
            BitmapSource bs = null;
            IntPtr ip = new IntPtr();
            try
            {
                if (source != null)
                {
                    ip = source.GetHbitmap();

                    bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                       IntPtr.Zero, Int32Rect.Empty,
                       System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    //source.Dispose();
                }
            }
            finally
            {
                
                DeleteObject(ip);
              //  GC.Collect();
            }

            return bs;
        }


      



    }
}
