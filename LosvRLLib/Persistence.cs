using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace LosvRLLib {
  public class Persistence<T> where T : class {
    public Persistence() {
      _jsonSerializer = new DataContractJsonSerializer( typeof( T ) );
    }

    private readonly DataContractJsonSerializer _jsonSerializer;
    
    public T LoadJson( string name ) {
      var json = File.ReadAllText( name, Encoding.UTF8 );
      var memoryStream = new MemoryStream( Encoding.UTF8.GetBytes( json ) );
      var t = _jsonSerializer.ReadObject( memoryStream ) as T;
      memoryStream.Close();
      return t;
    }

    public void SaveJson( T obj, string name ) {
      var memoryStream = new MemoryStream();
      _jsonSerializer.WriteObject( memoryStream, obj );
      var json = Encoding.Default.GetString( memoryStream.ToArray() );
      File.WriteAllText( name, json, Encoding.UTF8 );    
    }
  }
}