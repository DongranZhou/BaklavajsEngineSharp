using System;
using Baklavajs;
using OpenAI;
using Newtonsoft.Json;
using System.Linq;

namespace Examples.Calculator;

public class ZhipuCalculator : ICalculator
{
    public async Task<Dictionary<string, object>> Calculate(Dictionary<string, object> inputs, NodeState state, EngineContext context)
    {
        ChatAPI api = new ChatAPI(inputs["baseURL"] as string,inputs["apiKey"] as string);
        var messages = GetMessages(inputs);
        var res = await api.Chat(new ChatRequest<ChatMessage>
        {
            model = inputs["model"] as string,
            messages = messages
        });
        return new Dictionary<string, object>() { { "message", res.choices[0].message.content } };
    }

    List<ChatMessage> GetMessages(Dictionary<string, object> inputs)
    {
        List<ChatMessage> messages = new List<ChatMessage>();
        if (inputs.ContainsKey("history") && !string.IsNullOrEmpty(inputs["history"] as string))
            messages = JsonConvert.DeserializeObject<List<ChatMessage>>(inputs["history"] as string);
        int sysIndex = messages.FindIndex(x => x.role == "system");
        if (sysIndex < 0)
            messages.Insert(0, new ChatMessage { role = "system", content = inputs["system"] as string });
        else
            messages[sysIndex].content = inputs["system"] as string;
        messages.Add(new ChatMessage{ role = "user" , content = inputs["message"] as string});
        return messages;
    }
}
