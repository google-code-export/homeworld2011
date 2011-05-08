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
using PlagueEngine.EventsSystem;
using PlagueEngine.GUI;
using Nuclex.Input;
using PlagueEngine.Particles;

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
        private String title = String.Empty;


        private GUI.GUI gui = null;
        private Renderer renderer = null;
        private ContentManager contentManager = null;
        private Input.Input input = null;
        private GameObjectsFactory gameObjectsFactory = null;
        private PhysicsManager physicsManager = null;
        private EventsSystem.EventsSystem eventsSystem = null;
        private EventsHistorian eventsHistorian = null;
        private Level Level = null;
        private ParticleManager particleManager = null;

        private readonly RenderConfig defaultRenderConfig = new RenderConfig(800, 600, false, false, false);


        public bool gameStopped = false;
        /****************************************************************************/




        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Game(String title)
        {
            this.title = title;
            Window.Title = title;
            this.IsMouseVisible = true;

#if DEBUG
            Diagnostics.Game = this;
            Diagnostics.ShowDiagnostics = true;
            Diagnostics.ForceGCOnUpdate = true;
            Diagnostics.LimitUpdateTimeStep = false;
            Diagnostics.ShowLogWindow = true;
            Diagnostics.OpenLogFile("log");
#endif

            contentManager = new ContentManager(this, "Content");

            input = new Input.Input(this, Services);

            gui = new GUI.GUI(this, Services,input.ComponentsFactory.CreateMouseListenerComponent(null,true));

            particleManager = new ParticleManager();

            InitRenderer();

            particleManager.CreateFactory(contentManager, renderer);

            physicsManager = new PhysicsManager(contentManager);

            gameObjectsFactory = new GameObjectsFactory(renderer.ComponentsFactory,
                                                        input.ComponentsFactory,
                                                        null,
                                                        contentManager.GameObjectsDefinitions,
                                                        physicsManager.physicsComponentFactory,
                                                        particleManager.particleFactory);


            Level = new Level(gameObjectsFactory);

            eventsSystem = new EventsSystem.EventsSystem(Level);

            eventsHistorian = new EventsHistorian(20);

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

            renderer.InitHelpers();


            Level.PutSomeObjects();
            

            //contentManager.SaveLevel("TestLevel2.lvl", testLevel.SaveLevel());

            //testLevel.LoadLevel(contentManager.LoadLevel("TestLevel2.lvl"));

            renderer.batchedMeshes.CommitMeshTransforms();
#if DEBUG
            GameObjectEditorWindow gameObjectEditor = new GameObjectEditorWindow(gameObjectsFactory, contentManager, renderer, input, gui, this);
            gameObjectEditor.setLevel(Level, "TestLevel2.lvl");
#endif

            InitGUI();  
            
            base.Initialize();              
#if DEBUG
            Diagnostics.PushLog("Initialization complete");
#endif
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
            renderer.LoadEffects();
#if DEBUG
            Diagnostics.PushLog("Loading content complete");
#endif
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
#if DEBUG
            Diagnostics.PushLog("Unloading content complete");
            Diagnostics.CloseLogFile(); 
#endif
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
#if DEBUG
            Diagnostics.Update(gameTime.ElapsedGameTime);
#endif
            TimeControl.Update(gameTime.ElapsedGameTime);

            input.Update();
            //TODO: sprawdziæ czy konieczne i usun¹æ jeli niepotrzebne
            gui.Update(gameTime);
            
            eventsSystem.Update();
            if (!gameStopped)
            {
                physicsManager.Update(((float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond));
                base.Update(gameTime);
            }

            particleManager.Update(gameTime);
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
            renderer.Draw(gameTime.ElapsedGameTime,gameTime);
            gui.Draw(gameTime);
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

#if DEBUG
            Diagnostics.PushLog("Exiting");
#endif
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
        /// ParticleManager
        /****************************************************************************/
        internal ParticleManager ParticleManager
        {
            get
            {
                return this.particleManager;
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
        /// Init GUI
        /****************************************************************************/
        private void InitGUI()
        {
            //GraphicsDevice.Reset();
            gui.Initialize(GraphicsDevice);
            //GraphicsDevice.Reset();
            //gui.Initialize(GraphicsDevice);
            //this.Components.Add(gui.Manager);
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




        /****************************************************************************/
        /// Flush Events History
        /****************************************************************************/
        public void FlushEventsHistory()
        {
            if (eventsHistorian != null) eventsHistorian.Flush();
        }
        /****************************************************************************/


    }
    /********************************************************************************/


}
/************************************************************************************/