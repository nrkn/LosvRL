using System;
using LosvRLLib;
using NrknLib.ConsoleView;

namespace LosvRLConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      var console = new SystemConsoleView();
      //.NET Console.MoveBufferArea is too slow for the buffering to be useful.
      //it's actually faster to just update the whole screen
      var game = new Game( console ){ UseBuffer = false };
      var command = String.Empty;
      do {
        game.Tick( command );
        command = Console.ReadKey( true ).Key.ToCommand();
      } while( command != ConsoleKey.Escape.ToCommand() );
      game.Save();
    }
  }

  public static class Extensions {
    public static string ToCommand( this ConsoleKey info ) {
      return ( (int) info ).ToString();
    }
  }
}
