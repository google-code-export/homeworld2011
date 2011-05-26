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
        private Mercenary                 mercenary          = null;
        private MercenariesManager        mercenariesManager = null;
        private KeyboardListenerComponent keyboard           = null;
        private MouseListenerComponent    mouse              = null;
        
        private SpriteFont                mercenaryName      = null;
        private Vector2                   mercenaryNamePos;

        private Vector2                   localPosition;

        private bool nextMerc   = false;
        private bool prevMerc   = false;
        private bool exitInv    = false;
        private bool scrollUp   = false;
        private bool scrollDown = false;
        private bool scroll     = false;

        private Vector2 mousePos;
        private Vector2 realMousePos;

        private SlotContent[,] Items;

        private float scrollStep          = 0;
        private int   scrollMaxOffset     = 0;
        private int   scrollCurrentOffset = 0;

        private IStorable pickedItem     = null;
        private Slot      pickedItemSlot = Slot.Empty;
        private List<int> pickedItemSlots;
        private int       newPickedItemOrientation = 1;
        private int       oldPickedItemOrientation = 1;
        /****************************************************************************/


        /****************************************************************************/
        /// Consts (lol! Vector2 nie może być const)
        /****************************************************************************/
        private Vector2 NextMercButtonPos   = new Vector2(385, 30);
        private Vector2 PrevMercButtonPos   = new Vector2(308, 30);
        private Vector2 ExitInvButtonPos    = new Vector2(405, 0);
        private Vector2 CurrItemIconPos     = new Vector2(26, 40);
        private Vector2 ScrollUpButtonPos   = new Vector2(400, 223);
        private Vector2 ScrollDownButtonPos = new Vector2(400, 561);
        private Vector2 ScrollButtonBasePos = new Vector2(400, 237);
        private Vector2 SlotsStartPos       = new Vector2(31, 120);
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(FrontEndComponent         frontEnd,
                         KeyboardListenerComponent keyboard,
                         MouseListenerComponent    mouse,
                         Mercenary                 mercenary,
                         MercenariesManager        mercenariesManager)
        {
            this.frontEnd           = frontEnd;
            this.mercenary          = mercenary;
            this.mercenariesManager = mercenariesManager;
            this.keyboard           = keyboard;
            this.mouse              = mouse;

            mouse.Modal    = true;
            keyboard.Modal = true;

            keyboard.SubscibeKeys   (OnKey, Keys.Escape,Keys.Space);
            mouse.SubscribeKeys     (OnMouseKey, MouseKeyAction.LeftClick);
            mouse.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move,MouseMoveAction.Scroll);
            
            frontEnd.Draw = OnDraw;

            mercenaryName       = frontEnd.GetFont("Arial");

            SetupMercenary();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            localPosition = new Vector2((screenWidth/2) - 420,100);
            /***********************/
            /// Draw Main Inventory
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
            // Picked Item
            if (pickedItem != null)
            {
                if (newPickedItemOrientation > 0)
                {
                    spriteBatch.Draw(frontEnd.Texture, realMousePos, pickedItem.GetSlotsIcon(), Color.White);
                }
                else
                {
                    spriteBatch.Draw(frontEnd.Texture, realMousePos + new Vector2(pickedItem.GetSlotsIcon().Height,0), pickedItem.GetSlotsIcon(), Color.White, MathHelper.PiOver2, new Vector2(0, 0), 1, SpriteEffects.None, 0);                
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
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Key
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ref ExtendedMouseKeyState mouseKeyState)
        {   
         
            if (mouseKeyState.WasPressed())
            {
                /*************************/
                /// Scroll Button
                /*************************/
                if (mousePos.X > ScrollButtonBasePos.X      &&
                    mousePos.X < ScrollButtonBasePos.X + 15 &&
                    mousePos.Y > ScrollButtonBasePos.Y +      scrollStep * scrollCurrentOffset &&
                    mousePos.Y < ScrollButtonBasePos.Y + 14 + scrollStep * scrollCurrentOffset)
                {
                    scroll = true;
                }
                /*************************/
                /// Pick Current Item
                /*************************/
                else if (mousePos.X > CurrItemIconPos.X      &&
                         mousePos.X < CurrItemIconPos.X + 50 &&
                         mousePos.Y > CurrItemIconPos.Y      &&
                         mousePos.Y < CurrItemIconPos.Y + 50 )
                {
                    pickedItem = mercenary.currentObject as IStorable;
                    if (pickedItem != null)
                    {
                        pickedItemSlot = Slot.CurrentItem;
                        mercenary.StoreCurrentItem();
                        mouse.CursorVisible = false;
                        newPickedItemOrientation = 1;
                        oldPickedItemOrientation = 1;
                    }
                }
                /*************************/
                /// Pick Item From Inventory
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
                        pickedItemSlots        = CalculateSlots(pickedItem, mercenary.Items[pickedItem].Slot, mercenary.Items[pickedItem].Orientation, true);

                        oldPickedItemOrientation = mercenary.Items[pickedItem].Orientation;
                        newPickedItemOrientation = oldPickedItemOrientation;

                        mercenary.Items.Remove(pickedItem);
                        
                        foreach (int slot in slotsToClean)
                        {
                            Items[slot % 11, slot / 11].Item = null;
                        }

                        pickedItemSlot = Slot.Inventory;
                        mouse.CursorVisible = false;
                    }
                }
            }
            
            if (mouseKeyState.IsDown())
            {
                if (pickedItem != null)
                {
                    if (mousePos.X > SlotsStartPos.X &&
                        mousePos.X < SlotsStartPos.X + 11 * 32  &&
                        mousePos.Y > SlotsStartPos.Y            &&
                        mousePos.Y < SlotsStartPos.Y + (Items.GetLength(1) < 15 ? Items.GetLength(1) : 15) * 32)
                    {
                        int x = (int)((mousePos.X - SlotsStartPos.X) / 32);
                        int y = (int)((mousePos.Y - SlotsStartPos.Y) / 32) + scrollCurrentOffset;

                        int width  = (pickedItem.GetSlotsIcon().Width  / 32) - 1;
                        int height = (pickedItem.GetSlotsIcon().Height / 32) - 1;

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
                                if (i > Items.GetLength(0) - 1) blockAll = true;
                                else if (j > Items.GetLength(1) -1) blockAll = true;
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
                                else
                                {
                                    Items[i, j].Hover = true;
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
                }
                else if (!scroll)
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
                    else
                    {
                        UncheckButtons();
                    }
                }
            }
            else if (mouseKeyState.WasReleased())
            {
                /*************************/
                /// Drop Picked Item
                /*************************/
                if (pickedItem != null)
                {

                    /*************************/
                    /// On Current Item
                    /*************************/
                    if (mousePos.X > CurrItemIconPos.X &&
                        mousePos.X < CurrItemIconPos.X + 50 &&
                        mousePos.Y > CurrItemIconPos.Y &&
                        mousePos.Y < CurrItemIconPos.Y + 50)
                    {
                        if (mercenary.currentObject == null)
                        {
                            mercenary.PickItem(pickedItem as GameObjectInstance);
                            pickedItem = null;
                            mouse.CursorVisible = true;
                        }
                        else
                        {                            
                            IStorable tmp = mercenary.currentObject as IStorable;
                            mercenary.StoreCurrentItem();
                            mercenary.PickItem(pickedItem as GameObjectInstance);
                            pickedItem = tmp;
                            pickedItemSlot = Slot.Empty;
                        }
                    }
                    /*************************/
                    /// On Inventory
                    /*************************/
                    else if(mousePos.X > SlotsStartPos.X &&
                            mousePos.X < SlotsStartPos.X + 11 * 32  &&
                            mousePos.Y > SlotsStartPos.Y            &&
                            mousePos.Y < SlotsStartPos.Y + (Items.GetLength(1) < 15 ? Items.GetLength(1) : 15) * 32)
                    {
                        int x = (int)((mousePos.X - SlotsStartPos.X) / 32);
                        int y = (int)((mousePos.Y - SlotsStartPos.Y) / 32) + scrollCurrentOffset;

                        int width  = (pickedItem.GetSlotsIcon().Width  / 32) - 1;
                        int height = (pickedItem.GetSlotsIcon().Height / 32) - 1;

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
                                if      (i > Items.GetLength(0) - 1)    block = true;
                                else if (j > Items.GetLength(1) - 1)    block = true;
                                else if (Items[i, j].Blank         )    block = true;
                                else if (Items[i, j].Item != null  )    block = true;

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
                            mercenary.Items.Add(pickedItem, new ItemPosition(slots.ElementAt(0),newPickedItemOrientation));
                            pickedItem = null;
                            pickedItemSlot = Slot.Empty;
                            mouse.CursorVisible = true;                            
                        }
                    }
                    else
                    {
                        PutBackPickedItem();
                    }

                }
                /*************************/
                /// Scroll
                /*************************/
                else if (scroll)
                {
                    scroll = false;
                }
                /*************************/
                /// Next Merc
                /*************************/                
                if (nextMerc)
                {
                    mercenary = mercenariesManager.GetNextMercenary(mercenary);
                    SetupMercenary();
                }
                /*************************/
                /// Prev Merc
                /*************************/                
                else if (prevMerc)
                {
                    mercenary = mercenariesManager.GetPrevMercenary(mercenary);
                    SetupMercenary();
                }
                /*************************/
                /// Exit Inventory
                /*************************/                
                else if (exitInv)
                {
                    SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                    mouse.Modal = false;
                    keyboard.Modal = false;
                }
                /*************************/
                /// Scroll Down
                /*************************/                
                else if (scrollDown)
                {
                    if (scrollCurrentOffset != scrollMaxOffset) scrollCurrentOffset++;
                }
                /*************************/
                /// Scroll Up
                /*************************/                
                else if (scrollUp)
                {
                    if (scrollCurrentOffset != 0) scrollCurrentOffset--;
                }
            }
            else
            {
                UncheckButtons();
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
        /// Uncheck Buttons
        /****************************************************************************/
        private void UncheckButtons()
        {
            nextMerc    = false;
            prevMerc    = false;
            exitInv     = false;
            scrollUp    = false;
            scrollDown  = false;
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
                else
                {
                    realMousePos = new Vector2(mouseMovementState.Position.X, mouseMovementState.Position.Y);
                    mousePos = realMousePos - localPosition;
                }
            }
            else
            {
                if (scrollCurrentOffset + (mouseMovementState.ScrollDifference/120) > scrollMaxOffset)
                {
                    scrollCurrentOffset = scrollMaxOffset;
                }
                else if (scrollCurrentOffset + (mouseMovementState.ScrollDifference/120) < 0)
                {
                    scrollCurrentOffset = 0;
                }
                else
                {
                    scrollCurrentOffset += (int)(mouseMovementState.ScrollDifference/120);
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
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            frontEnd.ReleaseMe();
            frontEnd = null;
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
                    mercenary.PickItem(pickedItem as GameObjectInstance);
                    pickedItem = null;
                    mouse.CursorVisible = true;
                    break;

                case Slot.Inventory:
                    foreach (int slot in pickedItemSlots)
                    {
                        Items[slot % 11, slot / 11].Item = pickedItem;
                    }
                    mercenary.Items.Add(pickedItem,new ItemPosition(pickedItemSlots.ElementAt(0),oldPickedItemOrientation));
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
        public int MercenariesManager { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/