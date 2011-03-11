using System.Collections.Generic;
using System.IO;
using System.Linq;
using NrknLib.Geometry;
using NrknLib.Geometry.Extensions;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities;

namespace Tests {
  class Program {
    static void Main( string[] args ) {
      const int size = 1000;
      var blocks = new Grid<bool>( new Size( size, size ) );
      blocks.SetEach( () => RandomHelper.Random.NextDouble() < 0.4  );
      var line = new Line( new Point( 0, 0 ), new Point( size - 1, size - 1 ) );
      var points = line.DrunkenWalk( 0.25, blocks.Bounds ).Distinct();
      var pointsHash = new HashSet<IPoint>( points );
      blocks.SetEach( ( value, point ) => pointsHash.Contains( point ) ? false : value );

      var flood = blocks.FloodFill( new Point( 0, 0 ) );

      var fill = new Grid<bool>( new Size( size, size ) );
      foreach( var point in flood ) {
        fill[ point ] = true;
      }

      File.WriteAllText( "blocks.pbm", blocks.ToPbm() );
      File.WriteAllText( "fill.pbm", fill.ToPbm() );
    }
  }
}
