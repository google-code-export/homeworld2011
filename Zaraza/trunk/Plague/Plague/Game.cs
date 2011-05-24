using System;
using Microsoft.Xna.Framework;
using PlagueEngine.TimeControlSystem;
using PlagueEngine.Resources;
using PlagueEngine.Rendering;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.Tools;
using PlagueEngine.Physics;
using PlagueEngine.Particles;

namespace PlagueEngine
{

    /********************************************************************************/
    /// Game
    /********************************************************************************/
    public class Game : Microsoft.Xna.Framework.Game
    {

        /****************************************************************************/
        public   String                    Title              { get; set; }
        internal Renderer                  Renderer           { get; private set; }
        internal GUI.GUI                   GUI                { get; private set; }
        internal ContentManager            ContentManager     { get; set; }
        internal EventsHistorian           EventsHistorian    { get; private set; }
        internal ParticleManager           ParticleManager    { get; private set; }
        internal Input.Input               Input              { get; private set; }
        internal GameObjectsFactory        GameObjectsFactory { get; private set; }
        internal PhysicsManager            PhysicsManager     { get; private set; }
        
        internal Level Level { get; private set; }
        
        private readonly RenderConfig _defaultRenderConfig = new RenderConfig(800, 600, false, false, false);
        
        public bool GameStopped { get;  set; }

        internal Clock RendererClock = null;
        internal Clock PhysicsClock  = null;
        /****************************************************************************/

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Game(String title)
        {
            Title = title;
            Window.Title = title;
            IsMouseVisible = true;

#if DEBUG
            Diagnostics.Game                = this;
            Diagnostics.ShowDiagnostics     = true;
            Diagnostics.ForceGCOnUpdate     = true;
            Diagnostics.LimitUpdateTimeStep = false;
            Diagnostics.ShowLogWindow       = true;
            Diagnostics.OpenLogFile("log");
#endif

            ContentManager = new ContentManager(this, "Content");

            ParticleManager = new ParticleManager();

            InitRenderer();

            Input = new Input.Input(this, Services,Renderer.Device);

            GUI = new GUI.GUI(Services, Input.ComponentsFactory.CreateMouseListenerComponent(null, true));

            ParticleManager.CreateFactory(ContentManager, Renderer);

            PhysicsManager = new PhysicsManager(ContentManager);

            Level = new Level(ContentManager);
            Level.GameObjectsFactory.Init(Renderer.ComponentsFactory,
                                          Input.ComponentsFactory,
                                          GUI.ComponentsFactory,
                                          ContentManager.GameObjectsDefinitions,
                                          PhysicsManager.physicsComponentFactory,
                                          ParticleManager.particleFactory,
                                          this);
            Level.Init();

            EventsHistorian = new EventsHistorian(20);

            Renderer.InitDebugDrawer(PhysicsManager);

            RendererClock = TimeControl.CreateClock();
            PhysicsClock  = TimeControl.CreateClock();
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

            Renderer.InitHelpers();

            Level.NewLevel("TestLevel.lvl");
            Level.PutSomeObjects();

            Renderer.batchedMeshes.CommitMeshTransforms();
            
#if DEBUG
            var gameObjectEditor = new GameObjectEditorWindow(Level, ContentManager, Renderer, Input, this);
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
            Renderer.LoadEffects();

            Renderer.LoadFonts("Courier New", "Courier New Bold","Arial");

            Input.SetCursorTexture(ContentManager.LoadTexture2D("cursor"), 4, 4, 
                                   new[] { "Default","QuestionMark","Footsteps","Target",
                                                  "Hand",   "Person",      "Targeting","0",
                                                  "1",      "2",            "3",       "4",
                                                  "Up",     "Down",         "Left",    "Right" });
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
            ContentManager.Unload();
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

            Input.Update();            
            GUI.Update(gameTime);

            Level.UpdateEventsSystem();
            
            if (!GameStopped)
            {
                PhysicsManager.Update((float)PhysicsClock.DeltaTime.TotalSeconds);                
            }

            ParticleManager.Update(gameTime);
            
            Level.Update(gameTime.ElapsedGameTime);

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
            Renderer.Draw(RendererClock.DeltaTime,gameTime);
            GUI.Draw(gameTime);
            Input.Draw();
            base.Draw(gameTime);
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// On Exiting
        /****************************************************************************/
        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            ContentManager.SaveDefaultProfile();

#if DEBUG
            Diagnostics.PushLog("Exiting");
#endif
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Init Renderer
        /****************************************************************************/
        private void InitRenderer()
        {
            RenderConfig renderConfig;
            try
            {
                renderConfig = ContentManager.LoadConfiguration<RenderConfig>();
            }
            catch (System.IO.IOException)
            {
                renderConfig = _defaultRenderConfig;
                ContentManager.SaveConfiguration(_defaultRenderConfig);
            }
            Renderer = new Renderer(this, renderConfig);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init GUI
        /****************************************************************************/
        private void InitGUI()
        {
            GUI.Initialize(GraphicsDevice);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Flush Events History
        /****************************************************************************/
        public void FlushEventsHistory()
        {
            if (EventsHistorian != null) EventsHistorian.Flush();
        }
        /****************************************************************************/


    }
    /********************************************************************************/


}
/************************************************************************************/