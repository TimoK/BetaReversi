The WTH file(s) come from http://www.ffothello.org/informatique/la-base-wthor/

The files are then preprocessed by the WTB_reader.py to generate the neural network inputs. The neuralNetworkInputsSmall file is the first 200 games from 1999, the neuralNetworkInputs1999 are all the 7686 recorded games from 1999.

On each line of the neural network inputs, we have the following:
First the state of the board, from left to right and then down to up (or up to down? Not sure, check this).
Then the x coordinate of the move that followed the board state
Then the y coordinate of the move that followed the board state
Then the color of the player that made the move
Then the final marginal score of that player (so player score minus opponent score)

We want the coordinates of the move for the policy network, and the final marginal score for the value network.

'b': Black
'w': White
'e': Empty