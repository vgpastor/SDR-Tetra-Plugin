// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.Display
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SDRSharp.Tetra
{
  [DesignTimeVisible(true)]
  [Category("SDRSharp")]
  [Description("Display Panel")]
  public class Display : UserControl
  {
    private Bitmap _buffer;
    private Graphics _graphics;
    private IContainer components;

    public Display()
    {
      Rectangle clientRectangle = this.ClientRectangle;
      int width = clientRectangle.Width;
      clientRectangle = this.ClientRectangle;
      int height = clientRectangle.Height;
      this._buffer = new Bitmap(width, height, PixelFormat.Format32bppArgb);
      this._graphics = Graphics.FromImage((Image) this._buffer);
      this.InitializeComponent();
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.DoubleBuffer, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.UpdateStyles();
    }

    public unsafe void Perform(float* displayInputBuffer, int length)
    {
      if ((IntPtr) displayInputBuffer == IntPtr.Zero)
        return;
      Graphics graphics = this._graphics;
      Rectangle clientRectangle = this.ClientRectangle;
      int width1 = clientRectangle.Width;
      float num1 = (float) clientRectangle.Height * 0.5f;
      float num2 = num1 / 3.141593f;
      float num3 = (float) clientRectangle.Width / (float) length;
      int width2 = clientRectangle.Width;
      graphics.Clear(Color.Black);
      using (new Pen(Color.Green, 1f))
      {
        using (Pen pen1 = new Pen(Color.LightGreen, 1f))
        {
          using (Pen pen2 = new Pen(Color.Gray))
          {
            using (new Font("Arial", 8f))
            {
              using (new SolidBrush(Color.White))
              {
                int num4 = 1;
                int num5 = (int) (Math.PI / 2.0 * (double) num2);
                graphics.DrawLine(pen2, 0.0f, num1 + (float) num5, (float) clientRectangle.Width, num1 + (float) num5);
                graphics.DrawLine(pen2, 0.0f, num1 - (float) num5, (float) clientRectangle.Width, num1 - (float) num5);
                graphics.DrawLine(pen2, 0.0f, num1, (float) clientRectangle.Width, num1);
                for (int index = 0; index < length; ++index)
                {
                  float num6 = num1 - displayInputBuffer[index] * num2;
                  float x1 = (float) index * num3;
                  if ((double) num6 > (double) clientRectangle.Height)
                    num6 = (float) clientRectangle.Height;
                  else if ((double) num6 < 0.0)
                    num6 = 0.0f;
                  if ((double) x1 > (double) clientRectangle.Width)
                    x1 = (float) clientRectangle.Width;
                  else if ((double) x1 < 0.0)
                    x1 = 0.0f;
                  graphics.DrawLine(pen1, x1, num6, x1 + (float) num4, num6);
                }
              }
            }
          }
        }
      }
    }

    public static void ConfigureGraphics(Graphics graphics)
    {
      graphics.CompositingMode = CompositingMode.SourceCopy;
      graphics.CompositingQuality = CompositingQuality.HighSpeed;
      graphics.SmoothingMode = SmoothingMode.None;
      graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
      graphics.InterpolationMode = InterpolationMode.Low;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Display.ConfigureGraphics(e.Graphics);
      e.Graphics.DrawImageUnscaled((Image) this._buffer, 0, 0);
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      if (this.ClientRectangle.Width <= 0 || this.ClientRectangle.Height <= 0)
        return;
      this._buffer.Dispose();
      this._graphics.Dispose();
      this._buffer = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height, PixelFormat.Format32bppArgb);
      this._graphics = Graphics.FromImage((Image) this._buffer);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackgroundImageLayout = ImageLayout.Zoom;
      this.DoubleBuffered = true;
      this.Name = "Display";
      this.ResumeLayout(false);
    }
  }
}
