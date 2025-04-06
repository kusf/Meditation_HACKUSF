import time
from llm import LLM
from promptBridgeReader import *
from dataBridgeWriter import *
from voice import *

# This is the main loop for the code. This loop will first start the AI, throw data into the data bridge, the front end will pick it up, do what's necessary, and then write back to the prompt bridge where this file will
# continue the progress of the AI.

if __name__ == "__main__":

    model = LLM()           #runs the first prompt
    writeDict = model.get_data()        #create base dictionary to write to data bridge
    writeDict['id'] = -1
    writeDictToDataBridge(writeDict)     #passes dict into data bridge
    createVoiceWAV(model['text'], getDataBridgePath())      #create voice
    #writeImageToDataBridge()           #create background
    markDataBridgeComplete()

    oldID = -1
    while True:
        promptBridgeData = readPromptBridgeData()
        if promptBridgeData['id'] == oldID:
            time.sleep(5)       #halts the program for 5 seconds
            continue

        oldID = promptBridgeData['id']
        newPrompt = promptBridgeData['prompt']
        model(newPrompt)
        writeDict = model.get_data()
        writeDict['id'] = random.randint(0, 9999)
        writeDictToDataBridge(writeDict)     #passes first prompt results into data bridge
        createVoiceWAV(model['text'], getDataBridgePath())      #create voice
        markDataBridgeComplete()