namespace Exemplos.FaceApi
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    public static class ImageManipulation
    {
        // Converte um array de bytes em uma instância de Bitmap
        public static Bitmap ToBitmap(this byte[] byteArray)
        {
            return (Bitmap)Image.FromStream(new MemoryStream(byteArray));
        }

        // Converte uma instância de Bitmap em um array de bytes em formato PNG
        public static byte[] ToPngByteArray(this Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }            
        }

        // Recorta uma imagem por sua menor dimensão (largura ou altura), à partir de um retângulo de referência
        public static Bitmap CropRectangleToSmallerSide(this Bitmap original, int cropRectTop, int cropRectLeft, int cropRectWidth, int cropRectHeight)
        {
            var cropRect = new Rectangle(cropRectLeft, cropRectTop, cropRectWidth, cropRectHeight);
        
            var minDimension = Math.Min(original.Width, original.Height);

            var newRect = new Rectangle(
                minDimension == original.Width ? 0 : Math.Max(0, cropRect.Left - ((minDimension - cropRect.Width) / 2)),
                minDimension == original.Height ? 0 : Math.Max(0, cropRect.Top - ((minDimension - cropRect.Height) / 2)), 
                minDimension, 
                minDimension);
            
            return original.Clone(newRect, original.PixelFormat);
        }

        // Redimensiona uma instância de Bitmap
        public static Bitmap Resize(this Bitmap original, int width, int height)
        {
            var resized = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(original, 0, 0, width, height);
                using (MemoryStream output = new MemoryStream())
                {
                    var qualityParamId = Encoder.Quality;
                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(qualityParamId, 75);
                    var codec = ImageCodecInfo.GetImageDecoders()
                                              .FirstOrDefault(c => c.FormatID == ImageFormat.Png.Guid);
                    resized.Save(output, codec, encoderParameters);
                    output.Flush();
                    return (Bitmap)Image.FromStream(output);
                }
            }                    
        }
    }
}