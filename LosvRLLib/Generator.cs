using System;
using System.Collections.Generic;
using System.Linq;
using NrknLib.Geometry;
using NrknLib.Geometry.Extensions;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities;

namespace LosvRLLib {
  public class Generator {
    public Generator( Map map ) {
      _map = map;
    }

    private readonly Map _map;

    public void GenerateLevel() {
      var noiseStart = DateTime.Now;
      GenerateNoise();

      var pathsStart = DateTime.Now;
      GeneratePaths();

      var riversStart = DateTime.Now;
      GenerateRivers();

      var wallsStart = DateTime.Now;
      GenerateWalls();

      var colorsStart = DateTime.Now;
      GenerateColors();

      var treesStart = DateTime.Now;
      GenerateTrees();

      var blocksStart = DateTime.Now;
      GenerateBlocks();

      var end = DateTime.Now;

      //_log.AppendLine( "Generation time" );
      //_log.AppendLine( "  Noise   " + ( pathsStart - noiseStart ).TotalMilliseconds + "ms" );
      //_log.AppendLine( "  Paths   " + ( riversStart - pathsStart ).TotalMilliseconds + "ms" );
      //_log.AppendLine( "  Rivers  " + ( wallsStart - riversStart ).TotalMilliseconds + "ms" );
      //_log.AppendLine( "  Walls   " + ( colorsStart - wallsStart ).TotalMilliseconds + "ms" );
      //_log.AppendLine( "  Colors  " + ( treesStart - colorsStart ).TotalMilliseconds + "ms" );
      //_log.AppendLine( "  Trees   " + ( blocksStart - treesStart ).TotalMilliseconds + "ms" );
      //_log.AppendLine( "  Blocks  " + ( end - blocksStart ).TotalMilliseconds + "ms" );
      //_log.AppendLine( "  " + new String( '-', 20 ) );
      //_log.AppendLine( "  TOTAL   " + ( end - noiseStart ).TotalMilliseconds + "ms" );
    }

    private void GenerateBlocks() {
      _map.Blocks.SetEach(
        ( block, point ) =>
          !_map.Paths[ point ] &&
          ( _map.Trees[ point ] != "."
          || _map.Walls[ point ]
          || _map.Rivers[ point ] )
      );
    }

    private void GenerateTrees() {
      _map.Noise.ForEach(
        ( t, point ) =>
          _map.Trees[ point ] = DoubleToForestItem( t / 255.0 )
      );
    }

    private void GenerateColors() {
      _map.Colors.SetEach( RandomHelper.Random.NextDouble );
    }

    private void GenerateWalls() {
      var building = new Rectangle( new Size( 5, 6 ) );

      var wallPoints = new List<IPoint>();
      foreach( var line in building.Lines ) {
        wallPoints.AddRange( line.Bresenham() );
      }

      foreach( var line in building.Lines.Translate( new Point( 10, 7 ) ) ) {
        wallPoints.AddRange( line.Bresenham() );
      }

      var rotated1 = new Point( 50, 7 );
      foreach( var line in building.Lines.Translate( rotated1 ).Rotate( 45, rotated1 ) ) {
        wallPoints.AddRange( line.Bresenham() );
      }

      var rotated3 = new Point( 35, 7 );
      foreach( var line in building.Lines.Translate( rotated3 ).Rotate( 90, rotated3 ) ) {
        wallPoints.AddRange( line.Bresenham() );
      }

      foreach( var point in wallPoints ) {
        _map.Walls[ point ] = true;
      }
    }

    private void GenerateRivers() {
      var line = new Line( new Point( 18, 1 ), new Point( _map.Size.Width - 20, _map.Size.Height - 1 ) );
      var points = line.DrunkenWalk( 0.75, _map.Rivers.Bounds ).Distinct();

      foreach( var point in points ) {
        _map.Rivers[ point ] = true;
      }
    }

    private void GeneratePaths() {
      var line1 = new Line( new Point( 1, 1 ), new Point( _map.Size.Width - 1, _map.Size.Height - 1 ) );
      var line2 = new Line( new Point( 1, _map.Size.Height - 1 ), new Point( _map.Size.Width - 1, 1 ) );
      var line3 = new Line( new Point( 1, 1 ), _map.Center );

      var lines = new[] { line1, line2, line3 };
      var points = lines.SelectMany( line => line.DrunkenWalk( 0.5, _map.Noise.Bounds ) ).Distinct();

      foreach( var point in points ) {
        _map.Paths[ point ] = true;
      }
    }

    private void GenerateNoise() {
      _map.Noise = (Grid<byte>) new Grid<byte>( _map.Size ).Generate( 0.0525, 1.0, 0.75, 5 );
    }

    static string DoubleToForestItem( double value ) {
      return
        RandomHelper.Random.NextDouble() > value ?
          value < 0.5 ? "♠"
          : value < 0.6 ? "♣"
          : value < 0.7 ? "T"
          : "t"
        : ".";
    }
  }
}