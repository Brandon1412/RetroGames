using FluentValidation;
using RetroGames.WebApi.ApiModels.Jogos;

namespace RetroGames.WebApi.Validadores.Jogos;

public class JogoRequestValidator : AbstractValidator<JogoRequest>
{
    public JogoRequestValidator()
    {
        RuleFor(j => j.Nome)
            .NotEmpty().WithMessage("O nome do jogo e obrigatorio.")
            .MaximumLength(150);

        RuleFor(j => j.Descricao)
            .NotEmpty().WithMessage("A descricao do jogo e obrigatoria.")
            .MaximumLength(1000);

        RuleFor(j => j.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("Informe um ano de lancamento valido.");

        RuleFor(j => j.Preco)
            .GreaterThan(0)
            .WithMessage("O preco deve ser maior que zero.");

        RuleFor(j => j.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque nao pode ser negativo.");

        RuleFor(j => j.IdPlataforma)
            .GreaterThan(0);

        RuleFor(j => j.IdCondicaoJogo)
            .GreaterThan(0);

        RuleFor(j => j.IdsGeneros)
            .NotEmpty()
            .WithMessage("Informe pelo menos um genero.");

        RuleForEach(j => j.IdsGeneros)
            .GreaterThan(0);
    }
}
