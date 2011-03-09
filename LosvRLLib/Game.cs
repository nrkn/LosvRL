using System;
using System.Collections.Generic;
using System.IO;
using NrknLib.Color;
using NrknLib.Color.Extensions;
using NrknLib.ConsoleView;
using NrknLib.ConsoleView.ColorConverters;
using NrknLib.ConsoleView.Demo;
using NrknLib.Geometry;
using NrknLib.Geometry.Extensions;
using NrknLib.Geometry.Interfaces;

namespace LosvRLLib {
  public class Game {
    public Game( IConsoleView console ) {
      _mapPersistence = new Persistence<Map>();

      if( File.Exists( MapFile ) ) {
        _map = _mapPersistence.Load( MapFile );
      } else {
        _map = new Map();
        var generator = new Generator( _map );
        generator.GenerateLevel();
        _mapPersistence.Save( _map, MapFile );
      }
      Console = console;
      Console.HideCursor();
    }

    private const string MapFile = "map.bin";
    private readonly Map _map;
    private readonly Persistence<Map> _mapPersistence;
    public IConsoleView Console { get; set; }

    public List<object> Tick( string command ) {
      var oldLocation = new Point( _map.Location.X, _map.Location.Y );
      ExecuteAction( command );
      if( !_map.Noise.Bounds.InBounds( _map.PlayerLocation ) || _map.BlocksPlayer[ _map.PlayerLocation ] ) {
        _map.Location = new Point( oldLocation.X, oldLocation.Y );
      }
      _map.Fov = (Grid<bool>) _map.BlocksSight.Fov( 10, _map.PlayerLocation );

      _map.Seen.SetEach( ( seen, point ) =>
        seen || _map.Fov[ point ]
      );

      var viewportGrid = _map.Noise.Copy( _map.Viewport );

      viewportGrid.ForEach( p => {
        var point = new Point( p.X + _map.Location.X, p.Y + _map.Location.Y );

        var foregroundColor = GetForegroundColor( p, point );
        var backgroundColor = GetBackgroundColor( point );
        var value = ( p.Equals( _map.Center ) ? "@" : GetTile( point ) )[ 0 ];

        var cell = Console.GetCell( p.X, p.Y );
        var newCell = new ConsoleCell { ForegroundColor = foregroundColor, BackgroundColor = backgroundColor, Char = value, X = p.X, Y = p.Y };

        if( !cell.Equals( newCell ) ) {
          Console.SetCursorPosition( p.X, p.Y );
          Console.ForegroundColor = foregroundColor;
          Console.BackgroundColor = backgroundColor;
          Console.Write( value.ToString() );
        }
      } );

      return Console.Flush();
    }

    public void Save()
    {
      _mapPersistence.Save( _map, MapFile );
    }

    private string GetTile( Point point ) {
      return
        !_map.Noise.Bounds.InBounds( point ) || !_map.Fov[ point ] && !_map.Seen[ point ] ? " "
        : _map.Walls[ point ] && _map.Paths[ point ] ? "+"
        : _map.Walls[ point ] ? "#"
        : _map.Paths[ point ] && _map.Rivers[ point ] ? "="
        : _map.Paths[ point ] ? "."
        : _map.Rivers[ point ] ? "~"
        : _map.Mountains[ point ] ? "^"
        : _map.Trees[ point ];
    }

    private ConsoleColor GetForegroundColor( IPoint p, IPoint point, bool raw = false ) {
      return
        !raw && ( p.Equals( _map.Center ) || !_map.Noise.Bounds.InBounds( point ) ) ? ConsoleColor.White
        : !raw && !_map.Fov[ point ] && _map.Seen[ point ] ? ConsoleColor.Gray
        : _map.Paths[ point ] && _map.Mountains[ point ] ? ConsoleColor.Gray
        : _map.Mountains[ point ] ? ConsoleColor.White
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

    private ConsoleColor GetBackgroundColor( IPoint point, bool raw = false ) {
      return
        !raw && ( !_map.Noise.Bounds.InBounds( point ) || !_map.Seen[ point ] ) ? ConsoleColor.Black
        : !raw && !_map.Fov[ point ] && _map.Seen[ point ] ? ConsoleColor.DarkGray
        : _map.Walls[ point ] ? ConsoleColor.Gray
        : _map.Paths[ point ] && _map.Rivers[ point ] ? ConsoleColor.DarkYellow
        : _map.Rivers[ point ] ? ConsoleColor.DarkBlue
        : _map.Mountains[ point ] ? ConsoleColor.DarkYellow
        : _map.Paths[ point ] ? ConsoleColor.Green
        : ConsoleColor.DarkGreen;
    }

    private void Dump() {
      var color = new Grid<Rgba>( _map.Size );
      color.SetEach(
        ( rgba, point ) => {
          var cBack = GetBackgroundColor( point, true );
          var fore = ConsoleToRgbaConverter.FormatColor( GetForegroundColor( new Point(), point, true ) );
          var back = ConsoleToRgbaConverter.FormatColor( cBack );
          if( cBack == ConsoleColor.DarkGreen && _map.Trees[ point ] != "." ) {
            back = new Rgba( 0, 255, 0 );
            var noiseToLightness = ( ( _map.Noise[ point ] / 255.0 ) * 0.5 ) + 0.125;
            return back.SetBrightness( noiseToLightness * back.ToHsla().Lightness );
          }
          return back.Average( fore, 2 );
        }
      );
      File.WriteAllText( "map.ppm", color.ToPpm() );
      File.WriteAllText( "noise.pgm", _map.Noise.ToPgm() );
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
        case "77":
          Dump();
          break;
      }
      return Direction.None;
    }
  }
}