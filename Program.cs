using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Dynacoop
{
    class Program
    {
        static void Main(string[] args)
        {
            CriarConta();
            Console.ReadKey();
        }
        static void CriarConta()
        {
            IOrganizationService service = Connection.Obter();

            Entity conta = new Entity("account");

            CriaLinha();
            Console.WriteLine("CRIAÇÃO DE CONTAS");
            CriaLinha(false);
            Console.WriteLine("Por favor informe o nome da conta:");
            conta["name"] = Console.ReadLine();

            CriaLinha();
            conta["telephone1"] = ValidaTelefoneCliente();

            CriaLinha();
            conta["cr27f_portedeempresa"] = new OptionSetValue(OpcoesPorteEmpresa()); 

            CriaLinha();
            conta["cr27f_totaldeoportunidades"] = new Money(Convert.ToInt32(ValidacaoNumeroTotal()));

            CriaLinha();
            conta["cr27f_totaldeprodutos"] = new Decimal(Convert.ToDouble(ValidacaoNumeroTotal("produtos")));

            CriaLinha();
            conta["cr27f_clienterelacionado"] = new EntityReference("lead", InsereNomeCliente(service)); 

            CriaLinha();
            Guid idContato = OpcoesCriarContato(service);
            if (idContato != Guid.Empty)
            {
                Console.WriteLine("Contato criado");
                CriaLinha(false);
                conta["primarycontactid"] = new EntityReference("contact", idContato);
            }
            service.Create(conta);

            Console.WriteLine("Conta criada");
        }
        static Guid InsereNomeCliente(IOrganizationService service)
        {
            Guid clienteId = Guid.Empty;
            Console.WriteLine("Por favor informe o tópico do cliente potencial relacionado:");
            var nomeClienteRelacionado = Console.ReadLine();
            var entidadeLead = RequisicaoLead(service, nomeClienteRelacionado);
            if (entidadeLead == null)
            {
                CriaLinha();
                Console.WriteLine("Cliente potencial não encontrado, tente novamente");
                CriaLinha(false);
                clienteId = InsereNomeCliente(service);
            }
            else
            {
                clienteId = entidadeLead.Id;
            }
            return clienteId;
        }
        static string ValidaTelefoneCliente()
        {
            Console.WriteLine("Por favor informe o telefone:");
            var telefoneCliente = Console.ReadLine();
            if (!Int32.TryParse(telefoneCliente, out int n))
            {
                CriaLinha();
                Console.WriteLine("Numero invalido, tente novamente");
                CriaLinha(false);
                telefoneCliente = ValidaTelefoneCliente();
            }
            return telefoneCliente;
        }
        static Guid OpcoesCriarContato(IOrganizationService service)
        {
            Console.WriteLine("Você deseja criar um contato para essa conta? (S/N)");
            string escolha = Console.ReadLine();
            Guid idContato = Guid.Empty;
            switch (escolha.ToUpper())
            {
                case "S":
                    idContato = CriarContato(service);
                    break;
                case "N":
                    CriaLinha();
                    Console.WriteLine("Obrigado por usar nossos serviços");
                    break;
                default:
                    CriaLinha();
                    Console.WriteLine("Responda com S/N.");
                    CriaLinha(false);
                    idContato = OpcoesCriarContato(service);
                    break;
            }
            return idContato;
        }
        static Guid CriarContato(IOrganizationService service)
        {
            Entity contato = new Entity("contact");

            CriaLinha();
            Console.WriteLine("Por favor informe o primeiro nome do contato:");
            contato["firstname"] = Console.ReadLine();

            CriaLinha();
            Console.WriteLine("Por favor informe o sobrenome do contato:");
            contato["lastname"] = Console.ReadLine();

            CriaLinha();
            Console.WriteLine("Por favor informe o email do contato:");
            contato["emailaddress1"] = Console.ReadLine();

            return service.Create(contato);
        }
        static string ValidacaoNumeroTotal(string campo = "oportunidades")
        {
            Console.WriteLine($"Por favor informe o total de {campo}:"); //int
            string resposta = Console.ReadLine();
            bool validacao = true;
            if (campo == "oportunidades")
            {
                validacao = !Int32.TryParse(resposta, out int n);
            }
            else
            {
                resposta = resposta.Replace('.', ',');
                validacao = !Decimal.TryParse(resposta, out decimal n);
            }
            if (validacao)
            {
                CriaLinha();
                Console.WriteLine("Por favor informe um numero válido:");
                CriaLinha(false);
                resposta = ValidacaoNumeroTotal(campo);
            }
            return resposta;
        }
        static int OpcoesPorteEmpresa()
        {
            Console.WriteLine("Por favor informe o porte da empresa:\n" +
            "-----------------------------\n" +
            "(1) Pequena\n" +
            "(2) Media\n" +
            "(3) Grande\n" +
            "-----------------------------");
            string escolha = Console.ReadLine();
            return Acao(escolha);
        }
        static Entity RequisicaoLead(IOrganizationService service, string nomeLead)
        {
            var fetchXml = $@"
            <fetch top=""1"">
            <entity name=""lead"">
                <attribute name=""leadid"" />
                <filter>
                <condition attribute=""subject"" operator=""eq"" value=""{nomeLead}"" />
                </filter>
            </entity>
            </fetch>";
            return service.RetrieveMultiple(new FetchExpression(fetchXml)).Entities.FirstOrDefault();
        }
        static int Acao(string escolha)
        {
            CriaLinha();
            int porteEmpresaConta = 0;
            switch (escolha.ToUpper())
            {
                case "1":
                case "PEQUENA":
                    porteEmpresaConta = 446060000;
                    break;
                case "2":
                case "MEDIA":
                    porteEmpresaConta = 446060001;
                    break;
                case "3":
                case "GRANDE":
                    porteEmpresaConta = 446060002;
                    break;
                default:
                    CriaLinha();
                    Console.WriteLine("Escolha uma opção valida.");
                    CriaLinha(false);
                    porteEmpresaConta = OpcoesPorteEmpresa();
                    break;
            }
            return porteEmpresaConta;
        }
        static void CriaLinha(bool valida = true)
        {
            if (valida)
            {
                Console.Clear();
                Console.WriteLine("-----------------------------");
            }
            else
            {
                Console.WriteLine("-----------------------------");
            }

        }
    }
}
