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
    public unsafe partial class Display : UserControl
    {
        private Bitmap _buffer;
        private Graphics _graphics;
 
        public Display()
        {            
            _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppArgb);
            _graphics = Graphics.FromImage(_buffer);

            InitializeComponent();
            
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        public void Perform(float* displayInputBuffer, int length)
        {
            if (displayInputBuffer == null) return;

            var graphics = _graphics;
            var graphicsRect = ClientRectangle;

            var xCenter = (graphicsRect.Width * 0.5f);
            var yCenter = (graphicsRect.Height * 0.5f);

            var gainY = (float)(yCenter / Math.PI);
            var gainX = (float)graphicsRect.Width / length;

            var showLength = graphicsRect.Width;
            graphics.Clear(Color.Black);

            using (var spectrumPen = new Pen(Color.Green, 1.0f))
            using (var linePen = new Pen(Color.LightGreen, 1.0f))
            using (var gridPen = new Pen(Color.Gray))
            using (var textFont = new Font("Arial", 8f))
            using (var textBrush = new SolidBrush(Color.White))
            {
                var eyeLength = 1;

                var newX = 0.0f;
                var newY = (float)yCenter;

                var gridPi2 = (int)(Math.PI * 0.5f * gainY);
                graphics.DrawLine(gridPen, 0, yCenter + gridPi2, graphicsRect.Width, yCenter + gridPi2);
                //graphics.DrawString("Pi/2", textFont, textBrush, 0, yCenter - gridPi2);
                graphics.DrawLine(gridPen, 0, yCenter - gridPi2, graphicsRect.Width, yCenter - gridPi2);
                //graphics.DrawString("-Pi/2", textFont, textBrush, 0, yCenter + gridPi2);
                graphics.DrawLine(gridPen, 0, yCenter, graphicsRect.Width, yCenter);
                //graphics.DrawString("0", textFont, textBrush, 0, yCenter);

                for (int i = 0; i < length; i++)
                {
                    newY = (float)(yCenter - (displayInputBuffer[i] * gainY));
                    newX = i * gainX;

                    if (newY > graphicsRect.Height) newY = graphicsRect.Height;
                    else if (newY < 0) newY = 0;
                    if (float.IsNaN(newY)) newY = 0;

                    if (newX > graphicsRect.Width) newX = graphicsRect.Width;
                    else if (newX < 0) newX = 0;

                    graphics.DrawLine(linePen, newX, newY, newX + eyeLength, newY);
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
            ConfigureGraphics(e.Graphics);
            e.Graphics.DrawImageUnscaled(_buffer, 0, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                _buffer.Dispose();
                _graphics.Dispose();
                _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppArgb);
                _graphics = Graphics.FromImage(_buffer);
            }
        }
    }
}
