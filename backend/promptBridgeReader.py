import json
import os
from pathlib import Path
import random

def getPromptBridgePath():
    return Path(os.path.dirname(os.path.realpath(__file__))).parent.absolute() / 'DataBridge' / 'PromptBridge.json'             #find Prompt Bridge in parent parent folder w/ write.


def readPromptBridgeData():
    returnResult = {}
    with open(getPromptBridgePath(), 'r') as file:       #find Prompt Bridge in parent parent folder w/ write.
        returnResult = json.loads(file)
    return returnResult['prompt']