from transformers import pipeline
from datasets import load_dataset
import soundfile as sf
import torch
import os
from pathlib import Path

synthesiser = pipeline("text-to-speech", "microsoft/speecht5_tts")
embeddings_dataset = load_dataset("Matthijs/cmu-arctic-xvectors", split="validation")
speaker_embedding = torch.tensor(embeddings_dataset[7306]["xvector"]).unsqueeze(0)
# You can replace this embedding with your own as well.

def getPromptBridgePath():
    currentProgramPath = Path(os.path.dirname(os.path.realpath(__file__)))      #get this files parent folder
    return str(currentProgramPath.parent.absolute()) + '\\DataBridge\\voice.wav'             #find Prompt Bridge in parent parent folder w/ write.

def createVoiceWAV(inputString):
    speech = synthesiser(inputString, forward_params={"speaker_embeddings": speaker_embedding})
    sf.write(getPromptBridgePath(), speech["audio"], samplerate=speech["sampling_rate"])