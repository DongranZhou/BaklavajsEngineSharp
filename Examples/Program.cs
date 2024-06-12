// See https://aka.ms/new-console-template for more information
using Baklavajs;
using Examples;
using Newtonsoft.Json;


Console.WriteLine("Hello, World!");
EditorState state = JsonConvert.DeserializeObject<EditorState>(File.ReadAllText("./assets/app.json")) ;
DependencyEngine<string> engine = new DependencyEngine<string>(state);
engine.AddCalculator("InputNode",new InputCalculator());
engine.AddCalculator("ReplaceTextNode",new ReplaceCalculator());
engine.AddCalculator("MatchTextNode",new MatchCalculator());
Dictionary<string,Dictionary<string,object>> res = await engine.RunOnce("测试 001 数据");
Console.WriteLine(JsonConvert.SerializeObject(res));