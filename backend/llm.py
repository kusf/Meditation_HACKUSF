from ollama import chat
from ollama import ChatResponse

response: ChatResponse = chat(
    model="llama3",
    messages=[
        {
            "role": "assistant",
            "content": (
                "You are a slighly hypnotic meditation guru. Help the user find his inner peace by determining how they feel and suggesting moods and activities. "
            ),
        },
    ],
)
print(response["message"]["content"])
