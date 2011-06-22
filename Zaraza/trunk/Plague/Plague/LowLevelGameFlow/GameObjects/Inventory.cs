using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.Input;
using PlagueEngine.EventsSystem;


/************************************************************************************/
/// UWAGA!!!
/************************************************************************************
 * W tym kodzie panuje bałagan. Zdaję sobie sprawę że warto byłoby to posprzątać...
 * ale czasu mało. Poza tym początkowo nie zważałem na rozległą tu copy-paste, nie 
 * wiedziałem, że aż tak się rozrośnie :/ Będe sie starał to co nowe upokaowywać lepiej 
 * nieco. Starych rzeczy jeżeli nie będę potrzebował ruszać to nie ruszam.
 * Pozdrawiam radiosłuchaczy.
/************************************************************************************/


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Inventory
    /********************************************************************************/
    class Inventory : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private FrontEndComponent         frontEnd           = null;
        private KeyboardListenerComponent keyboard           = null;
        private MouseListenerComponent    mouse              = null;
        
        private MercenariesManager        mercenariesManager = null;
        private Container                 container          = null;
        private SpriteFont                mercenaryName      = null;

        private SpriteFont                AmmoFont           = null;

        /*****************/
        /// Merc1
        /*****************/
        private Mercenary  mercenary         = null;                
        private Vector2    mercenaryNamePos;
        private SlotContent[,] Items;
        /*****************/
        

        /*****************/
        /// Merc2
        /*****************/
        private Mercenary  mercenary2        = null;        
        private Vector2    mercenary2NamePos;
        private SlotContent[,] Items2;
        /*****************/


        /******************/
        /// Inventory Tab1
        /******************/
        private bool    nextMerc            = false;
        private bool    prevMerc            = false;
        private bool    exitInv             = false;
        private bool    scrollUp            = false;
        private bool    scrollDown          = false;
        private bool    scroll              = false;
        private float   scrollStep          = 0;
        private int     scrollMaxOffset     = 0;
        private int     scrollCurrentOffset = 0;
        /******************/


        /******************/
        /// Inventory Tab2
        /******************/
        private bool    exitInv2             = false;
        private bool    scrollUp2            = false;
        private bool    scrollDown2          = false;
        private bool    scroll2              = false;
        private float   scrollStep2          = 0;
        private int     scrollMaxOffset2     = 0;
        private int     scrollCurrentOffset2 = 0;
        /******************/


        /*****************/
        /// Mouse Pos
        /*****************/        
        private Vector2 localPosition;
        private Vector2 mousePos;
        private Vector2 realMousePos;
        /*****************/


        /*****************/
        /// Dump
        /*****************/        
        private SlotContent[,]  dumpContent;        
        private String          dumpTitle               = "Dump";
        private bool            dump                    = false;
        private bool            dumpButton              = false;
        private bool            dumpScrollUp            = false;
        private bool            dumpScrollDown          = false;
        private bool            dumpScroll              = false;
        private float           dumpScrollStep          = 0;
        private int             dumpScrollMaxOffset     = 0;
        private int             dumpScrollCurrentOffset = 0;
        private Dictionary<StorableObject, ItemPosition> dumpItems = new Dictionary<StorableObject, ItemPosition>();
        /*****************/


        /*****************/
        /// Ammo
        /*****************/
        private bool     AmmoLoader         = false;
        private AmmoClip CurrentAmmoClip    = null;
        private bool     AmmoLoaderExit     = false;
        private bool     AmmoLoaderUnload   = false;
        private object[] AmmoSlots          = new object[4];
        private bool[]   AmmoSlotsLoad      = new bool[4];
        private bool[]   AmmoSlotsLoadOne   = new bool[4];
        private bool     AmmoUnloadOne      = false;
        /*****************/


        /*****************/
        /// Item Desc
        /*****************/
        private bool           ItemDesc      = false;
        private StorableObject DescribedItem = null;
        private bool           ItemDescExit  = false;
        /*****************/


        /*****************/
        /// Firearm
        /*****************/
        private bool    FirearmWindow     = false;
        private Firearm CurrentFirearm    = null;
        private bool    FirearmWindowExit = false;
        /*****************/        
        

        private StorableObject pickedItem     = null;
        private Slot      pickedItemSlot = Slot.Empty;
        private List<int> pickedItemSlots;
        private int       newPickedItemOrientation = 1;
        private int       oldPickedItemOrientation = 1;
        private bool      pickedFromMerc2 = false;
        private int       pickedFromSlot = 0;

        private bool leftControl = false;

        private AmmunitionInfo ammunitionInfo = null;

        private bool highlights = false;
        /****************************************************************************/


        /****************************************************************************/
        /// Consts (lol! Vector2 nie może być const)
        /****************************************************************************/
        private Vector2 NextMercButtonPos   = new Vector2(385, 30);
        private Vector2 PrevMercButtonPos   = new Vector2(308, 30);
        private Vector2 ExitInvButtonPos    = new Vector2(20, 0);
        private Vector2 CurrItemIconPos     = new Vector2(26, 40);
        private Vector2 WeaponIconPos       = new Vector2(99, 40);
        private Vector2 SideArmIconPos      = new Vector2(174, 40);
        private Vector2 ScrollUpButtonPos   = new Vector2(400, 223);
        private Vector2 ScrollDownButtonPos = new Vector2(400, 561);
        private Vector2 ScrollButtonBasePos = new Vector2(400, 237);
        private Vector2 SlotsStartPos       = new Vector2(31, 120);
        
        private Vector2 ExitInv2ButtonPos    = new Vector2(805, 0);
        private Vector2 CurrItemIconPos2     = new Vector2(542, 40);
        private Vector2 WeaponIconPos2       = new Vector2(615, 40);
        private Vector2 SideArmIconPos2      = new Vector2(690, 40);
        private Vector2 ScrollUp2ButtonPos   = new Vector2(424, 223);
        private Vector2 ScrollDown2ButtonPos = new Vector2(424, 561);
        private Vector2 Scroll2ButtonBasePos = new Vector2(424, 237);
        private Vector2 SlotsStartPos2       = new Vector2(451, 120);

        private Vector2 DumpButtonPos           = new Vector2(400, 120);
        private Vector2 DumpSlotsStartPos       = new Vector2(451, 30);
        private Vector2 DumpScrollUpButtonPos   = new Vector2(425, 50);
        private Vector2 DumpScrollDownButtonPos = new Vector2(425, 260);
        private Vector2 DumpScrollButtonBasePos = new Vector2(425, 64);

        private Vector2 CurrAmmoIconPos              = new Vector2(500,342);
        private Vector2 AmmoLoaderExitButtonPos      = new Vector2(420,310);
        private Vector2 AmmoLoaderUnloadButtonPos    = new Vector2(517, 392);
        private Vector2 AmmoLoaderUnloadOneButtonPos = new Vector2(690, 365);
        private Vector2 AmmoSlotsLoadButtonPos       = new Vector2(472, 462);
        private Vector2 AmmoSlotsLoadOneButtonPos    = new Vector2(506, 495);
        private Vector2 AmmoSlotsIconPos             = new Vector2(455, 480);
        private Vector2 BulletIconPos                = new Vector2(563, 354);

        private Vector2 DescribedItemIconPos  = new Vector2(500, 342);
        private Vector2 ItemDescExitButtonPos = new Vector2(420, 310);

        private Vector2 FirearmIconPos         = new Vector2(440, 345);
        private Vector2 FirearmAmmoClipIconPos = new Vector2(500, 340);
        private Vector2 AccessoriesIconPos     = new Vector2(580, 340);
        private Vector2 FirearmExitButtonPos   = new Vector2(420, 310);

        private const int    dumpCapacity     = 200;
        private const String defaultDumpTitle = "Dump";
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(FrontEndComponent         frontEnd,
                         KeyboardListenerComponent keyboard,
                         MouseListenerComponent    mouse,
                         Mercenary                 mercenary,
                         Mercenary                 mercenary2,
                         MercenariesManager        mercenariesManager,
                         Container                 container)
        {
            this.frontEnd           = frontEnd;
            this.mercenary          = mercenary;
            this.mercenary2         = mercenary2;
            
            this.mercenariesManager = mercenariesManager;
            this.container          = container;
            
            this.keyboard           = keyboard;
            this.mouse              = mouse;

            mouse.Modal    = true;
            keyboard.Modal = true;

            keyboard.SubscibeKeys   (OnKey, Keys.Escape,Keys.Space,Keys.E,Keys.Tab,Keys.D,Keys.F);
            keyboard.SubscibeKeys   (delegate(Keys key, ExtendedKeyState state) { leftControl = state.IsDown(); }, Keys.LeftControl);

            mouse.SubscribeKeys     (OnMouseKey, MouseKeyAction.LeftClick,MouseKeyAction.RightClick);
            mouse.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move,MouseMoveAction.Scroll);
            
            frontEnd.Draw = OnDraw;

            mercenaryName = frontEnd.GetFont("Arial");
            AmmoFont      = frontEnd.GetFont("Arial");
 
            mouse.SetCursor("Default");

            SetupMercenary();
            if (mercenary2 != null) SetupMercenary2();

            if (container != null)
            {
                PrepareDump(container.Slots, container.Name, container.Items);
                dump = true;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {            
            localPosition = new Vector2((screenWidth/2) - 420,100);
            
            Vector2 pickedItemOffset = Vector2.Zero;
            if (pickedItem != null)
            {
                pickedItemOffset.X = (pickedItem.SlotsIcon.Width  / 2) - 32;
                pickedItemOffset.Y = (pickedItem.SlotsIcon.Height / 2) - 32;
            }

            /***********************/
            #region Draw Main Inventory
            /***********************/
                /***********************/
                #region Background
                /***********************/
                spriteBatch.Draw(frontEnd.Texture, localPosition, new Rectangle(0, 0, 420, 620), Color.White);
                // Merc Head
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(328, 5), mercenary.InventoryIcon, Color.White);
                // Merc Name
                spriteBatch.DrawString(mercenaryName, mercenary.Name, localPosition + mercenaryNamePos, Color.LightGray);
                // Switch Merc Button
                if (mercenariesManager != null)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + NextMercButtonPos, (nextMerc ? new Rectangle(1290, 14, 15, 14) : new Rectangle(1290, 0, 15, 14)), Color.White);
                    spriteBatch.Draw(frontEnd.Texture, localPosition + PrevMercButtonPos, (prevMerc ? new Rectangle(1275, 14, 15, 14) : new Rectangle(1275, 0, 15, 14)), Color.White);
                }
                // Exit Inventory Button
                spriteBatch.Draw(frontEnd.Texture, localPosition + ExitInvButtonPos, (exitInv ? new Rectangle(1260, 14, 15, 14) : new Rectangle(1260, 0, 15, 14)), Color.White);
                // Scroll
                if (scrollMaxOffset > 0)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollUpButtonPos, (scrollUp ? new Rectangle(1305, 14, 15, 14) : new Rectangle(1305, 0, 15, 14)), Color.White);
                    spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollDownButtonPos, (scrollDown ? new Rectangle(1320, 14, 15, 14) : new Rectangle(1320, 0, 15, 14)), Color.White);
                    spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollButtonBasePos + new Vector2(0, scrollStep * scrollCurrentOffset), (scroll ? new Rectangle(1335, 14, 15, 14) : new Rectangle(1335, 0, 15, 14)), Color.White);
                }
                // Dump Button
                if (container == null && mercenary2 == null)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + DumpButtonPos, (dumpButton ? new Rectangle(1319, 29, 19, 52) : new Rectangle(1300, 29, 19, 52)), Color.White);
                }
                /***********************/
                #endregion
                /***********************/
                #region Items
                /***********************/
                // Slots
                for (int y = 0; y < Items.GetLength(1) && y < 15; ++y)
                {
                    for (int x = 0; x < Items.GetLength(0); ++x)
                    {
                        if (!Items[x, y + scrollCurrentOffset].Blank)
                        {
                            if (Items[x, y + scrollCurrentOffset].Blocked)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos + new Vector2(32 * x, 32 * y), new Rectangle(1324, 81, 32, 32), Color.White);
                            }
                            else if (Items[x, y + scrollCurrentOffset].Hover)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos + new Vector2(32 * x, 32 * y), new Rectangle(1356, 81, 32, 32), Color.White);
                            }
                            else if (Items[x, y + scrollCurrentOffset].Tiny)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos + new Vector2(32 * x, 32 * y), new Rectangle(1292, 81, 32, 32), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos + new Vector2(32 * x, 32 * y), new Rectangle(1260, 81, 32, 32), Color.White);
                            }

                            Items[x, y + scrollCurrentOffset].Hover = false;
                            Items[x, y + scrollCurrentOffset].Blocked = false;
                        }
                    }
                }
                spriteBatch.Draw(frontEnd.Texture, 
                                 localPosition + SlotsStartPos + new Vector2(103, 0), 
                                 new Rectangle( 1260, 
                                                177 + 32 * scrollCurrentOffset, 
                                                146, 
                                                64 - (scrollCurrentOffset > 2 ? 64 : 32 * scrollCurrentOffset)), 
                                 Color.White);

                int tinySlotsOffset = (int)Math.Round(mercenary.TinySlots / 11.0f);

                spriteBatch.Draw(frontEnd.Texture, 
                                 localPosition + SlotsStartPos + new Vector2(112, (tinySlotsOffset * 32) - (32 * scrollCurrentOffset)), 
                                 new Rectangle( 1260, 
                                                241 + (32 * (scrollCurrentOffset - tinySlotsOffset > 0 ? scrollCurrentOffset - tinySlotsOffset : 0)), 
                                                128,
                                                180 - (scrollCurrentOffset > 6 ? 180 : (32 * (scrollCurrentOffset - tinySlotsOffset > 0 ? scrollCurrentOffset - tinySlotsOffset : 0)))), 
                                 Color.White);                    
                // Items in inventory
                foreach (KeyValuePair<StorableObject, ItemPosition> pair in mercenary.Items)
                {
                    Rectangle rect = pair.Key.SlotsIcon;
                    int itemSlot = pair.Value.Slot;
                    int height = (pair.Value.Orientation < 0 ? rect.Width / 32 : rect.Height / 32);
                    int y = itemSlot / 11;
                    int diff = scrollCurrentOffset - y;
                    int diff2 = y + height - (scrollCurrentOffset + 15);

                    if (diff > 0)
                    {
                        if (pair.Value.Orientation < 0)
                        {
                            rect.X += 32 * diff;
                            rect.Width -= 32 * diff;
                        }
                        else
                        {
                            rect.Y += 32 * diff;
                            rect.Height -= 32 * diff;
                        }
                    }
                    else
                    {
                        diff = 0;
                    }

                    if (diff2 > 0)
                    {
                        if (pair.Value.Orientation < 0)
                        {
                            rect.Width -= 32 * diff2;
                        }
                        else
                        {
                            rect.Height -= 32 * diff2;
                        }
                    }

                    if (y > scrollCurrentOffset - height && y < scrollCurrentOffset + 15)
                    {
                        if (pair.Value.Orientation > 0)
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos + new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset + diff)), rect, CheckAmmunitionInfo(pair.Key) ? Color.White : Color.Black);
                        }
                        else
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos + new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset + diff)) + new Vector2(pair.Key.SlotsIcon.Height, 0), rect, CheckAmmunitionInfo(pair.Key) ? Color.White : Color.Black, MathHelper.PiOver2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                        }

                        if (diff2 <= 0)
                        {
                            Type ItemType = pair.Key.GetType();

                            if (pair.Value.Orientation > 0)
                            {
                                DrawValue(spriteBatch,
                                        pair.Key,
                                        localPosition +
                                        SlotsStartPos +
                                        new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset)) +
                                        new Vector2(pair.Key.SlotsIcon.Width, pair.Key.SlotsIcon.Height));
                            }
                            else
                            {
                                DrawValue(spriteBatch,
                                        pair.Key,
                                        localPosition +
                                        SlotsStartPos +
                                        new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset)) +
                                        new Vector2(pair.Key.SlotsIcon.Height, pair.Key.SlotsIcon.Width));
                            }                               
                        }
                    }                   
                }            
                /***********************/
                #endregion
                /***********************/                       
            /***********************/
            #endregion
            /***********************/
                             
       
            /***********************/
            #region Draw Second Inventory
            /***********************/
            if (mercenary2 != null)
            {
                /***********************/
                #region Background
                /***********************/
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(420, 0), new Rectangle(420, 0, 420, 620), Color.White);
                // Merc Head
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(461, 5), mercenary2.InventoryIcon, Color.White);
                //// Merc Name
                spriteBatch.DrawString(mercenaryName, mercenary2.Name, localPosition + mercenary2NamePos, Color.LightGray);
                // Exit Inventory Button
                spriteBatch.Draw(frontEnd.Texture, localPosition + ExitInv2ButtonPos, (exitInv2 ? new Rectangle(1260, 14, 15, 14) : new Rectangle(1260, 0, 15, 14)), Color.White);
                // Scroll
                if (scrollMaxOffset2 > 0)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollUp2ButtonPos, (scrollUp2 ? new Rectangle(1305, 14, 15, 14) : new Rectangle(1305, 0, 15, 14)), Color.White);
                    spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollDown2ButtonPos, (scrollDown2 ? new Rectangle(1320, 14, 15, 14) : new Rectangle(1320, 0, 15, 14)), Color.White);
                    spriteBatch.Draw(frontEnd.Texture, localPosition + Scroll2ButtonBasePos + new Vector2(0, scrollStep2 * scrollCurrentOffset2), (scroll2 ? new Rectangle(1335, 14, 15, 14) : new Rectangle(1335, 0, 15, 14)), Color.White);
                }
                /***********************/
                #endregion
                /***********************/                
                #region Items
                /***********************/
                // Slots
                for (int y = 0; y < Items2.GetLength(1) && y < 15; ++y)
                {
                    for (int x = 0; x < Items2.GetLength(0); ++x)
                    {
                        if (!Items2[x, y + scrollCurrentOffset2].Blank)
                        {
                            if (Items2[x, y + scrollCurrentOffset2].Blocked)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos2 + new Vector2(32 * x, 32 * y), new Rectangle(1324, 81, 32, 32), Color.White);
                            }
                            else if (Items2[x, y + scrollCurrentOffset2].Hover)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos2 + new Vector2(32 * x, 32 * y), new Rectangle(1356, 81, 32, 32), Color.White);
                            }
                            else if (Items2[x, y + scrollCurrentOffset2].Tiny)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos2 + new Vector2(32 * x, 32 * y), new Rectangle(1292, 81, 32, 32), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos2 + new Vector2(32 * x, 32 * y), new Rectangle(1260, 81, 32, 32), Color.White);
                            }

                            Items2[x, y + scrollCurrentOffset2].Hover = false;
                            Items2[x, y + scrollCurrentOffset2].Blocked = false;
                        }
                    }
                }
                spriteBatch.Draw(frontEnd.Texture,
                                 localPosition + SlotsStartPos2 + new Vector2(103, 0),
                                 new Rectangle(1260,
                                                177 + 32 * scrollCurrentOffset2,
                                                146,
                                                64 - (scrollCurrentOffset2 > 2 ? 64 : 32 * scrollCurrentOffset2)),
                                 Color.White);

                tinySlotsOffset = (int)Math.Round(mercenary2.TinySlots / 11.0f);

                spriteBatch.Draw(frontEnd.Texture,
                                 localPosition + SlotsStartPos2 + new Vector2(112, (tinySlotsOffset * 32) - (32 * scrollCurrentOffset2)),
                                 new Rectangle(1260,
                                                241 + (32 * (scrollCurrentOffset2 - tinySlotsOffset > 0 ? scrollCurrentOffset2 - tinySlotsOffset : 0)),
                                                128,
                                                180 - (scrollCurrentOffset2 > 6 ? 180 : (32 * (scrollCurrentOffset2 - tinySlotsOffset > 0 ? scrollCurrentOffset2 - tinySlotsOffset : 0)))),
                                 Color.White);        
                // Items in inventory
                foreach (KeyValuePair<StorableObject, ItemPosition> pair in mercenary2.Items)
                {
                    Rectangle rect = pair.Key.SlotsIcon;
                    int itemSlot = pair.Value.Slot;
                    int height = (pair.Value.Orientation < 0 ? rect.Width / 32 : rect.Height / 32);
                    int y = itemSlot / 11;
                    int diff = scrollCurrentOffset2 - y;
                    int diff2 = y + height - (scrollCurrentOffset2 + 15);

                    if (diff > 0)
                    {
                        if (pair.Value.Orientation < 0)
                        {
                            rect.X += 32 * diff;
                            rect.Width -= 32 * diff;
                        }
                        else
                        {
                            rect.Y += 32 * diff;
                            rect.Height -= 32 * diff;
                        }
                    }
                    else
                    {
                        diff = 0;
                    }

                    if (diff2 > 0)
                    {
                        if (pair.Value.Orientation < 0)
                        {
                            rect.Width -= 32 * diff2;
                        }
                        else
                        {
                            rect.Height -= 32 * diff2;
                        }
                    }

                    if (y > scrollCurrentOffset2 - height && y < scrollCurrentOffset2 + 15)
                    {
                        if (pair.Value.Orientation > 0)
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos2 + new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset2 + diff)), rect, CheckAmmunitionInfo(pair.Key) ? Color.White : Color.Black);
                        }
                        else
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos2 + new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset2 + diff)) + new Vector2(pair.Key.SlotsIcon.Height, 0), rect, CheckAmmunitionInfo(pair.Key) ? Color.White : Color.Black, MathHelper.PiOver2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                        }
                        
                        if (diff2 <= 0)
                        {
                            Type ItemType = pair.Key.GetType();

                            if (pair.Value.Orientation > 0)
                            {
                                DrawValue(spriteBatch,
                                        pair.Key,
                                        localPosition +
                                        SlotsStartPos2 +
                                        new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset2)) +
                                        new Vector2(pair.Key.SlotsIcon.Width, pair.Key.SlotsIcon.Height));
                            }
                            else
                            {
                                DrawValue(spriteBatch,
                                        pair.Key,
                                        localPosition +
                                        SlotsStartPos2 +
                                        new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset2)) +
                                        new Vector2(pair.Key.SlotsIcon.Height, pair.Key.SlotsIcon.Width));
                            }
                        }
                    }
                }
                /***********************/
                #endregion
                /***********************/
            }
            /***********************/
            #endregion
            /***********************/


            /***********************/
            #region Draw Dump
            /***********************/
            if (dump)
            {
                // Background
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(420,0), new Rectangle(840, 310, 420, 310), Color.White);
                // Title
                spriteBatch.DrawString(frontEnd.GetFont("Arial"), dumpTitle, localPosition + new Vector2(620 - frontEnd.GetFont("Arial").MeasureString(dumpTitle).X/2, 0), Color.LightGray);
                // Scroll
                if (dumpScrollMaxOffset > 0)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + DumpScrollUpButtonPos, (dumpScrollUp ? new Rectangle(1305, 14, 15, 14) : new Rectangle(1305, 0, 15, 14)), Color.White);
                    spriteBatch.Draw(frontEnd.Texture, localPosition + DumpScrollDownButtonPos, (dumpScrollDown ? new Rectangle(1320, 14, 15, 14) : new Rectangle(1320, 0, 15, 14)), Color.White);
                    spriteBatch.Draw(frontEnd.Texture, localPosition + DumpScrollButtonBasePos + new Vector2(0, dumpScrollStep * dumpScrollCurrentOffset), (dumpScroll ? new Rectangle(1335, 14, 15, 14) : new Rectangle(1335, 0, 15, 14)), Color.White);
                }
                // Slots
                for (int y = 0; y < dumpContent.GetLength(1) && y < 8; ++y)
                {
                    for (int x = 0; x < dumpContent.GetLength(0); ++x)
                    {
                        if (!dumpContent[x, y + dumpScrollCurrentOffset].Blank)
                        {
                            if (dumpContent[x, y + dumpScrollCurrentOffset].Blocked)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + DumpSlotsStartPos + new Vector2(32 * x, 32 * y), new Rectangle(1324, 81, 32, 32), Color.White);
                            }
                            else if (dumpContent[x, y + dumpScrollCurrentOffset].Hover)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + DumpSlotsStartPos + new Vector2(32 * x, 32 * y), new Rectangle(1356, 81, 32, 32), Color.White);
                            }
                            else if (dumpContent[x, y + dumpScrollCurrentOffset].Tiny)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + DumpSlotsStartPos + new Vector2(32 * x, 32 * y), new Rectangle(1292, 81, 32, 32), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + DumpSlotsStartPos + new Vector2(32 * x, 32 * y), new Rectangle(1260, 81, 32, 32), Color.White);
                            }

                            dumpContent[x, y + dumpScrollCurrentOffset].Hover = false;
                            dumpContent[x, y + dumpScrollCurrentOffset].Blocked = false;
                        }
                    }
                }
                // Items
                foreach (KeyValuePair<StorableObject, ItemPosition> pair in dumpItems)
                {
                    Rectangle rect = pair.Key.SlotsIcon;
                    int itemSlot = pair.Value.Slot;
                    int height = (pair.Value.Orientation < 0 ? rect.Width / 32 : rect.Height / 32);
                    int y = itemSlot / 11;
                    int diff = dumpScrollCurrentOffset - y;
                    int diff2 = y + height - (dumpScrollCurrentOffset + 8);

                    if (diff > 0)
                    {
                        if (pair.Value.Orientation < 0)
                        {
                            rect.X += 32 * diff;
                            rect.Width -= 32 * diff;
                        }
                        else
                        {
                            rect.Y += 32 * diff;
                            rect.Height -= 32 * diff;
                        }
                    }
                    else
                    {
                        diff = 0;
                    }

                    if (diff2 > 0)
                    {
                        if (pair.Value.Orientation < 0)
                        {
                            rect.Width -= 32 * diff2;
                        }
                        else
                        {
                            rect.Height -= 32 * diff2;
                        }
                    }

                    if (y > dumpScrollCurrentOffset - height && y < dumpScrollCurrentOffset + 8)
                    {
                        if (pair.Value.Orientation > 0)
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + DumpSlotsStartPos + new Vector2(32 * (itemSlot % 11), 32 * (y - dumpScrollCurrentOffset + diff)), rect, CheckAmmunitionInfo(pair.Key) ? Color.White : Color.Black);
                        }
                        else
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + DumpSlotsStartPos + new Vector2(32 * (itemSlot % 11), 32 * (y - dumpScrollCurrentOffset + diff)) + new Vector2(pair.Key.SlotsIcon.Height, 0), rect, CheckAmmunitionInfo(pair.Key) ? Color.White : Color.Black, MathHelper.PiOver2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                        }

                        if (diff2 <= 0)
                        {
                            Type ItemType = pair.Key.GetType();

                            if (pair.Value.Orientation > 0)
                            {
                                DrawValue(spriteBatch,
                                        pair.Key,
                                        localPosition +
                                        DumpSlotsStartPos +
                                        new Vector2(32 * (itemSlot % 11), 32 * (y - dumpScrollCurrentOffset)) +
                                        new Vector2(pair.Key.SlotsIcon.Width, pair.Key.SlotsIcon.Height));
                            }
                            else
                            {
                                DrawValue(spriteBatch,
                                        pair.Key,
                                        localPosition +
                                        DumpSlotsStartPos +
                                        new Vector2(32 * (itemSlot % 11), 32 * (y - dumpScrollCurrentOffset)) +
                                        new Vector2(pair.Key.SlotsIcon.Height, pair.Key.SlotsIcon.Width));
                            }
                        }
                    }
                }
            }
            /***********************/
            #endregion
            /***********************/


            /***********************/
            #region Draw Ammo Loader
            /***********************/
            if (AmmoLoader)
            {
                // Background
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(420, 310), new Rectangle(1628, 0, 420, 310), Color.White);
                // Exit Button
                spriteBatch.Draw(frontEnd.Texture, localPosition + AmmoLoaderExitButtonPos, (AmmoLoaderExit ? new Rectangle(1260, 14, 15, 14) : new Rectangle(1260, 0, 15, 14)), Color.White);
                // Draw Current Clip                
                spriteBatch.Draw(frontEnd.Texture, localPosition + CurrAmmoIconPos, CurrentAmmoClip.Icon, Color.White);
                DrawValue(spriteBatch, CurrentAmmoClip, localPosition + CurrAmmoIconPos + new Vector2(50, 50));
                // Draw Ammo Clip Name
                spriteBatch.DrawString(AmmoFont, CurrentAmmoClip.Name, localPosition + new Vector2(620 - AmmoFont.MeasureString(CurrentAmmoClip.Name).X / 2, 310), Color.LightGray);
                // Draw Ammo Name
                spriteBatch.DrawString(AmmoFont, CurrentAmmoClip.AmmunitionInfo.Name, localPosition + new Vector2(620 - AmmoFont.MeasureString(CurrentAmmoClip.AmmunitionInfo.Name).X / 2, 417), Color.LightGray);
                // Draw Unload Button
                spriteBatch.Draw(frontEnd.Texture, localPosition + AmmoLoaderUnloadButtonPos, (AmmoLoaderUnload ? new Rectangle(1320, 14, 15, 14) : new Rectangle(1320, 0, 15, 14)), Color.White);
                // Draw Onload One button
                spriteBatch.Draw(frontEnd.Texture, localPosition + AmmoLoaderUnloadOneButtonPos, (AmmoUnloadOne ? new Rectangle(1365, 14, 15, 14) : new Rectangle(1365, 0, 15, 14)), Color.White);
                // Draw Load Buttons
                for (int i = 0; i < 4; i++)
                {
                    spriteBatch.Draw(frontEnd.Texture, 
                                     localPosition + AmmoSlotsLoadButtonPos + new Vector2(93 * i,0),
                                     (AmmoSlotsLoad[i] ? new Rectangle(1305, 14, 15, 14) : new Rectangle(1305, 0, 15, 14)), 
                                     Color.White);
                }
                // Draw Load One Buttons
                for (int i = 0; i < 4; i++)
                {
                    spriteBatch.Draw(frontEnd.Texture,
                                     localPosition + AmmoSlotsLoadOneButtonPos + new Vector2(93 * i, 0),
                                     (AmmoSlotsLoadOne[i] ? new Rectangle(1350, 14, 15, 14) : new Rectangle(1350, 0, 15, 14)),
                                     Color.White);
                }
                ClearAmmoSlots();
                // Draw Slots Background
                if (pickedItem != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (mousePos.X > AmmoSlotsIconPos.X - pickedItemOffset.X + (93 * i) &&
                            mousePos.X < AmmoSlotsIconPos.X - pickedItemOffset.X + 50 + (93 * i) &&
                            mousePos.Y > AmmoSlotsIconPos.Y - pickedItemOffset.Y &&
                            mousePos.Y < AmmoSlotsIconPos.Y - pickedItemOffset.Y + 50)
                        {
                            if (!CheckEm(pickedItem, i))
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + AmmoSlotsIconPos + new Vector2(93 * i,0), new Rectangle(1450, 0, 50, 50), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + AmmoSlotsIconPos + new Vector2(93 * i, 0), new Rectangle(1400, 0, 50, 50), Color.White);
                            }
                        }
                    }
                }
                // Draw Slots                
                for (int i = 0; i < 4; i++)
                {
                    if (AmmoSlots[i] == null) continue;                                                          

                    spriteBatch.Draw(frontEnd.Texture,
                                     localPosition + AmmoSlotsIconPos + new Vector2(93 * i, 0),
                                     (AmmoSlots[i] as StorableObject).Icon,
                                     Color.White);

                    DrawValue(spriteBatch, AmmoSlots[i] as StorableObject, localPosition + AmmoSlotsIconPos + new Vector2(93 * i, 0) + new Vector2(50, 50));

                    if (AmmoSlots[i].GetType().Equals(typeof(AmmoBox)))
                    {
                        String version = AmmunitionVersionInfo.VersionToString((AmmoSlots[i] as AmmoBox).AmmunitionVersionInfo.Version);
                        spriteBatch.DrawString(AmmoFont, version, localPosition + new Vector2(480 - AmmoFont.MeasureString(version).X / 2 + (93 * i), 530), Color.LightGray);
                    }
                    else
                    {
                        if ((AmmoSlots[i] as AmmoClip).Content.Count != 0)
                        {
                            String version = AmmunitionVersionInfo.VersionToString((AmmoSlots[i] as AmmoClip).Content.Peek().Version);
                            spriteBatch.DrawString(AmmoFont, version, localPosition + new Vector2(480 - AmmoFont.MeasureString(version).X / 2 + (93 * i), 530), Color.LightGray);
                        }
                    }
                }
                // Draw Bullet
                if (CurrentAmmoClip.Content.Count > 0)
                {
                    spriteBatch.Draw(frontEnd.Texture,
                                     localPosition + BulletIconPos,
                                     GetBulletIcon(CurrentAmmoClip.AmmunitionInfo.Genre,CurrentAmmoClip.Content.Peek().Version),
                                     Color.White);

                    String version = AmmunitionVersionInfo.VersionToString(CurrentAmmoClip.Content.Peek().Version);

                    spriteBatch.DrawString(AmmoFont, version, localPosition + new Vector2(711, 364), Color.LightGray);                       
                }
            }
            /***********************/
            #endregion
            /***********************/


            /***********************/
            #region Draw Slots
            /***********************/
            StorableObject currenItem = null;
            Firearm weapon = null;
                /***********************/
                #region Slots Background
                /***********************/        
                if (pickedItem != null)
                {
                    if(mousePos.X > CurrItemIconPos.X - pickedItemOffset.X      &&
                       mousePos.X < CurrItemIconPos.X - pickedItemOffset.X + 50 &&
                       mousePos.Y > CurrItemIconPos.Y - pickedItemOffset.Y      &&
                       mousePos.Y < CurrItemIconPos.Y - pickedItemOffset.Y + 50)
                    {
                        currenItem = mercenary.CurrentObject;    

                        if (currenItem != null)
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + CurrItemIconPos, new Rectangle(1450, 0, 50, 50), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + CurrItemIconPos, new Rectangle(1400, 0, 50, 50), Color.White);
                        }
                    }
                    else if (mousePos.X > WeaponIconPos.X - pickedItemOffset.X      &&
                             mousePos.X < WeaponIconPos.X - pickedItemOffset.X + 50 &&
                             mousePos.Y > WeaponIconPos.Y - pickedItemOffset.Y      &&
                             mousePos.Y < WeaponIconPos.Y - pickedItemOffset.Y + 50)
                    {
                        weapon = mercenary.Weapon;
                        Firearm pickedFirearm = pickedItem as Firearm;
                        if(pickedFirearm != null)
                        {
                            if (weapon != null || pickedFirearm.SideArm)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + WeaponIconPos, new Rectangle(1450, 0, 50, 50), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + WeaponIconPos, new Rectangle(1400, 0, 50, 50), Color.White);
                            }
                        }
                        else
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + WeaponIconPos, new Rectangle(1450, 0, 50, 50), Color.White);
                        }
                    }
                    else if (mousePos.X > SideArmIconPos.X - pickedItemOffset.X &&
                             mousePos.X < SideArmIconPos.X - pickedItemOffset.X + 50 &&
                             mousePos.Y > SideArmIconPos.Y - pickedItemOffset.Y &&
                             mousePos.Y < SideArmIconPos.Y - pickedItemOffset.Y + 50)
                    {
                        weapon = mercenary.SideArm;
                        Firearm pickedFirearm = pickedItem as Firearm;
                        if (pickedFirearm != null)
                        {
                            if (weapon != null || !pickedFirearm.SideArm)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SideArmIconPos, new Rectangle(1450, 0, 50, 50), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SideArmIconPos, new Rectangle(1400, 0, 50, 50), Color.White);
                            }
                        }
                        else
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + SideArmIconPos, new Rectangle(1450, 0, 50, 50), Color.White);
                        }
                    }

                    if (mercenary2 != null)
                    {
                        if (mousePos.X > CurrItemIconPos2.X - pickedItemOffset.X &&
                            mousePos.X < CurrItemIconPos2.X - pickedItemOffset.X + 50 &&
                            mousePos.Y > CurrItemIconPos2.Y - pickedItemOffset.Y &&
                            mousePos.Y < CurrItemIconPos2.Y - pickedItemOffset.Y + 50)
                        {
                            currenItem = mercenary2.CurrentObject;

                            if (currenItem != null)
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + CurrItemIconPos2, new Rectangle(1450, 0, 50, 50), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + CurrItemIconPos2, new Rectangle(1400, 0, 50, 50), Color.White);
                            }
                        }
                        else if (mousePos.X > WeaponIconPos2.X - pickedItemOffset.X &&
                                 mousePos.X < WeaponIconPos2.X - pickedItemOffset.X + 50 &&
                                 mousePos.Y > WeaponIconPos2.Y - pickedItemOffset.Y &&
                                 mousePos.Y < WeaponIconPos2.Y - pickedItemOffset.Y + 50)
                        {
                            weapon = mercenary2.Weapon;
                            Firearm pickedFirearm = pickedItem as Firearm;
                            if (pickedFirearm != null)
                            {
                                if (weapon != null || pickedFirearm.SideArm)
                                {
                                    spriteBatch.Draw(frontEnd.Texture, localPosition + WeaponIconPos2, new Rectangle(1450, 0, 50, 50), Color.White);
                                }
                                else
                                {
                                    spriteBatch.Draw(frontEnd.Texture, localPosition + WeaponIconPos2, new Rectangle(1400, 0, 50, 50), Color.White);
                                }
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + WeaponIconPos2, new Rectangle(1450, 0, 50, 50), Color.White);
                            }
                        }
                        else if (mousePos.X > SideArmIconPos2.X - pickedItemOffset.X &&
                                 mousePos.X < SideArmIconPos2.X - pickedItemOffset.X + 50 &&
                                 mousePos.Y > SideArmIconPos2.Y - pickedItemOffset.Y &&
                                 mousePos.Y < SideArmIconPos2.Y - pickedItemOffset.Y + 50)
                        {
                            weapon = mercenary2.SideArm;
                            Firearm pickedFirearm = pickedItem as Firearm;
                            if (pickedFirearm != null)
                            {
                                if (weapon != null || !pickedFirearm.SideArm)
                                {
                                    spriteBatch.Draw(frontEnd.Texture, localPosition + SideArmIconPos2, new Rectangle(1450, 0, 50, 50), Color.White);
                                }
                                else
                                {
                                    spriteBatch.Draw(frontEnd.Texture, localPosition + SideArmIconPos2, new Rectangle(1400, 0, 50, 50), Color.White);
                                }
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + SideArmIconPos2, new Rectangle(1450, 0, 50, 50), Color.White);
                            }
                        }
                    }
                }
                /***********************/
                #endregion
                /***********************/                
                #region Slots Content
                /***********************/
                // Current Item            
                currenItem = mercenary.CurrentObject;
                if (currenItem != null)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + CurrItemIconPos, currenItem.Icon, CheckAmmunitionInfo(currenItem) ? Color.White : Color.Black);
                    DrawValue(spriteBatch, currenItem, localPosition + CurrItemIconPos + new Vector2(50, 50));
                }
                // Weapon
                weapon = mercenary.Weapon;
                if (weapon != null)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + WeaponIconPos, weapon.Icon, CheckAmmunitionInfo(weapon) ? Color.White : Color.Black);
                    if (weapon.AmmoClip != null) DrawValue(spriteBatch, weapon.AmmoClip, localPosition + WeaponIconPos + new Vector2(50, 50));
                }
                // Side Arm
                weapon = mercenary.SideArm;
                if (weapon != null)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + SideArmIconPos, weapon.Icon, CheckAmmunitionInfo(weapon) ? Color.White : Color.Black);
                    if (weapon.AmmoClip != null) DrawValue(spriteBatch, weapon.AmmoClip, localPosition + SideArmIconPos + new Vector2(50, 50));
                }
                if (mercenary2 != null)
                {
                    // Current Item
                    currenItem = mercenary2.CurrentObject;
                    if (currenItem != null)
                    {
                        spriteBatch.Draw(frontEnd.Texture, localPosition + CurrItemIconPos2, currenItem.Icon, CheckAmmunitionInfo(currenItem) ? Color.White : Color.Black);
                        DrawValue(spriteBatch, currenItem, localPosition + CurrItemIconPos2 + new Vector2(50, 50));
                    }
                    // Weapon
                    weapon = mercenary2.Weapon;
                    if (weapon != null)
                    {
                        spriteBatch.Draw(frontEnd.Texture, localPosition + WeaponIconPos2, weapon.Icon, CheckAmmunitionInfo(weapon) ? Color.White : Color.Black);
                        if (weapon.AmmoClip != null) DrawValue(spriteBatch, weapon.AmmoClip, localPosition + WeaponIconPos2 + new Vector2(50, 50));
                    }
                    // Side Arm
                    weapon = mercenary2.SideArm;
                    if (weapon != null)
                    {
                        spriteBatch.Draw(frontEnd.Texture, localPosition + SideArmIconPos2, weapon.Icon, CheckAmmunitionInfo(weapon) ? Color.White : Color.Black);
                        if (weapon.AmmoClip != null) DrawValue(spriteBatch, weapon.AmmoClip, localPosition + SideArmIconPos2 + new Vector2(50, 50));
                    }
                }
                /***********************/
                #endregion
                /***********************/                
            /***********************/
            #endregion
            /***********************/                                          


            /***********************/
            #region Draw Description
            /***********************/
            if (ItemDesc)
            {
                // Background
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(420, 310), new Rectangle(840, 0, 420, 310), Color.White);
                // Exit Button
                spriteBatch.Draw(frontEnd.Texture, localPosition + ItemDescExitButtonPos, (ItemDescExit ? new Rectangle(1260, 14, 15, 14) : new Rectangle(1260, 0, 15, 14)), Color.White);
                // Draw Item                
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(595,345), DescribedItem.Icon, Color.White);
                spriteBatch.DrawString(AmmoFont, DescribedItem.Name, localPosition + new Vector2(620 - AmmoFont.MeasureString(DescribedItem.Name).X / 2, 310), Color.LightGray);
                // Draw Description
                if (!String.IsNullOrEmpty(DescribedItem.Description))
                {
                    spriteBatch.DrawString(AmmoFont, DescribedItem.Description, localPosition + new Vector2(430, 440), Color.LightGray);
                }
                else
                {
                    spriteBatch.DrawString(AmmoFont, "...", localPosition + new Vector2(430, 440), Color.LightGray);
                }
            }
            /***********************/
            #endregion
            /***********************/


            /***********************/
            #region Firearm
            /***********************/
            if (FirearmWindow)
            {
                // Background
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(420, 310), new Rectangle(840, 0, 420, 310), Color.White);
                // Exit Button
                spriteBatch.Draw(frontEnd.Texture, localPosition + FirearmExitButtonPos, (FirearmWindowExit ? new Rectangle(1260, 14, 15, 14) : new Rectangle(1260, 0, 15, 14)), Color.White);
                // Draw Item                
                spriteBatch.Draw(frontEnd.Texture, localPosition + FirearmIconPos, CurrentFirearm.Icon, Color.White);
                spriteBatch.DrawString(AmmoFont, CurrentFirearm.Name, localPosition + new Vector2(620 - AmmoFont.MeasureString(CurrentFirearm.Name).X / 2, 310), Color.LightGray);
                // Draw Slots Background
                if (pickedItem != null)
                {
                    if (mousePos.X > FirearmAmmoClipIconPos.X - pickedItemOffset.X &&
                        mousePos.X < FirearmAmmoClipIconPos.X - pickedItemOffset.X + 50 &&
                        mousePos.Y > FirearmAmmoClipIconPos.Y - pickedItemOffset.Y &&
                        mousePos.Y < FirearmAmmoClipIconPos.Y - pickedItemOffset.Y + 50)
                    {
                        if (!CheckCompability(pickedItem as AmmoClip) && !CheckAmmo(pickedItem as AmmoBox,CurrentFirearm.AmmoClip))
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + FirearmAmmoClipIconPos, new Rectangle(1450, 0, 50, 50), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + FirearmAmmoClipIconPos, new Rectangle(1400, 0, 50, 50), Color.White);
                        }
                    }                    
                }                
                // Draw Ammo Clip
                spriteBatch.Draw(frontEnd.Texture, localPosition + FirearmAmmoClipIconPos + new Vector2(-12,-4), new Rectangle(1694,310,75,80) ,Color.White);
                spriteBatch.DrawString(AmmoFont, "Ammo", localPosition + FirearmAmmoClipIconPos + new Vector2(25 - AmmoFont.MeasureString("Ammo").X / 2, 51), Color.LightGray);
                if (CurrentFirearm.AmmoClip != null)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + FirearmAmmoClipIconPos, CurrentFirearm.AmmoClip.Icon, Color.White);
                    DrawValue(spriteBatch, CurrentFirearm.AmmoClip, localPosition + FirearmAmmoClipIconPos + new Vector2(50,50));
                }
                // Draw Accessories Bg
                if (pickedItem != null)
                {
                    for (int i = 0; i < CurrentFirearm.Accessories.Length; i++)
                    {
                        if (mousePos.X > AccessoriesIconPos.X - pickedItemOffset.X + (80 * i) &&
                            mousePos.X < AccessoriesIconPos.X - pickedItemOffset.X + 50 + (80 * i) &&
                            mousePos.Y > AccessoriesIconPos.Y - pickedItemOffset.Y &&
                            mousePos.Y < AccessoriesIconPos.Y - pickedItemOffset.Y + 50)
                        {
                            if (!CheckAccessoryCompability(pickedItem as Accessory, i))
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + AccessoriesIconPos + new Vector2(80 * i, 0), new Rectangle(1450, 0, 50, 50), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(frontEnd.Texture, localPosition + AccessoriesIconPos + new Vector2(80 * i, 0), new Rectangle(1400, 0, 50, 50), Color.White);
                            }
                        }
                    }
                }
                // Draw Accessories
                for(int i = 0; i < CurrentFirearm.Accessories.Length; i++)
                {
                    spriteBatch.Draw(frontEnd.Texture, localPosition + AccessoriesIconPos + new Vector2(-12 + 80 * i,-4), new Rectangle(1694, 310, 75, 80), Color.White);
                    spriteBatch.DrawString(AmmoFont, CurrentFirearm.Accessories[i].Genre, localPosition + AccessoriesIconPos + new Vector2(25 - (AmmoFont.MeasureString(CurrentFirearm.Accessories[i].Genre).X / 2) + (80 * i), 51), Color.LightGray);

                    if (CurrentFirearm.Accessories[i].Accessory != null)
                    {
                        spriteBatch.Draw(frontEnd.Texture, localPosition + AccessoriesIconPos + new Vector2(80 * i, 0), CurrentFirearm.Accessories[i].Accessory.Icon, Color.White);
                    }
                }

                // Draw Description
                if (!String.IsNullOrEmpty(CurrentFirearm.Description))
                {
                    spriteBatch.DrawString(AmmoFont, GlobalGameObjects.StringManager.Load<string>(CurrentFirearm.Description), localPosition + new Vector2(430, 440), Color.LightGray);
                }
                else
                {
                    spriteBatch.DrawString(AmmoFont, "...", localPosition + new Vector2(430, 440), Color.LightGray);
                }
            }
            /***********************/
            #endregion
            /***********************/


            /***********************/
            #region Draw Picked Item
            /***********************/
            if (pickedItem != null)
            {
                if (newPickedItemOrientation > 0)
                {
                    spriteBatch.Draw(frontEnd.Texture,
                                     realMousePos - new Vector2(pickedItem.SlotsIcon.Width / 2,
                                                                pickedItem.SlotsIcon.Height / 2),
                                    pickedItem.SlotsIcon,
                                    Color.White);
                }
                else
                {
                    spriteBatch.Draw(frontEnd.Texture,
                                     realMousePos + new Vector2(pickedItem.SlotsIcon.Height / 2,
                                                                -pickedItem.SlotsIcon.Width / 2),
                                     pickedItem.SlotsIcon,
                                     Color.White,
                                     MathHelper.PiOver2,
                                     new Vector2(0, 0),
                                     1,
                                     SpriteEffects.None,
                                     0);
                }
            }
            /***********************/
            #endregion
            /***********************/


            /***********************/
            #region Draw Info
            /***********************/
            if (pickedItem == null)
            {
                String Info = String.Empty;
                ammunitionInfo = null;
                /******************************/
                #region West Side (Bitch!)
                /******************************/
                if (mousePos.X > SlotsStartPos.X &&
                    mousePos.X < SlotsStartPos.X + 11 * 32 &&
                    mousePos.Y > SlotsStartPos.Y &&
                    mousePos.Y < SlotsStartPos.Y + (Items.GetLength(1) < 15 ? Items.GetLength(1) : 15) * 32)
                {
                    int x = (int)((mousePos.X - SlotsStartPos.X) / 32);
                    int y = (int)((mousePos.Y - SlotsStartPos.Y) / 32) + scrollCurrentOffset;

                    if (Items[x, y].Item != null) Info = Items[x, y].Item.GetInfo();
                    SetAmmunitionInfo(Items[x, y].Item);
                }
                /******************************/
                #endregion
                /******************************/
                #region East Side
                /******************************/
                else if (
                    mercenary2 != null &&
                    mousePos.X > SlotsStartPos2.X &&
                    mousePos.X < SlotsStartPos2.X + 11 * 32 &&
                    mousePos.Y > SlotsStartPos2.Y &&
                    mousePos.Y < SlotsStartPos2.Y + (Items2.GetLength(1) < 15 ? Items2.GetLength(1) : 15) * 32)
                {
                    int x = (int)((mousePos.X - SlotsStartPos2.X) / 32);
                    int y = (int)((mousePos.Y - SlotsStartPos2.Y) / 32) + scrollCurrentOffset2;

                    if (Items2[x, y].Item != null) Info = Items2[x, y].Item.GetInfo();
                    SetAmmunitionInfo(Items2[x, y].Item);
                }
                /******************************/
                #endregion
                /******************************/
                #region Dump
                /******************************/
                else if (dump &&
                         mousePos.X > DumpSlotsStartPos.X &&
                         mousePos.X < DumpSlotsStartPos.X + 11 * 32 &&
                         mousePos.Y > DumpSlotsStartPos.Y &&
                         mousePos.Y < DumpSlotsStartPos.Y + (dumpContent.GetLength(1) < 8 ? dumpContent.GetLength(1) : 8) * 32)
                {
                    int x = (int)((mousePos.X - DumpSlotsStartPos.X) / 32);
                    int y = (int)((mousePos.Y - DumpSlotsStartPos.Y) / 32) + dumpScrollCurrentOffset;

                    if (dumpContent[x, y].Item != null) Info = dumpContent[x, y].Item.GetInfo();
                    SetAmmunitionInfo(dumpContent[x, y].Item);
                }
                else if (FirearmWindow && 
                    mousePos.X > FirearmAmmoClipIconPos.X &&
                    mousePos.X < FirearmAmmoClipIconPos.X + 50 &&
                    mousePos.Y > FirearmAmmoClipIconPos.Y  &&
                    mousePos.Y < FirearmAmmoClipIconPos.Y + 50)
                {
                    if (CurrentFirearm.AmmoClip != null) Info = CurrentFirearm.AmmoClip.GetInfo();
                    SetAmmunitionInfo(CurrentFirearm.AmmoClip);
                }
                else if (FirearmWindow && mousePos.X > 420 && mousePos.Y > 310)
                {

                    for (int i = 0; i < CurrentFirearm.Accessories.Length; i++)
                    {
                        if (CurrentFirearm.Accessories[i].Accessory != null &&
                            mousePos.X > AccessoriesIconPos.X + (80 * i) &&
                            mousePos.X < AccessoriesIconPos.X + 50 + (80 * i) &&
                            mousePos.Y > AccessoriesIconPos.Y &&
                            mousePos.Y < AccessoriesIconPos.Y + 50)
                        {
                            Info = CurrentFirearm.Accessories[i].Accessory.GetInfo();
                            break;
                        }
                    }
                }
                /******************************/
                #endregion
                /******************************/
                #region Slots
                /******************************/
                else if (mousePos.X > CurrItemIconPos.X &&
                         mousePos.X < CurrItemIconPos.X + 50 &&
                         mousePos.Y > CurrItemIconPos.Y &&
                         mousePos.Y < CurrItemIconPos.Y + 50)
                {
                    currenItem = mercenary.CurrentObject;
                    if (currenItem != null) Info = currenItem.GetInfo();
                    SetAmmunitionInfo(currenItem);
                }
                else if (mousePos.X > WeaponIconPos.X &&
                         mousePos.X < WeaponIconPos.X + 50 &&
                         mousePos.Y > WeaponIconPos.Y &&
                         mousePos.Y < WeaponIconPos.Y + 50)
                {
                    weapon = mercenary.Weapon;
                    if (weapon != null) Info = weapon.GetInfo();
                    SetAmmunitionInfo(weapon);

                }
                else if (mousePos.X > SideArmIconPos.X &&
                         mousePos.X < SideArmIconPos.X + 50 &&
                         mousePos.Y > SideArmIconPos.Y &&
                         mousePos.Y < SideArmIconPos.Y + 50)
                {
                    weapon = mercenary.SideArm;
                    if (weapon != null) Info = weapon.GetInfo();
                    SetAmmunitionInfo(weapon);
                }

                if (mercenary2 != null)
                {
                    if (mousePos.X > CurrItemIconPos2.X &&
                        mousePos.X < CurrItemIconPos2.X + 50 &&
                        mousePos.Y > CurrItemIconPos2.Y &&
                        mousePos.Y < CurrItemIconPos2.Y + 50)
                    {
                        currenItem = mercenary2.CurrentObject;
                        if (currenItem != null) Info = currenItem.GetInfo();
                        SetAmmunitionInfo(currenItem);
                    }
                    else if (mousePos.X > WeaponIconPos2.X &&
                             mousePos.X < WeaponIconPos2.X + 50 &&
                             mousePos.Y > WeaponIconPos2.Y &&
                             mousePos.Y < WeaponIconPos2.Y + 50)
                    {
                        weapon = mercenary2.Weapon;
                        if (weapon != null) Info = weapon.GetInfo();
                        SetAmmunitionInfo(weapon);
                    }
                    else if (mousePos.X > SideArmIconPos2.X &&
                             mousePos.X < SideArmIconPos2.X + 50 &&
                             mousePos.Y > SideArmIconPos2.Y &&
                             mousePos.Y < SideArmIconPos2.Y + 50)
                    {
                        weapon = mercenary2.SideArm;
                        if (weapon != null) Info = weapon.GetInfo();
                        SetAmmunitionInfo(weapon);
                    }
                }
                /******************************/
                #endregion
                /******************************/

                // Drawing Black Border // LOL!
                spriteBatch.DrawString(AmmoFont, Info, realMousePos + new Vector2(1, 32), Color.Black);
                spriteBatch.DrawString(AmmoFont, Info, realMousePos + new Vector2(0, 31), Color.Black);
                spriteBatch.DrawString(AmmoFont, Info, realMousePos + new Vector2(-1, 32), Color.Black);
                spriteBatch.DrawString(AmmoFont, Info, realMousePos + new Vector2(0, 33), Color.Black);

                spriteBatch.DrawString(AmmoFont, Info, realMousePos + new Vector2(0, 32), Color.LightGray);
            }
            /***********************/
            #endregion
            /***********************/
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Key
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ref ExtendedMouseKeyState mouseKeyState)
        {
            if (mouseKeyAction == MouseKeyAction.LeftClick)
            {
                if (mouseKeyState.WasPressed())
                {
                    /*************************/
                    #region Scroll Button
                    /*************************/
                    if (mousePos.X > ScrollButtonBasePos.X &&
                        mousePos.X < ScrollButtonBasePos.X + 15 &&
                        mousePos.Y > ScrollButtonBasePos.Y + scrollStep * scrollCurrentOffset &&
                        mousePos.Y < ScrollButtonBasePos.Y + 14 + scrollStep * scrollCurrentOffset)
                    {
                        scroll = true;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Scroll 2 Button
                    /*************************/
                    else if (mousePos.X > Scroll2ButtonBasePos.X &&
                        mousePos.X < Scroll2ButtonBasePos.X + 15 &&
                        mousePos.Y > Scroll2ButtonBasePos.Y + scrollStep2 * scrollCurrentOffset2 &&
                        mousePos.Y < Scroll2ButtonBasePos.Y + 14 + scrollStep2 * scrollCurrentOffset2)
                    {
                        scroll2 = true;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Dump Scroll Button
                    /*************************/
                    else if (mousePos.X > DumpScrollButtonBasePos.X &&
                             mousePos.X < DumpScrollButtonBasePos.X + 15 &&
                             mousePos.Y > DumpScrollButtonBasePos.Y +      dumpScrollStep * dumpScrollCurrentOffset &&
                             mousePos.Y < DumpScrollButtonBasePos.Y + 14 + dumpScrollStep * dumpScrollCurrentOffset)
                    {
                        dumpScroll = true;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Pick Current Item
                    /*************************/
                    else if (mousePos.X > CurrItemIconPos.X &&
                             mousePos.X < CurrItemIconPos.X + 50 &&
                             mousePos.Y > CurrItemIconPos.Y &&
                             mousePos.Y < CurrItemIconPos.Y + 50)
                    {
                        pickedItem = mercenary.CurrentObject;
                        if (pickedItem != null)
                        {
                            pickedItemSlot = Slot.CurrentItem;
                            mercenary.StoreItem(0);
                            mouse.CursorVisible = false;
                            newPickedItemOrientation = 1;
                            oldPickedItemOrientation = 1;
                            pickedFromMerc2 = false;
                        }
                    }
                    else if (mercenary2 != null &&
                             mousePos.X > CurrItemIconPos2.X &&
                             mousePos.X < CurrItemIconPos2.X + 50 &&
                             mousePos.Y > CurrItemIconPos2.Y &&
                             mousePos.Y < CurrItemIconPos2.Y + 50)
                    {
                        pickedItem = mercenary2.CurrentObject;
                        if (pickedItem != null)
                        {
                            pickedItemSlot = Slot.CurrentItem;
                            mercenary2.StoreItem(0);
                            mouse.CursorVisible = false;
                            newPickedItemOrientation = 1;
                            oldPickedItemOrientation = 1;
                            pickedFromMerc2 = true;
                        }
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Pick Weapon
                    /*************************/
                    else if (mousePos.X > WeaponIconPos.X &&
                             mousePos.X < WeaponIconPos.X + 50 &&
                             mousePos.Y > WeaponIconPos.Y &&
                             mousePos.Y < WeaponIconPos.Y + 50)
                    {
                        pickedItem = mercenary.Weapon;
                        if (pickedItem != null)
                        {
                            pickedItemSlot = Slot.Weapon;
                            mercenary.StoreItem(1);
                            mouse.CursorVisible = false;
                            newPickedItemOrientation = 1;
                            oldPickedItemOrientation = 1;
                            pickedFromMerc2 = false;
                        }
                    }
                    else if (mercenary2 != null &&
                             mousePos.X > WeaponIconPos2.X &&
                             mousePos.X < WeaponIconPos2.X + 50 &&
                             mousePos.Y > WeaponIconPos2.Y &&
                             mousePos.Y < WeaponIconPos2.Y + 50)
                    {
                        pickedItem = mercenary2.Weapon;
                        if (pickedItem != null)
                        {
                            pickedItemSlot = Slot.Weapon;
                            mercenary2.StoreItem(1);
                            mouse.CursorVisible = false;
                            newPickedItemOrientation = 1;
                            oldPickedItemOrientation = 1;
                            pickedFromMerc2 = true;
                        }
                    }
                    /*************************/
                    #endregion
                    /*************************/                    
                    #region Pick SideArm
                    /*************************/
                    else if (mousePos.X > SideArmIconPos.X &&
                             mousePos.X < SideArmIconPos.X + 50 &&
                             mousePos.Y > SideArmIconPos.Y &&
                             mousePos.Y < SideArmIconPos.Y + 50)
                    {
                        pickedItem = mercenary.SideArm;
                        if (pickedItem != null)
                        {
                            pickedItemSlot = Slot.SideArm;
                            mercenary.StoreItem(2);
                            mouse.CursorVisible = false;
                            newPickedItemOrientation = 1;
                            oldPickedItemOrientation = 1;
                            pickedFromMerc2 = false;
                        }
                    }
                    else if (mercenary2 != null &&
                             mousePos.X > SideArmIconPos2.X &&
                             mousePos.X < SideArmIconPos2.X + 50 &&
                             mousePos.Y > SideArmIconPos2.Y &&
                             mousePos.Y < SideArmIconPos2.Y + 50)
                    {
                        pickedItem = mercenary2.SideArm;
                        if (pickedItem != null)
                        {
                            pickedItemSlot = Slot.SideArm;
                            mercenary2.StoreItem(2);
                            mouse.CursorVisible = false;
                            newPickedItemOrientation = 1;
                            oldPickedItemOrientation = 1;
                            pickedFromMerc2 = true;
                        }
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Pick Item From Inventory
                    /*************************/
                    else if (mousePos.X > SlotsStartPos.X &&
                             mousePos.X < SlotsStartPos.X + 11 * 32 &&
                             mousePos.Y > SlotsStartPos.Y &&
                             mousePos.Y < SlotsStartPos.Y + (Items.GetLength(1) < 15 ? Items.GetLength(1) : 15) * 32)
                    {
                        int x = (int)((mousePos.X - SlotsStartPos.X) / 32);
                        int y = (int)((mousePos.Y - SlotsStartPos.Y) / 32) + scrollCurrentOffset;

                        if (Items[x, y].Item != null)
                        {
                            pickedItem = Items[x, y].Item;
                            List<int> slotsToClean = CalculateSlots(pickedItem, mercenary.Items[pickedItem].Slot, mercenary.Items[pickedItem].Orientation, true);
                            pickedItemSlots = CalculateSlots(pickedItem, mercenary.Items[pickedItem].Slot, mercenary.Items[pickedItem].Orientation, true);

                            oldPickedItemOrientation = mercenary.Items[pickedItem].Orientation;
                            newPickedItemOrientation = oldPickedItemOrientation;

                            mercenary.Items.Remove(pickedItem);

                            foreach (int slot in slotsToClean)
                            {
                                Items[slot % 11, slot / 11].Item = null;
                            }

                            pickedItemSlot = Slot.Inventory;
                            pickedFromMerc2 = false;
                            mouse.CursorVisible = false;
                        }
                    }
                    else if (mercenary2 != null &&
                             mousePos.X > SlotsStartPos2.X &&
                             mousePos.X < SlotsStartPos2.X + 11 * 32 &&
                             mousePos.Y > SlotsStartPos2.Y &&
                             mousePos.Y < SlotsStartPos2.Y + (Items2.GetLength(1) < 15 ? Items2.GetLength(1) : 15) * 32)
                    {
                        int x = (int)((mousePos.X - SlotsStartPos2.X) / 32);
                        int y = (int)((mousePos.Y - SlotsStartPos2.Y) / 32) + scrollCurrentOffset2;

                        if (Items2[x, y].Item != null)
                        {
                            pickedItem = Items2[x, y].Item;
                            List<int> slotsToClean = CalculateSlots(pickedItem, mercenary2.Items[pickedItem].Slot, mercenary2.Items[pickedItem].Orientation, true);
                            pickedItemSlots = CalculateSlots(pickedItem, mercenary2.Items[pickedItem].Slot, mercenary2.Items[pickedItem].Orientation, true);

                            oldPickedItemOrientation = mercenary2.Items[pickedItem].Orientation;
                            newPickedItemOrientation = oldPickedItemOrientation;

                            mercenary2.Items.Remove(pickedItem);

                            foreach (int slot in slotsToClean)
                            {
                                Items2[slot % 11, slot / 11].Item = null;
                            }

                            pickedItemSlot = Slot.Inventory;
                            pickedFromMerc2 = true;
                            mouse.CursorVisible = false;
                        }
                    }
                    else if (dump &&
                             mousePos.X > DumpSlotsStartPos.X &&
                             mousePos.X < DumpSlotsStartPos.X + 11 * 32 &&
                             mousePos.Y > DumpSlotsStartPos.Y &&
                             mousePos.Y < DumpSlotsStartPos.Y + (dumpContent.GetLength(1) < 8 ? dumpContent.GetLength(1) : 8) * 32)
                    {
                        int x = (int)((mousePos.X - DumpSlotsStartPos.X) / 32);
                        int y = (int)((mousePos.Y - DumpSlotsStartPos.Y) / 32) + dumpScrollCurrentOffset;

                        if (dumpContent[x, y].Item != null)
                        {
                            pickedItem = dumpContent[x, y].Item;
                            List<int> slotsToClean = CalculateSlots(pickedItem, dumpItems[pickedItem].Slot, dumpItems[pickedItem].Orientation, true);
                            pickedItemSlots = CalculateSlots(pickedItem, dumpItems[pickedItem].Slot, dumpItems[pickedItem].Orientation, true);

                            oldPickedItemOrientation = dumpItems[pickedItem].Orientation;
                            newPickedItemOrientation = oldPickedItemOrientation;

                            dumpItems.Remove(pickedItem);

                            foreach (int slot in slotsToClean)
                            {
                                dumpContent[slot % 11, slot / 11].Item = null;
                            }

                            pickedItemSlot = Slot.Dump;
                            mouse.CursorVisible = false;
                        }
                    }
                    else if (FirearmWindow && mousePos.X > 420 && mousePos.Y > 310)
                    {

                        if (mousePos.X > FirearmAmmoClipIconPos.X &&
                            mousePos.X < FirearmAmmoClipIconPos.X + 50 &&
                            mousePos.Y > FirearmAmmoClipIconPos.Y &&
                            mousePos.Y < FirearmAmmoClipIconPos.Y + 50)
                        {
                            if (CurrentFirearm.AmmoClip != null)
                            {
                                pickedItem = CurrentFirearm.AmmoClip;
                                CurrentFirearm.DetachClip();

                                oldPickedItemOrientation = 1;
                                newPickedItemOrientation = oldPickedItemOrientation;
                                
                                pickedItemSlot = Slot.AmmoClip;
                                mouse.CursorVisible = false;
                            }
                            return;
                        }


                        for (int i = 0; i < CurrentFirearm.Accessories.Length; i++)
                        {
                            if (mousePos.X > AccessoriesIconPos.X + (80 * i) &&
                               mousePos.X < AccessoriesIconPos.X + 50 + (80 * i) &&
                               mousePos.Y > AccessoriesIconPos.Y &&
                               mousePos.Y < AccessoriesIconPos.Y + 50)
                            {
                                if(CurrentFirearm.Accessories[i].Accessory != null)
                                {
                                    pickedItem = CurrentFirearm.Accessories[i].Accessory;
                                    CurrentFirearm.DetachAccessory(i);
                                    
                                    oldPickedItemOrientation = 1;
                                    newPickedItemOrientation = oldPickedItemOrientation;
                                    
                                    pickedFromSlot = i;
                                    pickedItemSlot = Slot.Accessories;
                                    mouse.CursorVisible = false;

                                }
                                return;
                            }
                        }
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Dump
                    /*************************/
                    else if (mousePos.X > DumpButtonPos.X &&
                             mousePos.X < DumpButtonPos.X + 19 &&
                             mousePos.Y > DumpButtonPos.Y &&
                             mousePos.Y < DumpButtonPos.Y + 52)
                    {
                        if (container != null || mercenary2 != null) return;

                        if (dump)
                        {
                            dump = false;
                            dumpButton = false;
                            CloseDump();
                        }
                        else
                        {
                            dump = true;
                            dumpButton = true;
                            PrepareDump();
                        }
                    }
                    /*************************/
                    #endregion
                    /*************************/
                }

                if (mouseKeyState.IsDown())
                {
                    /******************************/
                    #region Drag & Drop
                    /******************************/
                    if (pickedItem != null)
                    {
                        mousePos.X -= (pickedItem.SlotsIcon.Width / 2) - 32;
                        mousePos.Y -= (pickedItem.SlotsIcon.Height / 2) - 32;

                        /******************************/
                        #region West Side (Bitch!)
                        /******************************/
                        if (mousePos.X > SlotsStartPos.X &&
                            mousePos.X < SlotsStartPos.X + 11 * 32 &&
                            mousePos.Y > SlotsStartPos.Y &&
                            mousePos.Y < SlotsStartPos.Y + (Items.GetLength(1) < 15 ? Items.GetLength(1) : 15) * 32)
                        {
                            int x = (int)((mousePos.X - SlotsStartPos.X) / 32);
                            int y = (int)((mousePos.Y - SlotsStartPos.Y) / 32) + scrollCurrentOffset;

                            int width = (pickedItem.SlotsIcon.Width / 32) - 1;
                            int height = (pickedItem.SlotsIcon.Height / 32) - 1;

                            if (newPickedItemOrientation < 0)
                            {
                                int tmp = width;
                                width = height;
                                height = tmp;
                            }

                            bool blockAll = false;

                            uint onTiny = 0;
                            uint onNormal = 0;

                            if (pickedFromMerc2)
                            {
                                if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) > 3) blockAll = true;
                            }    

                            for (int i = x + width; i >= x; --i)
                            {
                                for (int j = y + height; j >= y; --j)
                                {                               
                                    if (i > Items.GetLength(0) - 1) blockAll = true;
                                    else if (j > Items.GetLength(1) - 1) blockAll = true;
                                    else if (blockAll)
                                    {
                                        Items[i, j].Blocked = true;
                                    }
                                    else if (Items[i, j].Blank)
                                    {
                                        blockAll = true;
                                    }
                                    else if (Items[i, j].Item != null)
                                    {
                                        Items[i, j].Blocked = true;
                                    }
                                    else if (Items[i, j].Tiny && (width > 1 || height > 1))
                                    {
                                        Items[i, j].Blocked = true;
                                    }
                                    else
                                    {
                                        if (Items[i, j].Tiny) onTiny++;
                                        else onNormal++;

                                        if (onTiny > 0 && onNormal > 0) Items[i, j].Blocked = true;
                                        else Items[i, j].Hover = true;
                                    }
                                }
                            }

                            if (y + height - scrollCurrentOffset > 13)
                            {
                                if (scrollCurrentOffset != scrollMaxOffset) scrollCurrentOffset++;
                            }
                            else if (y - scrollCurrentOffset < 1)
                            {
                                if (scrollCurrentOffset != 0) scrollCurrentOffset--;
                            }
                        }
                        /******************************/
                        #endregion
                        /******************************/
                        #region East Side
                        /******************************/
                        else if (
                            mercenary2 != null &&
                            mousePos.X > SlotsStartPos2.X &&
                            mousePos.X < SlotsStartPos2.X + 11 * 32 &&
                            mousePos.Y > SlotsStartPos2.Y &&
                            mousePos.Y < SlotsStartPos2.Y + (Items2.GetLength(1) < 15 ? Items2.GetLength(1) : 15) * 32)
                        {
                            int x = (int)((mousePos.X - SlotsStartPos2.X) / 32);
                            int y = (int)((mousePos.Y - SlotsStartPos2.Y) / 32) + scrollCurrentOffset2;

                            int width = (pickedItem.SlotsIcon.Width / 32) - 1;
                            int height = (pickedItem.SlotsIcon.Height / 32) - 1;

                            if (newPickedItemOrientation < 0)
                            {
                                int tmp = width;
                                width = height;
                                height = tmp;
                            }

                            bool blockAll = false;

                            uint onTiny = 0;
                            uint onNormal = 0;

                            if (!pickedFromMerc2)
                            {
                                if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) > 3) blockAll = true;
                            }   

                            for (int i = x + width; i >= x; --i)
                            {
                                for (int j = y + height; j >= y; --j)
                                {
                                    if (i > Items2.GetLength(0) - 1) blockAll = true;
                                    else if (j > Items2.GetLength(1) - 1) blockAll = true; 
                                    else if (blockAll)
                                    {
                                        Items2[i, j].Blocked = true;
                                    }
                                    else if (Items2[i, j].Blank)
                                    {
                                        blockAll = true;
                                    }
                                    else if (Items2[i, j].Item != null)
                                    {
                                        Items2[i, j].Blocked = true;
                                    }
                                    else if (Items2[i, j].Tiny && (width > 1 || height > 1))
                                    {
                                        Items2[i, j].Blocked = true;
                                    }
                                    else
                                    {
                                        if (Items2[i, j].Tiny) onTiny++;
                                        else onNormal++;

                                        if (onTiny > 0 && onNormal > 0) Items2[i, j].Blocked = true;
                                        else Items2[i, j].Hover = true;
                                    }
                                }
                            }

                            if (y + height - scrollCurrentOffset2 > 13)
                            {
                                if (scrollCurrentOffset2 != scrollMaxOffset2) scrollCurrentOffset2++;
                            }
                            else if (y - scrollCurrentOffset2 < 1)
                            {
                                if (scrollCurrentOffset2 != 0) scrollCurrentOffset2--;
                            }
                        }
                        /******************************/
                        #endregion
                        /******************************/
                        #region Dump
                        /******************************/
                        else if (dump &&                            
                                 mousePos.X > DumpSlotsStartPos.X &&
                                 mousePos.X < DumpSlotsStartPos.X + 11 * 32 &&
                                 mousePos.Y > DumpSlotsStartPos.Y &&
                                 mousePos.Y < DumpSlotsStartPos.Y + (dumpContent.GetLength(1) < 8 ? dumpContent.GetLength(1) : 8) * 32)
                        {
                            int x = (int)((mousePos.X - DumpSlotsStartPos.X) / 32);
                            int y = (int)((mousePos.Y - DumpSlotsStartPos.Y) / 32) + dumpScrollCurrentOffset;

                            int width  = (pickedItem.SlotsIcon.Width / 32) - 1;
                            int height = (pickedItem.SlotsIcon.Height / 32) - 1;

                            if (newPickedItemOrientation < 0)
                            {
                                int tmp = width;
                                width = height;
                                height = tmp;
                            }

                            bool blockAll = false;

                            for (int i = x + width; i >= x; --i)
                            {
                                for (int j = y + height; j >= y; --j)
                                {
                                    if (i > dumpContent.GetLength(0) - 1) blockAll = true;
                                    else if (j > dumpContent.GetLength(1) - 1) blockAll = true;
                                    else if (blockAll)
                                    {
                                        dumpContent[i, j].Blocked = true;
                                    }
                                    else if (dumpContent[i, j].Blank)
                                    {
                                        blockAll = true;
                                    }
                                    else if (dumpContent[i, j].Item != null)
                                    {
                                        dumpContent[i, j].Blocked = true;
                                    }
                                    else
                                    {
                                        dumpContent[i, j].Hover = true;
                                    }
                                }
                            }

                            if (y + height - dumpScrollCurrentOffset > 6)
                            {
                                if (dumpScrollCurrentOffset != dumpScrollMaxOffset) dumpScrollCurrentOffset++;
                            }
                            else if (y - dumpScrollCurrentOffset < 1)
                            {
                                if (dumpScrollCurrentOffset != 0) dumpScrollCurrentOffset--;
                            }
                        }
                        /******************************/
                        #endregion
                        /******************************/
                    }
                    /******************************/
                    #endregion
                    /******************************/
                    #region Pressed Buttons
                    /******************************/
                    else if (!scroll && !scroll2 && !dumpScroll)
                    {
                        UncheckButtons();

                        /*************************/
                        /// Next Merc Button
                        /*************************/
                        if (mousePos.X > NextMercButtonPos.X &&
                            mousePos.X < NextMercButtonPos.X + 15 &&
                            mousePos.Y > NextMercButtonPos.Y &&
                            mousePos.Y < NextMercButtonPos.Y + 14)
                        {
                            nextMerc = true;
                        }
                        /*************************/
                        /// Prev Merc Button
                        /*************************/
                        else if (mousePos.X > PrevMercButtonPos.X &&
                                 mousePos.X < PrevMercButtonPos.X + 15 &&
                                 mousePos.Y > PrevMercButtonPos.Y &&
                                 mousePos.Y < PrevMercButtonPos.Y + 14)
                        {
                            prevMerc = true;
                        }
                        /*************************/
                        /// Exit Inv Button
                        /*************************/
                        else if (mousePos.X > ExitInvButtonPos.X &&
                                 mousePos.X < ExitInvButtonPos.X + 15 &&
                                 mousePos.Y > ExitInvButtonPos.Y &&
                                 mousePos.Y < ExitInvButtonPos.Y + 14)
                        {
                            exitInv = true;
                        }
                        /*************************/
                        /// Scroll Up
                        /*************************/
                        else if (mousePos.X > ScrollUpButtonPos.X &&
                                 mousePos.X < ScrollUpButtonPos.X + 15 &&
                                 mousePos.Y > ScrollUpButtonPos.Y &&
                                 mousePos.Y < ScrollUpButtonPos.Y + 14)
                        {
                            scrollUp = true;
                        }
                        /*************************/
                        /// Scroll Down
                        /*************************/
                        else if (mousePos.X > ScrollDownButtonPos.X &&
                                 mousePos.X < ScrollDownButtonPos.X + 15 &&
                                 mousePos.Y > ScrollDownButtonPos.Y &&
                                 mousePos.Y < ScrollDownButtonPos.Y + 14)
                        {
                            scrollDown = true;
                        }
                        /*************************/
                        /// Exit Inv 2 Button
                        /*************************/
                        else if (mousePos.X > ExitInv2ButtonPos.X &&
                                 mousePos.X < ExitInv2ButtonPos.X + 15 &&
                                 mousePos.Y > ExitInv2ButtonPos.Y &&
                                 mousePos.Y < ExitInv2ButtonPos.Y + 14)
                        {
                            exitInv2 = true;
                        }
                        /*************************/
                        /// Scroll Up 2
                        /*************************/
                        else if (mousePos.X > ScrollUp2ButtonPos.X &&
                                 mousePos.X < ScrollUp2ButtonPos.X + 15 &&
                                 mousePos.Y > ScrollUp2ButtonPos.Y &&
                                 mousePos.Y < ScrollUp2ButtonPos.Y + 14)
                        {
                            scrollUp2 = true;
                        }
                        /*************************/
                        /// Scroll Down 2
                        /*************************/
                        else if (mousePos.X > ScrollDown2ButtonPos.X &&
                                 mousePos.X < ScrollDown2ButtonPos.X + 15 &&
                                 mousePos.Y > ScrollDown2ButtonPos.Y &&
                                 mousePos.Y < ScrollDown2ButtonPos.Y + 14)
                        {
                            scrollDown2 = true;
                        }
                        /*************************/
                        /// Dump Scroll Up
                        /*************************/
                        else if (mousePos.X > DumpScrollUpButtonPos.X &&
                                 mousePos.X < DumpScrollUpButtonPos.X + 15 &&
                                 mousePos.Y > DumpScrollUpButtonPos.Y &&
                                 mousePos.Y < DumpScrollUpButtonPos.Y + 14)
                        {
                            dumpScrollUp = true;
                        }
                        /*************************/
                        /// Dump Scroll Down 
                        /*************************/
                        else if (mousePos.X > DumpScrollDownButtonPos.X &&
                                 mousePos.X < DumpScrollDownButtonPos.X + 15 &&
                                 mousePos.Y > DumpScrollDownButtonPos.Y &&
                                 mousePos.Y < DumpScrollDownButtonPos.Y + 14)
                        {
                            dumpScrollDown = true;
                        }
                        /*************************/
                        /// Ammo Loader Exit
                        /*************************/
                        else if (AmmoLoader &&
                                 mousePos.X > AmmoLoaderExitButtonPos.X &&
                                 mousePos.X < AmmoLoaderExitButtonPos.X + 15 &&
                                 mousePos.Y > AmmoLoaderExitButtonPos.Y &&
                                 mousePos.Y < AmmoLoaderExitButtonPos.Y + 14)
                        {
                            AmmoLoaderExit = true;
                        }
                        /*************************/
                        /// Ammo Loader Unload
                        /*************************/
                        else if (AmmoLoader &&
                                 mousePos.X > AmmoLoaderUnloadButtonPos.X &&
                                 mousePos.X < AmmoLoaderUnloadButtonPos.X + 15 &&
                                 mousePos.Y > AmmoLoaderUnloadButtonPos.Y &&
                                 mousePos.Y < AmmoLoaderUnloadButtonPos.Y + 14)
                        {
                            AmmoLoaderUnload = true;
                        }
                        /*************************/
                        /// Ammo Loader Unload One
                        /*************************/
                        else if (AmmoLoader &&
                                 mousePos.X > AmmoLoaderUnloadOneButtonPos.X &&
                                 mousePos.X < AmmoLoaderUnloadOneButtonPos.X + 15 &&
                                 mousePos.Y > AmmoLoaderUnloadOneButtonPos.Y &&
                                 mousePos.Y < AmmoLoaderUnloadOneButtonPos.Y + 14)
                        {
                            AmmoUnloadOne = true;
                        }
                        /*************************/
                        /// Item Desc Exit
                        /*************************/
                        else if (ItemDesc &&
                                 mousePos.X > ItemDescExitButtonPos.X &&
                                 mousePos.X < ItemDescExitButtonPos.X + 15 &&
                                 mousePos.Y > ItemDescExitButtonPos.Y &&
                                 mousePos.Y < ItemDescExitButtonPos.Y + 14)
                        {
                            ItemDescExit = true;
                        }
                        /*************************/
                        /// Firearm Exit
                        /*************************/
                        else if (FirearmWindow &&
                                 mousePos.X > FirearmExitButtonPos.X &&
                                 mousePos.X < FirearmExitButtonPos.X + 15 &&
                                 mousePos.Y > FirearmExitButtonPos.Y &&
                                 mousePos.Y < FirearmExitButtonPos.Y + 14)
                        {
                            FirearmWindowExit = true;
                        }
                        else
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (mousePos.X > AmmoSlotsLoadButtonPos.X +      (93 * i) &&
                                    mousePos.X < AmmoSlotsLoadButtonPos.X + 15 + (93 * i) &&
                                    mousePos.Y > AmmoSlotsLoadButtonPos.Y                 &&
                                    mousePos.Y < AmmoSlotsLoadButtonPos.Y + 14)
                                {
                                    AmmoSlotsLoad[i] = true;
                                    return;
                                }
                                else if (mousePos.X > AmmoSlotsLoadOneButtonPos.X +      (93 * i) &&
                                         mousePos.X < AmmoSlotsLoadOneButtonPos.X + 15 + (93 * i) &&
                                         mousePos.Y > AmmoSlotsLoadOneButtonPos.Y                 &&
                                         mousePos.Y < AmmoSlotsLoadOneButtonPos.Y + 14)
                                {
                                    AmmoSlotsLoadOne[i] = true;
                                    return;
                                }
                            }                            
                        }
                    }
                    /******************************/
                    #endregion
                    /******************************/
                }

                else if (mouseKeyState.WasReleased())
                {
                    /*************************/
                    #region Drop Picked Item
                    /*************************/
                    if (pickedItem != null)
                    {
                        /*************************/
                        #region On Current Item
                        /*************************/
                        if (mousePos.X > CurrItemIconPos.X &&
                            mousePos.X < CurrItemIconPos.X + 50 &&
                            mousePos.Y > CurrItemIconPos.Y &&
                            mousePos.Y < CurrItemIconPos.Y + 50)
                        {
                            if (mercenary.CurrentObject == null)
                            {
                                if (pickedFromMerc2)
                                {
                                    if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) < 3)
                                    {
                                        mercenary.PlaceItem(pickedItem,0);
                                        pickedItem = null;
                                        mouse.CursorVisible = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    mercenary.PlaceItem(pickedItem, 0);
                                    pickedItem = null;
                                    mouse.CursorVisible = true;
                                    return;
                                }
                            }
                            PutBackPickedItem();        
                        }
                        else if (mercenary2 != null                   &&
                                 mousePos.X > CurrItemIconPos2.X      &&
                                 mousePos.X < CurrItemIconPos2.X + 50 &&
                                 mousePos.Y > CurrItemIconPos2.Y      &&
                                 mousePos.Y < CurrItemIconPos2.Y + 50)
                        {
                            if (mercenary2.CurrentObject == null)
                            {
                                if (!pickedFromMerc2)
                                {
                                    if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) < 3)
                                    {
                                        mercenary2.PlaceItem(pickedItem, 0);
                                        pickedItem = null;
                                        mouse.CursorVisible = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    mercenary2.PlaceItem(pickedItem, 0);
                                    pickedItem = null;
                                    mouse.CursorVisible = true;
                                    return;
                                }
                            }
                            PutBackPickedItem();  
                        }                        
                        /*************************/
                        #endregion
                        /*************************/                        
                        #region On Weapon
                        /*************************/
                        else if (mousePos.X > WeaponIconPos.X &&
                                 mousePos.X < WeaponIconPos.X + 50 &&
                                 mousePos.Y > WeaponIconPos.Y &&
                                 mousePos.Y < WeaponIconPos.Y + 50)
                        {
                            if (mercenary.Weapon == null)
                            {
                                Firearm weapon = pickedItem as Firearm;
                                if (weapon != null)
                                {
                                    if (!weapon.SideArm)
                                    {
                                        if (pickedFromMerc2)
                                        {
                                            if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) < 3)
                                            {
                                                mercenary.PlaceItem(pickedItem,1);
                                                pickedItem = null;
                                                mouse.CursorVisible = true;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            mercenary.PlaceItem(pickedItem, 1);
                                            pickedItem = null;
                                            mouse.CursorVisible = true;
                                            return;
                                        }
                                    }
                                }                                
                            }
                            PutBackPickedItem();
                        }
                        else if (mercenary2 != null &&
                                 mousePos.X > WeaponIconPos2.X &&
                                 mousePos.X < WeaponIconPos2.X + 50 &&
                                 mousePos.Y > WeaponIconPos2.Y &&
                                 mousePos.Y < WeaponIconPos2.Y + 50)
                        {
                            if (mercenary2.Weapon == null)
                            {
                                Firearm weapon = pickedItem as Firearm;
                                if (weapon != null)
                                {
                                    if (!weapon.SideArm)
                                    {
                                        if (!pickedFromMerc2)
                                        {
                                            if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) < 3)
                                            {
                                                mercenary2.PlaceItem(pickedItem, 1);
                                                pickedItem = null;
                                                mouse.CursorVisible = true;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            mercenary2.PlaceItem(pickedItem, 1);
                                            pickedItem = null;
                                            mouse.CursorVisible = true;
                                            return;
                                        }
                                    }
                                }
                            }
                            PutBackPickedItem();
                        }
                        /*************************/
                        #endregion
                        /*************************/
                        #region On Side Arm
                        /*************************/
                        else if (mousePos.X > SideArmIconPos.X &&
                                 mousePos.X < SideArmIconPos.X + 50 &&
                                 mousePos.Y > SideArmIconPos.Y &&
                                 mousePos.Y < SideArmIconPos.Y + 50)
                        {
                            if (mercenary.SideArm == null)
                            {
                                Firearm weapon = pickedItem as Firearm;
                                if (weapon != null)
                                {
                                    if (weapon.SideArm)
                                    {
                                        if (pickedFromMerc2)
                                        {
                                            if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) < 3)
                                            {
                                                mercenary.PlaceItem(pickedItem, 2);
                                                pickedItem = null;
                                                mouse.CursorVisible = true;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            mercenary.PlaceItem(pickedItem, 2);
                                            pickedItem = null;
                                            mouse.CursorVisible = true;
                                            return;
                                        }
                                    }
                                }
                            }
                            PutBackPickedItem();
                        }
                        else if (mercenary2 != null &&
                                 mousePos.X > SideArmIconPos2.X &&
                                 mousePos.X < SideArmIconPos2.X + 50 &&
                                 mousePos.Y > SideArmIconPos2.Y &&
                                 mousePos.Y < SideArmIconPos2.Y + 50)
                        {
                            if (mercenary2.SideArm == null)
                            {
                                Firearm weapon = pickedItem as Firearm;
                                if (weapon != null)
                                {
                                    if (weapon.SideArm)
                                    {
                                        if (!pickedFromMerc2)
                                        {
                                            if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) < 3)
                                            {
                                                mercenary2.PlaceItem(pickedItem, 2);
                                                pickedItem = null;
                                                mouse.CursorVisible = true;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            mercenary2.PlaceItem(pickedItem, 2);
                                            pickedItem = null;
                                            mouse.CursorVisible = true;
                                            return;
                                        }
                                    }
                                }
                            }
                            PutBackPickedItem();
                        }
                        /*************************/
                        #endregion
                        /*************************/
                        #region Ammo Slots 
                        /*************************/
                        else if (AmmoLoader &&
                                 mousePos.X > AmmoSlotsIconPos.X &&
                                 mousePos.X < AmmoSlotsIconPos.X + 50 &&
                                 mousePos.Y > AmmoSlotsIconPos.Y &&
                                 mousePos.Y < AmmoSlotsIconPos.Y + 50)
                        {
                            PutOnAmmoSlot(pickedItem, 0);
                            PutBackPickedItem();
                        }
                        else if (AmmoLoader &&
                                 mousePos.X > AmmoSlotsIconPos.X + 93      &&
                                 mousePos.X < AmmoSlotsIconPos.X + 50 + 93 &&
                                 mousePos.Y > AmmoSlotsIconPos.Y &&
                                 mousePos.Y < AmmoSlotsIconPos.Y + 50)
                        {
                            PutOnAmmoSlot(pickedItem, 1);
                            PutBackPickedItem();
                        }
                        else if (AmmoLoader &&
                                 mousePos.X > AmmoSlotsIconPos.X + 93 + 93      &&
                                 mousePos.X < AmmoSlotsIconPos.X + 50 + 93 + 93 &&
                                 mousePos.Y > AmmoSlotsIconPos.Y &&
                                 mousePos.Y < AmmoSlotsIconPos.Y + 50)
                        {
                            PutOnAmmoSlot(pickedItem, 2);
                            PutBackPickedItem();
                        }
                        else if (AmmoLoader &&
                                 mousePos.X > FirearmAmmoClipIconPos.X + 93 + 93 + 93  &&
                                 mousePos.X < FirearmAmmoClipIconPos.X + 50 + 93 + 93 + 93 &&
                                 mousePos.Y > FirearmAmmoClipIconPos.Y &&
                                 mousePos.Y < FirearmAmmoClipIconPos.Y + 50)
                        {
                            PutOnAmmoSlot(pickedItem, 3);
                            PutBackPickedItem();
                        }
                        /*************************/
                        #endregion
                        /*************************/
                        #region Firearm Ammo Clip
                        /*************************/
                        else if (FirearmWindow &&
                                 mousePos.X > FirearmAmmoClipIconPos.X &&
                                 mousePos.X < FirearmAmmoClipIconPos.X + 50  &&
                                 mousePos.Y > FirearmAmmoClipIconPos.Y &&
                                 mousePos.Y < FirearmAmmoClipIconPos.Y + 50)
                        {
                            if (CheckCompability(pickedItem as AmmoClip) && CurrentFirearm.AmmoClip == null)
                            {
                                CurrentFirearm.AttachClip(pickedItem as AmmoClip);
                                pickedItem = null;
                                mouse.CursorVisible = true;
                            }
                            else if ((pickedItem as AmmoBox) != null && CurrentFirearm.AmmoClip != null)
                            {
                                AmmoBox ammo = pickedItem as AmmoBox;
                                if (ammo.AmmunitionInfo == CurrentFirearm.AmmoClip.AmmunitionInfo &&
                                    CurrentFirearm.AmmoClip.Content.Count < CurrentFirearm.AmmoClip.Capacity)
                                {
                                    for (;ammo.Amount > 0 && CurrentFirearm.AmmoClip.Content.Count < CurrentFirearm.AmmoClip.Capacity; --ammo.Amount)
                                    {
                                        Bullet bullet = new Bullet();
                                        bullet.Version = ammo.AmmunitionVersionInfo.Version;
                                        bullet.PPP = ammo.PPP;
                                        CurrentFirearm.AmmoClip.Content.Push(bullet);
                                    }

                                    if (ammo.Amount == 0)
                                    {
                                        SendEvent(new DestroyObjectEvent(ammo.ID), Priority.High, GlobalGameObjects.GameController);
                                        UpdateInventory();
                                    }
                                    else
                                    {
                                        mercenary.FindPlaceForItem(ammo, true);
                                    }

                                    pickedItem = null;
                                    mouse.CursorVisible = true;
                                }
                            }
                            else PutBackPickedItem();
                        }
                        /*************************/
                        #endregion
                        /*************************/
                        #region Firearm Accessories
                        /*************************/
                        else if (FirearmWindow &&
                                 CurrentFirearm.Accessories.Length > 0 && 
                                 mousePos.X > AccessoriesIconPos.X &&
                                 mousePos.X < AccessoriesIconPos.X + 50  &&
                                 mousePos.Y > AccessoriesIconPos.Y &&
                                 mousePos.Y < AccessoriesIconPos.Y + 50)
                        {
                            if (CheckAccessoryCompability(pickedItem as Accessory, 0))
                            {
                                CurrentFirearm.AttachAccessory(pickedItem as Accessory, 0);                                
                                pickedItem = null;
                                mouse.CursorVisible = true;
                            }
                            else PutBackPickedItem();
                        }
                        else if (FirearmWindow &&
                                 CurrentFirearm.Accessories.Length > 1 &&
                                 mousePos.X > AccessoriesIconPos.X + 80 &&
                                 mousePos.X < AccessoriesIconPos.X + 50 + 80 &&
                                 mousePos.Y > AccessoriesIconPos.Y &&
                                 mousePos.Y < AccessoriesIconPos.Y + 50)
                        {
                            if (CheckAccessoryCompability(pickedItem as Accessory, 1))
                            {
                                CurrentFirearm.AttachAccessory(pickedItem as Accessory, 1);
                                pickedItem = null;
                                mouse.CursorVisible = true;
                            }
                            else PutBackPickedItem();
                        }
                        else if (FirearmWindow &&
                                 CurrentFirearm.Accessories.Length > 2 &&
                                 mousePos.X > AccessoriesIconPos.X + 80 + 80 &&
                                 mousePos.X < AccessoriesIconPos.X + 50 + 80 + 80 &&
                                 mousePos.Y > AccessoriesIconPos.Y &&
                                 mousePos.Y < AccessoriesIconPos.Y + 50)
                        {
                            if (CheckAccessoryCompability(pickedItem as Accessory, 2))
                            {
                                CurrentFirearm.AttachAccessory(pickedItem as Accessory, 2);
                                pickedItem = null;
                                mouse.CursorVisible = true;
                            }
                            else PutBackPickedItem();
                        }
                        /*************************/
                        #endregion
                        /*************************/
                        #region Outside Inventory
                        /*************************/
                        else if (realMousePos.X < localPosition.X ||
                                 realMousePos.Y < localPosition.Y ||
                                 realMousePos.Y > localPosition.Y + 620 ||
                                 realMousePos.X > localPosition.X + 840 ||
                                 (mercenary2 == null && !dump && !AmmoLoader && !ItemDesc && !FirearmWindow && realMousePos.X > localPosition.X + 420) ||
                                 (mercenary2 == null && dump && !AmmoLoader && !ItemDesc && !FirearmWindow && realMousePos.Y > localPosition.Y + 310 && realMousePos.X > localPosition.X + 420) ||
                                 (mercenary2 == null && (AmmoLoader || ItemDesc || FirearmWindow) && !dump && realMousePos.Y < localPosition.Y + 310 && realMousePos.X > localPosition.X + 420)
                                 )
                        {
                            mercenary.DropItem(pickedItem);
                            CheckAmmoSlots(pickedItem);
                            pickedItem = null;
                            pickedItemSlot = Slot.Empty;
                            mouse.CursorVisible = true;
                        }
                        /*************************/
                        #endregion
                        /*************************/
                        #region On Inventory
                        /*************************/
                        else
                        {
                            mousePos.X -= (pickedItem.SlotsIcon.Width / 2) - 32;
                            mousePos.Y -= (pickedItem.SlotsIcon.Height / 2) - 32;

                            /*************************/
                            #region Main
                            /*************************/
                            if (mousePos.X > SlotsStartPos.X &&
                                mousePos.X < SlotsStartPos.X + 11 * 32 &&
                                mousePos.Y > SlotsStartPos.Y &&
                                mousePos.Y < SlotsStartPos.Y + (Items.GetLength(1) < 15 ? Items.GetLength(1) : 15) * 32)
                            {
                                int x = (int)((mousePos.X - SlotsStartPos.X) / 32);
                                int y = (int)((mousePos.Y - SlotsStartPos.Y) / 32) + scrollCurrentOffset;

                                int width = (pickedItem.SlotsIcon.Width / 32) - 1;
                                int height = (pickedItem.SlotsIcon.Height / 32) - 1;

                                if (newPickedItemOrientation < 0)
                                {
                                    int tmp = width;
                                    width = height;
                                    height = tmp;
                                }

                                bool block = false;
                                uint onTiny = 0;
                                uint onNormal = 0;

                                if (pickedFromMerc2)
                                {
                                    if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) > 3) block = true;
                                }

                                for (int i = x + width; i >= x; --i)
                                {
                                    for (int j = y + height; j >= y; --j)
                                    {
                                        if (i > Items.GetLength(0) - 1) block = true;
                                        else if (j > Items.GetLength(1) - 1) block = true;
                                        else if (Items[i, j].Blank) block = true;
                                        else if (Items[i, j].Item != null) block = true;
                                        else if (Items[i, j].Tiny && (width > 1 || height > 1)) block = true;
                                        else
                                        {
                                            if (Items[i, j].Tiny) onTiny++;
                                            else onNormal++;

                                            if (onTiny > 0 && onNormal > 0) block = true;
                                        }

                                        if (block) break;
                                    }
                                    if (block) break;
                                }

                                if (block)
                                {
                                    PutBackPickedItem();
                                }
                                else
                                {
                                    if (!mercenary.GroupAmmo(pickedItem as AmmoBox))
                                    {
                                        List<int> slots = new List<int>();
                                        for (int i = x; i < x + width + 1; ++i)
                                        {
                                            for (int j = y; j < y + height + 1; ++j)
                                            {
                                                Items[i, j].Item = pickedItem;
                                                slots.Add(i + (j * 11));
                                            }
                                        }
                                        mercenary.Items.Add(pickedItem, new ItemPosition(slots.ElementAt(0), newPickedItemOrientation));
                                    }

                                    pickedItem = null;
                                    pickedItemSlot = Slot.Empty;
                                    mouse.CursorVisible = true;
                                }
                            }
                            /*************************/
                            #endregion
                            /*************************/
                            #region Secondary
                            /*************************/
                            else if (mercenary2 != null &&
                                     mousePos.X > SlotsStartPos2.X &&
                                     mousePos.X < SlotsStartPos2.X + 11 * 32 &&
                                     mousePos.Y > SlotsStartPos2.Y &&
                                     mousePos.Y < SlotsStartPos2.Y + (Items2.GetLength(1) < 15 ? Items2.GetLength(1) : 15) * 32)
                            {
                                int x = (int)((mousePos.X - SlotsStartPos2.X) / 32);
                                int y = (int)((mousePos.Y - SlotsStartPos2.Y) / 32) + scrollCurrentOffset2;

                                int width = (pickedItem.SlotsIcon.Width / 32) - 1;
                                int height = (pickedItem.SlotsIcon.Height / 32) - 1;

                                if (newPickedItemOrientation < 0)
                                {
                                    int tmp = width;
                                    width = height;
                                    height = tmp;
                                }

                                bool block = false;
                                uint onTiny = 0;
                                uint onNormal = 0;

                                if (!pickedFromMerc2)
                                {
                                    if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) > 3) block = true;
                                }

                                for (int i = x + width; i >= x; --i)
                                {
                                    for (int j = y + height; j >= y; --j)
                                    {
                                        if (i > Items2.GetLength(0) - 1) block = true;
                                        else if (j > Items2.GetLength(1) - 1) block = true;
                                        else if (Items2[i, j].Blank) block = true;
                                        else if (Items2[i, j].Item != null) block = true;
                                        else if (Items2[i, j].Tiny && (width > 1 || height > 1)) block = true;
                                        else
                                        {
                                            if (Items2[i, j].Tiny) onTiny++;
                                            else onNormal++;

                                            if (onTiny > 0 && onNormal > 0) block = true;
                                        }

                                        if (block) break;
                                    }
                                    if (block) break;
                                }

                                if (block)
                                {
                                    PutBackPickedItem();
                                }
                                else
                                {
                                    if (!mercenary2.GroupAmmo(pickedItem as AmmoBox))
                                    {
                                        List<int> slots = new List<int>();
                                        for (int i = x; i < x + width + 1; ++i)
                                        {
                                            for (int j = y; j < y + height + 1; ++j)
                                            {
                                                Items2[i, j].Item = pickedItem;
                                                slots.Add(i + (j * 11));
                                            }
                                        }
                                        mercenary2.Items.Add(pickedItem, new ItemPosition(slots.ElementAt(0), newPickedItemOrientation));
                                    }
                                    pickedItem = null;
                                    pickedItemSlot = Slot.Empty;
                                    mouse.CursorVisible = true;
                                }
                            }
                            /*************************/
                            #endregion
                            /*************************/
                            #region Dump
                            /*************************/
                            else if (dump &&
                                     mousePos.X > DumpSlotsStartPos.X &&
                                     mousePos.X < DumpSlotsStartPos.X + 11 * 32 &&
                                     mousePos.Y > DumpSlotsStartPos.Y &&
                                     mousePos.Y < DumpSlotsStartPos.Y + (dumpContent.GetLength(1) < 8 ? dumpContent.GetLength(1) : 8) * 32)
                            {
                                int x = (int)((mousePos.X - DumpSlotsStartPos.X) / 32);
                                int y = (int)((mousePos.Y - DumpSlotsStartPos.Y) / 32) + dumpScrollCurrentOffset;

                                int width = (pickedItem.SlotsIcon.Width / 32) - 1;
                                int height = (pickedItem.SlotsIcon.Height / 32) - 1;

                                if (newPickedItemOrientation < 0)
                                {
                                    int tmp = width;
                                    width = height;
                                    height = tmp;
                                }

                                bool block = false;

                                for (int i = x + width; i >= x; --i)
                                {
                                    for (int j = y + height; j >= y; --j)
                                    {
                                        if (i > dumpContent.GetLength(0) - 1) block = true;
                                        else if (j > dumpContent.GetLength(1) - 1) block = true;
                                        else if (dumpContent[i, j].Blank) block = true;
                                        else if (dumpContent[i, j].Item != null) block = true;

                                        if (block) break;
                                    }
                                    if (block) break;
                                }

                                if (block)
                                {
                                    PutBackPickedItem();
                                }
                                else
                                {
                                    List<int> slots = new List<int>();
                                    for (int i = x; i < x + width + 1; ++i)
                                    {
                                        for (int j = y; j < y + height + 1; ++j)
                                        {
                                            dumpContent[i, j].Item = pickedItem;
                                            slots.Add(i + (j * 11));
                                        }
                                    }
                                    dumpItems.Add(pickedItem, new ItemPosition(slots.ElementAt(0), newPickedItemOrientation));
                                    pickedItem = null;
                                    pickedItemSlot = Slot.Empty;
                                    mouse.CursorVisible = true;
                                }
                            }
                            /*************************/
                            #endregion
                            /*************************/
                            else
                            {
                                PutBackPickedItem();
                            }
                        }
                        /*************************/
                        #endregion
                        /*************************/ 
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Scroll
                    /*************************/
                    else if (scroll)
                    {
                        scroll = false;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Next Merc
                    /*************************/
                    else if (nextMerc && mercenariesManager != null)
                    {
                        dump       = false;
                        dumpButton = false;
                        CloseDump();
                        CloseAmmoLoader();
                        CloseFirearm();
                        mercenary = mercenariesManager.GetNextMercenary(mercenary);
                        if (mercenary == mercenary2) mercenary = mercenariesManager.GetNextMercenary(mercenary);
                        SetupMercenary();
                        SendEvent(new SelectedObjectEvent(mercenary, Vector3.Zero), EventsSystem.Priority.High, mercenariesManager);
                        mercenary2 = null;                        
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Prev Merc
                    /*************************/
                    else if (prevMerc && mercenariesManager != null)
                    {                        
                        dump       = false;
                        dumpButton = false;
                        CloseDump();
                        CloseAmmoLoader();
                        CloseFirearm(); 
                        mercenary = mercenariesManager.GetPrevMercenary(mercenary);
                        if (mercenary == mercenary2) mercenary = mercenariesManager.GetPrevMercenary(mercenary);
                        SetupMercenary();
                        SendEvent(new SelectedObjectEvent(mercenary, Vector3.Zero), EventsSystem.Priority.High, mercenariesManager);
                        mercenary2 = null;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Exit Inventory
                    /*************************/
                    else if (exitInv)
                    {
                        CloseDump();
                        SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                        mouse.Modal = false;
                        keyboard.Modal = false;
                        mercenary.UpdateInventory = null;
                        mercenary = null;                        
                        mercenary2 = null;
                        if (container != null) SendEvent(new CloseEvent(), EventsSystem.Priority.Normal, container);
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Scroll Down
                    /*************************/
                    else if (scrollDown)
                    {
                        if (scrollCurrentOffset != scrollMaxOffset) scrollCurrentOffset++;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Scroll Up
                    /*************************/
                    else if (scrollUp)
                    {
                        if (scrollCurrentOffset != 0) scrollCurrentOffset--;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Scroll2
                    /*************************/
                    else if (scroll2)
                    {
                        scroll2 = false;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Dump Scroll Down
                    /*************************/
                    else if (dumpScrollDown)
                    {
                        if (dumpScrollCurrentOffset != dumpScrollMaxOffset) dumpScrollCurrentOffset++;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Dump Scroll Up
                    /*************************/
                    else if (dumpScrollUp)
                    {
                        if (dumpScrollCurrentOffset != 0) dumpScrollCurrentOffset--;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Dump Scroll
                    /*************************/
                    else if (dumpScroll)
                    {
                        dumpScroll = false;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Exit Inventory 2
                    /*************************/
                    else if (exitInv2)
                    {
                        mercenary2 = null;
                    }
                    else if (AmmoLoaderExit)
                    {
                        CloseAmmoLoader();
                    }
                    else if (AmmoLoaderUnload)
                    {
                        UnloadAmmo();
                    }
                    else if (ItemDescExit)
                    {
                        CloseDescription();
                    }
                    else if (FirearmWindowExit)
                    {
                        CloseFirearm();
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Scroll Down 2
                    /*************************/
                    else if (scrollDown2)
                    {
                        if (scrollCurrentOffset2 != scrollMaxOffset2) scrollCurrentOffset2++;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Scroll Up 2
                    /*************************/
                    else if (scrollUp2)
                    {
                        if (scrollCurrentOffset2 != 0) scrollCurrentOffset2--;
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Ammo Loader
                    /*************************/
                    else if (AmmoUnloadOne)
                    {
                        UnloadOne();
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (AmmoSlotsLoad[i])
                            {
                                LoadAmmo(i);
                                return;
                            }
                            else if (AmmoSlotsLoadOne[i])
                            {
                                LoadOne(i);
                                return;
                            }
                        }
                    }
                    /*************************/
                    #endregion
                    /*************************/
                }
                else
                {
                    UncheckButtons();
                }
            }
            else if(mouseKeyAction == MouseKeyAction.RightClick)
            {
                if (mouseKeyState.WasPressed())
                {
                    /*************************/
                    #region Process MercManager
                    /*************************/
                    if (mercenariesManager == null) return;

                    Mercenary merc = mercenariesManager.GetMercenaryFromIcon((int)realMousePos.X, (int)realMousePos.Y);
                    if (merc != null)
                    {
                        if (mercenary == merc)
                        {
                            SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                            mouse.Modal = false;
                            keyboard.Modal = false;
                        }
                        else
                        {
                            mercenary = merc;
                            SetupMercenary();
                        }
                    }
                    /*************************/
                    #endregion
                    /*************************/
                }
                else if (mouseKeyState.WasReleased() && mercenary2 == null)
                {
                    if (mousePos.X > CurrItemIconPos.X &&
                        mousePos.X < CurrItemIconPos.X + 50 &&
                        mousePos.Y > CurrItemIconPos.Y &&
                        mousePos.Y < CurrItemIconPos.Y + 50)
                    {
                        if ((mercenary.CurrentObject as AmmoClip) != null)
                        {
                            SetupAmmoLoader(mercenary.CurrentObject as AmmoClip);
                        }
                        else if ((mercenary.CurrentObject as Firearm) != null)
                        {
                            SetupFirearm(mercenary.CurrentObject as Firearm);
                        }
                        else ShowDescription(mercenary.CurrentObject);
                    }
                    else if (mousePos.X > WeaponIconPos.X &&
                             mousePos.X < WeaponIconPos.X + 50 &&
                             mousePos.Y > WeaponIconPos.Y &&
                             mousePos.Y < WeaponIconPos.Y + 50)
                    {
                        if (mercenary.Weapon != null)
                        {
                            SetupFirearm(mercenary.Weapon);
                        }
                    }
                    else if (mousePos.X > SideArmIconPos.X &&
                             mousePos.X < SideArmIconPos.X + 50 &&
                             mousePos.Y > SideArmIconPos.Y &&
                             mousePos.Y < SideArmIconPos.Y + 50)
                    {
                        if (mercenary.SideArm != null)
                        {
                            SetupFirearm(mercenary.SideArm);
                        }
                    }
                    else if (mousePos.X > SlotsStartPos.X &&
                             mousePos.X < SlotsStartPos.X + 11 * 32 &&
                             mousePos.Y > SlotsStartPos.Y &&
                             mousePos.Y < SlotsStartPos.Y + (Items.GetLength(1) < 15 ? Items.GetLength(1) : 15) * 32)
                    {
                        int x = (int)((mousePos.X - SlotsStartPos.X) / 32);
                        int y = (int)((mousePos.Y - SlotsStartPos.Y) / 32) + scrollCurrentOffset;

                        if (Items[x, y].Item != null)
                        {
                            if ((Items[x, y].Item as AmmoClip) != null)
                            {
                                SetupAmmoLoader(Items[x, y].Item as AmmoClip);
                            }
                            else if ((Items[x, y].Item as Firearm) != null)
                            {
                                SetupFirearm(Items[x, y].Item as Firearm);
                            }
                            else ShowDescription(Items[x, y].Item);
                        }
                    }
                    else if (dump &&
                             mousePos.X > DumpSlotsStartPos.X &&
                             mousePos.X < DumpSlotsStartPos.X + 11 * 32 &&
                             mousePos.Y > DumpSlotsStartPos.Y &&
                             mousePos.Y < DumpSlotsStartPos.Y + (dumpContent.GetLength(1) < 8 ? dumpContent.GetLength(1) : 8) * 32)
                    {
                        int x = (int)((mousePos.X - DumpSlotsStartPos.X) / 32);
                        int y = (int)((mousePos.Y - DumpSlotsStartPos.Y) / 32) + dumpScrollCurrentOffset;

                        if (dumpContent[x, y].Item != null)
                        {
                            if ((dumpContent[x, y].Item as AmmoClip) != null)
                            {
                                SetupAmmoLoader(dumpContent[x, y].Item as AmmoClip);
                            }
                            else if ((dumpContent[x, y].Item as Firearm) != null)
                            {
                                SetupFirearm(dumpContent[x, y].Item as Firearm);
                            }
                            else ShowDescription(dumpContent[x, y].Item);
                        }
                    }
                    else if (FirearmWindow &&
                            mousePos.X > FirearmAmmoClipIconPos.X &&
                            mousePos.X < FirearmAmmoClipIconPos.X + 50 &&
                            mousePos.Y > FirearmAmmoClipIconPos.Y &&
                            mousePos.Y < FirearmAmmoClipIconPos.Y + 50)
                    {
                        if (CurrentFirearm.AmmoClip != null) SetupAmmoLoader(CurrentFirearm.AmmoClip);
                    }
                    else if (FirearmWindow && mousePos.X > 420 && mousePos.Y > 310)
                    {

                        for (int i = 0; i < CurrentFirearm.Accessories.Length; i++)
                        {
                            if (CurrentFirearm.Accessories[i].Accessory != null &&
                                mousePos.X > AccessoriesIconPos.X + (80 * i) &&
                                mousePos.X < AccessoriesIconPos.X + 50 + (80 * i) &&
                                mousePos.Y > AccessoriesIconPos.Y &&
                                mousePos.Y < AccessoriesIconPos.Y + 50)
                            {
                                ShowDescription(CurrentFirearm.Accessories[i].Accessory);
                                break;
                            }
                        }
                    }
                    else if (AmmoLoader)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (mousePos.X > AmmoSlotsIconPos.X + (93 * i) &&
                                mousePos.X < AmmoSlotsIconPos.X + 50 + (93 * i) &&
                                mousePos.Y > AmmoSlotsIconPos.Y &&
                                mousePos.Y < AmmoSlotsIconPos.Y + 50)
                            {
                                AmmoSlots[i] = null;
                                break;
                            }
                        }
                    }
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update Inventory
        /****************************************************************************/
        private void UpdateInventory()
        {
            SetupMercenary();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Setup Mercenary
        /****************************************************************************/
        private void SetupMercenary()
        {
            mercenary.UpdateInventory = UpdateInventory;

            mercenaryNamePos    = new Vector2(353, 70);
            mercenaryNamePos.X -= mercenaryName.MeasureString(mercenary.Name).X / 2;

            int Slots      = (int)(mercenary.TinySlots + mercenary.Slots);
            int TinySlots  = (int)mercenary.TinySlots;
            int height = ((Slots % 11) > 0 ? 1 : 1) + Slots / 11;
            Items = new SlotContent[11, height];

            if (height > 15)
            {
                scrollMaxOffset     = height - 15;
                scrollStep          = 310.0f / (float)scrollMaxOffset;
                scrollCurrentOffset = 0;
            }
            else
            {
                scrollMaxOffset     = 0;
                scrollStep          = 0;
                scrollCurrentOffset = 0;            
            }

            int i = 0;
            for (int y = 0; y < Items.GetLength(1); ++y)
            {
                for (int x = 0; x < Items.GetLength(0); ++x)
                {
                    if (i < TinySlots) Items[x, y].Tiny = true;
                    if (i > Slots)     Items[x, y].Blank = true;
                    i++;
                }
            }

            foreach (KeyValuePair<StorableObject, ItemPosition> pair in mercenary.Items)
            {
                List<int> slots = CalculateSlots(pair.Key, pair.Value.Slot, pair.Value.Orientation, true);
                foreach (int slot in slots)
                {
                    Items[slot % 11, slot / 11].Item = pair.Key;
                }
            }
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Setup Mercenary2
        /****************************************************************************/
        private void SetupMercenary2()
        {
            mercenary2NamePos = new Vector2(488, 70);
            mercenary2NamePos.X -= mercenaryName.MeasureString(mercenary2.Name).X / 2;

            int Slots = (int)(mercenary2.TinySlots + mercenary2.Slots);
            int TinySlots = (int)mercenary2.TinySlots;
            int height = ((Slots % 11) > 0 ? 1 : 1) + Slots / 11;            
            Items2 = new SlotContent[11, height];

            if (height > 15)
            {
                scrollMaxOffset2 = height - 15;
                scrollStep2 = 310.0f / (float)scrollMaxOffset2;
                scrollCurrentOffset2 = 0;
            }
            else
            {
                scrollMaxOffset2 = 0;
                scrollStep2 = 0;
                scrollCurrentOffset2 = 0;
            }

            int i = 0;
            for (int y = 0; y < Items2.GetLength(1); ++y)
            {
                for (int x = 0; x < Items2.GetLength(0); ++x)
                {
                    if (i < TinySlots) Items2[x, y].Tiny = true;
                    if (i > Slots) Items2[x, y].Blank = true;
                    i++;
                }
            }

            foreach (KeyValuePair<StorableObject, ItemPosition> pair in mercenary2.Items)
            {
                List<int> slots = CalculateSlots(pair.Key, pair.Value.Slot, pair.Value.Orientation, true);
                foreach (int slot in slots)
                {
                    Items2[slot % 11, slot / 11].Item = pair.Key;
                }
            }
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Uncheck Buttons
        /****************************************************************************/
        private void UncheckButtons()
        {
            nextMerc         = false;
            prevMerc         = false;
            exitInv          = false;
            scrollUp         = false;
            scrollDown       = false;
            exitInv2         = false;
            scrollUp2        = false;
            scrollDown2      = false;
            dumpScrollDown   = false;
            dumpScrollUp     = false;
            AmmoLoaderExit   = false;
            AmmoLoaderUnload = false;
            AmmoUnloadOne    = false;
            ItemDescExit     = false;
            FirearmWindowExit = false;
            AmmoSlotsLoad    = new bool[4];
            AmmoSlotsLoadOne = new bool[4];
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Move
        /****************************************************************************/
        private void OnMouseMove(MouseMoveAction mouseMoveAction, ref ExtendedMouseMovementState mouseMovementState)
        {
            if (mouseMoveAction == MouseMoveAction.Move)
            {
                if (scroll)
                {
                    scrollCurrentOffset = (int)((mouseMovementState.Position.Y - localPosition.Y - ScrollButtonBasePos.Y) / scrollStep);

                    if      (scrollCurrentOffset < 0              ) scrollCurrentOffset = 0;
                    else if (scrollCurrentOffset > scrollMaxOffset) scrollCurrentOffset = scrollMaxOffset;
                }
                else if (scroll2)
                {
                    scrollCurrentOffset2 = (int)((mouseMovementState.Position.Y - localPosition.Y - Scroll2ButtonBasePos.Y) / scrollStep2);

                    if (scrollCurrentOffset2 < 0) scrollCurrentOffset2 = 0;
                    else if (scrollCurrentOffset2 > scrollMaxOffset2) scrollCurrentOffset2 = scrollMaxOffset2;                
                }
                else if (dumpScroll)
                {
                    dumpScrollCurrentOffset = (int)((mouseMovementState.Position.Y - localPosition.Y - DumpScrollButtonBasePos.Y) / dumpScrollStep);

                    if (dumpScrollCurrentOffset < 0) dumpScrollCurrentOffset = 0;
                    else if (dumpScrollCurrentOffset > dumpScrollMaxOffset) dumpScrollCurrentOffset = dumpScrollMaxOffset;                                
                }
                else
                {
                    realMousePos = new Vector2(mouseMovementState.Position.X, mouseMovementState.Position.Y);
                    mousePos = realMousePos - localPosition;
                }

            }
            else
            {
                if (mercenary2 == null)
                {
                    if (mousePos.X < 420)
                    {
                        if (scrollCurrentOffset + (mouseMovementState.ScrollDifference / 120) > scrollMaxOffset)
                        {
                            scrollCurrentOffset = scrollMaxOffset;
                        }
                        else if (scrollCurrentOffset + (mouseMovementState.ScrollDifference / 120) < 0)
                        {
                            scrollCurrentOffset = 0;
                        }
                        else
                        {
                            scrollCurrentOffset += (int)(mouseMovementState.ScrollDifference / 120);
                        }
                    }
                    else if(mousePos.Y < 310)
                    {
                        if (dumpScrollCurrentOffset + (mouseMovementState.ScrollDifference / 120) > dumpScrollMaxOffset)
                        {
                            dumpScrollCurrentOffset = dumpScrollMaxOffset;
                        }
                        else if (dumpScrollCurrentOffset + (mouseMovementState.ScrollDifference / 120) < 0)
                        {
                            dumpScrollCurrentOffset = 0;
                        }
                        else
                        {
                            dumpScrollCurrentOffset += (int)(mouseMovementState.ScrollDifference / 120);
                        }                        
                    }
                }
                else
                {
                    if (mousePos.X < 420)
                    {
                        if (scrollCurrentOffset + (mouseMovementState.ScrollDifference / 120) > scrollMaxOffset)
                        {
                            scrollCurrentOffset = scrollMaxOffset;
                        }
                        else if (scrollCurrentOffset + (mouseMovementState.ScrollDifference / 120) < 0)
                        {
                            scrollCurrentOffset = 0;
                        }
                        else
                        {
                            scrollCurrentOffset += (int)(mouseMovementState.ScrollDifference / 120);
                        }
                    }
                    else
                    {
                        if (scrollCurrentOffset2 + (mouseMovementState.ScrollDifference / 120) > scrollMaxOffset2)
                        {
                            scrollCurrentOffset2 = scrollMaxOffset2;
                        }
                        else if (scrollCurrentOffset2 + (mouseMovementState.ScrollDifference / 120) < 0)
                        {
                            scrollCurrentOffset2 = 0;
                        }
                        else
                        {
                            scrollCurrentOffset2 += (int)(mouseMovementState.ScrollDifference / 120);
                        }                    
                    }
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        public void OnKey(Keys key, ExtendedKeyState state)
        {
            if(state.WasPressed())
            {
                if (key == Keys.Escape )
                {
                    SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                    mouse.Modal    = false;
                    keyboard.Modal = false;
                    CloseDump();
                    mercenary.UpdateInventory = null;
                    mercenary = null;
                    mercenary2 = null;

                    if (container != null) SendEvent(new CloseEvent(), EventsSystem.Priority.Normal, container);
                }
                else if(key == Keys.Space)
                {
                    if(pickedItem != null) newPickedItemOrientation = -newPickedItemOrientation;
                }
                else if (key == Keys.E)
                {
                    CloseDump();
                    SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                    mouse.Modal = false;
                    keyboard.Modal = false;
                    mercenary.UpdateInventory = null;
                    mercenary = null;
                    mercenary2 = null;

                    if (container != null) SendEvent(new CloseEvent(), EventsSystem.Priority.Normal, container);
                }
                else if (key == Keys.D)
                {
                    if (container != null) return;
                    
                    if (dump)
                    {
                        dump = false;
                        dumpButton = false;
                        CloseDump();
                    }
                    else
                    {
                        dump = true;
                        dumpButton = true;
                        PrepareDump();
                    }
                }
                else if (key == Keys.Tab)
                {
                    if (mercenariesManager == null) return;

                    dump       = false;
                    dumpButton = false;
                    CloseDump();
                    CloseAmmoLoader();
                    CloseFirearm();

                    if (!leftControl)
                    {
                        mercenary = mercenariesManager.GetNextMercenary(mercenary);
                        if (mercenary == mercenary2) mercenary = mercenariesManager.GetNextMercenary(mercenary);
                        SetupMercenary();
                    }
                    else
                    {
                        mercenary = mercenariesManager.GetPrevMercenary(mercenary);
                        if (mercenary == mercenary2) mercenary = mercenariesManager.GetPrevMercenary(mercenary);
                        SetupMercenary();
                    }
                    SendEvent(new SelectedObjectEvent(mercenary, Vector3.Zero), EventsSystem.Priority.High, mercenariesManager);
                    mercenary2 = null;
                }
            }

            if (key == Keys.F) highlights = state.IsDown();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            frontEnd.ReleaseMe();
            frontEnd = null;
            mouse.ReleaseMe();
            mouse = null;
            keyboard.ReleaseMe();
            keyboard = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Slot
        /****************************************************************************/
        private enum Slot
        { 
            CurrentItem,
            Weapon,
            SideArm,
            Armor,
            Inventory,
            Empty,
            Dump,
            Accessories,
            AmmoClip
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Slot Content
        /****************************************************************************/
        private struct SlotContent
        {
            public StorableObject Item;
            
            public bool Blocked;
            public bool Tiny;
            public bool Hover;
            public bool Blank;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// PutBackPickedItem
        /****************************************************************************/
        private void PutBackPickedItem()
        {
            switch (pickedItemSlot)
            {
                case Slot.CurrentItem:
                    if (!pickedFromMerc2) mercenary.PlaceItem(pickedItem,0);
                    else mercenary2.PlaceItem(pickedItem,0);
                    
                    pickedItem = null;
                    mouse.CursorVisible = true;                    
                    
                    break;

                case Slot.Inventory:

                    if (!pickedFromMerc2)
                    {
                        foreach (int slot in pickedItemSlots) Items[slot % 11, slot / 11].Item = pickedItem;
                        mercenary.Items.Add(pickedItem, new ItemPosition(pickedItemSlots.ElementAt(0), oldPickedItemOrientation));
                    }
                    else
                    {
                        foreach (int slot in pickedItemSlots) Items2[slot % 11, slot / 11].Item = pickedItem;
                        mercenary2.Items.Add(pickedItem, new ItemPosition(pickedItemSlots.ElementAt(0), oldPickedItemOrientation));                    
                    }

                    pickedItem = null;
                    mouse.CursorVisible = true;
                    
                    break;

                case Slot.Weapon:
                    
                    if (!pickedFromMerc2) mercenary.PlaceItem(pickedItem,1);
                    else                  mercenary2.PlaceItem(pickedItem,1);
                    
                    pickedItem = null;
                    mouse.CursorVisible = true;          
                    break;

                case Slot.SideArm:

                    if (!pickedFromMerc2) mercenary.PlaceItem(pickedItem, 2);
                    else mercenary2.PlaceItem(pickedItem, 2);

                    pickedItem = null;
                    mouse.CursorVisible = true;
                    break;

                case Slot.Dump:
                    foreach (int slot in pickedItemSlots) dumpContent[slot % 11, slot / 11].Item = pickedItem;
                    dumpItems.Add(pickedItem, new ItemPosition(pickedItemSlots.ElementAt(0), oldPickedItemOrientation));                                        
                    
                    pickedItem = null;
                    mouse.CursorVisible = true;
                    break;
                
                case Slot.Accessories:

                    CurrentFirearm.AttachAccessory(pickedItem as Accessory, pickedFromSlot);
                    pickedItem = null;
                    mouse.CursorVisible = true;

                    break;
                
                case Slot.AmmoClip:
                    CurrentFirearm.AttachClip(pickedItem as AmmoClip);
                    pickedItem = null;
                    mouse.CursorVisible = true;

                
                    break;
            }
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Calculate Slots
        /****************************************************************************/
        public static List<int> CalculateSlots(StorableObject item, int slot,int orientation,bool oriented)
        { 
            List<int> slots = new List<int>();
            int width  = (item.SlotsIcon.Width  / 32) - 1;
            int height = (item.SlotsIcon.Height / 32) - 1;

            if (oriented && orientation < 0) 
            {
                int tmp = width;
                width = height;
                height = tmp;                                        
            }

            int x = slot % 11;
            int y = slot / 11;
            
            for (int i = x; i < x + width + 1; i++)
            {
                for (int j = y; j < y + height + 1; j++)
                {
                    slots.Add(i + (j * 11));
                }
            }

            return slots;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Prepare Dump
        /****************************************************************************/
        private void PrepareDump(uint slots = dumpCapacity, string title = null, Dictionary<StorableObject, ItemPosition> items = null)
        {
            dumpTitle = (String.IsNullOrEmpty(title) ? defaultDumpTitle : title);
            int height = (((int)slots % 11) > 0 ? 1 : 1) + (int)slots / 11;
            dumpContent = new SlotContent[11, height];
            dumpItems = (items == null ? new Dictionary<StorableObject, ItemPosition>() : items);

            int w = 0;
            for (int i = 0; i < dumpContent.GetLength(1); i++)
            {
                for (int j = 0; j < dumpContent.GetLength(0); j++)
                {
                    if (w > slots) dumpContent[j, i].Blank = true;
                    w++;
                }
            }

            if (height > 8)
            {
                dumpScrollMaxOffset     = height - 8;
                dumpScrollStep          = 182.0f / (float)dumpScrollMaxOffset;
                dumpScrollCurrentOffset = 0;
            }
            else
            {
                dumpScrollMaxOffset     = 0;
                dumpScrollStep          = 0;
                dumpScrollCurrentOffset = 0;
            }

            if (items != null)
            {
                foreach (KeyValuePair<StorableObject, ItemPosition> pair in items)
                {
                    List<int> itemSlots = CalculateSlots(pair.Key, pair.Value.Slot, pair.Value.Orientation, true);
                    foreach (int slot in itemSlots)
                    {
                        dumpContent[slot % 11, slot / 11].Item = pair.Key;
                    }
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Close Dump
        /****************************************************************************/
        private void CloseDump()
        {
            if (container == null)
            {
                foreach (KeyValuePair<StorableObject, ItemPosition> items in dumpItems)
                {
                    mercenary.DropItem(items.Key);
                    CheckAmmoSlots(items.Key);
                }
                dumpItems.Clear();
            }
            else
            {
                dumpItems = new Dictionary<StorableObject,ItemPosition>();                
            }            

            dumpContent = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw Value
        /****************************************************************************/
        private void DrawValue(SpriteBatch spriteBatch,StorableObject item,Vector2 pos)
        {
            Type ItemType = item.GetType();
            String valueToDraw = String.Empty;
            if (ItemType.Equals(typeof(AmmoBox)))
            {
                valueToDraw = (item as AmmoBox).Amount.ToString();
            }
            else if (ItemType.Equals(typeof(AmmoClip)))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append((item as AmmoClip).Content.Count.ToString());
                sb.Append("/");
                sb.Append((item as AmmoClip).Capacity.ToString());
                valueToDraw = sb.ToString();
            }
            else if (ItemType.Equals(typeof(Firearm)))
            {
                StringBuilder sb = new StringBuilder();
                Firearm firearm = item as Firearm;
                if (firearm.AmmoClip == null) return;

                sb.Append(firearm.AmmoClip.Content.Count.ToString());
                sb.Append("/");
                sb.Append(firearm.AmmoClip.Capacity.ToString());
                valueToDraw = sb.ToString();
            }

            if (!String.IsNullOrEmpty(valueToDraw))
            {
                pos -= AmmoFont.MeasureString(valueToDraw);

                spriteBatch.DrawString(AmmoFont, valueToDraw, pos + new Vector2(-1, 0), Color.Black);
                spriteBatch.DrawString(AmmoFont, valueToDraw, pos + new Vector2(0, -1), Color.Black);
                spriteBatch.DrawString(AmmoFont, valueToDraw, pos + new Vector2(1, 0), Color.Black);
                spriteBatch.DrawString(AmmoFont, valueToDraw, pos + new Vector2(0, 1), Color.Black);

                spriteBatch.DrawString(AmmoFont, valueToDraw, pos, Color.LightGray);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// SetupAmmoLoader
        /****************************************************************************/
        private void SetupAmmoLoader(AmmoClip clip)
        {
            AmmoLoader      = true;
            CurrentAmmoClip = clip;
            CloseDescription();
            CloseFirearm();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Close Ammo Loader
        /****************************************************************************/
        private void CloseAmmoLoader()
        {
            AmmoLoader      = false;
            CurrentAmmoClip = null;
            AmmoSlots       = new object[4];
        }
        /****************************************************************************/


        /****************************************************************************/
        // Unload Ammo
        /****************************************************************************/
        private void UnloadAmmo()
        {
            if (CurrentAmmoClip.Content.Count == 0) return;

            int foo = CurrentAmmoClip.Content.Count;
            for (int i = 0; i < foo; i++)
            {
                int current = CurrentAmmoClip.Content.Count;
                UnloadOne();
                if (current == CurrentAmmoClip.Content.Count) break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        // Unload One
        /****************************************************************************/
        private void UnloadOne()
        {
            if(CurrentAmmoClip.Content.Count == 0) return;

            Bullet bullet = CurrentAmmoClip.Content.Pop();
            /*******************************/
            #region Ammo Clip
            /*******************************/
            for (int i = 0; i < 4; i++)
            {
                if (AmmoSlots[i] != null)
                {
                    AmmoClip clip = AmmoSlots[i] as AmmoClip;
                    if (clip != null)
                    {
                        if (clip.Content.Count < clip.Capacity)
                        {
                            clip.Content.Push(bullet);
                            return;
                        }
                    }
                }
            }
            /*******************************/
            #endregion
            /*******************************/
            #region Ammo Box
            /*******************************/
            for (int i = 0; i < 4; i++)
            {
                if (AmmoSlots[i] != null)
                {
                    AmmoBox box = AmmoSlots[i] as AmmoBox;
                    if (box != null)
                    {
                        if (box.AmmunitionVersionInfo.Version == bullet.Version)
                        {
                            if (box.Amount < box.Capacity)
                            {
                                box.Amount++;
                                return;
                            }
                        }
                    }
                }
            }
            /*******************************/
            #endregion
            /*******************************/
            #region Empty Slot
            /*******************************/
            for (int i = 0; i < 4; i++)
            {
                if (AmmoSlots[i] == null)
                {
                    AmmoBoxData data = new AmmoBoxData();
                    data.Definition = CurrentAmmoClip.AmmunitionInfo.Name + " AmmoBox";
                    data.Version = bullet.Version;
                    data.PPP = bullet.PPP;
                    data.Amount = 1;
                    AmmoSlots[i] = CreateGameObject(data);
                    mercenary.FindPlaceForItem(AmmoSlots[i] as StorableObject, true);
                    return;
                }
            }
            /*******************************/
            #endregion
            /*******************************/
            CurrentAmmoClip.Content.Push(bullet);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Ammo
        /****************************************************************************/
        private void LoadAmmo(int ammoSlot)
        {
            if (CurrentAmmoClip.Content.Count == CurrentAmmoClip.Capacity) return;

            int foo = CurrentAmmoClip.Capacity - CurrentAmmoClip.Content.Count;
            
            for (int i = 0; 
                i < foo && CurrentAmmoClip.Content.Count < CurrentAmmoClip.Capacity; 
                i++)
            {
                int current = CurrentAmmoClip.Content.Count;
                LoadOne(ammoSlot);
                if (current == CurrentAmmoClip.Content.Count) break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load One 
        /****************************************************************************/
        private void LoadOne(int ammoSlot)
        {
            if (AmmoSlots[ammoSlot] != null && CurrentAmmoClip.Content.Count != CurrentAmmoClip.Capacity) 
            {
                // Przyadałby się jakiś zaśmierdziały interfejs a nie sprawdzanie dwóch typów :/
                AmmoBox ammoBox = AmmoSlots[ammoSlot] as AmmoBox;
                if (ammoBox != null)
                {
                    if (ammoBox.Amount > 0)
                    {
                        ammoBox.Amount--;
                        Bullet bullet = new Bullet();
                        bullet.Version = ammoBox.AmmunitionVersionInfo.Version;
                        bullet.PPP = ammoBox.PPP;
                        CurrentAmmoClip.Content.Push(bullet);

                        if (ammoBox.Amount == 0)
                        {
                            AmmoSlots[ammoSlot] = null;
                            mercenary.Items.Remove(ammoBox);
                            SendEvent(new DestroyObjectEvent(ammoBox.ID), Priority.High, GlobalGameObjects.GameController);
                            UpdateInventory();
                        }

                        return;
                    }
                }

                AmmoClip clip = AmmoSlots[ammoSlot] as AmmoClip;
                if (clip != null)
                {
                    if (clip.Content.Count > 0)
                    {
                        CurrentAmmoClip.Content.Push(clip.Content.Pop());
                        return;
                    }
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Put on Ammo Slot
        /****************************************************************************/
        private void PutOnAmmoSlot(object item,int slot)
        {
            AmmoBox ammoBox = item as AmmoBox;
            if (ammoBox != null)
            {
                if (ammoBox.AmmunitionInfo == CurrentAmmoClip.AmmunitionInfo)
                {
                    if(!AmmoSlots.Contains(ammoBox)) AmmoSlots[slot] = ammoBox;
                }
            }
            else
            {
                AmmoClip ammoClip = item as AmmoClip;
                if (ammoClip == null) return;

                if (ammoClip == CurrentAmmoClip) return;

                if (ammoClip.AmmunitionInfo == CurrentAmmoClip.AmmunitionInfo)
                {
                    if (!AmmoSlots.Contains(ammoClip)) AmmoSlots[slot] = ammoClip;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Check Ammo Slots
        /****************************************************************************/
        private void CheckAmmoSlots(object item)
        {            
            if (item == CurrentAmmoClip) CloseAmmoLoader();

            for (int i = 0; i < 4; i++)
            {
                if (AmmoSlots[i] == item) AmmoSlots[i] = null;
            }

            if (item == DescribedItem) CloseDescription();

            if (item == CurrentFirearm) CloseFirearm();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Check'em (http://memeshack.com/checkem/CheckEm_1297557534919.jpg)
        /****************************************************************************/
        private bool CheckEm(object item,int slot)
        {
            AmmoBox ammoBox = item as AmmoBox;
            if (ammoBox != null)
            {
                if (ammoBox.AmmunitionInfo == CurrentAmmoClip.AmmunitionInfo)
                {
                    if (!AmmoSlots.Contains(ammoBox)) return true;
                }
            }
            else
            {
                AmmoClip ammoClip = item as AmmoClip;
                if (ammoClip == null) return false;

                if (ammoClip == CurrentAmmoClip) return false;

                if (ammoClip.AmmunitionInfo == CurrentAmmoClip.AmmunitionInfo)
                {
                    if (!AmmoSlots.Contains(ammoClip)) return true;
                }
            }

            return false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Clear Ammo Slots
        /****************************************************************************/
        private void ClearAmmoSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                if (AmmoSlots[i] != null)
                {                    
                    AmmoBox ammoBox = AmmoSlots[i] as AmmoBox;
                    if (ammoBox != null)
                    {
                        if (ammoBox.Amount <= 0)
                        {
                            AmmoSlots[i] = null;
                        }
                    }
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Bullet Icon
        /****************************************************************************/
        private Rectangle GetBulletIcon(uint genre, uint version)
        {
            Rectangle rect = new Rectangle(1500, 0, 128, 32);

            switch (genre)
            {
                case 1: rect.Y += 352; break;
                case 2:
                case 3: rect.Y += 160; break;
            }

            switch (version)
            {                 
                case 0: 
                case 3:
                    break;
                case 1:
                    rect.Y += 32; break;
                case 2:
                case 4:
                    rect.Y += 64; break;
                case 5:
                    rect.Y += 96; break;
                case 6:
                    rect.Y += 128; break;
                case 7:
                    rect.Y += 160; break;
            }

            return rect;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Show Description
        /****************************************************************************/
        private void ShowDescription(StorableObject item)
        {
            if (item == null) return;

            ItemDesc = true;
            DescribedItem = item;
            CloseAmmoLoader();
            CloseFirearm();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Close Description
        /****************************************************************************/
        private void CloseDescription()
        {
            ItemDesc = false;
            DescribedItem = null;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Setup Firearm
        /****************************************************************************/
        private void SetupFirearm(Firearm firearm)
        {
            if (firearm == null) return;
            
            FirearmWindow = true;
            CurrentFirearm = firearm;
            CloseAmmoLoader();
            CloseDescription();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Close Firearm
        /****************************************************************************/
        private void CloseFirearm()
        {
            FirearmWindow = false;
            CurrentFirearm = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Check Compability
        /****************************************************************************/
        private bool CheckCompability(AmmoClip ammoClip)
        {
            if (ammoClip == null) return false;

             return ammoClip.Compability.Contains(CurrentFirearm.Name);                        
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Check Accessory Compability
        /****************************************************************************/
        private bool CheckAccessoryCompability(Accessory accessory,int accessorySlot)
        {
            if (accessory == null) return false;

            return CurrentFirearm.Accessories.ElementAt(accessorySlot).Genre.Equals(accessory.Genre);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set Ammunition Info
        /****************************************************************************/
        private void SetAmmunitionInfo(StorableObject item)
        { 
            if(item == null) return;
            Type type = item.GetType();
            
            if (type.Equals(typeof(Firearm)))
            {
                ammunitionInfo = (item as Firearm).Ammunition;
                return;
            }

            if (type.Equals(typeof(AmmoClip)))
            {
                ammunitionInfo = (item as AmmoClip).AmmunitionInfo;
                return;
            }

            if (type.Equals(typeof(AmmoBox)))
            {
                ammunitionInfo = (item as AmmoBox).AmmunitionInfo;
                return;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CheckAmmo
        /****************************************************************************/
        public bool CheckAmmo(AmmoBox ammoBox, AmmoClip ammoClip)
        {
            if (ammoBox == null || ammoClip == null) return false;

            if (ammoBox.AmmunitionInfo == CurrentFirearm.AmmoClip.AmmunitionInfo &&
                CurrentFirearm.AmmoClip.Content.Count < CurrentFirearm.AmmoClip.Capacity)
                return true;
            else return false;       
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Check Ammunition Info
        /****************************************************************************/
        private bool CheckAmmunitionInfo(StorableObject item)
        {            
            if (!highlights) return true;

            if (item == null || ammunitionInfo == null) return false;

            Type type = item.GetType();

            if (type.Equals(typeof(Firearm)))
            {
                if(ammunitionInfo == (item as Firearm).Ammunition) return true;
            }

            if (type.Equals(typeof(AmmoClip)))
            {
                if(ammunitionInfo == (item as AmmoClip).AmmunitionInfo) return true;
            }

            if (type.Equals(typeof(AmmoBox)))
            {
                if(ammunitionInfo == (item as AmmoBox).AmmunitionInfo) return true;
            }

            return false;
        }
        /****************************************************************************/



    }
    /********************************************************************************/


    /********************************************************************************/
    /// Item Position
    /********************************************************************************/
    class ItemPosition
    {
        public int Slot;
        public int Orientation;

        public ItemPosition(int slot, int orientation)
        {
            Slot        = slot;
            Orientation = orientation;
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Inventory Data
    /********************************************************************************/
    [Serializable]
    public class InventoryData : GameObjectInstanceData
    {
        public InventoryData()
        {
            Type = typeof(Inventory);
        }

        public int Mercenary          { get; set; }
        public int Mercenary2         { get; set; }
        public int MercenariesManager { get; set; }
        public int Container          { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/