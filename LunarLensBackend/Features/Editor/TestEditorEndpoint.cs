using FastEndpoints;

namespace LunarLensBackend.Features.Editor;

public class TestEditorEndpoint : Endpoint<EmptyRequest>
{
    public override void Configure()
    {
        Get("/editor/test");
        Policies("EditorOnly");
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        Console.WriteLine("Editor endpoint accessed successfully!");
        await SendOkAsync("Access granted");
    }
}