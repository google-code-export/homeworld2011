﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2011 Nuclex Development Labs

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
using System.Collections.Generic;
using System.Windows.Forms;

using NUnit.Framework;
using NMock;

namespace Nuclex.Input {

  /// <summary>Unit tests for the controller detector</summary>
  [TestFixture]
  internal class ControllerEventArgsTest {

    /// <summary>Verifies that the default constructor is working</summary>
    [Test]
    public void TestDefaultConstructor() {
      var arguments = new ControllerEventArgs();
      Assert.IsFalse(arguments.PlayerIndex.HasValue);
    }

    /// <summary>Verifies that the PlayerIndex constructor is working</summary>
    [Test]
    public void TestPlayerIndexConstructor() {
      var arguments = new ControllerEventArgs(ExtendedPlayerIndex.Seven);
      Assert.IsTrue(arguments.PlayerIndex.HasValue);
      Assert.AreEqual(ExtendedPlayerIndex.Seven, arguments.PlayerIndex.Value);
    }

  }

} // namespace Nuclex.Input

#endif // UNITTEST
