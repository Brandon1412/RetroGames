using System.ComponentModel.DataAnnotations;

namespace RetroGames.WebApi.ApiModels.Autenticacao;

public class CadastrarAdministradorRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve possuir pelo menos 8 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}
