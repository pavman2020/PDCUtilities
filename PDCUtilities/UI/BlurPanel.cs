using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PDCUtility
{
    public partial class UI
    {
        public class BlurPanel : System.Windows.Forms.Panel
        {
            public int ProgressBarHeight = 10;

            private System.Drawing.Bitmap m_oBitmap = null;

            private System.Drawing.Bitmap m_oBlurredBitmap = null;

            private System.Windows.Forms.ProgressBar m_oProgressBar = null;

            public BlurPanel()
            {
                this.Cursor = Cursors.WaitCursor;
            }

            public void Show(Button cancelButton = null, Control controlToCenter = null, bool showProgressBar = true)
            {
                if (null == m_oProgressBar)
                {
                    m_oProgressBar = new ProgressBar()
                    {
                        MarqueeAnimationSpeed = 10,
                        Name = "progressBar1",
                        Style = System.Windows.Forms.ProgressBarStyle.Marquee,
                        Step = 1,
                        Height = ProgressBarHeight,
                    };

                    this.Controls.Add(m_oProgressBar);
                }

                if (null != m_oBitmap)
                {
                    m_oBitmap.Dispose();
                    m_oBitmap = null;
                }

                if (null != m_oBlurredBitmap)
                {
                    m_oBlurredBitmap.Dispose();
                    m_oBlurredBitmap = null;
                }

                HandleResize(this, new EventArgs());

                Control cParent = this.Parent;

                //Rectangle bounds = new Rectangle(0, 0, this.Width, this.Height);
                Rectangle bounds = cParent.Bounds;
                bounds.X += X_OFFSET;
                bounds.Y += Y_OFFSET;
                bounds.Width -= W_OFFSET;
                bounds.Height -= H_OFFSET;

                m_oBitmap = new System.Drawing.Bitmap(bounds.Width, bounds.Height);

                using (Graphics g = Graphics.FromImage(m_oBitmap))
                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);

                if (null != m_oBitmap)
                {
                    m_oBlurredBitmap = Blur(m_oBitmap, 2);
                    this.BackgroundImage = m_oBlurredBitmap;
                }

                this.BringToFront();

                if (null != cancelButton) cancelButton.BringToFront();
                if (null != controlToCenter)
                {
                    controlToCenter.Left = (this.Width - controlToCenter.Width) / 2;
                    controlToCenter.Top = (this.Height - controlToCenter.Height) / 2;
                }

                this.Visible = true;
            }

            private static System.Drawing.Bitmap Blur(System.Drawing.Bitmap image, Int32 blurSize)
            {
                return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);
            }

            private static unsafe System.Drawing.Bitmap Blur(System.Drawing.Bitmap image, Rectangle rectangle, Int32 blurSize)
            {
                System.Drawing.Bitmap blurred = new System.Drawing.Bitmap(image.Width, image.Height);

                // make an exact copy of the bitmap provided
                using (Graphics graphics = Graphics.FromImage(blurred))
                    graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                        new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

                // Lock the bitmap's bits
                BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

                // Get bits per pixel for current PixelFormat
                int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);

                // Get pointer to first line
                byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

                // look at every pixel in the blur rectangle
                for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
                {
                    for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                    {
                        int avgR = 0, avgG = 0, avgB = 0;
                        int blurPixelCount = 0;

                        // average the color of the red, green and blue for each pixel in the
                        // blur size while making sure you don't go outside the image bounds
                        for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                        {
                            for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                            {
                                // Get pointer to RGB
                                byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                                avgB += data[0]; // Blue
                                avgG += data[1]; // Green
                                avgR += data[2]; // Red

                                blurPixelCount++;
                            }
                        }

                        avgR = avgR / blurPixelCount;
                        avgG = avgG / blurPixelCount;
                        avgB = avgB / blurPixelCount;

                        // now that we know the average for the blur size, set each pixel to that color
                        for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                        {
                            for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                            {
                                // Get pointer to RGB
                                byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                                // Change values
                                data[0] = (byte)avgB;
                                data[1] = (byte)avgG;
                                data[2] = (byte)avgR;
                            }
                        }
                    }
                }

                // Unlock the bits
                blurred.UnlockBits(blurredData);

                return blurred;
            }

            private void HandleResize(object sender, EventArgs e)
            {
                Location = new Point(0, 0);

                //Form form = findForm(this);
                Control form = this.Parent;

                if (null != form) Size = form.ClientSize;

                if (null != m_oProgressBar)
                {
                    m_oProgressBar.Left = 0;
                    m_oProgressBar.Top = this.Height - m_oProgressBar.Height;
                    m_oProgressBar.Width = this.Width;
                }
            }

            #region "Offsets"

            private int H_OFFSET
            {
                get
                {
                    try
                    {
                        //Form frm = findForm(this);
                        Control frm = this.Parent;

                        Rectangle screenRectangle = frm.RectangleToScreen(frm.ClientRectangle);

                        int i = frm.Height - screenRectangle.Height;
                        return i;
                    }
                    catch { }
                    return 0;
                }
            }

            private int W_OFFSET
            {
                get
                {
                    try
                    {
                        //Form frm = findForm(this);
                        Control frm = this.Parent;

                        Rectangle screenRectangle = frm.RectangleToScreen(frm.ClientRectangle);

                        int i = frm.Width - screenRectangle.Width;
                        return i;
                    }
                    catch { }
                    return 0;
                }
            }

            private int X_OFFSET
            {
                get
                {
                    try
                    {
                        //Form frm = findForm(this);
                        Control frm = this.Parent;

                        Rectangle screenRectangle = frm.RectangleToScreen(frm.ClientRectangle);

                        return screenRectangle.Left - frm.Left;
                    }
                    catch { }
                    return 0;
                }
            }

            private int Y_OFFSET
            {
                get
                {
                    try
                    {
                        //Form frm = findForm(this);
                        Control frm = this.Parent;

                        Rectangle screenRectangle = frm.RectangleToScreen(frm.ClientRectangle);

                        return screenRectangle.Top - frm.Top;
                    }
                    catch { }
                    return 0;
                }
            }

            #endregion "Offsets"
        }
    }
}