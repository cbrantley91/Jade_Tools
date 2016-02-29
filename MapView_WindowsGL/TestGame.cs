using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jade.MapView_WindowsGL
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TestGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect basicEffect;

        public TestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadShaders();

            InitializePoints();
            InitializeTriangleList();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void LoadShaders()
        {
            basicEffect = new BasicEffect(GraphicsDevice)
            {
                Alpha = 1f,
                VertexColorEnabled = true,
                LightingEnabled = false
            };
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
             || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Draw(gameTime, GraphicsDevice);
        }

        // TODO : build as an extension
        protected void Draw(GameTime gameTime, GraphicsDevice device)
        {
            Vector2 start = new Vector2(10, 10);
            Vector2 end = new Vector2(100, 100);

            device.Clear(Color.BlueViolet);

            StupidDrawLine(device, start, end);

            // TODO : Determine what logging framework should be used for MonoGame
            System.Console.WriteLine("Update : " + gameTime.ElapsedGameTime.ToString() + ", " + gameTime.TotalGameTime.ToString());

            // Change base?
            base.Draw(gameTime);
        }

        void StupidDrawLine(GraphicsDevice device, Vector2 start, Vector2 end)
        {
            /*int points = 2;

             VertexPositionColor[] primitiveList = new VertexPositionColor[points];

            primitiveList[0] = new VertexPositionColor(new Vector3(start, 1), Color.White);
            primitiveList[1] = new VertexPositionColor(new Vector3(end, 1), Color.White);*/

            /*            Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
                        Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(
                            0,
                            (float) device.Viewport.Width,
                            (float) device.Viewport.Height,
                            0,
                            1.0f, 1000.0f);
                            */
            /* short[] lineListIndices = new short[(points * 2) - 2];

            for (int pt_ndx = 0; pt_ndx < points - 1; pt_ndx++)
            {
                lineListIndices[pt_ndx * 2] = (short)(pt_ndx);
                lineListIndices[(pt_ndx * 2) + 1] = (short)(pt_ndx + 1);
            } */

            device.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame
        };

            /*int points = 1;
            int width = 10, height = 10;
            var triangleListIndices = new short[18] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5, 4, 5, 6, 6, 5, 7 };

            var primitiveList = new VertexPositionColor[points];

            for (int x = 0; x < points / 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    primitiveList[(x * 2) + y] = new VertexPositionColor(
                        new Vector3(x * 100, y * 100, 0), Color.White);
                }
            }
            */

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) { 
                pass.Apply();
                DrawTriangleList(device);
            }
                
        }

        short[] triangleListIndices;
        VertexPositionColor[] pointList;
        int points = 8;
        VertexDeclaration vertexDeclaration = new VertexDeclaration(new VertexElement[]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
                });

        VertexBuffer vertexBuffer;

        /// <summary>
        /// Initializes the point list.
        /// </summary>
        private void InitializePoints()
        {
            pointList = new VertexPositionColor[points];

            for (int x = 0; x < points / 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    pointList[(x * 2) + y] = new VertexPositionColor(
                        new Vector3(x * 100, y * 100, 0), Color.White);
                }
            }

            // Initialize the vertex buffer, allocating memory for each vertex.
            vertexBuffer = new VertexBuffer(GraphicsDevice, vertexDeclaration,
                points, BufferUsage.None);

            // Set the vertex buffer data to the array of vertices.
            vertexBuffer.SetData<VertexPositionColor>(pointList);
        }

        /// <summary>
        /// Initializes the triangle list.
        /// </summary>
        private void InitializeTriangleList()
        {
            int width = 4;
            int height = 2;

            triangleListIndices = new short[(width - 1) * (height - 1) * 6];

            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    triangleListIndices[(x + y * (width - 1)) * 6] = (short)(2 * x);
                    triangleListIndices[(x + y * (width - 1)) * 6 + 1] = (short)(2 * x + 1);
                    triangleListIndices[(x + y * (width - 1)) * 6 + 2] = (short)(2 * x + 2);

                    triangleListIndices[(x + y * (width - 1)) * 6 + 3] = (short)(2 * x + 2);
                    triangleListIndices[(x + y * (width - 1)) * 6 + 4] = (short)(2 * x + 1);
                    triangleListIndices[(x + y * (width - 1)) * 6 + 5] = (short)(2 * x + 3);
                }
            }
        }

        /// <summary>
        /// Draws the triangle list.
        /// </summary>
        private void DrawTriangleList(GraphicsDevice device)
        {
            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleList,
                pointList,
                0,   // vertex buffer offset to add to each element of the index buffer
                8,   // number of vertices to draw
                triangleListIndices,
                0,   // first index element to read
                6    // number of primitives to draw
            );
        }

    }
}
