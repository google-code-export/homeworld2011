using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using PlagueEngine.TimeControlSystem;
using PlagueEngine.Resources;
using PlagueEngine.Rendering;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.HighLevelGameFlow;



/************************************************************************************/
/// PlagueEngine
/************************************************************************************/
namespace PlagueEngine
{

    /********************************************************************************/
    /// Game
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /********************************************************************************/
    public class Game : Microsoft.Xna.Framework.Game
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private String              title               = String.Empty;
        private Renderer            renderer            = null;
        private ContentManager      contentManager      = null;
        private GameObjectsFactory  gameObjectsFactory  = null;
        private Level               testLevel           = null;

        private readonly RenderConfig defaultRenderConfig = new RenderConfig(1024, 768, false, false, false);
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Game(String title)
        {
            this.title          = title;
            Window.Title        = title;
            this.IsMouseVisible = true;

            Diagnostics.Game                = this;
            Diagnostics.ShowDiagnostics     = true;
            Diagnostics.ForceGCOnUpdate     = true;
            Diagnostics.LimitUpdateTimeStep = false;
            Diagnostics.ShowLogWindow       = true;                        
            Diagnostics.OpenLogFile("log");
            
            contentManager = new ContentManager(this,"Content");            
            InitRenderer();

            gameObjectsFactory = new GameObjectsFactory(renderer.ComponentsFactory,
                                                        contentManager.GameObjectsDefinitions);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Initialize
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /****************************************************************************/
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here            
            base.Initialize();
            
            testLevel = new Level(gameObjectsFactory);
            //testLevel.LoadLevel(contentManager.LoadLevel("TestLevel.lvl"));
            
            testLevel.PutSomeObjects();
            contentManager.SaveLevel("TestLevel.lvl",testLevel.SaveLevel());

            Diagnostics.PushLog("Initialization complete");
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Load Content
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /****************************************************************************/
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here            
            Diagnostics.PushLog("Loading content complete");
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Unload Content
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        /****************************************************************************/
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Diagnostics.PushLog("Unloading content complete");
            Diagnostics.CloseLogFile();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /****************************************************************************/
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            Diagnostics.Update(gameTime.ElapsedGameTime);
            TimeControl.Update(gameTime.ElapsedGameTime);

            testLevel.Update(gameTime.ElapsedGameTime);
            base.Update(gameTime);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /****************************************************************************/
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here      
            renderer.Draw();
            base.Draw(gameTime);           
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Exiting
        /****************************************************************************/
        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            
            contentManager.SaveDefaultProfile();

            Diagnostics.PushLog("Exiting");
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Title
        /****************************************************************************/
        public String Title
        {
            set
            {
                title = value;
            }

            get
            {
                return title;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init Renderer
        /****************************************************************************/
        private void InitRenderer()
        {
            RenderConfig renderConfig = null;
            try
            {
                renderConfig = contentManager.LoadConfiguration<RenderConfig>();
            }
            catch (System.IO.IOException)
            {
                renderConfig = defaultRenderConfig;
                contentManager.SaveConfiguration(defaultRenderConfig);
            }
            renderer = new Renderer(this, renderConfig);       
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Content Manager
        /****************************************************************************/
        internal ContentManager ContentManager
        {
            get
            {
                return contentManager;
            }
        }
        /****************************************************************************/
        
    }
    /********************************************************************************/

}
/************************************************************************************/