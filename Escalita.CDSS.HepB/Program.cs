using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;

// Cargar configuración
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var openAiKey = config["OpenAI:ApiKey"];

if (string.IsNullOrEmpty(openAiKey))
{
    Console.WriteLine("La clave de OpenAI no está configurada.");
    return;
}

// Configurar Kernel Memory (modo serverless en memoria)
// Replace WithMemoryStore with the correct method or configuration if it does not exist
var memory = new KernelMemoryBuilder()
    .WithOpenAI(new OpenAIConfig
    {
        APIKey = openAiKey,
        TextModel = "gpt-3.5-turbo",
        EmbeddingModel = "text-embedding-ada-002"
    })
    .WithSimpleVectorDb() // <-- Reemplaza a VolatileMemoryStore
    .Build<MemoryServerless>();

// Cargar documentos PDF en memoria
var documentos = new Dictionary<string, string>
{
    { "doc001", "uptodate-hep-b.pdf" },
    //{ "doc002", "who-hep-b-guidance.pdf" },
    //{ "doc003", "aasld-hep-b-guidance.pdf" }
};

foreach (var doc in documentos)
{
    if (!File.Exists(doc.Value))
    {
        Console.WriteLine($"El archivo {doc.Value} no se encuentra.");
        continue;
    }

    Console.WriteLine($"Cargando documento: {doc.Value}");
    await memory.ImportDocumentAsync(doc.Value, documentId: doc.Key);
}

// Esperar a que los documentos estén listos
Console.WriteLine("Esperando a que se complete la ingesta de documentos...");

foreach (var docId in documentos.Keys)
{
    while (!await memory.IsDocumentReadyAsync(docId))
    {
        Console.WriteLine($"Ingestando {docId}...");
        await Task.Delay(1000);
    }
}

Console.WriteLine("\nTodos los documentos han sido ingeridos correctamente.");

// Interfaz de preguntas
Console.WriteLine("\nHaz una pregunta sobre los documentos cargados:");
var pregunta = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(pregunta))
{
    var respuesta = await memory.AskAsync(pregunta);

    Console.WriteLine($"\n➡ Pregunta: {pregunta}\n");
    Console.WriteLine($"✅ Respuesta:\n{respuesta.Result}\n");

    if (respuesta.RelevantSources?.Any() == true)
    {
        Console.WriteLine("📚 Fuentes relevantes:");
        foreach (var fuente in respuesta.RelevantSources)
        {
            var fecha = fuente.Partitions.FirstOrDefault()?.LastUpdate.ToString("yyyy-MM-dd") ??
                        "sin fecha";
            Console.WriteLine(
                $"- {fuente.SourceName} ({fuente.Link ?? "sin enlace"}) - Última actualización: {fecha}");
        }
    }
    else
    {
        Console.WriteLine("No se encontraron fuentes relevantes.");
    }
}
else
{
    Console.WriteLine("No se ingresó ninguna pregunta.");
}
