using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

var builder = Kernel.CreateBuilder();
// 👇🏼 Using Phi-4 with Ollama
builder.AddOllamaChatCompletion("llama2", new Uri("http://localhost:11434"));

var kernel = builder.Build();

ChatCompletionAgent agent = new() // 👈🏼 Definition of the agent
{
    Instructions = "Answer questions about C# and .NET",
    Name = "C# Agent",
    Kernel = kernel
};
Console.WriteLine("Type 'exit' to stop the chat.");
Console.WriteLine("Ask me anything about C# and .NET!");
string myChat = Console.ReadLine();
while (myChat != "exit")
{
    ChatHistory chat =
[
    new ChatMessageContent(AuthorRole.User,
                           myChat)
];

    await foreach (var response in agent.InvokeAsync(chat))
    {
        chat.Add(response);
        Console.WriteLine(response.Message.Content);
    }
    Console.WriteLine("Ask me anything else!");
    Console.WriteLine("Type 'exit' to stop the chat.");
    myChat = Console.ReadLine();
}
