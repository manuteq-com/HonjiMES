
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace HonjiMES.Helper
{
    public class BarcodeHelper
    {
        /// <summary>
        /// 生成二維碼
        /// </summary>
        /// <param name="text">內容</param>
        /// <param name="width">寬度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public static byte[] CreateQrCode(string text,int width,int height)
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = width,
                    Height = height,
                }
            };

            var qrCodeImage = writer.Write(text);

            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }


    }
}