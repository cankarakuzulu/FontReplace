using UnityEditor;
using UnityEngine;

namespace Voodoo.Sauce.Font
{
    public class AssetInfo : System.IEquatable<AssetInfo>
    {
        public string guid;
        public string path;
        public bool isLegal;

        Object _object;
        public Object Object => _object ?? (_object = AssetDatabase.LoadAssetAtPath<Object>(path));
        
        public override int GetHashCode() => guid.GetHashCode() ^ path.GetHashCode();
        public override bool Equals(object other) => Equals(other as AssetInfo);
        public bool Equals(AssetInfo other) => guid.Equals(other.guid) && path.Equals(path);
        public static bool operator ==(AssetInfo v1, AssetInfo v2) => v1.Equals(v2) == true;
        public static bool operator !=(AssetInfo v1, AssetInfo v2) => v1.Equals(v2) == false;
    }

}
