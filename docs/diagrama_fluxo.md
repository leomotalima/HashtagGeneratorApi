# 🧠 Diagrama de Fluxo — Hashtag Generator API

O diagrama abaixo mostra o fluxo de execução do projeto **Hashtag Generator API**, desde a requisição feita pelo usuário até a geração das hashtags pelo modelo do **Ollama**.

---

```mermaid
sequenceDiagram
    autonumber
    participant U as 🧑 Usuário
    participant A as ⚙️ HashtagGenerator API (.NET)
    participant O as 🧠 Ollama (Modelo LLM Local)

    U->>A: POST /hashtags (texto + quantidade)
    Note right of A: A API recebe a requisição JSON<br/>e monta o prompt de geração
    A->>O: POST /api/generate (prompt + modelo configurado)
    Note right of O: O Ollama processa o texto<br/>usando o modelo local (ex: llama3.2:3b)
    O-->>A: Retorna JSON {"hashtags": [...]}
    A-->>U: Retorna resposta final JSON com as hashtags
