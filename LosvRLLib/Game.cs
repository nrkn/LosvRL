using System;
using System.Collections.Generic;
using System.IO;
using NrknLib.ConsoleView;
using NrknLib.ConsoleView.Demo;
using NrknLib.Geometry;
using NrknLib.Geometry.Extensions;
using NrknLib.Geometry.Interfaces;

namespace LosvRLLib {
  public class Game {
    public Game( IConsoleView console ) {
      var mapPersistence = new Persistence<Map>();

      if( File.Exists( mapFile ) ) {
        _map = mapPersistence.LoadJson( mapFile );
      } else {
        _map = new Map();
        var generator = new Generator( _map );
        generator.GenerateLevel();
        mapPersistence.SaveJson( _map, mapFile );
      }
      Console = console;
      UseBuffer = true;
    }

    private const string mapFile = "map.js";
    private readonly Map _map;
    public IConsoleView Console { get; set; }
    public bool UseBuffer { get; set; }

    public List<object> Tick( string command ) {
      var oldLocation = new Point( _map.Location.X, _map.Location.Y );
      var direction = ExecuteAction( command );
      if( !_map.Noise.Bounds.InBounds( _map.PlayerLocation ) || _map.Blocks[ _map.PlayerLocation ] ) {
        _map.Location = new Point( oldLocation.X, oldLocation.Y );
        direction = Direction.None;
      }
      _map.Fov = (Grid<bool>) _map.Blocks.Fov( 10, _map.PlayerLocation );

      _map.Seen.SetEach( ( seen, point ) =>
        seen || _map.Fov[ point ]
      );

      var source = new Rectangle( _map.ViewportSize );
      var target = new Point( 0, 0 );
      var oldCenter = new Point( _map.Center.X, _map.Center.Y );
      if( UseBuffer ) {
        switch( direction ) {
          case Direction.Left:
            source.Right--;
            target.X++;
            oldCenter.X++;
            break;
          case Direction.Right:
            source.Left++;
            oldCenter.X--;
            break;
          case Direction.Up:
            source.Bottom--;
            target.Y++;
            oldCenter.Y++;
            break;
          case Direction.Down:
            source.Top++;
            oldCenter.Y--;
            break;
        }
      }

      Console.SetCursorPosition( 0, 0 );

      var viewportGrid = _map.Noise.Copy( _map.Viewport );

      var tiles = new List<ConsoleCell>();

      viewportGrid.ForEach( p => {
        var point = new Point( p.X + _map.Location.X, p.Y + _map.Location.Y );

        var draw =
          !UseBuffer
          || direction == Direction.None
          || p.Equals( _map.Center )
          || p.Equals( oldCenter )
          || ( direction == Direction.Left && p.X == 0 )
          || ( direction == Direction.Right && p.X == viewportGrid.Width - 1 )
          || ( direction == Direction.Up && p.Y == 0 )
          || ( direction == Direction.Down && p.Y == viewportGrid.Height - 1 );

        tiles.Add(
          draw ? new ConsoleCell {
            ForegroundColor = GetForegroundColor( p, point ),
            BackgroundColor = GetBackgroundColor( point ),
            Char = ( p.Equals( _map.Center ) ? "@" : GetTile( point ) )[ 0 ]
          }
          : default( ConsoleCell )
        );
      } );

      if( UseBuffer ) Console.MoveBufferArea( source.Left, source.Top, source.Width, source.Height, target.X, target.Y );
      Console.Blit( tiles );

      return Console.Flush();
    }

    private string GetTile( Point point ) {
      return
        !_map.Noise.Bounds.InBounds( point ) || !_map.Fov[ point ] && !_map.Seen[ point ] ? " "
        : _map.Walls[ point ] && _map.Paths[ point ] ? "+"
        : _map.Walls[ point ] ? "#"
        : _map.Paths[ point ] && _map.Rivers[ point ] ? "="
        : _map.Paths[ point ] ? "."
        : _map.Rivers[ point ] ? "~"
        : _map.Trees[ point ];
    }

    private ConsoleColor GetForegroundColor( IPoint p, Point point ) {
      return
        p.Equals( _map.Center ) || !_map.Noise.Bounds.InBounds( point ) ? ConsoleColor.White
        : !_map.Fov[ point ] && _map.Seen[ point ] ? ConsoleColor.Gray
        : _map.Walls[ point ] ? ConsoleColor.DarkGray
        : _map.Paths[ point ] && _map.Rivers[ point ] ? ConsoleColor.DarkRed
        : _map.Paths[ point ] ? ConsoleColor.DarkGreen
        : _map.Rivers[ point ] ? ConsoleColor.Blue
        : _map.Colors[ point ] < 0.75 ? ConsoleColor.Green
        : _map.Colors[ point ] < 0.96 ? ConsoleColor.Yellow
        : _map.Colors[ point ] < 0.97 ? ConsoleColor.DarkRed
        : _map.Colors[ point ] < 0.98 ? ConsoleColor.DarkMagenta
        : ConsoleColor.Cyan;
    }

    private ConsoleColor GetBackgroundColor( Point point ) {
      return
        !_map.Noise.Bounds.InBounds( point ) || !_map.Seen[ point ] ? ConsoleColor.Black
        : !_map.Fov[ point ] && _map.Seen[ point ] ? ConsoleColor.DarkGray
        : _map.Walls[ point ] ? ConsoleColor.Gray
        : _map.Paths[ point ] && _map.Rivers[ point ] ? ConsoleColor.DarkYellow
        : _map.Paths[ point ] ? ConsoleColor.Green
        : _map.Rivers[ point ] ? ConsoleColor.DarkBlue
        : ConsoleColor.DarkGreen;
    }

    private Direction ExecuteAction( string command ) {
      switch( command ) {
        case "38":
          _map.Location.Y--;
          return Direction.Up;
        case "40":
          _map.Location.Y++;
          return Direction.Down;
        case "37":
          _map.Location.X--;
          return Direction.Left;
        case "39":
          _map.Location.X++;
          return Direction.Right;
        case "87":
          goto case "38";
        case "65":
          goto case "37";
        case "68":
          goto case "39";
        case "83":
          goto case "40";
      }
      return Direction.None;
    }
  }
}