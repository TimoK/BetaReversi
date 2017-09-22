# Copyright 2015 The TensorFlow Authors. All Rights Reserved.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# ==============================================================================

"""A very simple MNIST classifier.

See extensive documentation at
https://www.tensorflow.org/get_started/mnist/beginners
"""
from __future__ import absolute_import
from __future__ import division
from __future__ import print_function

import argparse
import sys
import numpy as np


from tensorflow.examples.tutorials.mnist import input_data

import tensorflow as tf

FLAGS = None

dimSize = 8
boardSize = dimSize * dimSize

def next_batch(num, data, labels):
    '''
    Return a total of `num` random samples and labels. 
    '''
    idx = np.arange(0 , len(data))
    np.random.shuffle(idx)
    idx = idx[:num]
    data_shuffle = [data[ i] for i in idx]
    labels_shuffle = [labels[ i] for i in idx]

    return np.asarray(data_shuffle), np.asarray(labels_shuffle)

def coord2d_to_coord1d(x, y):
    return y * 8 + x

def getTileValue(character, player):
    opponent = 'b'
    if(player == 'b'):
        opponent = 'w'
    if(character == player):
        return 1
    if(character == opponent):
        return -1
    return 0
    
def getBoardValue(boardString, player):
    boardValue = []
    for character in boardString:
        boardValue.append(getTileValue(character, player))
    return boardValue
    
def getData(line):
    inputRaw, xChar, yChar, player, endScoreChar = line.split(" ")
    x = int(xChar)
    y = int(yChar)
    coord1d = coord2d_to_coord1d(x, y)
    inputData = getBoardValue(inputRaw, player)
    outputData = np.zeros(boardSize)
    outputData[coord1d] = 1
    return inputData, outputData

def getDataFromFile(file, dataPoints):
    inputData = []
    outputData = []
    for dataPointCounter in range(dataPoints):
        line = file.readline()
        inputPoint, outputPoint = getData(line)
        inputData.append(inputPoint)
        outputData.append(outputPoint)
    return inputData, outputData

def weight_variable(shape):
    initial = tf.truncated_normal(shape, stddev=0.1)
    return tf.Variable(initial)

def bias_variable(shape):
    initial = tf.constant(0.1, shape=shape)
    return tf.Variable(initial)

def main(_):
    # Import data
    print("Starting to read data")
    file = open("../GameFiles/neuralNetworkInputs1999.txt", "r") 
    trainingSetInput, trainingSetOutput = getDataFromFile(file, 10000)
    validationSetInput, validationSetOutput = getDataFromFile(file, 500)
    testSetInput, testSetOutput = getDataFromFile(file, 5000)   
    file.close()
    
    print("Creating model")    
    hiddenLayerSize = 50
    # Create the model
    x = tf.placeholder(tf.float32, [None, boardSize])
    W_inputToHidden = weight_variable([boardSize, hiddenLayerSize])
    b_hidden = bias_variable([hiddenLayerSize])
    hiddenLayer = tf.sigmoid(tf.matmul(x, W_inputToHidden) + b_hidden)
    W_hiddenToOutput = weight_variable([hiddenLayerSize, boardSize])
    b_output = bias_variable([boardSize])
    y = tf.matmul(hiddenLayer, W_hiddenToOutput) + b_output

    # Define loss and optimizer
    y_ = tf.placeholder(tf.float32, [None, boardSize])

    # The raw formulation of cross-entropy,
    #
    #   tf.reduce_mean(-tf.reduce_sum(y_ * tf.log(tf.nn.softmax(y)),
    #                                 reduction_indices=[1]))
    #
    # can be numerically unstable.
    #
    # So here we use tf.nn.softmax_cross_entropy_with_logits on the raw
    # outputs of 'y', and then average across the batch.
    cross_entropy = tf.reduce_mean(tf.nn.softmax_cross_entropy_with_logits(labels=y_, logits=y))
    train_step = tf.train.GradientDescentOptimizer(0.5).minimize(cross_entropy)

    sess = tf.InteractiveSession()
    tf.global_variables_initializer().run()
    
    with tf.name_scope('accuracy'):
        correct_prediction = tf.equal(tf.argmax(y, 1), tf.argmax(y_, 1))
        correct_prediction = tf.cast(correct_prediction, tf.float32)
    accuracy = tf.reduce_mean(correct_prediction)
    
    print("Beginning training")
    # Train
    for batchTrainCounter in range(10000):
        batch_xs, batch_ys = next_batch(10, trainingSetInput, trainingSetOutput)
        if batchTrainCounter % 100 == 0:
            loggerBatch_training_x, loggerBatch_training_y = next_batch(500, trainingSetInput, trainingSetOutput)
            train_accuracy = accuracy.eval(feed_dict={
                x: loggerBatch_training_x, y_: loggerBatch_training_y})
            validation_accuracy = accuracy.eval(feed_dict={
                x: validationSetInput, y_: validationSetOutput})
            print('step %d, training accuracy %g, validation accuracy %g' % (batchTrainCounter, train_accuracy, validation_accuracy))
        sess.run(train_step, feed_dict={x: batch_xs, y_: batch_ys})

    # Test trained model
    correct_prediction = tf.equal(tf.argmax(y, 1), tf.argmax(y_, 1))
    
    print(sess.run(accuracy, feed_dict={x: testSetInput,
                                      y_: testSetOutput}))

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    FLAGS, unparsed = parser.parse_known_args()
    tf.app.run(main=main, argv=[sys.argv[0]] + unparsed)