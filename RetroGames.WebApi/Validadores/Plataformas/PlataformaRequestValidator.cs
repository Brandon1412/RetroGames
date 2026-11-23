
using FluentValidation;
using RetroGames.WebApi.ApiModels.Plataformas;

namespace RetroGames.WebApi.Validadores.Plataformas;

public class PlataformaRequestValidator : AbstractValidator<PlataformaRequest>
{
    public PlataformaRequestValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .WithMessage("O nome da plataforma é obrigatório.")
            .MaximumLength(80)
            .WithMessage("O nome da plataforma deve ter no máximo 80 caracteres.");

        RuleFor(p => p.Descricao)
            .NotEmpty()
            .WithMessage("A descrição da plataforma é obrigatória.")
            .MaximumLength(255)
            .WithMessage("A descrição deve ter no máximo 255 caracteres.");
    }
}
