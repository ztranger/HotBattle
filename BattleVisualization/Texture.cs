using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace BattleVisualization
{
    public class Texture : IDisposable
    {
        public int GlHandle { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Texture(Bitmap bitmap)
        {
            GlHandle = GL.GenTexture();
            Bind();

            Width = bitmap.Width;
            Height = bitmap.Height;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmapData.Width, bitmapData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, GlHandle);
        }

        #region Disposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    GL.DeleteTexture(GlHandle);
                }
                _disposed = true;
            }
        }

        ~Texture()
        {
            Dispose(false);
        }

        #endregion
    }
}