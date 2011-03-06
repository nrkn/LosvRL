using NrknLib.Geometry;
using NrknLib.Geometry.Interfaces;

namespace LosvRLLib
{
  public class World
  {
    public World()
    {
      Size = new Size(750, 750);
      ViewportSize = new Size(80, 24);
      Location = new Point(0, 0);
    }
    
    private IGrid<byte> _noise;
    private IGrid<bool> _paths;
    private IGrid<bool> _rivers;
    private IGrid<bool> _walls;
    private IGrid<double> _colors;
    private IGrid<char> _trees;
    private IGrid<bool> _blocks;
    private IGrid<bool> _fov;
    private IGrid<bool> _seen;

    private ISize _size;
    private ISize _viewportSize;
    private IPoint _location;

    private IPoint Center
    {
      get { return new Point(ViewportSize.Width / 2, ViewportSize.Height / 2); }
    }

    private IPoint PlayerLocation
    {
      get
      {
        return new Point(Location.X + Center.X, Location.Y + Center.Y);
      }
    }

    private IRectangle Viewport
    {
      get
      {
        return new Rectangle(
          Location.Y,
          Location.X + (ViewportSize.Width - 1),
          Location.Y + (ViewportSize.Height - 1),
          Location.X
        );
      }
    }

    public ISize Size
    {
      get { return _size; }
      set { _size = value; }
    }

    public IGrid<byte> Noise
    {
      get { return _noise; }
      set { _noise = value; }
    }

    public IGrid<bool> Seen
    {
      get { return _seen; }
      set { _seen = value; }
    }

    public IGrid<bool> Fov
    {
      get { return _fov; }
      set { _fov = value; }
    }

    public IGrid<bool> Blocks
    {
      get { return _blocks; }
      set { _blocks = value; }
    }

    public IGrid<char> Trees
    {
      get { return _trees; }
      set { _trees = value; }
    }

    public IGrid<double> Colors
    {
      get { return _colors; }
      set { _colors = value; }
    }

    public IGrid<bool> Walls
    {
      get { return _walls; }
      set { _walls = value; }
    }

    public IGrid<bool> Rivers
    {
      get { return _rivers; }
      set { _rivers = value; }
    }

    public IGrid<bool> Paths
    {
      get { return _paths; }
      set { _paths = value; }
    }

    public ISize ViewportSize
    {
      get { return _viewportSize; }
      set { _viewportSize = value; }
    }

    public IPoint Location
    {
      get { return _location; }
      set { _location = value; }
    }
  }
}
