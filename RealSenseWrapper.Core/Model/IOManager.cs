using RealSenseWrapper.Core.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace RealSenseWrapper.Core.Model
{
    static class IOManager
    {
        
        /// <summary>
        /// Store into the compiled folder the user photo used to retrieve user information
        /// </summary>
        /// <param name="faceImageSource">face image source</param>
        /// <param name="imageFileName">face image name</param>
        /// <returns>1 if store success, 0 otherwise</returns>
        public static int SaveUserImage(BitmapSource faceImageSource, string imageFileName)
        {
            int resultCode = 1;

            if (File.Exists(imageFileName))
            {
                File.Delete(imageFileName);
            }

            using (FileStream saveSnapshotStream = new FileStream(imageFileName, FileMode.CreateNew))
            {
                try
                {
                    JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
                    jpgEncoder.QualityLevel = 100;
                    jpgEncoder.Frames.Add(BitmapFrame.Create(faceImageSource));
                    jpgEncoder.Save(saveSnapshotStream);

                    saveSnapshotStream.Flush();
                    saveSnapshotStream.Close();
                    saveSnapshotStream.Dispose();
                }
                catch (Exception e)
                {
                     resultCode = 0;
                }
            }

            return resultCode;
        }
    }
}
