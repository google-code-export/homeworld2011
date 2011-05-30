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
        private SpriteFont                mercenaryName      = null;

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
        private bool    nextMerc2            = false;
        private bool    prevMerc2            = false;
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


        private IStorable pickedItem     = null;
        private Slot      pickedItemSlot = Slot.Empty;
        private List<int> pickedItemSlots;
        private int       newPickedItemOrientation = 1;
        private int       oldPickedItemOrientation = 1;
        private bool      pickedFromMerc2 = false;

        private bool leftControl = false;
        /****************************************************************************/


        /****************************************************************************/
        /// Consts (lol! Vector2 nie może być const)
        /****************************************************************************/
        private Vector2 NextMercButtonPos   = new Vector2(385, 30);
        private Vector2 PrevMercButtonPos   = new Vector2(308, 30);
        private Vector2 ExitInvButtonPos    = new Vector2(20, 0);
        private Vector2 CurrItemIconPos     = new Vector2(26, 40);
        private Vector2 ScrollUpButtonPos   = new Vector2(400, 223);
        private Vector2 ScrollDownButtonPos = new Vector2(400, 561);
        private Vector2 ScrollButtonBasePos = new Vector2(400, 237);
        private Vector2 SlotsStartPos       = new Vector2(31, 120);
        
        private Vector2 NextMerc2ButtonPos   = new Vector2(518, 30);
        private Vector2 PrevMerc2ButtonPos   = new Vector2(440, 30);
        private Vector2 ExitInv2ButtonPos    = new Vector2(805, 0);
        private Vector2 CurrItemIconPos2     = new Vector2(542, 40);
        private Vector2 ScrollUp2ButtonPos   = new Vector2(424, 223);
        private Vector2 ScrollDown2ButtonPos = new Vector2(424, 561);
        private Vector2 Scroll2ButtonBasePos = new Vector2(424, 237);
        private Vector2 SlotsStartPos2       = new Vector2(451, 120);
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(FrontEndComponent         frontEnd,
                         KeyboardListenerComponent keyboard,
                         MouseListenerComponent    mouse,
                         Mercenary                 mercenary,
                         Mercenary                 mercenary2,
                         MercenariesManager        mercenariesManager)
        {
            this.frontEnd           = frontEnd;
            this.mercenary          = mercenary;
            this.mercenary2         = mercenary2;
            this.mercenariesManager = mercenariesManager;
            this.keyboard           = keyboard;
            this.mouse              = mouse;

            mouse.Modal    = true;
            keyboard.Modal = true;

            keyboard.SubscibeKeys   (OnKey, Keys.Escape,Keys.Space,Keys.LeftControl);
            mouse.SubscribeKeys     (OnMouseKey, MouseKeyAction.LeftClick,MouseKeyAction.RightClick);
            mouse.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move,MouseMoveAction.Scroll);
            
            frontEnd.Draw = OnDraw;

            mercenaryName       = frontEnd.GetFont("Arial");

            SetupMercenary();
            if (mercenary2 != null) SetupMercenary2();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {            
            localPosition = new Vector2((screenWidth/2) - 420,100);

            /***********************/
            #region Draw Main Inventory
            /***********************/
            // Background
            spriteBatch.Draw(frontEnd.Texture, localPosition, new Rectangle(0, 0, 420, 620), Color.White);
            // Merc Head
            spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(328, 5), mercenary.InventoryIcon, Color.White);
            // Merc Name
            spriteBatch.DrawString(mercenaryName, mercenary.Name, localPosition + mercenaryNamePos, Color.WhiteSmoke);
            // Switch Merc Button
            spriteBatch.Draw(frontEnd.Texture, localPosition + NextMercButtonPos, (nextMerc ? new Rectangle(1290, 14, 15, 14) : new Rectangle(1290, 0, 15, 14)), Color.White);
            spriteBatch.Draw(frontEnd.Texture, localPosition + PrevMercButtonPos, (prevMerc ? new Rectangle(1275, 14, 15, 14) : new Rectangle(1275, 0, 15, 14)), Color.White);
            // Exit Inventory Button
            spriteBatch.Draw(frontEnd.Texture, localPosition + ExitInvButtonPos, (exitInv ? new Rectangle(1260, 14, 15, 14) : new Rectangle(1260, 0, 15, 14)), Color.White);
            // Current Item
            IStorable currenItem = mercenary.currentObject as IStorable;
            if (currenItem != null) spriteBatch.Draw(frontEnd.Texture, localPosition + CurrItemIconPos, currenItem.GetIcon(), Color.White);
            // Scroll
            spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollUpButtonPos,   (scrollUp   ? new Rectangle(1305, 14, 15, 14) : new Rectangle(1305, 0, 15, 14)), Color.White);
            spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollDownButtonPos, (scrollDown ? new Rectangle(1320, 14, 15, 14) : new Rectangle(1320, 0, 15, 14)), Color.White);
            spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollButtonBasePos + new Vector2(0, scrollStep * scrollCurrentOffset), (scroll ? new Rectangle(1335, 14, 15, 14) : new Rectangle(1335, 0, 15, 14)), Color.White);            
            // Slots
            for (int y = 0; y < Items.GetLength(1) && y < 15; ++y)
            {
                for (int x = 0; x < Items.GetLength(0); ++x)
                {
                    if (!Items[x, y + scrollCurrentOffset].Blank)
                    {
                        if(Items[x, y + scrollCurrentOffset].Blocked)
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos + new Vector2(32 * x, 32 * y), new Rectangle(1324, 81, 32, 32), Color.White);
                        }
                        else if(Items[x, y + scrollCurrentOffset].Hover)
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

                        Items[x, y + scrollCurrentOffset].Hover   = false;
                        Items[x, y + scrollCurrentOffset].Blocked = false;
                    }
                }
            }
            // Items in inventory
            foreach (KeyValuePair<IStorable, ItemPosition> pair in mercenary.Items)
            {
                Rectangle rect = pair.Key.GetSlotsIcon();
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
                        spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos + new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset + diff)), rect, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos + new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset + diff)) + new Vector2(pair.Key.GetSlotsIcon().Height, 0), rect, Color.White, MathHelper.PiOver2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                    }
                }
            }
            /***********************/
            #endregion
            /***********************/
                                    
            /***********************/
            #region Draw Second Inventory
            /***********************/
            if (mercenary2 != null)
            {
                // Background
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(420, 0), new Rectangle(420, 0, 420, 620), Color.White);
                // Merc Head
                spriteBatch.Draw(frontEnd.Texture, localPosition + new Vector2(461, 5), mercenary2.InventoryIcon, Color.White);
                //// Merc Name
                spriteBatch.DrawString(mercenaryName, mercenary2.Name, localPosition + mercenary2NamePos, Color.WhiteSmoke);
                //// Switch Merc Button
                spriteBatch.Draw(frontEnd.Texture, localPosition + NextMerc2ButtonPos, (nextMerc2 ? new Rectangle(1290, 14, 15, 14) : new Rectangle(1290, 0, 15, 14)), Color.White);
                spriteBatch.Draw(frontEnd.Texture, localPosition + PrevMerc2ButtonPos, (prevMerc2 ? new Rectangle(1275, 14, 15, 14) : new Rectangle(1275, 0, 15, 14)), Color.White);
                // Exit Inventory Button
                spriteBatch.Draw(frontEnd.Texture, localPosition + ExitInv2ButtonPos, (exitInv2 ? new Rectangle(1260, 14, 15, 14) : new Rectangle(1260, 0, 15, 14)), Color.White);
                // Current Item
                currenItem = mercenary2.currentObject as IStorable;
                if (currenItem != null) spriteBatch.Draw(frontEnd.Texture, localPosition + CurrItemIconPos2, currenItem.GetIcon(), Color.White);
                // Scroll
                spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollUp2ButtonPos, (scrollUp2 ? new Rectangle(1305, 14, 15, 14) : new Rectangle(1305, 0, 15, 14)), Color.White);
                spriteBatch.Draw(frontEnd.Texture, localPosition + ScrollDown2ButtonPos, (scrollDown2 ? new Rectangle(1320, 14, 15, 14) : new Rectangle(1320, 0, 15, 14)), Color.White);
                spriteBatch.Draw(frontEnd.Texture, localPosition + Scroll2ButtonBasePos + new Vector2(0, scrollStep2 * scrollCurrentOffset2), (scroll2 ? new Rectangle(1335, 14, 15, 14) : new Rectangle(1335, 0, 15, 14)), Color.White);
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
                // Items in inventory
                foreach (KeyValuePair<IStorable, ItemPosition> pair in mercenary2.Items)
                {
                    Rectangle rect = pair.Key.GetSlotsIcon();
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
                            spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos2 + new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset2 + diff)), rect, Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(frontEnd.Texture, localPosition + SlotsStartPos2 + new Vector2(32 * (itemSlot % 11), 32 * (y - scrollCurrentOffset2 + diff)) + new Vector2(pair.Key.GetSlotsIcon().Height, 0), rect, Color.White, MathHelper.PiOver2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                        }
                    }
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
                    spriteBatch.Draw(frontEnd.Texture, realMousePos, pickedItem.GetSlotsIcon(), Color.White);
                }
                else
                {
                    spriteBatch.Draw(frontEnd.Texture, realMousePos + new Vector2(pickedItem.GetSlotsIcon().Height, 0), pickedItem.GetSlotsIcon(), Color.White, MathHelper.PiOver2, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                }
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
                    #region Pick Current Item
                    /*************************/
                    else if (mousePos.X > CurrItemIconPos.X &&
                             mousePos.X < CurrItemIconPos.X + 50 &&
                             mousePos.Y > CurrItemIconPos.Y &&
                             mousePos.Y < CurrItemIconPos.Y + 50)
                    {
                        pickedItem = mercenary.currentObject as IStorable;
                        if (pickedItem != null)
                        {
                            pickedItemSlot = Slot.CurrentItem;
                            mercenary.StoreCurrentItem();
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
                        pickedItem = mercenary2.currentObject as IStorable;
                        if (pickedItem != null)
                        {
                            pickedItemSlot = Slot.CurrentItem;
                            mercenary2.StoreCurrentItem();
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

                            int width = (pickedItem.GetSlotsIcon().Width / 32) - 1;
                            int height = (pickedItem.GetSlotsIcon().Height / 32) - 1;

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
                                    else if (Items[i, j].Tiny && (width > 2 || height > 2))
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

                            int width = (pickedItem.GetSlotsIcon().Width / 32) - 1;
                            int height = (pickedItem.GetSlotsIcon().Height / 32) - 1;

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
                                    else if (Items2[i, j].Tiny && (width > 2 || height > 2))
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
                    }
                    /******************************/
                    #endregion
                    /******************************/
                    #region Pressed Buttons
                    /******************************/
                    else if (!scroll && !scroll2)
                    {
                        /*************************/
                        /// Next Merc Button
                        /*************************/
                        if (mousePos.X > NextMercButtonPos.X &&
                            mousePos.X < NextMercButtonPos.X + 15 &&
                            mousePos.Y > NextMercButtonPos.Y &&
                            mousePos.Y < NextMercButtonPos.Y + 14)
                        {
                            UncheckButtons();
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
                            UncheckButtons();
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
                            UncheckButtons();
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
                            UncheckButtons();
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
                            UncheckButtons();
                            scrollDown = true;
                        }
                        /*************************/
                        /// Next Merc 2 Button
                        /*************************/
                        else if (mousePos.X > NextMerc2ButtonPos.X &&
                            mousePos.X < NextMerc2ButtonPos.X + 15 &&
                            mousePos.Y > NextMerc2ButtonPos.Y &&
                            mousePos.Y < NextMerc2ButtonPos.Y + 14)
                        {
                            UncheckButtons();
                            nextMerc2 = true;
                        }
                        /*************************/
                        /// Prev Merc 2 Button
                        /*************************/
                        else if (mousePos.X > PrevMerc2ButtonPos.X &&
                                 mousePos.X < PrevMerc2ButtonPos.X + 15 &&
                                 mousePos.Y > PrevMerc2ButtonPos.Y &&
                                 mousePos.Y < PrevMerc2ButtonPos.Y + 14)
                        {
                            UncheckButtons();
                            prevMerc2 = true;
                        }
                        /*************************/
                        /// Exit Inv 2 Button
                        /*************************/
                        else if (mousePos.X > ExitInv2ButtonPos.X &&
                                 mousePos.X < ExitInv2ButtonPos.X + 15 &&
                                 mousePos.Y > ExitInv2ButtonPos.Y &&
                                 mousePos.Y < ExitInv2ButtonPos.Y + 14)
                        {
                            UncheckButtons();
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
                            UncheckButtons();
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
                            UncheckButtons();
                            scrollDown2 = true;
                        }
                        else
                        {
                            UncheckButtons();
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
                            if (mercenary.currentObject == null)
                            {
                                if (pickedFromMerc2)
                                {
                                    if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) < 3)
                                    {
                                        mercenary.PickItem(pickedItem as GameObjectInstance);
                                        pickedItem = null;
                                        mouse.CursorVisible = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    mercenary.PickItem(pickedItem as GameObjectInstance);
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
                            if (mercenary2.currentObject == null)
                            {
                                if (!pickedFromMerc2)
                                {
                                    if (Vector3.Distance(mercenary2.World.Translation, mercenary.World.Translation) < 3)
                                    {
                                        mercenary2.PickItem(pickedItem as GameObjectInstance);
                                        pickedItem = null;
                                        mouse.CursorVisible = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    mercenary2.PickItem(pickedItem as GameObjectInstance);
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
                        #region On Inventory
                        /*************************/
                        else if (mousePos.X > SlotsStartPos.X &&
                                 mousePos.X < SlotsStartPos.X + 11 * 32 &&
                                 mousePos.Y > SlotsStartPos.Y &&
                                 mousePos.Y < SlotsStartPos.Y + (Items.GetLength(1) < 15 ? Items.GetLength(1) : 15) * 32)
                        {
                            int x = (int)((mousePos.X - SlotsStartPos.X) / 32);
                            int y = (int)((mousePos.Y - SlotsStartPos.Y) / 32) + scrollCurrentOffset;

                            int width = (pickedItem.GetSlotsIcon().Width / 32) - 1;
                            int height = (pickedItem.GetSlotsIcon().Height / 32) - 1;

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
                                    else if (Items[i, j].Tiny && (width > 2 || height > 2)) block = true;
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
                                pickedItem = null;
                                pickedItemSlot = Slot.Empty;
                                mouse.CursorVisible = true;
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

                            int width = (pickedItem.GetSlotsIcon().Width / 32) - 1;
                            int height = (pickedItem.GetSlotsIcon().Height / 32) - 1;

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
                                    else if (Items2[i, j].Tiny && (width > 2 || height > 2)) block = true;
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
                                pickedItem = null;
                                pickedItemSlot = Slot.Empty;
                                mouse.CursorVisible = true;
                            }
                        }
                        /*************************/
                        #endregion
                        /*************************/
                        else PutBackPickedItem();
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
                    if (nextMerc)
                    {
                        mercenary = mercenariesManager.GetNextMercenary(mercenary);
                        if (mercenary == mercenary2) mercenary = mercenariesManager.GetNextMercenary(mercenary);
                        SetupMercenary();
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Prev Merc
                    /*************************/
                    else if (prevMerc)
                    {
                        mercenary = mercenariesManager.GetPrevMercenary(mercenary);
                        if (mercenary == mercenary2) mercenary = mercenariesManager.GetPrevMercenary(mercenary);
                        SetupMercenary();
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Exit Inventory
                    /*************************/
                    else if (exitInv)
                    {
                        SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                        mouse.Modal = false;
                        keyboard.Modal = false;
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
                    #region Next Merc2
                    /*************************/
                    else if (nextMerc2)
                    {
                        mercenary2 = mercenariesManager.GetNextMercenary(mercenary2);
                        if (mercenary2 == mercenary) mercenary2 = mercenariesManager.GetNextMercenary(mercenary2);
                        SetupMercenary2();
                    }
                    /*************************/
                    #endregion
                    /*************************/
                    #region Prev Merc2
                    /*************************/
                    else if (prevMerc2)
                    {
                        mercenary2 = mercenariesManager.GetPrevMercenary(mercenary2);
                        if (mercenary2 == mercenary) mercenary2 = mercenariesManager.GetPrevMercenary(mercenary2);
                        SetupMercenary2();
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
                    Mercenary merc = mercenariesManager.GetMercenaryFromIcon((int)realMousePos.X, (int)realMousePos.Y);
                    if (merc != null)
                    {
                        if (mercenary == merc)
                        {
                            SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                            mouse.Modal = false;
                            keyboard.Modal = false;
                        }
                        else if (mercenary2 == merc)
                        {
                            mercenary2 = null;
                        }
                        else if (leftControl)
                        {
                            mercenary2 = merc;
                            SetupMercenary2();
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
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Setup Mercenary
        /****************************************************************************/
        private void SetupMercenary()
        {
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

            foreach (KeyValuePair<IStorable, ItemPosition> pair in mercenary.Items)
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

            foreach (KeyValuePair<IStorable, ItemPosition> pair in mercenary2.Items)
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
            nextMerc    = false;
            prevMerc    = false;
            exitInv     = false;
            scrollUp    = false;
            scrollDown  = false;
            nextMerc2   = false;
            prevMerc2   = false;
            exitInv2    = false;
            scrollUp2   = false;
            scrollDown2 = false;
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
                }
                else if(key == Keys.Space)
                {
                    if(pickedItem != null) newPickedItemOrientation = -newPickedItemOrientation;
                }
            }

            if(key == Keys.LeftControl) leftControl = state.IsDown();
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
            Empty
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Slot Content
        /****************************************************************************/
        private struct SlotContent
        {
            public IStorable Item;
            public bool      Blocked;
            public bool      Tiny;
            public bool      Hover;
            public bool      Blank;
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
                    if (!pickedFromMerc2) mercenary.PickItem(pickedItem as GameObjectInstance);
                    else                  mercenary2.PickItem(pickedItem as GameObjectInstance);
                    
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
            }
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Calculate Slots
        /****************************************************************************/
        private List<int> CalculateSlots(IStorable item, int slot,int orientation,bool oriented)
        { 
            List<int> slots = new List<int>();
            int width  = (item.GetSlotsIcon().Width  / 32) - 1;
            int height = (item.GetSlotsIcon().Height / 32) - 1;

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
    }
    /********************************************************************************/

}
/************************************************************************************/