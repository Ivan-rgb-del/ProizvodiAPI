using FluentValidation;
using ProizvodiAPI.Application.DTOs;

namespace ProizvodiAPI.Application.Validators
{
    public class KreirajProizvodValidator : AbstractValidator<KreirajProizvodDTO>
    {
        public KreirajProizvodValidator()
        {
            RuleFor(x => x.Naziv)
                .NotEmpty().WithMessage("Naziv ne može biti prazan!")
                .MinimumLength(2).WithMessage("Naziv mora imati najmanje 2 karaktera!")
                .MaximumLength(100).WithMessage("Naziv ne može imati više od 100 karaktera!");

            RuleFor(x => x.Opis)
                .NotEmpty().WithMessage("Opis ne može biti prazan!")
                .MaximumLength(500).WithMessage("Opis ne može imati više od 500 karaktera!");

            RuleFor(x => x.Cena)
                .GreaterThan(0).WithMessage("Cena mora biti veća od 0!");

            RuleFor(x => x.Kolicina)
                .GreaterThanOrEqualTo(0).WithMessage("Količina ne može biti negativna!");

            RuleFor(x => x.Kategorija)
                .NotEmpty().WithMessage("Kategorija ne može biti prazna!")
                .Must(k => k == "Elektronika" || k == "Nameštaj" || k == "Odeća")
                .WithMessage("Kategorija mora biti: Elektronika, Nameštaj ili Odeća!");
        }
    }
}
