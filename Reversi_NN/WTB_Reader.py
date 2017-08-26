'''
Created on 26 aug. 2017

@author: Timo
'''               

def getScore(board, player):
    score = 0;
    for x in range(8):
        for y in range(8):
            if board[x][y] == player:
                score += 1
    return score

def outOfBounds(x, y):
    if x < 0 or y < 0 or x >= 8 or y >= 8:
        return True
    else:
        return False

def getCoordinates(tormove):
    x = int((tormove / 10) - 1)
    y = int((tormove % 10) - 1)
    return [x, y]
    

def getStartBoard():
    board = []
    for i in range(8):
        board.append([0] * 8)
    board[3][3] = -1
    board[3][4] = 1
    board[4][3] = 1
    board[4][4] = -1
    return board

def getDirections():
    directions = []
    for x in range(-1, 2):
        for y in range(-1, 2):
            if x != 0 or y != 0:
                directions.append([x, y])
    return directions

directions = getDirections()

def playMove(board, x, y, player):
    board[x][y] = player
    opponent = -player
    # print('Move performed: ', x + 1, " ", y + 1)
    for direction in directions:
        move_x = x
        move_y = y
        pointsToChange = []
        needToChange = False
        while True:
            move_x += direction[0]
            move_y += direction[1]
            if outOfBounds(move_x, move_y) or board[move_x][move_y] == 0:
                break
            if board[move_x][move_y] == player:
                needToChange = True
                break
            if board[move_x][move_y] == opponent:
                pointsToChange.append([move_x, move_y])
        if needToChange:
            for [point_x, point_y] in pointsToChange:
                board[point_x][point_y] = player
                
def listPossibleMoves(board, player):
    possibleMoves = []
    opponent = -player
    for x in range(8):
        for y in range(8):
            if board[x][ y] != 0:
                continue
            for direction in directions:
                move_x = x
                move_y = y
                opponentEncountered = False
                while True:
                    move_x += direction[0]
                    move_y += direction[1]
                    if outOfBounds(move_x, move_y) or board[move_x][ move_y] == 0:
                        break
                    if board[move_x][move_y] == player:
                        if opponentEncountered:
                            possibleMoves.append([move_x, move_y])
                        break
                    if board[move_x][move_y] == opponent:
                        opponentEncountered = True
    return possibleMoves

NNInput = []

def getNNInput(board, move_coordinates, player):
    neuralNetworkInput = ""
    for y in range(8):
        for x in range(8):
            if(board[x][y] == 0):
                neuralNetworkInput += "e"
            if(board[x][y] == 1):
                neuralNetworkInput += "w"
            if(board[x][y] == -1):
                neuralNetworkInput += "b"
    neuralNetworkInput += " " +  str(move_coordinates[0]) + " " + str(move_coordinates[1]) + " "
    if(player == 1):
        neuralNetworkInput += "w"
    else:
        neuralNetworkInput += "b"
    return neuralNetworkInput

def appendMarginalScoreToNNInput(gameNNInput, whiteMarginalScore):
    blackMarginalScore = -whiteMarginalScore
    gameNNInputWithScore = []
    for NNinputString in gameNNInput:
        if(NNinputString[len(NNinputString) - 1] == 'w'):
            NNinputString += " " + str(whiteMarginalScore)
        else:
            NNinputString += " " + str(blackMarginalScore)
        gameNNInputWithScore.append(NNinputString)
    return gameNNInputWithScore
            
def playGame(tormoves):
    gameNNInput = []
    board = getStartBoard()
    player = 1
    turnCounter = 1
    for tormove in tormoves:
        if tormove == 0:
            break
        coordinates = getCoordinates(tormove)
        gameNNInput.append(getNNInput(board, coordinates, player))
        playMove(board, coordinates[0], coordinates[1], player)
        if listPossibleMoves(board, -player):
            player = -player
        
        # print("White Score: ", (getScore(board, 1), "  Black Score: ", getScore(board, -1), "  Turn Number: " , turnCounter))
        turnCounter += 1
    whiteMarginalScore = getScore(board, 1) - getScore(board, -1)
    gameNNInput = appendMarginalScoreToNNInput(gameNNInput, whiteMarginalScore)
    NNInput.extend(gameNNInput)
    
if __name__ == '__main__':   
    gameFile = open("../GameFiles/WTH_1999.wtb", "rb")
    
    header = gameFile.read(16)     
            
    ended = False
    gamesMoves = []
    while True:
        gameheader = gameFile.read(8)
        if ended:
            break
        gameMoves = []
        for i in range(0, 60):
            moveByteList = gameFile.read(1)
            if not moveByteList:
                ended = True
                break
            moveByte = moveByteList[0]
            gameMoves.append(moveByte)
            #print(moveByte)
        gamesMoves.append(gameMoves)
        #print(' ')
        # Remove the line below to load in more than 1 game
        # break
    gameCounter = 1
    for gameMoves in gamesMoves:
        if gameCounter % 100 == 0:
            print("Playing game ", gameCounter, " out of ", len(gamesMoves), ".")
        gameCounter += 1 
        playGame(gameMoves)
    gameFile.close()
    print("Getting to the writing part")
    nnFile = open("../GameFiles/neuralNetworkInputs.txt","w")
    for NNInputString in NNInput:
        nnFile.write(NNInputString)
        nnFile.write("\n")
    nnFile.close()
    print("Done")


    



    
    
    
    
    
    
    
    
    
      
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    