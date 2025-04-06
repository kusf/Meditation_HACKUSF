import json
import os
from pathlib import Path
import generate_image

def getDataBridgePath():
    return (
        Path(os.path.dirname(os.path.realpath(__file__))).parent.absolute()
        / "DataBridge"
        / "DataBridge.json"
    )  # find Prompt Bridge in parent parent folder w/ write.

def writeDictToDataBridge(inputDict):
    with open(
        getDataBridgePath(), "w", encoding="utf-8"
    ) as file:  # find Data Bridge in parent parent folder w/ write.
        json.dump(inputDict, file, ensure_ascii=False, indent=4)  # dump dict

def clearDataBridge():
    clearDict = {}
    clearDict['id'] = -1
    with open(getDataBridgePath(), 'w', encoding='utf-8') as file:       #find Data Bridge in parent parent folder w/ write.
        json.dump(clearDict, file, ensure_ascii=False, indent=4)        #dump dict

# Not done
def writeImageToDataBridge(inputDict):
    with open("data.json", "w", encoding="utf-8") as f:
        json.dump(inputDict, f, ensure_ascii=False, indent=4)
