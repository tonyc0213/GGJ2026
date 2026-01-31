using Data.ScriptableObject.Abstract;
using UnityEngine;

namespace PartsGen
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ColorTable")]
    public class ColorTable : ScriptableObjectIndex<int, Color> { }
}