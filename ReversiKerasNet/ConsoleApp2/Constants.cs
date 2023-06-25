using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiNeuralNet
{
    public static class Constants
    {
        /// <summary>
        /// The length of the board in one direction
        /// </summary>
        public const int BOARD_LENGTH = 8;
        /// <summary>
        /// The number of tiles on the board
        /// </summary>
        public const int BOARD_TILES = BOARD_LENGTH * BOARD_LENGTH;
        /// <summary>
        /// The place where we save and load our files from. Bit hacky to work with a relative path instead of resources, but can be refactored later.
        /// </summary>
        public const string FILE_PATH = "..\\..\\..\\..\\..\\Gamefiles\\";
    }
}
