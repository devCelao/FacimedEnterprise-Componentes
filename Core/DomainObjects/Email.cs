using System.Text.RegularExpressions;

namespace Core.DomainObjects;

public class Email
{
    public const int EnderecoMaxLenght = 254;
    public const int EnderecoMinLenght = 5;

    public string? EnderecoEmail { get; private set; }

    //Construtor do EF
    protected Email() { }

    public Email(string endereco)
    {
        if (!Validar(endereco)) throw new DomainException("E-mail inválido");
        EnderecoEmail = endereco;
    }

    public static bool Validar(string email)
    {
        var regexEmail = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

        return regexEmail.IsMatch(email);
    }
}