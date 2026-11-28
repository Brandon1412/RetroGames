using FluentValidation;
using RetroGames.WebApi.ApiModels.Plataformas;

namespace RetroGames.WebApi.Validadores.Plataformas;

public class PlataformaRequestValidator : AbstractValidator<PlataformaRequest>
{
    public PlataformaRequestValidator()
    {
        string[] categoriasPermitidas =
        [
            "Console de mesa",
            "Console portátil",
            "Arcade",
            "Híbrido",
            "Outro"
        ];

        RuleFor(p => p.Nome)
            .NotEmpty()
            .WithMessage("O nome da plataforma é obrigatório.")
            .MaximumLength(80)
            .WithMessage("O nome da plataforma deve ter no máximo 80 caracteres.");

        RuleFor(p => p.Fabricante)
            .MaximumLength(80)
            .WithMessage("O fabricante deve ter no máximo 80 caracteres.");

        RuleFor(p => p.AnoLancamento)
            .Must(ano =>
                !ano.HasValue ||
                (ano.Value >= 1972 && ano.Value <= DateTime.Now.Year))
            .WithMessage(
                $"O ano de lançamento deve estar entre 1972 e {DateTime.Now.Year}.");

        RuleFor(p => p.Categoria)
            .Must(categoria =>
                string.IsNullOrWhiteSpace(categoria) ||
                Array.Exists(categoriasPermitidas, item => item == categoria))
            .WithMessage("Selecione uma categoria válida.");

        RuleFor(p => p.Descricao)
            .MaximumLength(255)
            .WithMessage("As outras informações devem ter no máximo 255 caracteres.");
    }
}
