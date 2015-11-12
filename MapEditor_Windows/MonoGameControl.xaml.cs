using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jade.MapView_WindowsGL;

namespace Jade.MapEditor_Windows
{
    /// <summary>
    /// Interaction logic for MonoGameControl.xaml
    /// </summary>
    public partial class MonoGameControl : UserControl
    {
        private GraphicsDeviceService graphicsService;
        private MonoGameImageSource imageSource;

        // TODO : remove game dependency; have DrawFunction be linked to external game
        private TimeManagedGame game;

        /// <summary>
        /// Gets the GraphicsDevice behind the control.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsService.GraphicsDevice; }
        }

        /// <summary>
        /// Invoked when the XnaControl needs to be redrawn.
        /// </summary>
        public Action<GraphicsDevice> DrawFunction;

        public MonoGameControl()
        {
            InitializeComponent();

            game = new TimeManagedGame();

            // hook up an event to fire when the control has finished loading
            Loaded += new RoutedEventHandler(MonoGameControl_Loaded);
            DrawFunction += game.Draw;
        }

        ~MonoGameControl()
        {
            imageSource.Dispose();

            // release on finalizer to clean up the graphics device
            if (graphicsService != null)
                graphicsService.Release();
        }

        void MonoGameControl_Loaded(object sender, RoutedEventArgs e)
        {
            // if we're not in design mode, initialize the graphics device
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                InitializeGraphicsDevice();
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            // if we're not in design mode, recreate the 
            // image source for the new size
            if (DesignerProperties.GetIsInDesignMode(this) == false &&
                graphicsService != null)
            {
                // recreate the image source
                imageSource.Dispose();
                imageSource = new MonoGameImageSource(
                    GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
                rootImage.Source = imageSource.WriteableBitmap;
            }

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void InitializeGraphicsDevice()
        {
            if (graphicsService == null)
            {
                // add a reference to the graphics device
                graphicsService = GraphicsDeviceService.AddRef(
                    (PresentationSource.FromVisual(this) as HwndSource).Handle);

                // create the image source
                imageSource = new MonoGameImageSource(
                    GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
                rootImage.Source = imageSource.WriteableBitmap;

                // hook the rendering event
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        /// <summary>
        /// Draws the control and allows subclasses to override 
        /// the default behavior of delegating the rendering.
        /// </summary>
        protected virtual void Render()
        {
            // invoke the draw delegate so someone will draw something pretty
            if (DrawFunction != null)
                DrawFunction(GraphicsDevice);
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // set the image source render target
            GraphicsDevice.SetRenderTarget(imageSource.RenderTarget);

            // allow the control to draw
            Render();

            // unset the render target
            GraphicsDevice.SetRenderTarget(null);

            // commit the changes to the image source
            imageSource.Commit();
        }
    }
}
