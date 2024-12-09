using FastEndpoints;

namespace LunarLensBackend.Features.Writer;

public class TestWriterEndpoint : Endpoint<EmptyRequest>
{
    public override void Configure()
    {
        Get("/writer/test");
        Policies("WriterOnly");
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        Console.WriteLine("Writer endpoint accessed successfully!");
        await SendOkAsync("Access granted");
    }
}