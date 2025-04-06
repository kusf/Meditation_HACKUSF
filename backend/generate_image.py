import torch
from diffusers import StableDiffusionPipeline

def generate_image(prompt: str) -> None:
    model_id = "OFA-Sys/small-stable-diffusion-v0"
    pipe = StableDiffusionPipeline.from_pretrained(model_id, torch_dtype=torch.float16)
    pipe = pipe.to("mps") #mac only, use cuda for other

    image = pipe("I am going to give you a prompt. Make this picture atmospheric and spiritual, yet not religion. Do not inlude human beings. I repeat, no humans." + prompt).images[0]  
    
    image.save("image.png")

