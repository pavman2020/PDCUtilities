using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace PDCUtility
{
    public class Bitmap
    {
        public static System.Drawing.Bitmap ScaleBitmap(Image oldBitmap, SizeF oNewSize)
        {
            return ScaleBitmap(oldBitmap, new Size(Convert.ToInt32(oNewSize.Width), Convert.ToInt32(oNewSize.Height)));
        }

        public static System.Drawing.Bitmap ScaleBitmap(Image oldBitmap, Size oNewSize)
        {
            return new System.Drawing.Bitmap(oldBitmap, oNewSize);
        }

        public static System.Drawing.Bitmap RotateBitmap90Degrees(System.Drawing.Bitmap b)
        {
            System.Drawing.Bitmap returnBitmap = new System.Drawing.Bitmap(b.Height, b.Width);
            Graphics g = Graphics.FromImage(returnBitmap);
            g.TranslateTransform((float)b.Height / 2, (float)b.Width / 2);
            g.RotateTransform(90);
            g.TranslateTransform(-(float)b.Height / 2, -(float)b.Width / 2);
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }

        public static Image SetImageOpacity(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix
                    {

                        //set the opacity  
                        Matrix33 = opacity
                    };

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static Graphics CenterImage(Graphics oGrfx, Point oCenterPt, System.Drawing.Bitmap oImage)
        {
            Point P1 = new Point(oCenterPt.X - (oImage.Width / 2), oCenterPt.Y - (oImage.Height / 2));
            Rectangle Rect = new Rectangle(P1, oImage.Size);
            oGrfx.DrawImage(oImage, Rect);
            return oGrfx;
        }

        public static SizeF GetScalingFactor(SizeF oOrigSize, SizeF oDesiredSize, bool bMaintainAspectRatio = false)
        {
            SizeF oRet = new SizeF();

            if (0 != oOrigSize.Width) oRet.Width = oDesiredSize.Width / oOrigSize.Width;
            if (0 != oOrigSize.Height) oRet.Height = oDesiredSize.Height / oOrigSize.Height;

            if ((0 == oRet.Width) && (0 != oRet.Height)) oRet.Width = oRet.Height;
            if ((0 == oRet.Height) && (0 != oRet.Width)) oRet.Height = oRet.Width;

            if (bMaintainAspectRatio)
            {
                if (oRet.Height > oRet.Width) oRet.Height = oRet.Width;
                if (oRet.Width > oRet.Height) oRet.Width = oRet.Height;
            }

            return oRet;
        }


        public static System.Drawing.Bitmap MakeTransparent(System.Drawing.Bitmap oImage)
        {
            System.Drawing.Bitmap oRet = new System.Drawing.Bitmap(oImage);
            {
                System.Drawing.Color oCol = oRet.GetPixel(1, 1);
                oRet.MakeTransparent(oCol);
            }

            return oRet;
        }
    }
}
