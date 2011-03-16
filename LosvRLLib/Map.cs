using System;
using System.Collections.Generic;
using NrknLib.Geometry;
using NrknLib.Geometry.Interfaces;

namespace LosvRLLib {
  [Serializable]
  public class Map {
    public Map() {
      Size = new Size( 1000, 1000 );
      ViewportSize = new Size( 48, 24 );
      Location = new Point( 0, 0 );
      PlayerLocation = new Point( Size.Width / 2, Size.Height - 2 );
      Noise = new Grid<byte>( Size );
      Paths = new Grid<bool>( Size );
      SilentPaths = new Grid<bool>( Size );
      Mountains = new Grid<bool>( Size );
      Rivers = new Grid<bool>( Size );
      Walls = new Grid<bool>( Size );
      Colors = new Grid<double>( Size );
      Trees = new Grid<string>( Size );
      BlocksPlayer = new Grid<bool>( Size );
      BlocksSight = new Grid<bool>( Size );
      Reachable = new Grid<bool>( Size );
      Fov = new Grid<bool>( Size );
      Seen = new Grid<bool>( Size );
      Hubs = new List<Point>();      
    }

    public IPoint Center {
      get { return new Point( ViewportSize.Width / 2, ViewportSize.Height / 2 ); }
    }

    public IPoint PlayerLocation {
      get {
        return new Point( Location.X + Center.X, Location.Y + Center.Y );
      }
      set {
        Location.X = value.X - Center.X;
        Location.Y = value.Y - Center.Y;
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
    public Grid<bool> BlocksPlayer { get; set; }
    public Grid<bool> BlocksSight { get; set; }
    public Grid<bool> Reachable { get; set; }
    public Grid<string> Trees { get; set; }
    public Grid<double> Colors { get; set; }
    public Grid<bool> Walls { get; set; }
    public Grid<bool> Rivers { get; set; }
    public Grid<bool> Paths { get; set; }
    public Grid<bool> SilentPaths { get; set; }
    public List<Point> Hubs { get; set; }
    public Grid<bool> Mountains { get; set; }
    public Size ViewportSize { get; set; }
    public Point Location;
  }
}