using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornNote.Inking;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;

namespace SimpleSample
{
  public struct Extent
  {
    float xmin;  /**< The minimal x value. */
    float ymin;  /**< The minimal y value. */
    float xmax;  /**< The maximal x value. */
    float ymax;  /**< The maximal y value. */
    public Extent(float x, float y, float x2, float y2)
    {
      xmin = x;
      ymin = y;
      xmax = x2;
      ymax = y2;
    }
  };

  public class RenderContext
  {
    public RenderContext(CanvasDrawingSession session, Extent extent)
    {
      Session = session;
      Extent = extent;
    }
    public CanvasDrawingSession Session;
    public Extent Extent;
  }

  class InkingManager
  {
    public Pen Pen { get; set; }

    private List<CanvasGeometry> _previousGeometry;

    private CanvasVirtualControl _canvasInking;
    private CanvasVirtualControl _canvasRendering;
    private Win2DPath _pathBuilder;

    public InkingManager(CanvasVirtualControl canvasInking, CanvasVirtualControl canvasRendering)
    {
      _canvasInking = canvasInking;
      _canvasInking.RegionsInvalidated += OnRegionsInvalidated;

      _canvasRendering = canvasRendering;
      _canvasRendering.RegionsInvalidated += OnRegionsInvalidated;

      _previousGeometry = new List<CanvasGeometry>();

      Pen = new Pen();
      Pen.PenDraw += OnPenDraw;
      Pen.PenInvalidated += OnPenInvalidated;
    }

    private void OnRegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
    {
      foreach (var region in args.InvalidatedRegions)
      {
        using (var session = sender.CreateDrawingSession(region))
        {
          var extent = new Extent((float)region.Left, (float)region.Top, (float)region.Right, (float)region.Bottom);
          Pen.Draw(extent, new RenderContext(session, extent));
        }
      }
    }

    private async void OnPenInvalidated(Extent extent, bool isTemporary)
    {
      if (isTemporary)
      {
        if (!_canvasInking.ReadyToDraw)
          return;

        await _canvasInking.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
          var screenExtentPx = new Rect(0, 0, (float)_canvasInking.Size.Width, (float)_canvasInking.Size.Height);
          _canvasInking.Invalidate(screenExtentPx);
        });
      }
      else
      {
        if (!_canvasRendering.ReadyToDraw)
          return;

        await _canvasRendering.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
          var screenExtentPx = new Rect(0, 0, (float)_canvasInking.Size.Width, (float)_canvasInking.Size.Height);
          _canvasRendering.Invalidate(screenExtentPx);
        });
      }
    }

    private void OnPenDraw(Stroke stroke, bool isTemporary, object context)
    {
      if (stroke == null || stroke.Points == null)
        return;

      // chose the right canvas
      if (isTemporary)
        _pathBuilder = new Win2DPath(_canvasInking);
      else
        _pathBuilder = new Win2DPath(_canvasRendering);

      // create the path
      {
        var pointsCount = stroke.Points.Count;
        for (int i = 0; i < pointsCount; i++)
        {
          var point = stroke.Points[i];
          if (i == 0)
            _pathBuilder.MoveTo(point.X, point.Y);
          else
            _pathBuilder.LineTo(point.X, point.Y);
        }
        _pathBuilder.Close();
      }

      // draw
      var geometry = _pathBuilder.GetGeometry();
      if (geometry != null)
      {
        var renderContext = context as RenderContext;
        var session = renderContext.Session;

        if (isTemporary)
          session.DrawGeometry(geometry, Colors.Blue, 3);
        else
        {
          // draw previous stroke, kind of ugly :/
          _previousGeometry.Add(geometry);

          foreach (var previousGeometry in _previousGeometry)
            session.DrawGeometry(previousGeometry, Colors.Blue, 3);
        }
      }

    }
  }
}
