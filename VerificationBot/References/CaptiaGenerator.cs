using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.IO;

namespace FencingtrackerBot.References
{
    public class Captcha
    {
        // Constant class values
        private const int ImageHeight = 100;
        private const int ImageWidth = 210;

        // Readonly class objects
        private readonly Color TextColor = Color.Black;

        private readonly int[] FontSizes = new int[]
        {
            15, 20, 25, 30, 35
        };

        private readonly string[] FontNames = new string[] 
        {
            "Comic Sans MS",
            "Arial",
            "Times New Roman",
            "Georgia",
            "Verdana",
            "Geneva"
        };

        private readonly FontStyle[] FontStyles = new FontStyle[] 
        {
             FontStyle.Bold,
             FontStyle.Italic,
             FontStyle.Regular,
             FontStyle.Strikeout,
             FontStyle.Underline
        };

        private readonly HatchStyle[] HatchStyles = new HatchStyle[]
        {
            HatchStyle.BackwardDiagonal, HatchStyle.Cross,
            HatchStyle.DashedDownwardDiagonal, HatchStyle.DashedHorizontal,
            HatchStyle.DashedUpwardDiagonal, HatchStyle.DashedVertical,
            HatchStyle.DiagonalBrick, HatchStyle.DiagonalCross,
            HatchStyle.Divot, HatchStyle.DottedDiamond, HatchStyle.DottedGrid,
            HatchStyle.ForwardDiagonal, HatchStyle.Horizontal,
            HatchStyle.HorizontalBrick, HatchStyle.LargeCheckerBoard,
            HatchStyle.LargeConfetti, HatchStyle.LargeGrid,
            HatchStyle.LightDownwardDiagonal, HatchStyle.LightHorizontal,
            HatchStyle.LightUpwardDiagonal, HatchStyle.LightVertical,
            HatchStyle.Max, HatchStyle.Min, HatchStyle.NarrowHorizontal,
            HatchStyle.NarrowVertical, HatchStyle.OutlinedDiamond,
            HatchStyle.Plaid, HatchStyle.Shingle, HatchStyle.SmallCheckerBoard,
            HatchStyle.SmallConfetti, HatchStyle.SmallGrid,
            HatchStyle.SolidDiamond, HatchStyle.Sphere, HatchStyle.Trellis,
            HatchStyle.Vertical, HatchStyle.Wave, HatchStyle.Weave,
            HatchStyle.WideDownwardDiagonal, HatchStyle.WideUpwardDiagonal, HatchStyle.ZigZag
        };

        private Random Random;

        public Captcha()
        {
            Random = new Random();
        }

        private string GetCaptchaText()
        {
            return Random.Next(100000, 999999).ToString();
        }

        private Color GenerateRandom(int From, int To)
        {
            return Color.FromArgb(Random.Next(From, To), Random.Next(From, To), Random.Next(From, To));
        }

        private Bitmap GetBitMap()
        {
            Bitmap BitMap = new Bitmap(ImageWidth, ImageHeight, PixelFormat.Format32bppArgb);

            Graphics Graphics = Graphics.FromImage(BitMap);
            Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            // Create canvas in the middle of the image space
            RectangleF Canvas = new RectangleF(0, 0, ImageWidth, ImageHeight);
            Brush CanvasColor = new HatchBrush(HatchStyles[Random.Next(HatchStyles.Length-1)], GenerateRandom(100, 255), Color.LightGray);

            Graphics.FillRectangle(CanvasColor, Canvas);

            Matrix Matrix = new Matrix();

            for (int i = 0; i < CaptchaName.Length; i++)
            {
                Matrix.Reset();

                int XPos = ImageWidth / (CaptchaName.Length) * i;
                int YPos = ImageHeight / 3;

                Matrix.RotateAt(Random.Next(-40, 40), new PointF(XPos, YPos));
                Graphics.Transform = Matrix;

                Graphics.DrawString(CaptchaName.Substring(i, 1), new Font(FontNames[Random.Next(FontNames.Length - 1)], FontSizes[Random.Next(FontSizes.Length - 1)], FontStyles[Random.Next(FontStyles.Length-1)]), new SolidBrush(GenerateRandom(0, 100)), new PointF(XPos, YPos));
                Graphics.ResetTransform();
            }

            return BitMap;
        }
        public string CaptchaName { get; private set; }
        public string GenerateCaptcha()
        {
            string FileName = Path.GetRandomFileName().Replace(".", "");
            CaptchaName = GetCaptchaText();
            GetBitMap().Save($"./{FileName}.jpg", ImageFormat.Jpeg);

            return FileName;
        }
    }
}
