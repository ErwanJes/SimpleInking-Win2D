using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;

namespace UnicornNote.Inking
{
  public class Win2DPath
  {
    private ICanvasResourceCreator _device;
    private CanvasPathBuilder _pathBuilder;
    private CanvasGeometry _geometry;
    private Vector2 _tmpVector1;
    private Vector2 _tmpVector2;

    public Win2DPath(ICanvasResourceCreator device)
    {
      _device = device;
      _tmpVector1 = new Vector2();
      _tmpVector2 = new Vector2();
    }

    public CanvasGeometry GetGeometry()
    {
      return _geometry;
    }

    public void MoveTo(float x, float y)
    {
      _geometry = null;
      _pathBuilder = new CanvasPathBuilder(_device);
      _pathBuilder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.Winding);
      _pathBuilder.BeginFigure(x, y);
    }

    public void LineTo(float x, float y)
    {
      _pathBuilder.AddLine(x, y);
    }

    public void QuadTo(float cx, float cy, float ax, float ay)
    {
      _tmpVector1.X = cx;
      _tmpVector1.Y = cy;
      _tmpVector2.X = ax;
      _tmpVector2.Y = ay;
      _pathBuilder.AddQuadraticBezier(_tmpVector1, _tmpVector2);
    }

    public void Close()
    {
      _pathBuilder.EndFigure(CanvasFigureLoop.Open);
      _geometry = CanvasGeometry.CreatePath(_pathBuilder);
      _pathBuilder = null;
    }
  }
}
