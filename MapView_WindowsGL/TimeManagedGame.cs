using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Jade.MapView_WindowsGL {
    /// <summary>
    /// Required for tool integration, since WPF doesn't provide time management the same way MonoGame does.
    /// </summary>
    public class TimeManagedGame : TestGame {
        private Stopwatch stopwatch = new Stopwatch();
        private long lastMillis;

        /// <summary>
        /// Allows to application to update scenery/visual effects with each draw call.
        /// </summary>
        public void Start() {
            lastMillis = stopwatch.ElapsedMilliseconds;
            stopwatch.Start();
        }

        /// <summary>
        /// Retrieves the elapsed time and calculates the delta time since the last call.
        /// </summary>
        /// <returns>A gametime object to use for update logic</returns>
        private GameTime CalculateGameTime() {
            stopwatch.Stop();
            long currentTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Start();

            GameTime gameTime = new GameTime(TimeSpan.FromMilliseconds(currentTime),
                TimeSpan.FromMilliseconds(currentTime - lastMillis));
            lastMillis = currentTime;

            return gameTime;
        }

        /// <summary>
        /// Updates the scene and draws to the graphics device provided.
        /// </summary>
        /// <param name="device">The graphics device to draw to.</param>
        public void Draw(GraphicsDevice device) {
            Draw(CalculateGameTime(), device);
        }
    }
}
