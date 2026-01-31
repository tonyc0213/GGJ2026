using System.Collections.Generic;
using Data.ScriptableObject.Abstract;
using UnityEngine;

namespace Identification
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SnippetPositions")]
    public class SnippetPositionSO : ScriptableObjectIndex<int, List<Vector3>> { }
}