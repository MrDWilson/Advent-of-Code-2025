dev-dotnet:
	cd dotnet && dotnet build

dev-typescript:
	cd typescript && npm install -g bun && bun install

dev-python:
	cd python && python3 -m venv .venv && . .venv/bin/activate && pip install -r requirements.txt

run-dotnet:
	cd dotnet && dotnet run

run-typescript:
	cd typescript && bun run index.ts

run-python:
	cd python && if [ -d .venv ]; then . .venv/bin/activate; fi && python main.py