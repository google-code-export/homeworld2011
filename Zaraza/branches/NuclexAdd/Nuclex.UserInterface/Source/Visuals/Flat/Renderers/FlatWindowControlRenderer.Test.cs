﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

#if UNITTEST

using System;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock;

using Nuclex.UserInterface.Controls.Desktop;

using Is = NMock.Is;

namespace Nuclex.UserInterface.Visuals.Flat.Renderers {

  /// <summary>Unit Test for the flat window control renderer</summary>
  [TestFixture]
  internal class FlatWindowControlRendererTest : ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public override void Setup() {
      base.Setup();

      this.window = new WindowControl();
      this.window.Title = "Title";
      this.window.Bounds = new UniRectangle(10, 10, 100, 100);
      Screen.Desktop.Children.Add(this.window);

      this.renderer = new FlatWindowControlRenderer();
    }

    /// <summary>Verifies that the renderer is able to render the window</summary>
    [Test]
    public void TestRenderNormal() {

      // Make sure the renderer draws at least a frame and the window's title
      MockedGraphics.Expects.One.Method(
        m => m.DrawElement(null, RectangleF.Empty)
      ).WithAnyArguments();

      MockedGraphics.Expects.One.Method(
        m => m.DrawString(null, RectangleF.Empty, null)
      ).With(Is.Anything, Is.Anything, Is.EqualTo("Title"));

      // Let the renderer draw the window into the mocked graphics interface
      renderer.Render(this.window, MockedGraphics.MockObject);

      // And verify the expected drawing commands were issues
      Mockery.VerifyAllExpectationsHaveBeenMet();

    }

    /// <summary>Window renderer being tested</summary>
    private FlatWindowControlRenderer renderer;
    /// <summary>Window used to test the window renderer</summary>
    private WindowControl window;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
