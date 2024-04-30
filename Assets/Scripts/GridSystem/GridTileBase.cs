using UnityEngine;
using UnityEngine.Serialization;

namespace GridSystem
{
    public class GridTileBase : MonoBehaviour
    {
        public LandType LandType;
        public int LandIndex;

        public void Init(int landIndex)
        {
            if (LandType == LandType.Land)
                LandIndex = landIndex;
        }
    }

    public enum LandType
    {
        Ocean,
        Land
    }
}