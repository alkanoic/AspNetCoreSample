namespace T4Execute;

public partial class SampleGenerator
{
    public SampleGenerateArgs args { get; }

    // とりあえずコンストラクタで受け取る
    public SampleGenerator(SampleGenerateArgs args)
    {
        this.args = args;
    }
}

public class SampleGenerateArgs
{
    public required string AssemblyPath { get; set; }
}
