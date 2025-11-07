using Mailing.Moduled.Contracts;
using Mailing.Moduled.Sources;

namespace Mailing.Tests;

public sealed class MailingSendingTests
{
    private readonly TestEmailSenderSource _emailSendersSource = new TestEmailSenderSource()
        .Add("rexbur221282@yandex.ru", "hufqekwcsmlovddr")
        .Add("rexbur221282@mail.ru", "HRXti5SPV2puqgKQsAiO")
        .Add("jimkrauz@gmail.com", "gwmi vamd wvku fbtd");

    [Fact]
    private async Task Send_Google_Email()
    {
        string destination = "jimkrauz@gmail.com";
        string senderName = "yandex.ru";
        IEmailSender sender = await _emailSendersSource.Get(senderName, CancellationToken.None);
        await sender
            .FormEmailMessage()
            .Send(destination, "Test Sending Subject", "Test Sending Body");
    }

    [Fact]
    private async Task Send_Google_MailRu()
    {
        string destination = "jimkrauz@gmail.com";
        string senderName = "mail.ru";
        IEmailSender sender = await _emailSendersSource.Get(senderName, CancellationToken.None);
        await sender
            .FormEmailMessage()
            .Send(destination, "Test Sending Subject", "Test Sending Body");
    }

    [Fact]
    private async Task Send_Google_GoogleCom()
    {
        string destination = "rexbur221282@yandex.ru";
        string senderName = "google.com";
        IEmailSender sender = await _emailSendersSource.Get(senderName, CancellationToken.None);
        await sender
            .FormEmailMessage()
            .Send(destination, "Test Sending Subject", "Test Sending Body");
    }
}