using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using PlagueEngine.TimeControlSystem;
using PlagueEngine.Resources;
using PlagueEngine.Rendering;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.Input;
using PlagueEngine.Tools;
using PlagueEngine.Physics;

/************************************************************************************/
/// PlagueEngine
/************************************************************************************/
namespace PlagueEngine
{

    /********************************************************************************/
    /// Game
    /********************************************************************************/
    public class Game : Microsoft.Xna.Framework.Game
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private String              title               = String.Empty;
        
        private Renderer            renderer            = null;
        private ContentManager      contentManager      = null;
        private Input.Input         input               = null;
        private GameObjectsFactory  gameObjectsFactory  = null;
        private PhysicsManager      physicsManager      = null;
        // TODO: Stworzyæ manager leveli, który automagicznie bêdzie wczytywa³ kolejne levele
        private Level               testLevel           = null;

        private readonly RenderConfig defaultRenderConfig = new RenderConfig(800, 600, false, false, false);
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
            input          = new Input.Input(this);

            InitRenderer();

            physicsManager = new PhysicsManager();

            gameObjectsFactory = new GameObjectsFactory(renderer.ComponentsFactory,
                                                        input.ComponentsFactory,                                        
                                                        contentManager.GameObjectsDefinitions,
                                                        physicsManager.physicsComponentFactory);

            renderer.InitDebugDrawer(physicsManager);
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
            base.Initialize();
            
            testLevel = new Level(gameObjectsFactory);
            
            testLevel.PutSomeObjects();

            //contentManager.SaveLevel("TestLevel2.lvl", testLevel.SaveLevel());
            
            //testLevel.LoadLevel(contentManager.LoadLevel("TestLevel2.lvl"));
            
            renderer.batchedMeshes.CommitMeshTransforms();


            GameObjectEditorWindow gameObjectEditor = new GameObjectEditorWindow(gameObjectsFactory, contentManager,renderer);
            gameObjectEditor.setLevel(testLevel, "TestLevel2.lvl");


            
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
            renderer.batchedMeshes.LoadEffects();
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
            contentManager.Unload();
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
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.F1))
            {
                renderer.debugDrawer.Disable();
            }
            else
            {
                renderer.debugDrawer.Enable();
            }

            Diagnostics.Update(gameTime.ElapsedGameTime);
            TimeControl.Update(gameTime.ElapsedGameTime);
            physicsManager.Update((float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond);
            input.Update();
            
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
            //contentManager.SaveLevel("TestLevel2.lvl", testLevel.SaveLevel());

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