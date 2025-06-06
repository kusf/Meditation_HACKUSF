import time
from llm import LLM
from promptBridgeReader import *
from dataBridgeWriter import *
from voice import *
from generate_image import generate_image

# This is the main loop for the code. This loop will first start the AI, throw data into the data bridge, the front end will pick it up, do what's necessary, and then write back to the prompt bridge where this file will
# continue the progress of the AI.

if __name__ == "__main__":

    print("Clearing past data...")
    clearDataBridge()
    clearPromptBridge()

    print("Generating First Prompt...")
    model = LLM()           #runs the first prompt
    model.print_data()
    writeDict = model.get_data()        #create base dictionary to write to data bridge
    writeDict['id'] = random.randint(0, 9999)       #generate random ID

    f = open(getVoiceTXTPath(), "w")        #write value of the prompt for voice transcription
    f.write(writeDict["text"])
    f.close()

    createVoiceWAV()        #create voice
    generate_image(model.get_data()["image description"])       #generate image
    print("Writen to data bridge.")
    print('\n')

    writeDictToDataBridge(writeDict)     #passes dict into data bridge
    oldID = -1
    while True:
        promptBridgeData = readPromptBridgeData()       #waits for the prompt to respond.
        if promptBridgeData['id'] == oldID:
            print("Sleeping...")
            time.sleep(5)       #halts the program for 5 seconds
            continue

        print("Generating Text...")
        print('\n')
        oldID = promptBridgeData['id']
        newPrompt = promptBridgeData['prompt']
        model(newPrompt)
        model.print_data()
        writeDict = model.get_data()
        writeDict['id'] = random.randint(0, 9999)
        f = open(getVoiceTXTPath(), "w")
        f.write(writeDict["text"])
        f.close()
        createVoiceWAV()
        generate_image(model.get_data()["image description"])
        writeDictToDataBridge(writeDict)     #passes first prompt results into data bridge
    
