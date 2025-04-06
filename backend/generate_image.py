import torch
from diffusers import StableDiffusionPipeline

def generate_image(prompt: str) -> None:
    model_id = "OFA-Sys/small-stable-diffusion-v0"
    pipe = StableDiffusionPipeline.from_pretrained(model_id, torch_dtype=torch.float16)
    pipe = pipe.to("mps") #mac only, use cuda for other

    image = pipe(prompt).images[0]  
    
    image.save("image.png")

