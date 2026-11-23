
namespace RetroGames.WebApi.ApiModels.Respostas;

public class ApiResponse<T>
{
    public int ResponseCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public Dictionary<string, string[]>? Errors { get; set; }

    public static ApiResponse<T> Sucesso(
        T? data,
        string message = "Operacao realizada com sucesso.")
    {
        return new ApiResponse<T>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Erro(
        int responseCode,
        string message)
    {
        return new ApiResponse<T>
        {
            ResponseCode = responseCode,
            Message = message
        };
    }
}
