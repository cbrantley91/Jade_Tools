// Initial workddone for XNA by Nick Gravelyn
// More information : (http://blogs.msdn.com/b/nicgrave/archive/2010/07/25/rendering-with-xna-framework-4-0-inside-of-a-wpf-application.aspx)

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jade.MapEditor_Windows
{
    // TODO : move to different library

    /// <summary>
    /// A wrapper for a RenderTarget2D and WriteableBitmap 
    /// that handles taking the XNA rendering and moving it 
    /// into the WriteableBitmap which is consumed as the
    /// ImageSource for an Image control.
    /// </summary>
    public class MonoGameImageSource : IDisposable
    {
        // a buffer array that gets the data from the render target
        private byte[] buffer;

        /// <summary>
        /// Gets the render target used for this image source.
        /// </summary>
        public RenderTarget2D RenderTarget
        { get; private set; }

        /// <summary>
        /// Gets the underlying WriteableBitmap that can 
        /// be bound as an ImageSource.
        /// </summary>
        public WriteableBitmap WriteableBitmap
        { get; private set; }

        /// <summary>
        /// Creates a new XnaImageSource.
        /// </summary>
        /// <param name="graphics">The GraphicsDevice to use.</param>
        /// <param name="width">The width of the image source.</param>
        /// <param name="height">The height of the image source.</param>
        public MonoGameImageSource(GraphicsDevice graphics, int width, int height)
        {
            // create the render target and buffer to hold the data
            RenderTarget = new RenderTarget2D(graphics, width, height, false,
                SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            buffer = new byte[width * height * 4];
            WriteableBitmap = new WriteableBitmap(width, height, 96, 96,
                PixelFormats.Bgra32, null);
        }

        ~MonoGameImageSource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            var renderTarget = RenderTarget;
            if (renderTarget != null) renderTarget.Dispose();

            if (disposing)
                GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Commits the render target data into our underlying bitmap source.
        /// </summary>
        public void Commit()
        {
            // get the data from the render target
            var renderTarget = RenderTarget;
            var writeableBitmap = WriteableBitmap;

            // TODO : do some logging
            if (renderTarget == null || writeableBitmap == null)
                return;

            renderTarget.GetData(buffer);

            // because the only 32 bit pixel format for WPF is 
            // BGRA but XNA is all RGBA, we have to swap the R 
            // and B bytes for each pixel
            for (int i = 0; i < buffer.Length - 2; i += 4)
            {
                byte r = buffer[i];
                buffer[i] = buffer[i + 2];
                buffer[i + 2] = r;
            }

            // write our pixels into the bitmap source
            writeableBitmap.Lock();
            Marshal.Copy(buffer, 0, writeableBitmap.BackBuffer, buffer.Length);
            writeableBitmap.AddDirtyRect(
                new Int32Rect(0, 0, renderTarget.Width, renderTarget.Height));
            writeableBitmap.Unlock();
        }
    }
}
