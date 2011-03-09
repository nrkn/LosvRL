using System.Collections.Generic;
using System.Linq;
using NrknLib.Geometry;
using NrknLib.Geometry.Extensions;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities;
using NrknLib.Utilities.Extensions;

namespace LosvRLLib {
  public class Generator {
    public Generator( Map map ) {
      _map = map;
    }

    private readonly Map _map;

    public void GenerateLevel() {
      GenerateNoise();
      GenerateMountains();
      GeneratePaths();
      GenerateRivers();
      GenerateWalls();
      GenerateColors();
      GenerateTrees();
      GenerateBlocksPlayer();
      GenerateBlocksSight();
    }

    private void GenerateMountains() {
      const int top = 1;
      var bottom = _map.Size.Height - 1;
      const int left = 1;
      var right = _map.Size.Width - 1;
      var leftCorner = (int) ( _map.Size.Width * 0.3 );
      var rightCorner = (int) ( _map.Size.Width * 0.7 );
      var topCorner = (int) ( _map.Size.Height * 0.3 );
      var bottomCorner = (int) ( _map.Size.Height * 0.7 );

      var linePoints = new[] {
        new Point( left, topCorner ),
        new Point( leftCorner, top ),
        new Point( rightCorner, top ),
        new Point( right, topCorner ),
        new Point( right, bottomCorner ),
        new Point( rightCorner, bottom ),
        new Point( leftCorner, bottom ),
        new Point( left, bottomCorner )
      };

      var lines = linePoints.Pairs( true ).Select( p => new Line( p.Item1, p.Item2 ) );
      var points = lines.SelectMany( line => line.DrunkenWalk( 0.75, _map.Noise.Bounds ) ).Distinct();

      foreach( var point in points ) {
        _map.Mountains[ point ] = true;
      }

      //fill in mountains
      var columns = _map.Mountains.Columns.ToList();
      var rows = _map.Mountains.Rows.ToList();

      for( var y = 0; y < rows.Count; y++ ) {
        var row = rows[ y ];
        var cells = row.ToList();
        var first = cells.IndexOf( true );
        var last = cells.LastIndexOf( true );   
        for( var x = 0; x < first; x++ ) {
          _map.Mountains[ x, y ] = true;
        }
        for( var x = last; x < cells.Count; x++ ) {
          _map.Mountains[ x, y ] = true;
        }
      }

      for( var x= 0; x < columns.Count; x++ ) {
        var column = columns[ x ];
        var cells = column.ToList();
        var first = cells.IndexOf( true );
        var last = cells.LastIndexOf( true );
        for( var y = 0; y < first; y++ ) {
          _map.Mountains[ x, y ] = true;
        }
        for( var y = last; y < cells.Count; y++ ) {
          _map.Mountains[ x, y ] = true;
        }        
      }
    }

    private void GenerateBlocksSight() {
      _map.BlocksSight.SetEach(
        ( block, point ) =>
          !_map.Paths[ point ] && ( 
            _map.Mountains[ point ]
            || !( new[]{ ".", "t", "T" } ).Contains( _map.Trees[ point ] )
            || _map.Walls[ point ]
          )
      );
    }

    private void GenerateBlocksPlayer() {
      _map.BlocksPlayer.SetEach(
        ( block, point ) =>
          !_map.Paths[ point ] && ( 
            _map.Mountains[ point ]
            || _map.Trees[ point ] != "."
            || _map.Walls[ point ]
            || _map.Rivers[ point ] 
          )
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
      //var building = new Rectangle( new Size( 5, 6 ) );

      //var wallPoints = new List<IPoint>();
      //foreach( var line in building.Lines ) {
      //  wallPoints.AddRange( line.Bresenham() );
      //}

      //foreach( var line in building.Lines.Translate( new Point( 10, 7 ) ) ) {
      //  wallPoints.AddRange( line.Bresenham() );
      //}

      //var rotated1 = new Point( 50, 7 );
      //foreach( var line in building.Lines.Translate( rotated1 ).Rotate( 45, rotated1 ) ) {
      //  wallPoints.AddRange( line.Bresenham() );
      //}

      //var rotated3 = new Point( 35, 7 );
      //foreach( var line in building.Lines.Translate( rotated3 ).Rotate( 90, rotated3 ) ) {
      //  wallPoints.AddRange( line.Bresenham() );
      //}

      //foreach( var point in wallPoints ) {
      //  _map.Walls[ point ] = true;
      //}
    }

    private void GenerateRivers() {
      //var line = new Line( new Point( 18, 1 ), new Point( _map.Size.Width - 20, _map.Size.Height - 1 ) );
      //var points = line.DrunkenWalk( 0.75, _map.Rivers.Bounds ).Distinct();

      //foreach( var point in points ) {
      //  _map.Rivers[ point ] = true;
      //}
    }

    private void GeneratePaths() {
      //var line1 = new Line( new Point( 1, 1 ), new Point( _map.Size.Width - 1, _map.Size.Height - 1 ) );
      //var line2 = new Line( new Point( 1, _map.Size.Height - 1 ), new Point( _map.Size.Width - 1, 1 ) );
      var line3 = new Line( new Point( 1, 1 ), _map.PlayerLocation );

      //var lines = new[] { line1, line2, line3 };
      var lines = new[] { line3 };
      var bounds = new Rectangle( 1, _map.Size.Width - 2, _map.Size.Height - 2, 1 );
      var points = lines.SelectMany( line => line.DrunkenWalk( 0.75, bounds ) ).Distinct();

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