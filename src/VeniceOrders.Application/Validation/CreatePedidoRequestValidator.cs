using FluentValidation;
using VeniceOrders.Application.DTOs;

namespace VeniceOrders.Application.Validation
{
    public sealed class CreatePedidoRequestValidator : AbstractValidator<CreatePedidoRequestDto>
    {
        public CreatePedidoRequestValidator()
        {
            RuleFor(x => x.ClienteId).NotEmpty();

            RuleFor(x => x.Itens)
                .NotEmpty()
                .WithMessage("O pedido deve conter ao menos um item.");

            RuleForEach(x => x.Itens).ChildRules(i =>
            {
                i.RuleFor(p => p.Produto).NotEmpty();
                i.RuleFor(p => p.Quantidade).GreaterThan(0);
                i.RuleFor(p => p.PrecoUnitario).GreaterThan(0);
            });
        }
    }
}


