using System;
using NrknLib.Geometry;
using NrknLib.Geometry.Interfaces;

namespace LosvRLLib {
  [Serializable]
  public class Map {
    public Map() {
      Size = new Size( 750, 750 );
      ViewportSize = new Size( 80, 24 );
      Location = new Point( 0, 0 );

      Noise = new Grid<byte>( Size );
      Paths = new Grid<bool>( Size );
      Rivers = new Grid<bool>( Size );
      Walls = new Grid<bool>( Size );
      Colors = new Grid<double>( Size );
      Trees = new Grid<string>( Size );
      Blocks = new Grid<bool>( Size );
      Fov = new Grid<bool>( Size );
      Seen = new Grid<bool>( Size );
    }

    public IPoint Center {
      get { return new Point( ViewportSize.Width / 2, ViewportSize.Height / 2 ); }
    }

    public IPoint PlayerLocation {
      get {
        return new Point( Location.X + Center.X, Location.Y + Center.Y );
      }
    }

    public IRectangle Viewport {
      get {
        return new Rectangle(
          Location.Y,
          Location.X + ( ViewportSize.Width - 1 ),
          Location.Y + ( ViewportSize.Height - 1 ),
          Location.X
        );
      }
    }

    public Size Size { get; set; }
    public Grid<byte> Noise { get; set; }
    public Grid<bool> Seen { get; set; }
    public Grid<bool> Fov { get; set; }
    public Grid<bool> Blocks { get; set; }
    public Grid<string> Trees { get; set; }
    public Grid<double> Colors { get; set; }
    public Grid<bool> Walls { get; set; }
    public Grid<bool> Rivers { get; set; }
    public Grid<bool> Paths { get; set; }
    public Size ViewportSize { get; set; }
    public Point Location;
  }
}