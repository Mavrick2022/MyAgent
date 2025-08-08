using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

var builder = Kernel.CreateBuilder();
// Add the Ollama chat completion service
// Ensure you have Ollama running and the model "llama3.2:3b" is available
// You can change the model name and URI as needed
builder.AddOllamaChatCompletion("llama3.2:3b", new Uri("http://localhost:11434"));


var kernel = builder.Build();

ChatCompletionAgent agent = new() // 👈🏼 Definition of the agent
{
    Instructions = "You are an agent who creates funny stories. If you find any information related to the user through plugins or tools, include it in the story to make it more personal and engaging.",
    Name = "Story Agent",
    Description = "This agent can create funny stories based on user input.",
    Kernel = kernel,
    Arguments = new KernelArguments(new PromptExecutionSettings
    { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }) // 👈🏼 Set the function choice behavior to auto
    
};

var CityInformationTool =
    KernelPluginFactory.CreateFromFunctions(
        "CityInformation",
        "Get the residence city from the user",
        [KernelFunctionFactory.CreateFromMethod(GetCity)]);
var UserAgeTool =
    KernelPluginFactory.CreateFromFunctions(
        "UserAge",
        "Get the age of a user",
        [KernelFunctionFactory.CreateFromMethod(GetUserAge)]);
agent.Kernel.Plugins.Add(CityInformationTool);
agent.Kernel.Plugins.Add(UserAgeTool);


string userMessage = string.Empty;

do
{
    Console.WriteLine("What kind of story would you like me to tell you? Type 'exit' to stop the chat.");
    userMessage = Console.ReadLine();
    if (!string.IsNullOrEmpty(userMessage) && userMessage != "exit")
  {

        
        ChatHistory chat =
        [
            new ChatMessageContent(AuthorRole.User,
                                userMessage)
        ];

        await foreach (var response in agent.InvokeAsync(chat)) // 👈🏼 Invoke the agent with the chat history
        {
            // Process each response from the agent
            chat.Add(response);
            Console.WriteLine(response.Message.Content); // Display the agent's response
        }
        userMessage = string.Empty;
    }
   
}
while (userMessage != "exit");

[Description("Get the residence city from the user")]
string GetCity(string name)
{
    // This method simulates fetching the city based on the user's name
    Console.WriteLine($"GetCity called with: {name}");
    string city = string.Empty;
    switch (name.ToLower())
    {
        case "mario rossi":
            city = "Roma";
            break;
        case "maria bianchi":
            city = "Milano";
            break;
        case "mirko verdi":
            city = "Napoli";
            break;
        case "giovanni neri":
            city = "Bologna";
            break;
        default:
            city = "Unknown City"; // Return a default value if the name is not recognized
            break;
    }
    return city;
}
[Description("Get the age of a user")]
int GetUserAge(string name)
{
    
    Console.WriteLine($"GetUserAge called with: {name}");
    int years = 0;

    switch (name.ToLower())
    {
        case "mario rossi":
            years = 23;
            break;
        case "maria bianchi":
            years = 24;
            break;
        case "mirko verdi":
            years = 55;
            break;
        case "giovanni neri":
            years = 23;
            break;
        default:
            years = 0;
            break;
    }
    Console.WriteLine($"GetUserAge called with: {name} years: {years}");
    return years;
}
   