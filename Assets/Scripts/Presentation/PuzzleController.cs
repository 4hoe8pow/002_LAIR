using Application;
using UnityEngine;

namespace Presentation
{
    public class PuzzleController : MonoBehaviour
    {
        private IPuzzleInputPort _inputPort;
        public void Initialize(IPuzzleInputPort inputPort)
        {
            _inputPort = inputPort;
        }
        public void OnTileSwipe(int x, int y, SwipeDirection direction)
        {
            _inputPort.OnTileSwipe(x, y, direction);
        }
    }
}
