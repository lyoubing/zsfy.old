using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Common
{
  public  class ImageHelper
    {
      public static System.Drawing.Image GenThumbnail(System.Drawing.Image imageFrom, int width, int height)
      {
          // 源图宽度及高度 
          int imageFromWidth = imageFrom.Width;
          int imageFromHeight = imageFrom.Height;

          // 生成的缩略图实际宽度及高度 
          if (width >= imageFromWidth && height >= imageFromHeight)
          {
              return imageFrom;

          }
          else
          {
              // 生成的缩略图在上述"画布"上的位置 
              int X = 0;
              int Y = 0;

              decimal wpercent = (decimal)width / imageFromWidth;
              decimal hpercent = (decimal)height / imageFromHeight;
              if (wpercent > hpercent)
              {
                  width = (int)(imageFromWidth * hpercent);
              }
              else if (wpercent < hpercent)
              {
                  height = (int)(imageFromHeight * wpercent);
              }

              // 创建画布 
              Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
              bmp.SetResolution(imageFrom.HorizontalResolution, imageFrom.VerticalResolution);
              using (Graphics g = Graphics.FromImage(bmp))
              {
                  // 用白色清空 
                  g.Clear(Color.White);

                  // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
                  g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                  // 指定高质量、低速度呈现。 
                  g.SmoothingMode = SmoothingMode.HighQuality;

                  // 在指定位置并且按指定大小绘制指定的 Image 的指定部分。 
                  g.DrawImage(imageFrom, new System.Drawing.Rectangle(X, Y, width, height), new System.Drawing.Rectangle(0, 0, imageFromWidth, imageFromHeight), GraphicsUnit.Pixel);

                  return bmp;
              }
          }
      }


      //public static BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
      //{
      //    MemoryStream ms = new MemoryStream();
      //    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
      //    BitmapImage bit3 = new BitmapImage();
      //    bit3.BeginInit();
      //    bit3.StreamSource = ms;
      //    bit3.EndInit();
      //    return bit3;
      //}
    }
}
