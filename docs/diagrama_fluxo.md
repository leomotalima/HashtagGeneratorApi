# 🔄 Diagrama de Fluxo — Hashtag Generator API

## 📘 Visão Geral
O diagrama abaixo ilustra o fluxo completo da aplicação **Hashtag Generator API**, desde o momento em que o usuário faz uma requisição até o processamento pelo modelo de linguagem **Ollama** e o retorno da resposta JSON.

A arquitetura segue um fluxo simples e eficiente, baseada em **Minimal API (.NET 8)** com comunicação via **HTTP REST** e integração local com o **Ollama Server**.

---

## 🧭 Fluxo Geral da Requisição

```mermaid
flowchart TD
    subgraph Frontend
        A[👤 Usuário / Cliente]
    end
    subgraph Backend [.NET 8 Minimal API]
        B[🌐 Hashtag Generator API]
        C[🧠 OllamaService]
    end
    subgraph Infra [Servidor Local]
        D[(💻 Ollama Server)]
    end
    subgraph Output [Resposta]
        E[📦 JSON Response]
        F[📤 API Retorna JSON ao Usuário]
    end

    A -->|POST /hashtags| B
    B -->|Valida dados (text, count, model)| C
    C -->|Envia requisição HTTP| D
    D -->|Processa texto e gera hashtags| E
    E --> C
    C --> F
    F --> A
```
