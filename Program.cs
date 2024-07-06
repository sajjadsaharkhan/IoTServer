using System.Net.Http.Headers;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/api/upload", async (HttpRequest request) =>
    {
        const string botToken = "6414123044:AAFSEf5tfb7TCVv4__wiPa7EWKnHb0xuOo4";
        const string chatId = "-1002239234726";

        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();
        var jsonDocument = JsonDocument.Parse(body);
        var base64Image = jsonDocument.RootElement.GetProperty("image").GetString();

        var imageBytes = Convert.FromBase64String(base64Image!);
        using var content = new MultipartFormDataContent();
        var fileStreamContent = new ByteArrayContent(imageBytes);
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(fileStreamContent, "photo", "image.jpg");

        var telegramApiUrl = $"https://api.telegram.org/bot{botToken}/sendPhoto?chat_id={chatId}";
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(telegramApiUrl, content);

        var responseBody = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? Results.Ok("Image sent to Telegram successfully.")
            : Results.StatusCode((int)response.StatusCode);
    }).WithName("Upload")
    .WithOpenApi();

app.Run();