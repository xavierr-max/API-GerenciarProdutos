using System.Net;
using System.Net.Mail;

namespace GerenciarProdutos.Services;

public class EmailService
{
    public bool Send(
        string toName, //nome do cliente
        string toEmail, //email do cliente
        string subject, //assunto do email
        string body, //corpo do email
        string fromName = "Equipe GerenciarTarefas", //nome do servico
        string fromEmail = "email@gerenciartarefas") //nome do email do servico
    {
        var smtpClient = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port); //identifica o host e porta do servico de email a enviar
        
        smtpClient.Credentials = new NetworkCredential(Configuration.Smtp.Username, Configuration.Smtp.Password); //usa a api do sendgrid como um "perfil de um servico de gmail" para poder chegar ao cliente
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; //maneira que vai enviar o email
        smtpClient.EnableSsl = true; //habilita um metodo de envio seguro pela porta 587
        var mail = new MailMessage(); //instancia um objeto estruturado afim de enviar o email, recebendo como parametro o Send
        
        mail.From = new MailAddress(fromEmail, fromName);
        mail.To.Add(new MailAddress(toEmail, toName));
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true; //permite html no corpo do email

        try //tenta enviar o email
        {
            smtpClient.Send(mail);
            return true;
        }
        catch (Exception e) //O CODIGO ESTA CAINDO NO CATCH POR NAO POSSUIR O DOMINIO NO SENDGRID
        {
            return false;
        }
    }
}