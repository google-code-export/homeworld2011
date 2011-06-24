﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

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

using Nuclex.Input;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Input;

using Is = NMock.Is;

namespace Nuclex.UserInterface.Visuals.Flat.Renderers {

  /// <summary>Unit Test for the flat vertical slider control renderer</summary>
  [TestFixture]
  internal class FlatVertialSliderControlRendererTest : ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public override void Setup() {
      base.Setup();

      this.slider = new VerticalSliderControl();
      this.slider.ThumbSize = 0.5f;
      this.slider.ThumbPosition = 0.5f;
      this.slider.Bounds = new UniRectangle(10, 10, 100, 100);
      Screen.Desktop.Children.Add(this.slider);

      this.renderer = new FlatVerticalSliderControlRenderer();
    }

#if false // Sliders can't be disabled currently
    /// <summary>
    ///   Verifies that the slider renderer can render disabled sliders
    /// </summary>
    [Test]
    public void TestRenderDisabled() {
      this.slider.Enabled = false;
      drawAndExpectState("disabled");
    }
#endif

    /// <summary>
    ///   Verifies that the slider renderer can render normal sliders
    /// </summary>
    [Test]
    public void TestRenderNormal() {
      drawAndExpectState("normal");
    }

    /// <summary>
    ///   Verifies that the slider renderer can render highlighted sliders
    /// </summary>
    [Test]
    public void TestRenderHighlighted() {
      Screen.InjectMouseMove(20.0f, 55.0f); // Move the mouse cursor over the thumb
      drawAndExpectState("highlighted");
    }

    /// <summary>
    ///   Verifies that the slider renderer can render depressed sliders
    /// </summary>
    [Test]
    public void TestRenderDepressed() {

      // Move the mouse cursor over the slider and press it
      Screen.InjectMouseMove(20.0f, 55.0f);
      Screen.InjectMousePress(MouseButtons.Left);
      drawAndExpectState("depressed");

    }

    /// <summary>
    ///   Lets the renderer draw the slider and verifies that the slider used
    ///   the skin elements from the expected state
    /// </summary>
    /// <param name="expectedState">
    ///   Expected state the skin elements should be using
    /// </param>
    private void drawAndExpectState(string expectedState) {

      // Make sure the renderer draws at least two frames
      MockedGraphics.Expects.One.Method(
        m => m.DrawElement(null, RectangleF.Empty)
      ).WithAnyArguments();
      MockedGraphics.Expects.One.Method(m => m.DrawElement(null, RectangleF.Empty)).With(
        Is.StringContaining(expectedState), Is.Anything
      );

      // Let the renderer draw the slider into the mocked graphics interface
      renderer.Render(this.slider, MockedGraphics.MockObject);

      // And verify the expected drawing commands were issues
      Mockery.VerifyAllExpectationsHaveBeenMet();

    }

    /// <summary>Slider renderer being tested</summary>
    private FlatVerticalSliderControlRenderer renderer;
    /// <summary>Slider used to test the slider renderer</summary>
    private VerticalSliderControl slider;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
