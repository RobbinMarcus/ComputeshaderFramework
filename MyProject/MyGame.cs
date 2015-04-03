using SharpDX;
using SharpDX.Direct3D;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;
using System.IO;
using System.Diagnostics;

namespace MyProject
{
    internal sealed class MyGame : Game
    {
        GraphicsDeviceManager DeviceManager;
        GraphicsDevice Device;
        SpriteBatch spriteBatch;


        public MyGame()
        {
            DeviceManager = new GraphicsDeviceManager(this);
            DeviceManager.PreferredGraphicsProfile = new FeatureLevel[] { FeatureLevel.Level_11_0, };
            Device = DeviceManager.GraphicsDevice;
        }


        protected override void Initialize()
        {
            Window.Title = "Compute Shader Example";
            IsMouseVisible = true;
            Device = DeviceManager.GraphicsDevice;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            base.LoadContent();

            // Instantiate a SpriteBatch
            spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));

            Example example = new Example(Device);
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}