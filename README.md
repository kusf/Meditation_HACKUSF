# Meditation_HACKUSF

## Requirements
To install python requirements
```bash
python -m pip install -r requirements.txt
  
```

To run Ollama. Make the model from the MakeFile.  Make sure that you have qwen2.5 installed. Then serve.
```bash
ollama create model -f "backend/ModelFile" && ollama serve
```

