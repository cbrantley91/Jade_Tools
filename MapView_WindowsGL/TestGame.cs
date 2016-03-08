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
        #region Fields

        // TODO : cleanup unnecessary
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect basicEffect;
        VertexPositionColor[] pointList, trianglePointList, gridLines;
        VertexBuffer vertexBuffer;
        MouseState mouseStateCurrent, mouseStatePrevious;
        int points = 8;

        #endregion Fields

        #region Lifecycle
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
            this.IsMouseVisible = true;

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

        #endregion Lifecycle

        #region Updates

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

            mouseStateCurrent = Mouse.GetState();

            // TODO : do mouse checking here

            mouseStatePrevious = mouseStateCurrent;

            // TODO: Add update logic here

            base.Update(gameTime);
        }

        #endregion Updates

        #region Drawing

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

        private void DrawGridLines(GraphicsDevice device)
        {
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, gridLines, 0, gridLines.Length / 2);
        }

        // TODO - cbrantley91 : remove
        #region Test Draw Code

        void StupidDrawLine(GraphicsDevice device, Vector2 start, Vector2 end)
        {
            device.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame
            };

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                DrawGridLines(device);
            }
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

        private void DrawTriangleStrip(GraphicsDevice device)
        {
            device.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                trianglePointList,
                0, 3
            );
        }

        #endregion

        #endregion Drawing

        #region Plotting

        #region Grid
        private const int minimumNumberOfLinesForGrid = 4;

        /// <summary>
        /// Builds a line list representing a grid.  This function uses only 3 points to specify a grid, since it
        /// assumes all lines to be parallel, and so dimensions are fixed.
        /// </summary>
        /// <param name="topLeft">The top-left point of the grid</param>
        /// <param name="bottomLeft">The bottom-left point of the grid</param>
        /// <param name="topRight">The top-right point of the grid</param>
        /// <param name="numHorizontalDivisors">The number of dividing horizontal lines; this will result in (numHorizontalDivisors + 1) horizontal rows</param>
        /// <param name="numVerticalDivisors">The number of dividing vertical lines; this will result in (numVerticalDivisors + 1) vertical columns</param>
        /// <param name="horizontalColor">Color for horizontal lines</param>
        /// <param name="verticalColor">Color for vertical lines</param>
        /// <returns></returns>
        protected VertexPositionColor[] PlotGridLines(Vector2 topLeft, Vector2 bottomLeft, Vector2 topRight,
            int numHorizontalDivisors, int numVerticalDivisors)
        {
            return PlotGridLines(topLeft, bottomLeft, topRight, numHorizontalDivisors, numVerticalDivisors, Color.White, Color.White);
        }

        /// <summary>
        /// Builds a line list representing a grid.  This function uses only 3 points to specify a grid, since it
        /// assumes all lines to be parallel, and so dimensions are fixed.
        /// </summary>
        /// <param name="topLeft">The top-left point of the grid</param>
        /// <param name="bottomLeft">The bottom-left point of the grid</param>
        /// <param name="topRight">The top-right point of the grid</param>
        /// <param name="numHorizontalDivisors">The number of dividing horizontal lines; this will result in (numHorizontalDivisors + 1) horizontal rows</param>
        /// <param name="numVerticalDivisors">The number of dividing vertical lines; this will result in (numVerticalDivisors + 1) vertical columns</param>
        /// <param name="horizontalColor">Color for horizontal lines</param>
        /// <param name="verticalColor">Color for vertical lines</param>
        /// <returns></returns>
        protected VertexPositionColor[] PlotGridLines(Vector2 topLeft, Vector2 bottomLeft, Vector2 topRight,
            int numHorizontalDivisors, int numVerticalDivisors, Color horizontalColor, Color verticalColor)
        {
            int numberOfLines = minimumNumberOfLinesForGrid + numHorizontalDivisors + numVerticalDivisors;
            int numberOfVertices = numberOfLines * 2, lineNdx = 0;
            int numberOfVerticalLines = minimumNumberOfLinesForGrid / 2 + numVerticalDivisors;
            int numberOfHorizontalLines = minimumNumberOfLinesForGrid / 2 + numHorizontalDivisors;

            VertexPositionColor[] vertexList = new VertexPositionColor[numberOfVertices];

            // deltaXVert is the space between the parallel vertical lines
            // i.e., 2 dividing lines means the grid has been divided into 3 vertical columns, of deltaXVert width
            float deltaXVert = (topRight.X - topLeft.X) / ((numVerticalDivisors + 1) * 1.0f);

            // deltaYVert is the Y offset for the top of the vertical line, so a slant can be visible
            // i.e., the first vertical line should stretch from (0,0) to (0,100), but the second should be (X,10) to (X,110)
            float deltaYVert = (topRight.Y - topLeft.Y) / ((numVerticalDivisors + 1) * 1.0f);

            // deltaXHoriz is similar to deltaYVert : specifies the horizontal offset, so the lines can slant
            // i.e., the first horizontal line should stretch from (0,0) to (100,0), but the second should be (10,Y) to (110,Y)
            float deltaXHoriz = (bottomLeft.X - topLeft.X) / ((numHorizontalDivisors + 1) * 1.0f);

            // deltaYHoriz is the space between parallel horizontal lines
            // i.e., 2 dividing lines means the grid has been divided into 3 horizontal rows, of deltaYHoriz width
            float deltaYHoriz = (bottomLeft.Y - topLeft.Y) / ((numHorizontalDivisors + 1) * 1.0f);

            for (int vertNdx = 0; vertNdx < numberOfVerticalLines; vertNdx++)
            {
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(topLeft.X + deltaXVert * vertNdx, topLeft.Y + deltaYVert * vertNdx, 0), verticalColor);
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(bottomLeft.X + deltaXVert * vertNdx, bottomLeft.Y + deltaYVert * vertNdx, 0), verticalColor);
            }

            for (int horizNdx = 0; horizNdx < numberOfHorizontalLines; horizNdx++)
            {
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(topLeft.X + deltaXHoriz * horizNdx, topLeft.Y + deltaYHoriz * horizNdx, 0), horizontalColor);
                vertexList[lineNdx++] = new VertexPositionColor(new Vector3(topRight.X + deltaXHoriz * horizNdx, topRight.Y + deltaYHoriz * horizNdx, 0), horizontalColor);
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

        #endregion Grid

        // TODO - cbrantley91 : Remove
        #region Test Draw Code

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

            gridLines = PlotGridLines(new Vector2(80, 0), new Vector2(0, 400), new Vector2(400, 0), 6, 4, Color.Red, Color.Purple);

            // Initialize the vertex buffer, allocating memory for each vertex.
            vertexBuffer = new VertexBuffer(GraphicsDevice, vertexDeclaration,
                points, BufferUsage.None);

            // Set the vertex buffer data to the array of vertices.
            vertexBuffer.SetData<VertexPositionColor>(pointList);
        }

        VertexDeclaration vertexDeclaration = new VertexDeclaration(new VertexElement[]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
                });

        #endregion

        #endregion Plotting

    }
}
