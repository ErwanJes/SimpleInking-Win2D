using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public delegate void PenActionHandler(Stroke stroke, bool isTemporary, object context);
    public event PenActionHandler PenDraw;

    public delegate void PenInvalidateHandler(Extent extent, bool isTemporary);
    public event PenInvalidateHandler PenInvalidated;

    private Stroke _stroke { get; set; }
    private bool _isTemporary;

    public void PenDown(CaptureInfo captureInfo)
    {
      _isTemporary = true;
      _stroke = new Stroke();
      _stroke.AddPoint(captureInfo);
    }

    public void PenMove(CaptureInfo captureInfo)
    {
      _stroke.AddPoint(captureInfo);

      Extent e = new Extent();
      if (PenInvalidated != null)
        PenInvalidated(e, _isTemporary);
    }

    public void PenUp(CaptureInfo captureInfo)
    {
      _isTemporary = false;
      _stroke.AddPoint(captureInfo);

      Extent e = new Extent();
      if (PenInvalidated != null)
        PenInvalidated(e, _isTemporary);
    }

    public void Draw(Extent extent, RenderContext context)
    {
      if (PenDraw != null)
        PenDraw(_stroke, _isTemporary, context);
    }

  }
}
