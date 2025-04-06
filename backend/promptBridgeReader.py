import json
import os
from pathlib import Path
import random

def getPromptBridgePath():
    return Path(os.path.dirname(os.path.realpath(__file__))).parent.absolute() / 'DataBridge' / 'PromptBridge.json'             #find Prompt Bridge in parent parent folder w/ write.


def readPromptBridgeData():
    returnResult = {}
    with open(getPromptBridgePath(), 'r') as file:       #find Prompt Bridge in parent parent folder w/ write.
        returnResult = json.load(file)
    return returnResult

def clearPromptBridge():
    clearDict = {}
    clearDict['id'] = -1
    with open(getPromptBridgePath(), 'w', encoding='utf-8') as file:       #find Data Bridge in parent parent folder w/ write.
        json.dump(clearDict, file, ensure_ascii=False, indent=4)        #dump dict