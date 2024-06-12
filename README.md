# BaklavajsEngineSharp

C# 运行 Baklavajs 数据

## 使用

### js保存数据

```ts
const text = JSON.stringify(baklava.editor.save())
```

### c# 读取数据并运行

RunOnce传入全局变量，

```csharp
EditorState state = JsonConvert.DeserializeObject<EditorState>(File.ReadAllText("./assets/app.json")) ;
//泛型为全局数据类型
DependencyEngine<string> engine = new DependencyEngine<string>(state);
//注册对应执行计算器，key 对应节点（node）的类型（type）
engine.AddCalculator("InputNode",new InputCalculator()); 
engine.AddCalculator("ReplaceTextNode",new ReplaceCalculator());
engine.AddCalculator("MatchTextNode",new MatchCalculator());
//运行并传入全局数据，返回结果
Dictionary<string,Dictionary<string,object>> res = await engine.RunOnce("测试 001 数据");
```
