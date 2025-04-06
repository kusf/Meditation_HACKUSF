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
    return Path(os.path.dirname(os.path.realpath(__file__))).parent.absolute() / 'DataBridge' / 'voice.wav'             #find Prompt Bridge in parent parent folder w/ write.

def createVoiceWAV(inputString):
    speech = synthesiser(inputString, forward_params={"speaker_embeddings": speaker_embedding})
    sf.write(getPromptBridgePath(), speech["audio"], samplerate=speech["sampling_rate"])

#print(pipeline.tokenizer)
## Testing The Wav Creation
# speech = synthesiser("As we begin this meditation, I invite you to take a deep breath in through", forward_params={"speaker_embeddings": speaker_embedding})
# # Read the text from the "voiceText.txt" file
# with open("voiceText.txt", "r") as file:
#     text = file.read()

# speech = synthesiser(text, forward_params={"speaker_embeddings": speaker_embedding})

# sf.write("speech.wav", speech["audio"], samplerate=speech["sampling_rate"])

# sf.write("speech.wav", speech["audio"], samplerate=speech["sampling_rate"])
