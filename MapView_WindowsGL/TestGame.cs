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

            device.Clear(Color.Black);

            StupidDrawLine(device, start, end);

            // TODO : Determine what logging framework should be used for MonoGame
            System.Console.WriteLine("Update : " + gameTime.ElapsedGameTime.ToString() + ", " + gameTime.TotalGameTime.ToString());

            // Change base?
            base.Draw(gameTime);
        }

        private VertexPositionColor[] PlotGridLines(Vector2 topLeft, Vector2 bottomLeft, Vector2 topRight, int numHorizontalDivisors, int numVerticalDivisors)
        {
            return PlotGridLines(topLeft, bottomLeft, topRight, numHorizontalDivisors, numVerticalDivisors, Color.White, Color.White);
        }

        private VertexPositionColor[] PlotGridLines(Vector2 topLeft, Vector2 bottomLeft, Vector2 topRight, int numHorizontalDivisors, int numVerticalDivisors,
            Color horizontalColor, Color verticalColor)
        {
            int minimumNumberOfLines = 4, lineNdx = 0;
            int numberOfLines = minimumNumberOfLines + numHorizontalDivisors + numVerticalDivisors;
            int numberOfVertices = numberOfLines * 2;
            int numberOfVerticalLines = minimumNumberOfLines / 2 + numVerticalDivisors;
            int numberOfHorizontalLines = minimumNumberOfLines / 2 + numHorizontalDivisors;

            VertexPositionColor[] vertexList = new VertexPositionColor[numberOfVertices];

            float deltaXVert = (topRight.X - topLeft.X) / ((numVerticalDivisors + 1) * 1.0f);
            //float deltaYVert = (topRight.Y - topLeft.Y) / ((numberOfVerticalLines + 1) * 1.0f);

            //float deltaXHoriz = (bottomLeft.X - topLeft.Y) / ((numberOfHorizontalLines + 1) * 1.0f);
            float deltaYHoriz = (bottomLeft.Y - topLeft.Y) / ((numHorizontalDivisors + 1) * 1.0f);

            for (int vertNdx = 0; vertNdx < numberOfVerticalLines; vertNdx++)
            {
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(topLeft.X + deltaXVert * vertNdx, topLeft.Y, 0), verticalColor);
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(bottomLeft.X + deltaXVert * vertNdx, bottomLeft.Y, 0), verticalColor);
            }

            for (int horizNdx = 0; horizNdx < numberOfHorizontalLines; horizNdx++)
            {
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(topLeft.X, topLeft.Y + deltaYHoriz * horizNdx, 0), horizontalColor);
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(topRight.X, topRight.Y + deltaYHoriz * horizNdx, 0), horizontalColor);
            }

            return vertexList;
        }

        // TODO : move into a draw utility class
        private VertexPositionColor[] PlotGridLines(float leftX, float topY, float rightX, float bottomY, int numHorizontalDivisors, int numVerticalDivisors)
        {
            int minimumNumberOfLines = 4, lineNdx = 0;
            int numberOfLines = minimumNumberOfLines + numHorizontalDivisors + numVerticalDivisors;
            int numberOfVertices = numberOfLines * 2;
            int numberOfVerticalLines = minimumNumberOfLines / 2 + numVerticalDivisors;
            int numberOfHorizontalLines = minimumNumberOfLines / 2 + numHorizontalDivisors;

            VertexPositionColor[] vertexList = new VertexPositionColor[numberOfVertices];

            float deltaX = (rightX - leftX) / ((numVerticalDivisors + 1) * 1.0f);
            for (int vertNdx = 0; vertNdx < numberOfVerticalLines; vertNdx++)
            {
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(leftX + deltaX * vertNdx, topY, 0), Color.White);
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(leftX + deltaX * vertNdx, bottomY, 0), Color.White);
            }

            float deltaY = (bottomY - topY) / ((numHorizontalDivisors + 1) * 1.0f);
            for (int horizNdx = 0; horizNdx < numberOfHorizontalLines; horizNdx++)
            {
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(leftX, topY + deltaY * horizNdx, 0), Color.White);
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(rightX, topY + deltaY * horizNdx, 0), Color.White);
            }

            return vertexList;
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
                DrawGridLines(device);
            }
                
        }

        VertexPositionColor[] pointList, trianglePointList, gridLines;
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
                new VertexPositionColor(new Vector3(0, 0, 0), Color.White),
                new VertexPositionColor(new Vector3(400, 400, 0), Color.Red),
                new VertexPositionColor(new Vector3(400, 400, 0), Color.Red),
                new VertexPositionColor(new Vector3(400, 0, 0), Color.Beige),
                new VertexPositionColor(new Vector3(20, 5, 0), Color.Aquamarine),
                new VertexPositionColor(new Vector3(15, -5, 0), Color.Cyan),
                new VertexPositionColor(new Vector3(20, 10, 0), Color.Cornsilk),
                new VertexPositionColor(new Vector3(-10, -20, 0), Color.CornflowerBlue)
            };

            trianglePointList = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(0, 0, 0), Color.White),
                new VertexPositionColor(new Vector3(200, 0, 0), Color.White),
                new VertexPositionColor(new Vector3(200, 200, 0), Color.White),
                new VertexPositionColor(new Vector3(0, 200, 0), Color.White),
                new VertexPositionColor(new Vector3(0, 0, 0), Color.White),
            };

            gridLines = PlotGridLines(new Vector2(0, 0), new Vector2(0, 400), new Vector2(400, 0), 4, 4, Color.Red, Color.Purple);

            // Initialize the vertex buffer, allocating memory for each vertex.
            vertexBuffer = new VertexBuffer(GraphicsDevice, vertexDeclaration,
                points, BufferUsage.None);

            // Set the vertex buffer data to the array of vertices.
            vertexBuffer.SetData<VertexPositionColor>(pointList);
        }

        /// <summary>
        /// Draws the triangle list.
        /// </summary>
        private void DrawLines(GraphicsDevice device)
        {
            device.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.LineList,
                pointList,
                0,   // vertex buffer offset to add to each element of the index buffer
                4    // number of vertices to draw
                );
        }

        private void DrawGridLines(GraphicsDevice device)
        {
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, gridLines, 0, gridLines.Length / 2);
        }

        private void DrawTriangleStrip(GraphicsDevice device)
        {
            device.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                trianglePointList,
                0, 3
            );
        }

    }
}
