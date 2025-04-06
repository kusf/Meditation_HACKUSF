from transformers import pipeline
from datasets import load_dataset
import soundfile as sf
import torch
import os
from pathlib import Path
from nltk import tokenize, download 

from pydub import AudioSegment
from pydub.effects import low_pass_filter

download('punkt_tab')

synthesiser = pipeline("text-to-speech", "microsoft/speecht5_tts")
embeddings_dataset = load_dataset("Matthijs/cmu-arctic-xvectors", split="validation")
speaker_embedding = torch.tensor(embeddings_dataset[7306]["xvector"]).unsqueeze(0)
# You can replace this embedding with your own as well.

def getPromptBridgePath():
    return Path(os.path.dirname(os.path.realpath(__file__))).parent.absolute() / 'DataBridge' / 'voice.wav'             #find Prompt Bridge in parent parent folder w/ write.

def createVoiceWAV(inputString):
    speech = synthesiser(inputString, forward_params={"speaker_embeddings": speaker_embedding})
    sf.write(getPromptBridgePath(), speech["audio"], samplerate=speech["sampling_rate"])

def text_batches(text: str, max: int = 600):
    sentences = tokenize.sent_tokenize(text, language="english")
    #print(sentences) # Sentences
    batches = []
    temp = ""
    for sentence in sentences:
        if len(temp) + len(sentence) < max:
            temp += sentence + " "
        else:
            batches.append(temp)
            temp = sentence

    batches.append(temp)
    #print(batches)
    return batches

## Testing The Wav Creation
# Read the text from the "voiceText.txt" file

def getVoiceTXTPath():
    return Path(os.path.dirname(os.path.realpath(__file__))).parent.absolute() / 'DataBridge' / 'voice.txt'             #find Prompt Bridge in parent parent folder w/ write.
with open(getVoiceTXTPath(), "r") as file:
    text = text_batches(file.read())

# Cancatenate the text 
# Initialize an empty list to store the audio data
all_audio_data = []

for batch in text: 
    #print(batch) #Prints the sentences
    speech = synthesiser(batch, forward_params={"speaker_embeddings": speaker_embedding})
    
    #print(speech)
    
    audio_tensor = torch.from_numpy(speech["audio"])

    # Append the audio data to the list
    all_audio_data.append(audio_tensor)

# Concatenate all the audio data into one single array
final_audio = torch.cat(all_audio_data, dim=0)

# THIS IS HARD CODED, use your function for the path here if you want
#sf.write("speech.wav", final_audio.numpy(), samplerate=speech["sampling_rate"])

#print(type(speech["audio"]))
#print("done with og voice")    # Original voice

#==================== reverb
# Load the generated audio
audio = AudioSegment.from_wav("speech.wav")

# Create a fake "reverb" by overlaying delayed and filtered versions
def apply_reverb(sound, delay_ms=100, decay=0.5, repeats=3):
    combined = sound
    for i in range(1, repeats + 1):
        # Create a softer echo
        echo = sound - (i * 9)  # lower volume with each repeat

        # Add silence in front of the echo to simulate delay
        delay_segment = AudioSegment.silent(duration=i * delay_ms)
        echo = delay_segment + echo

        # Optionally low-pass filter to simulate muffled reflection
        echo = low_pass_filter(echo, cutoff=1500)

        # Overlay echo onto the original sound
        combined = combined.overlay(echo)
    return combined


# Apply the reverb effect
reverb_audio = apply_reverb(audio)

# Export the new version
reverb_audio.export(getPromptBridgePath(), format="wav")
#print("done with reverb")   # Reverb voice
#==================== reverb