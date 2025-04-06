
import json
import os
from pathlib import Path
import random

def getDataBridgePath():
    currentProgramPath = Path(os.path.dirname(os.path.realpath(__file__)))
          #get this files parent folder
    return currentProgramPath.parent.absolute() / 'DataBridge' / 'DataBridge.json'             #find Data Bridge in parent parent folder w/ write.

def writeDictToDataBridge(inputDict):
    with open(getDataBridgePath(), 'w', encoding='utf-8') as file:       #find Data Bridge in parent parent folder w/ write.
        json.dump(inputDict, file, ensure_ascii=False, indent=4)        #dump dict

def markDataBridgeComplete(inputDict):
    inputDict['id'] = random.randint(0, 9999 + 1)           #generate random ID
    writeDictToDataBridge(inputDict)

def clearDataBridge(inputDict):
    inputDict['id'] = 0
    with open(getDataBridgePath(), 'w', encoding='utf-8') as file:       #find Data Bridge in parent parent folder w/ write.
        json.dump(inputDict, file, ensure_ascii=False, indent=4)        #dump dict


#Not done
def writeImageToDataBridge(inputDict):
    with open('data.json', 'w', encoding='utf-8') as f:
        json.dump(inputDict, f, ensure_ascii=False, indent=4)
