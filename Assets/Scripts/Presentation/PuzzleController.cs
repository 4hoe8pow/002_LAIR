using Application;
using Domain;
using UnityEngine;

namespace Presentation
{
    // コントローラ: InputHandlerから呼ばれる
    public class PuzzleController : MonoBehaviour
    {
        private IPuzzleInputPort _inputPort;
        public void Initialize(IPuzzleInputPort inputPort)
        {
            _inputPort = inputPort;
        }
        public void OnTileSwipe(TileAddress address, SwipeDirection direction)
        {
            _inputPort.OnTileSwipe(address, direction);
        }
    }
}
