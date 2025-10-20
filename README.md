<p align="center">
  <img src="https://cdn-icons-png.flaticon.com/512/1006/1006771.png" alt="Hashtag Generator API Logo" width="160"/>
</p>


<h1 align="center"><i><b>Hashtag Generator API</b></i> - Checkpoint (FIAP)</h1>

<p align="center">
**Disciplina:** Advanced Business Development with .NET  
**Professor Orientador:** Leonardo Gasparini Romão  
Minimal API desenvolvida em <b>.NET 8</b> que integra com o <b>Ollama</b> para geração inteligente de hashtags via modelo de linguagem local.
</p>

---

## 🏷️ Etiquetas

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)  
[![C#](https://img.shields.io/badge/C%23-Minimal%20API-green.svg)](https://learn.microsoft.com/aspnet/core)  
[![Ollama](https://img.shields.io/badge/AI-Ollama-black.svg)](https://ollama.com/)  
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)  
[![FIAP](https://img.shields.io/badge/FIAP-2TDSB-red.svg)](https://www.fiap.com.br/)

---

## 🎯 Visão Geral

O **Hashtag Generator API** é uma **Minimal API** desenvolvida em **.NET 8** que utiliza o **Ollama** — um modelo de linguagem local — para gerar hashtags relevantes a partir de textos enviados pelo usuário.

O projeto demonstra o uso prático de:  
- Consumo de **APIs REST locais** com `HttpClient`  
- **Structured Outputs (JSON Schema)**  
- **Boas práticas REST**  
- Integração com IA **executando localmente**, sem dependência de nuvem  

> 💡 Desenvolvido como parte da disciplina **Advanced Business Development with .NET**, aplicando integração de APIs com Inteligência Artificial Local (Ollama).

---

## 🧠 Arquitetura do Sistema

O projeto segue uma arquitetura **Minimalista e Configurável**, separando lógica e parâmetros no `appsettings.json`.

### 🧩 Diagrama de Fluxo (Mermaid)

```mermaid
sequenceDiagram
    autonumber
    participant U as 🧑 Usuário
    participant A as ⚙️ HashtagGenerator API (.NET)
    participant O as 🧠 Ollama (Modelo LLM Local)

    U->>A: POST /hashtags (texto + quantidade)
    A->>O: POST /api/generate (prompt estruturado)
    O-->>A: Retorna JSON {"hashtags": [...]}
    A-->>U: Retorna JSON final com as hashtags
```

---

## ⚙️ Funcionalidades Principais

- ✅ Endpoint `POST /hashtags` para geração de hashtags  
- 🧩 Integração local com **Ollama API**  
- ⚙️ Configuração flexível via `appsettings.json`  
- 📡 Consumo com `HttpClient`  
- 💾 Teste via arquivo `.http` ou Postman  
- 🧱 Estrutura limpa e extensível  

---

## 🧰 Tecnologias Utilizadas

- **.NET 8 (Minimal API)**  
- **C# 12**  
- **Ollama (local LLM)**  
- **HttpClient**  
- **Swagger / JSON**  
- **VS Code / Postman**  

---

## 🧩 Estrutura do Projeto

```
HASHTAGGENERATORAPI/
├── bin/
├── Controllers/
│   └── HashtagController.cs
├── docs/
│   ├── arquitetura.md
│   ├── diagrama_fluxo.md
│   └── endpoints.md
├── Models/
│   ├── HashtagRequest.cs
│   └── HashtagResponse.cs
├── obj/
├── Services/
│   └── OllamaService.cs
├── .gitattributes
├── .gitignore
├── appsettings.json
├── HashtagGeneratorApi.csproj
├── Program.cs
├── README.md
└── test.http
```

<p align="center">
  [🎥 Assistir à demonstração completa no YouTube](https://youtu.be/_2VPrjS74WY)
</p>

---

## 📄 Documentação da API

### 🔹 Geração de Hashtags

```http
POST /hashtags
Content-Type: application/json

{
  "text": "Inteligência Artificial aplicada em análise de dados",
  "count": 8,
  "model": "llama3:latest"
}
```

**Resposta esperada:**

```json
{
  "hashtags": [
    "#InteligenciaArtificial",
    "#DataScience",
    "#MachineLearning",
    "#BigData",
    "#Automacao"
  ]
}
```

---

## ⚙️ Configuração e Execução

1️⃣ **Instalar o .NET 8**  
[https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)  

2️⃣ **Instalar o Ollama**  
[https://ollama.com/download](https://ollama.com/download)  

3️⃣ **Baixar modelo leve**  

```bash
ollama pull llama3.2:3b
```

4️⃣ **Confirmar se o Ollama está rodando**  

```bash
curl http://localhost:11434/api/tags
```

5️⃣ **Executar a API**  

```bash
dotnet run
```

6️⃣ **Testar o endpoint**  
Use o `test.http` ou Postman para enviar o POST `/hashtags`.

---

## 🧩 Exemplo appsettings.json

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3.2:3b"
  }
}
```

---

## 🧪 Testes Locais

Execute:

```bash
dotnet run
```

A aplicação responderá em:

```
http://localhost:5000
```

Envie requisições com:  
- `test.http` (VS Code)  
- `curl`  
- `Postman`  

---

## 🧪 Testes Detalhados da Hashtag Generator API

<details>
<summary>🔹 Clique para expandir os testes dos endpoints</summary>

### 🔹 Teste do endpoint `/ping`

```http
GET http://localhost:5000/ping
```

### 🔹 Testes do endpoint `/hashtags`

**1️⃣ Count informado**

```http
POST http://localhost:5000/hashtags
Content-Type: application/json

{
  "text": "Inteligência Artificial aplicada em análise de dados",
  "count": 8,
  "model": "llama3.2:3b"
}
```

**2️⃣ Count não informado (padrão = 10)**

```http
POST http://localhost:5000/hashtags
Content-Type: application/json

{
  "text": "Teste sem count",
  "model": "llama3.2:3b"
}
```

**3️⃣ Count maior que 30 (limite máximo)**

```http
POST http://localhost:5000/hashtags
Content-Type: application/json

{
  "text": "Teste count alto",
  "count": 50,
  "model": "llama3.2:3b"
}
```

**4️⃣ Modelo vazio (usar padrão)**

```http
POST http://localhost:5000/hashtags
Content-Type: application/json

{
  "text": "Teste sem modelo",
  "count": 5,
  "model": ""
}
```

**5️⃣ Campo `text` vazio**

```http
POST http://localhost:5000/hashtags
Content-Type: application/json

{
  "text": "",
  "count": 5,
  "model": "llama3.2:3b"
}
```

**6️⃣ Corpo nulo**

```http
POST http://localhost:5000/hashtags
Content-Type: application/json

null
```

**7️⃣ Falha Ollama (opcional: desligar Ollama para testar fallback)**

```http
POST http://localhost:5000/hashtags
Content-Type: application/json

{
  "text": "Teste falha Ollama",
  "count": 5,
  "model": "llama3.2:3b"
}
```

</details>

---

## 🧠 Aprendizados

Durante o desenvolvimento, foram aplicadas:  
- Boas práticas de integração API ↔️ IA Local  
- Prompts estruturados e controle de saída JSON  
- Configuração de ambiente via `appsettings.json`  
- Uso de Minimal API com código limpo e eficiente

---

## 📚 Documentação Técnica

| Documento | Descrição |
|------------|------------|
| [🏗️ Arquitetura do Sistema](docs/arquitetura.md) | Estrutura geral da API, camadas e responsabilidades |
| [🔄 Diagrama de Fluxo](docs/diagrama_fluxo.md) | Fluxo completo da requisição, do cliente ao Ollama Server |
| [🧩 Endpoints da API](docs/endpoints.md) | Detalhamento das rotas, parâmetros e exemplos de requisições |

---

📦 **Repositório GitHub:** [leomotalima/HashtagGeneratorApi](https://github.com/leomotalima/HashtagGeneratorApi)

---

## 👨‍💻 Autor

| Nome | RM | Responsabilidade |
|------|----|------------------|
| **Léo Mota Lima** | 557851 | Desenvolvimento da API, integração com Ollama, documentação e diagrama técnico |

---

## 📜 Licença

Distribuído sob a licença **MIT**.  
Consulte [LICENSE](https://choosealicense.com/licenses/mit/) para mais detalhes.

---

## 🔗 Referências

- [Ollama Docs](https://ollama.com/library)  
- [Microsoft Docs – Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)  
- [FIAP - Advanced Business Development with .NET](https://www.fiap.com.br/)

