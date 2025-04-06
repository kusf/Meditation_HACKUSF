import re
from ollama import Client
from ollama import ChatResponse


class LLM:
    def __init__(self):
        self.client = Client(host="http://localhost:11434")
        response: ChatResponse = self.client.chat(
            model="model",
            messages=[
                {
                    "role": "user",
                    "content": "Hello, I am a new person. Please give me a new adventure and <options>!",
                },
            ],
        )
        self.data = self.formatted_return(response["message"]["content"])

    def __call__(self, option: str):
        response: ChatResponse = self.client.chat(
            model="model",
            messages=[
                {
                    "role": "user",
                    "content": option,
                },
            ],
        )
        self.data = self.formatted_return(response["message"]["content"])

    def formatted_return(self, input) -> dict:
        try:
            return {
                "text": input[: input.index("<option>")],
                "options": re.findall(r"<option>(.+)</option>", input),
                "image description": re.findall(r"<image>(.+)</image>", input)[0],
                "intensity": re.findall(r"<intensity>(.+)</intensity>", input)[0],
            }
        except re.PatternError:
            self.__call__(
                "Please include a responde that has <options> <image> and <intensity>"
            )
            return {
                "text": input[: input.index("<option>")],
                "options": re.findall(r"<option>(.+)</option>", input),
                "image description": re.findall(r"<image>(.+)</image>", input)[0],
                "intensity": re.findall(r"<intensity>(.+)</intensity>", input)[0],
            }

    # returns the LLM response
    def get_data(self):
        return self.data

    def print_data(self):
        print(self.data)
