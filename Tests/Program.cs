using System;
using NrknLib.Color;
using NrknLib.Color.Extensions;

namespace Tests {
  class Program {
    static void Main( string[] args ) {
      var green = new Rgba( 0, 255, 0 );
      var hsl = green.ToHsla();
      var random = new Random();
      var b = (byte) random.Next( 256 );
      var darkGreen = green.SetBrightness( green.ToHsla().Lightness * 0.5 );
      Console.WriteLine( green.ToHtml() );
      Console.WriteLine( green.ToHsla().ToRgba().ToHtml() );
      Console.WriteLine( hsl.Hue + " " + hsl.Saturation + " " + hsl.Lightness );
      Console.WriteLine( darkGreen.ToHtml() );
      Console.ReadKey();
    }
  }
}
