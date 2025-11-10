using System.Security.Cryptography;
using System.Text;

namespace Mailers.Tests;

public sealed class CreateMailerTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    [Fact]
    private async Task Create_Mailer_Success()
    {
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("mail@email.com", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.True(mailer.IsSuccess);

        var insertMailer = scope.ServiceProvider
            .GetRequiredService<IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>>>();
        var insertArgument = new InsertMailerFunctionArgument(session, mailer);
        var insertion = await insertMailer.Invoke(insertArgument, CancellationToken.None);
        Assert.True(insertion.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Duplicate_Email_Failure()
    {
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("mail@email.com", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.True(mailer.IsSuccess);

        var insertMailer = scope.ServiceProvider
            .GetRequiredService<IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>>>();
        var insertArgument = new InsertMailerFunctionArgument(session, mailer);
        await insertMailer.Invoke(insertArgument, CancellationToken.None);
        var insertion = await insertMailer.Invoke(insertArgument, CancellationToken.None);
        Assert.True(insertion.IsFailure);
    }

    [Fact]
    private async Task Register_Mailer_In_Application_Success()
    {
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var registration = scope.ServiceProvider
            .GetRequiredService<IAsyncFunction<RegisterMailerInApplicationFunctionArgument, Result<Mailer>>>();
        var input = new RegisterMailerInApplicationFunctionArgument("mail@email.com", "sdssad-dsadsadsa-dsadsasda",
            session);
        var registered = await registration.Invoke(input, CancellationToken.None);
        Assert.True(registered.IsSuccess);
    }

    [Fact]
    private void Create_Invalid_Email()
    {
        string email = "invalid-email";
        var scope = services.Scope();
        var createEmail = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateEmailFunctionArgument, Result<Email>>>();
        var result = createEmail.Invoke(new CreateEmailFunctionArgument(email));
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Create_Mailer_Invalid_Email()
    {
        await using var scope = services.Scope();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("invalid-email", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.False(mailer.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Empty_Email()
    {
        await using var scope = services.Scope();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.False(mailer.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Empty_SmtpPassword()
    {
        await using var scope = services.Scope();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("test@email.com", "");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.False(mailer.IsSuccess);
    }

    [Fact]
    private async Task Encrypt_String_Tests()
    {
        AesEncryption encryption = new("1234567890123456");
        string plainText = "Hello world string.";
        string encryptedText = await encryption.EncryptAsync(plainText);
        Assert.NotEqual(plainText, encryptedText);
    }

    [Fact]
    private async Task Decrypt_String_Tests()
    {
        AesEncryption encryption = new("1234567890123456");
        string plainText = "Hello world string.";
        string encryptedText = await encryption.EncryptAsync(plainText);
        string decryptedText = await encryption.DecryptAsync(encryptedText);
        Assert.Equal(plainText, decryptedText);
    }

    [Fact]
    private async Task Decrypt_String_Tests_Failure()
    {
        AesEncryption encryption = new("1234567890123456");
        string plainText = "Hello world string.";
        string encryptedFake = "Not a hello world string.";
        string decryptedText = await encryption.DecryptAsync(encryptedFake);
        Assert.NotEqual(plainText, decryptedText);
    }

    [Fact]
    private async Task Decrypt_String_Tests_Invalid_Format_Failure()
    {
        AesEncryption encryption = new("1234567890123456");
        string plainText = "Hello world string.";
        string encryptedFake = Guid.NewGuid().ToString();
        string decryptedText = await encryption.DecryptAsync(encryptedFake);
        Assert.NotEqual(plainText, decryptedText);
    }

    public sealed class AesEncryptionEngine(string keyText)
    {
        private readonly Lazy<byte[]> _keyBytes = new(() => Encoding.UTF8.GetBytes(keyText));
        private readonly Lazy<byte[]> _ivBytes = new(() => Encoding.UTF8.GetBytes(keyText));
        private byte[] Key => _keyBytes.Value;
        private byte[] IV => _ivBytes.Value;

        public AesScope ProvideDecryptor()
        {
            return new AesDecryptionScope(Key, IV);
        }

        public AesScope ProvideEncryptor()
        {
            return new AesEncryptionScope(Key, IV);
        }
    }

    public abstract class AesScope : IDisposable
    {
        protected readonly Aes Aes;

        public AesScope(byte[] keyBytes, byte[] ivBytes)
        {
            Aes = Aes.Create();
            Aes.IV = ivBytes;
            Aes.Key = keyBytes;
        }

        public abstract AesStream ProvideStream();

        public virtual void Dispose()
        {
            Aes.Dispose();
        }
    }

    public sealed class DisposableResources
    {
        private readonly Dictionary<Type, (IDisposable, bool)> _disposables = [];

        public DisposableResources Add(IDisposable disposable)
        {
            const bool isDisposed = false;
            Type type = disposable.GetType();
            _disposables.Add(type, (disposable, isDisposed));
            return this;
        }

        public DisposableResources DisposeItem(IDisposable disposable)
        {
            Type type = disposable.GetType();
            if (!_disposables.TryGetValue(type, out (IDisposable, bool) value) || value.Item2)
                return this;
            value.Item1.Dispose();
            _disposables.Remove(type);
            return this;
        }

        public DisposableResources Disposed()
        {
            foreach ((IDisposable, bool) disposable in _disposables.Values)
                DisposeItem(disposable.Item1);
            return new DisposableResources();
        }
    }

    public abstract class AesStream : IDisposable
    {
        protected readonly ICryptoTransform _transform;

        public AesStream(ICryptoTransform transform)
        {
            _transform = transform;
        }

        public virtual void Dispose()
        {
            _transform.Dispose();
        }

        public abstract Task<string> WorkWithAsync(string plainText);
        public abstract string WorkWith(string plainText);
    }

    public sealed class AesDecryptionStream : AesStream
    {
        private readonly DisposableResources _disposables = new();
        private readonly MemoryStream _ms;
        private readonly CryptoStream _cs;
        private readonly StreamReader _sr;

        public AesDecryptionStream(ICryptoTransform transform) : base(transform)
        {
            _ms = new MemoryStream();
            _cs = new CryptoStream(_ms, transform, CryptoStreamMode.Read);
            _sr = new StreamReader(_cs);
            _disposables.Add(_sr).Add(_cs).Add(_ms);
        }

        public AesDecryptionStream(AesDecryptionStream stream, MemoryStream ms) : base(stream._transform)
        {
            _disposables = stream._disposables.Disposed();
            _ms = ms;
            _cs = new CryptoStream(_ms, stream._transform, CryptoStreamMode.Read);
            _sr = new StreamReader(_cs);
            _disposables.Add(_sr).Add(_cs).Add(_ms);
        }

        public override async Task<string> WorkWithAsync(string plainText)
        {
            byte[] textBytes = Convert.FromBase64String(plainText);
            using AesDecryptionStream stream = new(this, new MemoryStream(textBytes));
            return await stream._sr.ReadToEndAsync();
        }

        public override string WorkWith(string plainText)
        {
            byte[] textBytes = Convert.FromBase64String(plainText);
            using AesDecryptionStream stream = new(this, new MemoryStream(textBytes));
            return stream._sr.ReadToEnd();
        }

        public override void Dispose()
        {
            _ms.Dispose();
            _cs.Dispose();
            _sr.Dispose();
            base.Dispose();
        }
    }

    public sealed class AesEncryptionStream : AesStream
    {
        private readonly DisposableResources _disposables = new();
        private readonly MemoryStream _ms;
        private readonly StreamWriter _sw;
        private readonly CryptoStream _cs;

        public AesEncryptionStream(ICryptoTransform transform) : base(transform)
        {
            _ms = new MemoryStream();
            _cs = new CryptoStream(_ms, transform, CryptoStreamMode.Write);
            _sw = new StreamWriter(_cs);
            _disposables = _disposables.Add(_sw).Add(_cs).Add(_ms);
        }

        public override async Task<string> WorkWithAsync(string plainText)
        {
            await _sw.WriteAsync(plainText);
            _sw.Close();
            return Convert.ToBase64String(_ms.ToArray());
        }

        public override string WorkWith(string plainText)
        {
            _sw.Write(plainText);
            _sw.Close();
            return Convert.ToBase64String(_ms.ToArray());
        }

        public override void Dispose()
        {
            _disposables.Disposed();
            base.Dispose();
        }
    }

    public sealed class AesDecryptionScope : AesScope
    {
        private readonly ICryptoTransform _decryptor;

        public AesDecryptionScope(byte[] keyBytes, byte[] ivBytes) : base(keyBytes, ivBytes)
        {
            _decryptor = Aes.CreateDecryptor();
        }

        public override AesStream ProvideStream()
        {
            return new AesDecryptionStream(_decryptor);
        }

        public override void Dispose()
        {
            base.Dispose();
            _decryptor.Dispose();
        }
    }

    public sealed class AesEncryptionScope : AesScope
    {
        private readonly ICryptoTransform _encryptor;

        public AesEncryptionScope(byte[] keyBytes, byte[] ivBytes) : base(keyBytes, ivBytes)
        {
            _encryptor = Aes.CreateEncryptor(keyBytes, ivBytes);
        }

        public override AesStream ProvideStream()
        {
            return new AesEncryptionStream(_encryptor);
        }

        public override void Dispose()
        {
            base.Dispose();
            _encryptor.Dispose();
        }
    }

    public sealed class AesEncryption(string keyText)
    {
        private readonly AesEncryptionEngine _engine = new(keyText);

        public async Task<string> EncryptAsync(string plainText)
        {
            using AesScope scope = _engine.ProvideEncryptor();
            using AesStream stream = scope.ProvideStream();
            return await stream.WorkWithAsync(plainText);
        }

        public async Task<string> DecryptAsync(string encryptedText)
        {
            try
            {
                using AesScope scope = _engine.ProvideDecryptor();
                using AesStream stream = scope.ProvideStream();
                return await stream.WorkWithAsync(encryptedText);
            }
            catch (FormatException format)
            {
                return string.Empty;
            }
        }
    }
}