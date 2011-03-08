using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LosvRLLib {
  public class Persistence<T> where T : class {
    public T Load( string name )
    {
      var formatter = new BinaryFormatter();
      var stream = new FileStream( name, FileMode.Open, FileAccess.Read, FileShare.Read );
      var obj = (T)formatter.Deserialize(stream);
      stream.Close();
      return obj;
    }

    public void Save( T obj, string name )
    {
      var formatter = new BinaryFormatter();
      var stream = new FileStream( name, FileMode.Create, FileAccess.Write, FileShare.None );
      formatter.Serialize( stream, obj );
      stream.Close();      
    }
  }
}