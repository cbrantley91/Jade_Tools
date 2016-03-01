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
                LightingEnabled = false,
                Projection = Matrix.CreateOrthographicOffCenter
                    (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
                    graphics.GraphicsDevice.Viewport.Height, 0,    // bottom, top
                    0, 1)
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
            device.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame
        };

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
            pointList = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(100, 100, 0), Color.White),
                new VertexPositionColor(new Vector3(200, 200, 0), Color.Red),
                new VertexPositionColor(new Vector3(10, 5, 0), Color.Red),
                new VertexPositionColor(new Vector3(5, -5, 0), Color.Beige),
                new VertexPositionColor(new Vector3(20, 5, 0), Color.Aquamarine),
                new VertexPositionColor(new Vector3(15, -5, 0), Color.Cyan),
                new VertexPositionColor(new Vector3(20, 10, 0), Color.Cornsilk),
                new VertexPositionColor(new Vector3(-10, -20, 0), Color.CornflowerBlue)
            };

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
            device.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.LineList,
                pointList,
                0,   // vertex buffer offset to add to each element of the index buffer
                4    // number of vertices to draw
                );
        }

    }
}
