using System.ComponentModel.DataAnnotations;

namespace RetroGames.WebApi.ApiModels.Autenticacao;

public class AlterarMinhaSenhaRequest
{
    [Required(ErrorMessage = "Informe sua senha atual.")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    [MinLength(8, ErrorMessage = "A nova senha deve possuir pelo menos 8 caracteres.")]
    public string NovaSenha { get; set; } = string.Empty;
}
