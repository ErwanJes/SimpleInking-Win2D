// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Microsoft.Graphics.Canvas.Geometry;
using System.Collections.Generic;
using Windows.Foundation;

namespace SimpleSample
{
  /// <summary>
  /// Draws some graphics using Win2D
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public const int POINTER_ID_INACTIVE = -1;

    private int _activePointerId;

    private InkingManager _inkingManager;

    public MainPage()
    {
      this.InitializeComponent();
      canvasRendering.Height = 10000;
      canvasInking.Height = 10000;

      _activePointerId = POINTER_ID_INACTIVE;

      _inkingManager = new InkingManager(canvasInking, canvasRendering);
    }

    private void CanvasControl_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
      var point = e.GetCurrentPoint(sender as UIElement);

      if (_activePointerId == POINTER_ID_INACTIVE)
      {
        if (e.Pointer.IsInContact)
        {
          _activePointerId = (int)point.PointerId;
          _inkingManager.Pen.PenDown(new CaptureInfo
          {
            X = (float) point.Position.X,
            Y = (float) point.Position.Y,
          });
        }
      }
    }

    private void CanvasControl_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
      if (_activePointerId != POINTER_ID_INACTIVE)
      {
        var point = e.GetCurrentPoint(sender as UIElement);

        _inkingManager.Pen.PenMove(new CaptureInfo
        {
          X = (float)point.Position.X,
          Y = (float)point.Position.Y,
        });
      }
    }

    private void CanvasControl_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
      if (_activePointerId != POINTER_ID_INACTIVE)
      {
        var point = e.GetCurrentPoint(sender as UIElement);

        _inkingManager.Pen.PenUp(new CaptureInfo
        {
          X = (float)point.Position.X,
          Y = (float)point.Position.Y,
        });
      }

      _activePointerId = POINTER_ID_INACTIVE;
    }

    private void Grid_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
    {

    }
  }
}
