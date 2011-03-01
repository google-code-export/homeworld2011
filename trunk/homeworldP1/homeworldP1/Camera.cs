using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace homeworldP1
{
    /// <summary>
    /// tryby pracy kamery
    /// value
    /// </summary>
	public enum cameraMode
	{
		Free, Linked, Radar
	}
	/// <summary>
	/// klasa kamery
	/// </summary>
	public class Camera : DrawableGameComponent
	{
        
        const Double minDist=10;
        const Double maxDist=1000;

		private cameraMode workMode = cameraMode.Free;
        private Quaternion curAngle;
        private Double dist = (minDist);
		
		#region PositionAndDirection
		/// <summary> pozycja kamery </summary>
		protected Vector3 position;
		/// <summary>
		/// target - cel kamery
		/// w trybie Linked jest pobierany z jednego z obiektów na scenie
		/// w trybach Free i Radar obliczany jest na podstawie hAngle, hAngleDelta i vAngle, vAngleDelta
		/// </summary>
		protected Vector3 target;
		/// <summary>
		/// zapamietywany stan rolki myszy, do oddalania i przybliżania kamery
		/// </summary>
		protected int MouseWheel;
		#endregion*/
		#region Matrices
		/// <summary>
		/// macierz widoku
		/// prywatna, udostępniana za pomocą funkcji View()
		/// </summary>
		protected Matrix view;
		/// <summary>
		/// macierz projekcji
		/// prywatna, udostępniana za pomocą funkcji Projection()
		/// </summary>
		protected Matrix projection;
		#endregion
		#region SomeHelpers
		/// <summary>
		/// info o graphixie gry
		/// </summary>
		protected GraphicsDeviceManager graphics;
		/// <summary>
		/// takie zabawne okienko w ktorym fajnie jest sobie czasem wypisać jakiś tekst
		/// nalezy usunąć w momencie kiedy projekt zacznie działać
		/// żeby w nim coś wypisać trzeba odkomentować 3 linie w konstruktorze i uzywać metody logi.addText("plepleple");
		/// </summary>
		Logi logi;
		#endregion
		#region MouseHelpers
		/// <summary>
		/// czy lewy przycisk myszy (LMB) jest klikniety
		/// </summary>
		protected bool leftDown;
		/// <summary>
		/// pozycja myszy w momencie wciśnięcia lewego przycisku muszy
		/// </summary>
		protected Vector2 leftPosition;
		/// <summary>
		/// czy środkowy przycisk myszy (MMB) jest klikniety
		/// </summary>
		protected bool middleDown;
		/// <summary>
		/// pozycja myszy w momencie wciśnięcia środkowego przycisku muszy 
		/// </summary>
		protected Vector2 middlePosition;
		/// <summary>
		/// czy prawy przycisk myszy (RMB) jest klikniety
		/// </summary>
		protected bool rightDown;
		/// <summary>
		/// pozycja myszy w momencie wciśnięcia prawego przycisku muszy 
		/// </summary>
		protected Vector2 rightPosition;
		#endregion
		#region Constructors
		/// <summary>
		/// konstruktor
		/// </summary>
		/// <param name="_graphics">kamera potrzebuje info o rozdzielczości, macierzach, ekranach itp to tu jej to wszystko dajemy</param>
		/// <param name="position">pozycja początkowa kamery</param>
		/// <param name="rotation">początkowy obrót kamery jako kwaternion</param>
		public Camera(GraphicsDeviceManager _graphics, Vector3 position, Quaternion rotation)
			: base(null)
		{
            graphics = _graphics;
            curAngle = rotation;
            target = position;
            rightDown = false;
		}
		/// <summary>
		/// konstruktor
		/// </summary>
		/// <param name="_graphics">kamera potrzebuje info o rozdzielczości, macierzach, ekranach itp to tu jej to wszystko dajemy</param>
		/// <param name="position">pozycja początkowa kamery</param>
		/// <param name="target">pozycja początkowa targetu kamery</param>
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="height"></param>
		/// <returns></returns>
		/*public Vector3 mouseTo3D(float height)
		{
			MouseState mState = Mouse.GetState();
			Viewport vp = graphics.GraphicsDevice.Viewport;
			Vector3 pos1 = vp.Unproject(new Vector3(mState.X, mState.Y, 0), projection, view, Matrix.Identity);
			Vector3 pos2 = vp.Unproject(new Vector3(mState.X, mState.Y, 1), projection, view, Matrix.Identity);
			Vector3 dir = Vector3.Normalize(pos2 - pos1);

			if (dir.Y != 0)
			{
				Vector3 x = pos1 - dir * (pos1.Y / dir.Y);
				return x;
			}
			else
			{
				return Vector3.Zero;
			}
		}*/


        /// <summary>
        /// Calculates the matrices.
        /// </summary>
		public void calculateMatrices()
		{
			view = Matrix.CreateLookAt(position, target, Vector3.Up);
			projection = Matrix.CreatePerspectiveFieldOfView(
				MathHelper.PiOver4,
				graphics.GraphicsDevice.Viewport.AspectRatio,
				1.0f,
				10000.0f);
		}

        /// <summary>
        /// Calculates the position.
        /// </summary>
        private void calculatePosition()
        {
            
            position = target + Vector3.Multiply(Vector3.Transform(Vector3.UnitX, curAngle), (float)dist);
            calculateMatrices();
        }

        /// <summary>
        /// Updates the specified gametime.
        /// </summary>
        /// <param name="gametime">The gametime.</param>
		override public void Update(GameTime gametime)
		{
            MouseState mState = Mouse.GetState();
            KeyboardState kState = Keyboard.GetState();
            Vector3 lrAxis = Vector3.Cross((position - target), Vector3.Up);
            lrAxis.Normalize();
            Vector3 fbAxis = Vector3.Cross(lrAxis, Vector3.Up);
            fbAxis.Normalize();
            #region Mouse Middle Button Handling
            if ( mState.MiddleButton == ButtonState.Pressed )
            {
                switch (workMode)
                {
                    case cameraMode.Radar:
                    case cameraMode.Free:
                        //TODO: podpięcie do wskazanego obiektu;
                        workMode = cameraMode.Linked;
                        break;
                    case cameraMode.Linked:
                        //TODO: umożliwić przepięcie na nowy obiekt
                        dist = minDist;
                        break;
                    default:
                        //TODO: tutaj walić jakiś wyjątek.
                        break;
                }
            }
            #endregion
            #region Mouse Scroll Wheel Handling  
            if (mState.ScrollWheelValue > MouseWheel)
            {
                dist -= 10;
                if (dist < minDist) dist = minDist;
            }
            else if (mState.ScrollWheelValue < MouseWheel)
            {
                dist += 10;
                if (dist > maxDist) dist = maxDist;
            }
            MouseWheel = mState.ScrollWheelValue; 
            #endregion
            #region Mouse Right Button Handling
            if (mState.RightButton == ButtonState.Pressed)
            {
                if (!rightDown)
                {
                    rightPosition.X = mState.X;
                    rightPosition.Y = mState.Y;
                    rightDown = true;
                }
                else
                {
                    if (rightPosition.X > mState.X)
                    {
                        curAngle = Quaternion.Multiply(curAngle, Quaternion.CreateFromAxisAngle(Vector3.Up,  0.05f));// (Vector3.Up, 5.0f));
                        //curAngle *= Quaternion.CreateFromAxisAngle(Vector3.Up,  5.0f);
                    }
                    else if (rightPosition.X < mState.X)
                    {
                        curAngle = Quaternion.Multiply(curAngle, Quaternion.CreateFromAxisAngle(Vector3.Up, -0.05f));
                    }

                    if (rightPosition.Y > mState.Y)
                    {
                        curAngle = Quaternion.Multiply(curAngle, Quaternion.CreateFromAxisAngle(lrAxis, -0.05f));
                    }
                    else if (rightPosition.Y < mState.Y)
                    {
                        curAngle = Quaternion.Multiply(curAngle, Quaternion.CreateFromAxisAngle(lrAxis, 0.05f));
                    }
                    //TODO: dorobić obracanie
                    //curAngle *= new Quaternion(Vector3.Multiply(target - position, (new Vector3(mState.X, mState.Y, 1))), 15);
                    //hAngleDelta = ((MS.X - rightPosition.X) / 800.0f);
                    //vAngleDelta = ((rightPosition.Y - MS.Y) / (-600.0f));
                    //changeTarget();
                }
            }

            #endregion
            #region Mouse Border Handling
            int screenH = graphics.GraphicsDevice.ScissorRectangle.Height;
            int screenW = graphics.GraphicsDevice.ScissorRectangle.Width;

           

            if (mState.Y < 10)           target += fbAxis;
            if (mState.Y > screenH - 10) target -= fbAxis;
          
            if (mState.X < 10)           target += lrAxis;
            if (mState.X > screenW - 10) target -= lrAxis;

            #endregion
            #region Keyboard Handling
            if (kState.IsKeyDown(Keys.Left))
            {
                target += lrAxis;
            }
            if (kState.IsKeyDown(Keys.Right))
            {
                target -= lrAxis;
            }
            if (kState.IsKeyDown(Keys.Up))
            {
                target += fbAxis;
            }
            if (kState.IsKeyDown(Keys.Down))
            {
                target -= fbAxis;
            }

            #endregion
            
            calculatePosition();
            #region Trash01
            /**if (MS.RightButton == ButtonState.Pressed)
            {
                if (!rightDown)
                {
                    rightPosition.X = MS.X;
                    rightPosition.Y = MS.Y;
                    rightDown = true;
                }
                else
                {
                    hAngleDelta = ((MS.X - rightPosition.X) / 800.0f);
                    vAngleDelta = ((rightPosition.Y - MS.Y) / (-600.0f));
                    changeTarget();
                }
            }
            calculatePosition();
            if (MS.RightButton == ButtonState.Released)
            {
                vAngle += vAngleDelta;
                vAngleDelta = 0.0f;
                hAngle += hAngleDelta;
                hAngleDelta = 0.0f;
                rightDown = false;
            }*/
            #endregion
        }
		/// <summary>
		/// TODO: dodać opis bo nie wiem co to i na co to.
		/// </summary>
		/// <param name="mousePosition"></param>
		/// <param name="viewport"></param>
		/// <returns></returns>
		public Ray GetMouseRay(Vector2 mousePosition, Viewport viewport)
		{
			Vector3 near = new Vector3(mousePosition, 0);
			Vector3 far = new Vector3(mousePosition, 1);

			near = viewport.Unproject(near, projection, view, Matrix.Identity);
			far = viewport.Unproject(far, projection, view, Matrix.Identity);

			return new Ray(near, Vector3.Normalize(far - near));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Matrix View()
		{
			return view;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Matrix Projection()
		{
			return projection;
		}
	}
}