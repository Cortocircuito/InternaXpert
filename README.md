# Document Q&A con OpenAI y Kernel Memory

Este proyecto es una aplicación de consola en C# que permite cargar documentos PDF en memoria utilizando [Microsoft Kernel Memory](https://github.com/microsoft/kernel-memory) y realizar preguntas sobre su contenido mediante la API de OpenAI (GPT-3.5-Turbo).

## Características

- Carga de documentos PDF en memoria para procesamiento.
- Uso de embeddings con el modelo `text-embedding-ada-002`.
- Consulta semántica a través de la API de OpenAI.
- Respuestas enriquecidas con referencias a fuentes relevantes del documento.

## Requisitos

- [.NET 7.0 o superior](https://dotnet.microsoft.com/en-us/download)
- Cuenta y clave de API de OpenAI
- Documentos PDF válidos para análisis

## Configuración

1. Clona este repositorio:

   ```bash
   git clone https://github.com/tu-usuario/tu-repositorio.git
   cd tu-repositorio
