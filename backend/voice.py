from transformers import pipeline
from datasets import load_dataset
import soundfile as sf
import torch


synthesiser = pipeline("text-to-speech", "microsoft/speecht5_tts")

embeddings_dataset = load_dataset("Matthijs/cmu-arctic-xvectors", split="validation")
speaker_embedding = torch.tensor(embeddings_dataset[7306]["xvector"]).unsqueeze(0)
# You can replace this embedding with your own as well.

# Read the text from the "voiceText.txt" file
with open("voiceText.txt", "r") as file:
    text = file.read()

speech = synthesiser(text, forward_params={"speaker_embeddings": speaker_embedding})

sf.write("speech.wav", speech["audio"], samplerate=speech["sampling_rate"])


