using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SimpleSample
{
  public class Stroke
  {
    public List<CaptureInfo> Points { get; set; }

    public void AddPoint(CaptureInfo point)
    {
      if (Points == null)
        Points = new List<CaptureInfo>();

      Points.Add(point);
    }
  }

  public class Pen
  {
    private List<Stroke> _previousStrokes;
    public delegate void PenActionHandler(Stroke stroke, bool isTemporary, object context);
    public event PenActionHandler PenDraw;

    public delegate void PenInvalidateHandler(Extent extent);
    public event PenInvalidateHandler PenInvalidated;

    private Stroke _stroke { get; set; }

    public Pen()
    {
      _previousStrokes = new List<Stroke>();
    }

    public void PenDown(CaptureInfo captureInfo)
    {
      if (_stroke != null)
        _previousStrokes.Add(_stroke);

      _stroke = new Stroke();
      _stroke.AddPoint(captureInfo);
    }

    public void PenMove(CaptureInfo captureInfo)
    {
      _stroke.AddPoint(captureInfo);

      Extent e = new Extent();
      if (PenInvalidated != null)
        PenInvalidated(e);
    }

    public void PenUp(CaptureInfo captureInfo)
    {
      _stroke.AddPoint(captureInfo);

      Extent e = new Extent();
      if (PenInvalidated != null)
        PenInvalidated(e);
    }

    public void Draw(Extent extent, RenderContext context)
    {
      if (PenDraw != null)
      {
        foreach (var previousStroke in _previousStrokes)
          PenDraw(previousStroke, false, context);
        PenDraw(_stroke, true, context);
      }
    }

  }
}
